#this class is used to prepare data with multiple responses by creating a new table with the average of all the reponses
PrepareData<-setRefClass("PrepareData", fields = list(signalRespDF="data.frame",
                                                      avgSignalRespDF="data.frame"),
                         methods = list(
                           getOrigDataframe=function(){
                             return(signalRespDF)
                           },
                           setAvgRespDF=function(psAvgRespDF){
                             avgSignalRespDF<<-psAvgRespDF
                           },
                           getAvgRespDF=function(){
                             return(avgSignalRespDF)
                           },
                           createAvgRespDF=function(){
                             responsesOnlyDF= subset(signalRespDF, select = -c(Index, x, event))
                             avgResponses=c()
                             for(i in 1:nrow(signalRespDF)){
                               avgResponses=c(avgResponses, mean(as.numeric(responsesOnlyDF[i,])))
                             }
                             #print(avgResponses)
                             avgResponseDF=data.frame(
                               Index=signalRespDF$Index,
                               x=signalRespDF$x,
                               y=avgResponses,
                               event=signalRespDF$event
                             )
                             setAvgRespDF(avgResponseDF)
                           }
                         ))


#testInstance<-PrepareData$new(signalRespDF=data_obs)
#testInstance$getOrigDataframe()
#testInstance$createAvgRespDF()
#data_obs2=testInstance$createAvgRespDF()