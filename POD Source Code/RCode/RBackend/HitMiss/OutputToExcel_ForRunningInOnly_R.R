outputToExcel_HM=function(){
  #output order is as follows:
  #flaw transform (if any)
  #orig dataframe
  #residual dataframe
  #iterationTable
  #aValues
  #covar matrix
  #goodness of fit
  #format a values so that it's human readable
  if(transformType==1){
    flawTransform = "Linear"
  }else if(transformType==2){
    flawTransform = "Log"
  }else{
    flawTransform = "inverse"
  }
  excelOuputWK=createWorkbook()
  for(i in 1:length(results)){
    #for loop will start here
    sheet=createSheet(excelOuputWK, paste("Inspector",i, sep =''))
    
    aValues[[1]]=data.frame(
      a25=aValues[[i]][[1]],
      a50=aValues[[i]][[2]],
      a90=aValues[[i]][[3]],
      a90_95=aValues[[i]][[4]]
    )
    #ditto or the covar matrix
    covarianceMatrix[[i]]=data.frame(
      v11=covarianceMatrix[[i]][1,1],
      v12=covarianceMatrix[[i]][2,1],
      v21=covarianceMatrix[[i]][1,2],
      v22=covarianceMatrix[[i]][2,2]
    )
    #format failure to converge
    if(failedtoConverge[[i]]==1){
      converged="False"
    }else{
      converged="True"
    }
    #model type used 
    addDataFrame(as.data.frame("Logit Model Type: "), sheet = sheet, startRow = 1, startColumn = 1, row.names = FALSE, col.names = FALSE)
    addDataFrame(as.data.frame(type), sheet = sheet, startRow = 1, startColumn = 2, row.names = FALSE, col.names = FALSE)
    #did the algorithm converge?
    addDataFrame(as.data.frame("Algorithm Coverged: "), sheet = sheet, startRow = 1, startColumn = 4, row.names = FALSE, col.names = FALSE)
    addDataFrame(as.data.frame(converged), sheet = sheet, startRow = 1, startColumn = 5, row.names = FALSE, col.names = FALSE)
    #confidence interval used
    addDataFrame(as.data.frame("Confidence Interval Type: "), sheet = sheet, startRow = 2, startColumn = 1, row.names = FALSE, col.names = FALSE)
    addDataFrame(as.data.frame(CItype0), sheet = sheet, startRow = 2, startColumn = 2, row.names = FALSE, col.names = FALSE)
    #flaw transform
    addDataFrame(as.data.frame("Flaw Transform: "), sheet = sheet, startRow = 3, startColumn = 1, row.names = FALSE, col.names = FALSE)
    addDataFrame(as.data.frame(flawTransform), sheet = sheet, startRow = 3, startColumn = 2, row.names = FALSE, col.names = FALSE)
    #values
    addDataFrame(as.data.frame("A VAlues: "), sheet = sheet, startRow = 4, startColumn = 1, row.names = FALSE, col.names = FALSE)
    addDataFrame(aValues[[1]], sheet = sheet, startRow = 5, startColumn = 1, row.names = FALSE)
    #covar matrix
    addDataFrame(as.data.frame("Covariance matrix: "), sheet = sheet, startRow = 7, startColumn = 1, row.names = FALSE, col.names = FALSE)
    addDataFrame(covarianceMatrix[[i]], sheet = sheet, startRow = 8, startColumn = 1, row.names = FALSE)
    #goodnessOfFit
    addDataFrame(as.data.frame("Goodness of Fit: "), sheet = sheet, startRow = 10, startColumn = 1, row.names = FALSE, col.names = FALSE)
    addDataFrame(as.data.frame(goodnessOfFit[[i]]), sheet = sheet, startRow = 10, startColumn = 2, row.names = FALSE, col.names = FALSE)
    #orig dataframe
    addDataFrame(as.data.frame("Original Dataframe: "), sheet = sheet, startRow = 12, startColumn = 1, row.names = FALSE, col.names = FALSE)
    addDataFrame(orig[[i]], sheet = sheet, startRow = 13, startColumn = 1, row.names = FALSE)
    #residual Dataframe
    addDataFrame(as.data.frame("Residual Dataframe: "), sheet = sheet, startRow = 12, startColumn = 6, row.names = FALSE, col.names = FALSE)
    addDataFrame(residTab[[i]], sheet = sheet, startRow = 13, startColumn = 6, row.names = FALSE)
    #iterations table
    addDataFrame(as.data.frame("Iteration Table: "), sheet = sheet, startRow = 4, startColumn = 7, row.names = FALSE, col.names = FALSE)
    addDataFrame(iterationTable[[i]], sheet = sheet, startRow = 5, startColumn = 7, row.names = FALSE)
    #residual Dataframe
    addDataFrame(as.data.frame("POD Dataframe: "), sheet = sheet, startRow = 12, startColumn = 13, row.names = FALSE, col.names = FALSE)
    addDataFrame(results[[i]], sheet = sheet, startRow = 13, startColumn = 13, row.names = FALSE)
    
    autoSizeColumn(sheet=sheet, colIndex = 1:100)
  }
  
  #make a POD table with al inspectors
  # sheet=createSheet(excelOuputWK, "POD_Inspectors_ALL")
  # addDataFrame(outputExcelPODAndConfidence_All_HM()[[1]], sheet = sheet, startRow = 1, startColumn = 1, row.names = FALSE)
  # autoSizeColumn(sheet=sheet, colIndex = 1:100)
  # sheet=createSheet(excelOuputWK, "POD_Confidence_ALL")
  # addDataFrame(outputExcelPODAndConfidence_All_HM()[[2]], sheet = sheet, startRow = 1, startColumn = 1, row.names = FALSE)
  # autoSizeColumn(sheet=sheet, colIndex = 1:100)
  saveWorkbook(excelOuputWK, "C:/Users/gohmancm/Desktop/Ryan Moores- POD Data/myExcelOutput.xlsx")
}
outputExcelPODAndConfidence_All_HM=function(){
  POD_Dataframe_All=data.frame(
    flaw=orig[[1]]$flaw
  )
  for (i in 1:length(results)){
    currPOD=results[[i]]$pod
    POD_Dataframe_All=cbind(POD_Dataframe_All, as.data.frame(currPOD))
    names(POD_Dataframe_All)[names(POD_Dataframe_All) == 'currPOD'] <- paste("Inspector", i, sep = '')
  }
  Confidence_Dataframe_All=data.frame(
    flaw=orig[[1]]$flaw
  )
  for (i in 1:length(results)){
    currPODConf=results[[i]]$Confidence_Interval
    Confidence_Dataframe_All=cbind(Confidence_Dataframe_All, currPODConf)
    names(POD_Dataframe_All)[names(POD_Dataframe_All) == 'currPODConf'] <- paste("Inspector", i, sep = '')
  }
  return(list(POD_Dataframe_All, Confidence_Dataframe_All))
}

# outputToExcel=function(){
#   #output order is as follows:
#   #flaw transform (if any)
#   #orig dataframe
#   #residual dataframe
#   #iterationTable
#   #aValues
#   #covar matrix
#   #goodness of fit
#   #format a values so that it's human readable
#   if(transformType==1){
#     flawTransform = "Linear"
#   }else if(transformType==2){
#     flawTransform = "Log"
#   }else{
#     flawTransform = "inverse"
#   }
#   excelOuputWK=createWorkbook()
#   
#   #for loop will start here
#   sheet=createSheet(excelOuputWK, paste("Inspector",1, sep =''))
#   
#   aValues[[1]]=data.frame(
#     a25=aValues[[1]][[1]],
#     a50=aValues[[1]][[2]],
#     a90=aValues[[1]][[3]],
#     a90_95=aValues[[1]][[4]]
#   )
#   #ditto or the covar matrix
#   covarianceMatrix[[1]]=data.frame(
#     v11=covarianceMatrix[[1]][1,1],
#     v12=covarianceMatrix[[1]][2,1],
#     v21=covarianceMatrix[[1]][1,2],
#     v22=covarianceMatrix[[1]][2,2]
#   )
#   #format failure to converge
#   if(failedtoConverge[[1]]==1){
#     converged="False"
#   }else{
#     converged="True"
#   }
#   #model type used 
#   addDataFrame(as.data.frame("Logit Model Type: "), sheet = sheet, startRow = 1, startColumn = 1, row.names = FALSE, col.names = FALSE)
#   addDataFrame(as.data.frame(type), sheet = sheet, startRow = 1, startColumn = 2, row.names = FALSE, col.names = FALSE)
#   #did the algorithm converge?
#   addDataFrame(as.data.frame("Algorithm Coverged: "), sheet = sheet, startRow = 1, startColumn = 4, row.names = FALSE, col.names = FALSE)
#   addDataFrame(as.data.frame(converged), sheet = sheet, startRow = 1, startColumn = 5, row.names = FALSE, col.names = FALSE)
#   #confidence interval used
#   addDataFrame(as.data.frame("Confidence Interval Type: "), sheet = sheet, startRow = 2, startColumn = 1, row.names = FALSE, col.names = FALSE)
#   addDataFrame(as.data.frame(CItype0), sheet = sheet, startRow = 2, startColumn = 2, row.names = FALSE, col.names = FALSE)
#   #flaw transform
#   addDataFrame(as.data.frame("Flaw Transform: "), sheet = sheet, startRow = 3, startColumn = 1, row.names = FALSE, col.names = FALSE)
#   addDataFrame(as.data.frame(flawTransform), sheet = sheet, startRow = 3, startColumn = 2, row.names = FALSE, col.names = FALSE)
#   #values
#   addDataFrame(as.data.frame("A VAlues: "), sheet = sheet, startRow = 4, startColumn = 1, row.names = FALSE, col.names = FALSE)
#   addDataFrame(aValues[[1]], sheet = sheet, startRow = 5, startColumn = 1, row.names = FALSE)
#   #covar matrix
#   addDataFrame(as.data.frame("Covariance matrix: "), sheet = sheet, startRow = 7, startColumn = 1, row.names = FALSE, col.names = FALSE)
#   addDataFrame(covarianceMatrix[[1]], sheet = sheet, startRow = 8, startColumn = 1, row.names = FALSE)
#   #goodnessOfFit
#   addDataFrame(as.data.frame("Goodness of Fit: "), sheet = sheet, startRow = 10, startColumn = 1, row.names = FALSE, col.names = FALSE)
#   addDataFrame(as.data.frame(goodnessOfFit[[1]]), sheet = sheet, startRow = 10, startColumn = 2, row.names = FALSE, col.names = FALSE)
#   #orig dataframe
#   addDataFrame(as.data.frame("Original Dataframe: "), sheet = sheet, startRow = 12, startColumn = 1, row.names = FALSE, col.names = FALSE)
#   addDataFrame(orig[[1]], sheet = sheet, startRow = 13, startColumn = 1, row.names = FALSE)
#   #residual Dataframe
#   addDataFrame(as.data.frame("Residual Dataframe: "), sheet = sheet, startRow = 12, startColumn = 6, row.names = FALSE, col.names = FALSE)
#   addDataFrame(residTab[[1]], sheet = sheet, startRow = 13, startColumn = 6, row.names = FALSE)
#   #iterations table
#   addDataFrame(as.data.frame("Iteration Table: "), sheet = sheet, startRow = 75, startColumn = 1, row.names = FALSE, col.names = FALSE)
#   addDataFrame(iterationTable[[1]], sheet = sheet, startRow = 76, startColumn = 1, row.names = FALSE)
#   #residual Dataframe
#   addDataFrame(as.data.frame("POD Dataframe: "), sheet = sheet, startRow = 12, startColumn = 13, row.names = FALSE, col.names = FALSE)
#   addDataFrame(results[[1]], sheet = sheet, startRow = 13, startColumn = 13, row.names = FALSE)
#   
#   autoSizeColumn(sheet=sheet, colIndex = 1:100)
#   saveWorkbook(excelOuputWK, "C:/Users/gohmancm/Desktop/Ryan Moores- POD Data/myExcelOutput.xlsx")
# }