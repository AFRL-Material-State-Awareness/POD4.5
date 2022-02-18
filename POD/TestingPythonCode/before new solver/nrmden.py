
#/* nrmden.cpp
#*
#*    normal distribution function (mean 0, std dev 1)
#*
#*       \frac{1}{\sqrt{2\pi\sigma} \exp(-\frac{(x-\mu)^2}{2\sigma^2})
#*/
#include "stdafx.h"
#include <math.h>
import math

twopi = 2.0 * math.pi
rt2pi = math.sqrt(twopi)


def nrmden3(z, mu, sigma):
    z = (z - mu) / sigma
    func = math.exp(-0.5 * z * z) / math.sqrt(twopi * sigma)
    return func


def nrmden(z):
    func = math.exp(-0.5 * z * z) / rt2pi
    return func

if __name__ == '__main__':
    print('nrmden(23.2) ' + str(nrmden(23.2)))
    print('nrmden3(15,0.2,.5) ' + str(nrmden3(15, 0.2, .5)))


