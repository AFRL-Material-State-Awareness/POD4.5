from __future__ import division #forces floating point instead of integer math
from math import fabs, exp, log
from fcn import ahat_fcn
from leqslv import leqslv
from linreg import linear_regression
#from temp_CPodDoc import CPodDoc

nsig = 5; # - # of significant digits accuracy
itmax = 20; # Max. # of iteratons for convergence algorithm
mindif = pow(10.0,-nsig);

def ahat_sysolv(x, jb, fnorm, pDoc):

    deter = fn = 0.0
    d = f = [0,0,0]
    n = 3
    for iterr in range(itmax) :
        ifer = ahat_fcn(x,f,jb,pDoc)
        for i in range(n) : d[i] = f[i]
        deter = leqslv(jb,d)
        #print("Iteration "+str(iterr));
        #print("f: "+str((f[0],f[1],f[2])));
        #print("d: "+str((d[0],d[1],d[2])));
        if (deter==0.0) :
            raise("zero determinant in sysolv");
        for i in range (n) :
            if (fabs(d[i])>mindif):
                for i in range(i) : x[j] -= d[j];
                if iterr<itmax : return 0
                else : return -1
        fn = 0.0;
        for i in range(n) : fn += f[i]*f[i];
        if (fn>1.0) : raise("converence to non-zero F");
        fnorm = fn;
        return ifer;
        


if __name__ == '__main__':
    from CPodDoc import CPodDoc
    pDoc = CPodDoc()
    pDoc.GetInfo()
    pDoc.ParseInfo()
    pDoc.ahat_solve()
    
    npts2 = pDoc.npts+pDoc.acount+pDoc.bcount
    print(npts2)
    lr = linear_regression() 
    
    lr.regress(pDoc.rlonx,pDoc.rlony,npts2)
    x = [lr.intercept,lr.slope,lr.rms]
    fnorm = 0
    print("x: "+str(x))
    jb = [[0,0,0],[0,0,0],[0,0,0]]
    print("jb: "+str(jb))
    print('ahat_sysolv(x, jb, fnorm,pDoc) '+str(ahat_sysolv(x, jb, fnorm, pDoc)))
    print("x: "+str(x))
    print("jb[0]: "+str(jb[0]))
    print("jb[1]: "+str(jb[1]))
    print("jb[2]: "+str(jb[2]))
    
    #lr.regress([log(each) for each in pDoc.rlonx],[log(each) for each in pDoc.rlony],pDoc.npts)
    lr.regress([log(each) for each in pDoc.rlonx],[log(each) for each in pDoc.rlony],pDoc.npts)
    x = [lr.intercept,lr.slope,lr.rms]
    lr.regress([each for each in pDoc.rlony],pDoc.rlonx,pDoc.npts)
    x2 = [lr.intercept,lr.slope,lr.rms]
    print('ahat_sysolv(x, jb, fnorm, pDoc) '+str(ahat_sysolv(x, jb, fnorm, pDoc)))
    print("x: "+str(x))
    print("jb[0]: "+str(jb[0]))
    print("jb[1]: "+str(jb[1]))
    print("jb[2]: "+str(jb[2]))
    
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
    print(endpt)
    print(startpt)
    r1 = (pDoc.rlonx,[exp(log(each)*x[1]+x[0]) for each in pDoc.rlonx],x[2])
    temp = [exp(log(each)*x[1]+x[0]) for each in pDoc.rlonx]
    residuals = [pDoc.rlony[each] - temp[each] for each in range(len(temp))]
    #print(r1)
    podplot.fit((pDoc.rlonx,[each for each in pDoc.rlony]),r1)
    podplot.resid((pDoc.rlonx,residuals))
    
    
    
    
