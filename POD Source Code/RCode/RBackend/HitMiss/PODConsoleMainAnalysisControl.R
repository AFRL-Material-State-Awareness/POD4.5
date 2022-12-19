oneInspector=function(){
  newAnalysis<<-HMAnalysis$new(hitMissDF=hitMissDF, modelType=type, CIType=CItype0, N=nrow(hitMissDF), normSampleAmount=1500)
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

runMainAnalysis=function(){
  #hitMissDF<-read.csv("C:/Users/gohmancm/Desktop/RSS/HitMissData_Good.csv")
  #hitMissDF<-read.csv("C:/Users/gohmancm/Desktop/RSS/HitMissData_Bad.csv")
  #hitMissDF<-read.csv("C:/Users/gohmancm/Desktop/newPODrepository/HitMiss/HitMissData_Bad.csv")
  #hitMissDF$x<-1/hitMissDF$x
  #hitMissDF$y.1=NULL
  #hitMissDF[1,3]=1.5
  hitMissDF<-read.csv("C:/Users/gohmancm/Desktop/Ryan Moores- POD Data/PODDataRHF-HitMiss.csv")
  hitMissDF<-hitMissDF[ , colSums(is.na(hitMissDF))==0]
  #hitMissDF$x= 1/hitMissDF$x
  codeLocation=dirname(rstudioapi::getSourceEditorContext()$path)
  
  #get the working directory, used for debugging only
  transformType=1
  hitMissDF<-TransformHitMiss_HM(hitMissDF, transformType)
  if(transformType==2){
    isLog=TRUE
  }else{
    isLog=FALSE
  }
  names(hitMissDF)[names(hitMissDF) == 'IN.1.HM'] <- 'y'
  #hitMissDF<-subset(hitMissDF, select = c(Index, x, Inspector1, Inspector2))
  #names(hitMissDF)[names(hitMissDF) == 'Inspector1'] <- 'y'
  #CItype0="MLR"
  #CHOOSE MODEL TYPE(comment out the one you don't want)
  #type="Firth Logistic Regression"
  #type="Logistic Regression"
  #begin analysis
  begin=Sys.time()
  #oneInspector()
  MultipleInspectors()
  WriteOutResultsMuliple_POD(results)
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
  
  
  
  outputToExcel_HM()
}
