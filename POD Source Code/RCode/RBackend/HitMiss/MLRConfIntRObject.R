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
#     along with this program.  If not, see <https://www.gnu.org/licenses>

#this class is used to calculate the MODIFIED Likelihood ratio confidence interval of the POD curve (WARNING: VERY SLOW)
# very similar to the LRConfintRObject except higher order approximation (hoa) is applied to deal with smoothness issues
#TODO: consider reimplmenting this in c++ in order to speed up to process

# parameters:
# LogisticRegressionResult = the result of the logistic regression as a glm object
# CIDataFrame = the dataframe used to store the MLR confidence interval and POD values
# calcLinearCombo = the linear combination matrix cast as a regular list (rather than an S3 mcprofile object).
# This was done because the class would not accept the mcprofile object

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
                                          #print(castCalcLinCombo)
                                          #cat('\n')
                                          ci.logit.profile.mlr = try(confint.mcprofile(hoa(castCalcLinCombo), level = 0.95, adjust = "none",
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