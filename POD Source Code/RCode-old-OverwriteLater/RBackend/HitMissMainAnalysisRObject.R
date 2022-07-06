#main hitmiss analysis object when pass into R ( entry point is detAnalysisApproach() )
HMAnalysis <- setRefClass("HMAnalysis",
                        fields = list(hitMissDF = "data.frame", 
                                      CIType = "character",
                                      #also known as profile size
                                      N="numeric",
                                      #also known as a_x_n
                                      normSampleAmount="numeric",
                                      results="data.frame",
                                      a_values="list"),
                        methods = list(
                          getHitMissDF = function(){
                            return(hitMissDF)
                          },
                          setCIType = function(psCIType){
                            CIType <<- psCIType
                          },
                          getCIType = function(){
                            return(CIType)
                          },
                          setResults = function(psResults){
                            results <<- psResults
                          },
                          getResults = function(){
                            return(results)
                          },
                          setKeyAValues=function(psAValues){
                            a_values<<-psAValues
                          },
                          getKeyAValues=function(){
                            return(a_values)
                          },
                          detAnalysisApproach=function(){
                            #execute logit regression with original dataset
                            logitResults=executeLogitHM()
                            #check which confidence interval should be calculated
                            if(CIType=="Standard Wald"){
                              #set t_trans
                              t_trans=logitResults$fitted.values
                              #calculate wald using conf intervals class
                              newWaldCI= WaldConfInt$new(LogisticRegressionResult=logitResults)
                              newWaldCI$executeStandardWald()
                              Confidence_Interval=newWaldCI$getCIDataFrame()
                              #set the dataframe to return to c#
                              setResults(cbind(hitMissDF, t_trans, Confidence_Interval))
                              #get key a values
                              new_Acalc=GenAValuesOnPODCurve$new(LogisticRegressionResult=logitResults,inputDataFrameLogistic=hitMissDF)
                              new_Acalc$calcAValuesStandardWald()
                              setKeyAValues(new_Acalc$getAValuesList())
                            }
                            else if(CIType=="Modified Wald"){
                              #calc logit regression
                              #execute logit regression with original dataset
                              logitResults=executeLogitHM()
                              #generate normally distributed crack sizes
                              newNormalCrackDist=GenNormFit$new(cracks=hitMissDF$x, sampleSize=normSampleAmount, 
                                                                Nsample=N)
                              newNormalCrackDist$genNormalFit()
                              simCracks=newNormalCrackDist$getSimCrackSizesArray()
                              x = sort(simCracks)[seq(from=1,to=length(simCracks),length.out=normSampleAmount)]
                              #standard wald calculation
                              newWaldCI= WaldConfInt$new(LogisticRegressionResult=logitResults,simCrackVals=x)
                              newWaldCI$executeModifiedWald()
                              Confidence_Interval=newWaldCI$getCIDataFrame()
                              #set the dataframe to return to c#
                              setResults(cbind(x,Confidence_Interval))
                              #get key a values
                              new_Acalc=GenAValuesOnPODCurve$new(LogisticRegressionResult=logitResults,inputDataFrameLogistic=cbind(x,Confidence_Interval))
                              new_Acalc$calcAValuesModWald()
                              setKeyAValues(new_Acalc$getAValuesList())
                            }
                            else if(CIType=="LR"){
                              ptm <- proc.time()
                              newLinearComboInstance=new("LinearComboGenerator", LogisticRegressionResult=logitResults, 
                                                         simCracks=0.0,samples=normSampleAmount)
                              calcLinearCombo= newLinearComboInstance$genLinearCombinations()
                              x=newLinearComboInstance$getSimCracks()
                              newLRConfInt=LikelihoodRatioConfInt$new(LogisticRegressionResult=logitResults)
                              newLRConfInt$setLinCombo(calcLinearCombo)
                              newLRConfInt$executeLR()
                              print("Time of Calculation")
                              print(proc.time() - ptm)
                              Confidence_Interval=newLRConfInt$getCIDataFrame()
                              setResults(cbind(x, Confidence_Interval))
                              #get key a values
                              new_Acalc=GenAValuesOnPODCurve$new(LogisticRegressionResult=logitResults,inputDataFrameLogistic=cbind(x,Confidence_Interval))
                              new_Acalc$calca9095LR()
                              setKeyAValues(new_Acalc$getAValuesList())
                              #TODO: TRY TO KEEP THIS OUT OF THE GLOBAL ENVIRONMENT
                              #remove calcLinearCombo from the global environment
                              rm(calcLinearCombo, envir = .GlobalEnv)
                            }
                            else if(CIType=="MLR"){
                              ptm <- proc.time()
                              newLinearComboInstance=new("LinearComboGenerator", LogisticRegressionResult=logitResults, 
                                                         simCracks=0.0,samples=normSampleAmount)
                              calcLinearCombo= newLinearComboInstance$genLinearCombinations()
                              x=newLinearComboInstance$getSimCracks()
                              newMLRConfInt=ModifiedLikelihoodRatioConfInt$new(LogisticRegressionResult=logitResults)
                              newMLRConfInt$setLinCombo(calcLinearCombo)
                              newMLRConfInt$executeLR()
                              print("Time of Calculation")
                              print(proc.time() - ptm)
                              Confidence_Interval=newMLRConfInt$getCIDataFrame()
                              setResults(cbind(x, Confidence_Interval))
                              #get key a values
                              new_Acalc=GenAValuesOnPODCurve$new(LogisticRegressionResult=logitResults,inputDataFrameLogistic=cbind(x,Confidence_Interval))
                              new_Acalc$calca9095MLR()
                              setKeyAValues(new_Acalc$getAValuesList())
                              #TODO: TRY TO KEEP THIS OUT OF THE GLOBAL ENVIRONMENT
                              #remove calcLinearCombo from the global environment
                              rm(calcLinearCombo, envir = .GlobalEnv)
                            }
                            else{
                              stop("Confidence interval type not found!")
                            }
                          },
                          #executes the logit approximation of the hitmiss dataframe and returns
                          #the LReg.mod list
                          executeLogitHM=function(){
                            #execute logit regression with original dataset
                            newLogitModel=HMLogitApproximation$new(inputDataFrameLogistic=hitMissDF)
                            newLogitModel$calcLogitResults()
                            return(newLogitModel$getLogitResults())
                          },
                          #used for Debugging ONLY
                          plotSimdata=function(df){
                            myPlot=ggplot(data=df, mapping=aes(x=x, y=t_trans))+geom_point()+
                              ggtitle("POD Curve:", CIType)
                            print(myPlot)
                          },
                          plotCI=function(df){
                            myPlot=ggplot(data=df, mapping=aes(x=x, y=Confidence_Interval))+geom_point()+
                              ggtitle("Confidence interval:", CIType)
                            print(myPlot)
                          }
                          
                        ))
