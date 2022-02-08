# When running a ranked set sample, a subset of the data is chosen. 
# This function randomly samples the data and returns which indices should be 
# used. 
# Note that the same indices should be used for all inspectors, and this 
# function returns a data frame with maxResamples sets of random indices. 

getRSSindices=function(df,maxResamples){
  rssamp=RSS.fn(df)
  index=as.vector(rssamp$sample.y)
  
  for(resamples in 1:(maxResamples-1)){
    rssamp=RSS.fn(df)
    index=rbind(index,as.vector(rssamp$sample.y))
  }
  return(index)
}