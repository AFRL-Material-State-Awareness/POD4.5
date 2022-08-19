library(ggplot2)
library(reticulate)
library(MASS)
library(mcprofile)
library(parallel)
library(logistf)
# #Test Code
#flag used for genNormFitClassR
isLog=FALSE
#Sys.which("python")
#use_python('C:/ProgramData/Anaconda3')
use_condaenv('C:/ProgramData/Anaconda3')
filepath=dirname(rstudioapi::getSourceEditorContext()$path)
source_python(paste(filepath,"/../PythonRankedSetSampling/RSSRowSorter.py", sep=""))
source_python(paste(filepath,"/../PythonRankedSetSampling/CyclesArrayGenerator.py", sep=""))
source_python(paste(filepath,"/../PythonRankedSetSampling/MainRSSamplingClass.py", sep=""))
source_python(paste(filepath,"/../PythonRankedSetSampling/RSS2DArrayGenerator.py", sep=""))
#rscripts
source(paste(filepath,"/RankedSetSamplingMainRObject.R", sep = ""))
source(paste(filepath,"/MainRSSamplingDataInR.R", sep = ""))
source(paste(filepath,"/RSSComponentsObject.R", sep = ""))
source(paste(filepath,"/RankedSetRegGen.R", sep = ""))
source(paste(filepath,"/HMLogitApproximationRObject.R", sep = ""))
source(paste(filepath,"/HMFirthApproximationRObject.R", sep = ""))
source(paste(filepath,"/GenPODCurveRSS.R", sep = ""))
source(paste(filepath,"/GenRSS_A_Values.R", sep = ""))
source(paste(filepath,"/WaldCI_RObject.R", sep = ""))
source(paste(filepath,"/HitMissMainAnalysisRObject.R", sep = ""))
source(paste(filepath,"/GenNormFitClassR.R", sep = ""))
source(paste(filepath,"/GenAValuesOnPODCurveRObject.R", sep = ""))
source(paste(filepath,"/LRConfIntRObject.R", sep = ""))
source(paste(filepath,"/MLRConfIntRObject.R", sep = ""))
#testData<-read.csv("C:/Users/gohmancm/Desktop/PODv4Point5FullProjectFolder/CSharpBackendTempSolution/CSharpBackendTempSolutionForPOD4Point5/RCode/RBackend/HitMissData_Good.csv")
#testData<-read.csv("C:/Users/gohmancm/Desktop/PODv4Point5FullProjectFolder/CSharpBackendTempSolution/CSharpBackendTempSolutionForPOD4Point5/RCode/RBackend/HitMissData_Bad.csv")
#testData<-read.csv("C:/Users/gohmancm/Desktop/PODv4Point5FullProjectFolder/CSharpBackendTempSolution/CSharpBackendTempSolutionForPOD4Point5/RCode/RBackend/HitMissInfo_BadLL.csv")
#testData<-read.csv("C:/Users/gohmancm/Desktop/PODv4Point5FullProjectFolder/CSharpBackendTempSolution/CSharpBackendTempSolutionForPOD4Point5/RCode/RBackend/newHitMissRSSTest.csv")
#testData <- read.csv("C:/Users/gohmancm/Desktop/PODv4Point5FullProjectFolder/PODv4Point5Attemp1/PODv4/POD Source Code/RCode/RBackend/HitMiss/HitMissData_Bad_2.csv")
#testData<-read.csv("C:/Users/gohmancm/Desktop/RSS/HitMissData_Bad.csv")
testData<-read.csv("C:/Users/gohmancm/Desktop/newPODrepository/HitMiss/HitMissData_Bad.csv")
#options(warn=-1)
#set the number of resamples here(usually 30 is sufficient)
#used for modified wald

regression="Firth Logistic Regression"
#regression="Logistic Regression"

normSamp=500
resamplesMax=30
#conf int type
ciType="LR"
start.time <- Sys.time()
set_m=6

if(ciType=="StandardWald"){
  set_r=floor(nrow(testData)/set_m)
}else if(ciType=="ModifiedWald"){
  set_r=floor(normSamp/set_m)
}else if(ciType=="LR"){
  set_r=floor(normSamp/set_m)
}else if(ciType=="MLR"){
  set_r=floor(normSamp/set_m)
}else{
  stop("CI type not valid!")
}
newRSSComponent<-RSSComponents$new()
newRSSComponent$initialize(maxResamples=resamplesMax, set_mInput=set_m, set_rInput=set_r,regressionType=regression, excludeNAInput=TRUE)
newHMRSSInstance<-HMAnalysis$new(hitMissDF=testData, CIType=ciType, modelType=regression, N=nrow(testData),
                                 normSampleAmount=normSamp, rankedSetSampleObject=newRSSComponent)
newHMRSSInstance$initializeRSS()
resultDF<-newHMRSSInstance$getResults()
aValResults<-newHMRSSInstance$getKeyAValues()
resid<-newHMRSSInstance$getGoodnessOfFit()
residualDF<-newHMRSSInstance$getResidualTable()
iteration<-newHMRSSInstance$getIterationTable()
end.time <- Sys.time()
time.taken <- end.time - start.time
print("total execution time was:")
print(time.taken)
resultDF<-na.omit(resultDF)
covarMatrix=newHMRSSInstance$getCovMatrix()
goodFitYAY=newHMRSSInstance$getGoodnessOfFit()
newHMRSSInstance$plotSimdata(resultDF)
newHMRSSInstance$plotCI(resultDF)


#newHMRSSInstance$plotCIRSS(resultDF)


