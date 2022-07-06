library("RSSampling")
library(MASS)
library(logistf)
library(pracma)
initializeReturnDataFrame<-function(){
  resultsInitialize=data.frame(Intercept =0,    Slope     =0,   
                               a90       =0,    a9095     =0,    
                               AIC       =0,    Iter      =0,    Converged =0,    Separated =0,
                               Uneven =0, Overlap=0, Insp=0,  N=0, 
                               Model = "", a50=0, a25=0, a9095_LR=0, a9095_MLR =0, NumNA=0,
                               a_x_n   = 500,
                               profile_pts = profile_pts)
  results=resultsInitialize
}


getLogitResults<-function(hitMissDF, standardWald=TRUE, modifiedWald=FALSE, 
                        lr_CI=FALSE, mlr_CI=FALSE){
  
  #separated dataset (if the model fails, it will become 1 and warn the user)
  #LReg_test$converged is also boolean
  separated<-0
  options(warn=2)
  
  #tries to fit a genralized linear model to the dataframe
  LReg_test.mod<-try(glm(formula = y ~ x, data=hitMissDF, family=binomial), FALSE)
  failed<-inherits(LReg_test.mod, "try-error")
  if(failed || !LReg_test.mod$converged){
    #distributes a warning about wacky data (WORK ON THIS LATER)
    separated<-0
  }
  
  #Build whatever model is possible
  options(warn=0)
  LReg.mod <- glm(formula= y ~ x, data=hitMissDF, family=binomial)
  
  #Fit a normal distribution to the flaw sizes so that you get a
  #smooth curve (500 points?)
  normalFit_x <- fitdistr(hitMissDF$x, "normal")
  #TEMPS
  a_x_n=500
  profile_pts=500
  
  ### This is where the mean and standard deviation are calculated
  a=rnorm(mean= normalFit_x$estimate[1], sd = normalFit_x$estimate[2], n=max(a_x_n, profile_pts))
  for (idx in 1:length(a)){
    while(a[idx] <= 0){
      a[idx] = rnorm(mean=normalFit_x$estimate[1], sd=normalFit_x$estimate[2], n=1)
    }
    
  }
  
  # Estimate the one-sided Wald 95% confidence interval
  # Calculate the 95% lower Wald confidence interval (in linear space, 
  # which will appear as an upper interval in probability space)
  LReg.lwr <- LReg.predictions$fit - (qnorm(0.95) * LReg.predictions$se.fit)
  # Log odds value at 90% POD
  LOd=log(0.9/(1-0.9))
  # Calculate a90: flaw size at 90% probability
  LReg.mod$a90 = unname((LOd-LReg.mod$coefficients[1])/LReg.mod$coefficients[2])
  LReg.mod$a25 = unname((log(0.25/(1-0.25))-LReg.mod$coefficients[1])/LReg.mod$coefficients[2])
  LReg.mod$a50 = unname((-LReg.mod$coefficients[1])/LReg.mod$coefficients[2]) # muhat
  
  # Predict the fitted values and the standard errors for the model 
  LReg.predictions=predict(LReg.mod, type="link",se.fit=TRUE)
  
  # TO DO: sigmahat = PREDICT THE STANDARD ERROR AT a90.
  muhat    = LReg.mod$a50
  sigmahat = predict(LReg.mod, newdata=data.frame(x=LReg.mod$a90), type="response",se.fit=TRUE)$se.fit
  
  # predict points along the POD curve
  LReg.fit<-LReg.predictions$fit
  
  #approx a9095
  #a9095: the flaw size at 90% probabilityalong the wald 95% CI curve
  #LReg.mod$a90_95=approx(LReg.lwr, hitMissDF$x,LOd)$y
  #transform the confidence interval into  probability space
  a_x = a[linspace(1,length(a),hitMissDF$a_x_n[1])]
  # Predict results for these a_x flaw sizes.
  ptm <- proc.time()
  linear_pred=predict(LReg.mod, type="link", se.fit=TRUE, newdata=data.frame(x=a_x))
  #varcovmat =vcov(LReg.mod)
  #se_i = sqrt(varcovmat[1,1]+2*varcovmat[1,2]*a_x+varcovmat[2,2]*a_x^2)
  #probs = LReg.mod$family$linkinv(linear_pred$fit)
  probsAt95CI = LReg.mod$family$linkinv(linear_pred$fit-qnorm(0.95)*se_i)
  LogisticRegressionDataFrame <<-LReg.mod 
  print(LogisticRegressionDataFrame)
}
ggplot(data=LReg.mod, mapping=aes(x=LReg.mod$data$x, y=LReg.mod$probsa9095))+geom_point()



hitMissDF <- read.csv("C:/Users/gohmancm/Desktop/PODv4Point5FullProjectFolder/RCode/RBackend/HitMissData_Good_1TestSet.csv")
getLogitResults(hitMissDF)


ggplot(data=LReg.mod, mapping=aes(x=LReg.mod$data$x, y=LReg.mod$fitted.values))+geom_point()