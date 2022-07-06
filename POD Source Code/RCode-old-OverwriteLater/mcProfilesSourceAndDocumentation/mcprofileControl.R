#' mcprofile Control Arguments
#' 
#' Control arguments for the mcprofile function
#' 
#' @param maxsteps Maximum number of points to be used for profiling each parameter.
#' @param alpha Highest significance level allowed for the profile t-statistics (Bonferroni adjusted)
#' @param del Suggested change on the scale of the profile t-statistics. Default value chosen to allow profiling at about 10 parameter values.
#' @seealso \code{\link{mcprofile}}
#' @keywords misc

mcprofileControl <-
  function(maxsteps=10, alpha=0.01, del=function(zmax) zmax/5){
    list(maxsteps=maxsteps, alpha=alpha, del=del)
  }