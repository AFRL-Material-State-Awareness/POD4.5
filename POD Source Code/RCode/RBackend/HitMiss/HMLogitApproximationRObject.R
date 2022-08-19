#this class is used for logit approximation when dealing with hitMiss Data
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
                                                                          #LRegModelGlobal<<-LRegModel
                                                                          #set the results for the class instance
                                                                          setLogitResults(LRegModel)
                                                                        }
                                                                      ))
