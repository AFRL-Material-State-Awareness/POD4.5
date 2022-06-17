
'''
int a_opt = 0;
int r_opt = 0;
int a_row, r_row;

CString funcstr(CString inp, int transform, CString name);
double funcr(double value, int transform, int row);
double funcinv(double value, int transform, int row);
'''

from math import *
#import xlrd, xlwt

#def IsCustom(doc):
#    return (doc.a_opt==5 or doc.r_opt==5)

#def a_str(doc):
#    return funcstr("ahat",doc.r_opt,"uhat")

#def r_str(doc):
#    return funcstr("ahat",doc.r_opt,"uhat")


def a_fwd(value, transform):
    return funcr(value, transform)


def r_fwd(value, transform):
    return funcr(value, transform)


def a_inv(value, transform):
    return funcinv(value, transform)


def r_inv(value, transform):
    return funcinv(value, transform)


def funcstr(inp, transform, name):
    f = ""
    if transform == 1:
        f = inp
    elif transform == 2:
        f = "ln(" + inp + ")"
    elif transform == 3:
        f = "exp(" + inp + ")"
    elif transform == 4:
        f = "1/" + inp
    elif transform == 5:
        f = name + "(" + inp + ")"
    else:
        f = "UNKNOWN-" + inp
    return f


def funcr(value, transform):
    if transform == 1:
        f = value
    elif transform == 2:
        if value > 0.0:
            f = log(value)
        else:
            f = value #float(0.000000000001)
            #f = float('NaN')
        #f = (value>0.0? log(value): -700.0)
    elif transform == 3:
        try:
            f = exp(value)
        except(OverflowError):
            #print(str(value) + ' could not be transformed')
            f = float('NaN')
    elif transform == 4:
        if value != 0.0:
            f = 1.0 / value
        else:
            f = value
    #elif transform == 5:
        #info = doc.podfile.sheet_by_name("Info")
        # insert values in column 3, read result in column 4

        #xlValue(row,3,v)
        #vr = xlGetValue(row,4);
        #f = (V_VT(&vr)==VT_R8? V_R8(&vr): 0.0);
        #break;
    else:
        f = 0.0
    return f

#this is where the code performs transformations back to linear space (i.e. ln(x) to x
def funcinv(value, transform):
    if transform == 1:
        f = value
    elif transform == 2:
        try:
            f = exp(value)
        except(OverflowError):
            #print(str(value) + ' could not be inverse transformed')
            f = float('NaN')
    elif transform == 3:
        if value > 0.0:
            f = log(value)
        else:
            f = value #float(0.000000000001)
            #f = float('NaN')

        #f = (value>0.0? log(value): -700.0);
    elif transform == 4:
        if value != 0.0:
            f = 1.0 / value
        else:
            f = value
    else:
        f = 0.0
    return f

#Test each function

def testall(option, value):
    print('a_opt = ' + str(option))
    print('r_opt = ' + str(option))
    print('value = ' + str(value))

    #print('IsCustom() '+str(IsCustom(doc)))

    #print('a_str() '+str(a_str(doc)))
    print('a_fwd(value) ' + str(a_fwd(value, option)))
    print('a_inv(value) ' + str(a_inv(value, option)))

    #print('r_str() '+str(r_str(doc)))
    print('r_fwd(value) ' + str(r_fwd(value, option)))
    print('r_inv(value) ' + str(r_inv(value, option)))
    print('')

if __name__ == '__main__':

    #from CPodDoc import CPodDoc
    #doc = CPodDoc()

    testall(1, 322.45)
    testall(2, 322.45)
    testall(3, 322.45)
    testall(4, 322.45)
    testall(5, 322.45)
    print("None of the tests threw errors.")



