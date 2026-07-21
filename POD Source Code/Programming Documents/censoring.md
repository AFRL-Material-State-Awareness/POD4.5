# End-to-End Censoring Process

This document explains how censored data is defined, processed, and analyzed in the Â-vs-a analysis workflow, from the C# backend to the R analysis script.

## Overview

The censoring status of each data point is determined in the C# code before the analysis is sent to R. An `event` flag is created for each row to mark it as uncensored, left-censored, or right-censored. This flag is then used in the R script to perform a survival regression analysis, which correctly accounts for censored data when fitting the statistical model.

A critical aspect of this process is that the censoring status for a given data point (or "crack") is determined based on the value of **only one** of its associated response columns, even when multiple responses (e.g., from multiple inspectors) are included in the analysis.

It is also important to note that the C# methods responsible for this process (`UpdateCensoredData` in `Analysis.cs` and `createDataFrameinGlobalEnvr` in `AHatAnalysisRControl.cs`) are **tightly coupled**. The first method prepares the data structures that the second method consumes to build the data frame for R. Any modifications to the logic in one file will likely require corresponding changes in the other to ensure the analysis runs correctly.

---

## 1. Censoring Setup in C#

The setup process occurs in the C# backend, which prepares the data and creates an `event` vector in the R environment before the main analysis is called.

### Key Files:
-   `./Analysis/Analysis.cs`
-   `./CSharpBackendTempSolutionForPOD4Point5/CSharpBackendWithR/AHatAnalysisRControl.cs`
-   `./Data/AnalysisData.cs`

### Process Steps:

**1. Populating the Censored Lists**

Before the `event` vector can be created, the C# code first determines which specific response values are considered censored. This happens in the `UpdateCensoredData` method within `Analysis.cs`.

This method is called just before an analysis runs. It iterates through all response values for each activated response column and compares them to the user-defined censor levels (`InResponseMin` and `InResponseMax`).

-   If a response value is less than `InResponseMin`, it's added to a temporary `censoredResponsesL` list.
-   If a response value is greater than `InResponseMax`, it's added to a temporary `censoredResponsesR` list.

Finally, these temporary lists are used to populate the `ResponsesCensoredLeft` and `ResponsesCensoredRight` dictionaries on the `AHatAnalysisObject`. These are the dictionaries that the `createDataFrameinGlobalEnvr` method uses to determine the `event` status for each row.

**2. Determining the `event` Status**

The logic is in the `createDataFrameinGlobalEnvr` method within `AHatAnalysisRControl.cs`. This method iterates through each data row and checks a single, primary response value against the `ResponsesCensoredLeft` and `ResponsesCensoredRight` lists populated in the previous step. It then builds an `event` vector in R, where each element corresponds to a row:
-   `1`: Uncensored (the value is between the left and right censor levels).
-   `0`: Right-Censored (the value is in the `ResponsesCensoredRight` list).
-   `2`: Left-Censored (the value is in the `ResponsesCensoredLeft` list).

**3. Primary Response Column Selection**

The choice of which response column to use for this check is crucial. The code uses the column name stored in the `SignalResponseName` property of the `AHatAnalysisObject`.

This property is set in the `UpdateData` method within `AnalysisData.cs`. The code iterates through all "activated" response columns and assigns the last one in the list to `SignalResponseName`. **Therefore, the censoring status for all rows is determined exclusively by the values in the last response column selected by the user.**

**4. DataFrame Assembly**

After the `event` vector is built in R, the `BuildDataFrame` method (in `AHatAnalysisRControl.cs`) assembles the final `AHatDFTest` data frame. This data frame, which includes the `event` column, is then passed to the main R analysis script.

---

## 2. Censoring Usage in R

The R script receives the data frame with the `event` column already defined and uses it to perform the appropriate regression analysis.

### Key Files:
-   `./RCode/RBackend/SignalResponseCode/SignalResponseMainAnalysisRObject.R`
-   `./RCode/RBackend/SignalResponseCode/PrepareDataWithMultipleResponsesRObject.R`

### Process Steps:

1.  **Averaging Response Values**
    If multiple response columns (`y0`, `y1`, etc.) are present, the first step in the R script is to average them. The `createAvgRespDF` function in `PrepareDataWithMultipleResponsesRObject.R` calculates the `rowMeans` of all response columns.

    This averaging process **does not use the `event` column**. The `event` column is explicitly excluded from the averaging calculation, and the original `event` value for each row is simply carried over and attached to the new averaged data.

2.  **Survival Regression**
    The core of the censoring analysis happens in the `genAhatVersusACensored` method within `SignalResponseMainAnalysisRObject.R`.
    -   A `Surv` object, which is the standard way to represent censored data in R, is created:
        ```R
        censored.a.hat <- Surv(time = signalRespDF$y.trans, time2 = signalRespDF$event, event=signalRespDF$event, type="interval")
        ```
        This tells R that the `y.trans` value should be interpreted according to the corresponding `event` flag (e.g., as a value known only to be "greater than X" if right-censored).
    -   This `Surv` object is then used as the response variable in the `survreg` (survival regression) function:
        ```R
        a.hat.vs.a.censored <- survreg(formula = censored.a.hat ~ x, dist = "gaussian", data = a.hat.censor.df)
        ```
    The `survreg` function fits the linear model while correctly accounting for the nature of the censored data points, producing a more accurate model fit than if the censored points were simply ignored or treated as actual values. All subsequent calculations, including the POD curve and residuals, are derived from this robust model.

---

## 3. Known Issue: Data Overwriting with Multiple Responses

A significant issue exists in the `UpdateCensoredData` method in `Analysis.cs` when an analysis includes multiple response columns (e.g., from multiple inspectors).

### The Problem
The `AHATAnalysisObject.Flaws` and `AHATAnalysisObject.FlawsCensored` properties are simple lists, not dictionaries keyed by response name. However, the code that populates them runs inside a loop that iterates through each response column. As a result, the content of these two lists is **overwritten in each iteration**, and their final state only reflects the data from the **last response column** processed.

### The Consequences
This has two major effects on the analysis:

1.  **Incorrect Flaw List for R**: The `AHATAnalysisObject.Flaws` list is used to build the `cracks` vector that is sent to the R engine. Because it only contains data from the last response column, the entire statistical analysis is performed using a set of flaws that was filtered based on the criteria of only the last inspector.

2.  **Incorrect Censored Flaw List**: The `AHATAnalysisObject.FlawsCensored` list is used to generate censored residual tables and to determine if the analysis has enough data to run. Because this list is also based only on the last inspector, these tables and checks can be incorrect.

While the `UpdateCensoredData` method has been refactored for efficiency, this specific behavior has been preserved to maintain consistent output until the correct statistical approach for handling multiple response columns is determined.
