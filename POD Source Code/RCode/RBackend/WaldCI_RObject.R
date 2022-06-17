#this class is used to calculated the confidence interval using standard or modified wald(depending on the input)
WaldConfInt <- setRefClass("WaldConfInt", fields = list(LogisticRegressionResult="glm", simCrackVals="numeric", CIDataFrame="data.frame"),  methods = list(
                                                                          #this class will pass back the confidence interval values as a dataframe
                                                                          setCIDataFrame=function(psCIDataFrame){
                                                                            CIDataFrame<<-psCIDataFrame
                                                                          },
                                                                          getCIDataFrame=function(){
                                                                            return(CIDataFrame)
                                                                          },
                                                                          #determines standard wald confidence interval (no simulated points were generated)
                                                                          executeStandardWald=function(){
                                                                            # Predict the fitted values and the standard errors for the model 
                                                                            # Estimate the one-sided Wald 95% confidence interval
                                                                            # Calculate the 95% lower Wald confidence interval
                                                                            linear_pred=predict(LogisticRegressionResult, type="link", se.fit=TRUE)
                                                                            #generate the variance-covariance matrix
                                                                            varcovmat =vcov(LogisticRegressionResult)
                                                                            sigmaOfCrack_i = sqrt(varcovmat[1,1]+2*varcovmat[1,2]*
                                                                                                    LogisticRegressionResult$data$x+varcovmat[2,2]*
                                                                                                    LogisticRegressionResult$data$x^2)
                                                                            probs = LogisticRegressionResult$family$linkinv(linear_pred$fit)
                                                                            probsAt95CI = LogisticRegressionResult$family$linkinv(linear_pred$fit-qnorm(0.95)*sigmaOfCrack_i)
                                                                            #print(length(probsAt95CI))
                                                                            Confidence_Interval<-probsAt95CI
                                                                            #a90_95 = try(approx(probsAt95CI,a_x,0.9)$y,TRUE)
                                                                            #print(a90_95)
                                                                            setCIDataFrame(data.frame(Confidence_Interval))
                                                                          },
                                                                          #determines the modified wald CI
                                                                          executeModifiedWald=function(){
                                                                            # Predict the fitted values and the standard errors for the model
                                                                            linear_pred=predict(LogisticRegressionResult, type="link", se.fit=TRUE, 
                                                                                                newdata=data.frame(x=simCrackVals))
                                                                            varcovmat =vcov(LogisticRegressionResult)
                                                                            sigmaOfCrack_i = sqrt(varcovmat[1,1]+2*varcovmat[1,2]*
                                                                                                    simCrackVals+varcovmat[2,2]*
                                                                                                    simCrackVals^2)
                                                                            t_trans = LogisticRegressionResult$family$linkinv(linear_pred$fit)
                                                                            probsAt95CI = LogisticRegressionResult$family$linkinv(linear_pred$fit-qnorm(0.95)*sigmaOfCrack_i)
                                                                            #View(cbind(simCrackVals, probs,probsAt95CI))
                                                                            Confidence_Interval<-probsAt95CI
                                                                            setCIDataFrame(cbind(t_trans,data.frame(Confidence_Interval)))
                                                                          }
                                                                        ))
