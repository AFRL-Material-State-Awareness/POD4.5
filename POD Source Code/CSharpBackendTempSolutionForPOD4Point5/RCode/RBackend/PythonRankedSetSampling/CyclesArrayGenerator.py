#this class generates an array of random integers between 0 and N-1(N is sample size)
#the class also parses the RSSampling array getting m * k^2 (the length of one cycle must be equal
#to k for our RSS technique). 
import numpy as np
class CyclesArrayGenerator():
    def __init__(self, fullRankedSetArray, data, mInput):
        self.__fullArraySize=fullRankedSetArray
        self.__fullArray=None
        self.__parsedArray=None
        self.data=data
        self.m=mInput
    def genFullRSSArray(self):
        #original sample Size
        sampleSize=len(self.data)
        #generate a numpy array of random integers 
        fullArray=np.random.randint(0, sampleSize,self.__fullArraySize)
        self.FullCyclesArray=fullArray
    def parseArray(self):
        #append to a traditional python list is supposed to be faster than numpy
        setOfRows=[]
        givenRow=[]
        for i in range(1, len(self.__fullArray)+1):
            givenRow.append(self.__fullArray[i-1])
            if(i%self.m==0):
                setOfRows.append(givenRow)
                givenRow=[]
        self.RowsInCycles=setOfRows
    @property
    def FullCyclesArray(self):
        return self.__fullArray
    @FullCyclesArray.setter
    def FullCyclesArray(self, value):
        self.__fullArray = value
    @property
    def RowsInCycles(self):
        return self.__parsedArray
    @RowsInCycles.setter
    def RowsInCycles(self, value):
        self.__parsedArray = value
