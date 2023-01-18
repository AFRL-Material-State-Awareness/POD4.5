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

#this class is used for logit approximation when dealing with hitMiss Data

# parameters:
# inputDataFrameLogistic = the input dataframe to be used with logistic regression (i.e. the original hit/miss dataframe)
# modelFailed = flag used to inform the user if the model failed
# separated = flag used to inform the user if the data is separated
# logitResults = the results of the maximum likelihood logistic regression as a glm object

HMLogitApproximation <- setRefClass("HMLogitApproximation", fields = list(inputDataFrameLogistic="data.frame",
                                                                          separated="numeric",
                                                                          convergedFail="numeric",
                                                                          logitResults="glm"), methods = list(
                                                                        #establish getters and setters for logit results and separation flag
                                                                        setLogitResults=function(psLogitResults){
                                                                          logitResults <<- psLogitResults
                                                                        },
                                                                        getLogitResults=function(){
                                                                          return(logitResults)
                                                                        },
                                                                        getSeparatedFlag=function(){
                                                                          return(separated)
                                                                        },
                                                                        getConvergedFailFlag=function(){
                                                                          return(convergedFail)
                                                                        },
                                                                        #function used to get the results for logit and assign the dataframe
                                                                        calcLogitResults=function(){
                                                                          #separated dataset (if the model fails, it will become 1 and warn the user)
                                                                          #LReg_test$converged is also boolean
                                                                          separated<<-0
                                                                          convergedFail<<-0
                                                                          #options(warn=2)
                                                                          #use this to run without warnings
                                                                          options(warn=-1)
                                                                          #transform the column names to x,y
                                                                          y<-colnames(inputDataFrameLogistic[3])
                                                                          x<-colnames(inputDataFrameLogistic[2])
                                                                          #tries to fit a genralized linear model to the dataframe
                                                                          LReg_test.mod<-try(glm(formula = y ~ x, data=inputDataFrameLogistic, 
                                                                                                 family=binomial), FALSE)
                                                                          failed<-inherits(LReg_test.mod, "try-error")
                                                                          
                                                                          if(failed || !LReg_test.mod$converged){
                                                                            #distributes a warning about wacky data
                                                                            separated<<-1
                                                                          }
                                                                          if(!LReg_test.mod$converged){
                                                                            #used to let the user know the algorithm did not converge
                                                                            convergedFail<<-1
                                                                          }
                                                                          #Build whatever model is possible
                                                                          options(warn=0)
                                                                          LRegModel <- glm(formula= y ~ x, data=inputDataFrameLogistic, family=binomial)
                                                                          #set the results for the class instance
                                                                          setLogitResults(LRegModel)
                                                                        }
                                                                      ))
