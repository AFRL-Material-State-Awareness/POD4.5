# Refactoring Plan: Box-Cox Transformation Logic

## 1. Problem Statement

The codebase currently contains at least four different, inconsistent implementations of Box-Cox transformation and inverse-transformation logic. This duplication exists across the analysis pipeline, the UI controls, and the data-shaping classes, creating a high risk of bugs and making maintenance difficult.

The core issue is the lack of a single source of truth for these transformations, especially when handling edge cases (like non-positive values) required for UI display.

## 2. Recommended Solution: The Decoupled Display Transformer

The recommended approach is to formally separate the concerns of **statistical analysis transformation** from **UI display transformation**. This acknowledges that the requirements for each are different, while still consolidating the scattered logic into two distinct, authoritative sources.

This solution consists of two main components:

1.  **`BoxCoxTransformer` (Existing)**: This class will remain the single source of truth for the *purely statistical* forward transformation of data immediately before it is sent to the R engine for analysis. Its logic should not be concerned with UI edge cases.

2.  **`DisplayTransformer` (New)**: A new service class will be created to act as the single source of truth for all *post-analysis, display-related* transformations. This includes inverse transforms for axis labels, data tables, and any other UI element.

### State Management

To handle the dependency on analysis-specific parameters (like `lambda`, `K1`, `K2`), the `DisplayTransformer` will not be a static class. Instead, it will be instantiated for each analysis.

-   **Instantiation**: After an analysis is run, the `BoxCoxTransformer` produces a `BoxCoxAnalysisResult` object containing the calculated parameters. This result object will be passed into the constructor of the new `DisplayTransformer` instance.
-   **Updates**: If a parameter like `lambda` is changed by the user, the `Analysis` class will be responsible for creating a **new instance** of the `DisplayTransformer` with the updated results. This re-instantiation pattern is simple, safe, and avoids complex mutable state.

## 3. Implementation Plan

1.  **Create New `DisplayTransformer` Service**
    -   Create a new file, `DisplayTransformer.cs`, in the `CSharpBackendTempSolutionForPOD4Point5/CSharpBackendWithR/` project.
    -   Define an `IDisplayTransformer` interface with methods like `InverseTransformValue(double value)` and `TransformValue(double value)`.
    -   Implement the `DisplayTransformer` class. Its constructor will accept a `BoxCoxAnalysisResult` object and store the necessary parameters.
    -   Consolidate the piecewise, edge-case-aware inverse transformation logic from `Analysis.cs`, `DataPointChart.cs`, and `TransformBackCSharpTablesAHAT.cs` into this new class.

2.  **Refactor `Analysis.cs`**
    -   This class will now own the instance of `IDisplayTransformer`.
    -   It will be responsible for instantiating and re-instantiating the `DisplayTransformer` whenever the `BoxCoxAnalysisResult` changes.
    -   The logic within `TransformValueForYAxis` and `InvertTransformValueForYAxis` will be removed and replaced with calls to the `IDisplayTransformer` instance.

3.  **Refactor `TransformBackCSharpTablesAHAT.cs`**
    -   Modify its constructor to accept an `IDisplayTransformer` instance.
    -   Remove all local transformation logic (the large `switch` statements and `NthRoot` implementation).
    -   Replace the logic with calls to the appropriate methods on the provided `IDisplayTransformer` instance.

4.  **Refactor `DataPointChart.cs`**
    -   This class needs access to the `IDisplayTransformer` instance, likely passed down from the `Analysis` object.
    -   Remove the local `TransformBackValue` and `TransformValue` methods.
    -   Update the axis labeling methods (`LabelLinearAxis`, etc.) to use the `IDisplayTransformer` instance for all value-to-string conversions that require an inverse transform.

## 4. Affected Files

The following files will be created or require modification to implement this solution:

*   **New File**:
    -   `./CSharpBackendTempSolutionForPOD4Point5/CSharpBackendWithR/DisplayTransformer.cs`
*   **Files to be Modified**:
    -   `./Analysis/Analysis.cs`
    -   `./CSharpBackendTempSolutionForPOD4Point5/CSharpBackendWithR/TransformBackCSharpTablesAHAT.cs`
    -   `./Controls/DataPointChart.cs`
    -   `./Data/UpdateOutputForAHatData.cs` (To pass the `IDisplayTransformer` into `TransformBackCSharpTablesAHAT`).
    -   `./CSharpBackendTempSolutionForPOD4Point5/CSharpBackendWithR/BoxCoxTransformer.cs` (To ensure `BoxCoxAnalysisResult` contains all necessary parameters like `OriginalDataContainsZero`).
