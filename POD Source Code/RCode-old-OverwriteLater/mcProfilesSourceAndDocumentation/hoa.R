#' Higher order asymptotics using the modified likelihood root
#' 
#' Transforms a signed root deviance profile to a modified likelihood root profile.
#' 
#' @param object An object of class mcprofile
#' @param maxstat Limits the statistic to a maximum absolute value (default=10)
#' @return An object of class mcprofile with a hoa profile in the srdp slot.
#' @seealso \code{\link{mcprofile}}
#' @keywords misc
#' @examples 
#' #######################################
#' ## cell transformation assay example ##
#' #######################################
#' 
#' str(cta)
#' ## change class of cta$conc into factor
#' cta$concf <- factor(cta$conc, levels=unique(cta$conc))
#' 
#' ggplot(cta, aes(y=foci, x=concf)) + 
#'   geom_boxplot() +
#'   geom_dotplot(binaxis = "y", stackdir = "center", binwidth = 0.2) + 
#'   xlab("concentration")
#'   
#'   
#' # glm fit assuming a Poisson distribution for foci counts
#' # parameter estimation on the log link
#' # removing the intercept
#' fm <- glm(foci ~ concf-1, data=cta, family=poisson(link="log"))
#' 
#' ### Comparing each dose to the control by Dunnett-type comparisons
#' # Constructing contrast matrix
#' library(multcomp)
#' CM <- contrMat(table(cta$concf), type="Dunnett")
#' 
#' # calculating signed root deviance profiles
#' (dmcp <- mcprofile(fm, CM))
#' # computing profiles for the modified likelihood root
#' hp <- hoa(dmcp)
#' 
#' plot(hp)
#' 
#' # comparing confidence intervals
#' confint(hp)
#' confint(dmcp)


hoa <- function(object, maxstat=10){
  model <- object$object
  CM <- object$CM
  sumobj <- summary(model)
  Kuv <- sumobj$cov.unscaled
  r <- lapply(object$srdp, function(sr) sr[, 2])
  q <- lapply(wald(object)$srdp, function(sr) sr[, 2])
  j <- (1/det(Kuv))
  vc <- apply(CM, 1, function(cm) det(rbind(Kuv[cm != 0, cm != 0])))
  j.1 <- j * vc
  
  j.0 <- lapply(object$cobject, function(vm) sapply(vm, function(x) {
    xt <- try(1/(detvarorglm(x)), silent = TRUE)
    if (class(xt)[1] == "try-error") xt <- NA
    xt[xt > 10^12] <- NA
    return(xt)
  }))
  rho <- lapply(1:length(j.0), function(i) sqrt(j.1[i]/j.0[[i]]))
  adjr <- lapply(1:length(rho), function(i) r[[i]] + log(rho[[i]] * q[[i]]/r[[i]])/r[[i]])
  adjr <- lapply(adjr, function(x){
    x[is.infinite(x)] <- NA
    return(x)
  })
  
  rsrdp <- lapply(1:length(adjr), function(i) {
    srdpi <- object$srdp[[i]]
    b <- srdpi[, 1]
    rstar <- adjr[[i]]    
    srdpi[, 2] <- rstar
    return(na.omit(srdpi))
  })  
  
  rsrdp <- lapply(rsrdp, function(hsrdp){
    hsrdp[abs(hsrdp[,2]) < maxstat,]        
  })
  
  rest <- sapply(rsrdp, function(x) {
    x <- na.omit(x)
    ispl <- try(interpSpline(x[, 1], x[, 2]), silent = TRUE)
    pfun <- function(xc, obj) predict(obj, xc)$y
    try(uniroot(pfun, range(predict(ispl)$x), obj = ispl)$root, 
        silent = TRUE)
  })
  
  object$srdp <- rsrdp
  object$adjestimates <- rest
  return(object)           
}


             
             