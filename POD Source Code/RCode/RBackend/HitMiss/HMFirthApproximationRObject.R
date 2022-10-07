#this class is used to calculate Firth logistic regression
#for hitmiss data
#library(logistf)
#library(roxygen2)
HMFirthApproximation <- setRefClass("HMFirthApproximation", fields = list(inputDataFrameFirth="data.frame", 
                                                                          modelFailed="logical", 
                                                                          separated="numeric",
                                                                          firthResults="glm"),
                             methods=list(
                               initialize=function(inputDataFrameFirthInput=data.frame(matrix(ncol = 1, nrow = 0))
                                                   , modelFailedInput=FALSE){
                                 inputDataFrameFirth<<-inputDataFrameFirthInput
                                 #firthResults<<-firthResultsInput
                                 modelFailed<<-modelFailedInput
                               },
                               #establish getters and setters for logit results and separation flag
                               setFirthResults=function(psFirthResults){
                                 firthResults <<- psFirthResults
                               },
                               getFirthResults=function(){
                                 return(firthResults)
                               },
                               setSeparatedFlag=function(sepFlag){
                                 separated<<-sepFlag
                               },
                               getSeparatedFlag=function(){
                                 return(separated)
                               },
                               calcFirthResults=function(){
                                 # FIRTH'S BIAS ADJUSTED #####
                                 firth.mod=try(logistf(y ~ x, data=inputDataFrameFirth, firth=TRUE,dataout=TRUE),FALSE)
                                 modelFailed<<-inherits(firth.mod,"try-error")
                                 #firth.modList=list()
                                 #firth.modList=append(firth.modList, firth.mod)
                                 #setFirthResults(firth.modList)
                                 if(modelFailed){
                                   print("firth model failed!")
                                 }
                                 else{
                                   firth.modglm=firth.mod
                                   currRVersion=R.Version()
                                   if(as.numeric(currRVersion$major)<=4){
                                     firth.modglm=convertTOGLMObject(firth.mod)
                                   }
                                   #firth.modglmGlobal<<-firth.modglm
                                   
                                   #TRY and fix this? much more efficent code (doesn't work with mcprofile because p>0 error)
                                   #unclass(firth.modglm)
                                   #class(firth.modglm)<-"glm"
                                   #firth.modglm$family$family="binomial"
                                   #firth.modglm$family$link="logit"
                                   ### ****** functions within glm not getting passed to firth properly which is 
                                   ### causing mcprofile to error out
                                   #tempglmObject = glm(formula = y ~ x, family = binomial, data = inputDataFrameFirth)
                                   #add family attribute
                                   #firth.modglm$family=tempglmObject$family
                                   ##
                                   #firth.modglm$qr=tempglmObject$qr
                                   #rank is used to calculate vcov
                                   #firth.modglm$rank=tempglmObject$rank
                                   #add the df.residual
                                   #firth.modglm$df.residual=tempglmObject$df.residual
                                   #firth.modGlobal<<-firth.modglm
                                   #print("DONE")
                                 }
                                 setFirthResults(firth.modglm)
                               },
                               convertTOGLMObject=function(firth.mod){
                                 # Predict values using Firth Model
                                 firth.probs=matrix(firth.mod$predict)
                                 AIC=-2*(firth.mod$loglik-2)[2]#extractAIC(firth.mod)[2],
                                 # NOTE: MAY NOT WORK WITH NEWER VERSIONS OF mcprofile! TO DO: Find work-around.
                                 # Firth was not supported by mcprofile (which is required to do LR and MLR) when this code was written, so a fake-out dataframe was created. 
                                 firth.glm.mod = glm(formula = y ~ x, family = binomial, data = inputDataFrameFirth)
                                 # Terms replaced with firth values 
                                 firth.glm.mod$coefficients	=	firth.mod$coefficients
                                 firth.glm.mod$residuals	=	(firth.mod$y-firth.mod$predict)/((firth.mod$predict*(1-firth.mod$predict)))
                                 firth.glm.mod$fitted.values	=	firth.mod$predict
                                 firth.glm.mod$linear.predictors	=	firth.mod$linear.predictors
                                 firth.glm.mod$deviance	=	-2*firth.mod$loglik[2]
                                 firth.glm.mod$aic	=	-2*(firth.mod$loglik-2)[2]
                                 firth.glm.mod$null.deviance	=	-2*firth.mod$loglik[1]
                                 firth.glm.mod$iter	=	firth.mod$iter
                                 firth.glm.mod$prior.weights	=	firth.mod$weights
                                 firth.glm.mod$y	=	firth.mod$data$y
                                 firth.glm.mod$converged	=	(modelFailed==FALSE)
                                 firth.glm.mod$model$x	=	firth.mod$data$x
                                 firth.glm.mod$model$y = firth.mod$data$y
                                 firth.glm.mod$formula	=	firth.mod$formula
                                 firth.glm.mod$data	=	firth.mod$data 
                                 # Fake-out terms
                                 firth.glm.mod$effects	=	 firth.glm.mod$effects[1:length(firth.glm.mod$effects)]=0
                                 firth.glm.mod$R 	=	diag(2)
                                 firth.glm.mod$weights	=	rep(0,nrow(inputDataFrameFirth))
                                 # All other values in firth.glm.mod are kept as-is.
                                 return(firth.glm.mod)
                               }
                             ))

