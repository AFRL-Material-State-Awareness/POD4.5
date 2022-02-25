# FIRTH: LOGISIC REGRESSION WITH FIRTH's BIAS ADJUSTMENT #####

# Fits a firth's bias adjusted logistic regression to the data in df and
# returns the results of fitting the model as "output_rows" dataframe
# If a second variable is given in the function call (usually "folderLocation"),
# then the function will output an additional file containing POD curve values. 
# If the second variable is empty, the points file will not save.

getFirthResults<-function(df, writeResults=FALSE){  

  #print(paste("Firth Modeling for Inspector", df$Insp[1]))
  
    # Fit FIRTH'S BIAS ADJUSTED model
  firth.mod=logistf(y ~x, data=df, firth=TRUE,dataout=TRUE)
  
  # Predict the probability values
  firth.probs=matrix(firth.mod$predict)
  
  # Calculate the standard errors at each flaw (crack) size
  for(n in 1:length(df$x)){
    crack=df$x[n]
    firth.mod$se[n]=sqrt(t(c(1,crack)%*%firth.mod$var%*%c(1,crack)))
  }
  
  # Estimate the one-sided Wald 95% confidence interval
  firth.lwr <- firth.mod$linear.predictors - (qnorm(0.95) * firth.mod$se)
  
  # Predict points along the POD curve
  firth.fit <- firth.mod$linear.predictors
  
  # Log odds at 90% Probability
  LOd=log(0.9/(1-0.9))
  
  # Calculate a90: flaw size at 90% probability
  firth.mod$a90 = unname((LOd-firth.mod$coefficients[1])/firth.mod$coefficients[2])
  firth.mod$a25 = unname((log(0.25/(1-0.25))-firth.mod$coefficients[1])/firth.mod$coefficients[2])
  firth.mod$a50 = unname((-firth.mod$coefficients[1])/firth.mod$coefficients[2])
  
  # Approximate a9095 using linear interpolation
  # a9095: the flaw size at 90% probability along the Wald 95% CI curve
  firth.mod$a90_95=approx(firth.lwr,df$x,LOd)$y

  # If the points file is desired, writeResults location given.  
  if(writeResults!=FALSE){
    results_all_pts=data.frame(
      x           = df$x,
      y_hat       = firth.fit,
      se          = firth.mod$se,
      probs       = exp(firth.fit)/(1+exp(firth.fit)),
      probsAt95CI = exp(firth.lwr)/(1+exp(firth.lwr)),
      model       = "Firth",
      insp        = df$Insp[1],
      n           = df$N[1]
    )
    fileName=paste(writeResults,"StdWald_results_Insp_",df$Insp[1],"_Points.csv",sep="")
    write.table(results_all_pts, file = fileName, row.names = FALSE, 
                append = TRUE, col.names = FALSE, sep = ", ")
  }
  
  # Create output variable with the model results
  outputs_row=data.frame(
    Intercept=unname(firth.mod$coefficients[1]),
    Slope=unname(firth.mod$coefficients[2]),
    a25=firth.mod$a25,
    a50=firth.mod$a50,
    a90=firth.mod$a90,
    a9095=firth.mod$a90_95,
    AIC=extractAIC(firth.mod)[2],
    Iter=firth.mod$iter,
    Converged=NA,
    Separated=NA,
    Uneven =df$Uneven[1],
    Overlap=df$Overlap[1],
    Insp=df$Insp[1],
    N=df$N[1],
    Model="Firth",
    CI = "Std. Wald"
  )
  
  # Return the model results
  df<- outputs_row
}