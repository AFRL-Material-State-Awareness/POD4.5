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

# this class is used to calculated the confidence interval using standard or modified wald(depending on the input)

# parameters:
# LogisticRegressionResult = the result of the logistic regression as a glm object
# simCrackVals = the array holding the normally distributed crack values (only used in modified wald)
# CIDataFrame = the dataframe containing the confidence interval and the POD values
# covarMatrix = the variance-covariance matrix values

WaldConfInt <- setRefClass("WaldConfInt", fields = list(LogisticRegressionResult="glm", simCrackVals="numeric", 
                                                        CIDataFrame="data.frame", covarMatrix="matrix"),  methods = list(
                                                                          #this class will pass back the confidence interval values as a dataframe
                                                                          setCIDataFrame=function(psCIDataFrame){
                                                                            CIDataFrame<<-psCIDataFrame
                                                                          },
                                                                          getCIDataFrame=function(){
                                                                            return(CIDataFrame)
                                                                          },
                                                                          setCovarMatrix=function(psMatrix){
                                                                            covarMatrix<<-psMatrix
                                                                          },
                                                                          getCovarMatrix=function(){
                                                                            return(covarMatrix)
                                                                          },
                                                                          #determines standard wald confidence interval (no simulated points were generated)
                                                                          executeStandardWald=function(){
                                                                            # Predict the fitted values and the standard errors for the model 
                                                                            # Estimate the one-sided Wald 95% confidence interval
                                                                            # Calculate the 95% lower Wald confidence interval
                                                                            linear_pred=predict(LogisticRegressionResult, type="link", se.fit=TRUE)
                                                                            #generate the variance-covariance matrix
                                                                            varcovmat =vcov(LogisticRegressionResult)
                                                                            #store the covariance matrix, so it can be returned to the UI
                                                                            setCovarMatrix(varcovmat)
                                                                            sigmaOfCrack_i = sqrt(varcovmat[1,1]+2*varcovmat[1,2]*
                                                                                                    LogisticRegressionResult$data$x+varcovmat[2,2]*
                                                                                                    LogisticRegressionResult$data$x^2)
                                                                            probs = LogisticRegressionResult$family$linkinv(linear_pred$fit)
                                                                            probsAt95CI = LogisticRegressionResult$family$
                                                                              linkinv(linear_pred$fit-qnorm(0.95)*sigmaOfCrack_i)
                                                                            Confidence_Interval<-probsAt95CI
                                                                            #globalSimga<<-sigmaOfCrack_i
                                                                            #varcovmatG<<-varcovmat
                                                                            setCIDataFrame(data.frame(Confidence_Interval))
                                                                          },
                                                                          #determines the modified wald CI
                                                                          executeModifiedWald=function(){
                                                                            # Predict the fitted values and the standard errors for the model
                                                                            linear_pred=predict(LogisticRegressionResult, type="link", se.fit=TRUE, 
                                                                                                newdata=data.frame(x=simCrackVals))
                                                                            varcovmat =vcov(LogisticRegressionResult)
                                                                            #store the covariance matrix, so it can be returned to the UI
                                                                            setCovarMatrix(varcovmat)
                                                                            sigmaOfCrack_i = sqrt(varcovmat[1,1]+2*varcovmat[1,2]*
                                                                                                    simCrackVals+varcovmat[2,2]*
                                                                                                    simCrackVals^2)
                                                                            t_trans = LogisticRegressionResult$family$linkinv(linear_pred$fit)
                                                                            probsAt95CI = LogisticRegressionResult$family$
                                                                              linkinv(linear_pred$fit-qnorm(0.95)*sigmaOfCrack_i)
                                                                            Confidence_Interval<-probsAt95CI
                                                                            setCIDataFrame(cbind(t_trans,data.frame(Confidence_Interval)))
                                                                          }
                                                                        ))
