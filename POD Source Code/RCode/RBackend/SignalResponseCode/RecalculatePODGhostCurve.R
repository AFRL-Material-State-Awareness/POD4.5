

RecalcOriginalPOD<- setRefClass("RecalcOriginalPOD", fields = list(signalRespDFFull="data.frame",
                                                                   y_dec="numeric",
                                                                   mu="numeric",
                                                                   modelType="numeric",
                                                                   lambda="numeric",
                                                                   sigma="numeric",
                                                                   varCovarMatrix="matrix",
                                                                   aVPOD="matrix",
                                                                   PODCurveAll="data.frame",
                                                                   originalData="data.frame"),
                                methods=list(
                                  setPODCurveAll=function(setPODAll){
                                    PODCurveAll<<-setPODAll
                                  },
                                  getPODCurveAll=function(){
                                    resultsPOD<-data.frame(
                                      flaw= PODCurveAll$defect_sizes,
                                      pod = PODCurveAll$probabilities,
                                      confidence= PODCurveAll$defect_sizes_upCI
                                    )
                                    return(resultsPOD)
                                  },
                                  prepareData=function(){
                                    testInstance<-PrepareData$new(signalRespDF=signalRespDFFull)
                                    testInstance$getOrigDataframe()
                                    testInstance$createAvgRespDF()
                                    signalRespDFFull<<-testInstance$createAvgRespDF()
                                  },
                                  recalcPOD=function(){
                                    #preprocess the input if there's more than one inspector
                                    originalData<<-signalRespDFFull
                                    prepareData()
                                    #perform necessary transforms
                                    #performTransforms()
                                    # Fitting a linear model
                                    linearModel_lm <- lm(y ~ x, data = signalRespDFFull, na.action=na.omit)
                                    #set the linear model so it can be returned
                                    linearModDF=data.frame(
                                      #Index= 1:length(linearModel_lm$fitted.values),
                                      x=signalRespDFFull$x,
                                      fit=linearModel_lm$fitted.values
                                    )
                                    #append the responses after the fitted value
                                    responsesAll<-subset(originalData, select = -c(Index, x, event))
                                    linearModDF<-cbind(linearModDF, responsesAll)
                                    #generate ahat versus acensored
                                    ahatvACensored<-genAhatVersusACensored()
                                    #update paramters if any points are censored
                                    if(0 %in% signalRespDFFull$event ||  2 %in% signalRespDFFull$event){
                                      #set the linear model so it can be returned
                                      linearModDF=data.frame(
                                        x=signalRespDFFull$x,
                                        fit=ahatvACensored$linear.predictors
                                      )
                                      #append the responses after the fitted value
                                      responsesAll<-subset(originalData, select = -c(Index, x, event))
                                      linearModDF<-cbind(linearModDF, responsesAll)
                                    }
                                    #find the key a values and the covariance matrix
                                    genAvaluesAndMatrix(ahatvACensored)
                                    #make POD curve
                                    genPODCurve()
                                    
                                  },
                                  genAhatVersusACensored=function(){
                                    # It expects a censored object; this data is not censored but we can still use it.
                                    # Note, we are using the Box Cox transformed y and f_a (log) transformed x
                                    censored.a.hat <- Surv(time = signalRespDFFull$y, time2 = signalRespDFFull$event, event=signalRespDFFull$event, type="interval")
                                    a.hat.censor.df <- data.frame(censored.a.hat, x = signalRespDFFull$x)
                                    # Here's the linear model.
                                    a.hat.vs.a.censored <- survreg(formula = censored.a.hat ~ x,
                                                                   dist = "gaussian", data = a.hat.censor.df)
                                    return(a.hat.vs.a.censored)
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
                                    a.covariance.matrix <- a.hat.vs.a.censored$var
                                    varCovarMatrix<<-a.covariance.matrix
                                    aMu <- (a.hat.decision - a.b0)/a.b1
                                    aSigma <- a.tau/a.b1
                                    POD.transition.matrix <- matrix(c(1, aMu, 0, 0, aSigma, -a.tau), nrow = 3, byrow = FALSE)
                                    a.VCV <- (-1/a.b1)^2 * t(POD.transition.matrix)
                                    a50 <- aMu
                                    z90 <- qnorm(0.9)
                                    a.U = (-1/a.b1)*matrix(c(1, aMu, 0, 0, aSigma, -a.tau), nrow = 3, byrow = FALSE)
                                    a.V_POD = t(a.U)%*%varCovarMatrix%*%a.U
                                    SD.a.90 = sqrt(a.V_POD[1,1]+2*z90*a.V_POD[1,2]+(z90^2)*a.V_POD[2,2])
                                    #set parameters
                                    aVPOD<<-a.V_POD
                                    # assign mu and sigma values
                                    mu <<- a50
                                    sigma <<- aSigma
                                  },
                                  genPODCurve=function(){
                                    #calculate POD dataframe
                                    newPODSR=GenPODSignalResponse$new()
                                    newPODSR$initialize(a.V_POD=aVPOD, aMu=mu, aSigma=sigma)
                                    newPODSR$genPODCurve()
                                    setPODCurveAll(newPODSR$getPODSR())
                                  }
                                )
                                                                   
                    )

#### Used for testing
#used for Debugging ONLY
plotSimdata=function(df){
  myPlot=ggplot(data=df, mapping=aes(x=flaw, y=pod))+geom_point()+
    ggtitle(paste("POD Curve"))#+scale_x_continuous(limits = c(0,1.0))+scale_y_continuous(limits = c(0,1))
  print(myPlot)
}
#used for Debugging ONLY
plotCI=function(df){
  myPlot=ggplot(data=df, mapping=aes(x=flaw, y=confidence))+geom_point()+
    ggtitle(paste("Confidence interval Curve"))#+scale_x_continuous(limits = c(0,1.0))+scale_y_continuous(limits = c(0,1))
  print(myPlot)
}

loopAmount=20
avgTime=c()
for (i in 1:loopAmount){
  start.time <- Sys.time()
  data_obs = read.csv(paste("C:/Users/gohmancm/Desktop/PODv4.5ExampleDataRepo/PODv4.5ExampleDatasets/aHat/dataFromPlots.csv",sep=""), header=TRUE)

  data_obs=data.frame(
    Index=data_obs$Index,
    x=data_obs$Length,
    y=data_obs$A11,
    event=rep(1, nrow(data_obs))
  )
  data_obs$x=log(data_obs$x)
  data_obs$event=c( 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1)
  lambda=0
  newSRAnalysis<-RecalcOriginalPOD$new(signalRespDFFull=data_obs,y_dec=5, modelType=1, lambda=lambda)
  newSRAnalysis$recalcPOD()
  PODCurveAll<<-newSRAnalysis$getPODCurveAll()
  end.time <- Sys.time()
  time.taken <- end.time - start.time

  avgTime=c(avgTime, time.taken)
}
avg=sum(avgTime)/loopAmount
avg
PODCurveAll$flaw=exp(PODCurveAll$flaw)
plotSimdata(PODCurveAll)