####This is only used when running POD analyses in R exclusively (i.e. without the PODv4.5 UI)
#1=linear
#2=log
#3=inverse
###function used to transform the dataframe to linear, log, or inverse
TransformHitMiss_HM=function(hitMIssDF, transformType){
  if(transformType==1){
    return(hitMIssDF)
  }else if(transformType==2){
    hitMIssDF$x=log(hitMIssDF$x)
    return(hitMIssDF)
  }else{
    hitMIssDF$x=1/(hitMIssDF$x)
    return(hitMIssDF)
  }
}
###Functions are used to transform back to linear space for signal Response
TransformOriginalDataFrameBack_HM=function(originalDF, transformType){
  if(transformType==1){
    originalDF$transformFlaw=originalDF$flaw
    return(originalDF)
  }else if(transformType==2){
    originalDF$transformFlaw=originalDF$flaw
    originalDF$flaw=exp(originalDF$flaw)
    return(originalDF)
  }else{
    originalDF$transformFlaw=originalDF$flaw
    originalDF$flaw=1/(originalDF$flaw)
    return(originalDF)
  }
}
TransformResultsBack_HM=function(results, transformType){
  if(transformType==1){
    results$transformFlaw=results$flaw
    return(results)
  }else if(transformType==2){
    results$transformFlaw=results$flaw
    results$flaw=exp(results$flaw)
    return(results)
  }else{
    results$transformFlaw=results$flaw
    results$flaw=1/(results$flaw)
    return(results)
  }
}
TransformResidualTableBack_HM=function(residTable, transformType){
  if(transformType==1){
    residTable$transformFlaw=residTable$flaw
    return(residTable)
  }else if(transformType==2){
    residTable$transformFlaw=residTable$flaw
    residTable$flaw=exp(residTable$flaw)
    return(residTable)
  }else{
    residTable$transformFlaw=residTable$flaw
    residTable$flaw=1/(residTable$flaw)
    return(residTable)
  }
}
TransformBackAValues_HM=function(aValues, transformType){
  for(i in 1:length(aValues)){
    if(transformType==1){
      break
    }else if(transformType==2){
      aValues[[i]]=exp(aValues[[i]])
    }else{
      aValues[[i]]=1/aValues[[i]]
    }
    return(aValues)
  }
}
####################OLD CODE
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


#assign(".lib.loc", "C:/Users/gohmancm/Desktop/newPODrepository/POD Source Code/RLibs/win-library/3.5", envir = environment(.libPaths))