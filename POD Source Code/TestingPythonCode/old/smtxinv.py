'''
/* smtxinv.cpp
*    3x3 symetric matrix inversion
*/
'''
from __future__ import division #forces floating point instead of integer math
from math import fabs

def smtxinv(x, xi):
    # double A,B,C,D,E,F;
    # double DETER;
    A = x[0][0]
    print ("A: "+str(A))
    B = x[0][1]

    C = x[0][2]
    D = x[1][1]
    E = x[1][2]
    F = x[2][2]
   
    DETER=A*D*F+2.0*B*E*C-B*B*F-C*C*D-E*E*A
    print ("DETER: "+str(DETER))
   
    if (fabs(DETER)<1e-14):
       return 0.0, None # zero determinant

    xi[0][0]=(D*F-E*E)/DETER
    xi[0][1]=(C*E-B*F)/DETER
    xi[0][2]=(B*E-C*D)/DETER
    xi[1][1]=(A*F-C*C)/DETER
    xi[1][2]=(B*C-A*E)/DETER
    xi[2][2]=(A*D-B*B)/DETER
    xi[1][0]=xi[0][1]
    xi[2][0]=xi[0][2]
    xi[2][1]=xi[1][2]

    return DETER, xi

if __name__ == '__main__':
    x = [[2,2,3],[4,5,6],[7,8,9]]
    print ("x: "+ str(x))

    xi = [[2,2,3],[4,5,6],[7,8,9]]
    print ("xi: "+ str(xi))

    deter, xi = smtxinv(x, xi)

    print ("x = [[1,2,3],[4,5,6],[7,8,9]]")
    print ("deter, xi = smtxinv(x)")
    print ("deter: " + str(deter))
    print ("xi: " + str(xi))

