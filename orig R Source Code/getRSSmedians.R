# RSS: RANKED SET SAMPLING WITH LOGISIC REGRESSIONS #####

# Uses a subset of the data, as given by "index_insp1"
# Calls the logistic regression model for each subset of data
# Averages over the results from all the logistic regressions and
# returns the results of fitting the model as "output_rows" dataframe

# No points file option is provided here, but one could be predicted using 
# the final model fit.

getRSSmedians=function(df,index_insp1){
  
  #print(paste("RSS Modeling for Insp ", df$Insp[1]))
  
  # Subset #1
  # Save the first subset of data into "df_rss" variable
  resamples=1
    df_rss=df[1:length(index_insp1[resamples,]),]
    df_rss$x=df$x[index_insp1[resamples,]]
    df_rss$y=df$y[index_insp1[resamples,]]
    df_rss$Uneven = ( df_rss$N[1]-sum(df_rss$y))/ df_rss$N[1]
    df_rss$Uneven_lower=min(df_rss$Uneven,sum(df_rss$y)/ df_rss$N[1])
    df_rss$Overlap=length(df_rss[df_rss$x>=min(df_rss$x[df_rss$y==1]) & df_rss$x<=max(df_rss$x[df_rss$y==0]), ]$x)/ df_rss$N[1]
  # Fit a logistic regression for the first subset of data
    getRSSResults=getLogitResults(df_rss)
    print(paste("Resample",resamples))
  
  # Subsets #2 - #maxResamples
  maxResamples=length(index_insp1[,1])
  for(resamples in 2:(maxResamples)){
    # Save the subset of data into "df_rss" variable
      df_rss=df[1:length(index_insp1[resamples,]),]
      df_rss$x=df$x[index_insp1[resamples,]]
      df_rss$y=df$y[index_insp1[resamples,]]
      df_rss$Uneven = ( df_rss$N[1]-sum(df_rss$y))/ df_rss$N[1]
      df_rss$Uneven_lower=min(df_rss$uneven,sum(df_rss$y)/ df_rss$N[1])
      df_rss$Overlap=length(df_rss[df_rss$x>=min(df_rss$x[df_rss$y==1]) & df_rss$x<=max(df_rss$x[df_rss$y==0]), ]$x)/ df_rss$N[1]
    # Fit a logistic regression to the current data subset.
      getRSSResults=rbind(getRSSResults,getLogitResults(df_rss))
      print(paste("Resample",resamples))
  }
  
  # Print how many NA subsets occurred. If it is many, the user may want to resample.
  # Averages are available for RSS using and RSS excluding NA values.
  # The average which excludes NA values should be used. 
  # Including NA leads to questionable results (ie, a90 > a9095)
  
  getRSSResultsComplete = getRSSResults[complete.cases(getRSSResults),]
  print(paste("RSS had ", length(getRSSResults$N)-length(getRSSResultsComplete$N),
              " fitted models with NA."))
  
  # RSS Medians when NA models are allowed to be included 
  getMedianRSSResult =     data.frame( 
               Intercept	= round(median(getRSSResults$Intercept,na.rm=TRUE),5),
               Slope	= round(median(getRSSResults$Slope,na.rm=TRUE),5),
               a25	=	 round(median(getRSSResults$a25,na.rm=TRUE),5),
               a50	=	 round(median(getRSSResults$a50,na.rm=TRUE),5),
               a90	=	 round(median(getRSSResults$a90,na.rm=TRUE),5),
               a9095	= round(median(getRSSResults$a9095,na.rm=TRUE),5),
               AIC	=	 round(median(getRSSResults$AIC),5),
               Iter	= round(median(getRSSResults$Iter),5),
               Converged	= round(median(getRSSResults$Converged),5),
               Separated	= round(median(getRSSResults$Separated),5),
               Uneven	= round(median(getRSSResults$Uneven),5),
               Overlap	= round(median(getRSSResults$Overlap),5),
               Insp	= round(median(getRSSResults$Insp),5),
               N	=	 round(median(getRSSResults$N),5),
               Model	="RSS with NA",
               CI = "Std. Wald")
  # RSS Medians when NA models are excluded  
  getMedianRSSResultComplete =    data.frame( 
               Intercept	= round(median(getRSSResultsComplete$Intercept,na.rm=TRUE),5),
               Slope	= round(median(getRSSResultsComplete$Slope,na.rm=TRUE),5),
               a25	=	 round(median(getRSSResultsComplete$a25,na.rm=TRUE),5),
               a50	=	 round(median(getRSSResultsComplete$a50,na.rm=TRUE),5),
               a90	=	 round(median(getRSSResultsComplete$a90,na.rm=TRUE),5),
               a9095	= round(median(getRSSResultsComplete$a9095,na.rm=TRUE),5),
               AIC	=	 round(median(getRSSResultsComplete$AIC),5),
               Iter	= round(median(getRSSResultsComplete$Iter),5),
               Converged	= round(median(getRSSResultsComplete$Converged),5),
               Separated	= round(median(getRSSResultsComplete$Separated),5),
               Uneven	= round(median(getRSSResultsComplete$Uneven),5),
               Uneven_lower= round(median(getRSSResultsComplete$Uneven_lower),5),
               Overlap	= round(median(getRSSResultsComplete$Overlap),5),
               Insp	= round(median(getRSSResultsComplete$Insp),5),
               N	=	 round(median(getRSSResultsComplete$N),5),
               Model	="RSS",
               CI = "Std. Wald")
  
  return(list(getMedianRSSResult, getMedianRSSResultComplete))
}
