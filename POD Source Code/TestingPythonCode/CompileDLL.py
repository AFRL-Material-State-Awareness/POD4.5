import clr
import shutil
import os
import zipfile
from System import Array


class CompileDLL(object):
    """description of class"""
    def __init__(self):
        self._modules = []

    def ensure_dir(self, f):
        d = os.path.dirname(f)
        if not os.path.exists(d):
            os.makedirs(d)

    def Compile(self):

        pythonFolder = "..\\PythonEnvironment\\Lib\\"

        #if you run into a missing module error you probably need to add more to your compile modules list

        self._modules.Add(["", "FileLogger"])
        self._modules.Add(["", "pyevent"])
        self._modules.Add(["", "alogam"])
        self._modules.Add(["", "gammds"])
        self._modules.Add(["", "betain"]) #5

        self._modules.Add(["", "lookup_table"])
        self._modules.Add(["", "new_pf"])
        self._modules.Add(["", "CPodDoc"])
        self._modules.Add(["", "linreg"])
        self._modules.Add(["", "mdnord"]) #10

        self._modules.Add(["", "sysolv"])        
        self._modules.Add(["", "fcn"])
        self._modules.Add(["", "leqslv"])
        self._modules.Add(["", "funcr"])
        self._modules.Add(["", "qsort"]) #15

        self._modules.Add(["", "phinv"])
        self._modules.Add(["", "nrmden"])
        self._modules.Add(["", "alnorm"])
        self._modules.Add(["", "PODaccessories"])
        self._modules.Add(["", "PODglobals"]) #20

        self._modules.Add(["", "smtxinv"])
        self._modules.Add([pythonFolder, "numbers"])
        self._modules.Add([pythonFolder, "decimal"])
        self._modules.Add([pythonFolder, "__future__"])
        self._modules.Add([pythonFolder, "random"]) #25

        self._modules.Add([pythonFolder, "warnings"])
        self._modules.Add([pythonFolder, "linecache"])
        self._modules.Add([pythonFolder, "os"])
        self._modules.Add([pythonFolder, "ntpath"])
        self._modules.Add([pythonFolder, "copy"]) #30

        self._modules.Add([pythonFolder, "types"])
        self._modules.Add([pythonFolder, "weakref"])
        self._modules.Add([pythonFolder, "UserDict"])
        self._modules.Add([pythonFolder, "_abcoll"])
        self._modules.Add([pythonFolder, "abc"]) #35

        self._modules.Add([pythonFolder, "_weakrefset"])
        self._modules.Add([pythonFolder, "collections"])
        self._modules.Add([pythonFolder, "keyword"])
        self._modules.Add([pythonFolder, "heapq"])
        self._modules.Add([pythonFolder, "bisect"]) #40

        self._modules.Add([pythonFolder, "stat"])
        self._modules.Add([pythonFolder, "genericpath"])

        fileList = Array.CreateInstance(str, len(self._modules))

        i = 0
        for file in self._modules:
            fileList[i] = file[0] + file[1] + ".py"
            i = i + 1

        self.ensure_dir("..\\POD v4\\bin\\")
        self.ensure_dir("..\\POD v4\\bin\\Debug\\")
        self.ensure_dir("..\\POD v4\\bin\\Release\\")

        clr.CompileModules("..\\POD v4\\bin\\Debug\\" + "POD_All.dll", fileList[0], fileList[1], fileList[2], fileList[3], fileList[4], fileList[5], fileList[6], 
                            fileList[7], fileList[8], fileList[9], fileList[10], fileList[11], fileList[12], fileList[13], fileList[14], fileList[15], fileList[16], fileList[17], fileList[18],
                            fileList[19], fileList[20], fileList[21], fileList[22], fileList[23], fileList[24], fileList[25], fileList[26], fileList[15], fileList[27], fileList[28], fileList[29],
                            fileList[30], fileList[31], fileList[32], fileList[33], fileList[34], fileList[35], fileList[36], fileList[37], fileList[38], fileList[39], fileList[40], fileList[41])

        shutil.copyfile("..\\POD v4\\bin\\Debug\\POD_All.dll", "..\\PythonDLLs\\POD_All.dll")
        shutil.copyfile("..\\POD v4\\bin\\Debug\\POD_All.dll", "..\\POD v4\\bin\\Release\\POD_All.dll")

        #clr.CompileModules("..\\PythonDLLs\\" + "POD_All.dll", fileList[0], fileList[1], fileList[2], fileList[3], fileList[4], fileList[5], fileList[6], 
        #                    fileList[7], fileList[8], fileList[9], fileList[10], fileList[11], fileList[12], fileList[13], fileList[14], fileList[15], fileList[16], fileList[17], fileList[18],
        #                    fileList[19], fileList[20], fileList[21], fileList[22], fileList[23], fileList[24], fileList[25], fileList[26], fileList[15], fileList[27], fileList[28], fileList[29],
        #                    fileList[30], fileList[31], fileList[32], fileList[33], fileList[34], fileList[35], fileList[36], fileList[37], fileList[38])

        #self._modules = []

        #self._modules.Add([pythonFolder, "numbers"])
        #self._modules.Add([pythonFolder, "decimal"])
        #self._modules.Add([pythonFolder, "__future__"])
        #self._modules.Add([pythonFolder, "random"])
        #self._modules.Add([pythonFolder, "warnings"])
        #self._modules.Add([pythonFolder, "linecache"])
        #self._modules.Add([pythonFolder, "os"])
        #self._modules.Add([pythonFolder, "ntpath"])
        #self._modules.Add([pythonFolder, "copy"])
        #self._modules.Add([pythonFolder, "types"])
        #self._modules.Add([pythonFolder, "weakref"])
        #self._modules.Add([pythonFolder, "UserDict"])
        #self._modules.Add([pythonFolder, "_abcoll"])
        #self._modules.Add([pythonFolder, "abc"])
        #self._modules.Add([pythonFolder, "_weakrefset"])
        #self._modules.Add([pythonFolder, "collections"])
        #self._modules.Add([pythonFolder, "keyword"])
        #self._modules.Add([pythonFolder, "heapq"])
        #self._modules.Add([pythonFolder, "bisect"])

        #for file in self._modules:
        #    clr.CompileModules("..\\POD v4\\bin\\Debug\\" + file[1] + ".dll", file[0] + file[1] + ".py")
        #    clr.CompileModules("..\\PythonDLLs\\" + file[1] + ".dll", file[0] + file[1] + ".py")

if __name__ == '__main__':

    comp = CompileDLL()

    comp.Compile()


