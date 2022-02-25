# LOGIT: LOGISIC REGRESSION #####

# Fits a logistic regression to the data in df and
# returns the results of fitting the model as "output_rows" dataframe
# If a second variable is given in the function call (usually "folderLocation"),
# then the function will output an additional file containing POD curve values. 
# If the second variable is empty, the points file will not save.

getLogitResults<- function(df, writeResults=FALSE){
  
  #print(paste("Logit Modeling for Inspector", df$Insp[1]))
  
  # Initiate the model to have no separation
  separated=0
  # Allow R to flag any warnings in the modeling
  options(warn=2)
  # Attempt to fit the logistic regression. 
  # If there are warnings or errors, change the separation flag to 1. 
  LReg_test.mod <- try(glm(formula = y ~ x, data=df, family = binomial), FALSE)
  failed <- inherits(LReg_test.mod,"try-error")
  if(failed || !LReg_test.mod$converged)
  { #print("Separated Dataset.")
    separated=1
  }#else{ print("Converges")}
  
  # Now, ignoring errors, build whatever logistic regression model is possible 
  options(warn=0)
  LReg.mod <- glm(formula = y ~ x, data=df, family = binomial)
  
  # Log odds value at 90% POD
  LOd=log(0.9/(1-0.9))
  
  # Calculate a90: flaw size at 90% probability
  LReg.mod$a90 = unname((LOd-LReg.mod$coefficients[1])/LReg.mod$coefficients[2])
  LReg.mod$a25 = unname((log(0.25/(1-0.25))-LReg.mod$coefficients[1])/LReg.mod$coefficients[2])
  LReg.mod$a50 = unname((-LReg.mod$coefficients[1])/LReg.mod$coefficients[2])
  
  # Predict the fitted values and the standard errors for the model 
  LReg.predictions=predict(LReg.mod, type="link",se.fit=TRUE)

  # Estimate the one-sided Wald 95% confidence interval
    # Calculate the 95% lower Wald confidence interval (in linear space, 
    # which will appear as an upper interval in probability space)
  LReg.lwr <- LReg.predictions$fit - (qnorm(0.95) * LReg.predictions$se.fit)
  
  # Predict points along the POD curve
  LReg.fit <- LReg.predictions$fit
  
  # Approximate a9095 using linear interpolation
  # a9095: the flaw size at 90% probability along the Wald 95% CI curve
  LReg.mod$a90_95=approx(LReg.lwr,df$x,LOd)$y
  
  # If the points file is desired, writeResults location given.
  if(writeResults!=FALSE){
    results_all_pts=data.frame(
      x           = LReg.mod$model$x,
      y_hat       = LReg.fit,
      se          = LReg.predictions$se.fit,
      probs       = exp(LReg.fit)/(1+exp(LReg.fit)),
      probsAt95CI = exp(LReg.lwr)/(1+exp(LReg.lwr)),
      model       = "Logit",
      insp        = df$Insp[1],
      n           = df$N[1]
    )
    fileName=paste(writeResults,"StdWald_results_Insp_",df$Insp[1],"_Points.csv",sep="")
    write.table(results_all_pts, file = fileName, row.names = FALSE, 
                append = TRUE, col.names = FALSE, sep = ", ")
  }
  
  # Set the fitted model results variable 
  outputs_row=data.frame(
    Intercept=unname(LReg.mod$coefficients[1]),
    Slope=unname(LReg.mod$coefficients[2]),
    a25=LReg.mod$a25,
    a50=LReg.mod$a50,
    a90=LReg.mod$a90,
    a9095=LReg.mod$a90_95,
    AIC=LReg.mod$aic,
    Iter=LReg.mod$iter,
    Converged=LReg.mod$converged,
    Separated=separated,
    Uneven =df$Uneven[1],
    Overlap=df$Overlap[1],
    Insp=df$Insp[1],
    N=df$N[1],
    Model="Logit",
    CI = "Std. Wald"
    )
  # Return the fitted model results variable 
  outputs_row
}


