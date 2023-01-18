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
#     along with this program.  If not, see <https://www.gnu.org/licenses/>

# used to generate the ranked set samples in R (might replace with numpy and python later since it's faster)
# For additional information on ranked set sampling, see "A method for unbiased selective sampling, using ranked sets"
# by GA McIntyre (1952)

# parameters:
# testData = the original input dataframe to be used for Ranked set sampling
# set_r = the value for r when generating the ranked set samples
# set_m = the value for m when generating the ranked set samples
# rssDataFrame = the resulting ranked set sample dataframe (equivalent to 1 resample)
# oneCycle = one cycle in the ranked set sampling array
# fullRSSArray = the full ranked set sampling matrix generated to created a ranked set sample dataset

RSSamplingMain_R<-setRefClass("RSSamplingMain", fields=list(testData="data.frame",
                                                          set_r="numeric",
                                                          set_m="numeric",
                                                          rssDataFrame="data.frame",
                                                          oneCycle="numeric",
                                                          fullRSSArray="numeric"),
                            methods=list(
                            initialize=function(testDataInput=data.frame(matrix(ncol = 1, nrow = 1)),
                                                set_rInput=nrow(testDataInput),
                                                set_mInput=6,
                                                rssDataFrameInput=data.frame(matrix(ncol = 1, nrow = 1))){
                              testData<<-testDataInput
                              set_r<<- set_rInput
                              set_m<<- set_mInput
                              rssDataFrame<<-rssDataFrameInput
                              oneCycle<<-set_m*set_m
                              fullRSSArray<<-set_r*oneCycle
                            },
                            getRSSDAtaFrame=function(){
                              return(rssDataFrame)
                            },
                            setRSSDataFrame=function(psRSSDataFrame){
                              rssDataFrame<<-psRSSDataFrame
                            },
                            performRSS=function(){
                              rowsInCycles=cyclesArrayGenerator()
                              sortedRows=rssSorter(rowsInCycles)
                              genRSSSample(sortedRows)
                            },
                            cyclesArrayGenerator=function(){
                              fullSizeArray=sample(1:nrow(testData), fullRSSArray, replace=T)
                              setOfRows=list()
                              givenRow=c()
                              for(i in 1:length(fullSizeArray)){
                                givenRow=append(givenRow, fullSizeArray[i])
                                #print(i%%set_m)
                                if(i%%set_m == 0){
                                  setOfRows=append(setOfRows, list(givenRow))
                                  givenRow=c()
                                }
                              }
                              rowsInCycles=setOfRows
                              return(rowsInCycles)
                            },
                            rssSorter=function(rowsInCycles){
                              for(i in 1:length(rowsInCycles)){
                                rowsInCycles[[i]]=sort(rowsInCycles[[i]])
                              }
                              sortedRows=rowsInCycles
                              return(sortedRows)
                            },
                            genRSSSample=function(sortedRows){
                              arrayOfDiagonals=list()
                              currDiagonal=list()
                              #resets after each cycle
                              currRow=1
                              #increment index for each row
                              grabIndex=1
                              for(i in 1:length(sortedRows)){
                                currDiagonal=append(currDiagonal, list(testData[sortedRows[[i]][grabIndex],]))
                                if(currRow%%set_m==0){
                                  arrayOfDiagonals=append(arrayOfDiagonals, currDiagonal)
                                  currDiagonal=list()
                                  currRow=1
                                  grabIndex=1
                                  next
                                }
                                grabIndex=grabIndex+1
                                currRow=currRow+1
                              }
                              resultsDataFrame=arrayOfDiagonals[[1]]
                              for(i in 2:length(arrayOfDiagonals)){
                                resultsDataFrame=rbind(resultsDataFrame, arrayOfDiagonals[[i]])
                              }
                              setRSSDataFrame(resultsDataFrame)
                            }
                            ))
