#' Inverse logit transformation of Confidence Intervals
#' 
#' Inverse logit transformation of confidence interval estimates in mcpCI objects.
#' 
#' @param x An object of class mcpCI
#' 
#' @return An object of class mcpCI with transformed estimates.
#' 
#' @family confidence interval transformations
#' @seealso \code{\link{exp}}, \code{\link{confint.mcprofile}}
#' 
#' @keywords misc

expit.mcpCI <-
  function(x){
    expit <- function(x) 1/(1 + exp(-x))
    x$estimate <- expit(x$estimate)
    x$confint <- expit(x$confint)
    return(x)
  }