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
#     along with this program.  If not, see <https://www.gnu.org/licenses/>.

# this class is used to calculate the Likelihood ratio confidence interval and
# the MODIFIED Likelihood ratio confidence interval of the POD curve (WARNING: VERY SLOW)

# parameters:
# ~Logistic Regression result= the result of the logistic regression (either firth or maximum likelihood) as an S3 object
# ~inputDataFrameLogistic= the input dataframe with the flaw sizes. This can either be the original hitmiss dataframe if standard
# wald is used or simulated cracks (normally distributed) if modified wald, LR, or MLR is used
# ~a_Values= The list of critical a values along with the value of sigma in this order...
# a25 
# a50
# a90
# sigma
# a9095
GenAValuesOnPODCurve <- setRefClass("GenAValuesOnPODCurve", 
                                              fields = list(LogisticRegressionResult="glm",
                                                            inputDataFrameLogistic="data.frame",
                                                            a_Values="list"), methods = list(
                                              setAValuesList=function(psAValues){
                                                a_Values<<-psAValues
                                              },
                                              getAValuesList=function(){
                                                return(a_Values)
                                              },
                                              #this class will pass back the confidence interval values as a dataframe
                                              calcAValuesStandardWald=function(){
                                                #set up an empty list to store the key 'a' values
                                                aValuesSW=list()
                                                # Calculate the 95% lower Wald confidence interval(again)
                                                #might remoove this to make it slightly more efficent
                                                linear_pred=predict(LogisticRegressionResult, type="link", se.fit=TRUE)
                                                LReg.lwr <- linear_pred$fit - (qnorm(0.95) * linear_pred$se.fit)
                                                # Log odds value at 90% POD
                                                LOd=log(0.9/(1-0.9))
                                                # Calculate a25,a50, and a90(flaw size at 90% probability)
                                                a25 = unname((log(0.25/(1-0.25))-LogisticRegressionResult$coefficients[1])/
                                                               LogisticRegressionResult$coefficients[2])
                                                aValuesSW=append(aValuesSW, a25)
                                                a50 = unname((-LogisticRegressionResult$coefficients[1])/
                                                               LogisticRegressionResult$coefficients[2]) # muhat
                                                aValuesSW=append(aValuesSW, a50)
                                                a90 = unname((LOd-LogisticRegressionResult$coefficients[1])/
                                                               LogisticRegressionResult$coefficients[2])
                                                aValuesSW=append(aValuesSW, a90)
                                                #standard error at a 90 which is passed back to the UI
                                                sigmahat = predict(LogisticRegressionResult, newdata=data.frame(x=a90), type="response",se.fit=TRUE)$se.fit
                                                aValuesSW=append(aValuesSW, sigmahat)
                                                a90_95=approx(LReg.lwr,inputDataFrameLogistic$x,LOd)$y
                                                aValuesSW=append(aValuesSW, a90_95)
                                                #set array to be returned to main analysis objec class
                                                setAValuesList(aValuesSW)
                                              },
                                              calcAValuesModWald=function(){
                                                aValuesMW=calcPODAValuesStandard()
                                                simCracks=inputDataFrameLogistic$x
                                                probsAt95CI=inputDataFrameLogistic$Confidence_Interval
                                                a90_95 = try(approx(probsAt95CI,simCracks,0.9)$y,TRUE)
                                                if(!is.numeric(a90_95)){
                                                  a90_95=NA
                                                }
                                                aValuesMW=append(aValuesMW, a90_95)
                                                
                                                #set array to be returned to main analysis objec class
                                                setAValuesList(aValuesMW)
                                              },
                                              calca9095LR=function(){
                                                aValuesLR=calcPODAValuesStandard()
                                                #print(inputDataFrameLogistic)
                                                simCracks=inputDataFrameLogistic$x
                                                #print(simCracks)
                                                probsAt95CI=inputDataFrameLogistic$Confidence_Interval
                                                #print(probsAt95CI)
                                                a9095 = try(approx(probsAt95CI,simCracks,0.9)$y,TRUE)
                                                if(!is.numeric(a9095)){
                                                  a9095=NA
                                                }
                                                aValuesLR=append(aValuesLR, a9095)
                                                setAValuesList(aValuesLR)
                                                
                                              },
                                              calca9095MLR=function(){
                                                aValuesMLR=calcPODAValuesStandard()
                                                simCracks=inputDataFrameLogistic$x
                                                probsAt95CI=inputDataFrameLogistic$Confidence_Interval
                                                #print(probsAt95CI)
                                                a9095 = try(approx(probsAt95CI,simCracks,0.9)$y,TRUE)
                                                if(!is.numeric(a9095)){
                                                  a9095=NA
                                                }
                                                aValuesMLR=append(aValuesMLR, a9095)
                                                setAValuesList(aValuesMLR)
                                              },
                                              calcPODAValuesStandard=function(){
                                                aValuesS=list()
                                                simCracks=inputDataFrameLogistic$x
                                                probsAt95CI=inputDataFrameLogistic$Confidence_Interval
                                                linear_pred=predict(LogisticRegressionResult, type="link", se.fit=TRUE, newdata=data.frame(x=simCracks))
                                                a25 = approx(LogisticRegressionResult$family$linkinv(linear_pred$fit),simCracks,0.25)$y #a_i[probs==0.25]
                                                aValuesS=append(aValuesS, a25)
                                                a50 = approx(LogisticRegressionResult$family$linkinv(linear_pred$fit),simCracks,0.5)$y #a_i[probs==0.5]
                                                aValuesS=append(aValuesS, a50)
                                                a90 = approx(LogisticRegressionResult$family$linkinv(linear_pred$fit),simCracks,0.9)$y #a_i[probs==0.9]
                                                aValuesS=append(aValuesS, a90)
                                                sigmahat = predict(LogisticRegressionResult, newdata=data.frame(x=a90), type= "response",se.fit=TRUE)$se.fit
                                                aValuesS=append(aValuesS, sigmahat)
                                                return(aValuesS)
                                              }
                                   ))