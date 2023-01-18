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

# This class is used to get the critical A values when ranked set sampling is used

# parameters: 
# pODRSSDF = The POD curve generated from taking the medians of the logisitc regression results
# logitResultsList = a list of the logistic regression result objects (firth or maximum likelihood). This should
# be of size 30 by default (maxresamples is typically set to 30)
# excludeNA = boolean flag to determine if NA values are excluded when taking the median (should always be set to true)
# RSSDataFrmaes = a list of the ranked set sample dataframes generated (should also be 30 like logitResultsList)
# aValuesList = a list of the median aValues based on the ranked set sample dataframes

GenRSS_A_Values<- setRefClass("GenRSS_A_Values", fields = list(pODRSSDF="data.frame", logitResultsList="list", 
                                                               excludeNA= "logical", RSSDataFrames="list", aValuesList="list"),
                              methods = list(
                                initialize=function(pODRSSDFInput=data.frame(matrix(ncol = 1, nrow = 0)), excludeNAInput=TRUE,
                                                    logitResultsListInput=list(),
                                                    RSSDataFramesInput=list(), aValuesListInput=list()){
                                  pODRSSDF<<-pODRSSDFInput
                                  excludeNA<<-excludeNAInput
                                  logitResultsList<<-logitResultsListInput
                                  RSSDataFrames<<-RSSDataFramesInput
                                  aValuesList<<-aValuesListInput
                                },
                                setAvaluesList=function(psAValues){
                                  aValuesList<<-psAValues
                                },
                                getAValuesList=function(){
                                  return(aValuesList)
                                },
                                genAValueswald=function(){
                                  a_ValuesLocal=list()
                                  #for loop through the dataframe and look for a25,a50, and a90
                                  #store all three in the list(a90 may not exist which would be NA)
                                  for(i in 1:nrow(pODRSSDF)){
                                    if(isTRUE(all.equal(pODRSSDF[i,2], .25)) || isTRUE(all.equal(pODRSSDF[i,2], .50))
                                       || isTRUE(all.equal(pODRSSDF[i,2], .90))){
                                      a_ValuesLocal=append(a_ValuesLocal, pODRSSDF[i,1])
                                      if(isTRUE(all.equal(pODRSSDF[i,2], .90))){
                                        #create an empty slot for sigma
                                        a_ValuesLocal=append(a_ValuesLocal, -1)
                                        #store a9095 afterward
                                        a_ValuesLocal=append(a_ValuesLocal, pODRSSDF[i,3])
                                      }
                                    }
                                  }
                                  #Now we will calculate sigma
                                  sigmas=c()
                                  # Log odds value at 90% POD (standard error at .90)
                                  LOd=log(0.9/(1-0.9))
                                  for(i in 1:length(logitResultsList)){
                                    #calc values for sigma
                                    a90 = unname((LOd-logitResultsList[[i]]$coefficients[1])/
                                                   logitResultsList[[i]]$coefficients[2])
                                    thisSigmahat = predict(logitResultsList[[i]], newdata=data.frame(x=a90), type="response",se.fit=TRUE)$se.fit
                                    sigmas=c(sigmas, c(thisSigmahat))
                                  }
                                  a_ValuesLocal[4]=median(sigmas, na.rm=excludeNA)
                                  setAvaluesList(a_ValuesLocal)
                                },
                                genAValuesStandWald=function(){
                                  a_ValuesLocal=list()
                                  a25s=c()
                                  a50s=c()
                                  a90s=c()
                                  sigmas=c()
                                  a9095s=c()
                                  for(i in 1:length(logitResultsList)){
                                    newAValuesModObject=GenAValuesOnPODCurve$new(LogisticRegressionResult=logitResultsList[[i]],
                                                                                 inputDataFrameLogistic=pODRSSDF)
                                    newAValuesModObject$calcAValuesStandardWald()
                                    currAValues=newAValuesModObject$getAValuesList()
                                    a25s=c(a25s, c(currAValues[1]))
                                    a50s=c(a50s, c(currAValues[2]))
                                    a90s=c(a90s, c(currAValues[3]))
                                    sigmas=c(sigmas, c(currAValues[4]))
                                    a9095s=c(a9095s, c(currAValues[5]))
                                  }
                                  a_ValuesLocal=CalcAValueMedians(a_ValuesLocal,a25s, a50s, a90s, sigmas, a9095s)
                                  setAvaluesList(a_ValuesLocal)
                                },
                                genAValuesModWald=function(){
                                  a_ValuesLocal=list()
                                  a25s=c()
                                  a50s=c()
                                  a90s=c()
                                  sigmas=c()
                                  a9095s=c()
                                  for(i in 1:length(logitResultsList)){
                                    newAValuesModObject=GenAValuesOnPODCurve$new(LogisticRegressionResult=logitResultsList[[i]],
                                                                                 inputDataFrameLogistic=pODRSSDF)
                                    newAValuesModObject$calcAValuesModWald()
                                    currAValues=newAValuesModObject$getAValuesList()
                                    a25s=c(a25s, c(currAValues[1]))
                                    a50s=c(a50s, c(currAValues[2]))
                                    a90s=c(a90s, c(currAValues[3]))
                                    sigmas=c(sigmas, c(currAValues[4]))
                                    a9095s=c(a9095s, c(currAValues[5]))
                                  }
                                  a_ValuesLocal=CalcAValueMedians(a_ValuesLocal,a25s, a50s, a90s, sigmas, a9095s)
                                  setAvaluesList(a_ValuesLocal)
                                },
                                genAValuesLR=function(){
                                  a_ValuesLocal=list()
                                  a25s=c()
                                  a50s=c()
                                  a90s=c()
                                  sigmas=c()
                                  a9095s=c()
                                  for(i in 1:length(logitResultsList)){
                                    newAValuesModObject=GenAValuesOnPODCurve$new(LogisticRegressionResult=logitResultsList[[i]],
                                                                                 inputDataFrameLogistic=pODRSSDF)
                                    newAValuesModObject$calca9095LR()
                                    currAValues=newAValuesModObject$getAValuesList()
                                    a25s=c(a25s, c(currAValues[1]))
                                    a50s=c(a50s, c(currAValues[2]))
                                    a90s=c(a90s, c(currAValues[3]))
                                    sigmas=c(sigmas, c(currAValues[4]))
                                    a9095s=c(a9095s, c(currAValues[5]))
                                  }
                                  a_ValuesLocal=CalcAValueMedians(a_ValuesLocal,a25s, a50s, a90s, sigmas, a9095s)
                                  setAvaluesList(a_ValuesLocal)
                                },
                                genAValuesMLR=function(){
                                  a_ValuesLocal=list()
                                  a25s=c()
                                  a50s=c()
                                  a90s=c()
                                  sigmas=c()
                                  a9095s=c()
                                  for(i in 1:length(logitResultsList)){
                                    newAValuesModObject=GenAValuesOnPODCurve$new(LogisticRegressionResult=logitResultsList[[i]],
                                                                                 inputDataFrameLogistic=pODRSSDF)
                                    newAValuesModObject$calca9095MLR()
                                    currAValues=newAValuesModObject$getAValuesList()
                                    a25s=c(a25s, c(currAValues[1]))
                                    a50s=c(a50s, c(currAValues[2]))
                                    a90s=c(a90s, c(currAValues[3]))
                                    sigmas=c(sigmas, c(currAValues[4]))
                                    a9095s=c(a9095s, c(currAValues[5]))
                                  }
                                  a_ValuesLocal=CalcAValueMedians(a_ValuesLocal,a25s, a50s, a90s, sigmas, a9095s)
                                  setAvaluesList(a_ValuesLocal)
                                },
                                CalcAValueMedians=function(a_ValuesLocal, a25s, a50s, a90s, sigmas, a9095s){
                                  a_ValuesLocal=append(a_ValuesLocal, median(as.numeric(a25s), na.rm=TRUE))
                                  a_ValuesLocal=append(a_ValuesLocal, median(as.numeric(a50s), na.rm=TRUE))
                                  a_ValuesLocal=append(a_ValuesLocal, median(as.numeric(a90s), na.rm=TRUE))
                                  a_ValuesLocal=append(a_ValuesLocal, median(as.numeric(sigmas), na.rm=TRUE))
                                  a_ValuesLocal=append(a_ValuesLocal, median(as.numeric(a9095s), na.rm=TRUE))
                                  return(a_ValuesLocal)
                                }
                              ))