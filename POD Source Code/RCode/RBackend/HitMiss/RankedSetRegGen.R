#This class generates the RSS indices and converts the subset data of 'r' cycles into
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
                                      #r and python arrays)
                                      print("starting python code!!!")
                                      newGenSamples=RSSamplingMain(testData, set_r, set_m, TRUE)
                                      for(i in 1:maxResamples){
                                        newGenSamples$performRSS()
                                        rankedSetResults=append(rankedSetResults, list(newGenSamples$RSSDataFrame))
                                      }
                                      setRankedSetResults(rankedSetResults)
                                      #end.time <- Sys.time()
                                      #time.taken <- end.time - start.time
                                      #print("total execution time was RSS:")
                                      #print(time.taken)
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