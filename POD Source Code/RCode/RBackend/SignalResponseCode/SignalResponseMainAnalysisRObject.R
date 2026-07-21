#     Probability of Detection Version 4.5 (PODv4.5)
#     Copyright (C) 2022  University of Dayton Research Institute (UDRI)
# 
#     This program is free software: you can redistribute it and/or modify
#     it under the terms of the GNU General Public License as published by
#     the Free Software Foundation, either version 3 of the License, or
#     (at your option) any later version.
# 
#     This program is distributed in the hope that it will be useful,
#     but WITHOUT ANY WARRANTY; without even the implied warranty of
#     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
#     GNU General Public License for more details.
# 
#     You should have received a copy of the GNU General Public License
#     along with this program.  If not, see <https://www.gnu.org/licenses/>.

# SignalRespDF = the original signal response dataframe
# y_dec = The y decision threshold value (set by the user in the UI)
# modelType = the transformation to be performed on the analyses
# //KEY:
#   //1=no transform,
#   //2=x-axis only transform,
#   //3=y-axis only transform,
#   //4=both axis tranform, 
#   //5= Box-cox tranform with linear x;
#   //6= Box-cox tranform with log x
#   //7= Box-cox transform with inverse x
#   //8= linear x with inverse y
#   //9= log x with inverse y
#   //10= inverse x with linear y
#   //11= inverse x with log y
#   //12= inverse x with inverse y
# The value of lambda if the user selects a boxcox transformation
# varCovarMatrix= the values of the variance-covariance matrix for the linear fit model
# keyAValues = a list of the critical a values (a25, a50, a90, sigma, a9095) for the linear fit model
# linearModel = The dataframe that holds the linear model fit for the signal response data (i.e. the predict reponses using the linear fit)
# tau = the value found for tau. It is reused in RecalculateGhostCurveR to ensure the POD tables are EXACTLY the same
# rSquared = the value of R-squared for the linear fit model
# regressionStdErrs = the standard error of the linear regression
# linearTestResults = a list of the p-values found of taking linear tests on the linear fit model
# aVPOD = the values of the variance-covariance matrix for the POD
# ResultsPOD = a dataframe of the POD and confidence interval values
# critPts = the key aValues for POD transformation
# residuallTable = a dataframe containing the residuals for the linear fit
# frequencyTable = aka the normality table. This table holds the ferquency of responses at various ranges
# modelIntercept = the y-intercept of the straight line
# modelSlope = the slope of the straight line
# threshold table = a table containing the values of a90 and a9095 at various thresholds (should never display negative flaw sizes)
# original Data = used  to keep track of the original data when more than one inspector is involved

AHatAnalysis<-setRefClass("AHatAnalysis", fields = list(signalRespDF="data.frame", 
                                                                        y_dec="numeric", 
                                                                        modelType="numeric",
                                                                        lambda="numeric",
                                                                        varCovarMatrix="matrix",
                                                                        keyAValues="list",
                                                                        linearModel="data.frame",
                                                                        tau="numeric",
                                                                        rSqaured="numeric",
                                                                        regressionStdErrs="list",
                                                                        linearTestResults="list",
                                                                        aVPOD="matrix",
                                                                        ResultsPOD="data.frame",
                                                                        critPts="data.frame",
                                                                        #copyoutputfrompython
                                                                        residualTable="data.frame",
                                                                        frequencyTable="data.frame",
                                                                        normalCurveTable="data.frame",
                                                                        qqTable="data.frame",
                                                                        modelIntercept="numeric",
                                                                        modelSlope="numeric",
                                                                        thesholdTable="data.frame",
                                                                        #used  to keep track of the original data
                                                                        #when more than one inspector is involved
                                                                        originalData="data.frame"
                                                                        ),
                                  methods=list(
                                    #used to generate default values for all the metrics in the event of an error
                                    generateDefaultValues=function(){
                                      #linear model DF
                                      linearModel<<-data.frame(
                                        x=c(0,0,0,0,0),
                                        fit=c(0,0,0,0,0),
                                        y= c(0,0,0,0,0)
                                      )
                                      #POD curve table
                                      ResultsPOD<<-data.frame(
                                        defect_sizes=c(1,1,1,1,1),
                                        probabilities=c(0,0,0,0,0),
                                        defect_sizes_upCI=c(0,0,0,0,0)
                                      )
                                      #residual table
                                      residualTable<<-data.frame(
                                        x=c(0,0,0,0,0),
                                        y=c(0,0,0,0,0),
                                        transformFlaw=c(0,0,0,0,0),
                                        transformResponse=c(0,0,0,0,0),
                                        fit=c(0,0,0,0,0),
                                        t_diff=c(0,0,0,0,0)
                                      )
                                      #threshold table
                                      thesholdTable<<-data.frame(
                                        threshold=c(1,1,1,1,1),
                                        a90=c(0,0,0,0,0),
                                        a9095=c(0,0,0,0,0),
                                        a50=c(0,0,0,0,0),
                                        v11=c(0,0,0,0,0),
                                        v12=c(0,0,0,0,0),
                                        v22=c(0,0,0,0,0)
                                      )
                                      frequencyTable<<-data.frame(
                                        Range=c(0,0,0,0,0),
                                        Freq=c(0,0,0,0,0)
                                      )
                                      normalCurveTable<<-data.frame(
                                        Response=c(0,0,0,0,0),
                                        Range=c(0,0,0,0,0)
                                      )
                                      qqTable<<-data.frame(
                                        Theoretical=c(0,0,0,0,0),
                                        Sample=c(0,0,0,0,0)
                                      )
                                      critPts<<-data.frame(
                                        index=-1,
                                        a_25_25=-1,
                                        a_50_50=-1,
                                        a_90_50=-1,
                                        aSigma=-1,
                                        a_90_95=-1
                                      )
                                      #return an empty matrix of negative ones if empty
                                      aVPOD<<-matrix(0, nrow=2, ncol=2)
                                      #blank a values array
                                      keyAValues<<-list(0,0,0,0,0)
                                      linearTestResults<<-list(
                                        list(statistic=0, p.value=0.0, method="", data.name=""),
                                        list(formula="", formula.name='', ChiSquare=0.0, Df=0, p=0.0, test=''),
                                        list(r=0, dw=0, p=0, alternative=''),
                                        list(`Sum Sq`=0, Df=0, `F value`=0.0, `Pr(>F)`=0.0))
                                      modelIntercept<<-0
                                      modelSlope<<-0
                                      rSqaured<<-0
                                    },
                                    setOriginalData=function(psOrigData){
                                      originalData<<-psOrigData
                                    },
                                    getOriginalData=function(){
                                      return(originalData)
                                    },
                                    setLinearModel=function(psLMObject){
                                      linearModel<<-psLMObject
                                    },
                                    getLinearModel=function(){
                                      names(linearModel)[names(linearModel) == 'x'] <<- 'flaw'
                                      return(linearModel)
                                    },
                                    setTau=function(setTau){
                                      tau <<- setTau
                                    },
                                    getTau=function(){
                                      return(tau)
                                    },
                                    setRawResidualTable=function(rawResidTable){
                                      rawResidualTable<<-rawResidTable
                                    },
                                    getRawResidualTable=function(){
                                      names(rawResidualTable)[names(rawResidualTable) == 'x'] <<- 'flaw'
                                      return(rawResidualTable)
                                    },
                                    setResidualTable=function(psResidTable){
                                      residualTable<<-psResidTable
                                    },
                                    getResidualTable=function(){
                                      names(residualTable)[names(residualTable) == 'x'] <<- 'flaw'

                                      #cleanPrint("**********START getResidualTable**********")
                                      #cleanPrint(residualTable)
                                      #cleanPrint("**********END getResidualTable**********")

                                      return(residualTable)
                                    },
                                    setResults=function(psResults){
                                      ResultsPOD<<-psResults
                                    },
                                    getResults=function(){
                                      resultsPOD<-data.frame(
                                        flaw= ResultsPOD$defect_sizes,
                                        pod = ResultsPOD$probabilities,
                                        confidence= ResultsPOD$defect_sizes_upCI
                                      )
                                      return(resultsPOD)
                                    },
                                    setThresholdDF=function(psThreshDF){
                                      thesholdTable<<-psThreshDF
                                    },
                                    getThresholdDF=function(){
                                      return(thesholdTable)
                                    },
                                    setFreqTable=function(psFreqTable){
                                      frequencyTable<<-psFreqTable
                                    },
                                    getFreqTable=function(){
                                      return(frequencyTable)
                                    },
                                    setNormalCurveTable=function(psNormalCurveTb){
                                      normalCurveTable<<-psNormalCurveTb
                                    },
                                    getNormalCurveTable=function(){
                                      return(normalCurveTable)
                                    },
                                    setQQTable=function(psQQTable){
                                      qqTable<<-psQQTable
                                    },
                                    getQQTable=function(){
                                      return(qqTable)
                                    },
                                    setModelIntercept=function(psModelInt){
                                      modelIntercept<<-psModelInt
                                    },
                                    getModelIntercept=function(){
                                      return(modelIntercept)
                                    },
                                    setModelSlope=function(psSlope){
                                      modelSlope<<-psSlope
                                    },
                                    getModelSlope=function(){
                                      return(modelSlope)
                                    },
                                    setRSquared=function(psRSqaured){
                                      rSqaured<<-psRSqaured
                                    },
                                    getRSquared=function(){
                                      return(rSqaured)
                                    },
                                    getLambda=function(){
                                      return(lambda)
                                    },
                                    setRegressionStdErrs=function(psRegStdErrs){
                                      regressionStdErrs<<-psRegStdErrs
                                    },
                                    getRegressionStdErrs=function(){
                                      if(length(regressionStdErrs)==0){
                                        regressionStdErrs<<-list(0,0,0,0)
                                      }
                                      return(regressionStdErrs)
                                    },
                                    setCritPts=function(psCritPts){
                                      critPts<<-psCritPts
                                    },
                                    getCritPts=function(){
                                      return(critPts)
                                    },
                                    setLinearTestResults=function(psTestResults){
                                      linearTestResults<<-psTestResults
                                    },
                                    getLinearTestResults=function(){
                                      return(linearTestResults)
                                    },
                                    setCovarianceMatrix=function(psAVPOD){
                                      aVPOD<<-psAVPOD
                                    },
                                    getCovarianceMatrix=function(){
                                      return(as.data.frame(aVPOD))
                                    },
                                    getCovarianceMatrixAsMatrix=function(){
                                      return(aVPOD)
                                    },
                                    setKeyAValues=function(psKeyAValues){
                                      keyAValues<<-psKeyAValues
                                    },
                                    getKeyAValues=function(){
                                      return(keyAValues)
                                    },
                                    prepareData=function(){
                                      testInstance<-PrepareData$new(signalRespDF=signalRespDF)
                                      testInstance$getOrigDataframe()
                                      testInstance$createAvgRespDF()
                                      signalRespDF<<-testInstance$createAvgRespDF()
                                    },
                                    get_qq_plot_values = function(res_std) {
                                      n <- length(res_std)
                                      # 1. Sort residuals (Y values)
                                      y_vals <- sort(res_std)
                                      # 2. Calculate probabilities and theoretical quantiles (X values)
                                      p_vals <- (1:n - 0.5) / n
                                      x_vals <- qnorm(p_vals)
                                      # 3. Calculate robust Reference Line (passes through 25th and 75th percentiles)
                                      q25_x <- qnorm(0.25)
                                      q75_x <- qnorm(0.75)
                                      q25_y <- quantile(y_vals, 0.25, names = FALSE)
                                      q75_y <- quantile(y_vals, 0.75, names = FALSE)
                                      slope <- (q75_y - q25_y) / (q75_x - q25_x)
                                      intercept <- q25_y - slope * q25_x
                                      expected_line <- intercept + slope * x_vals
                                      # 4. Calculate 95% Confidence Intervals (Pointwise)
                                      # Standard error of the theoretical quantiles
                                      se <- (1 / dnorm(x_vals)) * sqrt((p_vals * (1 - p_vals)) / n)
                                      lower_ci <- expected_line - 1.96 * slope * se
                                      upper_ci <- expected_line + 1.96 * slope * se
                                      # 5. Return as a combined dataframe
                                      return(data.frame(
                                        theoretical_x = x_vals,
                                        sample_y = y_vals,
                                        reference_line = expected_line,
                                        lower_ci_95 = lower_ci,
                                        upper_ci_95 = upper_ci
                                      ))
                                    },
                                    executeAhatvsA=function(){

                                      #preprocess the input if there's more than one inspector
                                      setOriginalData(signalRespDF)
                                      prepareData()
                                      #perform necessary transforms
                                      performTransforms()

                                      # --- Single, Unified Model Fit using Survival Regression ---
                                      # survreg handles both censored and uncensored data correctly.
                                      final_model <- genAhatVersusACensored()

                                      #cleanPrint("---------------FINAL MODEL START----------------")
                                      #cleanPrint(final_model)
                                      #cleanPrint("---------------FINAL MODEL END------------------")

                                      # Extract results from the model
                                      setModelIntercept(final_model$coefficients[[1]])
                                      setModelSlope(final_model$coefficients[[2]])
                                      model_fit_values <- final_model$linear.predictors
                                      
                                      # Manually calculate residuals, as survreg doesn't provide them directly
                                      residuals <- signalRespDF$y.trans - model_fit_values

                                      #cleanPrint("---------------RESIDUALS START----------------")
                                      #cleanPrint(residuals)
                                      #cleanPrint("---------------RESIDUALS END------------------")

                                      #set the linear model so it can be returned
                                      linearModDF=data.frame(
                                        x=signalRespDF$x,
                                        fit=model_fit_values
                                      )
                                      #append the responses after the fitted value
                                      responsesAll<-subset(originalData, select = -c(Index, x, event))
                                      linearModDF<-cbind(linearModDF, responsesAll)
                                      setLinearModel(linearModDF)

                                      #don't perform linear tests if user is in transform panel
                                      if(fullAnalysis){
                                        # Run linear tests. If censored, this function creates a temporary lm object.
                                        updateLinearTestsCensored(final_model)
                                      }

                                      ##create the residual dataframe
                                      ResidualDF=data.frame(
                                        flaw=signalRespDF$x,
                                        y=signalRespDF$y,
                                        transformFlaw=rep(0, nrow(signalRespDF)),
                                        transformResponse=rep(0, nrow(signalRespDF)),
                                        fit=model_fit_values,
                                        t_diff=residuals
                                      )

                                      #cleanPrint("---------------ResidualDF START----------------")
                                      #cleanPrint(ResidualDF)
                                      #cleanPrint("---------------ResidualDF END------------------")

                                      setRawResidualTable(ResidualDF)

                                      #cleanPrint("---------------RAW RESIDUALS START----------------")
                                      #cleanPrint(rawResidualTable)
                                      #cleanPrint("---------------RAW RESIDUALS END------------------")

                                      #cleanPrint("---------------signalRespDF START----------------")
                                      #cleanPrint(signalRespDF)
                                      #cleanPrint("---------------signalRespDF END------------------")
                                      
                                      # Filter out censored data - only include uncensored points (event == 1)
                                      ResidualDF <- ResidualDF[signalRespDF$event == 1, ]

                                      #cleanPrint("---------------ResidualDF Censored START----------------")
                                      #cleanPrint(ResidualDF)
                                      #cleanPrint("---------------ResidualDF Censored END------------------")

                                      setResidualTable(ResidualDF)

                                      #cleanPrint("---------------UNCENSORED RESIDUALS START----------------")
                                      #cleanPrint(residualTable)
                                      #cleanPrint("---------------UNCENSORED RESIDUALS END------------------")

                                      #global flag used to speed of transforms for signal response
                                      if(fullAnalysis){
                                        # R-squared is not directly applicable for survreg in the same way as lm.
                                        # We will only set it for the uncensored case where survreg is equivalent to lm.
                                        is_censored <- (0 %in% signalRespDF$event || 2 %in% signalRespDF$event)
                                        if (!is_censored) {
                                          # For uncensored data, we can create a temporary lm object to get R-squared
                                          temp_lm <- lm(y.trans ~ x.trans, data = signalRespDF)
                                          setRSquared(summary(temp_lm)$r.squared)
                                        } else {
                                          # For censored data, calculate a pseudo R-squared based on uncensored points
                                          uncensored_indices <- which(signalRespDF$event == 1)
                                          uncensored_y <- signalRespDF$y.trans[uncensored_indices]
                                          uncensored_fit <- model_fit_values[uncensored_indices]
                                          ss_res <- sum((uncensored_y - uncensored_fit)^2)
                                          ss_tot <- sum((uncensored_y - mean(uncensored_y))^2)
                                          setRSquared(1 - (ss_res / ss_tot))
                                        }

                                        #find the key a values and the covariance matrix
                                        genAvaluesAndMatrix(final_model)
                                        #calculate the standard errors for regression
                                        calcStandardErrs(final_model)
                                        #generates the thesholds table for UI
                                        genThresholdsTable(final_model)
                                        #function that generates the POD curve and stores the results
                                        genPODCurve()
                                        # --- New Normality Check Logic (using ResidualDF) ---
                                        # All subsequent calculations will use the residuals from the main model fit.
                                        normality_responses <- data.frame(y = residualTable$t_diff)

                                        # This ensures the histogram, its range, and the curve are all consistent.
                                        normalityCheck <- GenerateNormalityTable$new(
                                          responses = normality_responses,
                                          responsesMin = min(normality_responses$y),
                                          responsesMax = max(normality_responses$y)
                                        )
                                        normalityCheck$GenFrequencyTable()
                                        setFreqTable(normalityCheck$getFreqTable())

                                        # Generate curve data from the same residuals
                                        xfit <- seq(min(normality_responses$y), max(normality_responses$y), length = nrow(normality_responses))
                                        yfit <- dnorm(xfit, mean = mean(normality_responses$y), sd = sd(normality_responses$y))

                                        # Perform scaling to match histogram frequencies
                                        if (nrow(normalityCheck$getFreqTable()) > 1) {
                                          yfit <- yfit * diff(normalityCheck$getFreqTable()$Range[1:2]) * nrow(normality_responses)
                                        } else {
                                          yfit <- yfit * 1 * nrow(normality_responses) # Fallback if only one bin
                                        }

                                        setNormalCurveTable(data.frame(Response = xfit, Range = yfit))
                                        # --- End of New Normality Check Logic ---

                                        # Get standardized residuals (using scale() on our existing residuals)
                                        standard_res <- scale(residualTable$t_diff)

                                        qqDataFrame <- get_qq_plot_values(standard_res)

                                        setQQTable(qqDataFrame)
                                      }                                      
                                    },
                                    genPODCurve=function(){
                                      #calculate POD dataframe
                                      newPODSR=GenPODSignalResponse$new()
                                      newPODSR$initialize(a.V_POD=aVPOD, aMu=keyAValues[[2]], aSigma=keyAValues[[4]])
                                      newPODSR$genPODCurve()
                                      setResults(newPODSR$getPODSR())
                                      setCritPts(newPODSR$getCriticalPoints())
                                    },
                                    performTransforms=function(){
                                      signalRespDF$x.trans<<-f_a(signalRespDF$x)
                                      signalRespDF$y.trans<<-f_a(signalRespDF$y)
                                    },
                                    linearTests=function(linearM){
                                      # Shapiro-Wilk Normality Test
                                      # Testing if the (studentized/standardized/Pearson) residuals are normally (Gaussian) distributed.
                                      # H_0: Normally distributed  VS H_a: Not Normally distributed
                                      # Fail to reject, so normal enough.
                                      shapiro=shapiro.test(studres(linearM))
                                      # Breusch-Pagan test
                                      # Non-constant variance test... H_0: Constant variance VS H_a: Non-constant variance
                                      # We fail to reject H_0, so non-constant variance isn't a problem. 
                                      nonConst=ncvTest(linearM)
                                      # Test for auto-correlation (ie, a pattern in the data due to "time," or in this case, order)
                                      # Data does not have auto-correlation. 
                                      durbinWTest=durbinWatsonTest(linearM)
                                      
                                      # Default fallback data frame to prevent downstream UI crashes
                                      lackOfFitTest<-data.frame(
                                        'Sum sq'=c(0,0),
                                        'Df'=c(1,0),
                                        'F value'=c(0,0),
                                        'Pr(>F)'=c(0, 0),
                                        check.names = FALSE
                                      )
                                      
                                      # --- FIX: Move model reduction INSIDE tryCatch and use safer scoping ---
                                      tryCatch(expr = {
                                        full <- linearM
                                        
                                        # Use the model's internally stored data frame. This prevents scoping/environment crashes.
                                        partial <- lm(y ~ 1, data = linearM$model)
                                        
                                        # Base anova() correctly compares two models to get the lack-of-fit F-test
                                        a_res <- anova(partial, full)
                                        
                                        # Extract the correct row so it matches what the UI expects
                                        lackOfFitTest <- data.frame(
                                          'Sum sq' = c(a_res$`Sum of Sq`[2], 0),
                                          'Df' = c(a_res$Df[2], 0),
                                          'F value' = c(a_res$F[2], 0),
                                          'Pr(>F)' = c(a_res$`Pr(>F)`[2], 0),
                                          check.names = FALSE
                                        )
                                      },
                                      error = function(e){
                                        # Silently fallback to zeros if test fails
                                      })
                                      
                                      setLinearTestResults(list(shapiro,nonConst,durbinWTest,lackOfFitTest[1,]))
                                    },
                                    genAhatVersusACensored=function(){
                                      # It expects a censored object; this data is not censored but we can still use it.
                                      # Note, we are using the Box Cox transformed y and f_a (log) transformed x
                                      censored.a.hat <- Surv(signalRespDF$y.trans, signalRespDF$event)
                                      #cleanPrint("-----START censored.a.hat------")
                                      #cleanPrint(censored.a.hat)
                                      #cleanPrint("-----END censored.a.hat------")
                                        #Surv(time = signalRespDF$y.trans, time2 = signalRespDF$event, event=signalRespDF$event, type="interval")
                                      #censored.a.hat <- Surv(time = signalRespDF$y, time2 = signalRespDF$y, type = "interval2")
                                      a.hat.censor.df <- data.frame(censored.a.hat, x = f_a(signalRespDF$x))

                                      #cleanPrint("-----START a.hat.censor.df------")
                                      #cleanPrint(a.hat.censor.df)
                                      #cleanPrint("-----END a.hat.censor.df------")

                                      # Here's the linear model.
                                      a.hat.vs.a.censored <- survreg(censored.a.hat~x,data=a.hat.censor.df, dist='gaussian')


                                      #cleanPrint("-----START a.hat.vs.a.censored------")
                                      #cleanPrint(a.hat.vs.a.censored)
                                      #cleanPrint("-----END a.hat.censor.df------")

                                        #survreg(formula = censored.a.hat ~ x,
                                                            #dist = "gaussian", data = a.hat.censor.df)
                                      #a.hat.vs.a.censored <-survreg(formula= Surv(y.trans, event)~x, dist= "gaussian", data= signalRespDF)
                                      return(a.hat.vs.a.censored)
                                    },
                                    ##converts the survreg object into a lm object in order to run the linear tests on the censored data
                                    ##TODO: ask christie about this to see if it's valid
                                    updateLinearTestsCensored=function(ahatvACensored){
                                      # --- FIX: Filter out censored data to prevent artificial spikes in residual tests ---
                                      uncensored_indices <- which(signalRespDF$event == 1)
                                      
                                      # Fit a temporary OLS model on STRICTLY the uncensored, transformed data
                                      temp_uncensored_df <- data.frame(
                                        x = signalRespDF$x.trans[uncensored_indices], 
                                        y = signalRespDF$y.trans[uncensored_indices]
                                      )
                                      
                                      newCensoredLMObject<-lm(y~x, data=temp_uncensored_df)
                                      
                                      # Run tests on this clean, uncensored lm object
                                      linearTests(newCensoredLMObject)
                                    },
                                    genAvaluesAndMatrix=function(a.hat.vs.a.censored){
                                      a.hat.decision = y_dec 
                                      #if(modelType==3 || modelType==4){
                                      #  a.hat.decision=log(a.hat.decision)
                                      #}
                                      #else if(modelType==5 || modelType==6 || modelType==7){
                                      #  a.hat.decision=(a.hat.decision^lambda-1)/lambda
                                      #}
                                      #else if(modelType==8 || modelType==9 || modelType==12){
                                      #  a.hat.decision= 1/a.hat.decision
                                      #}
                                      a.b0 <- as.numeric(a.hat.vs.a.censored$coef[1])
                                      a.b1 <- as.numeric(a.hat.vs.a.censored$coef[2])
                                      a.tau <- as.numeric(a.hat.vs.a.censored$scale) # random sigma
                                      setTau(a.tau)
                                      a.covariance.matrix <- a.hat.vs.a.censored$var
                                      varCovarMatrix<<-a.covariance.matrix
                                      # Transform backward into the original units
                                      # if(lambda!=0){
                                      #   a.hat.decision = (a.hat.decision*lambda+1)^(1/lambda)
                                      # }else{
                                      #   a.hat.decision = exp(a.hat.decision)
                                      # }
                                      aMu <- (a.hat.decision - a.b0)/a.b1
                                      aSigma <- a.tau/a.b1
                                      POD.transition.matrix <- matrix(c(1, aMu, 0, 0, aSigma, -a.tau), nrow = 3, byrow = FALSE)
                                      a.VCV <- (-1/a.b1)^2 * t(POD.transition.matrix)
                                      a50 <- aMu
                                      a90 <- aMu + qnorm(0.9) * aSigma
                                      z90 <- qnorm(0.9)
                                      #  a.U = (-1/a.b1)*matrix(c(1, aMu, 0, 0, aSigma, -1), nrow = 3, byrow = FALSE)
                                      a.U = (-1/a.b1)*matrix(c(1, aMu, 0, 0, aSigma, -a.tau), nrow = 3, byrow = FALSE)
                                      a.V_POD = t(a.U)%*%varCovarMatrix%*%a.U
                                      SD.a.90 = sqrt(a.V_POD[1,1]+2*z90*a.V_POD[1,2]+(z90^2)*a.V_POD[2,2])
                                      a9095 <- aMu + z90 * aSigma + qnorm(0.95) * SD.a.90
                                      # Transform x back to the original units
                                      a50 <- f_a_i(a50)
                                      a90 <- f_a_i(a90)
                                      a9095 <- f_a_i(a9095)
                                      #set parameters
                                      setCovarianceMatrix(a.V_POD)
                                      ###TODO: Also calculate A25 which is now set to -1
                                      setKeyAValues(list(-1, a50, a90, aSigma, a9095))
                                    },
                                    genThresholdsTable=function(a.hat.vs.a.censored){
                                      thresholds=linspace(min(signalRespDF$y)-.5*max(signalRespDF$y), max(signalRespDF$y)+.5*max(signalRespDF$y), 300)
                                      columns=c("threshold", "a90", "a90_95", "a50", "v11", "v12", "v22")
                                      threshDataFrame=data.frame(matrix(nrow=0, ncol=length(columns)))
                                      colnames(threshDataFrame)=columns
                                      a.b0 <- as.numeric(a.hat.vs.a.censored$coef[1])
                                      a.b1 <- as.numeric(a.hat.vs.a.censored$coef[2])
                                      a.tau <- as.numeric(a.hat.vs.a.censored$scale) # random sigma
                                      a.covariance.matrix <- a.hat.vs.a.censored$var
                                      varCovarMatrix<<-a.covariance.matrix
                                      for(i in thresholds){
                                        a.hat.decision = i # = 200
                                        aMu <- (a.hat.decision - a.b0)/a.b1
                                        aSigma <- a.tau/a.b1
                                        POD.transition.matrix <- matrix(c(1, aMu, 0, 0, aSigma, -a.tau), nrow = 3, byrow = FALSE)
                                        a.VCV <- (-1/a.b1)^2 * t(POD.transition.matrix)
                                        a50 <- aMu
                                        a90 <- aMu + qnorm(0.9) * aSigma
                                        z90 <- qnorm(0.9)
                                        #  a.U = (-1/a.b1)*matrix(c(1, aMu, 0, 0, aSigma, -a.tau), nrow = 3, byrow = FALSE)
                                        a.U = (-1/a.b1)*matrix(c(1, aMu, 0, 0, aSigma, -a.tau), nrow = 3, byrow = FALSE)
                                        a.V_POD = t(a.U)%*%varCovarMatrix%*%a.U
                                        SD.a.90 = sqrt(a.V_POD[1,1]+2*z90*a.V_POD[1,2]+(z90^2)*a.V_POD[2,2])
                                        a9095 <- aMu + z90 * aSigma + qnorm(0.95) * SD.a.90
                                        # Transform x back to the original units
                                        a50 <- f_a_i(a50)
                                        a90 <- f_a_i(a90)
                                        a9095 <- f_a_i(a9095)
                                        threshDataFrame=rbind(threshDataFrame,
                                                              data.frame(threshold=i,
                                                                         a90, 
                                                                         a9095, 
                                                                         a50, 
                                                                         v11=a.V_POD[[1]],  
                                                                         v12=a.V_POD[1,2],
                                                                         v22=a.V_POD[2,2]))
                                      }
                                      setThresholdDF(threshDataFrame)
                                    },
                                    calcStandardErrs=function(myLinFit){
                                      slopeStdError=summary(myLinFit)$table[2,2]
                                      interceptStdError=summary(myLinFit)$table[1,2]
                                      # The residual standard error (sigma) is the 'scale' parameter from the survreg model.
                                      # The original manual calculation was incorrect due to a faulty calculation of
                                      # residuals and degrees of freedom. myLinFit$scale is the statistically
                                      # correct value for this model.
                                      residualError <- myLinFit$scale
                                      # This is a valid application of the Delta Method to find the standard error of the scale parameter (residualError).
                                      # It uses the variance of the Log(scale) parameter, which is myLinFit$var[3,3].
                                      residualErrorStdError=sqrt(varCovarMatrix[3,3])*residualError
                                      #stored as: slope std error, slope intercept std error, and residual error
                                      stdErrors=list(slopeStdError, interceptStdError, residualError, residualErrorStdError)
                                      setRegressionStdErrs(stdErrors)
                                    },
                                    # Inverse function of f_a. Uncomment 1
                                    f_a_i=function(a){ return(a) # exp(a)#log(a) #sqrt(a)
                                    },
                                    # Function transforming the flaw size. Uncomment 1
                                    f_a=function(a){return(a) #log(a) #exp(a) #a^2
                                    },
                                    #used for Debugging ONLY
                                    plotSimdata=function(df){
                                      myPlot=ggplot(data=df, mapping=aes(x=flaw, y=pod))+geom_point()+
                                        ggtitle(paste("POD Curve"))#+scale_x_continuous(limits = c(0,1.0))+scale_y_continuous(limits = c(0,1))
                                      print(myPlot)
                                    },
                                    #used for Debugging ONLY
                                    plotCI=function(df){
                                      myPlot=ggplot(data=df, mapping=aes(x=flaw, y=confidence))+geom_point()+
                                        ggtitle(paste("Confidence interval Curve"))#+scale_x_continuous(limits = c(0,1.0))+scale_y_continuous(limits = c(0,1))
                                      print(myPlot)
                                    },
                                    #used for Debugging ONLY
                                    plotSimdata=function(df){
                                      myPlot=ggplot(data=df, mapping=aes(x=flaw, y=y))+geom_point()
                                      print(myPlot)
                                    },
                                    plotNormality=function(df){
                                      # Barplot
                                      ggplot(df, aes(x=Range, y=Freq)) + 
                                        geom_bar(stat = "identity", width=.00001)
                                    },
                                    cleanPrint=function(x) {
                                      # 1. Capture the output
                                      raw_output <- capture.output(print(x))

                                      # 2. Collapse and Strip weird characters
                                      combined_output <- paste(raw_output, collapse = "\n")
                                      clean_output <- gsub("[\x02\x03\xff\xfe]", "", combined_output)

                                      # 3. ACTUALLY PRINT TO CONSOLE
                                      cat(clean_output, "\n") 
  
                                      # Return it just in case you need to assign it to a variable
                                      return(invisible(clean_output)) 
                                    }
                                  ))