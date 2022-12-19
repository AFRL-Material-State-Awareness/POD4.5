#used to generate the ranked set samples in R (might replace with numpy and python later since it's faster)
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
