#import necessary libraries
library(methods)
#dependencies from original R code
library(MASS)
library(ggplot2)
#library(mcprofile) ## MLR and LR
library(logistf)
library(splines)
library(parallel)
library(roxygen2)
library(xlsx)
library(minimcprofile)
hitMissDF<-read.csv("C:/Users/gohmancm/Desktop/RSS/HitMissData_Good.csv")
hitMissDF<-
  data.frame(
    Index=hitMissDF$Index,
    x=hitMissDF$x,
    y=hitMissDF$Inspector1
  )
#hitMissDF<-read.csv("C:/Users/gohmancm/Desktop/RSS/HitMissData_Bad.csv")
#hitMissDF<-read.csv("C:/Users/gohmancm/Desktop/newPODrepository/HitMiss/HitMissData_Bad.csv")
#hitMissDF$x<-1/hitMissDF$x
#hitMissDF$y.1=NULL
#hitMissDF[1,3]=1.5
#hitMissDF<-read.csv("C:/Users/gohmancm/Desktop/Ryan Moores- POD Data/PODDataRHF-HitMiss.csv")
hitMissDF<-read.csv("C:/Users/colin/Desktop/AFRL code-SECUREDONOTOPEN/PODv4.5/POD4.5/HitMiss/HitMissData_Good_1.csv")

hitMissDF<-hitMissDF[ , colSums(is.na(hitMissDF))==0]

#hitMissDF$x= 1/hitMissDF$x
codeLocation=dirname(rstudioapi::getSourceEditorContext()$path)
##### Import necessary R classes to perform hitmiss analysis
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
#source(paste(codeLocation,"/miniMcprofile.R",sep=""))
source(paste(codeLocation,"/TransformBackFunctions_ForRunningInOnly_R.R", sep = ""))
source(paste(codeLocation,"/HitMissInternalRGraphingOutputFunctions_NO_UI.R", sep = ""))
source(paste(codeLocation,"/OutputToExcel_ForRunningInOnly_R.R", sep = ""))

#get the working directory, used for debugging only
transformType=1
hitMissDF<-TransformHitMiss_HM(hitMissDF, transformType)
if(transformType==2){
  isLog=TRUE
}else{
  isLog=FALSE
}
#names(hitMissDF)[names(hitMissDF) == 'IN.1.HM'] <- 'y'
#hitMissDF<-subset(hitMissDF, select = c(Index, x, Inspector1, Inspector2))
#names(hitMissDF)[names(hitMissDF) == 'Inspector1'] <- 'y'
CItype0="LR"
#CHOOSE MODEL TYPE(comment out the one you don't want)
#type="Firth Logistic Regression"
type="Logistic Regression"
#begin analysis

oneInspector=function(){
  newAnalysis<<-HMAnalysis$new(hitMissDF=hitMissDF, modelType=type, CIType=CItype0, N=nrow(hitMissDF), normSampleAmount=500)
  newAnalysis$executeFullAnalysis()
  #newAnalysis$executeFitAnalysisOnly()
  results<<-TransformResultsBack_HM(newAnalysis$getResults(),transformType)
  residTab<<-TransformResidualTableBack_HM(newAnalysis$getResidualTable(), transformType)
  aValues<<-TransformBackAValues_HM(newAnalysis$getKeyAValues(), transformType)
  orig<<-TransformOriginalDataFrameBack_HM(newAnalysis$getHitMissDF(), transformType)
  covarianceMatrix<<-newAnalysis$getCovMatrix()
  goodnessOfFit<<-newAnalysis$getGoodnessOfFit()
  iterationTable<<-newAnalysis$getIterationTable()
  separatedQuestion<<-newAnalysis$getSeparation()
  failedtoConverge<<-newAnalysis$getConvergedFail()
  newAnalysis$plotSimdata(results)
  newAnalysis$plotCI(results)
}
MultipleInspectors=function(){
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
    newAnalysis<<-HMAnalysis$new(hitMissDF=thisAnalysis, modelType=type, CIType=CItype0, N=nrow(hitMissDF), normSampleAmount=500)
    newAnalysis$executeFullAnalysis()
    results<<-append(results,list(TransformResultsBack_HM(newAnalysis$getResults(),transformType)))
    residTab<<-append(residTab,list(TransformResidualTableBack_HM(newAnalysis$getResidualTable(), transformType)))
    aValues<<-append(aValues,list(TransformBackAValues_HM(newAnalysis$getKeyAValues(), transformType)))
    #aValues<<-append(aValues,list(newAnalysis$getKeyAValues(), transformType))
    orig<<-append(orig,list(TransformOriginalDataFrameBack_HM(newAnalysis$getHitMissDF(), transformType)))
    covarianceMatrix<<-append(covarianceMatrix,list(newAnalysis$getCovMatrix()))
    goodnessOfFit<<-append(goodnessOfFit,list(newAnalysis$getGoodnessOfFit()))
    iterationTable<<-append(iterationTable,list(newAnalysis$getIterationTable()))
    separatedQuestion<<-append(separatedQuestion,list(newAnalysis$getSeparation()))
    failedtoConverge<<-append(failedtoConverge,list(newAnalysis$getConvergedFail()))
  }
}

begin=Sys.time()
#oneInspector()
MultipleInspectors()
#WriteOutResultsMuliple_POD(results)
#WriteOutResultsMuliple_Confidence(results)
end=Sys.time()
print("execution time")
print(end-begin)

show(plottingData)
#show(plottingDataC)


filename= 'C:/Users/gohmancm/Desktop/Ryan Moores- POD Data/myExcelOutput.xlsx'

#outputList=list(orig[[1]], residTab[[1]], results[[1]], aValues[[1]])
#write.xlsx(orig[[1]], "C:/Users/gohmancm/Desktop/Ryan Moores- POD Data/myExcelOutput.xlsx", sheetName="Sheet1", append = FALSE)
#write.xlsx(aValues[[1]], "C:/Users/gohmancm/Desktop/Ryan Moores- POD Data/myExcelOutput.xlsx", sheetName="Sheet2", append = TRUE)



#outputToExcel_HM()

