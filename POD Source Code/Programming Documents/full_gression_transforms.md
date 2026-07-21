# Transform and Inverse Transform Flow in FullRegressionPanel.cs

The transformation logic in `Wizards\Steps\AHatVsANormalSteps\FullRegressionPanel.cs` is a coordinated effort between the UI controls, the central `Analysis` class, and a set of specialized transformer classes. The system is designed to apply transformations to the data for analysis and visualization, while allowing the user to interact with the data in both its original and transformed states.

## 1. Core Components

*   **`Wizards\Steps\AHatVsANormalSteps\FullRegressionPanel.cs`**: This class manages the UI controls for selecting transforms and displaying results. It orchestrates the application of transforms and the subsequent analysis.
*   **`Analysis.cs`**: This class acts as the data and analysis engine. It holds the current transform settings (`InFlawTransform`, `InResponseTransform`) and owns the instance of the `IDisplayTransformer`.
*   **`IDisplayTransformer` / `DisplayTransformer.cs`**: A service responsible for all post-analysis, display-related transformations, especially inverse transforms for UI elements. It contains logic to handle UI-specific edge cases (e.g., how to display negative numbers on a log-transformed axis).
*   **`IBoxCoxTransformer` / `BoxCoxTransformer.cs`**: This class is the single source of truth for the *purely statistical* forward transformation of data, specifically for the Box-Cox transform. It is used to calculate the `lambda` parameter and perform the initial transformation before the data is sent to the R engine.
*   **UI Controls**:
    *   `_xTransformBox` (`TransformBoxX`): A dropdown for selecting the transform for the flaw data (X-axis).
    *   `_yTransformBox` (`TransformBoxYHat`): A dropdown for selecting the transform for the response data (Y-axis).
    *   Numeric Controls (e.g., `aMinControl`, `thresholdControl`): Display and accept user input in the *original*, untransformed scale.
*   **Main Chart**: The chart displays the data in the *transformed* scale. The boundary lines on the chart are also set using transformed values.

---

## 2. The Role of Specialized Transformer Classes

As outlined in `Programming Documents\boxcox_ui.md`, the transformation logic is formally separated into two distinct responsibilities to improve consistency and maintainability.

### `BoxCoxTransformer`
This class is used exclusively for the rigorous, statistical Box-Cox transformation.

*   **Trigger**: When the user selects the "Box-Cox" transform in the UI, the `TransformBox_ValueChanged` event handler in `FullRegressionPanel.cs` calls `Analysis.CalculateAndSetBoxCoxParameters()`.
*   **Action**: This method in the `Analysis` class invokes the `BoxCoxTransformer`. The transformer takes the raw, non-censored data, determines the optimal `lambda` value by calling the R engine, and produces a `BoxCoxAnalysisResult` object containing `lambda` and other parameters (K1, K2).
*   **Purpose**: Its sole responsibility is to prepare the data for the statistical analysis engine. It does not handle any UI display logic.

### `DisplayTransformer`
This class is the single source of truth for all transformations related to the user interface.

*   **Instantiation**: After the `BoxCoxTransformer` runs, the `Analysis` class creates an instance of `DisplayTransformer`, passing it the `BoxCoxAnalysisResult` and the currently selected transform type (e.g., Log, Box-Cox).
*   **Delegation**: The core transformation methods in the `Analysis` class (`TransformValueForYAxis`, `InvertTransformValueForYAxis`) are now simple wrappers that delegate their calls directly to the current `IDisplayTransformer` instance.
*   **Purpose**: It translates values between the original data scale and the transformed scale required for display on charts and other UI elements. It contains specific logic to handle edge cases that would break a pure statistical transform, such as plotting negative numbers or zero on a logarithmic axis.

---

## 3. Forward Transformation (Applying a Transform)

1.  **User Selection**: The user selects a new transform (e.g., "Box-Cox") from the `_yTransformBox`.
2.  **Event Trigger**: The `TransformBox_ValueChanged` event fires in `FullRegressionPanel.cs`.
3.  **Parameter Calculation**: The handler calls `Analysis.CalculateAndSetBoxCoxParameters()`. This triggers the `BoxCoxTransformer` to calculate the `lambda` and creates a new `DisplayTransformer` instance within the `Analysis` class.
4.  **State Update**: The `Analysis.InResponseTransform` property is updated.
5.  **UI Update (Chart Boundaries)**: The boundary lines on the chart are updated. The handler calls `Analysis.TransformValueForYAxis(leftCensorControl.Value)`, which now delegates the call to the `DisplayTransformer` instance to get the correct transformed value for the UI.
    ```csharp
    // In Wizards\Steps\AHatVsANormalSteps\FullRegressionPanel.cs
    var y = Convert.ToDouble(Analysis.TransformValueForYAxis(leftCensorControl.Value));
    mainChart.SetLeftCensorBoundary(y, false);
    ```
6.  **Re-analysis**: `ForceUpdateAfterTransformChange()` is called, triggering `RunAnalysis()` to re-run the analysis using the newly transformed data.

---

## 4. Inverse Transformation (Updating UI from Chart)

1.  **User Interaction**: The user drags a boundary line on the main chart.
2.  **Event Trigger**: The `MainChart_LinesChanged` event fires.
3.  **Get Transformed Value**: The handler gets the new line position from the chart in the *transformed* scale.
4.  **Inverse Transform**: The handler calls `Analysis.InvertTransformValueForYAxis(value)`. This call is now delegated to the `DisplayTransformer` instance, which knows how to correctly invert the value from the display scale back to the original data scale, accounting for any UI-specific logic.
    ```csharp
    // In Wizards\Steps\AHatVsANormalSteps\FullRegressionPanel.cs
    this.thresholdControl.Value = Analysis.InvertTransformValueForYAxis(Convert.ToDecimal(value));
    ```
5.  **UI Update (Numeric Controls)**: The `thresholdControl`'s value is updated with the result, ensuring it stays synchronized with the chart.

This decoupled architecture ensures that the statistical transformation is pure and correct, while the display transformation is flexible enough to provide a clean and intuitive user experience, even with difficult edge cases.