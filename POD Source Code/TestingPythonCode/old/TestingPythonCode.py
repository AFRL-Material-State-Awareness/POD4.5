import clr
clr.AddReference('System.Drawing')
clr.AddReference('System.Windows.Forms')

from System.Drawing import *
from System.Windows.Forms import *
from CPodDoc import CPodDoc

class MyForm(Form):
    def __init__(self):
        # Create child controls and initialize form
        pDoc = CPodDoc()

        #pDoc.podfile = xlrd.open_workbook('../example2.xls',on_demand=True)
    
        pDoc.GetInfo()
        pDoc.ParseInfo()
        pDoc.GetData()
        #pDoc.ahat_solve()
        #pDoc.ahat_censor()

        #print(pDoc.__class__)
        pdict = pDoc.__dict__
        plist = list(pdict)
        plist.sort()
        for key in plist : 
            if type(pdict[key]) == type([]) and len(pdict[key])>3 :
                print(key+" "+str(type(pdict[key]))+" "+str(pdict[key][:3])+ " ...")
            else : print(key+" "+str(type(pdict[key])))+" "+str(pdict[key])
        
        xldict = pDoc.podfile.sheet_by_name("Data").__dict__
        xllist = list(xldict)
        xllist.sort()
        for each in range(5): print ("")
        
        pass


Application.EnableVisualStyles()
Application.SetCompatibleTextRenderingDefault(False)

form = MyForm()
Application.Run(form)




