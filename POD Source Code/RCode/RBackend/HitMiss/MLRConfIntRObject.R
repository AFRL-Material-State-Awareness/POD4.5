#this class is used to calculate the MODIFIED Likelihood ratio confidence interval of the POD curve (WARNING: VERY SLOW)
#TODO: consider reimplmenting this in c++ in order to speed up to process
ModifiedLikelihoodRatioConfInt <- setRefClass("ModifiedLikelihoodRatioConfInt", fields = list(LogisticRegressionResult="glm", 
                                        CIDataFrame="data.frame", calcLinearCombo="list"), 
                                        methods = list(
                                        #this class will pass back the confidence interval values as a dataframe
                                        setCIDataFrame=function(psCIDataFrame){
                                          CIDataFrame<<-psCIDataFrame
                                        },
                                        getCIDataFrame=function(){
                                          return(CIDataFrame)
                                        },
                                        #set LinearComboValue
                                        setLinCombo=function(psLinCombo){
                                          calcLinearCombo<<-psLinCombo
                                        },
                                        executeMLR=function(){
                                          ptm <- proc.time()
                                          castCalcLinCombo<-calcLinearCombo
                                          class(castCalcLinCombo)<-"mcprofile"
                                          ci.logit.profile.mlr = try(confint(hoa(castCalcLinCombo), level = 0.95, adjust = "none",
                                                                             alternative="greater",
                                                                             #make this a parameter to let the user choose the number of cores
                                                                             parallel = "parallel",ncpus=getOption("profile.ncpus", detectCores()),full=TRUE))
                                          print("confint time calc for MLR")
                                          print(proc.time() - ptm)
                                          if(is.numeric(ci.logit.profile.mlr$confint$lower)){
                                            profile.mlr.int <- try(data.frame(
                                              t_trans = LogisticRegressionResult$family$linkinv(ci.logit.profile.mlr$estimate$Estimate),
                                              Confidence_Interval= LogisticRegressionResult$family$linkinv(ci.logit.profile.mlr$confint$lower)),TRUE)
                                            #a9095_profile.mlr = try(approx(profile.mlr.int$lower,a_i_2,0.9)$y,TRUE)
                                            #if(!is.numeric(a9095_profile.mlr)){a9095_profile.mlr=NA}
                                          }else{a9095_profile.mlr=NA}
                                          #disp("profile.mlr")
                                          setCIDataFrame(profile.mlr.int)
                                        }
                                      ))