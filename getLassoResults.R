# LASSO: LOGISIC REGRESSION WITH REGULARIZATION #####

# Fits a lasso regression to the data in df and
# returns the results of fitting the model as "output_rows" dataframe
# If a second variable is given in the function call (usually "folderLocation"),
# then the function will output an additional file containing POD curve values. 
# If the second variable is empty, the points file will not save.

getLassoResults<-function(df, writeResults=FALSE){

  #print(paste("Lasso Modeling for Inspector", df$Insp[1]))
  
  # Calculate the lasso for multiple lambda values. 
  # Do k-folds validation to find the best lambda
  lasso.mod=glmnet(cbind(1,df$x),df$y,alpha=1,family="binomial",
                   lambda=seq(0,3,length=11))
  cv.out<-try(cv.glmnet(cbind(1,df$x),df$y,alpha=1,family="binomial"), FALSE)
  failed<-inherits(cv.out,"try-error")
  if(failed)
  { #print("Separated Dataset.")
    bestlambdalasso=0
    converged=0
  }else{ 
    bestlambdalasso=cv.out$lambda.min
    converged=1}
  
  # Fit the model using the best lambda
  lasso.mod=glmnet(cbind(1,df$x),df$y,alpha=1,family="binomial",
                   lambda=bestlambdalasso)
  
  # Predict the probability values for the POD curve
  lasso.mod$probs=predict(lasso.mod, s=bestlambdalasso, type="response",
                          newx=cbind(1,df$x))
  # Predict the linear values
  lasso.mod$predictions=predict(lasso.mod,type="link",se.fit=TRUE,newx=cbind(1,df$x))
  
  # Save the coefficients from the fitted model
  lasso.mod$coef=predict(lasso.mod, type="coefficients", s=bestlambdalasso)
  
  # Log odds at 90% probability
  LOd=log(0.9/(1-0.9))
  
  # Calculate a90: flaw size at 90% probability
  lasso.mod$a90 = unname((LOd-lasso.mod$coef[1])/lasso.mod$coef[3])
  lasso.mod$a25 = unname((log(0.25/(1-0.25))-lasso.mod$coef[1])/lasso.mod$coef[3])
  lasso.mod$a50 = unname((-lasso.mod$coef[1])/lasso.mod$coef[3])
  
  # To get confidence intervals on the parameters, I need a function
  yhat=predict(lasso.mod,cbind(1,df$x),s=lasso.mod$lambda)
  lasso.mod$se=lasso_se(cbind(1,df$x),df$y,yhat,lasso.mod)
  # To get Confidence interval on a9095, need standard error, which glmnet does
  # not provide since it is biased. However, SE can be calculated at each point 
  # using vcv matrix
    SE_lasso<-lasso.mod$se
    # Same X as before but lasso is in the wrong format
    options(warn=0)
    LReg.mod<-glm(formula = y ~ x, data=df, family = binomial)
    X<-model.matrix(LReg.mod)
    p_lasso <- lasso.mod$probs[1:df$N[1]]
    W_lasso <- diag(p_lasso*(1-p_lasso))
    # Variance covariance matrix for the parameters
    V_lasso <- solve(t(X)%*%W_lasso%*%X)
    SE_eachPt_lasso=sqrt(V_lasso[1]+df$x^2*V_lasso[4]+2*df$x*V_lasso[2])
  
  # Estimate the one-sided Wald 95% confidence interval using the SE estimate
  lasso.lwr <- lasso.mod$predictions - (qnorm(0.95) * SE_eachPt_lasso)

  # Predict points along the POD curve
  lasso.fit <- lasso.mod$predictions
  
  # Approximate a9095 using linear interpolation
  # a9095: the flaw size at 90% probability along the Wald 95% CI curve
    lasso.mod$a90_95=approx(exp(lasso.lwr)/(1+exp(lasso.lwr)),df$x,0.9)$y
  
  # If the points file is desired, writeResults location given.
  if(writeResults!=FALSE){
    results_all_pts=data.frame(
      x           = df$x,
      y_hat       = lasso.fit,
      se          = SE_eachPt_lasso,
      probs       = exp(lasso.fit)/(1+exp(lasso.fit)),
      probsAt95CI = exp(lasso.lwr)/(1+exp(lasso.lwr)),
      model       = "Lasso",
      insp        = df$Insp[1],
      n           = df$N[1]
    )
    fileName=paste(writeResults,"StdWald_results_",df$Insp[1],"_Points.csv",sep="")
    write.table(results_all_pts, file = fileName, row.names = FALSE, 
                append = TRUE, col.names = FALSE, sep = ", ")
  }
  
  # Save the model results to the output variable
  outputs_row=data.frame(
    Intercept=lasso.mod$coef[1],
    Slope=lasso.mod$coef[3],
    a25=lasso.mod$a25,
    a50=lasso.mod$a50,
    a90=lasso.mod$a90,
    a9095=lasso.mod$a90_95,
    AIC=(1-lasso.mod$dev.ratio)*lasso.mod$nulldev,
    Iter=0,
    Converged=converged,
    Separated=NA,
    Uneven =df$Uneven[1],
    Overlap=df$Overlap[1],
    Insp=df$Insp[1],
    N=df$N[1],
    Model="Lasso",
    CI = "Std. Wald"
  )
  
  # Return the model results
   df<- outputs_row
}