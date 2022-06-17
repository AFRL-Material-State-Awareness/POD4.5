#this class is used to calculate the MODIFIED Likelihood ratio confidence interval of the POD curve (WARNING: VERY SLOW)
#TODO: consider reimplmenting this in c++ in order to speed up to process
ModifiedLikelihoodRatioConfInt <- setRefClass("ModifiedLikelihoodRatioConfInt", fields = list(LogisticRegressionResult="glm", 
                                        CIDataFrame="data.frame"), #calcLinearCombo="mcprofile"), 
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
                                        executeLR=function(){
                                          ci.logit.profile.mlr = try(confint(hoa(calcLinearCombo), level = 0.95, adjust = "none",
                                                                             alternative="greater", 
                                                                             parallel = "multicore",ncpus=4),TRUE)
                                          if(is.numeric(ci.logit.profile.mlr$confint$lower)){
                                            profile.mlr.int <- try(data.frame(
                                              t_trans = LogisticRegressionResult$family$linkinv(ci.logit.profile.mlr$estimate$Estimate),
                                              Confidence_Interval= LogisticRegressionResult$family$linkinv(ci.logit.profile.mlr$confint$lower)),TRUE)
                                            #a9095_profile.mlr = try(approx(profile.mlr.int$lower,a_i_2,0.9)$y,TRUE)
                                            #if(!is.numeric(a9095_profile.mlr)){a9095_profile.mlr=NA}
                                          }#else{a9095_profile.mlr=NA}
                                          #disp("profile.mlr")
                                          setCIDataFrame( profile.mlr.int)
                                        }
                                      ))