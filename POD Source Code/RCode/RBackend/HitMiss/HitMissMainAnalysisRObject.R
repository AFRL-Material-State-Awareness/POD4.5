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

# main hitmiss analysis object when passed into R. The entry point is at initializeRSS() for ranked set sampling, executeFitAnalysisOnly() when
# the user is selecting a transform, and executeFullAnalysis() when the user does a full analysis with simple random sampling

# parameters:
# ~hitMissDF = the original hitmiss dataframe that was put into the analysis (index, flaw, hit/miss)
# ~CIType= The confidence interval type used when executing an analysis (does not apply when executeFitAnalysisOnly is being used). Options
# are "StandardWald", "ModifiedWald", "LR", and "MLR"
# ~N = The profile size, this is simply the amount of samples the hitmiss dataframe contains.
# ~normSampleAmount= Also known as a_x_n, this is the amount of normally distributed crack sizes generated in the event the user uses modified wald,
# LR, or MLR.
# ~iterationTable= the table that contains the number of iterations the algorithm performed before converging (or until it gave up on trying 
# to converge)
# ~results = The POD table (flaw, transformedflaw, POD, confidence_interval)
# ~residualTable = The table containing the difference between a hit/miss value and the t_trans for (i.e. how far away is a hit or miss from 
# a certain point on the POD curve)
# ~a_values = the list of critical a_values to be returned to the UI (a25, a50, a90, sigma, a9095)
# ~rankedSetSamplingObject= a class containing all of the unique components used for ranked set sampling (max resamples, m, r, regressionType, includeNAInput)
# ~covarianceMatrix = the values of the variance-covariance matrix as a list
# ~goodnessOfFit = the value for the goodness of fit. Found by taking the likelihood ratio
# ~separation = flag used to inform the user if the data is separated or not (0 = no sepeartion, 1 = separation)
# ~convergenceFailLogit = flag used to inform the user if the logistic regression failed to converge (0 = alogirithm converged, 1 =algorithm did not converge)

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
                                        goodnessOfFit="numeric",
                                        separation="numeric",
                                        convergeFailLogit="numeric"),
                          methods = list(
                            ##make defaults values function here
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
                              if(nrow(results!=0)){
                                results$Index <<- NULL
                                names(results)[names(results) == 'x'] <<- 'flaw'
                                names(results)[names(results) == 't_trans'] <<- 'pod'
                                names(results)[names(results) == 'y'] <<- 'confidence'
                              }
                              #return dataframe with 0's to prevent erros in c#
                              else{
                                results <<- data.frame(
                                  flaw= c(0,0,0,0,0),
                                  pod=c(0,0,0,0,0),
                                  Confidence_Interval=c(0,0,0,0,0)
                                )
                              }
                              return(results)
                            },
                            setResidualTable=function(psResidTable){
                              residualTable<<-psResidTable
                            },
                            getResidualTable=function(){
                              if(nrow(residualTable)==0){
                                retResidualTable=data.frame(
                                  flaw=c(0,0,0,0,0),
                                  transformFlaw=c(0,0,0,0,0),
                                  t_trans= c(0,0,0,0,0),
                                  Confidence_Interval=c(0,0,0,0,0)
                                )
                                
                                return(retResidualTable)
                              }
                              #rename column to flaw before returning
                              names(residualTable)[names(residualTable) == 'x'] <<- 'flaw'
                              return(residualTable)
                            },
                            setIterationTable =function(psiterTable){
                              iterationTable<<-psiterTable
                            },
                            getIterationTable=function(){
                              if(nrow(iterationTable)==0){
                                iterationTable<<-data.frame(
                                  trial=0,
                                  indexiteration=0,
                                  indexmu=0,
                                  sigma=0,
                                  fnorm=0,
                                  damping=0
                                )
                              }
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
                              if(length(a_values)==0){
                                a_values<<-list(-1,-1,-1,-1,-1)
                              }
                              return(a_values)
                            },
                            setCovMatrix=function(psMatrix){
                              covarianceMatrix<<-psMatrix
                            },
                            getCovMatrix=function(){
                              if(length(covarianceMatrix)==0){
                                #return an empty matrix of negative ones if empty
                                covarianceMatrix<<-matrix(0, nrow=2, ncol=2)
                              }
                              return(as.data.frame(covarianceMatrix))
                            },
                            setGoodnessOfFit=function(psGOF){
                              goodnessOfFit<<-psGOF
                            },
                            getGoodnessOfFit=function(){
                              if(length(goodnessOfFit)==0){
                                return(0.0)
                              }
                              return(goodnessOfFit)
                            },
                            setSeparation=function(psSeparated){
                              separation<<-psSeparated
                            },
                            getSeparation=function(){
                              if(length(separation)==0){
                                return(0)
                              }
                              return(separation)
                            },
                            setConvergedFail=function(psConverged){
                              convergeFailLogit<<-psConverged
                            },
                            getConvergedFail=function(){
                              if(length(convergeFailLogit)==0){
                                return(0)
                              }
                              return(convergeFailLogit)
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
                              setResidualTable(newRSSMainObject$getMedianResidDataframe())
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
                            #executes when generating sigmoid curves in the transform panel
                            executeFitAnalysisOnly=function(){
                              regressionResults<-determineRegressionType()
                              residualTableTemp<-data.frame(
                                x=hitMissDF$x,
                                transformFlaw=integer(nrow(hitMissDF)),
                                t_trans=regressionResults$fitted.values,
                                Confidence_Interval=rep(0,nrow(hitMissDF))
                              )
                              setResidualTable(residualTableTemp)
                            },
                            executeFullAnalysis=function(){
                              #execute regression with original dataset depending which type
                              #is called (logit, firth, lasso, etc)
                              regressionResults<-determineRegressionType()
                              generateResidualDF(regressionResults)
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
                                #get key a values
                                new_Acalc=GenAValuesOnPODCurve$new(LogisticRegressionResult=regressionResults,inputDataFrameLogistic=cbind(x,Confidence_Interval))
                                new_Acalc$calcAValuesModWald()
                                setKeyAValues(new_Acalc$getAValuesList())
                              }
                              else if(CIType=="LR"){
                                #set the covariance matrix
                                setCovMatrix(vcov(regressionResults))
                                newLinearComboInstance=new("LinearComboGenerator", LogisticRegressionResult=regressionResults, 
                                                           simCracks=0.0,samples=normSampleAmount)
                                calcLinearCombo=unclass(newLinearComboInstance$genLinearCombinations())
                                x=newLinearComboInstance$getSimCracks()
                                newLRConfInt=LikelihoodRatioConfInt$new(LogisticRegressionResult=regressionResults)
                                newLRConfInt$setLinCombo(calcLinearCombo)
                                newLRConfInt$executeLR()
                                Confidence_Interval=newLRConfInt$getCIDataFrame()
                                #this will be overwritten in c# sharp with the appropriate flaw transform
                                transformFlaw=integer(length(x))
                                setResults(cbind(x, transformFlaw, Confidence_Interval))
                                #get key a values
                                new_Acalc=GenAValuesOnPODCurve$new(LogisticRegressionResult=regressionResults,inputDataFrameLogistic=cbind(x,Confidence_Interval))
                                new_Acalc$calca9095LR()
                                setKeyAValues(new_Acalc$getAValuesList())
                              }
                              else if(CIType=="MLR"){
                                #set the covariance matrix
                                setCovMatrix(vcov(regressionResults))
                                newLinearComboInstance=new("LinearComboGenerator", LogisticRegressionResult=regressionResults, 
                                                           simCracks=0.0,samples=normSampleAmount)
                                calcLinearCombo= unclass(newLinearComboInstance$genLinearCombinations())
                                x=newLinearComboInstance$getSimCracks()
                                newMLRConfInt=ModifiedLikelihoodRatioConfInt$new(LogisticRegressionResult=regressionResults)
                                newMLRConfInt$setLinCombo(calcLinearCombo)
                                newMLRConfInt$executeMLR()
                                Confidence_Interval=newMLRConfInt$getCIDataFrame()
                                #this will be overwritten in c# sharp with the appropriate flaw transform
                                transformFlaw=integer(length(x))
                                setResults(cbind(x, transformFlaw, Confidence_Interval))
                                #get key a values
                                new_Acalc=GenAValuesOnPODCurve$new(LogisticRegressionResult=regressionResults,inputDataFrameLogistic=cbind(x,Confidence_Interval))
                                new_Acalc$calca9095MLR()
                                setKeyAValues(new_Acalc$getAValuesList())
                              }
                              else{
                                stop("Confidence interval type not found!")
                              }
                            },
                            #this generates the residual hit miss dataframe regardless of confidence interval type
                            generateResidualDF=function(regressionResults){
                              #regressionResultsGlobal<<-regressionResults
                              #make the residual table based on the parameters
                              makeResidualTable=data.frame(
                                flaw=hitMissDF$x,
                                transformFlaw= integer(nrow(hitMissDF)),
                                hitrate= hitMissDF$y,
                                t_trans= regressionResults$fitted.values,
                                diff=hitMissDF$y-regressionResults$fitted.values
                              )
                              setResidualTable(makeResidualTable)
                            },
                            #executes the logit approximation of the hitmiss dataframe and returns
                            #the LReg.mod list
                            executeLogitHM=function(){
                              #execute logit regression with original dataset
                              newLogitModel=HMLogitApproximation$new(inputDataFrameLogistic=hitMissDF, separated=0)
                              newLogitModel$calcLogitResults()
                              #get the separated flag in case data is separated
                              setSeparation(newLogitModel$getSeparatedFlag())
                              setConvergedFail(newLogitModel$getConvergedFailFlag())
                              return(newLogitModel$getLogitResults())
                            },
                            executeFirthHM=function(){
                              newFirth=HMFirthApproximation$new()
                              newFirth$initialize(inputDataFrameFirthInput = hitMissDF)
                              newFirth$calcFirthResults()
                              #might need change this later if there are cases when firth logistic regression doesn't converge
                              setSeparation(0)
                              setConvergedFail(0)
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
                                ggtitle(paste("Confidence interval:", CIType," model type:", modelType))#+scale_x_continuous(limits = c(0,1.0))+scale_y_continuous(limits = c(0,1))
                              print(myPlot)
                            }
                          ))
