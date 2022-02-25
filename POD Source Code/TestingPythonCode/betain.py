
from math import *
'''
/*
C---Arguments: BETAIN(i,i,i,i,o)
C
C   Computes incomplete Beta function ratio for arguments
C   X between zero and one, P and Q positive.
C   Log of complete Beta function, BETA, is assumed to be known.
C
C   X      = the value of the upper limit x.
C   P      = the value of the parameter p.
C   Q      = the value of the parameter q.
C   BETA   = the value of ln B(p,q)
C   IFAULT = a fault indicator, equal to:
C            1 if p <= 0 or q <= 0
C            2 if x < 0 or x > 1
C            0 otherwise.
C
C   Algorithm AS 63, Appl. Statis. (1973) Vol.22, P.409
*
*    This seems to be the same as Excel
*            BETADIST(x,alpha,beta)
*    which returns the cumulative beta probability density function.
*   The cumulative beta probability density function is commonly used
*   to study variation in the percentage of something across samples,
*   such as the fraction of the day people spend watching television.
*/
'''


def betain(x, p, q, beta):
    acu = 1e-8
    '''
        // test for admissibity of arguments
        if (p<=0.0 || q<=0.0) {
            AfxMessageBox("betain: p and q must be positive")
            return 0.0
        }
        if (x<0.0 || x>1.0) {
            AfxMessageBox("betain: x must be between 0 and 1")
            return 0.0
        }
        if (x==0.0 || x==1.0) return x
    '''
    if (p <= 0.0 or q <= 0.0):
        #########################################
        print("betain: p and q must be positive")
        #########################################
        return 0.0
    if (x < 0.0 or x > 1.0):
        ##########################################
        print("betain: x must be between 0 and 1")
        ##########################################
        return 0.0
    if (x == 0.0 or x == 1.0):
        return x
    psq = p + q
    cx = 1.0 - x
    if (p < psq * x):
        xx = cx
        cx = x
        pp = q
        qq = p
        index = True
    else:
        xx = x
        pp = p
        qq = q
        index = False
        term = 1.0
        ai = 1.0
        result = 1.0
        ns = int((qq + (xx * psq)))

    term = 1.0
    ai = 1.0
    result = 1.0
    ns = (int)(qq + (xx * psq))

    #// Use reduction formulae of Soper
    rx = xx / cx

    temp = qq - ai
    if (ns == 0):
        rx = xx
    loopCount = 0
    while (1):
        term *= temp * rx / (pp + ai)
        result += term
        temp = fabs(term)
        compare = acu * result
        if (temp <= acu and temp <= abs(compare)):
            break
        ai += 1
        ns -= 1
        if (ns >= 0):
            temp = qq - ai
            if (ns == 0):
                rx = xx
            continue
        temp = psq
        psq += 1
        loopCount += 1

    #print(loopCount)

    result *= exp(pp * log(xx) + (qq - 1.0) * log(cx) - beta) / pp
    if (index):
        result = 1.0 - result
    return result

if __name__ == '__main__':
    #betain(x, p, q, beta):
    print('betain(.1,.2,.3,.4) ' + str(betain(.1, .2, .3, .4)))
    print('betain(1,.2,.3,.4) ' + str(betain(1, .2, .3, .4)))
    print('betain(0,.2,.3,.4) ' + str(betain(0, .2, .3, .4)))
    print('betain(-.10,.2,.3,.4) ' + str(betain(-.10, .2, .3, .4)))
    print('betain(.1,-.2,.3,.4) ' + str(betain(.1, -.2, .3, .4)))
