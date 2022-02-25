'''/* fcn.cpp
*
*/
#include "stdafx.h"
#include <math.h>
#include "matrix.h"
#include "PodDoc.h"
'''

from mdnord import mdnord
from math import pi, sqrt, exp


'''
// defined in nrmden.cpp
double nrmden(double z);
'''


def ahat_fcn(x, f, jb, pDoc):
    ierr = 0
    acount = pDoc.acount
    bcount = pDoc.bcount
    npts = pDoc.npts
    
    rt2pi = sqrt(2.0*pi)
    y,xt,yf,z,q,tem,temm,tem2 = 0,0,0,0,0,0,0,0
    #print(acount,bcount,npts,rt2pi)

    i, iacnt, ibcnt = 0,0,0
    f[0] = f[1] = 0.0
    f[2] = -npts*x[2]*x[2]
    #print("f: "+str(f))
    jb[0][0] = npts;
    jb[0][1] = jb[0][2] = 0.0;
    jb[1][1] = jb[1][2] = 0.0;
    jb[2][2] = -npts;
    #print("jb: "+str(jb))
    
    rlonx = pDoc.rlonx
    rlony = pDoc.rlony
    xy = []
    #for i in range(len(rlonx)):
    #    print(rlonx[i],rlony[i] )
    for i in range(npts) :  #look at the first "npts" points
        xt = rlonx[i];        #xt is next value of x 
        #<fit equation>  y = intercept + slope * x
        y = (rlony[i] - x[0] - x[1] * xt);  #y is diff between fit and reading
        f[0] += y;  #sum of residual between fit and reading
        f[1] += y * xt;  #sum of residual * x
        f[2] += y * y;   #sum of residual squared
        #print(y)
        y /= x[2];     #residual = residual / curve fit RMS  (normalized Residual)
        #print(y)
        jb[0][1] += xt;
        jb[0][2] += 2.0*y;
        jb[1][1] += xt*xt;
        jb[1][2] += 2.0*y*xt;
        jb[2][2] += 3.0*y*y;
    #print(jb[0])
    #print(jb[1])
    #print(jb[2])
    
    f[2] = f[2]/x[2]
    iacnt = ibcnt = 0;
    for i in range(acount):  #look at the next "acount" points
        xt = rlonx[i+npts];  #xt is next x point
        yf = rlony[i+npts] - x[0] - x[1] * xt;  #yf is residual
        y = yf / x[2];  #y is normalized residual
        z = exp(-y*y/2.0)/rt2pi;  # z is (e^-resid^2/2)/sqrt(2*pi)
        q = mdnord(y);  #q is cumulative NDF of norm residual
        q = 1.0 - q;  #take 1 - probability
        if (q == 0.0) :  # if no probability at all, skip this point
            iacnt+=1; # count skipped points
            continue;
        tem = z / q;  #tem = (e^-resid^2)/sqrt2*pi)/CDF(norm resid)
        f[0] += tem*x[2];  
        f[1] += tem*xt*x[2];
        f[2] += tem*yf;
        temm = (tem-y)*tem;
        tem2 = y*temm+tem;
        jb[0][0] += temm;
        jb[0][1] += temm*xt;
        jb[0][2] += tem2;
        jb[1][1] += temm*xt*xt;
        jb[1][2] += tem2*xt;
        jb[2][2] += y * (tem2+tem);
    
    #print(jb[0])
    #print(jb[1])
    #print(jb[2])
    
    for i in range(bcount) :
        xt = rlonx[npts+acount+i];
        yf = rlony[npts+acount+i] - x[0] - x[1] * xt;
        y  = yf / x[2];
        z  = exp(-y * y / 2.0) / rt2pi;
        q = mdnord(y);
        if (q==0.0) :
            ibcnt += 1;
            continue;
        tem = z / q;
        f[0] -= tem * x[2];
        f[1] -= tem * xt * x[2];
        f[2] -= tem * yf;
        temm = (tem + y) * tem;
        tem2 = y * temm - tem;
        jb[0][0] += temm;
        jb[0][1] += temm *xt;
        jb[0][2] += tem2;
        jb[1][1] += temm * xt *xt;
        jb[1][2] += tem2 * xt;
        jb[2][2] += y*(tem2-tem);
    #print(jb[0])
    #print(jb[1])
    #print(jb[2])
    if (iacnt+ibcnt !=0): ierr = 10000+100*iacnt+ibcnt;
    return ierr;
    
def pf_fcn(x,f):
    return -1;
'''
void pf_fcn(double *x, double *f)
{
	int i;
	double z, pod, temp1, temp2;
	PFData *pfd = pDoc->pfdata;
	int npts = pDoc->npts;
	f[0] = f[1] = 0.0;
	for (i=0; i<npts; i++, pfd++) {
		z = (pfd->crkf-x[0])/x[1];
		pod = mdnord(z);
		temp1 = pod*(1.0-pod);
		temp2 = pfd->count*pod - pfd->above;
		if (temp1>0.0) temp2 *= nrmden(z)/temp1;
		else temp2  *= fabs(z);
		f[0] += temp2;
		f[1] += temp2*z;
	}
	f[0] /= x[1];
	f[1] /= x[1];
}
'''
def h(z):
    return exp(pi/sqrt(3.0)*z)

def pod_odds(z):
    if (z<-80.0) : return 0.0
    if (z>80.0) : return 1.0
    ht = h(z)
    return ht/(1.0+ht)

def odds_inv(p): return log(p/(1.0-p))/(pi/sqrt(3.0))

def odds_fcn(x,f):
    return -1
'''
void odds_fcn(double *x, double *f)
{
	int i;
	const double pi = 4.0*atan(1.0);
	const double fct = pi/sqrt(3.0);
	double z, pod, temp1;
	PFData *pfd = pDoc->pfdata;
	int npts = pDoc->npts;
	f[0] = f[1] = 0.0;
	for (i=0; i<npts; i++, pfd++) {
		z = (pfd->crkf-x[0])/x[1];
		pod = pod_odds(z);
		temp1 =  pfd->count*pod - pfd->above;
		f[0] += temp1;
		f[1] += temp1*z;
	}
	temp1 = fct/x[1];
	f[0] *= temp1;
	f[1] *= temp1;
}
'''

if __name__ == '__main__':
    from temp_CPodDoc import CPodDoc
    pDoc = CPodDoc()
    x = [1,1.5,.1]
    print("x: "+str(x))
    f = [4,5,6]
    print ("f: "+str(f))
    jb = [[7,8,9],[10,11,12],[13,14,15]]
    print("jb: "+str(jb))
    print("jb[0]: "+str(jb[0]))
    print("ahat_fcn(x, f, jb)"+str(ahat_fcn(x, f, jb,pDoc)))
    print("x: "+str(x))
    print ("f: "+str(f))
    print("jb[0]: "+str(jb[0]))
    print("jb[1]: "+str(jb[1]))
    print("jb[2]: "+str(jb[2]))
    
    print("h(0.01): "+str(h(0.01)))
    print("h(0.25): "+str(h(0.25)))
    print("h(0.5): "+str(h(0.5)))
    print("h(1.0): "+str(h(1.0)))
    print("h(1.5): "+str(h(1.5)))
    print("h(15): "+str(h(15)))
