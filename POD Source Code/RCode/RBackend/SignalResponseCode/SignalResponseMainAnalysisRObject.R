AHatAnalysis<-setRefClass("AHatAnalysis", fields = list(signalRespDF="data.frame", 
                                                                        y_dec="numeric", 
                                                                        modelType="numeric",
                                                                        lambda="numeric",
                                                                        varCovarMatrix="matrix",
                                                                        keyAValues="list",
                                                                        linearModel="data.frame",
                                                                        rSqaured="numeric",
                                                                        regressionStdErrs="list",
                                                                        linearTestResults="list",
                                                                        aVPOD="matrix",
                                                                        ResultsPOD="data.frame",
                                                                        critPts="data.frame",
                                                                        #copyoutputfrompython
                                                                        residualTable="data.frame",
                                                                        modelIntercept="numeric",
                                                                        modelSlope="numeric",
                                                                        thesholdTable="data.frame"
                                                                        ),
                                  methods=list(
                                    setLinearModel=function(psLMObject){
                                      linearModel<<-psLMObject
                                    },
                                    getLinearModel=function(){
                                      names(linearModel)[names(linearModel) == 'x'] <<- 'flaw'
                                      return(linearModel)
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
                                      #ResultsPOD$case=NULL
                                      #names(ResultsPOD)[names(ResultsPOD) == 'probabilities'] <- 'POD'
                                      ResultsPOD<<-data.frame(
                                        flaw= ResultsPOD$defect_sizes,
                                        pod = ResultsPOD$probabilities,
                                        confidence= ResultsPOD$defect_sizes_upCI
                                      )
                                      return(ResultsPOD)
                                    },
                                    setThresholdDF=function(psThreshDF){
                                      thesholdTable<<-psThreshDF
                                    },
                                    getThresholdDF=function(){
                                      return(thesholdTable)
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
                                    setKeyAValues=function(psKeyAValues){
                                      keyAValues<<-psKeyAValues
                                    },
                                    getKeyAValues=function(){
                                      return(keyAValues)
                                    },
                                    executeAhatvsA=function(){
                                      #print(signalResp)
                                      #perform necessary transforms
                                      performTransforms()
                                      # Fitting a linear model
                                      linearModel_lm <<- lm(y ~ x, data = signalRespDF, na.action=na.omit)
                                      setModelIntercept(linearModel_lm$coefficients[[1]])
                                      setModelSlope(linearModel_lm$coefficients[[2]])
                                      #set the linear model so it can be returned
                                      linearModDF=data.frame(
                                        #Index= 1:length(linearModel_lm$fitted.values),
                                        x=signalRespDF$x,
                                        fit=linearModel_lm$fitted.values,
                                        y=signalRespDF$y
                                        
                                      )
                                      setLinearModel(linearModDF)
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
                                      ahatvACensored<<-genAhatVersusACensored()
                                      #attach necessary attributes of the lm object to the survival censored object
                                      #ahatvACensored$rank=linearModel_lm$rank
                                      #ahatvACensored$qr=linearModel_lm$qr
                                      #update paramters if any points are censored
                                      if(0 %in% signalRespDF$event ||  2 %in% signalRespDF$event){
                                        setModelIntercept(ahatvACensored$coefficients[[1]])
                                        setModelSlope(ahatvACensored$coefficients[[2]])
                                        #set the linear model so it can be returned
                                        linearModDF=data.frame(
                                          #Index= 1:length(linearModel_lm$fitted.values),
                                          x=signalRespDF$x,
                                          fit=ahatvACensored$linear.predictors,
                                          y=signalRespDF$y
                                        )
                                        setLinearModel(linearModDF)
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
                                        #add residuals to the censored object
                                      }
                                      #get the value of R-squared(TODO: check if this changes with censored data)
                                      setRSquared(summary(linearModel_lm)$r.squared)
                                      #calculate the standard errors for regression
                                      calcStandardErrs(ahatvACensored)
                                      #find the key a values and the covariance matrix
                                      genAvaluesAndMatrix(ahatvACensored)
                                      #generates the thesholds table for UI
                                      genThresholdsTable(ahatvACensored)
                                      #calculate POD dataframe
                                      newPODSR=GenPODSignalResponse$new()
                                      #print(keyAValues)
                                      newPODSR$initialize(a.V_POD=aVPOD, aMu=keyAValues[[2]], aSigma=keyAValues[[4]])
                                      newPODSR$genPODCurve()
                                      setResults(newPODSR$getPODSR())
                                      setCritPts(newPODSR$getCriticalPoints())
                                      #peformTests
                                      linearTests(linearM=linearModel_lm)
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
                                      # Non-constant variance test... H_0: Constant variance VS H_a: Non-constant variance
                                      # We fail to reject H_0, so non-constant variance isn't a problem. 
                                      nonConst=ncvTest(linearM)
                                      # Test for auto-correlation (ie, a pattern in the data due to "time," or in this case, order)
                                      # Data does not have auto-correlation. 
                                      durbinWTest=durbinWatsonTest(linearM)
                                      setLinearTestResults(list(shapiro,nonConst,durbinWTest))
                                    },
                                    genAhatVersusACensored=function(){
                                      # It expects a censored object; this data is not censored but we can still use it.
                                      # Note, we are using the Box Cox transformed y and f_a (log) transformed x
                                      censored.a.hat <- Surv(time = signalRespDF$y.trans, time2 = signalRespDF$event, event=signalRespDF$event, type="interval")#, type = "interval2")
                                      #censored.a.hat <- Surv(time = signalRespDF$y, time2 = signalRespDF$y, type = "interval2")
                                      a.hat.censor.df <- data.frame(censored.a.hat, x = f_a(signalRespDF$x))
                                      # Here's the linear model.
                                      a.hat.vs.a.censored <- survreg(formula = censored.a.hat ~ x,
                                                                     dist = "gaussian", data = a.hat.censor.df)
                                      a.hat.vs.a.censored_Null <<- survreg(formula = censored.a.hat ~ 0+as.factor(x),
                                                                     dist = "gaussian", data = a.hat.censor.df)
                                      #a.hat.vs.a.censored <-survreg(formula= Surv(y.trans, event)~x, dist= "gaussian", data= signalRespDF)
                                      return(a.hat.vs.a.censored)
                                    },
                                    genAvaluesAndMatrix=function(a.hat.vs.a.censored){
                                      a.hat.decision = y_dec 
                                      if(modelType==3 || modelType==4){
                                        a.hat.decision=log(a.hat.decision)
                                      }
                                      else if(modelType==5){
                                        a.hat.decision=(a.hat.decision^lambda-1)/lambda
                                      }
                                      a.b0 <- as.numeric(a.hat.vs.a.censored$coef[1])
                                      a.b1 <- as.numeric(a.hat.vs.a.censored$coef[2])
                                      a.tau <- as.numeric(a.hat.vs.a.censored$scale) # random sigma
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
                                                                         v11=varCovarMatrix[[1]],  
                                                                         v12=varCovarMatrix[1,2],
                                                                         v22=varCovarMatrix[2,2]))
                                      }
                                      setThresholdDF(threshDataFrame)
                                    },
                                    calcStandardErrs=function(myLinFit){
                                      df=2
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
                                      #global<<-y_residualsSq
                                      slopeStdError=summary(myLinFit)$table[2,2]
                                      interceptStdError=summary(myLinFit)$table[1,2]
                                      residualError=sqrt(sum(y_residualsSq)/(nrow(signalRespDF)-df))
                                      #stored as: slope std error, slope intercept std error, and residual error
                                      stdErrors=list(slopeStdError, interceptStdError, residualError)
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
                                        ggtitle(paste("POD Curve"))#+scale_x_continuous(limits = c(0,1.0))+scale_y_continuous(limits = c(0,1))
                                      print(myPlot)
                                    },
                                    #used for Debugging ONLY
                                    plotSimdata=function(df){
                                      myPlot=ggplot(data=df, mapping=aes(x=flaw, y=y))+geom_point()
                                      print(myPlot)
                                    }
                                  ))
