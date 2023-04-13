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
                                    setResidualTable=function(psResidTable){
                                      residualTable<<-psResidTable
                                    },
                                    getResidualTable=function(){
                                      names(residualTable)[names(residualTable) == 'x'] <<- 'flaw'
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
                                    executeAhatvsA=function(){
                                      #preprocess the input if there's more than one inspector
                                      setOriginalData(signalRespDF)
                                      prepareData()
                                      #perform necessary transforms
                                      performTransforms()
                                      # Fitting a linear model
                                      linearModel_lm <- lm(y ~ x, data = signalRespDF, na.action=na.omit)
                                      setModelIntercept(linearModel_lm$coefficients[[1]])
                                      setModelSlope(linearModel_lm$coefficients[[2]])
                                      #set the linear model so it can be returned
                                      linearModDF=data.frame(
                                        #Index= 1:length(linearModel_lm$fitted.values),
                                        x=signalRespDF$x,
                                        fit=linearModel_lm$fitted.values
                                      )
                                      #append the responses after the fitted value
                                      responsesAll<-subset(originalData, select = -c(Index, x, event))
                                      linearModDF<-cbind(linearModDF, responsesAll)
                                      setLinearModel(linearModDF)
                                      #append the responses at the end of the flaws and fitted values
                                      responsesOnly=or
                                      setLinearModel(linearModDF)
                                      #don't perform linear tests if user is in transform panel
                                      if(fullAnalysis){
                                        #peformTests
                                        linearTests(linearM=linearModel_lm)
                                      }
                                      ##create the residual dataframe
                                      #ResidualDF=cbind(linearModDF, linearModel_lm$residuals)
                                      #names(ResidualDF)[names(ResidualDF) == 'linearModel_lm$residuals'] <- 't_diff'
                                      ResidualDF=data.frame(
                                        flaw=signalRespDF$x,
                                        y=signalRespDF$y,
                                        transformFlaw=rep(0, nrow(signalRespDF)),
                                        transformResponse=rep(0, nrow(signalRespDF)),
                                        fit=linearModel_lm$fitted.values,
                                        t_diff=linearModel_lm$residuals
                                      )
                                      setResidualTable(ResidualDF)
                                      #generate ahat versus acensored
                                      ahatvACensored<-genAhatVersusACensored()
                                      #attach necessary attributes of the lm object to the survival censored object
                                      #ahatvACensored$rank=linearModel_lm$rank
                                      #ahatvACensored$qr=linearModel_lm$qr
                                      #update paramters if any points are censored
                                      if(0 %in% signalRespDF$event ||  2 %in% signalRespDF$event){
                                        setModelIntercept(ahatvACensored$coefficients[[1]])
                                        setModelSlope(ahatvACensored$coefficients[[2]])
                                        #set the linear model so it can be returned
                                        linearModDF=data.frame(
                                          x=signalRespDF$x,
                                          fit=ahatvACensored$linear.predictors
                                        )
                                        #append the responses after the fitted value
                                        responsesAll<-subset(originalData, select = -c(Index, x, event))
                                        linearModDF<-cbind(linearModDF, responsesAll)
                                        setLinearModel(linearModDF)
                                        #calculate the residuals from the linear model
                                        residuals=c()
                                        for(i in 1:nrow(linearModDF)){
                                          thisResid=signalRespDF$y[i]-ahatvACensored$linear.predictors[i]
                                          residuals=c(residuals, thisResid)
                                        }
                                        ##create the residual dataframe
                                        ResidualDF=cbind(linearModDF, residuals)
                                        names(ResidualDF)[names(ResidualDF) == 'residuals'] <- 't_diff'
                                        ResidualDF=data.frame(
                                          flaw=signalRespDF$x,
                                          y=signalRespDF$y,
                                          transformFlaw=rep(0, nrow(signalRespDF)),
                                          transformResponse=rep(0, nrow(signalRespDF)),
                                          fit=ahatvACensored$linear.predictors,
                                          t_diff=residuals
                                        )
                                        setResidualTable(ResidualDF)
                                        #don't perform linear tests if user is in transform panel
                                        if(fullAnalysis){
                                          #update the linear regression tests with the censored data
                                          updateLinearTestsCensored(ahatvACensored)
                                        }
                                      }
                                      #global flag used to speed of transforms for signal response
                                      if(fullAnalysis){
                                        #get the value of R-squared(TODO: check if this changes with censored data)
                                        setRSquared(summary(linearModel_lm)$r.squared)
                                        #find the key a values and the covariance matrix
                                        genAvaluesAndMatrix(ahatvACensored)
                                        #calculate the standard errors for regression
                                        calcStandardErrs(ahatvACensored)
                                        #generates the thesholds table for UI
                                        genThresholdsTable(ahatvACensored)
                                        #function that generates the POD curve and stores the results
                                        genPODCurve()
                                        #generate the normality chart
                                        responsesCheck=data.frame(y=signalRespDF$y, event = signalRespDF$event)
                                        normalityCheck<-GenerateNormalityTable$new(responses=subset(responsesCheck, event==1), responsesMin = min(responsesCheck$y), responsesMax = max(responsesCheck$y))
                                        normalityCheck$GenFrequencyTable()
                                        setFreqTable(normalityCheck$getFreqTable())
                                        #generate a dataframe that creates a normal curve based on the responses
                                        xfit <- seq(min(signalRespDF$y), max(signalRespDF$y), length = nrow(signalRespDF)) 
                                        yfit <- dnorm(xfit, mean = mean(signalRespDF$y), sd = sd(signalRespDF$y)) 
                                        #yfit <- yfit * diff(h$mids[1:2]) * length(g) 
                                        yfit <- yfit * diff(normalityCheck$getFreqTable()$Range[1:2]) * length(signalRespDF$y) 
                                        setNormalCurveTable(data.frame(Response=xfit, Range=yfit))
                                        #setNormalCurveTable(data.frame(Response=signalRespDF$y, Range=dnorm(signalRespDF$y, mean = mean(signalRespDF$y),  sd =sd(signalRespDF$y))))
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
                                      #test for lack of fit (compare the models when using slope and without)
                                      full<-lm(y~x, data=signalRespDF)
                                      #partial<-lm(y~rep(full$coefficients[[1]], nrow(signalRespDF)), data= signalRespDF)
                                      partial<-lm(y~1, data= signalRespDF)
                                      #lackOfFitTest<-Anova(full, partial)
                                      lackOfFitTest<-data.frame(
                                        'Sum sq'=c(0,0),
                                        'Df'=c(1,0),
                                        'F value'=c(0,0),
                                        'Pr(>F)'=c(0, 0)
                                      )
                                      tryCatch(expr = {lackOfFitTest<-Anova(full, partial)},
                                               error = function(e){
                                                 #cat("error ocurred!")
                                               })
                                      setLinearTestResults(list(shapiro,nonConst,durbinWTest,lackOfFitTest[1,]))
                                    },
                                    genAhatVersusACensored=function(){
                                      # It expects a censored object; this data is not censored but we can still use it.
                                      # Note, we are using the Box Cox transformed y and f_a (log) transformed x
                                      censored.a.hat <- Surv(time = signalRespDF$y.trans, time2 = signalRespDF$event, event=signalRespDF$event, type="interval")
                                      #censored.a.hat <- Surv(time = signalRespDF$y, time2 = signalRespDF$y, type = "interval2")
                                      a.hat.censor.df <- data.frame(censored.a.hat, x = f_a(signalRespDF$x))
                                      # Here's the linear model.
                                      a.hat.vs.a.censored <- survreg(formula = censored.a.hat ~ x,
                                                                     dist = "gaussian", data = a.hat.censor.df)
                                      #a.hat.vs.a.censored <-survreg(formula= Surv(y.trans, event)~x, dist= "gaussian", data= signalRespDF)
                                      return(a.hat.vs.a.censored)
                                    },
                                    ##converts the survreg object into a lm object in order to run the linear tests on the censored data
                                    ##TODO: ask christie about this to see if it's valid
                                    updateLinearTestsCensored=function(ahatvACensored){
                                      newSignalDF_Fit<-data.frame(x= signalRespDF$x, y=ahatvACensored$linear.predictors)
                                      newCensoredLMObject<-lm(y~x, data=newSignalDF_Fit,na.action=na.omit)
                                      newCensoredLMObject$model$y=signalRespDF$y
                                      newResiduals=c()
                                      for(i in 1:length(newCensoredLMObject$fitted.values)){
                                        residual=newCensoredLMObject$model$y[i]-newCensoredLMObject$fitted.values[i]
                                        newResiduals=c(newResiduals, residual)
                                      }
                                      newCensoredLMObject$residuals=newResiduals
                                      #global<<-newCensoredLMObject
                                      linearTests(newCensoredLMObject)
                                    },
                                    genAvaluesAndMatrix=function(a.hat.vs.a.censored){
                                      a.hat.decision = y_dec 
                                      if(modelType==3 || modelType==4){
                                        a.hat.decision=log(a.hat.decision)
                                      }
                                      else if(modelType==5 || modelType==6 || modelType==7){
                                        a.hat.decision=(a.hat.decision^lambda-1)/lambda
                                      }
                                      else if(modelType==8 || modelType==9 || modelType==12){
                                        a.hat.decision= 1/a.hat.decision
                                      }
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
                                        #  a.U = (-1/a.b1)*matrix(c(1, aMu, 0, 0, aSigma, -1), nrow = 3, byrow = FALSE)
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
                                      #k=number of model parameters
                                      k=2
                                      y_residualsSq=c()
                                      x_residualsSq=c()
                                      for(i in 1:nrow(signalRespDF)){
                                        if(class(myLinFit)=="lm"){
                                          thisResid_y= signalRespDF$y[i] - myLinFit$fitted.values[i]
                                        }
                                        else{
                                          thisResid_y=  signalRespDF$y[i] - myLinFit$linear.predictors[i]
                                        }
                                        thisResid_x= signalRespDF$x[i]-mean(signalRespDF$x)
                                        y_residualsSq=c(y_residualsSq, thisResid_y^2)
                                        x_residualsSq=c(x_residualsSq, thisResid_x^2)
                                      }
                                      slopeStdError=summary(myLinFit)$table[2,2]
                                      interceptStdError=summary(myLinFit)$table[1,2]
                                      residualError=sqrt(sum(y_residualsSq)/(nrow(signalRespDF)-k-1))
                                      #copied from python code in PODv4.0---need to validate this
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
                                    }
                                  ))
