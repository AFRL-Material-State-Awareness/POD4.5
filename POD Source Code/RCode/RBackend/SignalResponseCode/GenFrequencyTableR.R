
GenerateNormalityTable <- setRefClass("GenerateNormalityTable", fields = list(responses="data.frame",
                                                                    responsesMin="numeric",
                                                                    responsesMax="numeric",
                                                                    frequencyTable="data.frame"),
                                      methods = list(
                                        setFreqTable=function(psFreqTable){
                                          frequencyTable<<-psFreqTable
                                        },
                                        getFreqTable=function(){
                                          return(frequencyTable)
                                        },
                                        GenFrequencyTable=function(){
                                          if(responsesMax > 0){
                                            maxTenVal<-RoundUpNice(responsesMax)
                                          }
                                          else{
                                            maxTenVal<-RoundUpNiceNeg(responsesMax)
                                          }
                                          incrementSeq<-c()
                                          incrementSeq<-DetermineIncrement(maxTenVal)
                                          outputTable<-as.data.frame(table(cut(responses$y, breaks=incrementSeq)))

                                          tempArray<-TrimEndZeros(outputTable$Freq)
                                          
                                          outputTable<-data.frame(
                                            Ranges=outputTable$Var1[1:length(tempArray)],
                                            Freq=tempArray
                                          )
                                          if(length(incrementSeq)>0){
                                            finalOutput=data.frame(
                                              Range=incrementSeq[2:(length(tempArray)+1)],
                                              Freq=outputTable$Freq
                                            )
                                          }else{
                                            stop("sequence was unable to generate values in noramlity plot")
                                          }
                                          setFreqTable(finalOutput)
                                        }
                                        DetermineIncrement=function(maxTenVal){
                                          incrementSeq<- seq(from=0, to = maxTenVal, length.out = SturgesRule())
                                          return(incrementSeq)
                                        },
                                        #this function applies sturges rule to determine the number of bins 
                                        #necessary for the histogram
                                        #source: https://www.statology.org/sturges-rule/
                                        SturgesRule=function(){
                                          #sturges rule using log base 2
                                          N=length(responses$y)
                                          return(ceiling(1+3.322*log(N, base = 2)))
                                        },
                                        GetBestIncrement=function(tensMax){
                                          if(tensMax >=1){
                                            return((nchar(as.character(tensMax))-1)*10)
                                          }
                                          else{
                                            return(((nchar(as.character(tensMax))-1)*nchar(as.character(tensMax))))
                                          }
                                        },
                                        # source : https://stackoverflow.com/questions/6461209/how-to-round-up-to-the-nearest-10-or-100-or-x
                                        # author : Tommy
                                        # author URL:  https://stackoverflow.com/users/662787/tommy
                                        RoundUpNice = function(x, nice=c(1,2,4,5,6,8,10)) {
                                          if(length(x) != 1) stop("'x' must be of length 1")
                                          10^floor(log10(x)) * nice[[which(x <= 10^floor(log10(x)) * nice)[[1]]]]
                                        },
                                        #use this one to round negative numbers nicely
                                        RoundUpNiceNeg = function(x, nice=c(10,8,6,5,4,2,1)) {
                                          if(length(x) != 1){ stop("'x' must be of length 1")}
                                          (10^floor(log10(-x)) * nice[[which(-x > 10^floor(log10(-x)) * nice)[[1]]]])*(-1)
                                        },
                                        TrimEndZeros=function(array){
                                          return(array[min(which(array != 0 )): max( which(array != 0 ))])
                                        }
                                      )
)