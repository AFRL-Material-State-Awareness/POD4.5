
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
                                          
                                          #maxTenVal<-round(responsesMax, digits = -1)
                                          maxTenVal<-RoundUpNice(responsesMax)
                                          #increment<-(maxTenVal)/5
                                          incrementSeq<-c()
                                          cat(GetBestIncrement(maxTenVal))
                                          cat('\n')
                                          cat(RoundUpNice(GetMedianDiff(responses$y)))
                                          cat('\n')
                                          if((responsesMin)>10){
                                            incrementSeq<-seq(from=0, to = maxTenVal, by = GetBestIncrement(maxTenVal))
                                          }else{
                                            incrementSeq<-seq(from=0, to = maxTenVal, by = RoundUpNice(GetMedianDiff(responses$y)))
                                          }
                                          outputTable<-as.data.frame(table(cut(responses$y, breaks=incrementSeq)))

                                          #names(outputTable)[names(outputTable) == 'Var1'] <- 'Ranges'
                                          tempArray<<-TrimEndZeros(outputTable$Freq)
                                          
                                          outputTable<-data.frame(
                                            Ranges=outputTable$Var1[1:length(tempArray)],
                                            Freq=tempArray
                                          )
                                          #return()
                                          if(length(incrementSeq)>0){
                                            finalOutput=data.frame(
                                              Range=incrementSeq[2:(length(tempArray)+1)],
                                              Freq=outputTable$Freq
                                            )
                                          }else{
                                            stop("sequence was unable to generate values in noramlity plot")
                                          }
                                          
                                          #outputTable<<-as.data.frame.matrix(table(cut(responses$y, breaks=seq(from=0, to = maxTenVal, by = GetBestIncrement(maxTenVal)))))
                                          setFreqTable(finalOutput)
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
                                        TrimEndZeros=function(array){
                                          return(array[min(which(array != 0 )): max( which(array != 0 ))])
                                        },
                                        GetMedianDiff=function(myList){
                                          diffs=c()
                                          for (i in 1:(length(myList)-1)){
                                            diffs=c(diffs, myList[i+1]-myList[i])
                                          }
                                          medianDiff=median(diffs)
                                          if(TRUE){
                                            medianDiff=max(diffs)
                                          }
                                          return(medianDiff)
                                        }
                                      )
)

#responsesCheck=data.frame(y=data_obs$A11)
#normalityCheck<-GenerateNormalityTable$new(responses=responsesCheck, responsesMin = min(responsesCheck$y), responsesMax = max(responsesCheck$y))
#normalityCheck$GenFrequencyTable()
#table(cut(responsesCheck$y,breaks=seq(from=0, to = responsesMax, by = increment)))
#outputTableGlobal<-normalityCheck$getFreqTable()
# GetMedianDiff=function(myList){
#   diffs=c()
#   for (i in 1:(length(myList)-1)){
#     diffs=c(diffs, myList[i+1]-myList[i])
#   }
#   medianDiff=median(diffs)
#   if(medianDiff==0){
#     medianDiff=max(diffs)
#   }
#   return(medianDiff)
# }
# responses=data_obs$y
# global=GetMedianDiff(responses)
