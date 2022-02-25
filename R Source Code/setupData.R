
setupData <- function(df, folderLocation=FALSE){
  # Reorder data first from smallest to largest flaws (df$x), 
  # then order by miss/hit (df$y)
  df <- df[order(df$x,df$y),] 
  
  # Number of observations
  n = length(df$x)
  
  # Create a dataframe with the necessary values
    # x = flaw size (continuous)
    # y = hit/miss call (0 or 1)
    # Insp = inspector
    # N = number of observations
    # Uneven = Ratio of misses to hits (0 to 0.5)
    # Overlap = % of flaw sizes between the smallest hit and largest miss
  df_1_insp=data.frame(
    index   = linspace(1,n,n),
    y       =      df$y,
    x       =      df$x,
    Insp    =   df$insp,
    N       =   n,
    Uneven = (n-sum(df$y))/n,
    Overlap=length(df[df$x>=min(df$x[df$y==1]) & df$x<=max(df$x[df$y==0]),]$x)/n
  )
    return(df_1_insp)    
}

