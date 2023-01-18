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
#     along with this program.  If not, see <https://www.gnu.org/licenses>

#This class generates the Ranked set Sample indices and converts the subset data of 'r' cycles into

# parameters:
# testData = the original input hit/miss dataframe
# maxresamples = the amount of ranked set samples to generate (30 by default)
# set_r = the value for r when generating the ranked set samples
# set_m = the value for m when generating the ranked set samples
# regType = the type of regression to be used (firth or maximum likelihood)
# RankedSetResults = a list of the ranked set sample dataframes generated
# RSSLogitResults = a list of regression results as glm objects

RankedSetRegGen=setRefClass("RankedSetRegGen", fields = list(testData="data.frame", maxResamples="numeric", 
                                                                 set_m="numeric", set_r="numeric", regType="character",
                                                                 RankedSetResults="list", RSSLogitResults="list"),
                                  methods=list(
                                    initialize=function(testDataInput=data.frame(matrix(ncol = 1, nrow = 1)), 
                                                        maxResamplesInput=30,
                                                        set_mInput=6, 
                                                        set_rInput=0,
                                                        regTypeInput=""){
                                      testData<<-testDataInput
                                      maxResamples<<-maxResamplesInput
                                      set_m<<-set_mInput
                                      if(set_rInput!=0){
                                        set_r<<-set_rInput
                                      }
                                      else{
                                        set_r<<-floor(nrow(testDataInput)/set_mInput)
                                      }
                                      regType<<-regTypeInput
                                    },
                                    setRankedSetResults=function(psRankedSetResults){
                                      RankedSetResults<<-psRankedSetResults
                                    },
                                    getRankedSetResults=function(){
                                      return(RankedSetResults)
                                    },
                                    setRSSLogitResults=function(psRSSLogitResults){
                                      #GLOBAL<<-psRSSLogitResults
                                      RSSLogitResults<<-psRSSLogitResults
                                    },
                                    getRSSLogitResults=function(){
                                      return(RSSLogitResults)
                                    },
                                    generateFullRSS=function(){
                                      generateRSSData()
                                      if(regType=="Logistic Regression"){
                                        genLogitValuesForSubsets()
                                      }
                                      else if(regType=="Firth Logistic Regression"){
                                        genFirthValuesForSubsets()
                                      }
                                      else{
                                        stop("model type not found!")
                                      }
                                    },
                                    generateRSSData=function(){
                                      #start.time <- Sys.time()
                                      #create a list to hold the results RSS Dataframes
                                      rankedSetResults=list()
                                      #excute python scripts (fourth parameter is to account for the indices between
                                      if(exists('RSSamplingMain')){
                                        print("Python was used")
                                        newGenSamples=RSSamplingMain(testData, set_r, set_m, TRUE)
                                        for(i in 1:maxResamples){
                                          newGenSamples$performRSS()
                                          rankedSetResults=append(rankedSetResults, list(newGenSamples$RSSDataFrame))
                                        }
                                      }
                                      else{
                                        print("Python was not used")
                                        newGenSamples=RSSamplingMain_R$new()
                                        newGenSamples$initialize(testDataInput=testData, set_rInput = set_r, set_mInput = set_m)
                                        for(i in 1:maxResamples){
                                          newGenSamples$performRSS()
                                          rankedSetResults=append(rankedSetResults, list(newGenSamples$getRSSDAtaFrame()))
                                        }
                                      }

                                      setRankedSetResults(rankedSetResults)
                                    },
                                    genLogitValuesForSubsets=function(){
                                      localLogitResultsList=list()
                                      for(i in RankedSetResults){
                                        thisLogitResult=executeLogitRSS(i)
                                        localLogitResultsList<-append(localLogitResultsList, list(thisLogitResult))
                                      }
                                      setRSSLogitResults(localLogitResultsList)
                                    },
                                    genFirthValuesForSubsets=function(){
                                      localFirthResultsList=list()
                                      for(i in RankedSetResults){
                                        thisFirthResult=executeFirthRSS(i)
                                        localFirthResultsList<-append(localFirthResultsList, list(thisFirthResult))
                                      }
                                      setRSSLogitResults(localFirthResultsList)
                                    },
                                    executeLogitRSS=function(currDataFrame){
                                      #execute logit regression with original dataset
                                      newLogitModel=HMLogitApproximation$new(inputDataFrameLogistic=currDataFrame)
                                      newLogitModel$calcLogitResults()
                                      return(newLogitModel$getLogitResults())
                                    },
                                    executeFirthRSS=function(currDataFrame){
                                      newFirthModel=HMFirthApproximation$new(inputDataFrameFirth=currDataFrame)
                                      newFirthModel$calcFirthResults()
                                      return(newFirthModel$getFirthResults())
                                    },
                                    countBadLogits=function(){
                                      allFailed=FALSE
                                      count=0
                                      for(i in RSSLogitResults){
                                        if(i$converged==FALSE){
                                          count=count+1
                                        }
                                      }
                                      print(paste("total failed", regType, "results"))
                                      print(count)
                                      #terminate the rest of the program if all the logits fail
                                      if(count==length(RSSLogitResults)){
                                        allFailed=TRUE
                                      }
                                      return(allFailed)
                                    }
                                ))