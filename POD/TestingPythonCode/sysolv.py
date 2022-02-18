
from __future__ import division
# forces floating point instead of integer mathsysolv.py
#from math import abs
from fcn import ahat_fcn,  pf_fcn,  odds_fcn, write_one_variance, write_linfit

from leqslv import leqslv,  tdleq
#from temp_CPodDoc import CPodDoc
from PODaccessories import pretty_print

nsig = 5  # - # of significant digits accuracy
itmax = 20  # Max. # of iteratons for convergence algorithm
mindif = pow(10.0, -nsig)

DEBUG = False

def ahat_sysolv(data):
    'This seems to estimate how good the regression fit is and slightly improve it?'
    data.deter = fn = 0.0
    d = data.res_sums = [0, 0, 0]
    n = 3
    for iterr in range(itmax):
        ifer = ahat_fcn(data)
        for i in range(n):
            d[i] = data.res_sums[i]
        leqslv(data)
        if DEBUG:
            print("Iteration " + str(iterr))
            #print("dd: " + str((dd[0],dd[1],dd[2])))
            print("variance: " + str((data.res_sums[0],data.res_sums[1],data.res_sums[2])))
        if (data.deter == 0.0):
            raise("zero determinant in sysolv")
        temp = [abs(d[i]) > mindif for i in range(n)]
        if DEBUG:
            print(temp)
        if any(temp) or data.linfit['rms'] < 0.0: #added additional check to counter really bad fits that pop up in some cases, trb, 06-08-2015
            data.linfit['intercept'] -= d[0]
            data.linfit['slope'] -= d[1]
            data.linfit['rms'] -= d[2]
            write_linfit(data.linfit, 'ahat_syssolv', 40)
        else:
            fn = 0.0
            for i in range(n):
                fn += data.res_sums[i] * data.res_sums[i]
            if (fn > 1.0):
                raise("converence to non-zero F")
            data.fnorm[0] = fn
            return ifer

    if iterr+1 < itmax:
        return 0
    else:
        #print("Did not converge!")
        return -1

def pf_norm(res_sums):
    'Just returns the maximum absolute residual'
    return max([abs(each) for each in  res_sums])

def pf_sysolv(data,  x, setinc):
    'This is the pass/fail solver using the linear model'
    DEBUG = False
    nret = True
    n = 2
    d = [0.01,  0.01]
    x1 = [0.0, 0.0]
    f1 = [0.0, 0.0]
    r = [0.0, 0.0]
    f = [0.0, 0.0]

    wIteration = 0
    wMu = x[0]
    wSigma = x[1]
    wFNorm = 0.0
    wDidDamp = False
    wDamp = 1.0

    if DEBUG:
        print("Solving using Linear model")
        print("iteration\tmu\tsigma\tfnorm")
    #x = [data.muhat,  data.sighat]
    for iter in range(itmax):
        
        pf_fcn(x, f,  data)
        data.res_sums = f
        data.fnorm[setinc] = pf_norm(f)
        wFNorm = data.fnorm[setinc]
        if DEBUG:
            pretty_print([iter,  x[0],  x[1],  data.fnorm[setinc]])
        for i in range(n):
            delta1 = d[i]/10.0
            delta2 = 1e-4*x[i];
            if abs(delta1)>abs(delta2):
                dif = delta1
            else:
                dif = delta2
            for j in range(n):
                x1[j] = x[j]
                if i == j:
                    x1[j] += dif
            pf_fcn(x1,  f1,  data)
            for j in range(n):
                data.variance[j][i] = (f1[j]-f[j])/dif
                write_one_variance(data.variance, j, i, 'pf_sysolv', 93)
        for i in range(n):
            d[i] = f[i]
        d = tdleq(data,  d)
        if data.deter == 0:
            return True
        r[0] = d[0]/x[0]
        r[1] = d[1]/x[1]
        dmax = pf_norm(r)/0.4
        didDamp = False
        if (dmax>1.0):
            if DEBUG:
                print("damping: "+str(dmax))
            d[0] /= dmax;
            d[1] /= dmax;
            didDamp = True
        #data.muhat -= d[0]
        #data.sighat -= d[1]
        x[0] -= d[0]
        x[1] -= d[1]
        if DEBUG:
            print("Muhat: " + str(x[0]) + " Sighat: " + str(x[1]))
        maxdif = pf_norm(d);
        #maxdif = pf_norm(d);

        data.save_pf_solve_iteration(setinc, wIteration, wMu, wSigma, wFNorm, wDidDamp, wDamp)

        wIteration = iter + 1
        wMu = x[0]
        wSigma = x[1]
        wDidDamp = didDamp
        wDamp = dmax

        if (maxdif<mindif):
            nret = False;
            break

    pf_fcn(x, f, data);
    data.res_sums = f
    data.fnorm[setinc] = pf_norm(data.res_sums);

    wFNorm = data.fnorm[setinc]

    data.save_pf_solve_iteration(setinc, wIteration, wMu, wSigma, wFNorm, wDidDamp, wDamp)
    
    return nret





def odds_sysolv(data,  x, setinc):
    'This is the pass/fail solver using the odds model'
    nret=-1;
    n = 2
    d = [0.01,  0.01]
    x1 = [0.0, 0.0]
    f1 = [0.0, 0.0]
    r = [0.0, 0.0]
    f = [0.0, 0.0]

    wIteration = 0
    wMu = x[0]
    wSigma = x[1]
    wFNorm = 0.0
    wDidDamp = False
    wDamp = 1.0

    if DEBUG:
        print("Solving using ODDS model")
        print("iteration\tmu\tsigma\tfnorm")
    #x = [data.muhat,  data.sighat]
    for iter in range(itmax):
        odds_fcn(x, f,  data)
        data.fnorm[setinc] = pf_norm(f)
        wFNorm = data.fnorm[setinc]
        if DEBUG:
            pretty_print([iter,  x[0],  x[1],  data.fnorm[setinc]])
        for i in range(n):
            delta1 = d[i]/10.0
            delta2 = 1e-4*x[i];
            if abs(delta1)>abs(delta2):
                dif = delta1
            else:
                dif = delta2
            for j in range(n):
                x1[j] = x[j]
                if i == j:
                    x1[j] += dif
            odds_fcn(x1,  f1,  data)
            for j in range(n):
                data.variance[j][i] = (f1[j]-f[j])/dif
                write_one_variance(data.variance, j, i, "odds_syssolv", 159);
        for i in range(n):
            d[i] = f[i]
        d = tdleq(data,  d)
        if data.deter == 0:
            return -1
        r[0] = d[0]/x[0]
        r[1] = d[1]/x[1]
        dmax = pf_norm(r)/0.4
        didDamp = False
        if (dmax>1.0):
            if DEBUG:
                print("damping: "+str(dmax))
            d[0] /= dmax;
            d[1] /= dmax;
            didDamp = True

        x[0] -= d[0]
        x[1] -= d[1]

        maxdif = pf_norm(d);
        #maxdif = pf_norm(d);

        data.save_pf_solve_iteration(setinc, wIteration, wMu, wSigma, wFNorm, wDidDamp, wDamp)

        wIteration = iter + 1
        wMu = x[0]
        wSigma = x[1]
        wDidDamp = didDamp
        wDamp = dmax

        

        if (maxdif<mindif):
            nret=0;
            break

    odds_fcn(x,f,  data);
    data.fnorm[setinc] = pf_norm(f);

    wFNorm = data.fnorm[setinc]

    data.save_pf_solve_iteration(setinc, wIteration+1, wMu, wSigma, wFNorm, wDidDamp, wDamp)

    return nret

if __name__ == '__main__':
    
    import pickle
    from CPodDoc import *
    f = open("pfsavedata.txt",  'rU')
    pfdata= pickle.load(f)
    f.close()
   
    x = [pfdata.muhat,  pfdata.sighat]
    pf_sysolv(pfdata, x)
    
