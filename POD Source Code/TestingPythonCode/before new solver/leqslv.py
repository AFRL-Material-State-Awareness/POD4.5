
'''
/* leqslv.cpp
*
*/
#include "stdafx.h"
#include <math.h>
#include "matrix.h"
#include "xlRoutines.h"

//extern void print(const char *fmt,...)
'''
from __future__ import division
# forces floating point instead of integer math
from math import fabs
import copy

def leqslv(data):  #rj= variance dd = residual_sums
    i, j, k, k2, n = 0, 0, 0, 0, 0
    imax = 0
    wtmp, wmax = 0.0, 0.0
    fact = 0.0
    pivot = 0.0
    wk = []
    n = len(data.variance)
    wk = copy.deepcopy(data.variance)

    #wk = data.variance
    
    data.deter = 1.0
    pivot = 1.0
    for k in range(n):
        k2 = k
        imax = k2
        wmax = wk[k2][k2]
        for i in range(k + 1, n):
            if (fabs(wk[i][k]) > fabs(wmax)):
                wmax = wk[i][k]
                imax = i
        if (wmax == 0.0):
            return -1  # singular matrix
        elif (imax != k):
            pivot = -pivot
            for i in range(0, n):
                wtmp = wk[k][i]
                wk[k][i] = wk[imax][i]
                wk[imax][i] = wtmp
            wtmp = data.res_sums[k2]
            data.res_sums[k2] = data.res_sums[imax]
            data.res_sums[imax] = wtmp
        for i in range(0, n):
            if (k2 != i and wk[i][k2] != 0.0):
                fact = wk[i][k2]
                for j in range(0, n):
                    wk[i][j] = wk[i][j] * wmax - fact * wk[k2][j]
                pivot *= wmax
                data.res_sums[i] = data.res_sums[i] * wmax - fact * data.res_sums[k2]
    data.deter = 1.0 / pivot
    for j in range(n):
        data.res_sums[j] /= wk[j][j]
        data.deter *= wk[j][j]


def tdleq(data,  res_sums):
    # in MFCv5 release mode, this routine would not work properly
    # until I set the following variables to static (instead of dynamic)
    # or added some debug code.
    a, b, c, d, e, f = 0, 0, 0, 0, 0, 0
    a = data.variance[0][0]
    b = data.variance[0][1]
    d = data.variance[1][0]
    e = data.variance[1][1]
    #print(jb)
    c = res_sums[0]
    f = res_sums[1]
    #print(dd)
    data.deter = (a * e) - (d * b)
    #print(det)
    if (fabs(data.deter) < 1e-10):
        print('Det ' + str(data.deter) + ' too low!')
        data.deter,  res_sums = 0.0, [0.0, 0.0]
        return res_sums
    #print(str(e)+' '+str(c)+' '+str(b)+' '+str(f))
    res_sums = [((e * c) - (b * f)) / data.deter, ((a * f) - (c * d)) / data.deter]
    return res_sums
    #print dd



