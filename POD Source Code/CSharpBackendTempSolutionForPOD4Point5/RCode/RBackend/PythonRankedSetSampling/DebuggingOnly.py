from MainRSSamplingClass import *
import pandas as pd
#############################################sample dataset: debugging only
indexList=[]
for i in range(0,30):
    indexList.append(i)
#k=sample subset size
set_k=6
#m=number of cycles(usually only needs to be 30)
set_m=5
testFlawList = [        0.153917693,
                        0.166910359,
                        0.181461883,
                        0.187918014,
                        0.191985489,
                        0.206296735,
                        0.22125183,
                        0.22254802,
                        0.223172984,
                        0.226625926,
                        0.234836897,
                        0.240048821,
                        0.245869667,
                        0.247247879,
                        0.24864422,
                        0.254521076,
                        0.259783658,
                        0.260924841,
                        0.270902464,
                        0.283500563,
                        0.284831232,
                        0.28762478,
                        0.291295773,
                        0.296634941,
                        0.308812298,
                        0.319062008,
                        0.322503578,
                        0.345716849,
                        0.356358449,
                        0.366939348
]
testReponses = [        0,
                        0,
                        0,
                        0,
                        0,
                        0,
                        1,
                        0,
                        0,
                        0,
                        0,
                        0,
                        0,
                        0,
                        1,
                        1,
                        0,
                        1,
                        1,
                        1,
                        1,
                        0,
                        1,
                        0,
                        1,
                        1,
                        1,
                        1,
                        1,
                        1
]
testData=[]
for i in range(0,30):
    testData.append([indexList[i], testFlawList[i], testReponses[i]])
df= pd.DataFrame(testData, columns=["Index", "x", "y"])

newGenSamples=RSSamplingMain(df, set_m, set_k)
newGenSamples.performRSS()
results=newGenSamples.RSSDataFrame
print(results)


