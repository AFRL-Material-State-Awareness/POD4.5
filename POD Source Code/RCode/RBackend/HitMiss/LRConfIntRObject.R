#this class is used to calculate the Likelihood ratio confidence interval of the POD curve (WARNING: SLOW)
#TODO: consider reimplmenting this in c++ in order to speed up to process
LikelihoodRatioConfInt <- setRefClass("LikelihoodRatioConfInt",#contains="mcprofile",#inheritPackage = TRUE,
                          fields = list(LogisticRegressionResult="glm", 
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
                           executeLR=function(){
                             ptm <- proc.time()
                             castCalcLinCombo<-calcLinearCombo
                             class(castCalcLinCombo)<-"mcprofile"
                             ci.logit.profile<-try(confintmcprofile(object = castCalcLinCombo, level = 0.95, adjust = "none",
                                                           alternative="greater", trace=TRUE, 
                                                           parallel = "parallel",ncpus=getOption("profile.ncpus", 
                                                           detectCores()-1) , full=TRUE))  # CI for beta_0 + beta_1 * x
                             print("confint time calc for LR")
                             print(proc.time() - ptm)
                             if(is.numeric(ci.logit.profile$confint$lower)){
                               #print(ci.logit.profile)
                               profile.lr.int <- try(data.frame(
                                 t_trans = LogisticRegressionResult$family$linkinv(ci.logit.profile$estimate$Estimate),
                                 Confidence_Interval= LogisticRegressionResult$family$linkinv(ci.logit.profile$confint$lower)),TRUE)
                               #a9095_profile.lr = try(approx(profile.lr.int$lower,a_i_2,0.9)$y,TRUE)
                               #if(!is.numeric(a9095_profile.lr)){a9095_profile.lr=NA}
                             }#else{a9095_profile.lr=NA}
                             setCIDataFrame(profile.lr.int)
                           }
                          ))
