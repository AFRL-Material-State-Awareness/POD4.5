from __future__ import division # forces floating point division
#from phinv import phinv
#from scipy import stats
from math import log,  fabs
from phinv import phinv

#so this code can be called from C#
import clr
clr.AddReference("MathNet.Numerics")
import MathNet.Numerics

from MathNet.Numerics.Distributions import Normal
from MathNet.Numerics.Distributions import ChiSquared

def deter(((a,b),(c,d))):
   #'determinant of a 2x2 matrix'
   return a*d - b*c

def invert(((a,b),(c,d))):
   #'inverts a 2x2 matrix'
   det = deter(((a,b),(c,d)))
   return (((d/det, -b/det), (-c/det, a/det)))

    #gamma = self.x90 #Initial guess for gamma parameter
    #b = 1/self.sighat #Initial guess for b parameter

class crack_new_bounds():
    'holds the information needed to do the new pf likelihood bounds for a single crack'
    def __init__(self, xi, yi, ni, mu, sigma):
        self.xi = xi
        self.yi = yi
        self.ni = ni
        self.L = self.Lcalc(mu, sigma)
        
    def Lcalc(self, mu, sigma):
        #'This is the first term of g_1, '
        pstar = self.Pstar(mu,  sigma)
        return self.yi*log(pstar)+(self.ni - self.yi)*log(1-pstar)
    
    def Pstar(self, mu,  sigma):
       #'POD with initial mu and sigma from linear model solver'
       #return stats.norm(0, 1).cdf((self.xi-mu)/sigma)
       return Normal.CDF(0, 1, (self.xi-mu)/sigma)

class pf_new_bounds():
    'holds data to calculate the Likelihood ratio based POD bounds'
    def __init__(self,  data):
        self.pod_level = data.pod_level
        self.mu = data.muhat
        self.sigma = data.sighat
        self.gamma = data.x90
        self.b = 1/self.sigma
        self.new_pf_flaws = [crack_new_bounds(flaw.crkf, flaw.above,  flaw.count, self.mu, self.sigma) for flaw in data.flaws]
    
    def K(self, alpha):
       #return 0.5*stats.chi2.isf(alpha,1)
       return 1.076

    def zXX(self, pod_level):
       return phinv(pod_level)
       #return 1.282

    def Pi(self, gamma, b,  crack,  pod_level):
       #'POD of xi with specified alpha'
        #return stats.norm(0, 1).cdf(self.zXX(pod_level) +b*(crack.xi-gamma))
        return Normal.CDF(0, 1, self.zXX(pod_level) +b*(crack.xi-gamma))

    def phi(self,  gamma, b,  crack,  pod_level):
        #return stats.norm(0, 1).pdf(self.zXX(pod_level) +b*(crack.xi-gamma))
        return Normal.PDF(0, 1, self.zXX(pod_level) +b*(crack.xi-gamma))

    def g_0(self, gamma, b, mu, sigma,  crack,  pod_level):
       #'mle of b'
       P = self.Pi(gamma, b, crack, pod_level)
       Q = 1 - P
       return (crack.yi-crack.ni*P)/(P*Q)*(crack.xi-gamma)*self.phi(gamma, b,  crack,  pod_level)
       
    def g_0sum(self, gamma, b, mu, sigma,  pod_level):
        sum = 0
        for crack in self.new_pf_flaws:
            P = self.Pi(gamma, b, crack, pod_level)
            Q = 1 - P
            g0 = (crack.yi-crack.ni*P)/(P*Q)*(crack.xi-gamma)*self.phi(gamma, b,  crack,  pod_level)
            sum += g0
        return sum

    def g_1(self, gamma, b, mu, sigma, crack, pod_level):
       #'K - likelihood ratio'
       P = self.Pi(gamma, b, crack, pod_level)
       Q = 1- P
       K = self.K(1-pod_level)
       return K-crack.L+crack.yi*log(P)+(crack.ni-crack.yi)*log(Q)
    
    def g_1sum(self, gamma, b, mu, sigma, pod_level):
        sum = 0
        for crack in self.new_pf_flaws:
            P = self.Pi(gamma, b, crack, pod_level)
            Q = 1- P
            K = self.K(1-pod_level)
            g1 =  -crack.L + crack.yi*log(P)+(crack.ni-crack.yi)*log(Q)
            sum += g1
        sum += K
        return sum
        
    def jacobian(self, gamma, b,  crack,  pod_level):
       #'partial derivatives of g_0 and g_1 wrt gamma and b'
       P = self.Pi(gamma, b, crack, pod_level)
       Q = 1-P
       Phi = self.phi(gamma, b,  crack,  pod_level)
       return ((b*(crack.ni*(crack.xi-gamma))/(P*Q)*Phi**2, 
                             -(crack.ni*(crack.xi-gamma)**2)/(P*Q)*Phi**2),
                             (b*(crack.yi-crack.ni*P)/(P*Q)*Phi, 
                             (crack.yi-crack.ni*P)/(P*Q)*(crack.xi-gamma)*Phi))
    
    def jsums(self, gamma, b, pod_level):
        sums = ((0.0, 0.0), (0.0, 0.0))
        for crack in self.new_pf_flaws:
            P = self.Pi(gamma, b, crack, pod_level)
            Q = 1-P
            Phi = self.phi(gamma, b,  crack,  pod_level)
            jb = self.jacobian(gamma, b,  crack,  pod_level)
            sums = ((sums[0][0]+jb[0][0],
                                  sums[0][1]+jb[0][1]),
                                  (sums[1][0]+jb[1][0],
                                    sums[1][1]+jb[1][1]))
        return sums
    
    def solve(self,  pod_level,  gamma,  b, mu,  sigma):
        DEBUG = False
        z90 = self.zXX(pod_level)
        
        #K = .5*stats.chi2.isf(1-pod_level, 1)
        #K = .5*(1.0-ChiSquared.CDF(1-pod_level, 1))
        K = 1.076

        if DEBUG:
            print("Initial")
            print("gamma "+str(gamma)+" b "+str(b))
            print("g_0 \t\t g_1 \t\t dgamma \t\t db \t\t gamma \t\t b")
            
        deltab = b
        deltagamma = gamma
        while fabs(deltagamma) > .0001 * gamma and fabs(deltab) > .0001* b:
            #print("iteration "+ str(i))

            jsums = ((0.0, 0.0), (0.0, 0.0))
            g_0sum = self.g_0sum(gamma, b, mu, sigma, pod_level)
            g_1sum = self.g_1sum(gamma, b, mu, sigma, pod_level)
            jsums = self.jsums(gamma, b, pod_level)
            det = deter(jsums)
            jinv = invert(jsums)
            deltagamma= (jinv[0][0] * g_1sum + jinv[0][1] * g_0sum)
            deltab = (jinv[1][0] * g_1sum + jinv[1][1] * g_0sum)
            deltagamma /= 3
            deltab /= 3

            b += deltab
            gamma += deltagamma 
            if DEBUG:
                print(str(g_0sum) + '\t\t' + str(g_1sum)
                      + '\t\t'+ str(deltagamma) + '\t\t' + str(deltab) 
                      + '\t\t' + str(gamma) + '\t\t' + str(b) )
            return (gamma, b)
