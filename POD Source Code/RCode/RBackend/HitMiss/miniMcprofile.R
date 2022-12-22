###########################################
# The following code in this R script is from the mcprofile package and is licensed under GPLv2 OR GPLv3
# (GPLv3 license header with preamble is shown below).
# The methods were pulled and the library was excluded due to a license incompatible dependency (mvtnorm)
# For more information on mcprofile, see : https://cran.r-project.org/web/packages/mcprofile/index.html
#
# author(s) and maintainer(s): Daniel Gerhard <00gerhard at gmail.com>
#
# CITATION:
#@Article{,
# title = {Simultaneous Small Sample Inference For Linear
#   Combinations Of Generalized Linear Model Parameters},
# author = {Daniel Gerhard},
# journal = {Communications in Statistics - Simulation and
#   Computation},
# year = {2016},
# volume = {48},
# issue = {8},
# pages = {2678-2690},
# doi = {10.1080/03610918.2014.895836},
# }
#
#
# LICENSE:
# GNU GENERAL PUBLIC LICENSE
# Version 3, 29 June 2007
# 
# Copyright (C) 2007 Free Software Foundation, Inc. <https://fsf.org/>
#   Everyone is permitted to copy and distribute verbatim copies
# of this license document, but changing it is not allowed.
# 
# Preamble
# 
# The GNU General Public License is a free, copyleft license for
# software and other kinds of works.
# 
# The licenses for most software and other practical works are designed
# to take away your freedom to share and change the works.  By contrast,
# the GNU General Public License is intended to guarantee your freedom to
# share and change all versions of a program--to make sure it remains free
# software for all its users.  We, the Free Software Foundation, use the
# GNU General Public License for most of our software; it applies also to
# any other work released this way by its authors.  You can apply it to
# your programs, too.
# 
# When we speak of free software, we are referring to freedom, not
# price.  Our General Public Licenses are designed to make sure that you
# have the freedom to distribute copies of free software (and charge for
#                                                         them if you wish), that you receive source code or can get it if you
# want it, that you can change the software or use pieces of it in new
# free programs, and that you know you can do these things.
# 
# To protect your rights, we need to prevent others from denying you
# these rights or asking you to surrender the rights.  Therefore, you have
# certain responsibilities if you distribute copies of the software, or if
# you modify it: responsibilities to respect the freedom of others.
# 
# For example, if you distribute copies of such a program, whether
# gratis or for a fee, you must pass on to the recipients the same
# freedoms that you received.  You must make sure that they, too, receive
# or can get the source code.  And you must show them these terms so they
# know their rights.
# 
# Developers that use the GNU GPL protect your rights with two steps:
#   (1) assert copyright on the software, and (2) offer you this License
# giving you legal permission to copy, distribute and/or modify it.
# 
# For the developers' and authors' protection, the GPL clearly explains
# that there is no warranty for this free software.  For both users' and
# authors' sake, the GPL requires that modified versions be marked as
# changed, so that their problems will not be attributed erroneously to
# authors of previous versions.
# 
# Some devices are designed to deny users access to install or run
# modified versions of the software inside them, although the manufacturer
# can do so.  This is fundamentally incompatible with the aim of
# protecting users' freedom to change the software.  The systematic
# pattern of such abuse occurs in the area of products for individuals to
# use, which is precisely where it is most unacceptable.  Therefore, we
# have designed this version of the GPL to prohibit the practice for those
# products.  If such problems arise substantially in other domains, we
# stand ready to extend this provision to those domains in future versions
# of the GPL, as needed to protect the freedom of users.
# 
#   Finally, every program is threatened constantly by software patents.
# States should not allow patents to restrict development and use of
# software on general-purpose computers, but in those that do, we wish to
# avoid the special danger that patents applied to a free program could
# make it effectively proprietary.  To prevent this, the GPL assures that
# patents cannot be used to render the program non-free.
# 
#
###########################################
#main function for linear combo generator- this function implements parallelization to speed up the process slightly
minimcprofile <-
  function(object, CM, control=minimcprofileControl(), grid=NULL){
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
    clust= makeCluster(detectCores()/2)
    clusterEvalQ(clust,library(quadprog))
    clusterExport(cl = clust, list("constructGrid", "glm_profiling", "orglm.fit"))
    prolist <- parLapply(clust, 1:nrow(CM), function(i){
      K <- CM[i,,drop=FALSE]
      glmpro <- glm_profiling(X, Y, W, etastart, O, fam, glmcontrol, est, OriginalDeviance, DispersionParameter, K, grid[[i]])
      return(glmpro)
    })
    stopCluster(clust)
    
    # prolist <- lapply(1:nrow(CM), function(i){
    #   K <- CM[i,,drop=FALSE]
    #   glmpro <- glm_profiling(X, Y, W, etastart, O, fam, glmcontrol, est, OriginalDeviance, DispersionParameter, K, grid[[i]])
    #   return(glmpro)
    # })

    out <- list()
    out$object <- object
    out$CM <- CM
    out$srdp <- lapply(prolist, function(x) x[[1]])
    out$optpar <- lapply(prolist, function(x) x[[2]])
    out$cobject <- lapply(prolist, function(x) x[[3]])
    if (df.needed) out$df <- df.residual(object) else df <- NULL
    class(out) <- "mcprofile"
    return(out)
  }
#standard mcprofile function (no parallelization used)
mcprofile <-
  function(object, CM, control=minimcprofileControl(), grid=NULL){
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
    clust= makeCluster(detectCores()-1)
    clusterEvalQ(clust,library(quadprog))
    clusterExport(cl = clust, list("constructGrid", "glm_profiling", "orglm.fit"))
    prolist <- parLapply(clust, 1:nrow(CM), function(i){
      K <- CM[i,,drop=FALSE]
      glmpro <- glm_profiling(X, Y, W, etastart, O, fam, glmcontrol, est, OriginalDeviance, DispersionParameter, K, grid[[i]])
      return(glmpro)
    })
    stopCluster(clust)
    
    # prolist <- lapply(1:nrow(CM), function(i){
    #   K <- CM[i,,drop=FALSE]
    #   glmpro <- glm_profiling(X, Y, W, etastart, O, fam, glmcontrol, est, OriginalDeviance, DispersionParameter, K, grid[[i]])
    #   return(glmpro)
    # })
    
    out <- list()
    out$object <- object
    out$CM <- CM
    out$srdp <- lapply(prolist, function(x) x[[1]])
    out$optpar <- lapply(prolist, function(x) x[[2]])
    out$cobject <- lapply(prolist, function(x) x[[3]])
    if (df.needed) out$df <- df.residual(object) else df <- NULL
    class(out) <- "mcprofile"
    return(out)
}
#second function
glm_profiling <-
  function(X, Y, W, etastart, O, fam, glmcontrol, est, OriginalDeviance, DispersionParameter, K, b){    
    z <- numeric(length(b))
    pmi <- matrix(nrow=length(b), ncol=length(est))
    obj <- list()
    Kest <- as.vector(K %*% est)
    signs <- rep(1, length(b))
    signs[Kest > b] <- -1
    
    bdist <- abs(b-Kest)
    nbi <- order(bdist[signs < 0])
    pbi <-  order(bdist[signs > 0]) + max(nbi)
    bind <- c(nbi,pbi)
    oeta <- logical(length=length(b))  
    oeta[max(nbi + 1)] <- TRUE
    
    etavec <- etastart
    for (i in bind){
      fm <- try(orglm.fit(x=X, y=Y, weights=W, etastart=etavec, offset=O, family=fam, control=glmcontrol, constr=K, rhs=b[i], nec=1), silent=TRUE)
      if (class(fm)[1] != "try-error"){
        zz <- signs[i]*sqrt(abs(fm$deviance - OriginalDeviance)/DispersionParameter)
        z[i] <- zz
        pmi[i,] <- fm$coefficients
        obj[[i]] <- fm
        if (fm$converged == TRUE & oeta[i] == FALSE) etavec <- X %*% fm$coefficients
        if (oeta[i] == TRUE) etavec <- etastart
      } else {
        z[i] <- NA
        obj[[i]] <- NA
      }
    }
    
    list(stats=cbind(b, z), param=pmi, object=obj)
  }
constructGrid <- function(object, CM, control){
  est <- coefficients(object)
  Kest <- CM  %*% est
  sdKest <- sqrt(diag(CM %*% vcov(object) %*% t(CM)))
  n <- NROW(object$y)
  p <- length(est)
  
  if (n > p){
    zmax <- sqrt(qf(1 - control$alpha/nrow(CM), 1, n - p))
  } else {
    zmax <- sqrt(qchisq(1 - control$alpha/nrow(CM), 1))
  }
  del <- control$del(zmax)
  sst <- c(-1*(control$maxsteps:1), 0:control$maxsteps)
  
  grid <- sapply(1:nrow(CM), function(k){
    sapply(1:length(sst), function(i){
      Kest[k] + sst[i] * del * sdKest[k]
    })
  })  
  return(grid)
}
#helper function for parameters
minimcprofileControl <-
  function(maxsteps=10, alpha=0.01, del=function(zmax) zmax/5){
    list(maxsteps=maxsteps, alpha=alpha, del=del)
  }
###large function that does glm fitting
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
    xxnames <- xnames#[fit$pivot]
    residuals <- (y - mu)/mu.eta(eta)
    nr <- min(sum(good), nvars)
    names(coef) <- xnames
  }
  names(residuals) <- ynames
  names(mu) <- ynames
  names(eta) <- ynames
  wt <- rep.int(0, nobs)
  wt[good] <- w^2
  names(wt) <- ynames
  names(weights) <- ynames
  names(y) <- ynames
  wtdmu <- if (intercept) sum(weights * y)/sum(weights) else linkinv(offset)
  nulldev <- sum(dev.resids(y, wtdmu, weights))
  n.ok <- nobs - sum(weights == 0)
  nulldf <- n.ok - as.integer(intercept)
  fit$rank <- rank <- if (EMPTY) 0 else qr(x)$rank
  resdf <- n.ok - rank
  fit <- list(coefficients = coef, residuals = residuals, fitted.values = mu, rank=rank, family = family, linear.predictors = eta, deviance = dev, null.deviance = nulldev, iter = iter, weights = wt, prior.weights = weights, df.residual = resdf, df.null = nulldf, y = y, X=x, converged = conv, boundary = boundary, aic=NA, constr=constr, rhs=rhs, nec=nec)
  class(fit) <- c("orglm", "glm", "lm")
  return(fit)
}
########################
## Higher order approximation function
hoa <- function(object, maxstat=10){
  model <- object$object
  CM <- object$CM
  sumobj <- summary(model)
  Kuv <- sumobj$cov.unscaled
  r <- lapply(object$srdp, function(sr) sr[, 2])
  q <- lapply(wald.mlr(object)$srdp, function(sr) sr[, 2])
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
##################################
#confidence interval function for mcprofile without the additional settings
#the default parameters for adjust are "none" and "greater" for the purpose of PODv4.5
confint.mcprofile <-
  function(object, parm, level=0.95, adjust=c("single-step","none","bonferroni"), alternative=c("two.sided","less","greater"), ...){
    pam <- c("bonferroni", "none", "single-step")
    if (!(adjust[1] %in% pam)) stop(paste("adjust has to be one of:", paste(pam, collapse=", ")))
    CM <- object$CM
    df <- object$df
    cr <- NULL
    sdlist <- object$srdp  
    spl <- lapply(sdlist, function(x){
      x <- na.omit(x)
      try(interpSpline(x[,1], x[,2]))
    })
    
    if (adjust[1] == "none" | nrow(CM) == 1){
      if (alternative[1] == "two.sided") alpha <- (1-level)/2 else alpha <- 1-level
      if (is.null(df)) quant <- qnorm(1-alpha) else quant <- qt(1-alpha, df=df)
    }
    if (alternative[1] == "greater"){
      ci <- data.frame(sapply(spl, function(x, quant){
        pfun <- function(xc, obj, quant) predict(obj, xc)$y-quant
        lower <- try(uniroot(pfun, range(predict(x)$x), obj=x, quant=-quant)$root, silent=TRUE)
        if (class(lower)[1] == "try-error") lower <- NA
        cbind(c(lower))
      }, quant=quant))
      names(ci) <- "lower"
    } 
    out <- list()
    out$estimate <- data.frame(Estimate=CM %*% coefficients(object$object))
    out$confint <- ci
    out$cr <- cr
    out$CM <- CM
    out$quant <- quant
    out$alternative <- alternative[1]
    out$level <- level
    out$adjust <- adjust[1]
    class(out) <- "mcpCI"
    return(out)
  }
#wald.mlr function used for calculating MLR
wald.mlr <-
  function(object){
    srdp <- object$srdp
    est <- object$CM %*% coefficients(object$object)
    sde <- sqrt(diag(object$CM %*% vcov(object$object) %*% t(object$CM)))
    wsrdp <- lapply(1:length(srdp), function(i){
      srdpi <- srdp[[i]]
      b <- srdpi[,1]
      srdpi[,2] <- (b-est[i])/sde[i]
      srdpi 
    })
    object$srdp <- wsrdp
    return(object)
  }
#this function is needed for applying higher order approximation to the MLR confidence interval
detvarorglm <- function(object){
  eta <- object$X %*% object$coefficients
  y <- object$y
  z <- as.vector(eta  + (y - object$family$linkinv(eta))/object$family$mu.eta(eta))
  w <- sqrt(object$weights)  
  xm <- object$X*w
  A <- object$constr
  lmf <- lm(z * w ~ xm-1)  
  
  sx <- summary(lmf)$cov.unscaled
  sA <- qr.solve(A %*% sx %*% t(A))
  M <- diag(ncol(xm)) - sx %*% t(A) %*% sA %*% A
  Ms <- (diag(ncol(xm)) - t(A) %*% sA %*% A %*% sx)
  vc <- M %*% sx %*% Ms
  
  svdd <- svd(vc)$d
  eok <- svdd > 2 * .Machine$double.eps  
  
  evv <- svd(vc[A!=0,A!=0])$d
  evok <- evv > 2 * .Machine$double.eps  
  
  dvv <- prod(evv[evok])
  dvca <- prod(svdd[eok])/dvv
  return(dvca)
}