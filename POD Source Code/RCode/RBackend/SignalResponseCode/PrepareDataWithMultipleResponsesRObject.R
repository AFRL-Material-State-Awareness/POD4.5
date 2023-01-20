#     Probability of Detection Version 4.5 (PODv4.5)
#     Copyright (C) 2022  University of Dayton Research Institute (UDRI)
# 
#     This program is free software: you can redistribute it and/or modify
#     it under the terms of the GNU General Public License as published by
#     the Free Software Foundation, either version 3 of the License, or
#     (at your option) any later version.
# 
#     This program is distributed in the hope that it will be useful,
#     but WITHOUT ANY WARRANTY; without even the implied warranty of
#     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
#     GNU General Public License for more details.
# 
#     You should have received a copy of the GNU General Public License
#     along with this program.  If not, see <https://www.gnu.org/licenses/>.

# this class is used to calculate the Likelihood ratio confidence interval and
# the MODIFIED Likelihood ratio confidence interval of the POD curve (WARNING: VERY SLOW)

# this class is used to prepare data with multiple responses by creating a new table with the average of all the reponses

# signalRespDF = the original signal response data frame supplied to the analysis
# avgSignalREspDF = all of the input dataframes with taking the average of the response values

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