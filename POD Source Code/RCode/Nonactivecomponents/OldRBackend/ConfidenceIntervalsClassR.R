#the following code is a class for determining the confidence intervals of a best fit curve such as logit for hit/miss data
#The four confidence intervals the user can choose is: Standard Wald, Modified Wald, Likelihood Ratio, and Modified Likelihood Ratio
ConfidenceIntervals <- setRefClass("ConfidenceIntervals", fields = list(LogisticRegressionResult="glm", 
                                                                      CIType="character", CIListDouble="list", CIDataFrame="data.frame"),  methods = list(
                                                                        #does not return useful in c# used for internal testing only                        
                                                                        getLogitList=function(){
                                                                          return(LogisticRegressionResult, executeStandardWald)
                                                                        },
                                                                        getConfidenceInterval=function(){
                                                                          return(CIDataFrame)
                                                                        },
                                                                        determineCIType=function(executeStandardWald, executeLR){
                                                                          if(CIType=="Standard Wald"){
                                                                            print("execute wald")
                                                                            executeStandardWald()
                                                                          }
                                                                          if(CIType=="Modified Wald"){
                                                                            #executeModifiedWald()
                                                                          }
                                                                          if(CIType=="LR"){
                                                                            print("Execute LR!")
                                                                            executeLR()
                                                                          }
                                                                          if(CIType=="MLR"){
                                                                            print("Execute MLR!")
                                                                            executeMLR()
                                                                          }
                                                                        },
                                                                        #determines standard wald confidence interval
                                                                        executeStandardWald=function(){
                                                                          #list object to be assigned to the CI instance
                                                                          standardWaldList=list()
                                                                          # Predict the fitted values and the standard errors for the model 
                                                                          LReg.predictions=predict(LogisticRegressionResult, type="link",se.fit=TRUE)
                                                                          # Estimate the one-sided Wald 95% confidence interval
                                                                          # Calculate the 95% lower Wald confidence interval (in linear space, 
                                                                          # which will appear as an upper interval in probability space)
                                                                          standardWaldCI = LReg.predictions$fit - (qnorm(.95) * LReg.predictions$se.fit)
                                                                          #get beta0 and bet1
                                                                          beta1=LogisticRegressionResult[[1]][[2]]
                                                                          beta0= LogisticRegressionResult[[1]][[1]]
                                                                          #transform the model to logit space from linear space
                                                                          for(i in 1:length(standardWaldCI)){
                                                                            standardWaldCI[[i]]= 1/(1+exp(-(beta1*standardWaldCI[[i]]+beta0)))
                                                                          }
                                                                          cInterval<-standardWaldCI
                                                                          CIDataFrame<<-data.frame(cInterval)
                                                                        },
                                                                        executeModifiedWald=function(){
                                                                          #Ask christie about this confidence interval technique
                                                                          variable1=1
                                                                        },
                                                                        executeLR=function(){
                                                                          # PROFILE LIKELIHOOD RATIO CONFIDENCE INTERVALS
                                                                          # Create a matrix of linear combinations to profile
                                                                          # NOTE: The "linear.combo" variable can be used for both the LR and 
                                                                          #MLR CIs if the same number of profiles is desired (highly recommended since this is SLOW).
                                                                          inputSamples=500
                                                                          newLinearComboInstance=new("LinearComboGenerator", LogisticRegressionResult=LogisticRegressionResult, 
                                                                                                     simCracks=0.0,samples=inputSamples)
                                                                         
                                                                          calcLinearCombo= newLinearComboInstance$genLinearCombinations()
                                                                          ci.logit.profile<-try(confint(object = calcLinearCombo, level = 0.95, adjust = "none",
                                                                                                        alternative="greater", trace=TRUE, 
                                                                                                        parallel = "multicore",ncpus=4),TRUE)  # CI for beta_0 + beta_1 * x
                                                                          if(is.numeric(ci.logit.profile$confint$lower)){
                                                                            #print(ci.logit.profile)
                                                                            profile.lr.int <- try(data.frame(
                                                                              simCracks=newLinearComboInstance$getSimCracks(),
                                                                              estimate = LogisticRegressionResult$family$linkinv(ci.logit.profile$estimate$Estimate),
                                                                              lower    = LogisticRegressionResult$family$linkinv(ci.logit.profile$confint$lower)),TRUE)
                                                                            a9095_profile.lr = try(approx(profile.lr.int$lower,a_i_2,0.9)$y,TRUE)
                                                                            if(!is.numeric(a9095_profile.lr)){a9095_profile.lr=NA}
                                                                          }else{a9095_profile.lr=NA}
                                                                          #disp("profile.lr")
                                                                          cInterval<-profile.lr.int
                                                                          CIDataFrame<<-cInterval
                                                                        },
                                                                        executeMLR=function(){
                                                                          # Profile through the "linear.combo" matrix 
                                                                          # NOTE: Creating the Modified Profile Likelihood Ratio confidence interval is 
                                                                          #compuationally intense (MLR even more than LR), so it is using multiple cores (ncpus=4, here). 
                                                                          #We may need to consider what the optimal setting is for government computers.
                                                                          inputSamples=500
                                                                          newLinearComboInstance=new("LinearComboGenerator", LogisticRegressionResult=LogisticRegressionResult, 
                                                                                                     simCracks=0.0,samples=inputSamples)
                                                                          calcLinearCombo= newLinearComboInstance$genLinearCombinations()
                                                                          ci.logit.profile.mlr = try(confint(hoa(calcLinearCombo), level = 0.95, adjust = "none",
                                                                                                             alternative="greater", 
                                                                                                             parallel = "multicore",ncpus=4),TRUE)
                                                                          if(is.numeric(ci.logit.profile.mlr$confint$lower)){
                                                                            profile.mlr.int <- try(data.frame(
                                                                              simCracks=newLinearComboInstance$getSimCracks(),
                                                                              estimate = LogisticRegressionResult$family$linkinv(ci.logit.profile.mlr$estimate$Estimate),
                                                                              lower    = LogisticRegressionResult$family$linkinv(ci.logit.profile.mlr$confint$lower)),TRUE)
                                                                            
                                                                            a9095_profile.mlr = try(approx(profile.mlr.int$lower,a_i_2,0.9)$y,TRUE)
                                                                            if(!is.numeric(a9095_profile.mlr)){a9095_profile.mlr=NA}
                                                                          }else{a9095_profile.mlr=NA}
                                                                          #disp("profile.mlr") 
                                                                          cInterval<-profile.mlr.int
                                                                          CIDataFrame<<-cInterval
                                                                        }
                                                                      ))

