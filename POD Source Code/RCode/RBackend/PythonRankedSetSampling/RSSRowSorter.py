#this class is designed to sort the randomly generated subsets for all 'm' cycles
import numpy as np
class RSSRowSorter():
    def __init__(self, rankedSetRows):
        self.__rankedSetRows=rankedSetRows
        self.__sortedRows=None
    def sortRowsFunction(self):
        #sort all the randomly sampled indices
        rowsSet=self.__rankedSetRows
        for i in range(0, len(rowsSet)):
            rowsSet[i]=np.sort(rowsSet[i])
        self.SortedRows=rowsSet
    @property
    def SortedRows(self):
        return self.__sortedRows
    @SortedRows.setter
    def SortedRows(self, value):
        self.__sortedRows = value
