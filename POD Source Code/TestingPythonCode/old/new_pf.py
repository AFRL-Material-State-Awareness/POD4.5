from __future__ import division # forces floating point division
#from phinv import phinv
#from scipy import stats
from math import log,  fabs,  sqrt
from phinv import phinv
from lookup_table import mydists

def deter(((a,b),(c,d))):
   #'determinant of a 2x2 matrix'
   return a*d - b*c

def invert(((a,b),(c,d))):
   #'inverts a 2x2 matrix'
   det = deter(((a,b),(c,d)))
   return (((d/det, -b/det), (-c/det, a/det)))

def dot(x,  y): 
    return (x[0]*y[0]+x[1]*y[1])

def ludcmp(input,  index):
    TINY = 1.0e-20
    n = len(input[0])
    vv = [0.0, 0.0]
    odd = 1.0
    for i in range(n):
        big=0.0
        for j in range(n):
            temp = fabs(input[i][j])
            if (temp > big): 
                big=temp
            if big == 0.0:
                print("Linear decomposition error, singular matrix.")
            vv[i]=1.0/big
    for j in range(n):
        for i in range(j):
            sum = input[i][j]
            for k in range(i):
                sum -= input[i][k]*input[k][j]
            input[i][j]=sum
        big=0.0
        for i in range(j, n):
            sum=input[i][j]
            for k in range(j):
                sum -= input[i][k]*input[k][j]
            input[i][j]=sum
            dum=vv[i]*fabs(sum)
            if dum>=big:
                big=dum
                imax=i
        if j!= imax:
            for k in range(n):
                dum=input[imax][k]
                input[imax][k]=input[j][k]
                input[j][k]=dum
            odd = -odd
            vv[imax]=vv[j]
        index[j]=imax
        if input[j][j]==0.0:
            input[j][j]=TINY
        if j != n-1:
            dum=1.0/input[j][j]
            for i in range(j+1, n):
                input[i][j] *= dum
    
def lubksp(input, index, output):
    ii = 0
    n = len(input[0])
    for i in range(n):
        ip = index[i]  
        sum = output[ip]
        output[ip] = output[i]
        if ii !=0:
            for j in range(ii):
                sum -= input[i][j]*output[j]
        elif sum!=0.0:
            ii=i+1
        output[i]=sum
    backsub = [n-i-1 for i in range(n)]
    for i in backsub:
        sum = output[i]
        for j in range(i+1, n):
            sum -= input[i][j]*output[j]
        output[i] = sum/input[i][i]

    
class crack_new_bounds():
    'holds the information needed to do the new pf likelihood bounds for a single crack'
    def __init__(self, xi, yi, ni, mu, sigma, cdftable):
        self.xi = xi
        self.yi = yi
        self.ni = ni
        self.cdftable = cdftable
        self.L = self.Lcalc(mu, sigma)

    
    def Lcalc(self, mu, sigma):
        #'This is the first term of g_1, '
        pstar = self.Pstar(mu,  sigma)
        return self.yi*log(pstar)+(self.ni - self.yi)*log(1-pstar)
    
    def Pstar(self, mu,  sigma):
       #'POD with initial mu and sigma from linear model solver'
       #return stats.norm(0, 1).cdf((self.xi-mu)/sigma)
       return self.cdftable[(self.xi-mu)/sigma]

class pf_new_bounds():
    'holds data to calculate the Likelihood ratio based POD bounds'
    def __init__(self,  data):
        self.pod_level = data.pod_level
        self.mu = data.muhat
        self.sigma = data.sighat
        self.gamma = data.x90
        self.b = 1/self.sigma
        self.stats = mydists()
        self.new_pf_flaws = [crack_new_bounds(flaw.crkf, flaw.above,  flaw.count, self.mu, self.sigma,  self.stats.normcdf) for flaw in data.flaws]
        self.tolerance = 1.0e-8
        self.mintolerance = 1.0e-12
    
    def K(self, alpha):
       #return 0.5*stats.chi2.isf(alpha,1)
       return 0.5*self.stats.invchi2[alpha]

    def zXX(self, pod_level):
       return phinv(pod_level)
       #return 1.282

    def Pi(self, gamma, b,  crack,  pod_level):
       #'POD of xi with specified alpha'
        #return stats.norm(0, 1).cdf(self.zXX(pod_level) +b*(crack.xi-gamma))
        return self.stats.normcdf[self.zXX(pod_level) +b*(crack.xi-gamma)]

    def phi(self,  gamma, b,  crack,  pod_level):
        #return stats.norm(0, 1).pdf(self.zXX(pod_level) +b*(crack.xi-gamma))
        return self.stats.normpdf[self.zXX(pod_level) +b*(crack.xi-gamma)]

    def g_1(self, gamma, b, mu, sigma,  crack,  pod_level):
       #'mle of b'
       P = self.Pi(gamma, b, crack, pod_level)
       Q = 1 - P
       return (crack.yi-crack.ni*P)/(P*Q)*(crack.xi-gamma)*self.phi(gamma, b,  crack,  pod_level)
       
    def g_1sum(self, gamma, b, mu, sigma,  pod_level):
        sum = 0
        for crack in self.new_pf_flaws:
            P = self.Pi(gamma, b, crack, pod_level)
            Q = 1 - P
            g0 = (crack.yi-crack.ni*P)/(P*Q)*(crack.xi-gamma)*self.phi(gamma, b,  crack,  pod_level)
            sum += g0
        return sum

    def g_0(self, gamma, b, mu, sigma, crack, pod_level):
       #'K - likelihood ratio'
       P = self.Pi(gamma, b, crack, pod_level)
       Q = 1- P
       K = self.K(1-pod_level)
       return K-crack.L+crack.yi*log(P)+(crack.ni-crack.yi)*log(Q)
    
    def g_0sum(self, gamma, b, mu, sigma, pod_level):
        sum = 0
        for crack in self.new_pf_flaws:
            P = self.Pi(gamma, b, crack, pod_level)
            Q = 1- P
            K = self.K(1-pod_level)
            g1 =  -crack.L + crack.yi*log(P)+(crack.ni-crack.yi)*log(Q)
            sum += g1
        sum += K
        return sum
    
    def gsums(self, gamma,  b,  mu,  sigma,  pod_level):
        return [self.g_0sum(gamma, b, mu, sigma, pod_level), self.g_1sum(gamma, b, mu, sigma, pod_level)]
    
    def jacobian(self, gamma, b,  crack,  pod_level):
       #'partial derivatives of g_0 and g_1 wrt gamma and b'
       P = self.Pi(gamma, b, crack, pod_level)
       Q = 1-P
       Phi = self.phi(gamma, b,  crack,  pod_level)
       return [[b*(crack.ni*(crack.xi-gamma))/(P*Q)*Phi**2, 
                             -(crack.ni*(crack.xi-gamma)**2)/(P*Q)*Phi**2],
                             [b*(crack.yi-crack.ni*P)/(P*Q)*Phi, 
                             (crack.yi-crack.ni*P)/(P*Q)*(crack.xi-gamma)*Phi]]
    
    def jsums(self, gamma, b, pod_level):
        sums = ((0.0, 0.0), (0.0, 0.0))
        for crack in self.new_pf_flaws:
            P = self.Pi(gamma, b, crack, pod_level)
            Q = 1-P
            Phi = self.phi(gamma, b,  crack,  pod_level)
            jb = self.jacobian(gamma, b,  crack,  pod_level)
            sums = [[sums[0][0]+jb[0][0],
                                  sums[0][1]+jb[0][1]],
                                  [sums[1][0]+jb[1][0],
                                    sums[1][1]+jb[1][1]]]
        return sums

    def linesearch(self, oldguesses, oldmetric,  deltas, guesses, linearsolution, stepmax):  #linesearch guarantees nearly global convergence
        DEBUG = False
        min_change = .001
        
        sum = linearsolution[0]**2+linearsolution[1]**2
        if sqrt(sum)>stepmax:
            linearsolution = (linearsolution[0]*stepmax/sum, linearsolution[1]*stepmax/sum)
        slope = deltas[0]*linearsolution[0]+deltas[1]*linearsolution[1]
        if slope > 0.0:
            print("Error!")  #??Looks for rounding errors??
        test = max(fabs(linearsolution[0]/max(fabs(oldguesses[0]), 1.0)), fabs(linearsolution[1]/max(fabs(oldguesses[1]), 1.0)))
        alpha_min = self.tolerance/test #alpha is a tiny increment for the line search
        alpha = 1.0
        alpha2 = 0.0
        metric2 = 0.0
        while(True):
            guesses = (oldguesses[0]-alpha*linearsolution[0], oldguesses[1]-alpha*linearsolution[1])
            functions = self.gsums(guesses[0], guesses[1], self.mu, self.sigma,  self.pod_level)
            metric = 0.5*dot(functions, functions)
            min_decrease = min_change*alpha*slope
            decrease = metric - oldmetric
            if DEBUG:
                print ("alpha "+str(alpha) + " decrease " + str(decrease))
            if alpha < alpha_min:
                return guesses,  (alpha*linearsolution[0], alpha*linearsolution[1])
            elif decrease <= min_decrease:
               return guesses,  (alpha*linearsolution[0], alpha*linearsolution[1])
            else:
                if alpha == 1.0:  
                    tempalpha = -slope/(2.0*(metric-oldmetric-slope))
                else:
                    righthandside1 = metric-oldmetric-alpha*slope
                    righthandside2 = metric2-oldmetric-alpha2*slope
                    a = (righthandside1/alpha**2-righthandside2/alpha2**2)/(alpha-alpha2)
                    b = (-alpha2*righthandside1/alpha**2+alpha*righthandside2/alpha2**2)/(alpha-alpha2)
                    if (a == 0.0):
                        tempalpha = -slope/(2*b)
                    else:
                        disc = b**2-3.0*a*slope
                        if disc < 0.0:
                            tempalpha = 0.5*alpha
                        elif b <= 0.0:
                            tempalpha = (-b*sqrt(disc))/(3.0*a)
                        else:
                            tempalpha = slope/(b+sqrt(disc))
                    if tempalpha > 0.5 * alpha:
                        tempalpha = 0.5 * alpha
            alpha2=alpha
            metric2 = metric
            alpha = max(tempalpha, 0.1*alpha)
        
        return guesses,  (alpha*linearsolution[0], alpha*linearsolution[1])

    def solve(self,  pod_level,  gamma,  b, mu,  sigma):

        DEBUG = False
        z90 = self.zXX(pod_level)
        K = self.K(1-pod_level)

        if DEBUG:
            print("Initial")
            print("gamma "+str(gamma)+" b "+str(b))
            print("g_0 \t\t g_1 \t\t dgamma \t\t db \t\t gamma \t\t b \t\t metric")
            
        guesses = (gamma,  b)
        deltas = (gamma,  b)
        functions = self.gsums(gamma,  b,  mu,  sigma,  pod_level) #g_0 and g_1
        metric = 0.5*dot(functions, functions)
        
        #Test if we guessed too well
        if fabs(functions[0]) < 0.01 * self.tolerance or fabs(functions[1]) < 0.01 * self.tolerance:  #check for initial guess at a root
            if DEBUG:
                print("Initial guess was a root!")
            return guesses
        
        #        jsums = self.jsums(gamma, b, pod_level)     #jacobians from pate
        #        deltagamma= (jinv[0][0] * g_1sum + jinv[0][1] * g_0sum)  #deltas computed be inverting J and multiplying by g
        #        deltab = (jinv[1][0] * g_1sum + jinv[1][1] * g_0sum)
        #        original_delta = 1/2*(deltagamma**2+deltab**2)  #neededdeltas = later
        #        b += deltab
        #        gamma += deltagamma
        #        lastdeltab = deltab
        #        lastdeltagamma = deltagamma
        
        stepmax = 100
        vectorlength = sqrt(gamma**2 + b**2)
        stepmax = stepmax * max(vectorlength,  2)
        iter = 0
        STOP = False
        while(iter < stepmax and STOP == False):
            iter += 1 
            #print("iteration "+ str(i))
            jsums = self.jsums(gamma, b, pod_level) #jacobians
            det = deter(jsums)
            jinv = invert(jsums) #inverted jacobians
            
            deltas = (jinv[0][0] * functions[0] + jinv[0][1]*functions[1], jinv[1][0] * functions[0] + jinv[1][1] * functions[1])
            oldguesses = guesses  #Store the old estimates and solutions, x
            linearsolution = [-functions[0], -functions[1]] #initial guess for linear solver
            index = [0, 0]
            ludcmp(jsums, index)
            lubksp(jsums, index, linearsolution)
            guesses, deltas = self.linesearch(oldguesses, metric, deltas, guesses, linearsolution, stepmax)
            metric = 0.5*dot(functions, functions)
            functions = self.gsums(guesses[0], guesses[1],  mu,  sigma,  pod_level) #g_0 and g_1
            test = max(fabs(deltas[0]), fabs(deltas[1]))
            if test < 1e-6 :
                STOP = True
            if DEBUG:
                print(str(functions[0]) + '\t' + str(functions[1])
                      + '\t'+ str(deltas[0]) + '\t' + str(deltas[1]) 
                      + '\t' + str(guesses[0]) + '\t' + str(guesses[1]) 
                      + '\t' + str(metric))
            
        return (guesses)
