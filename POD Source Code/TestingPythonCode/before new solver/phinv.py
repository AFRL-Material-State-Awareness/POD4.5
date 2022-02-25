
'''
/* phinv.cpp
*
*
*---Algorithm AS 11 APPL. STATIST. Algorithm (1977) vol. 26, P.118
*
*   Produces normal deviate corresponding to lower tail area of P.
*   Real version for EPS = 2 ** (-31)
*   The HASH SUMS are the sums of the moduli of the coefficient.
*   They have no inherent meanings but are included for use in
*   checking transcriptions.
*/

'''
from __future__ import division
# forces floating point instead of integer math
from math import *


def phinv(p):
    a0 = 2.50662823884
    a1 = -18.61500062529
    a2 = 41.39119773534
    a3 = -25.44106049637
    b1 = -8.47351093090
    b2 = 23.08336743743
    b3 = -21.06224101826
    b4 = 3.13082909833

    if str(a0 - a1 + a2 - a3 - b1 + b2 - b3 + b4) != str(143.70383558076):
        print("E means Error!")

    c0 = -2.78718931138
    c1 = -2.29796479134
    c2 = 4.85014127135
    c3 = 2.32121276858
    d1 = 3.54388924762
    d2 = 1.63706781897

    if str(-c0 - c1 + c2 + c3 + d1 + d2) != str(17.43746520924):
        print("E means Error!")

    split = 0.42
    q = p - 0.5
    func = 0.0
    if (fabs(q) > split):
        r = p
        if (q > 0.0):
            r = 1.0 - p
        if (r <= 0.0):
            return 0.0  # // (p should be between 0 and 1)
        r = sqrt(-log(r))
        func = (((c3 * r + c2) * r + c1) * r + c0) / ((d2 * r + d1) * r + 1.0)
        if (q < 0.0):
            func = -func
    else:
        r = q * q
        func = q * (((a3 * r + a2) * r + a1) * r + a0) / \
            ((((b4 * r + b3) * r + b2) * r + b1) * r + 1.0)
    return func

if __name__ == '__main__':
    for i in range(11):
        if i == 0:
            x = 0.0001
        elif i == 10:
            x = 0.9999
        else:
            x = 0.1 * i
        y = phinv(x)
        print("%9.4f\t %18.12f" % (x, y))

