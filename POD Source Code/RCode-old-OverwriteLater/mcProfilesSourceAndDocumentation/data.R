#' Aphid attraction at different light intensities
#' 
#' The light intensity (mumol/m^2s) of green LED light should be found, which attracts Aphis fabae best. At each of 4 replicates 20 aphids were put in a lightproof box with only one green LED at one end. All aphids that fly to the green light are caught and counted after a period of 5h. This procedure was replicated for 9 increasing light intensities.
#' 
#' @format A data frame with 36 observations on the following 3 variables.
#' \describe{
#'  \item{\code{light}}{a numeric vector denoting the concentration levels}
#'  \item{\code{black}}{a numeric vector with the number of aphids remaining in the box.}
#'  \item{\code{green}}{a numeric vector with the number of attracted aphids}
#' }
#' 
#' @references Akyazi, G (2009): Zum Einfluss auf Lichtintensitaet und Lichtqualitaet (Hochleistungs-LEDs) auf das Verhalten von Aphis fabae. IPP MSc 19.
"aphidlight"


#'Cell transformation assay dataset
#'
#' Balb//c 3T3 cells are treated with different concentrations of a carcinogen. Cells treated with a carcinogen do not stop proliferation. Number of foci (cell accumulations) are counted for 10 replicates per concentration level.
#' 
#' @format A data frame with 80 observations on the following 2 variables.
#' \describe{
#'   \item{\code{conc}}{a numeric vector denoting the concentration levels}
#'   \item{\code{foci}}{a numeric vector with the number of foci}
#' }
#' 
#' @references Thomas C (2008): ECVAM data
"cta"


#' Identifying the lethal dose of a crop protection product.
#' 
#' Increasing dose levels of a toxin, used as a pesticide for crop protection, is applied to non-target species. The lethal dose should be identified in this experiment. The dataset represents simulated data based on a real experiment.
#' 
#' @format A data frame with 6 observations on the following 3 variables.
#' \describe{
#'   \item{\code{dose}}{a numeric vector denoting the toxin concentration levels}
#'   \item{\code{dead}}{a numeric vector with the number of dead insects.}
#'   \item{\code{alive}}{a numeric vector with the number of surviving insects.}
#' }
#' 
#' @examples 
#' str(toxinLD)
#' 
#' ###############################################
#' # logistic regression on the logarithmic dose #
#' ###############################################
#' 
#' toxinLD$logdose <- log(toxinLD$dose)
#' fm <- glm(cbind(dead, alive) ~ logdose, data=toxinLD, family=binomial(link="logit"))
#' 
#' #############
#' # profiling #
#' #############
#' 
#' # contrast matrix
#' pdose <- seq(-1,2.3, length=7)
#' CM <- model.matrix(~ pdose)
#' 
#' # user defined grid to construct profiles
#' mcpgrid <- matrix(seq(-11,8,length=15), nrow=15, ncol=nrow(CM))
#' mc <- mcprofile(fm, CM, grid=mcpgrid)
#' 
#' ####################################
#' ## confidence interval calculation #
#' ####################################
#' 
#' # srdp profile
#' ci <- confint(mc)
#' ppdat <- data.frame(logdose=pdose)
#' ppdat$estimate <- fm$family$linkinv(ci$estimate$Estimate)
#' ppdat$lower <- fm$family$linkinv(ci$confint$lower)
#' ppdat$upper <- fm$family$linkinv(ci$confint$upper)
#' ppdat$method <- "profile"
#' 
#' # wald profile
#' wci <- confint(wald(mc))
#' wpdat <- ppdat
#' wpdat$estimate <- fm$family$linkinv(wci$estimate$Estimate)
#' wpdat$lower <- fm$family$linkinv(wci$confint$lower)
#' wpdat$upper <- fm$family$linkinv(wci$confint$upper)
#' wpdat$method <- "wald"
#' 
#' # higher order approximation
#' hci <- confint(hoa(mc))
#' hpdat <- ppdat
#' hpdat$estimate <- fm$family$linkinv(hci$estimate$Estimate)
#' hpdat$lower <- fm$family$linkinv(hci$confint$lower)
#' hpdat$upper <- fm$family$linkinv(hci$confint$upper)
#' hpdat$method <- "hoa"
#' 
#' # combine results
#' pdat <- rbind(ppdat, wpdat, hpdat)
#' 
#' 
#' #####################################
#' # estimating the lethal dose LD(25) #
#' #####################################
#' 
#' ld <- 0.25
#' pspf <- splinefun(ppdat$upper, pdose)
#' pll <- pspf(ld)
#' wspf <- splinefun(wpdat$upper, pdose)
#' wll <- wspf(ld)
#' hspf <- splinefun(hpdat$upper, pdose)
#' hll <- hspf(ld)
#' 
#' ldest <- data.frame(limit=c(pll, wll, hll), method=c("profile","wald", "hoa"))
#' 
#' ################################
#' # plot of intervals and LD(25) #
#' ################################
#' 
#' ggplot(toxinLD, aes(x=logdose, y=dead/(dead+alive))) + 
#'   geom_ribbon(data=pdat, aes(y=estimate, ymin=lower, ymax=upper, 
#'                              fill=method, colour=method, linetype=method), 
#'               alpha=0.1, size=0.95) +
#'   geom_line(data=pdat, aes(y=estimate, linetype=method), size=0.95) +
#'   geom_point(size=3) +
#'   geom_hline(yintercept=ld, linetype=2) +
#'   geom_segment(data=ldest, aes(x=limit, xend=limit, y=0.25, yend=-0.05, 
#'                                linetype=method), size=0.6, colour="grey2") +
#'   ylab("Mortality rate")
"toxinLD"
