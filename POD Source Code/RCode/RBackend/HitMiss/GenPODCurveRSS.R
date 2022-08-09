###Genertaes points for the POD curve and returns the result as a dataframe
#NOTE: the increment and step size plots 198 points total. It is designed to capture the critical A values to save time in the calculation
#the default constructor for c(0,0,0) is simply to fill the simCrackSizes metric
GenPODCurveRSS<-setRefClass("GenPODCurveRSS", fields = list(logitResultsPOD="list", excludeNA="logical",RSSDataFrames="list",
                            simCrackSizes="numeric",origDataSet= "data.frame", pODCurveDF="data.frame", covarMatrix="matrix", goodnessOfFit="numeric"),
                            methods = list(
                              initialize=function(logitResultsPODInput=list(),excludeNAInput=TRUE,RSSDataFramesInput=list() ,
                                                  simCrackSizesInput=c(0,0,0),origDataSetInput=data.frame(matrix(ncol = 1, nrow = 0))){
                                logitResultsPOD<<-logitResultsPODInput
                                excludeNA<<-excludeNAInput
                                RSSDataFrames<<-RSSDataFramesInput
                                simCrackSizes<<-simCrackSizesInput
                                origDataSet<<-origDataSetInput
                              },
                              getPODCurveDF=function(){
                                return(pODCurveDF)
                              },
                              setPODCurveDF=function(pspODCurveDF){
                                pODCurveDF<<-pspODCurveDF
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
                              newWaldGen=function(){
                                t_transDataFrameMed=data.frame("Index"=1:length(nrow(origDataSet)))
                                probsAt95CIDataFrameMed=data.frame("Index"=1:length(nrow(origDataSet)))
                                #store all of the values for the covariance matrices
                                covar11=c()
                                covar12=c()
                                covar21=c()
                                covar22=c()
                                #used to store the goodness of fit values
                                goodnessOfFitValues=c()
                                for(i in 1:length(logitResultsPOD)){
                                  linear_pred=predict(logitResultsPOD[[i]], type="link", se.fit=TRUE,
                                                      newdata=data.frame(x=origDataSet$x))
                                  #generate the variance-covariance matrix
                                  varcovmat =vcov(logitResultsPOD[[i]])
                                  #varCovarMatrixList=append(varCovarMatrixList, list(varcovmat))
                                  covar11=append(covar11, varcovmat[1,1])
                                  covar12=append(covar12, varcovmat[1,2])
                                  covar21=append(covar21, varcovmat[2,1])
                                  covar22=append(covar22, varcovmat[2,2])
                                  thisGoodnessOfFit=logitResultsPOD[[i]]$deviance/logitResultsPOD[[i]]$null.deviance
                                  goodnessOfFitValues=append(goodnessOfFitValues, thisGoodnessOfFit)
                                  sigmaOfCrack_i = sqrt(varcovmat[1,1]+2*varcovmat[1,2]*
                                                          logitResultsPOD[[i]]$data$x+varcovmat[2,2]*
                                                          logitResultsPOD[[i]]$data$x^2)
                                  currProbs = logitResultsPOD[[i]]$family$linkinv(linear_pred$fit)
                                  currProbsAt95CI = logitResultsPOD[[i]]$family$linkinv(linear_pred$fit-qnorm(0.95)*sigmaOfCrack_i)
                                  #print(length(probsAt95CI))
                                  t_transDataFrameMed=cbind(t_transDataFrameMed, currProbs)
                                  probsAt95CIDataFrameMed=cbind(probsAt95CIDataFrameMed, currProbsAt95CI)
                                }
                                setGoodnessOfFit(median(goodnessOfFitValues))
                                #globalVarCovar<<-varCovarMatrixList
                                medianCovarMatrix=matrix(nrow=2, ncol=2)
                                medianCovarMatrix[1,1]=median(covar11)
                                medianCovarMatrix[1,2]=median(covar12)
                                medianCovarMatrix[2,1]=median(covar21)
                                medianCovarMatrix[2,2]=median(covar22)
                                #globalcovar<<-medianCovarMatrix
                                setMedianCovarMatrix(medianCovarMatrix)
                                #create the POD and confint columns by taking the medians
                                t_transDataFrameMed$Index=NULL
                                probsAt95CIDataFrameMed$Index=NULL
                                #calculate medians and store them in final DF column 
                                t_trans=c()
                                Confidence_Interval=c()
                                #loop through the t_trans=POd and confident interval dataframe rows to generate the POD and CI curve
                                for(i in 1:nrow(origDataSet)){
                                  t_trans=c(t_trans, median(as.numeric(t_transDataFrameMed[i,]), na.rm = TRUE))
                                  Confidence_Interval=c(Confidence_Interval, 
                                                        median(as.numeric(probsAt95CIDataFrameMed[i,]), na.rm = TRUE))
                                }
                                PODCurve=data.frame(
                                  "x"=origDataSet$x,
                                  "t_trans"= t_trans,
                                  "Confidence_Interval"=Confidence_Interval
                                )
                                setPODCurveDF(PODCurve)
                              },
                              genPODModWald=function(){
                                #store all of the values for the covariance matrices
                                covar11=c()
                                covar12=c()
                                covar21=c()
                                covar22=c()
                                #used to store the goodness of fit values
                                goodnessOfFitValues=c()
                                t_transDataFrameMed=data.frame("Index"=1:length(simCrackSizes))
                                probsAt95CIDataFrameMed=data.frame("Index"=1:length(simCrackSizes))
                                for(i in 1:length(logitResultsPOD)){
                                  linear_pred=predict(logitResultsPOD[[i]], type="link", se.fit=TRUE, 
                                                      newdata=data.frame(x=simCrackSizes))
                                  varcovmat =vcov(logitResultsPOD[[i]])
                                  #varCovarMatrixList=append(varCovarMatrixList, list(varcovmat))
                                  covar11=append(covar11, varcovmat[1,1])
                                  covar12=append(covar12, varcovmat[1,2])
                                  covar21=append(covar21, varcovmat[2,1])
                                  covar22=append(covar22, varcovmat[2,2])
                                  thisGoodnessOfFit=logitResultsPOD[[i]]$deviance/logitResultsPOD[[i]]$null.deviance
                                  goodnessOfFitValues=append(goodnessOfFitValues, thisGoodnessOfFit)
                                  sigmaOfCrack_i = sqrt(varcovmat[1,1]+2*varcovmat[1,2]*
                                                          simCrackSizes+varcovmat[2,2]*
                                                          simCrackSizes^2)
                                  currt_trans = logitResultsPOD[[i]]$family$linkinv(linear_pred$fit)
                                  currProbsAt95CI = logitResultsPOD[[i]]$family$linkinv(linear_pred$fit-qnorm(0.95)*sigmaOfCrack_i)
                                  
                                  t_transDataFrameMed=cbind(t_transDataFrameMed, currt_trans)
                                  probsAt95CIDataFrameMed=cbind(probsAt95CIDataFrameMed, currProbsAt95CI)
                                }
                                setGoodnessOfFit(median(goodnessOfFitValues))
                                #globalVarCovar<<-varCovarMatrixList
                                medianCovarMatrix=matrix(nrow=2, ncol=2)
                                medianCovarMatrix[1,1]=median(covar11)
                                medianCovarMatrix[1,2]=median(covar12)
                                medianCovarMatrix[2,1]=median(covar21)
                                medianCovarMatrix[2,2]=median(covar22)
                                #globalcovar<<-medianCovarMatrix
                                setMedianCovarMatrix(medianCovarMatrix)
                                t_transDataFrameMed$Index=NULL
                                probsAt95CIDataFrameMed$Index=NULL
                                #calculate medians and store them in final DF column 
                                t_trans=c()
                                Confidence_Interval=c()
                                for(i in 1:length(simCrackSizes)){
                                  t_trans=c(t_trans, median(as.numeric(t_transDataFrameMed[i,]), na.rm = TRUE))
                                  Confidence_Interval=c(Confidence_Interval, 
                                                        median(as.numeric(probsAt95CIDataFrameMed[i,]), na.rm = TRUE))
                                }
                                PODCurve=data.frame(
                                  "x"=simCrackSizes,
                                  "t_trans"= t_trans,
                                  "Confidence_Interval"=Confidence_Interval
                                )
                                setPODCurveDF(PODCurve)
                              },
                              genPODLR=function(){
                                #logitResultsPODG<<-logitResultsPOD
                                print("starting LR RSS")
                                ##TODO: apply parallel processing to speed things up
                                #generate the Lr conf interval with RSS(WARNING: VERY SLOW)
                                #Initialize the K matrix
                                a=simCrackSizes
                                a_i_2 = sort(a)[seq(from=1,to=length(a),length.out=length(simCrackSizes))]
                                K = matrix(c(rep(1,length(a_i_2)),a_i_2),ncol=2)
                                finalConfInt=list()
                                print("no parallel")
                                for(i in 1:length(logitResultsPOD)){
                                  calcLinearCombo=unclass(genLinearCombosRSS(logitResultsPOD[[i]], K))
                                  newLRConfInt=LikelihoodRatioConfInt$new(LogisticRegressionResult=logitResultsPOD[[i]])
                                  newLRConfInt$setLinCombo(unclass(calcLinearCombo))
                                  newLRConfInt$executeLR()
                                  finalConfInt=append(finalConfInt, data.frame(newLRConfInt$getCIDataFrame()))
                                }
                                #finalConfIntGlobal<<-finalConfInt
                                t_transList=list()
                                confIntList=list()
                                for(i in 1:length(finalConfInt)){
                                  if(i%%2==1){
                                    t_transList=append(t_transList, finalConfInt[i])
                                  }
                                  else if (i%%2==0){
                                    confIntList=append(confIntList, finalConfInt[i])
                                  }
                                }
                                t_transDF=data.frame(t_transList)
                                confIntListDF=data.frame(confIntList)
                                #calculate medians and store them in final DF column 
                                t_trans=c()
                                Confidence_Interval=c()
                                for(i in 1:length(as.numeric(simCrackSizes))){
                                  t_trans=append(t_trans, median(as.numeric(t_transDF[i,]), na.rm = TRUE))
                                  Confidence_Interval=append(Confidence_Interval,
                                                       median(as.numeric(confIntListDF[i,]), na.rm = TRUE))
                                }
                                print("FINISHED!")
                                PODCurve=data.frame(
                                  "x"=simCrackSizes,
                                  "t_trans"= t_trans,
                                  "Confidence_Interval"=Confidence_Interval
                                )
                                #globalPODCurve<<-PODCurve
                                setPODCurveDF(PODCurve)
                                
                              },
                              genLinearCombosRSS=function(LogisticRegressionResult, KMatrix){
                                ptm <- proc.time()
                                linearCombo<-mcprofile(object = LogisticRegressionResult, CM = KMatrix)# Calculate -2log(Lambda)
                                print(proc.time() - ptm)
                                return(linearCombo)
                              },
                              genPODMLR=function(){
                                print("starting MLR RSS")
                                ##TODO: apply parallel processing to speed things up
                                #generate the Lr conf interval with RSS(WARNING: VERY SLOW)
                                #Initialize the K matrix
                                a=simCrackSizes
                                a_i_2 = sort(a)[seq(from=1,to=length(a),length.out=length(simCrackSizes))]
                                K = matrix(c(rep(1,length(a_i_2)),a_i_2),ncol=2)
                                finalConfInt=list()
                                print("no parallel")
                                for(i in 1:length(logitResultsPOD)){
                                  calcLinearCombo=unclass(genLinearCombosRSS(logitResultsPOD[[i]], K))
                                  newMLRConfInt=ModifiedLikelihoodRatioConfInt$new(LogisticRegressionResult=logitResultsPOD[[i]])
                                  newMLRConfInt$setLinCombo(unclass(calcLinearCombo))
                                  newMLRConfInt$executeMLR()
                                  finalConfInt=append(finalConfInt, data.frame(newMLRConfInt$getCIDataFrame()))
                                }
                                #finalConfIntGlobal<<-finalConfInt
                                t_transList=list()
                                confIntList=list()
                                for(i in 1:length(finalConfInt)){
                                  if(i%%2==1){
                                    t_transList=append(t_transList, finalConfInt[i])
                                  }
                                  else if (i%%2==0){
                                    confIntList=append(confIntList, finalConfInt[i])
                                  }
                                }
                                t_transDF=data.frame(t_transList)
                                confIntListDF=data.frame(confIntList)
                                #calculate medians and store them in final DF column 
                                t_trans=c()
                                Confidence_Interval=c()
                                for(i in 1:length(as.numeric(simCrackSizes))){
                                  t_trans=append(t_trans, median(as.numeric(t_transDF[i,]), na.rm = TRUE))
                                  Confidence_Interval=append(Confidence_Interval,
                                                             median(as.numeric(confIntListDF[i,]), na.rm = TRUE))
                                }
                                print("FINISHED!")
                                PODCurve=data.frame(
                                  "x"=simCrackSizes,
                                  "t_trans"= t_trans,
                                  "Confidence_Interval"=Confidence_Interval
                                )
                                setPODCurveDF(PODCurve)
                                
                              },
                              genLinearCombosRSS=function(LogisticRegressionResult, KMatrix){
                                ptm <- proc.time()
                                linearCombo<-mcprofile(object = LogisticRegressionResult, CM = KMatrix)# Calculate -2log(Lambda)
                                print(proc.time() - ptm)
                                return(linearCombo)
                              }
                          ))
