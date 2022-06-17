#' Fitting Order-Restricted Generalized Linear Models
#' 
#' \code{orglm.fit} is used to fit generalized linear models with restrictions on the parameters, specified by giving a description of the linear predictor, a description of the error distribution, and a description of a matrix with linear constraints. The \code{quadprog} package is used to apply linear constraints on the parameter vector.
#' 
#' Non-\code{NULL} \code{weights} can be used to indicate that different observations have different dispersions (with the values in \code{weights} being inversely proportional to the dispersions); or equivalently, when the elements of \code{weights} are positive integers \eqn{w_i}, that each response \eqn{y_i} is the mean of \eqn{w_i} unit-weight observations.  For a binomial GLM prior weights are used to give the number of trials when the response is the proportion of successes: they would rarely be used for a Poisson GLM.
#' If more than one of \code{etastart}, \code{start} and \code{mustart} is specified, the first in the list will be used.  It is often advisable to supply starting values for a \code{\link{quasi}} family, and also for families with unusual links such as \code{gaussian("log")}.
#' For the background to warning messages about \sQuote{fitted probabilities numerically 0 or 1 occurred} for binomial GLMs, see Venables & Ripley (2002, pp. 197--8).
#' 
#' @param x is a design matrix of dimension \code{n * p}
#' @param y is a vector of observations of length \code{n}
#' @param family a description of the error distribution and link function to be used in the model. This can be a character string naming a family function, a family function or the result of a call to a family function.  (See \code{\link{family}} for details of family functions.)
#' @param weights an optional vector of \sQuote{prior weights} to be used in the fitting process.  Should be \code{NULL} or a numeric vector.
#' @param start starting values for the parameters in the linear predictor.
#' @param etastart starting values for the linear predictor.
#' @param mustart starting values for the vector of means.
#' @param offset this can be used to specify an \emph{a priori} known component to be included in the linear predictor during fitting. This should be \code{NULL} or a numeric vector of length equal to the number of cases.  One or more \code{\link{offset}} terms can be included in the formula instead or as well, and if more than one is specified their sum is used.  See \code{\link{model.offset}}.
#' @param control a list of parameters for controlling the fitting process.  For \code{orglm.fit} this is passed to \code{\link{glm.control}}.
#' @param intercept logical. Should an intercept be included in the \emph{null} model?
#' @param constr a matrix with linear constraints. The columns of this matrix should correspond to the columns of the design matrix.
#' @param rhs right hand side of the linear constraint formulation. A numeric vector with a length corresponding to the rows of \code{constr}.
#' @param nec Number of equality constrints. The first \code{nec} constraints defined in \code{constr} are treated as equality constraints; the remaining ones are inequality constraints.
#' 
#' @return An object of class \code{"glm"} is a list containing at least the following components:
#' \describe{
#'   \item{coefficients}{a named vector of coefficients}
#'   \item{residuals}{the \emph{working} residuals, that is the residuals in the final iteration of the IWLS fit. Since cases with zero weights are omitted, their working residuals are \code{NA}.}
#'   \item{fitted.values}{the fitted mean values, obtained by transforming the linear predictors by the inverse of the link function.}
#'   \item{rank}{the numeric rank of the fitted linear model.}
#'   \item{family}{the \code{\link{family}} object used.}
#'   \item{linear.predictors}{the linear fit on link scale.}
#'   \item{deviance}{up to a constant, minus twice the maximized log-likelihood. Where sensible, the constant is chosen so that a saturated model has deviance zero.}
#'   \item{null.deviance}{The deviance for the null model, comparable with \code{deviance}. The null model will include the offset, and an intercept if there is one in the model.  Note that this will be incorrect if the link function depends on the data other than through the fitted mean: specify a zero offset to force a correct calculation.}
#'   \item{iter}{the number of iterations of IWLS used.}
#'   \item{weights}{the \emph{working} weights, that is the weights in the final iteration of the IWLS fit.}
#'   \item{prior.weights}{the weights initially supplied, a vector of \code{1}s if none were.}
#'   \item{df.residual}{the residual degrees of freedom of the unconstrained model.}
#'   \item{df.null}{the residual degrees of freedom for the null model.}
#'   \item{y}{if requested (the default) the \code{y} vector used. (It is a vector even for a binomial model.)}
#'   \item{converged}{logical. Was the IWLS algorithm judged to have converged?}
#'   \item{boundary}{logical. Is the fitted value on the boundary of the attainable values?}
#' }
#' 
#' @author Modification of the original glm.fit by Daniel Gerhard.
#'   The original R implementation of \code{glm} was written by Simon Davies working for Ross Ihaka at the University of Auckland, but has since been extensively re-written by members of the R Core team.
#'   The design was inspired by the S function of the same name described in Hastie & Pregibon (1992).
#'   
#' @references 
#'   \itemize{
#'     \item Dobson, A. J. (1990) \emph{An Introduction to Generalized Linear Models.} London: Chapman and Hall.
#'     \item Hastie, T. J. and Pregibon, D. (1992) \emph{Generalized linear models.} Chapter 6 of \emph{Statistical Models in S} eds J. M. Chambers and T. J. Hastie, Wadsworth & Brooks/Cole.
#'     \item McCullagh P. and Nelder, J. A. (1989) \emph{Generalized Linear Models.} London: Chapman and Hall.
#'     \item Venables, W. N. and Ripley, B. D. (2002) \emph{Modern Applied Statistics with S.} New York: Springer.
#'   }
#'   
#' @seealso \code{\link{glm}}, \code{\link[quadprog]{solve.QP}}
#'   
#' @keywords models

orglm.fit <- function (x, y, weights = rep(1, nobs), start = NULL, etastart = NULL, mustart = NULL, offset = rep(0, nobs), family = gaussian(), control = list(), intercept = TRUE, constr, rhs, nec){

  ###################
  orr <- function(x, y, constr, rhs, nec){
    unc <- lm.fit(x, y)
    tBeta <- as.vector(coefficients(unc))
    invW <- t(x) %*% x
    orsolve <- function(tBeta, invW, Constr, RHS, NEC) {
      Dmat <- 2 * invW
      dvec <- 2 * tBeta %*% invW
      Amat <- t(Constr)
      solve.QP(Dmat, dvec, Amat, bvec = RHS, meq = NEC)
    }
    orBeta <- tBeta
    val <- 0
    for (i in 1:control$maxit) {
      sqp <- orsolve(orBeta, invW, constr, rhs, nec)
      orBeta <- sqp$solution
      if (abs(sqp$value - val) <= control$epsilon) 
        break
      else val <- sqp$value
    }
    return(list(coefficients=orBeta))
  }
  
  ###############
  control <- do.call("glm.control", control)
  x <- as.matrix(x)
  if (is.vector(constr)) constr <- matrix(constr, nrow=1)
  if (ncol(constr) != ncol(x)) stop(paste("constr has not correct dimensions.\nNumber of columns (",ncol(constr),") should equal the number of parameters: ", ncol(x), sep=""))
  if (length(rhs) != nrow(constr)) stop(paste("rhs has a different number of elements than there are numbers of rows in constr (",length(rhs), " != ", nrow(constr), ")", sep=""))
  if (nec < 0) stop("nec needs to be positive")
  if (nec > length(rhs)) stop(paste("nec is larger than the number of constraints. (",nec," > ",length(rhs),")", sep=""))
  xnames <- dimnames(x)[[2L]]
  ynames <- if (is.matrix(y)) rownames(y) else names(y)
  conv <- FALSE
  nobs <- NROW(y)
  nvars <- ncol(x)
  EMPTY <- nvars == 0
  if (is.null(weights)) weights <- rep.int(1, nobs)
  if (is.null(offset)) offset <- rep.int(0, nobs)
  variance <- family$variance
  linkinv <- family$linkinv
  if (!is.function(variance) || !is.function(linkinv)) stop("'family' argument seems not to be a valid family object", call. = FALSE)
  dev.resids <- family$dev.resids
  aic <- family$aic
  mu.eta <- family$mu.eta
  unless.null <- function(x, if.null) if (is.null(x)) if.null  else x
  valideta <- unless.null(family$valideta, function(eta) TRUE)
  validmu <- unless.null(family$validmu, function(mu) TRUE)
  if (is.null(mustart)) {
    eval(family$initialize)
  } else {
    mukeep <- mustart
    eval(family$initialize)
    mustart <- mukeep
  }
  if (EMPTY) {
    eta <- rep.int(0, nobs) + offset
    if (!valideta(eta)) stop("invalid linear predictor values in empty model", call. = FALSE)
    mu <- linkinv(eta)
    if (!validmu(mu)) stop("invalid fitted means in empty model", call. = FALSE)
    dev <- sum(dev.resids(y, mu, weights))
    w <- ((weights * mu.eta(eta)^2)/variance(mu))^0.5
    residuals <- (y - mu)/mu.eta(eta)
    good <- rep(TRUE, length(residuals))
    boundary <- conv <- TRUE
    coef <- numeric()
    iter <- 0L
  } else {
    coefold <- NULL
    eta <- if (!is.null(etastart)) etastart else if (!is.null(start)) if (length(start) != nvars) stop(gettextf("length of 'start' should equal %d and correspond to initial coefs for %s", nvars, paste(deparse(xnames), collapse = ", ")), domain = NA) else {
      coefold <- start
      offset + as.vector(if (NCOL(x) == 1L) x * start else x %*% start)
    } else family$linkfun(mustart)
    mu <- linkinv(eta)
    if (!(validmu(mu) && valideta(eta))) stop("cannot find valid starting values: please specify some", call. = FALSE)
    devold <- sum(dev.resids(y, mu, weights))
    boundary <- conv <- FALSE
    
    #################################################
    for (iter in 1L:control$maxit) {
      good <- weights > 0
      varmu <- variance(mu)[good]
      if (any(is.na(varmu))) stop("NAs in V(mu)")
      if (any(varmu == 0)) stop("0s in V(mu)")
      mu.eta.val <- mu.eta(eta)
      if (any(is.na(mu.eta.val[good]))) stop("NAs in d(mu)/d(eta)")
      good <- (weights > 0) & (mu.eta.val != 0)
      if (all(!good)) {
        conv <- FALSE
        warning("no observations informative at iteration ", iter)
        break
      }
      z <- (eta - offset)[good] + (y - mu)[good]/mu.eta.val[good]
      w <- sqrt((weights[good] * mu.eta.val[good]^2)/variance(mu)[good])
      ngoodobs <- as.integer(nobs - sum(!good))      
      fit <- orr(x[good, , drop = FALSE] * w, z * w, constr, rhs, nec)      
      if (any(!is.finite(fit$coefficients))) {
        conv <- FALSE
        warning(gettextf("non-finite coefficients at iteration %d", iter), domain = NA)
        break
      }
      #if (nobs < fit$rank) stop(gettextf("X matrix has rank %d, but only %d observations", fit$rank, nobs), domain = NA)
      #start[fit$pivot] <- fit$coefficients
      start <- fit$coefficients
      eta <- drop(x %*% start)
      mu <- linkinv(eta <- eta + offset)
      dev <- sum(dev.resids(y, mu, weights))
      if (control$trace) cat("Deviance =", dev, "Iterations -", iter, "\n")
      boundary <- FALSE
      if (!is.finite(dev)) {
        if (is.null(coefold)) stop("no valid set of coefficients has been found: please supply starting values", call. = FALSE)
        warning("step size truncated due to divergence", call. = FALSE)
        ii <- 1
        while (!is.finite(dev)) {
          if (ii > control$maxit) stop("inner loop 1; cannot correct step size", call. = FALSE)
          ii <- ii + 1
          start <- (start + coefold)/2
          eta <- drop(x %*% start)
          mu <- linkinv(eta <- eta + offset)
          dev <- sum(dev.resids(y, mu, weights))
        }
        boundary <- TRUE
        if (control$trace) cat("Step halved: new deviance =", dev, "\n")
      }
      if (!(valideta(eta) && validmu(mu))) {
        if (is.null(coefold)) stop("no valid set of coefficients has been found: please supply starting values", call. = FALSE)
        warning("step size truncated: out of bounds", call. = FALSE)
        ii <- 1
        while (!(valideta(eta) && validmu(mu))){
          if (ii > control$maxit) stop("inner loop 2; cannot correct step size", call. = FALSE)
          ii <- ii + 1
          start <- (start + coefold)/2
          eta <- drop(x %*% start)
          mu <- linkinv(eta <- eta + offset)
        }
        boundary <- TRUE
        dev <- sum(dev.resids(y, mu, weights))
        if (control$trace) cat("Step halved: new deviance =", dev, "\n")
      }
      if (abs(dev - devold)/(0.1 + abs(dev)) < control$epsilon) {
        conv <- TRUE
        coef <- start
        break
      } else {
        devold <- dev
        coef <- coefold <- start
      }
    }

    ##############################
    if (!conv) warning("orglm.fit: algorithm did not converge", call. = FALSE)
    if (boundary) warning("orglm.fit: algorithm stopped at boundary value", call. = FALSE)
    eps <- 10 * .Machine$double.eps
    if (family$family == "binomial") {
      if (any(mu > 1 - eps) || any(mu < eps)) warning("orglm.fit: fitted probabilities numerically 0 or 1 occurred", call. = FALSE)
    }
    if (family$family == "poisson") {
      if (any(mu < eps)) warning("orglm.fit: fitted rates numerically 0 occurred", call. = FALSE)
    }
    #if (fit$rank < nvars) coef[fit$pivot][seq.int(fit$rank + 1, nvars)] <- NA
    xxnames <- xnames#[fit$pivot]
    residuals <- (y - mu)/mu.eta(eta)
    #fit$qr <- as.matrix(qr(x[good, , drop = FALSE] * w)$qr)
    nr <- min(sum(good), nvars)
    #if (nr < nvars) {
    #  Rmat <- diag(nvars)
    #  Rmat[1L:nr, 1L:nvars] <- fit$qr[1L:nr, 1L:nvars]
    #} else Rmat <- fit$qr[1L:nvars, 1L:nvars]
    #Rmat <- as.matrix(Rmat)
    #Rmat[row(Rmat) > col(Rmat)] <- 0
    names(coef) <- xnames
    #colnames(fit$qr) <- xxnames
    #dimnames(Rmat) <- list(xxnames, xxnames)
  }
  names(residuals) <- ynames
  names(mu) <- ynames
  names(eta) <- ynames
  wt <- rep.int(0, nobs)
  wt[good] <- w^2
  names(wt) <- ynames
  names(weights) <- ynames
  names(y) <- ynames
  #if (!EMPTY) names(fit$effects) <- c(xxnames[seq_len(fit$rank)], rep.int("", sum(good) - fit$rank))
  wtdmu <- if (intercept) sum(weights * y)/sum(weights) else linkinv(offset)
  nulldev <- sum(dev.resids(y, wtdmu, weights))
  n.ok <- nobs - sum(weights == 0)
  nulldf <- n.ok - as.integer(intercept)
  fit$rank <- rank <- if (EMPTY) 0 else qr(x)$rank
  resdf <- n.ok - rank
  #aic.model <- aic(y, n, mu, weights, dev) + 2 * rank
  fit <- list(coefficients = coef, residuals = residuals, fitted.values = mu, rank=rank, family = family, linear.predictors = eta, deviance = dev, null.deviance = nulldev, iter = iter, weights = wt, prior.weights = weights, df.residual = resdf, df.null = nulldf, y = y, X=x, converged = conv, boundary = boundary, aic=NA, constr=constr, rhs=rhs, nec=nec)
  class(fit) <- c("orglm", "glm", "lm")
  return(fit)
}

