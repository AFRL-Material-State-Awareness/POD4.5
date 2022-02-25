
'''/* fcn.cpp
*
*/
#include "stdafx.h"
#include <math.h>
#include "matrix.h"
#include "PodDoc.h"
'''

from mdnord import mdnord
from math import pi, sqrt, exp,  log,  fabs,  fsum
from nrmden import nrmden
from decimal import *


'''
// defined in nrmden.cpp
double nrmden(double z)
'''
def write_variance(variance, func, line):
    
    pass
    #f = open('C:/Temp/variance python.txt', 'a')

    #for i in range(len(variance)):
    #        for j in range(len(variance[i])):
    #            write_one_variance(variance, i, j, func, line)

    #f.close()

def write_msg(msg, func, line):
    
    pass
    
    #f = open('C:/Temp/variance python.txt', 'a')

    #f.write('{0}\n'.format(msg))

    #f.close()

def write_linfit(linfit, func, line):

    pass

    #f = open('C:/Temp/variance python.txt', 'a')

    #f.write('intercept = {0} slope = {1} rms = {2}\n'.format(Decimal(linfit['intercept']), Decimal(linfit['slope']), Decimal(linfit['rms'])))

    #f.close()

def write_one_value(label, value, func, line):

    pass
    #f = open('C:/Temp/variance python.txt', 'a')

    #f.write(func)
    #f.write('({0}): '.format(line))
    #f.write('{0}, {1}\n'.format(label, Decimal(value)))

    #f.close()
def write_one_variance(variance, r, c, func, line):
    
    pass
    #if func == 'ahat_print' and line == 2290:

    #    f = open('C:/Temp/variance python.txt', 'a')

    #    f.write(func)
    #    f.write('({0}): '.format(line))
    #    f.write('variance[{0}][{1}] = {2}\n'.format(r, c, Decimal(variance[r][c])))

    #    f.close()

def start_variance():

    pass
    #f = open('C:/Temp/variance python.txt', 'w')

    #f.close()

def write_fit_params(params, func, line):

    pass
    #f = open('C:/Temp/variance python.txt', 'a')
    
    #f.write(func)
    #f.write('({0}): '.format(line))
    #f.write('intercept = {0} slope = {1} rms = {2}\n'.format(Decimal(params['intercept']), Decimal(params['slope']), Decimal(params['rms'])))

    #f.close()

def write_ressum_params(params, func, line):

    pass
    #f = open('C:/Temp/variance python.txt', 'a')
    
    #f.write(func)
    #f.write('({0}): '.format(line))
    #f.write('res_sum_0 = {0} res_sum_1 = {1} res_sum_2 = {2}\n'.format(Decimal(params[0]), Decimal(params[1]), Decimal(params[2])))

    #f.close()

def write_list_params(list, listname, func, line):

    pass
    #f = open('C:/Temp/variance python.txt', 'a')
    
    #f.write(func)
    #f.write('({0}): '.format(line))

    #i = 0
    #for val in list:
    #    f.write('{0}[{1}] = {2} '.format(listname, i, Decimal(val)))
    #    i += 1

    #f.write('\n')
    
    #f.close()

def Write_Ordered_Dict(odict, func, line):

    pass
    #f = open('C:/Temp/variance python.txt', 'a')
    
    #f.write(func)
    #f.write('({0}): '.format(line))

    #firstName = odict.Keys[0]

    #for i in range(len(odict[firstName])):
        
    #    for name in odict.Keys:
    #        f.write('{0} '.format(Decimal(odict[name][i])))
    
    #    f.write('\n')
    
    #f.close()

def ahat_fcn(data):
    ierr = 0
    rt2pi = sqrt(2.0 * pi)
    y, xt, yf, z, q, tem, temm, tem2 = 0, 0, 0, 0, 0, 0, 0, 0
    acount = data.acount
    bcount = data.bcount
    npts = data.npts
    rlonx = data.rlonx
    rlony = data.rlony
    #yValues = []
    #yValues2 = []
    #xSum = 0.0
    #ySum = 0.0
    #variList00 = []
    #variList01 = []
    #variList02 = [] 
    #variList10 = []   
    #variList11 = []
    #variList12 = []
    #variList20 = []
    #variList21 = []
    #variList22 = []
    #resList0 = []
    #resList1 = []
    #resList2 = []
    #print("pf_fcn", acount, bcount, npts, rt2pi)

    i, iacnt, ibcnt = 0, 0, 0
    data.res_sums[0] = data.res_sums[1] = 0.0
    #resList0.append(data.res_sums[0])

    data.res_sums[2] = -npts * data.linfit['rms'] * data.linfit['rms']
    #resList2.append(data.res_sums[2])

    #print("f: "+str(f))
    data.variance[0][0] = npts
    write_one_variance(data.variance, 0, 0, 'ahat_fcn', 82)
    #variList00.append(data.variance[0][0])

    data.variance[0][1] = data.variance[0][2] = 0.0
    write_one_variance(data.variance, 0, 1, 'ahat_fcn', 86)
    write_one_variance(data.variance, 0, 2, 'ahat_fcn', 86)
    data.variance[1][1] = data.variance[1][2] = 0.0
    write_one_variance(data.variance, 1, 1, 'ahat_fcn', 89)
    write_one_variance(data.variance, 1, 2, 'ahat_fcn', 89)

    data.variance[2][2] = -npts
    write_one_variance(data.variance, 2, 2, 'ahat_fcn', 93)
    #variList22.append(data.variance[2][2])
    #print("variance: "+str(variance))
    
    write_fit_params(data.linfit, "ahat_fcn", 108);

    #for i in range(len(rlonx)):
    #    print(rlonx[i],rlony[i] )
    for i in range(npts):  # look at the first "npts" points
        xt = rlonx[i]        # xt is next value of x
        # <fit equation>  y = intercept + slope * x
        yf = rlony[i]
        y = (yf - data.linfit['intercept'] - data.linfit['slope'] * xt)
        #yValues.append(xt)
        #xSum += xt

        # y is diff between fit and reading
        data.res_sums[0] += y  # sum of residual between fit and reading
        #resList0.append(y)

        rTemp = y * xt  # sum of residual * x
        data.res_sums[1] += rTemp
        #resList1.append(rTemp)

        rTemp = y * y   # sum of residual squared
        data.res_sums[2] += rTemp
        #resList2.append(rTemp)
        #print(y)
        if data.linfit['rms'] != 0.0:
            y /= data.linfit['rms']  # residual = residual / curve fit RMS
        else:
            y = 0.0
        #yValues2.append(yf)
        #ySum += yf
        # (normalized Residual)
        #print(y)
        data.variance[0][1] += xt
        write_one_variance(data.variance, 0, 1, 'ahat_fcn', 132)
        #variList01.append(xt)

        vTemp = 2.0 * y
        data.variance[0][2] += vTemp
        write_one_variance(data.variance, 0, 2, 'ahat_fcn', 137)
        #variList02.append(vTemp)

        vTemp = xt * xt
        data.variance[1][1] += vTemp
        write_one_variance(data.variance, 1, 1, 'ahat_fcn', 142)
        #variList11.append(vTemp)

        vTemp = 2.0 * y * xt
        data.variance[1][2] += vTemp
        write_one_variance(data.variance, 1, 2, 'ahat_fcn', 147)
        #variList12.append(vTemp)

        vTemp = 3.0 * y * y
        data.variance[2][2] += vTemp
        write_one_variance(data.variance, 2, 2, 'ahat_fcn', 152)
        #variList22.append(vTemp)

    #print(variance[0])
    #print(variance[1])
    #print(variance[2])

    #data.res_sums[0] = fsum(resList0)
    #data.res_sums[1] = fsum(resList1)
    #data.res_sums[2] = fsum(resList2)

    #v00 = fsum(variList00)
    #v01 = fsum(variList01)
    #v02 = fsum(variList02)
    #v10 = fsum(variList10)  
    #v11 = fsum(variList11)
    #v12 = fsum(variList12)
    #v20 = fsum(variList20)
    #v21 = fsum(variList21)
    #v22 = fsum(variList22)

    #data.variance[0][0] = v00
    #data.variance[0][1] = v01
    #data.variance[0][2] = v02
    #data.variance[1][0] = v10
    #data.variance[1][1] = v11
    #data.variance[1][2] = v12
    #data.variance[2][0] = v20
    #data.variance[2][1] = v21
    #data.variance[2][2] = v22

    if data.linfit['rms'] != 0.0:
        data.res_sums[2] = data.res_sums[2] / data.linfit['rms']
    else:
        data.res_sums[2] = 0.0
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
        write_one_variance(data.variance, 0, 0, 'ahat_fcn', 203)
        data.variance[0][1] += temm * xt
        write_one_variance(data.variance, 0, 1, 'ahat_fcn', 205)
        data.variance[0][2] += tem2
        write_one_variance(data.variance, 0, 2, 'ahat_fcn', 207)
        data.variance[1][1] += temm * xt * xt
        write_one_variance(data.variance, 1, 1, 'ahat_fcn', 209)
        data.variance[1][2] += tem2 * xt
        write_one_variance(data.variance, 1, 2, 'ahat_fcn', 211)
        data.variance[2][2] += y * (tem2 + tem)
        write_one_variance(data.variance, 2, 2, 'ahat_fcn', 213)

    #print(variance[0])
    #print(variance[1])
    #print(variance[2])

    for i in range(bcount):
        xt = rlonx[npts + acount + i]
        yf = (rlony[npts + acount + i]
            - data.linfit['intercept'] - data.linfit['slope'] * xt)
        if data.linfit['rms'] != 0.0:
            y = yf / data.linfit['rms']
        else:
            y = 0.0
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
        write_one_variance(data.variance, 0, 0, 'ahat_fcn', 237)
        data.variance[0][1] += temm * xt
        write_one_variance(data.variance, 0, 1, 'ahat_fcn', 239)
        data.variance[0][2] += tem2
        write_one_variance(data.variance, 0, 2, 'ahat_fcn', 241)
        data.variance[1][1] += temm * xt * xt
        write_one_variance(data.variance, 1, 1, 'ahat_fcn', 243)
        data.variance[1][2] += tem2 * xt
        write_one_variance(data.variance, 1, 2, 'ahat_fcn', 245)
        data.variance[2][2] += y * (tem2 - tem)
        write_one_variance(data.variance, 2, 2, 'ahat_fcn', 247)
        
    data.variance[1][0] = -data.variance[0][1]
    write_one_variance(data.variance, 1, 0, 'ahat_fcn', 249)
    data.variance[2][0] = -data.variance[0][2]
    write_one_variance(data.variance, 2, 0, 'ahat_fcn', 251)
    data.variance[2][1] = -data.variance[1][2]
    write_one_variance(data.variance, 2, 1, 'ahat_fcn', 253)
    data.variance[0][0] = -data.variance[0][0]
    write_one_variance(data.variance, 0, 0, 'ahat_fcn', 255)
    data.variance[1][1] = -data.variance[1][1]
    write_one_variance(data.variance, 1, 1, 'ahat_fcn', 257)
    data.variance[2][2] = -data.variance[2][2]
    write_one_variance(data.variance, 2, 2, 'ahat_fcn', 259)
    data.variance[0][1] = -data.variance[0][1]
    write_one_variance(data.variance, 0, 1, 'ahat_fcn', 261)
    data.variance[0][2] = -data.variance[0][2]
    write_one_variance(data.variance, 0, 2, 'ahat_fcn', 263)
    data.variance[1][2] = -data.variance[1][2]
    write_one_variance(data.variance, 1, 2, 'ahat_fcn', 265)

    if (iacnt + ibcnt != 0):
        ierr = 10000 + 100 * iacnt + ibcnt

    #write_variance(data.variance, 'end of ahat_fcn')

    write_ressum_params(data.res_sums, "ahat_fcn FINAL", 289);

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


def h(z):

	return exp(pi/sqrt(3.0)*z);


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
