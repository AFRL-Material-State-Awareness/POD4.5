library(ggplot2)
library(reticulate)
library(MASS)
#(mcprofile)
library(parallel)
library(logistf)
library(splines)
# #Test Code

#Sys.which("python")
#use_python('C:/ProgramData/Anaconda3')
use_condaenv('C:/ProgramData/Anaconda3')
#use_python('C:/Users/colin/AppData/Local/Programs/Python/Python310-32')
filepath=dirname(rstudioapi::getSourceEditorContext()$path)
source_python(paste(filepath,"/../PythonRankedSetSampling/RSSRowSorter.py", sep=""))
source_python(paste(filepath,"/../PythonRankedSetSampling/CyclesArrayGenerator.py", sep=""))
source_python(paste(filepath,"/../PythonRankedSetSampling/MainRSSamplingClass.py", sep=""))
source_python(paste(filepath,"/../PythonRankedSetSampling/RSS2DArrayGenerator.py", sep=""))
#rscripts
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
source(paste(filepath,"/LinearComboGeneratorClassR.R",sep=""))

source(paste(filepath,"/LRConfIntRObject.R", sep = ""))
source(paste(filepath,"/MLRConfIntRObject.R", sep = ""))
source(paste(filepath,"/TransformBackFunctions_ForRunningInOnly_R.R", sep = ""))
source(paste(filepath,"/OutputToExcel_ForRunningInOnly_R.R", sep = ""))
source(paste(filepath,"/miniMcprofile.R",sep=""))
source(paste(filepath,"/RankedSetSamplingMainRObject.R", sep = ""))
#LOAD in data as CSV for POD Analysis
#testData<-read.csv("C:/Users/gohmancm/Desktop/POD4.5Project/POD4.5/POD Source Code/RCode/RBackend/HitMiss/HitMissData_Good.csv")
#testData<-read.csv("C:/Users/gohmancm/Desktop/Ryan Moores- POD Data/PODDataRHF-HitMiss.csv")
testData<-read.csv("C:/Users/colin/Desktop/AFRL code-SECUREDONOTOPEN/PODv4.5/POD4.5/HitMiss/HitMissData_Good_1.csv")
testData<-testData[ , colSums(is.na(testData))==0]
#testData<-read.csv("C:/Users/gohmancm/Desktop/RSS/HitMissData_Bad.csv")
#testData<-read.csv("C:/Users/gohmancm/Desktop/newPODrepository/HitMiss/HitMissData_Bad.csv")
#testData<-read.csv("C:/Users/gohmancm/Desktop/RCode/RBackend/HitMissInfo_BadLL.csv")
#testData<-read.csv("C:/Users/gohmancm/Desktop/PODv4Point5FullProjectFolder/CSharpBackendTempSolution/CSharpBackendTempSolutionForPOD4Point5/RCode/RBackend/HitMissData_Bad.csv")
#testData<-read.csv("C:/Users/gohmancm/Desktop/PODv4Point5FullProjectFolder/CSharpBackendTempSolution/CSharpBackendTempSolutionForPOD4Point5/RCode/RBackend/HitMissInfo_BadLL.csv")
#testData<-read.csv("C:/Users/gohmancm/Desktop/PODv4Point5FullProjectFolder/CSharpBackendTempSolution/CSharpBackendTempSolutionForPOD4Point5/RCode/RBackend/newHitMissRSSTest.csv")
#testData <- read.csv("C:/Users/gohmancm/Desktop/PODv4Point5FullProjectFolder/PODv4Point5Attemp1/PODv4/POD Source Code/RCode/RBackend/HitMiss/HitMissData_Bad_2.csv")
#testData<-read.csv("C:/Users/gohmancm/Desktop/RSS/HitMissData_Bad.csv")
#testData<-read.csv("C:/Users/gohmancm/Desktop/newPODrepository/HitMiss/HitMissData_Bad.csv")
##############################################################
#testData<-read.csv("C:/Users/colin/Desktop/HitMissResults_Good_1.csv")
testData=subset(testData, select = c(Index, x, Inspector1))
names(testData)[names(testData) == 'Inspector1'] <- 'y'
#names(testData)[names(testData) == 'IN.1.HM'] <- 'y'
#############################################################
transformType=1
testData<-TransformHitMiss_HM(testData, transformType)
#flag used for genNormFitClassR
if(transformType==2){
  isLog=TRUE
}else{
  isLog=FALSE
}
#options(warn=-1)
#set the number of resamples here(usually 30 is sufficient)
#used for modified wald

#CHOOSE MODEL TYPE(comment out the one you don't want)
#regression="Firth Logistic Regression"
regression="Logistic Regression"

normSamp=250
resamplesMax=5
#conf int type
CItype0="LR"
start.time <- Sys.time()
set_m=6

if(CItype0=="StandardWald"){
  set_r=floor(nrow(testData)/set_m)
}else if(CItype0=="ModifiedWald"){
  set_r=floor(normSamp/set_m)
}else if(CItype0=="LR"){
  set_r=floor(normSamp/set_m)
}else if(CItype0=="MLR"){
  set_r=floor(normSamp/set_m)
}else{
  stop("CI type not valid!")
}
oneInspector=function(){
  newRSSComponent<-RSSComponents$new()
  newRSSComponent$initialize(maxResamples=resamplesMax, set_mInput=set_m, set_rInput=set_r,regressionType=regression, excludeNAInput=TRUE)
  newHMRSSInstance<-HMAnalysis$new(hitMissDF=testData, CIType=CItype0, modelType=regression, N=nrow(testData),
                                   normSampleAmount=normSamp, rankedSetSampleObject=newRSSComponent)
  newHMRSSInstance$initializeRSS()
  resultDF<<-TransformResultsBack_HM(na.omit(newHMRSSInstance$getResults()), transformType)
  aValResults<<-TransformBackAValues_HM(newHMRSSInstance$getKeyAValues(), transformType)
  goodnessOfFit<<-newHMRSSInstance$getGoodnessOfFit()
  residualDF<<-TransformResidualTableBack_HM(newHMRSSInstance$getResidualTable(), transformType)
  iteration<<-newHMRSSInstance$getIterationTable()
  end.time <<- Sys.time()
  time.taken <<- end.time - start.time
  print("total execution time was:")
  print(time.taken)
  covarMatrix<<-newHMRSSInstance$getCovMatrix()
  #goodFitYAY=newHMRSSInstance$getGoodnessOfFit()
  #newHMRSSInstance$plotSimdata(resultDF)
  newHMRSSInstance$plotCI(resultDF)
}
#newHMRSSInstance$plotCIRSS(resultDF)

MultipleInspectors=function(hitMissDF){
  results<<-list()
  residTab<<-list()
  aValues<<-list()
  orig<<-list()
  covarianceMatrix<<-list()
  goodnessOfFit<<-list()
  iterationTable<<-list()
  separatedQuestion<<-list()
  failedtoConverge<<-list()
  responsesOnly<<-subset(hitMissDF, select = -c(Index,x) )
  for (i in 1:ncol(responsesOnly)){
    thisAnalysis=data.frame(
      Index=hitMissDF$Index,
      x=hitMissDF$x,
      y=responsesOnly[,i]
    )
    newRSSComponent<<-RSSComponents$new()
    newRSSComponent$initialize(maxResamples=resamplesMax, set_mInput=set_m, set_rInput=set_r,regressionType=regression, excludeNAInput=TRUE)
    newHMRSSInstance<<-HMAnalysis$new(hitMissDF=testData, CIType=CItype0, modelType=regression, N=nrow(testData),
                                     normSampleAmount=normSamp, rankedSetSampleObject=newRSSComponent)
    newHMRSSInstance$executeFullAnalysis()
    results<<-append(results,list(TransformResultsBack_HM(newHMRSSInstance$getResults(),transformType)))
    residTab<<-append(residTab,list(TransformResidualTableBack_HM(newHMRSSInstance$getResidualTable(), transformType)))
    aValues<<-append(aValues,list(TransformBackAValues_HM(newHMRSSInstance$getKeyAValues(), transformType)))
    orig<<-append(orig,list(TransformOriginalDataFrameBack_HM(newHMRSSInstance$getHitMissDF(), transformType)))
    covarianceMatrix<<-append(covarianceMatrix,list(newHMRSSInstance$getCovMatrix()))
    goodnessOfFit<<-append(goodnessOfFit,list(newHMRSSInstance$getGoodnessOfFit()))
    iterationTable<<-append(iterationTable,list(newHMRSSInstance$getIterationTable()))
    separatedQuestion<<-append(separatedQuestion,list(newHMRSSInstance$getSeparation()))
    failedtoConverge<<-append(failedtoConverge,list(newHMRSSInstance$getConvergedFail()))
  }
}
WriteOutResultsMuliple=function(results){
  colorList=c("red", "blue", "green", "yellow", "orange", "pink")
  combinedPODDataframe=data.frame(
    flaw=results[[1]]$flaw,
    POD_1=results[[1]]$pod
  )
  if(length(results)>1){
    for(i in 2:length(results)){
      thisPOD=data.frame(pod=results[[i]]$pod)
      names(thisPOD)[names(thisPOD)=='pod']<-(paste('POD_', i, sep=''))
      combinedPODDataframe=cbind(combinedPODDataframe, thisPOD)
    }
  }
  geomPoints=NULL
  plottingData<<-ggplot(combinedPODDataframe, aes(flaw))
  podOnly=subset(combinedPODDataframe, select = -c(flaw) )
  booleanArray=c()
  for(i in 1:ncol(responsesOnly)){ booleanArray=append(booleanArray, TRUE)}
  for(i in 1:length(podOnly)){
    #plottingData<<-plottingData+geom_point(aes_(y=combinedPODDataframe[,i+1]), color=colorList[[i]], show.legend = TRUE)
    if(booleanArray[i]){
      plottingData<<-plottingData+geom_point(aes_(y=combinedPODDataframe[,i+1], color=paste('Inspector',i)), show.legend = T)
    }
    #print(plottingData)
  }
  
  #ggplot(combinedPODDataframe, aes(flaw)) + geom_point(aes(y=POD_1), color=rgb(255, 0, 0,  maxColorValue = 255)) 
  #+ geom_point(aes(y=POD_2), color = "green")
}

begin=Sys.time()
oneInspector()
#MultipleInspectors(testData)
end=Sys.time()
print("execution time")
print(end-begin)
#WriteOutResultsMuliple(results)
show(plottingData)
#outputToExcel_HM()