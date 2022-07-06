#' exp transformation of Confidence Intervals
#' 
#' Exponential transformation of confidence interval estimates in mcpCI objects.
#' 
#' @param x An object of class mcpCI
#' 
#' @return An object of class mcpCI with transformed estimates.
#' 
#' @family confidence interval transformations
#' @seealso \code{\link{exp}}, \code{\link{confint.mcprofile}}
#' 
#' @keywords misc

exp.mcpCI <-
  function(x){
    x$estimate <- exp(x$estimate)
    x$confint <- exp(x$confint)
    return(x)
  }

