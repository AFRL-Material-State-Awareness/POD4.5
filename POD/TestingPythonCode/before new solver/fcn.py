
'''/* fcn.cpp
*
*/
#include "stdafx.h"
#include <math.h>
#include "matrix.h"
#include "PodDoc.h"
'''

from mdnord import mdnord
from math import pi, sqrt, exp,  log,  fabs
from nrmden import nrmden
#from scipy import stats


'''
// defined in nrmden.cpp
double nrmden(double z)
'''


def ahat_fcn(data):
    ierr = 0
    rt2pi = sqrt(2.0 * pi)
    y, xt, yf, z, q, tem, temm, tem2 = 0, 0, 0, 0, 0, 0, 0, 0
    acount = data.acount
    bcount = data.bcount
    npts = data.npts
    rlonx = data.rlonx
    rlony = data.rlony
    #print("pf_fcn", acount, bcount, npts, rt2pi)

    i, iacnt, ibcnt = 0, 0, 0
    data.res_sums[0] = data.res_sums[1] = 0.0
    data.res_sums[2] = -npts * data.linfit['rms'] * data.linfit['rms']
    #print("f: "+str(f))
    data.variance[0][0] = npts
    data.variance[0][1] = data.variance[0][2] = 0.0
    data.variance[1][1] = data.variance[1][2] = 0.0
    data.variance[2][2] = -npts
    #print("variance: "+str(variance))
    
    #for i in range(len(rlonx)):
    #    print(rlonx[i],rlony[i] )
    for i in range(npts):  # look at the first "npts" points
        xt = rlonx[i]        # xt is next value of x
        # <fit equation>  y = intercept + slope * x
        yf = rlony[i]
        y = (yf - data.linfit['intercept'] - data.linfit['slope'] * xt)
        # y is diff between fit and reading
        data.res_sums[0] += y  # sum of residual between fit and reading
        data.res_sums[1] += y * xt  # sum of residual * x
        data.res_sums[2] += y * y   # sum of residual squared
        #print(y)
        y /= data.linfit['rms']  # residual = residual / curve fit RMS
        # (normalized Residual)
        #print(y)
        data.variance[0][1] += xt
        data.variance[0][2] += 2.0 * y
        data.variance[1][1] += xt * xt
        data.variance[1][2] += 2.0 * y * xt
        data.variance[2][2] += 3.0 * y * y
    #print(variance[0])
    #print(variance[1])
    #print(variance[2])

    data.res_sums[2] = data.res_sums[2] / data.linfit['rms']
    iacnt = ibcnt = 0
    for i in range(acount):  # look at the next "acount" points
        xt = rlonx[i + npts]  # xt is next x point
        yf = rlony[i + npts] - data.linfit['intercept'] - data.linfit['slope'] * xt
        # yf is residual
        y = yf / data.linfit['rms']  # y is normalized residual
        z = exp(-y * y / 2.0) / rt2pi  # z is (e^-resid^2/2)/sqrt(2*pi)
        q = mdnord(y)  # q is cumulative NDF of norm residual
        q = 1.0 - q  # take 1 - probability
        if (q == 0.0):  # if no probability at all, skip this point
            iacnt += 1  # count skipped points
            continue
        tem = z / q  # tem = (e^-resid^2)/sqrt2*pi)/CDF(norm resid)
        data.res_sums[0] += tem * data.linfit['rms']
        data.res_sums[1] += tem * xt * data.linfit['rms']
        data.res_sums[2] += tem * yf
        temm = (tem - y) * tem
        tem2 = y * temm + tem
        data.variance[0][0] += temm
        data.variance[0][1] += temm * xt
        data.variance[0][2] += tem2
        data.variance[1][1] += temm * xt * xt
        data.variance[1][2] += tem2 * xt
        data.variance[2][2] += y * (tem2 + tem)

    #print(variance[0])
    #print(variance[1])
    #print(variance[2])

    for i in range(bcount):
        xt = rlonx[npts + acount + i]
        yf = (rlony[npts + acount + i]
            - data.linfit['intercept'] - data.linfit['slope'] * xt)
        y = yf / data.linfit['rms']
        z = exp(-y * y / 2.0) / rt2pi
        q = mdnord(y)
        if (q == 0.0):
            ibcnt += 1
            continue
        tem = z / q
        data.res_sums[0] -= tem * data.linfit['rms']
        data.res_sums[1] -= tem * xt * data.linfit['rms']
        data.res_sums[2] -= tem * yf
        temm = (tem + y) * tem
        tem2 = y * temm - tem
        data.variance[0][0] += temm
        data.variance[0][1] += temm * xt
        data.variance[0][2] += tem2
        data.variance[1][1] += temm * xt * xt
        data.variance[1][2] += tem2 * xt
        data.variance[2][2] += y * (tem2 - tem)
        
    data.variance[1][0] = -data.variance[0][1];
    data.variance[2][0] = -data.variance[0][2];
    data.variance[2][1] = -data.variance[1][2];
    data.variance[0][0] = -data.variance[0][0];
    data.variance[1][1] = -data.variance[1][1];
    data.variance[2][2] = -data.variance[2][2];
    data.variance[0][1] = -data.variance[0][1];
    data.variance[0][2] = -data.variance[0][2];
    data.variance[1][2] = -data.variance[1][2];

    if (iacnt + ibcnt != 0):
        ierr = 10000 + 100 * iacnt + ibcnt
    return ierr


def pf_fcn(x,  f,  data):
    #x[0] = muhat
    #x[1] = sighat
    #f = res_sums
    f[0],  f[1] = 0.0,  0.0
    for pfd in data.pfdata:
        z = (pfd.crkf-x[0])/x[1]
        pod = mdnord(z)
        temp1 = pod*(1.0-pod)
        temp2 = pfd.count * pod - pfd.above
        if (temp1>0.0):
            temp2 *= nrmden(z)/temp1
        else: 
            temp2  *= fabs(z)
        f[0] += temp2;
        f[1] += temp2*z;
    f[0] /= x[1]
    f[1] /= x[1]

def pod_odds(z):
    if (z < -80.0):
        return 0.0
    if (z > 80.0):
        return 1.0
    ht = h(z)
    return ht / (1.0 + ht)


def odds_inv(p):
    return log(p / (1.0 - p)) / (pi / sqrt(3.0))


def odds_fcn(x, f,  data):
    fct = pi/sqrt(3.0)
    f[0] = 0.0 
    f[1] = 0.0
    for pfd in data.pfdata:
        z = (pfd.crkf-x[0])/x[1]
        pod = pod_odds(z)
        temp1 =  pfd.count * pod - pfd.above
        f[0] += temp1
        f[1] += temp1*z
    temp1 = fct/x[1]
    f[0] *= temp1
    f[1] *= temp1

if __name__ == '__main__':
    npts = 5
    acount = 5
    bcount = 5

    x = [1, 1.5, .1]
    print("x: " + str(x))
    f = [4, 5, 6]
    print ("f: " + str(f))
    variance = [[7, 8, 9], [10, 11, 12], [13, 14, 15]]
    print("variance: " + str(variance))
    print("variance[0]: " + str(variance[0]))
    #print("pf_fcn(x, f, variance, (acount,bcount,npts,rlonx,rlony))"
    #    + str(pf_fcn(x, f, variance, (acount, bcount, npts, rlonx, rlony))))
    print("x: " + str(x))
    print ("f: " + str(f))
    print("variance[0]: " + str(variance[0]))
    print("variance[1]: " + str(variance[1]))
    print("variance[2]: " + str(variance[2]))

    print("h(0.01): " + str(h(0.01)))
    print("h(0.25): " + str(h(0.25)))
    print("h(0.5): " + str(h(0.5)))
    print("h(1.0): " + str(h(1.0)))
    print("h(1.5): " + str(h(1.5)))
    print("h(15): " + str(h(15)))
