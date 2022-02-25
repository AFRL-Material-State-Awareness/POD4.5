'''
/* leqslv.cpp
*
*/
#include "stdafx.h"
#include <math.h>
#include "matrix.h"
#include "xlRoutines.h"

//extern void print(const char *fmt,...);
'''
from __future__ import division #forces floating point instead of integer math
from math import fabs


def leqslv(rj, dd):
    i,j,k,k2,n = 0,0,0,0,0
    imax = 0
    wtmp, wmax = 0.0,0.0
    fact = 0.0
    deter, pivot = 0.0,0.0
    wk = []
    n = len(rj)
    wk = rj

    deter = pivot = 1.0
    for k in range(0,k):
        k2=k;
        imax=k2;
        wmax=wk[k2][k2];
        for i in range(k+1,n):
            if (fabs(wk[i][k]) > fabs(wmax)) :
                wmax = wk[i][k]
                imax = i
        if (wmax == 0.0): 
            return 0.0, [0.0 for each in range(n)]; # singular matrix
        elif (imax != k):
            pivot = -pivot;
            for i in range(0,n):
                wtmp = wk[k][i]
                wk[k][i] = wk[imax][i]
                wk[imax][i] = wtmp
            wtmp = dd[k2]
            dd[k2] = dd[imax]
            dd[imax] = wtmp
        for i in range(0,n):
            if (k2 != i and wk[i][k2] != 0.0):
                fact = wk[i][k2];
                for j in range(0,n):
                    wk[i][j] = wk[i][j]*wmax-fact*wk[k2][j];
                pivot *= wmax;
                dd[i] = dd[i]*wmax-fact*dd[k2];
    deter = 1.0/pivot;
    for j in range(0,n):
        dd[j] /= wk[j][j]
        deter *= wk[j][j]
    return deter, dd

def tdleq(jb, dd):
    # in MFCv5 release mode, this routine would not work properly
    # until I set the following variables to static (instead of dynamic)
    # or added some debug code.
    a, b, c, d, e, f, det = 0,0,0,0,0,0,0
    a = jb[0][0]
    b = jb[0][1]
    d = jb[1][0]
    e = jb[1][1]
    #print(jb)
    c = dd[0]
    f = dd[1]
    #print(dd)
    det = (a*e)-(d*b)
    #print(det)
    if (fabs(det)<1e-10):
        #print('Det too low!')
        return 0.0, [0.0, 0.0]
    #print(str(e)+' '+str(c)+' '+str(b)+' '+str(f))
    dd = [((e*c)-(b*f))/det,((a*f)-(c*d))/det]
    #print dd
    return det, dd;


