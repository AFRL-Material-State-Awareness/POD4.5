AHatAnalysis<-setRefClass("AHatAnalysis", fields = list(SignalRespDF="data.frame", 
                                                                        y_dec="numeric", 
                                                                        modelType="character",
                                                                        varCovarMatrix="matrix",
                                                                        keyAValues="list",
                                                                        linearModel="data.frame",
                                                                        linearTestResults="list",
                                                                        aVPOD="matrix",
                                                                        ResultsPOD="data.frame",
                                                                        critPts="data.frame",
                                                                        #copyoutputfrompython
                                                                        residualTable="data.frame",
                                                                        modelIntercept="numeric",
                                                                        modelSlope="numeric"
                                                                        ),
                                  methods=list(
                                    setLinearModel=function(psLMObject){
                                      linearModel<<-psLMObject
                                    },
                                    getLinearModel=function(){
                                      names(linearModel)[names(linearModel) == 'x'] <- 'flaw'
                                      return(linearModel)
                                    },
                                    setResidualTable=function(psResidTable){
                                      residualTable<<-psResidTable
                                    },
                                    getResidualTable=function(){
                                      names(residualTable)[names(residualTable) == 'x'] <- 'flaw'
                                      return(residualTable)
                                    },
                                    setResults=function(psResults){
                                      ResultsPOD<<-psResults
                                    },
                                    getResults=function(){
                                      names(ResultsPOD)[names(ResultsPOD) == 'x'] <- 'flaw'
                                      return(ResultsPOD)
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
                                      #perform necessary transforms
                                      performTransforms()
                                      # Fitting a linear model
                                      linearModel_lm <<- lm(y ~ x, data = SignalRespDF, na.action=na.omit)
                                      setModelIntercept(linearModel_lm$coefficients[[1]])
                                      setModelSlope(linearModel_lm$coefficients[[2]])
                                      #set the linear model so it can be returned
                                      linearModDF=data.frame(
                                        #Index= 1:length(linearModel_lm$fitted.values),
                                        x=SignalRespDF$x,
                                        y=SignalRespDF$y,
                                        fit=linearModel_lm$fitted.values
                                      )
                                      setLinearModel(linearModDF)
                                      ##create the residual dataframe
                                      ResidualDF=cbind(linearModDF, linearModel_lm$residuals)
                                      names(ResidualDF)[names(ResidualDF) == 'linearModel_lm$residuals'] <- 't_diff'
                                      setResidualTable(ResidualDF)
                                      #peformTests
                                      linearTests(linearM=linearModel_lm)
                                      #generate ahat versus acensored
                                      ahatvACensored=genAhatVersusACensored()
                                      #find the key a values and the covariance matrix
                                      genAvaluesAndMatrix(ahatvACensored)
                                      #calculate POD dataframe
                                      newPODSR=GenPODSignalResponse$new()
                                      newPODSR$initialize(a.V_POD=aVPOD, aMu=keyAValues[[1]], aSigma=keyAValues[[3]])
                                      newPODSR$genPODCurve()
                                      setResults(newPODSR$getPODSR())
                                      setCritPts(newPODSR$getCriticalPoints())
                                    },
                                    performTransforms=function(){
                                      SignalRespDF$x.trans<<-f_a(SignalRespDF$x)
                                      SignalRespDF$y.trans<<-f_a(SignalRespDF$y)
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
                                      censored.a.hat <- Surv(time = SignalRespDF$y.trans, time2 = SignalRespDF$y.trans, type = "interval2")
                                      #censored.a.hat <- Surv(time = SignalRespDF$y, time2 = SignalRespDF$y, type = "interval2")
                                      a.hat.censor.df <- data.frame(censored.a.hat, x = f_a(SignalRespDF$x))
                                      # Here's the linear model. 
                                      a.hat.vs.a.censored <- survreg(formula = censored.a.hat ~ x, 
                                                                     dist = "gaussian", data = a.hat.censor.df)
                                      return(a.hat.vs.a.censored)
                                    },
                                    genAvaluesAndMatrix=function(a.hat.vs.a.censored){
                                      a.hat.decision = y_dec # = 200
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
                                    # Inverse function of f_a. Uncomment 1
                                    f_a_i=function(a){ return(a) # exp(a)#log(a) #sqrt(a)
                                    },
                                    # Function transforming the flaw size. Uncomment 1
                                    f_a=function(a){return(a) #log(a) #exp(a) #a^2
                                    },
                                    #USED FOR DEBUGGING ONLY
                                    plotPOD=function(plotPoints, criticalPoints){
                                      # POD Plot  
                                      ggplot()+theme_bw()+
                                        geom_line(aes(plotPoints$defect_sizes,plotPoints$probabilities),colour="blue",size=1.5)+
                                        geom_line(aes(plotPoints$defect_sizes_upCI,plotPoints$probabilities),linetype=2,colour="darkcyan",size=1)+
                                        #    geom_point(aes(criticalPoints$a_50_50,0.5),colour=index,shape=1)+
                                        geom_hline(yintercept=0.9, colour="red")+
                                        geom_vline(xintercept=criticalPoints$a_90_50, colour="red",linetype=2)+
                                        geom_vline(xintercept=criticalPoints$a_90_95, colour="darkturquoise",linetype=2)+
                                        geom_point(aes(criticalPoints$a_90_50,0.9),shape=18,size=2, colour="red")+
                                        geom_point(aes(criticalPoints$a_90_95,0.9),shape=18,size=2, colour="darkturquoise")+
                                        xlim(min(na.exclude(plotPoints$defect_sizes)),max(na.exclude(plotPoints$defect_sizes)))+
                                        ylab("Probability of Detection, POD(a)")+
                                        xlab("Defect Size, a")
                                    },
                                    #used for Debugging ONLY
                                    plotSimdata=function(df){
                                      myPlot=ggplot(data=df, mapping=aes(x=x, y=y))+geom_point()
                                      print(myPlot)
                                    }
                                  ))
