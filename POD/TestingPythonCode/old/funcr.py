'''
int a_opt = 0;
int r_opt = 0;
int a_row, r_row;

CString funcstr(CString inp, int iopt, CString name);
double funcr(double v, int iopt, int row);
double funcinv(double v, int iopt, int row);
'''

from math import *
#import xlrd, xlwt

def IsCustom(doc):
    return (doc.a_opt==5 or doc.r_opt==5)

def a_str(doc):
    return funcstr("ahat",doc.r_opt,"uhat")

def r_str(doc):
    return funcstr("ahat",doc.r_opt,"uhat")

def a_fwd(v,row,iopt,doc):
    return funcr(v,row,iopt,doc)

def r_fwd(v,row,iopt,doc):
    return funcr(v,row,iopt,doc)
    
def a_inv(v,row,iopt,doc):
    return funcinv(v,row,iopt,doc)

def r_inv(v,row,iopt,doc):
    return funcinv(v,row,iopt,doc)

def funcstr(inp, iopt, name):
    f = ""
    if iopt == 1:
        f = inp
    elif iopt == 2:
        f = "ln("+inp+")"
    elif iopt == 3:
        f = "exp("+inp+")"
    elif iopt == 4:
        f = "1/"+inp
    elif iopt == 5:
        f = name+"("+inp+")"
    else:
        f = "UNKNOWN-"+inp
    return f

def funcr(v, row, iopt, doc):
    if iopt == 1:
        f = v
    elif iopt == 2:
        if v>0.0:
            f = log(v)
        else:
            f = -700.0
        #f = (v>0.0? log(v): -700.0)
    elif iopt == 3:
        f = exp(v)
    elif iopt == 4:
        f = 1.0/v
    elif iopt == 5:
        info = doc.podfile.sheet_by_name("Info")
        # insert values in column 3, read result in column 4
        
        xlValue(row,3,v)
        #vr = xlGetValue(row,4);
        #f = (V_VT(&vr)==VT_R8? V_R8(&vr): 0.0); 
        #break;
    else:  
        f = 0.0
    return f

def funcinv(v, iopt, row, doc):
    if iopt == 1:
        f = v
    elif iopt == 2:
        f = exp(v)
    elif iopt == 3:
        if v>0.0:
            f = log(v)
        else:
            f = -700.0
        #f = (v>0.0? log(v): -700.0);
    elif iopt == 4:
        f = 1.0/v
    else:
        f = 0.0
    return f
        
#Test each function
    
def testall(option,v,row,doc):
    
    
    print('a_opt = '+str(option))
    print('r_opt = '+str(option))
    print('v = '+str(v))
    
    print('IsCustom() '+str(IsCustom(doc)))
    
    print('a_str() '+str(a_str(doc)))
    print('a_fwd(v) '+str(a_fwd(v,row,option,doc)))
    print('a_inv(v) '+str(a_inv(v,row,option,doc)))
    
    print('r_str() '+str(r_str(doc)))
    print('r_fwd(v) '+str(r_fwd(v,row,option,doc)))   
    print('r_inv(v) '+str(r_inv(v,row,option,doc)))
    print('')
    
if __name__ == '__main__':

    from CPodDoc import CPodDoc
    doc = CPodDoc()
    
    testall(1,322.45,0,doc)
    testall(2,322.45,0,doc)
    testall(3,322.45,0,doc)
    testall(4,322.45,0,doc)
    testall(5,322.45,0,doc)
    print('None of the tests threw errors.')
    


