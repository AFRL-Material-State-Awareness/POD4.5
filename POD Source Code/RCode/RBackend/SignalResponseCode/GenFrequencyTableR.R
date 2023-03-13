#     Probability of Detection Version 4.5 (PODv4.5)
#     Copyright (C) 2022  University of Dayton Research Institute (UDRI)
# 
#     This program is free software: you can redistribute it and/or modify
#     it under the terms of the GNU General Public License as published by
#     the Free Software Foundation, either version 3 of the License, or
#     (at your option) any later version.
# 
#     This program is distributed in the hope that it will be useful,
#     but WITHOUT ANY WARRANTY; without even the implied warranty of
#     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
#     GNU General Public License for more details.
# 
#     You should have received a copy of the GNU General Public License
#     along with this program.  If not, see <https://www.gnu.org/licenses/>.

# parameters:
# Responses= the transformed responses for the given dataset
# responseMin = the minimum transformed response
# responseMax = the maximum transformed response
# frequency table = table containing the frequencies of responses at various ranges (this gets returned to the UI)

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
                                        GenFrequencyTable=function(override=1){
                                          if(responsesMax > 0){
                                            maxTenVal<-RoundUpNice(responsesMax)
                                          }else{
                                            maxTenVal<-RoundUpNiceNeg(responsesMax)
                                          }
                                          incrementSeq<-c()
                                          incrementSeq<-DetermineIncrement(maxTenVal, override)
                                          outputTable<-as.data.frame(table(cut(responses$y, breaks=incrementSeq)))

                                          tempArray<-TrimEndZeros(outputTable$Freq)
                                          #find the first index in temp array that isn't zero
                                          firstIndex=(which(outputTable$Freq != 0)[1])
                                          outputTable<-data.frame(
                                            Ranges=outputTable$Var1[firstIndex:(firstIndex+length(tempArray)-1)],
                                            Freq=tempArray
                                          )
                                          if(length(incrementSeq)>0){
                                            finalOutput=data.frame(
                                              Range=incrementSeq[(firstIndex+1):(firstIndex+(length(tempArray)))],
                                              Freq=outputTable$Freq
                                            )
                                            cat(nrow(finalOutput))
                                            # The 2^15 is the max number of iterations for the normal curve
                                            # if there aren't 10 bars after 15 iterations, give up
                                            if(nrow(finalOutput) < 10 && override < 2^15){
                                              override=override*2
                                              GenFrequencyTable(override)
                                            }
                                            else{
                                              setFreqTable(finalOutput)
                                            }
                                          }else{
                                            stop("sequence was unable to generate values in noramlity plot")
                                          }
                                          
                                        },
                                        DetermineIncrement=function(maxTenVal, override){
                                          if(maxTenVal > 0){
                                            incrementSeq<- seq(from=RoundDownNice(responsesMin), to = maxTenVal, length.out = SturgesRule()*override)
                                          }else{
                                            incrementSeq<- seq(from= RoundUpNice(-responsesMin)*(-1), to = maxTenVal, length.out = SturgesRule()*override)
                                          }
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
                                        RoundDownNice = function(x, nice=c(10,8,6,5,4,2,1)) {
                                          if(length(x) != 1){ stop("'x' must be of length 1")}
                                          (10^floor(log10(x)) * nice[[which(x > 10^floor(log10(x)) * nice)[[1]]]])
                                        },
                                        TrimEndZeros=function(array){
                                          return(array[min(which(array != 0 )): max( which(array != 0 ))])
                                        }
                                      )
)