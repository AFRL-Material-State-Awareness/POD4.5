
library(ggplot2) # gorgeous plots
library(gridExtra) # useful for plotting in grids
library(MASS) #contains boxcox and much more
#library(olsrr) #makes some nice plots to check assumptions
library(stats)
library(nlme) # contains gls = generalized least squares
library(pracma) #practical math...contains some functions from matlab
library(ggResidpanel)
library(car) # Need this for durbinWatsonTest
library(tibble)
library(survival)
library(corrplot)
folderLocation=dirname(rstudioapi::getSourceEditorContext()$path)
data_obs = read.csv(paste(folderLocation,'/Plot_Data_50.csv',sep=""), header=TRUE, col.names=c("y","x"))
data_obs=na.omit(data_obs)
# Larger decision thresholds shift the curve to larger flaw sizes. 
y_dec = 5     
# # Histogram should look normally distributed. It does not, but that's probably because there's so little data.
# ggplot(data)+theme_bw()+
#   geom_histogram(aes(x=y))+xlab("Response")
# 
# # Plot x vs y
# ggplot(data_obs, aes(x=x, y=y))+geom_point()+
#   ylab("Response")+xlab("Defect Size (a)")+theme_bw()+ylim(2,14)+xlim(0.11,0.31)
# 
# # Plot without the confidence interval
# ggplot(data_obs, aes(x=x, y=y))+geom_point()+geom_smooth(method='glm', formula=y ~ x, se=FALSE)+
#   ylab("Response")+xlab("Defect Size (a)")+theme_bw()+ylim(2,14)+xlim(0.11,0.31)
# 
# # Plot with confidence interval
# ggplot(data_obs, aes(x=x, y=y))+geom_point()+geom_smooth(method='glm', formula=y ~ x)+
#   ylab("Response")+xlab("Defect Size (a)")+theme_bw()+ylim(2,14)+xlim(0.11,0.31)

# Function transforming the flaw size. Uncomment 1
f_a=function(a){return(a) #log(a) #exp(a) #a^2
}
# Inverse function of f_a. Uncomment 1
f_a_i=function(a){ return(a) # exp(a)#log(a) #sqrt(a)
}

data_obs$x.trans=f_a(data_obs$x)
data_obs$y.trans=f_a(data_obs$y)
# Fitting a linear model
linearModel_lm = lm(y ~ x, data = data_obs, na.action=na.omit)

# Lots of plots to check assumptions
#summary(linearModel_lm)
plot(linearModel_lm)
resid_panel(linearModel_lm, plots="all")
# Histogram of the residuals should look normally distributed. It does not, but that's probably because there's so little data.
resid_panel(linearModel_lm)

linearTests=function(linearM){
  # Shapiro-Wilk Normality Test
  # Testing if the (studentized/standardized/Pearson) residuals are normally (Gaussian) distributed.
  # H_0: Normally distributed  VS H_a: Not Normally distributed
  # Fail to reject, so normal enough.
  shapiro=shapiro.test(studres(linearM))
  # Non-constant variance test... H_0: Constant variance VS H_a: Non-constant variance
  # We fail to reject H_0, so non-constant variance isn't a problem. 
  nonConst=ncvTest(linearM)
  # Test for auto-correlation (ie, a pattern in the data due to "time," or in this case, order)
  # Data does not have auto-correlation. 
  durbinWTest=durbinWatsonTest(linearM)
}

linearM=linearModel_lm
# It expects a censored object; this data is not censored but we can still use it.
# Note, we are using the Box Cox transformed y and f_a (log) transformed x
censored.a.hat <- Surv(time = data_obs$y.trans, time2 = data_obs$y.trans, type = "interval2")
#censored.a.hat <- Surv(time = data_obs$y, time2 = data_obs$y, type = "interval2")
a.hat.censor.df <- data.frame(censored.a.hat, x = f_a(data_obs$x))
# Here's the linear model. 
a.hat.vs.a.censored <- survreg(formula = censored.a.hat ~ x, 
                               dist = "gaussian", data = a.hat.censor.df)

#############################################################################################
#############################################################################################
a.hat.decision = y_dec # = 200
a.b0 <- as.numeric(a.hat.vs.a.censored$coef[1])
a.b1 <- as.numeric(a.hat.vs.a.censored$coef[2])
a.tau <- as.numeric(a.hat.vs.a.censored$scale) # random sigma
a.covariance.matrix <- a.hat.vs.a.censored$var
# Transform backward into the original units
# if(lambda!=0){
#   a.hat.decision = (a.hat.decision*lambda+1)^(1/lambda)
# }else{
#   a.hat.decision = exp(a.hat.decision)
# }
a.mu <- (a.hat.decision - a.b0)/a.b1
a.sigma <- a.tau/a.b1
POD.transition.matrix <- matrix(c(1, a.mu, 0, 0, a.sigma, -a.tau), nrow = 3, byrow = FALSE)
a.VCV <- (-1/a.b1)^2 * t(POD.transition.matrix)
a.50 <- a.mu
a.90 <- a.mu + qnorm(0.9) * a.sigma
z.90 <- qnorm(0.9)
#  a.U = (-1/a.b1)*matrix(c(1, a.mu, 0, 0, a.sigma, -1), nrow = 3, byrow = FALSE)
a.U = (-1/a.b1)*matrix(c(1, a.mu, 0, 0, a.sigma, -a.tau), nrow = 3, byrow = FALSE)
a.V_POD = t(a.U)%*%a.covariance.matrix%*%a.U
SD.a.90 = sqrt(a.V_POD[1,1]+2*z.90*a.V_POD[1,2]+(z.90^2)*a.V_POD[2,2])
a.90.95 <- a.mu + z.90 * a.sigma + qnorm(0.95) * SD.a.90
# Transform x back to the original units
a.50 <- f_a_i(a.50)
a.90 <- f_a_i(a.90)
a.90.95 <- f_a_i(a.90.95)

print("Variance Covariance Matrix for Fixed Effects")
a.covariance.matrix # 2x2
print("POD Transition Matrix")
POD.transition.matrix # matrix of size num_fix_ests+2*num_groups

tibble(
  Random_Variance = a.tau^2,
  Random_Sigma    = a.tau,
  Signal_Decision = a.hat.decision,
  estimated_mu    = a.mu,
  estimated_sigma = a.sigma
)

# Transform to POD space

a_p_q_df=function(p,q,mu,rand_sigma,V_pod_df){
  Var_mu = subset(V_pod_df,name=='var_mu')$value
  Var_sigma = subset(V_pod_df,name=='var_sigma')$value
  Cov_mu_sigma = subset(V_pod_df,name=='var_cov1')$value
  
  z_p = qnorm(p, mean=0, sd=1, lower.tail=TRUE)
  z_q = qnorm(q, mean=0, sd=1, lower.tail=TRUE)
  mu_x_p = mu+z_p*rand_sigma
  stdev_x_p = sqrt(Var_mu+2*z_p*Cov_mu_sigma+(z_p^2)*Var_sigma)
  
  return(mu_x_p+ z_q*stdev_x_p)
}

numPlotPoints=100
criticalPoints=data.frame(index=NULL,a_50_50=NULL,a_90_50=NULL,a_90_95=NULL)
plotPoints = data.frame(case=NULL, probabilities = NULL,defect_sizes=NULL, defect_sizes_upCI=NULL)
probabilities = linspace(0,1,numPlotPoints)


V_pod_df=data.frame(
  cat_level=rep(1,4),
  name=c("var_mu","var_cov1","var_cov2","var_sigma"),
  value=as.vector(a.V_POD)
  )

# Remember to undo the transformation on the flaw sizes to get them in the original units (f_a_i)
for (index in 1:length(unique(V_pod_df$cat_level))){
  V_pod_at_index = subset(V_pod_df,cat_level==index)
  a_50_50 = f_a_i(a_p_q_df(0.5, 0.50, a.mu[index], a.sigma, V_pod_at_index))
  a_90_50 = f_a_i(a_p_q_df(0.9, 0.50, a.mu[index], a.sigma, V_pod_at_index))
  a_90_95 = f_a_i(a_p_q_df(0.9, 0.95, a.mu[index], a.sigma, V_pod_at_index))
  criticalPoints=rbind(criticalPoints,data.frame(index,a_50_50,a_90_50,a_90_95))
  defect_sizes = f_a_i(a_p_q_df(probabilities, 0.5, a.mu[index], a.sigma, V_pod_at_index))
  defect_sizes_upCI = f_a_i(a_p_q_df(probabilities, 0.95, a.mu[index], a.sigma, V_pod_at_index))
  plotPoints=rbind(plotPoints,data.frame(
    case=as.factor(rep(index,numPlotPoints)),probabilities, defect_sizes,defect_sizes_upCI))
}

# POD Plot  
ggplot()+theme_bw()+
  geom_line(aes(plotPoints$defect_sizes,plotPoints$probabilities),colour="blue",size=1.5)+
  geom_line(aes(plotPoints$defect_sizes_upCI,plotPoints$probabilities),linetype=2,colour="darkcyan",size=1)+
  #    geom_point(aes(criticalPoints$a_50_50,0.5),colour=index,shape=1)+
  geom_hline(yintercept=0.9, colour="red")+
  geom_vline(xintercept=criticalPoints$a_90_50, colour="red",linetype=2)+
  geom_vline(xintercept=criticalPoints$a_90_95, colour="darkturquoise",linetype=2)+
  geom_point(aes(criticalPoints$a_90_50,0.9),shape=18,size=2, colour="red")+
  geom_point(aes(criticalPoints$a_90_95,0.9),shape=18,size=2, colour="darkturquoise")+
  xlim(min(na.exclude(plotPoints$defect_sizes)),max(na.exclude(plotPoints$defect_sizes)))+
  ylab("Probability of Detection, POD(a)")+
  xlab("Defect Size, a")
