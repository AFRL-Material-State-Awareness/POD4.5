#this class is used if the original input is 2D array
class RSS2DArrayGenerator():
    def __init__(self, dataset, m, sortedRows, usingR):
        self.dataset=dataset
        self.sortedRows=sortedRows
        self.m=m
        self.__results2D=None
        self.usingR=usingR
    def genRSSamples(self):
        #get the diagnals for each row set in a given cycle
        arrayOfDiagonals=[]
        currDiagonal=[]
        #resets for given cycle
        currRow=1
        #increment the index for each row
        grabIndex=0
        for i in self.sortedRows:
            if self.usingR==True:
                currDiagonal.append(self.dataset[i[grabIndex]-1])
            else:
                currDiagonal.append(self.dataset[i[grabIndex]])
             #reset when at end of cycle
            if(currRow%(self.m)==0):
                arrayOfDiagonals.append(currDiagonal)
                currDiagonal=[]
                currRow=1
                grabIndex=0
                continue
            grabIndex+=1
            currRow+=1
        self.__results2D=arrayOfDiagonals
    @property
    def Results2D(self):
        return self.__results2D
    @Results2D.setter
    def Results2D(self, value):
        self.__results2D = value
