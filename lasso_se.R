# This function was adapted from:
# https://www.reddit.com/r/statistics/comments/1vg8k0/standard_errors_in_glmnet/
# The lasso from glmnet does not return the standard errors because they are 
# very biased. However, they are required for confidence intervals, 
# so this method provides the best estimate of the standard error

lasso_se <- function(xs,y,yhat,my_mod){
  # Note, you can't estimate an intercept here
  n <- dim(xs)[1]
  k <- dim(xs)[2]
  sigma_sq <- sum((y-yhat)^2)/ (n-k)
  lam <- my_mod$lambda.min
  if(is.null(my_mod$lambda.min)==TRUE){lam <- 0}
  i_lams <- Matrix(diag(x=1,nrow=k,ncol=k),sparse=TRUE)
  xpx <- t(xs)%*%xs
  xpxinvplam <- solve(xpx+lam*i_lams)
  var_cov <- sigma_sq * (xpxinvplam %*% xpx %*% xpxinvplam)
  se_bs <- sqrt(diag(var_cov))
  #print('NOTE: These standard errors are very biased.')
  return(se_bs)
}