LinearComboGenerator <- setRefClass("LinearComboGenerator", fields = list(LogisticRegressionResult="glm", simCracks="numeric", samples="numeric"),  methods = list(
                                                                          ####This function generates the linear combinations for either LR or MLR
                                                                          genLinearCombinations=function(){
                                                                            print("Starting linear Combos")
                                                                            print("Btw, Did you know geico can save you 15% or more on car insurance?")
                                                                            # PROFILE LIKELIHOOD RATIO CONFIDENCE INTERVALS
                                                                            # Create a matrix of linear combinations to profile
                                                                            # NOTE: The "linear.combo" variable can be used for both the LR and MLR CIs if the same number of profiles is desired (highly recommended since this is SLOW).
                                                                            newGenNormInstance=new("GenNormFit",cracks=LogisticRegressionResult$data$x, sampleSize=samples, Nsample=nrow(LogisticRegressionResult$data),
                                                                                                   simCrackSizesArray=0.0)
                                                                            newGenNormInstance$genNormalFit()
                                                                            a=newGenNormInstance$getSimCrackSizesArray()
                                                                            a_i_2 = sort(a)[linspace(1,length(a),samples)]
                                                                            simCracks<<-a_i_2
                                                                            K = matrix(c(rep(1,length(a_i_2)),a_i_2),ncol=2)
                                                                            linearCombo<-mcprofile(object = LogisticRegressionResult, CM = K)  # Calculate -2log(Lambda)
                                                                            typeof(linearCombo)
                                                                            return(linearCombo)
                                                                          },
                                                                          getSimCracks=function(){
                                                                            return(simCracks)
                                                                          }
                                                                        ))