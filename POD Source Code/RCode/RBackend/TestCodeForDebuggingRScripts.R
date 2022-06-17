# #Test Code
#flag used for genNormFitClassR
isLog=FALSE
#import necessary libraries
library(methods)
#dependencies from original R code
library("RSSampling")
library(MASS)
#library(gdata)
#library(pracma)
library(mcprofile) ## MLR and LR
library(glmnet)
library(logistf)
hitMissDF <- read.csv("C:/Users/gohmancm/Desktop/PODv4Point5FullProjectFolder/RCode/RBackend/HitMissData_Bad.csv")
#hitMissDF <- read.csv("C:/Users/gohmancm/Desktop/PODv4Point5FullProjectFolder/RCode/RBackend/HitMissData_Good_1TestSet.csv")
#get the working directory, used for debugging only
codeLocation="C:/Users/gohmancm/Desktop/PODv4Point5FullProjectFolder/RCode/RBackend"
source(paste(codeLocation,"/HitMissMainAnalysisRObject.R",sep=""))
source(paste(codeLocation,"/WaldCI_RObject.R",sep=""))
source(paste(codeLocation,"/HMLogitApproximationRObject.R",sep=""))
source(paste(codeLocation,"/genNormFitClassR.R",sep=""))
source(paste(codeLocation,"/LinearComboGeneratorClassR.R",sep=""))
source(paste(codeLocation,"/LRConfIntRObject.R",sep=""))
source(paste(codeLocation,"/MLRConfIntRObject.R",sep=""))
source(paste(codeLocation,"/GenAValuesOnPODCurveRObject.R",sep=""))

# CItype0="MLR"
# # # for(i in CITypeList){
# newAnalysis<-HMAnalysis$new(hitMissDF=hitMissDF, CIType=CItype0, N=nrow(hitMissDF), normSampleAmount=500)
# newAnalysis$detAnalysisApproach()
# results<-newAnalysis$getResults()
# aValues<-newAnalysis$getKeyAValues()
# # if(CItype0== "Standard Wald"){
#   a9095Wald=aValues[[5]]
# }
# if(CItype0== "Modified Wald"){
#   a9095MWald=aValues[[5]]
# }
# if(CItype0== "LR"){
#   a9095LR=aValues[[5]]
# }
# if(CItype0== "MLR"){
#   a9095MLR=aValues[[5]]
# }
# #debugging
# newAnalysis$plotSimdata(results)
# newAnalysis$plotCI(results)


CITypeList=list("Standard Wald", "Modified Wald", "LR", "MLR")
###loops through all the CIs and returns the values
for(i in CITypeList){
  newAnalysis<-HMAnalysis$new(hitMissDF=hitMissDF, CIType=i, N=nrow(hitMissDF), normSampleAmount=250)
  newAnalysis$detAnalysisApproach()
  results<-newAnalysis$getResults()
  aValues<-newAnalysis$getKeyAValues()
  if(i== "Standard Wald"){
    stdWald=data.frame(
      a9095Wald=as.numeric(aValues)
    )
   
  }
  if(i== "Modified Wald"){
    MWald=data.frame(
      a9095MWald=as.numeric(aValues)
      )
  }
  if(i== "LR"){
    LR=data.frame(
      a9095LR=as.numeric(aValues)
    )
  }
  if(i== "MLR"){
    MLR=data.frame(
      a9095MLR=as.numeric(aValues)
    )
  }
  #debugging
  newAnalysis$plotSimdata(results)
  newAnalysis$plotCI(results)
}
resultsDFDeBUG<-cbind(stdWald,MWald, LR, MLR )





