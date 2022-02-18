import clr




class CompileDLL(object):
    """description of class"""
    def __init__(self):
        self._modules = []

    def Compile(self):

        pythonFolder = "C:\\Program Files (x86)\\IronPython 2.7\\Lib\\"

        self._modules.Add(["", "CPodDoc"])
        self._modules.Add(["", "linreg"])
        self._modules.Add(["", "mdnord"])
        self._modules.Add(["", "sysolv"])        
        self._modules.Add(["", "fcn"])
        self._modules.Add(["", "leqslv"])
        self._modules.Add(["", "funcr"])
        self._modules.Add(["", "qsort"])
        self._modules.Add(["", "phinv"])
        self._modules.Add(["", "nrmden"])
        self._modules.Add(["", "alnorm"])
        self._modules.Add(["", "PODaccessories"])
        self._modules.Add(["", "PODglobals"])
        self._modules.Add(["", "smtxinv"])
        self._modules.Add([pythonFolder, "__future__"])
        self._modules.Add([pythonFolder, "random"])
        self._modules.Add([pythonFolder, "warnings"])
        self._modules.Add([pythonFolder, "linecache"])
        self._modules.Add([pythonFolder, "os"])
        self._modules.Add([pythonFolder, "ntpath"])
        self._modules.Add([pythonFolder, "copy"])
        self._modules.Add([pythonFolder, "types"])
        self._modules.Add([pythonFolder, "weakref"])
        self._modules.Add([pythonFolder, "UserDict"])
        self._modules.Add([pythonFolder, "_abcoll"])
        self._modules.Add([pythonFolder, "abc"])
        self._modules.Add([pythonFolder, "_weakrefset"])
        self._modules.Add([pythonFolder, "collections"])
        self._modules.Add([pythonFolder, "keyword"])
        self._modules.Add([pythonFolder, "heapq"])
        self._modules.Add([pythonFolder, "bisect"])

        for file in self._modules:
            clr.CompileModules("..\\POD v4\\bin\\Debug\\" + file[1] + ".dll", file[0] + file[1] + ".py")
            clr.CompileModules("..\\PythonDLLs\\" + file[1] + ".dll", file[0] + file[1] + ".py")

if __name__ == '__main__':

    comp = CompileDLL()

    comp.Compile()


