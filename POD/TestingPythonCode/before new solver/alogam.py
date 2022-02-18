
'''
/* alogam.cpp
*
*
*   Evaluates natural logarithm of GAMMA(X)
*   for X greater than ZERO
*
*   X      = value of x for which Gamma(x) is required. Must be > 0
*
*   Algorithm ACM 291, Comm. ACM. (1966) Vol.9, P.684
*
*/

'''
from math import *


def alogam(x):
    pi = 4.0 * atan(1.0)
    a1 = log(2.0 * pi) / 2.0
    a2 = 1.0 / 1680.0
    a3 = 1.0 / 1260.0
    a4 = 1.0 / 360.0
    a5 = 1.0 / 12.0

    y = x

    if (x <= 0.0):
        print("alogam(x) not defined for non-positive x")
        return 0.0
    f = 0.0
    if (y < 7.0):
        f = 1.0
        while (y < 7.0):
            f *= y
            y += 1.0
        f = -log(f)
    z = 1.0 / (y * y)
    f += (y - 0.5) * log(y) - y + a1 + (((a3 - a2 * z) * z - a4) * z + a5) / y
    return f


if __name__ == '__main__':
    print('alogam(23.2) ' + str(alogam(23.2)))
    print('alogam(0.232) ' + str(alogam(0.232)))
    print('alogam(0.00232) ' + str(alogam(0.00232)))
    print('alogam(-23.2) ' + str(alogam(-23.2)))



