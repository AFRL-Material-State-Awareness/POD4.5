#This is the main Ranked Set Sampling R Object Class
RSSMainClassObject <- setRefClass("RSSMainClassObject", fields = list(dataFrame="data.frame",
                                                                      rsSComponentFromMain= "RSSComponents",
                                                                      CITypeRSS="character",
                                                                      normSampleAmount="numeric",
                                                                      medianAValues="list",
                                                                      pODDataFrame="data.frame",
                                                                      covarMatrix="matrix",
                                                                      goodnessOfFit="numeric"),
                                                        methods = list(
                                                        setMedianAValues=function(psMedianAValues){
                                                          medianAValues<<-psMedianAValues
                                                        },
                                                        getMedianAValues=function(){
                                                          return(medianAValues)
                                                        },
                                                        setPODDataFrame=function(psPODDataFrame){
                                                          pODDataFrame<<-psPODDataFrame
                                                        },
                                                        getPODDataFrame=function(){
                                                          return(pODDataFrame)
                                                        },
                                                        getMedianCovarMatrix=function(){
                                                          return(covarMatrix)
                                                        },
                                                        setMedianCovarMatrix=function(psCovarMatrix){
                                                          covarMatrix<<-psCovarMatrix
                                                        },
                                                        setGoodnessOfFit=function(psGoodFit){
                                                          goodnessOfFit<<-psGoodFit
                                                        },
                                                        getGoodnessOfFit=function(){
                                                          return(goodnessOfFit)
                                                        },
                                                        executeRSSPOD=function(){
                                                          if(CITypeRSS=="ModifiedWald" || CITypeRSS=="LR" || CITypeRSS=="MLR"){
                                                            simCracks=genModifiedWaldDataset()
                                                          }
                                                          #generate the logit data and store the results in the environment
                                                          newLogitSubsetsGen=RankedSetRegGen$new()
                                                          newLogitSubsetsGen$initialize(testDataInput=dataFrame,
                                                                                      maxResamples=rsSComponentFromMain$maxResamples,
                                                                                      set_mInput=rsSComponentFromMain$set_m,
                                                                                      set_rInput=rsSComponentFromMain$set_r,
                                                                                      regTypeInput=rsSComponentFromMain$regressionType)
                                                          newLogitSubsetsGen$generateFullRSS()
                                                          RankedResultsSet=newLogitSubsetsGen$getRankedSetResults()
                                                          
                                                          #execute class and return logit results
                                                          #generate POD curve and store key values for a25,a50,a90, sigma, and a9095
                                                          logitResultsSet=newLogitSubsetsGen$getRSSLogitResults()
                                                          checkFailures=newLogitSubsetsGen$countBadLogits()
                                                          #if at least one logit model converged, generate a pod curve
                                                          if(checkFailures==FALSE){
                                                            newPODCurve<-GenPODCurveRSS$new()
                                                            #Standard wald
                                                            if(CITypeRSS=="StandardWald"){
                                                              # newPODCurve$initialize(logitResultsPODInput=logitResultsSet,
                                                              #                        excludeNAInput=rsSComponentFromMain$excludeNA,
                                                              #                        RSSDataFrames=RankedResultsSet)
                                                              # newPODCurve$genMainPODDFSDWald()
                                                              newPODCurve$initialize(logitResultsPODInput=logitResultsSet,
                                                                                     excludeNAInput=rsSComponentFromMain$excludeNA,
                                                                                     RSSDataFrames=RankedResultsSet,
                                                                                     origDataSetInput=dataFrame)
                                                              newPODCurve$newWaldGen()
                                                            }
                                                            #Modified wald
                                                            else if(CITypeRSS=="ModifiedWald"){
                                                              newPODCurve$initialize(logitResultsPODInput=logitResultsSet, 
                                                                                     excludeNAInput=rsSComponentFromMain$excludeNA,
                                                                                     RSSDataFrames=RankedResultsSet,
                                                                                     simCrackSizes=simCracks)
                                                              newPODCurve$genPODModWald()
                                                            }
                                                            else if(CITypeRSS=="LR"){
                                                              newPODCurve$initialize(logitResultsPODInput=logitResultsSet, 
                                                                                     excludeNAInput=rsSComponentFromMain$excludeNA,
                                                                                     RSSDataFrames=RankedResultsSet,
                                                                                     simCrackSizes=simCracks)
                                                              newPODCurve$genPODLR()
                                                            }
                                                            else if(CITypeRSS=="MLR"){
                                                              newPODCurve$initialize(logitResultsPODInput=logitResultsSet, 
                                                                                     excludeNAInput=rsSComponentFromMain$excludeNA,
                                                                                     RSSDataFrames=RankedResultsSet,
                                                                                     simCrackSizes=simCracks)
                                                              newPODCurve$genPODMLR()
                                                            }
                                                            setGoodnessOfFit(newPODCurve$getGoodnessOfFit())
                                                            setMedianCovarMatrix(newPODCurve$getMedianCovarMatrix())
                                                            setPODDataFrame(newPODCurve$getPODCurveDF())
                                                            genAValues(logitResultsSet, RankedResultsSet)
                                                          }
                                                          else{
                                                            print("all logit models failed to converge!")
                                                          }
                                                          
                                                        },
                                                        genModifiedWaldDataset=function(){
                                                          #generate normally distributed crack sizes
                                                          newNormalCrackDist=GenNormFit$new(cracks=dataFrame$x, sampleSize=normSampleAmount, 
                                                                                            Nsample=nrow(dataFrame))
                                                          newNormalCrackDist$genNormalFit()
                                                          simCracks=newNormalCrackDist$getSimCrackSizesArray()
                                                          x = sort(simCracks)[seq(from=1,to=length(simCracks),
                                                                                  length.out=normSampleAmount)]
                                                          return(x)
                                                        },
                                                        genAValues=function(logitResults, RankedResults){
                                                          newAValuesCalc=GenRSS_A_Values$new()
                                                          newAValuesCalc$initialize(pODRSSDFInput=getPODDataFrame(), 
                                                                                    excludeNAInput= rsSComponentFromMain$excludeNA,
                                                                                    logitResultsListInput=logitResults,
                                                                                    RSSDataFramesInput=RankedResults)
                                                          if(CITypeRSS=="StandardWald"){
                                                            #newAValuesCalc$genAValueswald()
                                                            newAValuesCalc$genAValuesStandWald()
                                                          }
                                                          else if(CITypeRSS=="ModifiedWald"){
                                                            newAValuesCalc$genAValuesModWald()
                                                          }
                                                          setMedianAValues(newAValuesCalc$getAValuesList())
                                                          
                                                        }
                                                       ))
