GetPrecision=function(myNum){
  if( is.na(myNum)){
    return(0)
  }
  myString=as.character(myNum)
  myStringArray=strsplit(myString, "\\.")
  return(nchar(myStringArray[[1]][2]))
}
GetMaxPrecision=function(mydataframe){
  max=-1
  for(i in 1:nrow(mydataframe)){
    currPrecision=GetPrecision(mydataframe[[2]][i])
    if(max < currPrecision){
      max=currPrecision
    }
  }
  return(max)
}