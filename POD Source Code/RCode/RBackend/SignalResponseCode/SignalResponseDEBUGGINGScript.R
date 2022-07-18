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

#perform signal response analysis
newSRAnalysis<-AHatAnalysis$new(SignalRespDF=data_obs,y_dec=5)
newSRAnalysis$executeAhatvsA()
linResults<-newSRAnalysis$getLinearModel()
results<-newSRAnalysis$getResults()
critPoints<-newSRAnalysis$getCritPts()
keyAValues<-newSRAnalysis$getKeyAValues()
slope<-newSRAnalysis$getModelSlope()
intercept<-newSRAnalysis$getModelIntercept()
testResults<-newSRAnalysis$getLinearTestResults()
covarMatrix<-newSRAnalysis$getCovarianceMatrix()
resDF<-newSRAnalysis$getResidualTable()
newSRAnalysis$plotPOD(results,critPoints)
newSRAnalysis$plotSimdata(linResults)
