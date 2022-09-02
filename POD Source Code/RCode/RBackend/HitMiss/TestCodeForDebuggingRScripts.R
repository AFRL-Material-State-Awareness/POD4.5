# #Test Code
#flag used for genNormFitClassR
#isLog=FALSE
#import necessary libraries
library(methods)
#dependencies from original R code
library(MASS)
library(ggplot2)
library(mcprofile) ## MLR and LR
library(logistf)
library(splines)
library(parallel)
library(roxygen2)
#hitMissDF <- read.csv("C:/Users/gohmancm/Desktop/PODv4Point5FullProjectFolder/PODv4Point5Attemp1/PODv4/POD Source Code/RCode/RBackend/HitMiss/HitMissData_Bad_2.csv")
hitMissDF<-read.csv("C:/Users/gohmancm/Desktop/RSS/HitMissData_Good.csv")
#hitMissDF<-read.csv("C:/Users/gohmancm/Desktop/RSS/HitMissData_Bad.csv")
#hitMissDF<-read.csv("C:/Users/gohmancm/Desktop/newPODrepository/HitMiss/HitMissData_Bad.csv")
#hitMissDF$x<-1/hitMissDF$x
hitMissDF$y.1=NULL
#hitMissDF<-read.csv("C:/Users/colin/Desktop/HitMissResults_Good_1.csv")
#hitMissDF<-read.csv("C:/Users/gohmancm/Desktop/PODv4Point5FullProjectFolder/PODv4Point5Attemp1/PODv4/POD Source Code/RCode/RBackend/HitMiss/HitMissInfo_BadLL.csv")
#hitMissDF <- read.csv("C:/Users/gohmancm/Desktop/PODv4Point5FullProjectFolder/RCode/RBackend/HitMissData_Good_1TestSet.csv")
#get the working directory, used for debugging only
codeLocation=dirname(rstudioapi::getSourceEditorContext()$path)
source(paste(codeLocation,"/RSSComponentsObject.R",sep=""))
source(paste(codeLocation,"/RankedSetSamplingMainRObject.R",sep=""))
source(paste(codeLocation,"/HitMissMainAnalysisRObject.R",sep=""))
source(paste(codeLocation,"/WaldCI_RObject.R",sep=""))
source(paste(codeLocation,"/HMLogitApproximationRObject.R",sep=""))
source(paste(codeLocation,"/genNormFitClassR.R",sep=""))
source(paste(codeLocation,"/LinearComboGeneratorClassR.R",sep=""))
source(paste(codeLocation,"/LRConfIntRObject.R",sep=""))
source(paste(codeLocation,"/MLRConfIntRObject.R",sep=""))
source(paste(codeLocation,"/GenAValuesOnPODCurveRObject.R",sep=""))
source(paste(codeLocation,"/HMFirthApproximationRObject.R",sep=""))
source(paste(codeLocation,"/miniMcprofile.R",sep=""))
CItype0="LR"
#type="Firth Logistic Regression"
type="Logistic Regression"
begin=Sys.time()
#for(i in 1:10){
  newAnalysis<-HMAnalysis$new(hitMissDF=hitMissDF, modelType=type, CIType=CItype0, N=nrow(hitMissDF), normSampleAmount=500)
  newAnalysis$detAnalysisApproach()
  results<-newAnalysis$getResults()
  residTab<-newAnalysis$getResidualTable()
  aValues<-newAnalysis$getKeyAValues()
  orig<-newAnalysis$getHitMissDF()
  covarianceMatrix<-newAnalysis$getCovMatrix()
  goodnessOfFit<-newAnalysis$getGoodnessOfFit()
  iterationTable<-newAnalysis$getIterationTable()
  separatedQuestion<-newAnalysis$getSeparation()
  failedtoConverge<-newAnalysis$getConvergedFail()
  newAnalysis$plotSimdata(results)
  newAnalysis$plotCI(results)
#}
end=Sys.time()
print("execution time")
print(end-begin)
# if(CItype0== "Standard Wald"){
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
#debugging
#$plotSimdata(results)
#newAnalysis$plotCI(results)


# CITypeList=list("Standard Wald", "Modified Wald", "LR", "MLR")
# ###loops through all the CIs and returns the values
# for(i in CITypeList){
#   newAnalysis<-HMAnalysis$new(hitMissDF=hitMissDF, CIType=i, N=nrow(hitMissDF), normSampleAmount=250)
#   newAnalysis$detAnalysisApproach()
#   results<-newAnalysis$getResults()
#   aValues<-newAnalysis$getKeyAValues()
#   if(i== "Standard Wald"){
#     stdWald=data.frame(
#       a9095Wald=as.numeric(aValues)
#     )
#    
#   }
#   if(i== "Modified Wald"){
#     MWald=data.frame(
#       a9095MWald=as.numeric(aValues)
#       )
#   }
#   if(i== "LR"){
#     LR=data.frame(
#       a9095LR=as.numeric(aValues)
#     )
#   }
#   if(i== "MLR"){
#     MLR=data.frame(
#       a9095MLR=as.numeric(aValues)
#     )
#   }
#   #debugging
#   newAnalysis$plotSimdata(results)
#   newAnalysis$plotCI(results)
# }
# resultsDFDeBUG<-cbind(stdWald,MWald, LR, MLR )

for(i in 1:detectCores()){
  ptm <- proc.time()
  makeCluster(i)
  print("core time")
  print(proc.time() - ptm)
}



