'''
/* mdnord.cpp
*
*   Cumulative normal distribution function.
*
*   AS 66 in APPL Statistics Algorithms.  Renamed to mimic
*   the IMSL routine.
*
*/
#include "stdafx.h"
#include <math.h>
'''
from math import sqrt, log, exp

def mdnord(x):
    A1 = 0.398942280444
    A2 = 0.399903438504
    A3 = 5.75885480458
    A4 = 29.8213557808
    A5 = 2.62433121679
    A6 = 48.6959930692
    A7 =  5.92885724438
    B1 = 0.398942280385
    B2 =  3.8052E-8
    B3 =  1.00000615302
    B4 =  3.98064794e-4
    B5 =  1.98615381364
    B6 =  0.151679116635
    B7 =  5.29330324926
    B8 =  4.8385912808
    B9 =  15.1508972451
    B10 =  0.742380924027
    B11 = 30.789933034
    B12 = 3.99019417011
    alnorm = 0
    y = 0
    z = 0
    upper = False
    n = 15.0
    ltone = (n+9.0)/3.0
    u = 1e-38
    utzero = sqrt(-2.0*(log(u)+1.0))-0.3
    up  = upper
   
    z = x
    if (z<0.0):
        up = not up
        z = -z
    if (z<=ltone or (up and z<=utzero)):
        y = 0.5*z*z
        if (z>1.28):
            alnorm = B1*exp(-y)/(z-B2+B3/(z+B4+B5/(z-B6+B7/(z+B8-B9/(z+B10+B11/(z+B12))))))
        else :
            alnorm = 0.5-z*(A1-A2*y/(y+A3-A4/(y+A5+A6/(y+A7))))
    else :
        alnomr = 0.0
    if (not up) :
        alnorm = 1.0 - alnorm
    return alnorm

if __name__ == '__main__':
    print('mdnord(23.2) '+str(mdnord(23.2)))
    print('mdnord(2.0) '+str(mdnord(2.0)))
    print('mdnord(1.0) '+str(mdnord(1.0)))
    print('mdnord(0.432) '+str(mdnord(0.432)))
    print('mdnord(0.00232) '+str(mdnord(0.00232)))
    print('mdnord(-23.2) '+str(mdnord(-23.2)))
