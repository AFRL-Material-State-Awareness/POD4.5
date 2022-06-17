#' Construction of Multiple Contrast Profiles
#' 
#' Calculates signed root deviance profiles given a \code{\link{glm}} or \code{\link{lm}} object. The profiled parameters of interest are defined by providing a contrast matrix.
#' 
#' The profiles are calculates separately for each row of the contrast matrix. The profiles are calculated by constrained IRWLS optimization, implemented in function \code{orglm}, using the quadratic programming algorithm of package \code{quadprog}.
#' 
#' @param object An object of class \code{\link{glm}} or \code{\link{lm}}
#' @param CM A contrast matrix for the definition of parameter linear combinations (\code{CM \%*\% coefficients(object)}). The number of columns should be equal to the number of estimated parameters. Providing row names is recommendable.
#' @param control A list with control arguments. See \code{\link{mcprofileControl}}.
#' @param grid A matrix or list with profile support coordinates. Each column of the matrix or slot in a list corresponds to a row in the contrast matrix, each row of the grid matrix or element of a numeric vector in each list slot corresponds to a candidate of the contrast parameter. If NULL (default), a grid is found automatically similar to function \code{\link[MASS]{profile.glm}}.
#' @return An object of class mcprofile. The slot \code{srdp} contains the profiled signed root deviance statistics. The \code{optpar} slot contains a matrix with profiled parameter estimates.
#' @family mcprofile
#' @seealso \code{\link[MASS]{profile.glm}}, \code{\link[multcomp]{glht}}, \code{\link[multcomp]{contrMat}}, \code{\link{confint.mcprofile}}, \code{\link{summary.mcprofile}}, \code{\link[quadprog]{solve.QP}}
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
#' # plot profiles
#' plot(dmcp)
#' # confidence intervals
#' (ci <- confint(dmcp))
#' plot(ci)


mcprofile <-
function(object, CM, control=mcprofileControl(), grid=NULL) UseMethod("mcprofile")

#' @rdname mcprofile
mcprofile.glm <-
function(object, CM, control=mcprofileControl(), grid=NULL){
  if (is.null(rownames(CM))) rownames(CM) <- paste("C",1:nrow(CM), sep="")
  if (is.null(colnames(CM))) colnames(CM) <- names(coefficients(object))
  if (ncol(CM) != length(coefficients(object))) stop("Equal number of contrast and model coefficients needed!")

  df.needed <- family(object)$family == "gaussian" | length(grep("quasi", family(object)$family)) == 1 | length(grep("Negative Binomial", family(object)$family)) == 1

  ## construct grid
  if (is.null(grid)) grid <- constructGrid(object, CM, control)
  if (is.matrix(grid)) grid <- lapply(1:ncol(grid), function(i) na.omit(grid[,i]))
  ## check grid
  if (length(grid) != nrow(CM)) stop("Number of contrasts and grid support vectors differ!")
  if (any(sapply(grid, function(g) any(rank(g) != 1:length(g))))) stop("grid support is not in increasing order!") 
  if (any(sapply(grid, length) < 2)) stop("At least 2 grid supports per contrast are needed!")  

  ## model info
  est <- coefficients(object)
  OriginalDeviance <- object$deviance
  DispersionParameter <- summary(object)$dispersion ### Dispersion estimate @ every step needed?
  mf <- model.frame(object)
  Y <- model.response(mf)
  n <- NROW(Y)
  O <- model.offset(mf)
  if (!length(O)) O <- rep(0, n)
  W <- model.weights(mf)
  if (length(W) == 0L) W <- rep(1, n)
  X <- model.matrix(object)
  fam <- family(object)
  etastart <- X %*% est
  glmcontrol <- object$control

  ## profiling
  prolist <- lapply(1:nrow(CM), function(i){
    K <- CM[i,,drop=FALSE]
    glmpro <- glm_profiling(X, Y, W, etastart, O, fam, glmcontrol, est, OriginalDeviance, DispersionParameter, K, grid[[i]])
    return(glmpro)
  })  
    
  out <- list()
  out$object <- object
  out$CM <- CM
  out$srdp <- lapply(prolist, function(x) x[[1]])
  out$optpar <- lapply(prolist, function(x) x[[2]])
  out$cobject <- lapply(prolist, function(x) x[[3]])
  if (df.needed) out$df <- df.residual(object) else df <- NULL
  class(out) <- "mcprofile"
  out
}

#' @rdname mcprofile
mcprofile.lm <-
function(object, CM, control=mcprofileControl(), grid=NULL){
  oc <- as.list(object$call)
  oc$family <- call("gaussian")
  oc[[1]] <- as.symbol("glm")
  object <- eval(as.call(oc))
  mcprofile.glm(object, CM=CM, control=control, grid=grid)
}



