
LinearComboGenerator <- setRefClass("LinearComboGenerator", fields = list(LogisticRegressionResult="glm", 
                                                                          simCracks="numeric", samples="numeric"),  methods = list(
                                                            ####This function generates the linear combinations for either LR or MLR
                                                            genLinearCombinations=function(){
                                                              print("Starting linear Combos")
                                                              print("Btw, Did you know geico can save you 15% or more on car insurance?")
                                                              # PROFILE LIKELIHOOD RATIO CONFIDENCE INTERVALS
                                                              # Create a matrix of linear combinations to profile
                                                              # NOTE: The "linear.combo" variable can be used for both the LR and MLR CIs 
                                                              #if the same number of profiles is desired (highly recommended since this is SLOW).
                                                              newGenNormInstance=new("GenNormFit",cracks=LogisticRegressionResult$data$x, 
                                                                                    sampleSize=samples, Nsample=nrow(LogisticRegressionResult$data),
                                                                                    simCrackSizesArray=0.0)
                                                              newGenNormInstance$genNormalFit()
                                                              a=newGenNormInstance$getSimCrackSizesArray()
                                                              a_i_2 = sort(a)[seq(from=1,to=length(a),length.out=samples)]
                                                              simCracks<<-a_i_2
                                                              K = matrix(c(rep(1,length(a_i_2)),a_i_2),ncol=2)
                                                              ptm <- proc.time()
                                                              linearCombo<-minimcprofile(object = LogisticRegressionResult, CM = K)# Calculate -2log(Lambda)
                                                              #print("mcprofile time calc parallel")
                                                              p=proc.time() - ptm
                                                              print(p)
                                                              #linearCombo<-mcprofile(object = LogisticRegressionResult, CM = K)# Calculate -2log(Lambda)
                                                              #linearComboGlobal<<-linearCombo
                                                              return(linearCombo)
                                                            },
                                                            getSimCracks=function(){
                                                              return(simCracks)
                                                            }#,
                                                            # parallelAttempt=function(KMatrix){
                                                            #   #get number of cores
                                                            #   no_cores = detectCores()
                                                            #   #print("starting new lin combo")
                                                            #   # k1=KMatrix[1:250,]
                                                            #   # k2=KMatrix[251:500,]
                                                            #   # k3=KMatrix[501:750,]
                                                            #   # k4=KMatrix[751:1000,]
                                                            #   #ptm <- proc.time()
                                                            #   k1=KMatrix[1:100,]
                                                            #   k2=KMatrix[101:200,]
                                                            #   k3=KMatrix[201:300,]
                                                            #   k4=KMatrix[301:500,]
                                                            #   # k1=KMatrix[1:50,]
                                                            #   # k2=KMatrix[51:100,]
                                                            #   # k3=KMatrix[101:150,]
                                                            #   # k4=KMatrix[151:200,]
                                                            #   # k5=KMatrix[201:250,]
                                                            #   # k6=KMatrix[251:300,]
                                                            #   # k7=KMatrix[301:350,]
                                                            #   # k8=KMatrix[351:400,]
                                                            #   # k9=KMatrix[401:450,]
                                                            #   # k10=KMatrix[451:500,]
                                                            #   linearcombos=c()
                                                            #   matrices=list(k1,k2,k3,k4)
                                                            #   #matrices=list(k1,k2,k3,k4, k5, k6, k7, k8, k9, k10)
                                                            #   MATRIXGLOBAL<<-matrices
                                                            #   logit<<-LogisticRegressionResult
                                                            #   matrices<<-matrices
                                                            #   LogisticRegressionResultG<<-LogisticRegressionResult
                                                            #   linearcombos<<-linearcombos
                                                            #   #setup cluster
                                                            #   clust= makeCluster(no_cores)
                                                            #   clusterEvalQ(clust,library(quadprog))
                                                            #   #print("time taken parallel")
                                                            #   #print(proc.time() - ptm)
                                                            #   clusterExport(cl = clust, list("linearcombos", "LogisticRegressionResultG", "matrices",
                                                            #                                  "constructGrid", "glm_profiling", "minimcprofile",
                                                            #                                  "minimcprofileControl", "orglm.fit"))
                                                            # 
                                                            #   linearcombos=parLapply(clust, 1:4, function(i){
                                                            #     linearcombos=append(linearcombos,
                                                            #                         minimcprofile(object = LogisticRegressionResultG,
                                                            #                                        CM = matrices[[i]]))
                                                            #   })
                                                            #   stopCluster(clust)
                                                            #   finalLinCombo=linearcombos[[1]]
                                                            #   for(i in 2:4){
                                                            #     finalLinCombo[[2]]=rbind(finalLinCombo[[2]], linearcombos[[i]][[2]])
                                                            #     finalLinCombo$srdp=append(finalLinCombo$srdp, linearcombos[[i]]$srdp)
                                                            #     finalLinCombo$optpar=append(finalLinCombo$optpar, linearcombos[[i]]$optpar)
                                                            #     finalLinCombo$cobject=append(finalLinCombo$cobject, linearcombos[[i]]$cobject)
                                                            #   }                                                              
                                                            #   return(finalLinCombo)
                                                            # }
                                                        ))
