#options(warn=-1)
library(ggplot2) # gorgeous plots
library(gridExtra) # useful for plotting in grids
library(MASS) #contains boxcox and much more
#library(olsrr) #makes some nice plots to check assumptions
library(stats)
library(nlme) # contains gls = generalized least squares
library(pracma) #practical math...contains some functions from matlab
library(ggResidpanel)
library(carData)
library(car) # Need this for durbinWatsonTest
#library(tibble)
library(survival)
library(corrplot)
folderLocation=dirname(rstudioapi::getSourceEditorContext()$path)
source(paste(folderLocation, "/GenPODSignalResponeRObject.R", sep=""))
source(paste(folderLocation, "/SignalResponseMainAnalysisRObject.R", sep=""))
#data_obs = read.csv(paste(folderLocation,'/Plot_Data_50.csv',sep=""), header=TRUE, col.names=c("y","x"))
data_obs = read.csv(paste(folderLocation,"/dataFromPlots.csv",sep=""), header=TRUE)
data_obs=na.omit(data_obs)
begin<-proc.time()
#bc<-boxcox(data_obs$y~data_obs$x, plotit =FALSE)
lambda<-0
#data_obs$x=log(data_obs$x)
#lambda<-bc$x[which.max(bc$y)]
#data_obs$y<-(data_obs$y^lambda-1)/lambda
#data_obs$x=1/data_obs$x
#data_obs$y=data_obs$y3
#$y^1.11-1/1.11
#data_obs$event= c(2, 2, 1, 2, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0)
#data_obs$event= c(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 0, 1, 0, 0, 0, 0)
data_obs$event= c(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1)
#perform signal response analysis
#usually 40% of the highest signal response value
#newSRAnalysis<-AHatAnalysis$new(signalRespDF=data_obs,y_dec=1.145, modelType=6, lambda=lambda)
#newSRAnalysis$performTransforms()
#ahatvACensored<-newSRAnalysis$genAhatVersusACensored()
#newSRAnalysis$genAvaluesAndMatrix(ahatvACensored)
#newSRAnalysis$genPODCurve()
newSRAnalysis$executeAhatvsA()
linResults<-newSRAnalysis$getLinearModel()
results<-newSRAnalysis$getResults()
critPoints<-newSRAnalysis$getCritPts()
keyAValues<-newSRAnalysis$getKeyAValues()
slope<-newSRAnalysis$getModelSlope()
intercept<-newSRAnalysis$getModelIntercept()
testResults<-newSRAnalysis$getLinearTestResults()
r_squared<-newSRAnalysis$getRSquared()
stdErrors<-newSRAnalysis$getRegressionStdErrs()
covarMatrix<-newSRAnalysis$getCovarianceMatrix()
resDF<-newSRAnalysis$getResidualTable()
threshDF<-newSRAnalysis$getThresholdDF()
end<-proc.time()
print("total time:")
print(end-begin)
newSRAnalysis$plotSimdata(results)
newSRAnalysis$plotCI(results)

#newSRAnalysis$plotSimdata(linResults)
#residualError=c()
#for(i in 1:nrow(data_obs)){
#  thisError=data_obs$y[i]-mean(data_obs$y)
#  residualError=c(residualError, thisError^2)
#}
newSRAnalysis<-AHatAnalysis$new(signalRespDF=data_obs,y_dec=4.0, modelType=1, lambda=lambda)
slope<-newSRAnalysis$getModelSlope()



# full<-lm(y~x, data=data_obs)
# #constVector=rep(linearModel_lm$coefficients[[1]], nrow(data_obs))
# #partial<-lm(constVector~x, data= data_obs)
# #partial<-lm(1~x, data= data_obs)
# #partial$coefficients[[2]]=0
# #partial<-lm(y~poly(x, 2), data= data_obs)
# partial<-lm(y~1, data= data_obs)
# ANOVATable<-Anova(full, partial)
# pValue=1-ANOVATable$`Pr(>F)`[1]
# newSignalDF_Fit<-data.frame(x= data_obs$x, y=a.hat.vs.a.censored$linear.predictors)
# newCensoredLMObject<-lm(y~x, data=newSignalDF_Fit,na.action=na.omit)
# newCensoredLMObject$model$y=data_obs$y
# newResiduals=c()
# for(i in 1:length(newCensoredLMObject$fitted.values)){
#   residual=newCensoredLMObject$model$y[i]-newCensoredLMObject$fitted.values[i]
#   newResiduals=c(newResiduals, residual)
# }
# newCensoredLMObject$residuals=newResiduals
#boxCoxTEst<-boxcox(data_obs$y~data_obs$x)
#lambda <-boxCoxTEst$x[which.max(boxCoxTEst$y)]
