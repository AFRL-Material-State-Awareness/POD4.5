WriteOutResultsMuliple_POD=function(results){
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
  plottingData<<-ggplot(combinedPODDataframe, aes(flaw))+xlim(0, max(orig[[1]]$flaw)+.1*max(orig[[1]]$flaw))
  podOnly=subset(combinedPODDataframe, select = -c(flaw) )
  booleanArray=c()
  for(i in 1:length(podOnly)){ booleanArray=append(booleanArray, TRUE)}
  print(length(booleanArray))
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
WriteOutResultsMuliple_Confidence=function(results){
  colorList=c("red", "blue", "green", "yellow", "orange", "pink")
  combinedConfidenceDataframe=data.frame(
    flaw=results[[1]]$flaw,
    Confidence_Interval_1=results[[1]]$Confidence_Interval
  )
  if(length(results)>1){
    for(i in 2:length(results)){
      thisConfidenceInterval=data.frame(Confidence_Interval=results[[i]]$Confidence_Interval)
      names(thisConfidenceInterval)[names(thisConfidenceInterval)=='Confidence_Interval']<-(paste('Confidence_Interval_', i, sep=''))
      combinedConfidenceDataframe=cbind(combinedConfidenceDataframe, thisConfidenceInterval)
    }
  }
  geomPoints=NULL
  plottingDataC<<-ggplot(combinedConfidenceDataframe, aes(flaw))+xlim(0, max(orig[[1]]$flaw)+.1*max(orig[[1]]$flaw))
  ConfidenceIntervalOnly=subset(combinedConfidenceDataframe, select = -c(flaw) )
  booleanArray=c()
  for(i in 1:length(podOnly)){ booleanArray=append(booleanArray, TRUE)}
  print(length(booleanArray))
  for(i in 1:length(ConfidenceIntervalOnly)){
    #plottingData<<-plottingData+geom_point(aes_(y=combinedPODDataframe[,i+1]), color=colorList[[i]], show.legend = TRUE)
    if(booleanArray[i]){
      plottingDataC<<-plottingDataC+geom_point(aes_(y=combinedConfidenceDataframe[,i+1], color=paste('Inspector',i)), show.legend = T)
    }
    #print(plottingData)
  }
  #ggplot(combinedPODDataframe, aes(flaw)) + geom_point(aes(y=POD_1), color=rgb(255, 0, 0,  maxColorValue = 255)) 
  #+ geom_point(aes(y=POD_2), color = "green")
}