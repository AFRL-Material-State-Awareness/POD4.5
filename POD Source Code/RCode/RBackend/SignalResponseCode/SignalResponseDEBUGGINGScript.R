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
#data_obs$y=log(data_obs$y)
#data_obs$event= c(2, 2, 1, 2, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0)
data_obs$event= c(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 0, 1, 0, 0, 0, 0)
#data_obs$event= c(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1)
#perform signal response analysis
#usually 40% of the highest signal response value
newSRAnalysis<-AHatAnalysis$new(signalRespDF=data_obs,y_dec=1.145, modelType=1)
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
print(proc.time()-begin)
newSRAnalysis$plotSimdata(results)
newSRAnalysis$plotCI(results)
#newSRAnalysis$plotSimdata(linResults)
#residualError=c()
#for(i in 1:nrow(data_obs)){
#  thisError=data_obs$y[i]-mean(data_obs$y)
#  residualError=c(residualError, thisError^2)
#}

