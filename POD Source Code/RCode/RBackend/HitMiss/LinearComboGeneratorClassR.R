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
#     along with this program.  If not, see <https://www.gnu.org/licenses/>

# creates the linear combination matrix for finding the LR or MLR confidence intervals
# CPU parallelization is used to slightly increase the speed of the calculations

# parameters:
# LogisticRegressionResult = the result of the maximum likelihood or the firth logistic regression
# simCracks = the array of simulated cracks (usually 500 of them)
# samples = the original sample size

LinearComboGenerator <- setRefClass("LinearComboGenerator", fields = list(LogisticRegressionResult="glm", 
                                                                          simCracks="numeric", samples="numeric"),  methods = list(
                                                            ####This function generates the linear combinations for either LR or MLR
                                                            genLinearCombinations=function(){
                                                              print("Starting linear Combos")
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
                                                              #linearCombo<-mcprofile(object = LogisticRegressionResult, CM = K)# Calculate -2log(Lambda)
                                                              #print("mcprofile time calc parallel")
                                                              p=proc.time() - ptm
                                                              print(p)
                                                              return(linearCombo)
                                                            },
                                                            getSimCracks=function(){
                                                              return(simCracks)
                                                            }
                                                        ))
