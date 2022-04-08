from decimal import *
import os

class FileLogger():
    """description of class"""

    @staticmethod
    def Log_Start(file):

        if file is None or len(file) == 0:
            file = 'C:/Users/Public/Documents/UDRI/PODv4/Logs/log.txt'

        f = open(file, 'w')

        f.close()

    @staticmethod
    def Log_Msg(file, name, msg, func):

        if file is None or len(file) == 0:
            file = 'C:/Users/Public/Documents/UDRI/PODv4/Logs/log.txt'

        #directory = os.path.dirname(file)

        #FileLogger.make_sure_path_exists(directory)

        try:        
            f = open(file, 'a')
        except OSError as exception:
            f = open(file, 'w')

        line = 'Name: {0}, Message: {1}, Function: {2}\n'.format(name, msg, func)
        
        f.write(line)

        f.close()
        
    #@staticmethod
    #def make_sure_path_exists(path):
    #    try:
    #        os.makedirs(path)
    #    except OSError as exception:
    #        if exception.errno != errno.EEXIST:
    #            raise