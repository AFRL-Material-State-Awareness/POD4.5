# RSS: Doing the bootstrap here ######
RSS.fn = function(df){
  m = 6
  r=floor(df$N[1]/m)
  rssamp=con.Mrss(df$x, df$index, m=m, r=r, type="r", concomitant=TRUE, sets=TRUE)
  return(rssamp)
}