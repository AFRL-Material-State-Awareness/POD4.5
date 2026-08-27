<img src="Images/POD website logo.png" alt="POD Logo" width="300" />

# Probability of Detection Professional version 4.5        

  POD Pro v4.5 (Probability of Detection Professional version 4.5) is the new and improved version of PODv4 which incorporates advanced statistics and an r backend. There have also been bug fixes related to the original PODv4.

### License

  Unlike PODv4, POD Pro v4.5 is open-source-software licensed under the GNU General Public License version 3 or later (**GPLv3+**). This means that anyone has the right to change, copy, modify, use, or study all of the code within POD Pro v4.5 under the terms of GPLv3. For full license text, see [https://www.gnu.org/licenses/gpl-3.0.en.html].

### Installation Instructions

  If you wish to download POD Pro v4.5 for your computer, please click on the tag with the latest release (current latest release is POD Pro v4.5.0 beta as of January 2023). From there, simply download the installer (.msi). Once download, open the installer, and the wizard will guide you through the process. A copy of R-4.1.2 with the necessary libraries is included in the installation, so the application is ready to run upon completing the installation wizard. A shortcut for POD Pro v4.5 will be added to the desktop. If you cannot find the .exe, simply type POD Pro v4.5 in the search bar and the application should appear. 
  
### Running the Development version in Visual Studio

  POD Pro v4.5 was written in Visual Studio 2019. Clone this GitHub repository and open `Winforms POD UI.sln` (in the `POD Source Code` folder). Set the startup project to **FORMS**, select the **Debug | Any CPU** configuration, and run the solution (F5). The application builds and runs as **64-bit**, which is required — some statistical analyses exceed the ~2&nbsp;GB memory limit of a 32-bit process and crash — so do not use the **x86** (32-bit) configuration. The Windows Forms designer works normally in this configuration.

### Building without the Visual Studio IDE (Build Tools for Visual Studio 2019)

  You do not need the full Visual Studio IDE to compile POD Pro v4.5 — the free **Build Tools for Visual Studio 2019** can build it from the command line. This is the same MSBuild toolchain the project was written with, so the build targets **.NET Framework 4.6.1** and stays within the **C# 8** language level (Visual Studio 2019's ceiling). Please do not introduce C# 9 or newer language features.

#### Prerequisites

* **Build Tools for Visual Studio 2019** — free, from `https://aka.ms/vs/16/release/vs_buildtools.exe`. In the installer, select the **".NET desktop build tools"** workload, which provides MSBuild, the C# compiler, and the NuGet build targets.
* **.NET Framework 4.6.1 targeting pack.** The workload above normally installs it. If a build reports `error MSB3644: The reference assemblies for .NETFramework,Version=v4.6.1 were not found`, install the **.NET Framework 4.6.1 Developer Pack** from `https://dotnet.microsoft.com/download/dotnet-framework/net461` (or tick *".NET Framework 4.6.1 targeting pack"* under **Individual components** in the Visual Studio Installer).
* **`nuget.exe`** — the standalone NuGet CLI, used to restore this solution's packages (the projects use the older `packages.config` format). See *Installing `nuget.exe`* below.

#### Installing `nuget.exe`

  `nuget.exe` is a single self-contained executable, not an installer.

1. Download the latest `nuget.exe` from the NuGet downloads page — [https://www.nuget.org/downloads](https://www.nuget.org/downloads) (under **Windows x86 Commandline**, choose *latest recommended*) — or directly from [https://dist.nuget.org/win-x86-commandline/latest/nuget.exe](https://dist.nuget.org/win-x86-commandline/latest/nuget.exe).
2. Save it to a folder you will keep, for example `C:\Tools`.
3. Add that folder to your user `PATH` so you can run `nuget` from any terminal:
   * **Windows Settings:** press Start, search *"Edit environment variables for your account"*, select **Path** under *User variables*, click **Edit → New**, add the folder (e.g. `C:\Tools`), then **OK**.
   * **PowerShell** (persistent, current user):

     ```powershell
     [Environment]::SetEnvironmentVariable("Path", [Environment]::GetEnvironmentVariable("Path","User") + ";C:\Tools", "User")
     ```
4. Open a **new** terminal so the updated `PATH` takes effect, then verify with `nuget help` (it should print NuGet's help text).

#### Steps

  Run these from the `POD Source Code` folder (the one containing `Winforms POD UI.sln`):

1. Restore the NuGet packages:

   ```
   nuget restore "Winforms POD UI.sln"
   ```

2. Build the solution with the Build Tools 2019 MSBuild:

   ```
   "C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\amd64\MSBuild.exe" "Winforms POD UI.sln" /p:Configuration=Debug /m
   ```

  The whole application then compiles. (Tip: from a **"Developer Command Prompt for VS 2019"** — installed alongside the Build Tools — `MSBuild` is already on the `PATH`, so you can just run `MSBuild "Winforms POD UI.sln" /p:Configuration=Debug /m`.)

#### Notes

* **The three unit-test projects (`Data.UnitTests`, `Controls.UnitTests`, `Analyze.UnitTests`) are out of date and are not expected to compile.** They are not part of the application — a successful application build reports errors only in those projects.
* **Startup / bitness:** same as the Visual Studio section above — startup project **FORMS**, build **Debug | Any CPU** (64-bit); do not use the **x86** (32-bit) build.

### New Features in POD Pro v4.5.0

  POD Pro v4.5.0 is the first release of POD Pro v4.5. It offers a wide range of features not previously included in PODv4. In addition, the backend analysis for POD Pro v4.5 is written in the statistical programming langauge R in order to more easily accomadate the new statistical methods for POD through the use of CRAN libraries. The features specific to POD Pro v4.5.0 are listed below:

###### Hit/Miss Analysis

There have been many features and drastic improvements in hit/miss analysis for POD Pro v4.5.0:

* **Plotting Confidence Interval Curve**: In PODv4, the python algorithms often struggled to plot the confidence _a<sub>9095</sub>_ curve even when the logistic regression algorithm converges easily. POD Pro v4.5.0 can correctly generate and plot the wald _a<sub>9095</sub>_ curve for the user in the analyis wizard -> full regression analysis.

* **New Regression Technique**: Hit/Miss POD has traditionally been calculated using maximum likelihood logistic regression. However, in the cases, this algorithm may fail to converge when the data is separated (i.e. the 0s and 1s don't overlap), or when there are outliers present. To accomodate for this, POD Pro v4.5.0 offers _Firth's Bias-Reduced Logistic Regression_ . This new regression method is capable of producing an _a<sub>90</sub>_ curve for tricky datasets.

* **New Confidence Interval Techniques**: POD Pro v4.5.0 has three new statistical methods to general both the _a<sub>9095</sub>_ value and the _a<sub>9095</sub>_ curve. These methods include _Modified Wald_, _Likelihood Ratio confidence interval_, and _Modified Likelihood Ratio Confidence Interval_. More information can be found about these new confidence interval techniques in /'POD Documents'/

* **Alternative Sampling Method**: As an alternative to simple random sampling, POD Pro v4.5.0 offers Ranked Set sampling to assist in creating both the POD curve and the  _a<sub>9095</sub>_ curve. This feature is especially useful when the sample size is relatively small. 

A _Quick Help_ message box is present in the Hit Miss wizard in order to aid and explain to the user what each feature dropdown does. In addition, errors are more specfic when they occur (i.e. a9095 is infiniti), and will often give users suggestions in order to improve the quality of the results.

###### Signal Response Analysis

Signal Response has been rewritten to be calculated in R in addition to:

* **Box-Cox Transformation**: In addition to the transformations included in PODv4, POD Pro v4.5.0 can perform a box-cox transform on the signal responses. When clicked, a default lambda value will be generated by R. However, the user has freedom to alter lambda with a numeric up-down box in order to improve the test assumptions (equal variance, test for normality etc).

* **Normality histogram chart** : A normality chart featuring a histogram of the frequency of the responses has been added in order to aid the user in determining the normality of the signal responses. A normal curve with the mean and sigma of the response values is overlaid on the histogram to help the user see how 'normalish' are the responses.

Based on the provided R source files, the codebase for the Probability of Detection (POD) backend does not rely on deep traditional Object-Oriented inheritance (where one class inherits from a base class). Instead, it uses R's Reference Classes (`setRefClass`) to manage state and logic, and relies heavily on **composition and dependency**.

The architecture is divided into two primary analysis branches: **Hit/Miss Analysis** (for discrete/binary detection data) and **Signal Response Analysis** (for continuous $\hat{a}$ vs $a$ data).

Here is an explanation of the class hierarchy and structural relationships:

### 1. Hit/Miss Analysis Subsystem

The entry point for binary hit/miss data is the `HMAnalysis` class. It acts as a controller that coordinates various "worker" classes based on the user's settings.

* **Regression Models:** `HMAnalysis` delegates the statistical curve fitting to either `HMLogitApproximation` (standard logistic regression) or `HMFirthApproximation` (Firth's bias-adjusted logistic regression).
* **Confidence Intervals:** Once the curve is fit, `HMAnalysis` calculates confidence boundaries by calling `WaldConfInt`, `LikelihoodRatioConfInt`, or `ModifiedLikelihoodRatioConfInt` depending on the user's configuration.
* **Utilities:** It uses `GenAValuesOnPODCurve` to extract critical flaw sizes ($a_{90}$, $a_{90/95}$, etc.) and relies on `GenNormFit` and `LinearComboGenerator` to simulate crack sizes and generate combination matrices for the likelihood ratio tests.

### 2. Ranked Set Sampling (RSS) Subsystem

If the user enables Ranked Set Sampling instead of Simple Random Sampling, `HMAnalysis` hands control over to `RSSMainClassObject`.

* `RSSMainClassObject` holds an instance of `RSSComponents` (which stores the $m$ and $r$ parameters).
* It uses `RankedSetRegGen` to actually generate the samples (which in turn uses `RSSamplingMain` and fits regression models to the subsets).
* It uses `GenPODCurveRSS` to calculate the final POD values from the medians of the subsets and `GenRSS_A_Values` to calculate the critical flaw sizes for the RSS data.

### 3. Signal Response Subsystem

For continuous response data, the main controller is `AHatAnalysis`.

* It relies on `PrepareData` to average multiple responses if needed.
* It uses `GenPODSignalResponse` to convert the linear model and variance-covariance matrix into a standard POD curve space.
* It uses `GenerateNormalityTable` to generate histograms and frequency curves to check the normality of the residuals.
* There is also a `RecalcOriginalPOD` class used specifically to recalculate ghost curves using the $\tau$ values from previous analyses.

---

### Mermaid Class Diagram

Below is the Mermaid script detailing these composition and dependency relationships in the R Source Code:

```mermaid
classDiagram
    %% Core Hit/Miss System
    class HMAnalysis {
        +executeFullAnalysis()
        +executeFitAnalysisOnly()
        +initializeRSS()
    }
    class HMLogitApproximation
    class HMFirthApproximation
    class WaldConfInt
    class LikelihoodRatioConfInt
    class ModifiedLikelihoodRatioConfInt
    class GenAValuesOnPODCurve
    class LinearComboGenerator
    class GenNormFit

    HMAnalysis *-- RSSComponents : Contains
    HMAnalysis --> HMLogitApproximation : Uses
    HMAnalysis --> HMFirthApproximation : Uses
    HMAnalysis --> WaldConfInt : Uses
    HMAnalysis --> LikelihoodRatioConfInt : Uses
    HMAnalysis --> ModifiedLikelihoodRatioConfInt : Uses
    HMAnalysis --> GenNormFit : Uses
    HMAnalysis --> LinearComboGenerator : Uses
    HMAnalysis --> GenAValuesOnPODCurve : Uses
    HMAnalysis --> RSSMainClassObject : Delegates to (if RSS)

    %% Linear Combinations Sub-dependency
    LinearComboGenerator --> GenNormFit : Uses

    %% Ranked Set Sampling System
    class RSSMainClassObject {
        +executeRSSPOD()
    }
    class RSSComponents
    class RankedSetRegGen
    class RSSamplingMain
    class GenPODCurveRSS
    class GenRSS_A_Values

    RSSMainClassObject *-- RSSComponents : Contains
    RSSMainClassObject --> RankedSetRegGen : Uses
    RSSMainClassObject --> GenPODCurveRSS : Uses
    RSSMainClassObject --> GenRSS_A_Values : Uses
    RSSMainClassObject --> GenNormFit : Uses

    RankedSetRegGen --> RSSamplingMain : Uses
    RankedSetRegGen --> HMLogitApproximation : Uses
    RankedSetRegGen --> HMFirthApproximation : Uses

    GenPODCurveRSS --> LikelihoodRatioConfInt : Uses
    GenPODCurveRSS --> ModifiedLikelihoodRatioConfInt : Uses

    GenRSS_A_Values --> GenAValuesOnPODCurve : Uses

    %% Core Signal Response System
    class AHatAnalysis {
        +executeAhatvsA()
    }
    class RecalcOriginalPOD {
        +recalcPOD()
    }
    class PrepareData
    class GenPODSignalResponse
    class GenerateNormalityTable

    AHatAnalysis --> PrepareData : Uses
    AHatAnalysis --> GenPODSignalResponse : Uses
    AHatAnalysis --> GenerateNormalityTable : Uses

    RecalcOriginalPOD --> PrepareData : Uses
    RecalcOriginalPOD --> GenPODSignalResponse : Uses

```