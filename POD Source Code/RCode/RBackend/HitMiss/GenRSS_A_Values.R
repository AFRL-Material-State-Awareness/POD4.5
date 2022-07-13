
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
                                  a_ValuesLocal=append(a_ValuesLocal, median(as.numeric(a25s), na.rm=TRUE))
                                  a_ValuesLocal=append(a_ValuesLocal, median(as.numeric(a50s), na.rm=TRUE))
                                  a_ValuesLocal=append(a_ValuesLocal, median(as.numeric(a90s), na.rm=TRUE))
                                  a_ValuesLocal=append(a_ValuesLocal, median(as.numeric(sigmas), na.rm=TRUE))
                                  a_ValuesLocal=append(a_ValuesLocal, median(as.numeric(a9095s), na.rm=TRUE))
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
                                  a_ValuesLocal=append(a_ValuesLocal, median(as.numeric(a25s), na.rm=TRUE))
                                  a_ValuesLocal=append(a_ValuesLocal, median(as.numeric(a50s), na.rm=TRUE))
                                  a_ValuesLocal=append(a_ValuesLocal, median(as.numeric(a90s), na.rm=TRUE))
                                  a_ValuesLocal=append(a_ValuesLocal, median(as.numeric(sigmas), na.rm=TRUE))
                                  a_ValuesLocal=append(a_ValuesLocal, median(as.numeric(a9095s), na.rm=TRUE))
                                  setAvaluesList(a_ValuesLocal)
                                }
                              ))