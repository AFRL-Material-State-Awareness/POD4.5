###This class is used to perform ranked set sampling and calculate the logit results
#import numpy as np
import pandas as pd
import time
#only used if running from python. R imports all the needed python scripts on its own
try:
    from RSSRowSorter import *
    from CyclesArrayGenerator import *
    #from RSSDataFrameGenerator import *
    from RSS2DArrayGenerator import *
except:
    pass
#Main RSSampling Class
class RSSamplingMain():
    def __init__(self, dataset, r, m, usingRIter=False):
        #allows the user to insert a pandas dataframe or a 2d array
        #use this argument if running from r(programming langauge) to fix the index offset
        self.usingRIter=usingRIter
        self.dataset=dataset.to_numpy()
        #number of cycles
        self.r=int(r)
        #random subset sample size
        self.m=int(m)
        #used to hold a list of the final dataframe samples
        self.__rssDataframe=None
        #lenCyclek, must be equal to the subset size, m, for current ranking procedure
        self.lenCycle=self.m
        #sample size for one cycle
        self.oneCycle=self.m*self.lenCycle
        #get size of the entire RSS array (cast as in if necessary)
        self.fullRSSArray=self.r*self.oneCycle
        #results array
        self.resultsRSSArray=None
    def performRSS(self):
        #timer for debugging
        #starttime=time.time()
        newArrayGenator=CyclesArrayGenerator(self.fullRSSArray, self.dataset, self.m)
        newArrayGenator.genFullRSSArray()
        newArrayGenator.parseArray()
        sortRows=newArrayGenator.RowsInCycles
        newRowSorter=RSSRowSorter(sortRows)
        newRowSorter.sortRowsFunction()
        new2DRSS=RSS2DArrayGenerator(self.dataset,self.m, newRowSorter.SortedRows, self.usingRIter)
        new2DRSS.genRSSamples()
        self.resultsRSSArray=new2DRSS.Results2D
        self.genDataFrame()
        ###debugging
        #executiontime=(time.time()-starttime)
        #print(str(executiontime))
        #print("exuted random generating time above")
    def genDataFrame(self):
        outputDataFrameList=[]
        for i in self.resultsRSSArray:
            outputDataFrameList=outputDataFrameList+i
        finalDF=pd.DataFrame(outputDataFrameList, columns=['Index', 'x', 'y'])
        self.RSSDataFrame=finalDF
    @property
    def RSSDataFrame(self):
        return self.__rssDataframe
    @RSSDataFrame.setter
    def RSSDataFrame(self, value):
        self.__rssDataframe = value





    
