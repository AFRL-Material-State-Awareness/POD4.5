from __future__ import division #forces floating point instead of integer math

from math import pi, sqrt, exp, fabs, log
from mdnord import mdnord
from linreg import linear_regression
from leqslv import leqslv

import podplot

def pf_norm(f, n):
	maxdif = fabs(f[0])
	for i in range(n) :
		dif = fabs(f[i])
		if (dif>maxdif) : maxdif = dif
	return maxdif

def ahat_nonlinear_fcn(x, f, pDoc):
    print("Entering ahat_nonlinear_fcn **")

    acount = pDoc.acount
    bcount = pDoc.bcount
    npts = pDoc.npts

    rt2pi = sqrt(2.0*pi)
    #PARAMETER (RT2PI = 2.506628275)

    ierr = 0

    f=[0.0,0.0,0.0,0.0]
    f[2] = -npts*x[2]*x[2]
    #print('f: '+str(f)) 
    rlonx = pDoc.rlonx
    rlony = pDoc.rlony
    for i in range(npts):  #look at the first "npts" points
        xt = rlonx[i]      #xt is next value of x 
        yt = rlony[i]      #yt is next value of y
        yf = x[0] + x[1] * xt - x[3] * xt * yt  #yf is next value of fit
        #<fit equation> y = b + mx + cxy
        y = yt - yf  #y is residual between fit and data

        f[0] = f[0] + y  #sum of residuals
        f[1] = f[1] + y * xt  #sum of residuals times x
        f[2] = f[2] + y * y  #sum of residuals squared
        f[3] = f[3] + y *  xt * yt #sum of residuals times x and y
        
    print('x: '+str(x))
    print('f: '+str(f))
    #return (x,f)  #Begin orphaned code
    #y /= x[2]
    iacnt = ibcnt = 0
    for i in range(acount):
        xt = rlonx(i)
        yt = rlony(i)
        yf = x[0] + x[1] * xt - x[3] * xt * yt
        y = (yt - yf)/x[2]
        z = exp(-y*y/2.0)/rt2pi
        q = mdnord(y)
        q = 1.0 - q
        if (q == 0.0) :
	        iacnt += 1
	        continue
        tem = x[2] * z / q
        f[0] += tem
        f[1] += tem*xt
        f[3] += tem*xt*yt
        f[2] += tem*y

    for i in range(bcount) :
        xt = rlonx(i)
        yt = rlony(i)
        yf = x[0] + x[1] * xt - x[3] * xt * yt
        y  = (yt - yf) / x[2]
        z  = exp(-y * y / 2.0) / rt2pi
        q = mdnord(y)
        if (q==0.0) :
	        ibcnt += 1
	        continue
        tem = x[2] * z / q
        f[0] -= tem
        f[1] -= tem * xt
        f[3] -= tem * xt * yt
        f[2] -= tem * y

    if (iacnt+ibcnt !=0) : 
        ierr = 10000+100*iacnt+ibcnt
        return ierr  #end orphaned code
    return (x,f)

def ahat_nonlinear_sysolv(x, jb, fnorm, pDoc):
    print("Entering ahat_nonlinear_sysolv **")
	# defined in sysolv.cpp
    itmax = 20
    nsig = 5.0
    mindif = pow(10.0,-nsig)
	
    n = 4
    d = []
    f = []
    x1 = []
    f1 = []
    f2 = []
    
    #print("x1: "+str(x1))
    for i in range(n) : #initialize some variables
        d += [0.001]
        x1 += [0.0]
        f += [0.0]
        f1 += [0.0]
        f2 += [0.0]
        
    for each in range(itmax) :
        print ("** iteration ** "+str(each))
        (x,f) = ahat_nonlinear_fcn(x,f,pDoc)
        fn = pf_norm(f,n)
        for i in range(n) :
            print("i="+str(i))
            delta1 = d[i]/10.0
            delta2 = 1e-4*x[i]
            if fabs(delta1)>fabs(delta2) : 
                dif = delta1
            else : 
                dif = delta2
            print("dif: "+str(dif))
            for j in range(n) :
                x1[j] = x[j]
            
            x1[i] = x[i] - dif
            (x1,f1) = ahat_nonlinear_fcn(x1,f1, pDoc)
            x1[i] = x[i] + dif
            (x1,f2) = ahat_nonlinear_fcn(x1,f2, pDoc)
            for j in range(n) : 
                jb[j][i] = 0.5*(f2[j]-f1[j])/dif
            
        for i in range(n) : 
            d[i] = f[i]
        print("d: "+str(d))
        deter,d = leqslv(jb,d)
        print("Iteration " + str(each) +" fnorm " +str(fn) +"\n")
        #print("x: "+str(x))
        #print("f: "+str(f))
        print("d: "+str(d))
        #print("jb: "+str(jb))
        
        if (deter==0.0) :
            infomsg("zero determinant in sysolv")
            return -2
        maxdif = pf_norm(d,n)
        if (maxdif<mindif) :
            fnorm = pf_norm(f,n)
            return 0
        for i in range(n) : x[i] -= d[i]
        print("iter "+str(each)+" maxdif "+str(maxdif))
        print("\n\n")
        
        #-------------------  <DEBUG> crreate residuals charts
        r1 = (pDoc.rlonx,[exp((log(k)*x[1]+x[0])/(1+log(k)*x[3])) \
            for k in pDoc.rlonx],x[2])
        temp = [(k*x[1]+x[0])/(1+k*x[3]) \
            for k in pDoc.rlonx]
        residuals = [pDoc.rlony[k] - temp[k] \
            for k in range(len(temp))]
        #print(r1)
        podplot.fit((pDoc.rlonx,pDoc.rlony),r1,"-iter"+str(each))
        podplot.resid((pDoc.rlonx,residuals),"-iter"+str(each))
        #-------------------  </DEBUG>
    fnorm = pf_norm(f,n)
    print("last fnorm "+str(fnorm))
    return -1


if __name__ == '__main__':
    from CPodDoc import CPodDoc
    pDoc = CPodDoc()
    pDoc.GetInfo()
    pDoc.ParseInfo()
    pDoc.ahat_solve()
    
    npts2 = pDoc.npts+pDoc.acount+pDoc.bcount
    print(npts2)
    lr = linear_regression() 
    
    lr.regress([log(each) for each in pDoc.rlonx],[log(each) for each in pDoc.rlony],npts2)
    x = [lr.intercept,lr.slope,lr.rms,0.0]
    fnorm = 0
    print("x: "+str(x))
    jb = [[0,0,0,0],[0,0,0,0],[0,0,0,0],[0,0,0,0]]
    print("jb: "+str(jb))
    
    r1 = (pDoc.rlonx,[exp(log(each)*x[1]+x[0]) for each in pDoc.rlonx],x[2])
    temp = [exp(log(each)*x[1]+x[0]) for each in pDoc.rlonx]
    residuals = [(pDoc.rlony[each]) - (temp[each]) for each in range(len(temp))]
    #print(r1)
    podplot.fit((pDoc.rlonx,pDoc.rlony),r1,"linear fit")
    podplot.resid((pDoc.rlonx,residuals),"linear fit")
    
    print('ahat_nonlinear_sysolv(x, jb, fnorm,pDoc) '\
        +str(ahat_nonlinear_sysolv(x, jb, fnorm, pDoc)))
    print("x: "+str(x))
    print("jb: "+str(jb))

    '''lr.regress([log(each) for each in pDoc.rlonx],[log(each) for each in pDoc.rlony],pDoc.npts)
    x = [lr.intercept,lr.slope,lr.rms,0.0]
    lr.regress([log(each) for each in pDoc.rlony],pDoc.rlonx,pDoc.npts)
    x2 = [lr.intercept,lr.slope,lr.rms,0.0]
    
    print('ahat_nonlinear_sysolv(x, jb, fnorm, pDoc) '\
        +str(ahat_nonlinear_sysolv(x, jb, fnorm, pDoc)))
    print("x: "+str(x))
    print("jb: "+str(jb))
    
    import podplot
    from mdnord import mdnord
    steps = 100
    xcdf = [1.0*each/steps for each in range(steps)]
    #print (xcdf)
    firsthalf = [4.0*(1-mdnord(5.0*each)) for each in xcdf]
    firsthalf.reverse()
    cdf = firsthalf \
    + [4.0*(mdnord(5.0*(each+1.0/steps))) for each in xcdf]
    #print (cdf)
    xcdf = xcdf + [each+1.0 for each in xcdf]
    #xcdf = [each for each in xcdf]
    #print(xcdf)

    endpt = max(pDoc.rlonx)
    startpt = min(pDoc.rlonx)
    print(startpt)
    print(endpt)
    
    r1 = (pDoc.rlonx,[exp(log(each)*x[1]+x[0]) for each in pDoc.rlonx],x[2])
    print('pDoc.rlonx: '+str(pDoc.rlonx))
    print('x[1]: '+str(x[1]))
    print('x[0]: '+str(x[0]))
    print([(log(each)*x[1]+x[0]) for each in pDoc.rlonx])
    temp = [(log(each)*x[1]+x[0]) for each in pDoc.rlonx]
    print('temp: '+str(temp))
    residuals = [(pDoc.rlony[each]) - (temp[each]) for each in range(len(temp))]
    #print(r1)
    podplot.fit((pDoc.rlonx,pDoc.rlony),r1)
    podplot.resid((pDoc.rlonx,residuals))'''
    
    
    
    
