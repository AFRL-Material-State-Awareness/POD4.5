
from alogam import alogam
from math import exp,  log

def gammds(y, p):
    #double a, c, f, v;
    eps = 1e-6;

    #// Checks admissibility of arguments and value of F
    if (y<=0.0 or p<=0.0):
        #AfxMessageBox("Illegal argument in gammds");
        return 0.0;

    #// alogam is natural log of gamma function.

    f = exp(p*log(y)-alogam(p+1.0)-y);
    if f<=0:
        return -1;
    #ASSERT(f>0.0);

    #// series begins
    c = 1.0
    v = 1.0
    a = p
    while (c/v>eps):
        a += 1.0;
        c = c*y/a;
        v += c;
    return v*f;
