#/* alnorm.cpp
#*
#*   Calculates the lower or upper tail of the standardized normal curve
#*   corresponding to any given argument.
#*
#*   X = the argument value
#*   UPPER = .TRUE. if the area to be computed is "X to infinity"
#*           .FALSE. if the area to be computed is "minus infinity to X"
#*
#*   Algorithm AS 66 Appl. Statist. (1973) Vol.22, P.424
#*
#*
#*
#*---LTONE and UTZERO must be set to suit the particular computer
#*   (see written text)
#*/



import math

muk = 0.0008
sigk = 1.08


def alnorm(x, upper):
    a1 = 0.398942280444
    a2 = 0.399903438504
    a3 = 5.75885480458
    a4 = 29.8213557808
    a5 = 2.62433121679
    a6 = 48.6959930692
    a7 = 5.92885724438
    b1 = 0.398942280385
    b2 = 3.8052E-8
    b3 = 1.00000615302
    b4 = 3.98064794E-4
    b5 = 1.98615381364
    b6 = 0.151679116635
    b7 = 5.29330324926
    b8 = 4.8385912808
    b9 = 15.1508972451
    b10 = 0.742380924027
    b11 = 30.789933034
    b12 = 3.99019417011

    con = 1.28
    ltone = 7.0
    utzero = 18.66

    z = x
    up = upper
    if (z < 0.0):
        up = not upper
        z = -z

    if (z <= ltone or (up and z <= utzero)):
        y = 0.5 * z * z
        if (z > con):
            num = b1 * math.exp(-y)
            den = (z - b2 + b3 / (z + b4 + b5 / (z - b6 + b7 / (z + b8 - b9 /
                (z + b10 + b11 / (z + b12))))))
            func = num / den
        else:
            func = 0.5 - z * (a1 - a2 * y / (y + a3 - a4 /
                (y + a5 + a6 / (y + a7))))
    else:
        func = 0.0
    if not up:
        func = 1.0 - func
    return func
