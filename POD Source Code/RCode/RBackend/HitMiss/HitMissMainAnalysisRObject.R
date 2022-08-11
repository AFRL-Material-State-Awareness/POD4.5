#main hitmiss analysis object when pass into R ( entry point is detAnalysisApproach() )
HMAnalysis <- setRefClass("HMAnalysis",
                          fields = list(hitMissDF = "data.frame", 
                                        CIType = "character",
                                        modelType="character",
                                        #also known as profile size
                                        N="numeric",
                                        #also known as a_x_n
                                        normSampleAmount="numeric",
                                        iterationTable="data.frame",
                                        results="data.frame",
                                        residualTable="data.frame",
                                        a_values="list",
                                        rankedSetSampleObject="RSSComponents",
                                        covarianceMatrix="matrix",
                                        goodnessOfFit="numeric"),
                          methods = list(
                            getHitMissDF = function(){
                              #modify table for clean excel output later
                              hitMissDF_trans=data.frame(
                                #Index=1:nrow(hitMissDF),
                                flaw=hitMissDF$x,
                                transformFlaw=integer(nrow(hitMissDF)),
                                hitrate=hitMissDF$y
                              )
                              return(hitMissDF_trans)
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
                              results$Index <<- NULL
                              names(results)[names(results) == 'x'] <<- 'flaw'
                              names(results)[names(results) == 't_trans'] <<- 'pod'
                              names(results)[names(results) == 'y'] <<- 'hitrate'
                              return(results)
                            },
                            setResidualTable=function(psResidTable){
                              residualTable<<-psResidTable
                            },
                            getResidualTable=function(){
                              if(CIType=="StandardWald"){
                                retResidualTable=data.frame(
                                  flaw=residualTable$x,
                                  transformFlaw= residualTable$transformFlaw,
                                  hitrate= hitMissDF$y,
                                  t_trans= residualTable$t_trans,
                                  diff=hitMissDF$y-residualTable$t_trans
                                )
                              }
                              else{
                                retResidualTable=data.frame(
                                  flaw=residualTable$x,
                                  transformFlaw= residualTable$transformFlaw,
                                  t_trans= residualTable$t_trans,
                                  Confidence_Interval=residualTable$Confidence_Interval
                                )
                              }
                              return(retResidualTable)
                            },
                            setIterationTable =function(psiterTable){
                              iterationTable<<-psiterTable
                            },
                            getIterationTable=function(){
                              return(iterationTable)
                            },
                            setKeyAValues=function(psAValues){
                              #overwrite a9095 to the largest possible double if the value doesn't exist
                              if(is.na(psAValues[5])){
                                psAValues[5]<- .Machine$double.xmax
                              }
                              a_values<<-psAValues
                            },
                            getKeyAValues=function(){
                              return(a_values)
                            },
                            setCovMatrix=function(psMatrix){
                              covarianceMatrix<<-psMatrix
                            },
                            getCovMatrix=function(){
                              if(is.null(covarianceMatrix)){
                                #return an empty matrix of negative ones if empty
                                covarianceMatrix<<-matrix(0, nrow=2, ncol=2)
                              }
                              return(as.data.frame(covarianceMatrix))
                            },
                            setGoodnessOfFit=function(psGOF){
                              goodnessOfFit<<-psGOF
                            },
                            getGoodnessOfFit=function(){
                              return(goodnessOfFit)
                            },
                            #this is the entry point from c# if the user wants to do ranked set sampling
                            initializeRSS=function(){
                              #print("Startin RSS")
                              #print(rankedSetSampleObject)
                              newRSSMainObject=RSSMainClassObject$new(dataFrame=hitMissDF, 
                                                                      rsSComponentFromMain=rankedSetSampleObject,
                                                                      CITypeRSS=CIType,
                                                                      normSampleAmount=normSampleAmount)
                              newRSSMainObject$executeRSSPOD()
                              setResults(newRSSMainObject$getPODDataFrame())
                              setResidualTable(newRSSMainObject$getPODDataFrame())
                              setKeyAValues(newRSSMainObject$getMedianAValues())
                              setIterationTable(data.frame(
                                trial=1,
                                indexiteration=-1,
                                indexmu=-1,
                                sigma=-1,
                                fnorm=-1,
                                damping=1
                              ))
                              setCovMatrix(newRSSMainObject$getMedianCovarMatrix())
                              setGoodnessOfFit(newRSSMainObject$getGoodnessOfFit())
                            },
                            determineRegressionType=function(){
                              if(modelType=="Logistic Regression"){
                                detRegressionResults=executeLogitHM()
                              }
                              else if(modelType=="Firth Logistic Regression"){
                                detRegressionResults=executeFirthHM()
                              }
                              else{
                                stop("model type not found!")
                              }
                              return(detRegressionResults)
                            },
                            detAnalysisApproach=function(){
                              #execute regression with original dataset depending which type
                              #is called (logit, firth, lasso, etc)
                              regressionResults<-determineRegressionType()
                              #add iteration metrics(TEMP)
                              setIterationTable(data.frame(
                                trial=1,
                                indexiteration=regressionResults$iter,
                                indexmu=-1,
                                sigma=-1,
                                fnorm=-1,
                                damping=1
                              ))
                              #calculate the goodness of fit
                              setGoodnessOfFit(regressionResults$deviance/regressionResults$null.deviance)
                              if(CIType=="StandardWald"){
                                #print("start conf int")
                                #set t_trans
                                t_trans=regressionResults$fitted.values
                                #calculate wald using conf intervals class and store the covariance matrix
                                newWaldCI= WaldConfInt$new(LogisticRegressionResult=regressionResults)
                                newWaldCI$executeStandardWald()
                                Confidence_Interval=newWaldCI$getCIDataFrame()
                                setCovMatrix(newWaldCI$getCovarMatrix())
                                #set the dataframe to return to c#
                                x=hitMissDF$x
                                #this will be overwritten in c# sharp with the appropriate flaw transform
                                transformFlaw=integer(nrow(hitMissDF))
                                setResults(cbind(x, transformFlaw, t_trans, Confidence_Interval))
                                setResidualTable(cbind(x, transformFlaw, t_trans, Confidence_Interval))
                                #get key a values
                                new_Acalc=GenAValuesOnPODCurve$new(LogisticRegressionResult=regressionResults,inputDataFrameLogistic=hitMissDF)
                                new_Acalc$calcAValuesStandardWald()
                                setKeyAValues(new_Acalc$getAValuesList())
                              }
                              else if(CIType=="ModifiedWald"){
                                #calc logit regression
                                #execute logit regression with original dataset
                                #generate normally distributed crack sizes
                                newNormalCrackDist=GenNormFit$new(cracks=hitMissDF$x, sampleSize=normSampleAmount, 
                                                                  Nsample=N)
                                #newNormalCrackDist$initialize()
                                newNormalCrackDist$genNormalFit()
                                simCracks=newNormalCrackDist$getSimCrackSizesArray()
                                #simCracks<-append(simCracks, max(hitMissDF[,2]))
                                x = sort(simCracks)[seq(from=1,to=length(simCracks),length.out=normSampleAmount)]
                                #modified wald calculation
                                newWaldCI= WaldConfInt$new(LogisticRegressionResult=regressionResults,simCrackVals=x)
                                newWaldCI$executeModifiedWald()
                                Confidence_Interval=newWaldCI$getCIDataFrame()
                                setCovMatrix(newWaldCI$getCovarMatrix())
                                #this will be overwritten in c# sharp with the appropriate flaw transform
                                transformFlaw=integer(length(x))
                                #set the dataframe to return to c#
                                setResults(cbind(x, transformFlaw, Confidence_Interval))
                                setResidualTable(cbind(x, transformFlaw,  Confidence_Interval))
                                #get key a values
                                new_Acalc=GenAValuesOnPODCurve$new(LogisticRegressionResult=regressionResults,inputDataFrameLogistic=cbind(x,Confidence_Interval))
                                new_Acalc$calcAValuesModWald()
                                setKeyAValues(new_Acalc$getAValuesList())
                              }
                              else if(CIType=="LR"){
                                #ptm <- proc.time()
                                #set the covariance matrix
                                setCovMatrix(vcov(regressionResults))
                                newLinearComboInstance=new("LinearComboGenerator", LogisticRegressionResult=regressionResults, 
                                                           simCracks=0.0,samples=normSampleAmount)
                                calcLinearCombo=unclass(newLinearComboInstance$genLinearCombinations())
                                x=newLinearComboInstance$getSimCracks()
                                newLRConfInt=LikelihoodRatioConfInt$new(LogisticRegressionResult=regressionResults)
                                newLRConfInt$setLinCombo(calcLinearCombo)
                                newLRConfInt$executeLR()
                                #print("Time of Calculation")
                                #print(proc.time() - ptm)
                                Confidence_Interval=newLRConfInt$getCIDataFrame()
                                #this will be overwritten in c# sharp with the appropriate flaw transform
                                transformFlaw=integer(length(x))
                                setResults(cbind(x, transformFlaw, Confidence_Interval))
                                setResidualTable(cbind(x, transformFlaw, Confidence_Interval))
                                #get key a values
                                new_Acalc=GenAValuesOnPODCurve$new(LogisticRegressionResult=regressionResults,inputDataFrameLogistic=cbind(x,Confidence_Interval))
                                new_Acalc$calca9095LR()
                                setKeyAValues(new_Acalc$getAValuesList())
                              }
                              else if(CIType=="MLR"){
                                #ptm <- proc.time()
                                #set the covariance matrix
                                setCovMatrix(vcov(regressionResults))
                                newLinearComboInstance=new("LinearComboGenerator", LogisticRegressionResult=regressionResults, 
                                                           simCracks=0.0,samples=normSampleAmount)
                                calcLinearCombo= unclass(newLinearComboInstance$genLinearCombinations())
                                x=newLinearComboInstance$getSimCracks()
                                newMLRConfInt=ModifiedLikelihoodRatioConfInt$new(LogisticRegressionResult=regressionResults)
                                newMLRConfInt$setLinCombo(calcLinearCombo)
                                newMLRConfInt$executeMLR()
                                #print("Time of Calculation")
                                #print(proc.time() - ptm)
                                Confidence_Interval=newMLRConfInt$getCIDataFrame()
                                #this will be overwritten in c# sharp with the appropriate flaw transform
                                transformFlaw=integer(length(x))
                                setResults(cbind(x, transformFlaw, Confidence_Interval))
                                setResidualTable(cbind(x, transformFlaw, Confidence_Interval))
                                #get key a values
                                new_Acalc=GenAValuesOnPODCurve$new(LogisticRegressionResult=regressionResults,inputDataFrameLogistic=cbind(x,Confidence_Interval))
                                new_Acalc$calca9095MLR()
                                setKeyAValues(new_Acalc$getAValuesList())
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
                            executeFirthHM=function(){
                              newFirth=HMFirthApproximation$new()
                              newFirth$initialize(inputDataFrameFirthInput = hitMissDF)
                              newFirth$calcFirthResults()
                              return(newFirth$getFirthResults())
                            },
                            #used for Debugging ONLY
                            plotSimdata=function(df){
                              myPlot=ggplot(data=df, mapping=aes(x=flaw, y=pod))+geom_point()+
                                ggtitle(paste("POD Curve:", CIType," model type:", modelType))#+scale_x_continuous(limits = c(0,1.0))+scale_y_continuous(limits = c(0,1))
                              print(myPlot)
                            },
                            plotCI=function(df){
                              myPlot=ggplot(data=df, mapping=aes(x=flaw, y=Confidence_Interval))+geom_point()+
                                ggtitle(paste("Confidence interval:", CIType," model type:", modelType))+scale_x_continuous(limits = c(0,1.0))+scale_y_continuous(limits = c(0,1))
                              print(myPlot)
                            }
                          ))
