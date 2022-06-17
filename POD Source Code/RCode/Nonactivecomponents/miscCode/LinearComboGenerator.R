
#This class is used to claculate the linear combinations when the user selects LR or MLR CI 
LinearComboGenerator <- setRefClass("LinearComboGenerator", fields = list(LogisticRegressionResult="glm", samples="numeric"),  methods = list(
                                                                          #initialize =function()
                                                                          #{
                                                                          #  callSuper(linearCombo=mcprofile(object=LogisticRegressionResult, CM=matrix(c(rep(1,length(a_i_2)),a_i_2),ncol=2)))
                                                                          #}
                                                                          ####This function generates the linear combinations for either LR or MLR
                                                                          genLinearCombinations=function(){
                                                                            print("Starting linear Combos")
                                                                            print("Btw, Did you know geico can save you 15% or more on car insurance?")
                                                                            # PROFILE LIKELIHOOD RATIO CONFIDENCE INTERVALS
                                                                            # Create a matrix of linear combinations to profile
                                                                            # NOTE: The "linear.combo" variable can be used for both the LR and MLR CIs if the same number of profiles is desired (highly recommended since this is SLOW).
                                                                            normalFit_x = fitdistr(LogisticRegressionResult$data$x,"normal")
                                                                            a = rnorm(mean=normalFit_x$estimate[1],sd=normalFit_x$estimate[2],
                                                                                      n=max(samples,10))
                                                                            ptm <- proc.time()
                                                                            a_i_2 = sort(a)[linspace(1,length(a),samples)]
                                                                            K = matrix(c(rep(1,length(a_i_2)),a_i_2),ncol=2)
                                                                            linearCombo<-mcprofile(object = LogisticRegressionResult, CM = K)  # Calculate -2log(Lambda)
                                                                            #disp("linear.combo") 
                                                                            print(proc.time() - ptm)
                                                                            print("finished linear Combo")
                                                                            return(linearCombo)
                                                                          }
                                                                        ))