#this class is used for logit approximation when dealing with hitMiss Data
LogitApproximation <- setRefClass("LogitApproximation", fields = list(inputDataFrameLogistic="data.frame", LogisticRegressionDataFrame="glm", 
                                                                      CIType="character"), methods = list(
                                                #does not return useful in c# used for internal testing only                        
                                                getLogitList=function(calcConfInt){
                                                  return(LogisticRegressionDataFrame)
                                                },
                                                #used to get the value of muhat
                                                getMuHat=function(){
                                                  return(LogisticRegressionDataFrame$muhat)
                                                },
                                                #used to get the value of sighat
                                                getMuHat=function(){
                                                  return(LogisticRegressionDataFrame$sighat)
                                                },
                                                #generates the returns the residual fit dataframe
                                                #the elements are index, cracksize, HitOrMiss,  logitFit, diff, ConfInt
                                                getLogitFitTable=function(calcConfInt){
                                                  #assign local var from field
                                                  df<-inputDataFrameLogistic[,c("Index","x","y")]
                                                  #rename x
                                                  names(df)[names(df) == 'x'] <- 'crackSize'
                                                  #rename y
                                                  names(df)[names(df) == 'y'] <- 'HitRate'
                                                  #append logitFit
                                                  #LogitRegDataFame<-rbind(df, LogisticRegressionDataFrame$fitted.values)
                                                  #calculate confidence interval
                                                  ptm <- proc.time()
                                                  confIntListDouble=calcConfInt()
                                                  print("Time of Calculation")
                                                  print(proc.time() - ptm)
                                                  #create a dataframe with the fitted values, diff, and confInt
                                                  #TODO: ask Christie about using Diff
                                                  if(CIType=="Standard Wald"){
                                                    vdCIDataframe=cbind(as.numeric(LogisticRegressionDataFrame$fitted.values), 
                                                                        confIntListDouble)
                                                    LogitRegDataFame<-cbind(df, vdCIDataframe)
                                                    names(LogitRegDataFame)[names(LogitRegDataFame) == 'as.numeric(LogisticRegressionDataFrame$fitted.values)'] <- 't_trans'
                                                    names(LogitRegDataFame)[names(LogitRegDataFame) == 'cInterval'] <- 'Confidence_Interval'
                                                    return(LogitRegDataFame)
                                                  }
                                                  else if(CIType=="LR" || CIType=="MLR"){
                                                    LogitRegDataFame=confIntListDouble
                                                    #rename x
                                                    names(LogitRegDataFame)[names(LogitRegDataFame) == 'simCracks'] <- 'crackSize'
                                                    #rename y
                                                    names(LogitRegDataFame)[names(LogitRegDataFame) == 'estimate'] <- 't_trans'
                                                    names(LogitRegDataFame)[names(LogitRegDataFame) == 'lower'] <- 'Confidence_Interval'
                                                    return(LogitRegDataFame)
                                                  }
                                                  
                                                },
                                                #function used to get the results for logit and assign the dataframe
                                                calcLogitResults=function(hitMissDF){
                                                  #separated dataset (if the model fails, it will become 1 and warn the user)
                                                  #LReg_test$converged is also boolean
                                                  separated<-0
                                                  options(warn=2)
                                                  #transform the column names to x,y
                                                  y<-colnames(inputDataFrameLogistic[3])
                                                  x<-colnames(inputDataFrameLogistic[2])
                                                  #tries to fit a genralized linear model to the dataframe
                                                  LReg_test.mod<-try(glm(formula = y ~ x, data=hitMissDF, family=binomial), FALSE)
                                                  failed<-inherits(LReg_test.mod, "try-error")
                                                  
                                                  if(failed || !LReg_test.mod$converged){
                                                    #distributes a warning about wacky data (WORK ON THIS LATER)
                                                    separated<-1
                                                  }
                                                  #Build whatever model is possible
                                                  options(warn=0)
                                                  LReg.mod <- glm(formula= y ~ x, data=hitMissDF, family=binomial)
                                                  
                                                  newGenNormInstance=new("GenNormFit",cracks= hitMissDF$x,sampleSize=nrow(inputDataFrameLogistic),
                                                                         Ndata=hitMissDF$a_x_n, simCrackSizesArray=0.0)
                                                  newGenNormInstance$genNormalFit()
                                                  a=newGenNormInstance$getSimCrackSizesArray()
                                                  #Log odds value at 90%
                                                  LOd=log(0.9/(1-0.9))
                                                  # Calculate a90: flaw size at 90% probability
                                                  LReg.mod$a90 = unname((LOd-LReg.mod$coefficients[1])/LReg.mod$coefficients[2])
                                                  LReg.mod$a25 = unname((log(0.25/(1-0.25))-LReg.mod$coefficients[1])/LReg.mod$coefficients[2])
                                                  LReg.mod$a50 = unname((-LReg.mod$coefficients[1])/LReg.mod$coefficients[2]) # muhat
                                                  
                                                  # Predict the fitted values and the standard errors for the model 
                                                  LReg.predictions=predict(LReg.mod, type="link",se.fit=TRUE)
                                                  
                                                  # CHECK WITH CHRISTIE IF THIS IS CORRECT SIGMAHAT
                                                  LReg.mod$muhat    = LReg.mod$a50
                                                  LReg.mod$sigmahat = predict(LReg.mod, newdata=data.frame(x=LReg.mod$a90), type="response",se.fit=TRUE)$se.fit
                                                  #write to initialization variable to get returned to c#
                                                  LogisticRegressionDataFrame <<- LReg.mod
                                                  #return(LogisticRegressionDataFrame)
                                                },
                                                #calculates the confidence interval and returns the values as a numeric double
                                                calcConfInt=function(){
                                                  #inialize the confidence inteval class
                                                  newConfIntCalc<- new("ConfidenceIntervals", LogisticRegressionResult=LogisticRegressionDataFrame, 
                                                                       CIType=CIType, CIListDouble=list(), CIDataFrame=data.frame(c(1)))
                                                  #arguements will need to be added for other confInt types
                                                  newConfIntCalc$determineCIType(newConfIntCalc$executeStandardWald(), newConfIntCalc$executeLR())
                                                  return(newConfIntCalc$getConfidenceInterval())
                                                },
                                                #prints out the logistic regression chart
                                                #It's used for debugging only
                                                plotLogisticData=function(LReg.mod){
                                                  myPlot=ggplot(data=LReg.mod, mapping=aes(x=LReg.mod$data$x, y=LReg.mod$fitted.values))+geom_point()
                                                  print(myPlot)
                                                },
                                                plotSimdata=function(df){
                                                  myPlot=ggplot(data=df, mapping=aes(x=crackSize, y=t_trans))+geom_point()+
                                                    ggtitle("POD Curve:", CIType)
                                                  print(myPlot)
                                                },
                                                plotCI=function(df){
                                                  myPlot=ggplot(data=df, mapping=aes(x=crackSize, y=Confidence_Interval))+geom_point()+
                                                    ggtitle("Confidence interval:", CIType)
                                                  print(myPlot)
                                                }
                                              ))



library(methods)
#dependencies from original R code
library("RSSampling")
library(MASS)
#library(gdata)
library(pracma)
library(mcprofile) ## MLR and LR
library(glmnet)
library(logistf)
# #main<-function(){
#import the confint class in order to return the confidence intveral value for a given crack size
#get the working directory, used for debugging only
  codeLocation="C:/Users/gohmancm/Desktop/PODv4Point5FullProjectFolder/RCode/OldRBackend"
  source(paste(codeLocation,"/ConfidenceintervalsClassR.R",sep=""))
  source(paste(codeLocation,"/LinearComboGeneratorClassR.R",sep=""))
  source(paste(codeLocation,"/genNormFitClassR.R",sep=""))
  hitMissDF <- read.csv("C:/Users/gohmancm/Desktop/PODv4Point5FullProjectFolder/RCode/RBackend/HitMissData_Good_1TestSet.csv")
  #we will keep this in the global environment
  LogitApproximation<-new("LogitApproximation", inputDataFrameLogistic=hitMissDF, LogisticRegressionDataFrame=glm(formula = y ~ x, data=hitMissDF, family=binomial),
                          CIType="LR")
  LogitApproximation$calcLogitResults(LogitApproximation$inputDataFrameLogistic)
  LogitApproximation$plotLogisticData(LogitApproximation$getLogitList())
  writeToReturnDataFrame<-LogitApproximation$getLogitList()
  newDF<-LogitApproximation$getLogitFitTable(LogitApproximation$calcConfInt())
  #plot confidence interval
  LogitApproximation$plotSimdata(newDF)
  LogitApproximation$plotCI(newDF)

#  return(newDF)
#}
  # d <- data.frame(x=c(1,2,1,3),
  #                 y1=c(3,2,2,5),
  #                 y2=c(5,4,2,5))
  # library(ggplot2) 
  # library(reshape2) ## for melt()
  # dm  <- melt(d,id.var=1)
  # ggplot(data=dm,aes(x,value,colour=variable))+
  #   geom_point(alpha=0.2)+
  #   scale_colour_manual(values=c("red","blue"))+
  #   labs(x="games",y="variance")

