GenPODSignalResponse<-setRefClass("GenPODSignalResponse", fields=list(
                                 a.V_POD="matrix",
                                 aMu="numeric",
                                 aSigma="numeric",
                                 PODDataFrame="data.frame",
                                 criticalPoints="data.frame",
                                 keyAvalues="data.frame"
                                ),
                                 methods=list(
                                   initialize=function(a.V_PODInput=matrix(0, c(2,2)), aMuInput=0, aSigmaInput=0){
                                     a.V_POD<<-a.V_PODInput
                                     aMu<<-aMuInput
                                     aSigma<<-aSigmaInput
                                   },
                                   setPODSR=function(psPODDataFrame){
                                     PODDataFrame<<-psPODDataFrame
                                   },
                                   getPODSR=function(){
                                     return(PODDataFrame)
                                   },
                                   setCriticalPoints=function(psCriticalPoints){
                                     criticalPoints<<-psCriticalPoints
                                   },
                                   getCriticalPoints=function(){
                                     return(criticalPoints)
                                   },
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
                                   },
                                   genPODCurve=function(){
                                     numPlotPoints=101
                                     criticalPts=data.frame(index=NULL,a_50_50=NULL,a_90_50=NULL,a_90_95=NULL)
                                     plotPoints = data.frame(case=NULL, probabilities = NULL,defect_sizes=NULL, defect_sizes_upCI=NULL)
                                     #generate probabilities for pod curve
                                     probabilities = linspace(0,1,numPlotPoints)
                                     probabilities=probabilities[-1]
                                     probabilities=probabilities[-length(probabilities)]
                                     #smooth out top of curve
                                     additionalProbs= c(.995, .9975, .99875, .999375, .9996875, .99984375, .999921875, .9999609375, .99998046875,
                                                        .99999023437, .99999511718)
                                     probabilities=append(probabilities,additionalProbs )
                                     V_pod_df=data.frame(
                                       cat_level=rep(1,4),
                                       name=c("var_mu","var_cov1","var_cov2","var_sigma"),
                                       value=as.vector(a.V_POD)
                                     )
                                     
                                     # Remember to undo the transformation on the flaw sizes to get them in the original units (f_a_i)
                                     
                                     for (index in 1:length(unique(V_pod_df$cat_level))){
                                       V_pod_at_index = subset(V_pod_df,cat_level==index)
                                       #print(aMu)
                                       #print(aSigma)
                                       a_25_25 = f_a_i(a_p_q_df(0.5, 0.25, aMu[index], aSigma, V_pod_at_index))
                                       a_50_50 = f_a_i(a_p_q_df(0.5, 0.50, aMu[index], aSigma, V_pod_at_index))
                                       a_90_50 = f_a_i(a_p_q_df(0.9, 0.50, aMu[index], aSigma, V_pod_at_index))
                                       a_90_95 = f_a_i(a_p_q_df(0.9, 0.95, aMu[index], aSigma, V_pod_at_index))
                                       #criticalPts=rbind(criticalPts,data.frame(index,a_50_50,a_90_50,a_90_95))
                                       criticalPts=rbind(criticalPts,data.frame(index,a_25_25, a_50_50,a_90_50,aSigma, a_90_95))
                                       defect_sizes=f_a_i(a_p_q_df(probabilities, 0.5, aMu[index], aSigma, V_pod_at_index))
                                       #defect_sizesupCI = f_a_i(a_p_q_df(probabilities, 0.95, aMu[index], aSigma, V_pod_at_index))
                                       #plotPoints=rbind(plotPoints,data.frame(
                                      #   case=as.factor(rep(index,numPlotPoints)),probabilities, defect_sizes))#,defect_sizes_upCI))
                                     }
                                     defect_sizes_upCI=genStandardWald(defect_sizes)
                                     plotPoints=data.frame(defect_sizes, 
                                                           probabilities, 
                                                           defect_sizes_upCI)
                                     #set final parameters to be returned
                                     setPODSR(plotPoints)
                                     setCriticalPoints(criticalPts)
                                   },
                                   genStandardWald=function(defect_sizes){
                                     standardWald=function(x, aMu, aSigma, covarMatrix, a_p_q){
                                       return(aMu+ x*aSigma+ qnorm(.95, mean=0, sd=1, lower.tail=TRUE) *
                                                sqrt(covarMatrix[1,1]+2*x*covarMatrix[1,2]+x^2*covarMatrix[2,2])-a_p_q)
                                     }
                                     confident_interval=c()
                                     for(i in defect_sizes){
                                       #findRoot=uniroot(standardWald, interval=c(-100,100), aMu= .113276,
                                       #                  aSigma= .1380718, covarMatrix=a.V_POD, a_p_q=i)
                                       findRoot = tryCatch(uniroot(standardWald, interval=c(-10,10), aMu= aMu,
                                                      aSigma= aSigma, covarMatrix=a.V_POD, a_p_q=i), error = function(e) NaN)
                                       thisConfInt =pnorm(findRoot[[1]])
                                       confident_interval=c(confident_interval, c(thisConfInt))
                                     }
                                     return(confident_interval)
                                   },
                                   standardWald=function(x, aMu, aSigma, covarMatrix, a_p_q){
                                     return(aMu+ x*aSigma+ qnorm(.95, mean=0, sd=1, lower.tail=TRUE) *
                                              sqrt(covarMatrix[1,1]+2*x*covarMatrix[1,2]+x^2*covarMatrix[2,2])-a_p_q)
                                   },
                                   # Inverse function of f_a. Uncomment 1
                                   f_a_i=function(a){ return(a)#(a) # exp(a)#log(a) #sqrt(a)
                                   }
                                 ))
