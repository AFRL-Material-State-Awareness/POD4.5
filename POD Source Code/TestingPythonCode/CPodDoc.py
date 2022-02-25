
from linreg import linear_regression
from sysolv import ahat_sysolv,  pf_sysolv,  odds_sysolv
from funcr import *
from fcn import pod_odds,  odds_inv, write_variance, start_variance, write_one_variance, write_list_params, Write_Ordered_Dict, write_msg, write_one_value
from math import sqrt, log, factorial
from phinv import phinv
from alnorm import alnorm
from smtxinv import smtxinv
from mdnord import mdnord
from PODaccessories import *
from PODglobals import *
from collections import OrderedDict
from new_pf import *
from betain import *
from gammds import *
from nrmden import *
from time import time
from FileLogger import *
import pyevent

#from decimal import *

class Compare ():
    @staticmethod
    def eq(a, b, tol):
        return math.fabs(a - b) < tol

    @staticmethod
    def lteq(a, b, tol):
        return math.fabs(a - b) < tol or a < b

    @staticmethod
    def gteq(a, b, tol):
        return math.fabs(a - b) < tol or a > b

    @staticmethod
    def lt(a, b, tol):
        return (not (math.fabs(a - b) < tol)) and a < b

    @staticmethod
    def gt(a, b, tol):
        return (not (math.fabs(a - b) < tol)) and a > b


class SolveIteration ():

    def __init__(self, trial, iteration, mu, sigma, fnorm, didDamp, dampValue):
        self.trial = trial
        self.iteration = iteration
        self.mu = mu
        self.sigma = sigma
        self.fnorm = fnorm
        if didDamp == True:
            self.dampValue = dampValue
        else:
            self.dampValue = 1.0

class CAnal ():
    'Holds a given analysis'
    def __init__(self, flaw, data):
        self.flaw = flaw
        self.crksize = flaw.crksize
        self.crkf = flaw.crkf
        self.ybar = flaw.ybar
        self.ybar_inv = flaw.ybar_inv
        if data.ahat_model == NONLINEAR_MODEL:
            self.yfit = data.linfit['intercept'] + data.linfit['slope'] \
                * self.crkf / (1.0 * data.linfit['rms'] * data.crkf)
        else:
            self.yfit = data.linfit['intercept'] + data.linfit['slope'] \
                * self.crkf
        self.yfit_inv = r_inv(self.yfit, data.ahat_transform)
        self.diff = self.ybar - self.yfit
        if data.linfit['rms'] != 0.0:
            self.dstar = alnorm(self.diff / data.linfit['rms'], False)
        else:
            self.dstar = 0.0
        self.censored = flaw.censored


    def __lt__(self,other):
        return self.crksize < other.crksize


    def __le__(self,other):
        return self.crksize <= other.crksize


    def __eq__(self,other):
        return self.crksize == other.crksize


    def __ne__(self,other):
        return self.crksize != other.crksize


    def __gt__(self,other):
        return self.crksize > other.crksize


    def __ge__(self,other):
        return self.crksize >= other.crksize


class CInspection():
    def __init__(self, value=0, flag=0, IsIncluded=True):
        self.value = value
        self.flag = flag
        self.IsIncluded = IsIncluded


class CrackData():
    'Holds info about a single crack'
    def __init__(self):
        self.sequence = 0
        self.crksize = 0  # Crack Size
        self.crkf = 0  # transformed crack size
        self.rdata = []  # Raw measurements
        self.nsets = 0  # number of measurements
        self.count = 0  # number of inspections showing

        #// PassFail analysis
        self.above = 0

        #// Ahat analysis
        self.ybar = 0      # mean transformed ahat
        self.ybar_inv = 0  # mean untransformed ahat
        self.yfit = 0      # predicted transformed ahat
        self.yfit_inv = 0  # predicted untransformed ahat
        self.scnt = 0      # censoring value
        self.ssy = 0       # censoring?
        self.ncensor = 0   # censoring?
        self.a_transform = TRANS_NONE  # default to no transform
        self.ahat_transform = TRANS_NONE  # default to no transfor
        
        self.dstar = 0.0
        self.sw = 0.0

        self.censored = False


    def __float__(self):
        return self.crksize


    def __lt__(self,other):
        return Compare.lt(self.crksize, other.crksize, .00000001)


    def __le__(self,other):
        return Compare.lte(self.crksize, other.crksize, .00000001)


    def __eq__(self,other):
        return Compare.eq(self.crksize, other.crksize, .00000001)


    def __ne__(self,other):
        return not Compare.eq(self.crksize, other.crksize, .00000001)


    def __gt__(self,other):
        return Compare.gt(self.crksize, other.crksize, .00000001)


    def __ge__(self,other):
        return Compare.gte(self.crksize, other.crksize, .00000001)


    def IsIncluded(self):  # Check to see if any measurement is uncensored
        included = [each.IsIncluded for each in self.rdata]
        return any(included)


    def crack_ahat_censor(self, data):
        i, j, n = 0, 0, 0
        value = 0
        acnt, bcnt, self.count = 0, 0, 0
        self.ncensor = 0
        for i in range(self.nsets):
            if not self.rdata[i].IsIncluded:
                self.rdata[i].flag = IF_Missing
                continue
            if not IsNumber(self.rdata[i].value)\
            or not IsNumber(self.crksize):
                self.rdata[i].flag = IF_Censored
                continue
            value = self.rdata[i].value
            self.count += 1
            if Compare.lteq(value, data.signalmin, .000001):
                bcnt += 1
                self.rdata[i].flag = IF_Below
                
            elif Compare.gteq(value, data.signalmax, .000001) and data.signalmax > data.signalmin:
                acnt += 1
                self.rdata[i].flag = IF_Above
                
            else:
                self.rdata[i].flag = IF_Okay
        # If there are no inspection result values, set a flag and return
        if (self.count == 0):  # This is an ERROR condition
            self.ncensor = 6
            self.censored = True
            return
        # If there are inspection result values both above the saturation
        # and below the recording threshold for the same flaw, notify the
        # user and continue.
        if (acnt != 0 and bcnt != 0):  # This is an ERROR condition
            self.ncensor = 5
            self.censored = True
            return
        cnt = acnt + bcnt
        self.rmin = r_fwd(data.signalmin, self.ahat_transform)
        self.rmax = r_fwd(data.signalmax, self.ahat_transform)
        # if all the inspection results were censored ...
        if cnt == self.count:
            if acnt != 0:
                self.ybar = self.rmax
                self.ybar_inv = data.signalmax
                self.ncensor = 1
                self.censored = True
            elif bcnt != 0:
                self.ybar = self.rmin
                self.ybar_inv = data.signalmin
                self.ncensor = 2
                self.censored = True
            return

        r, ysum, ysqr = 0, 0, 0
        n = 0.0
        for j in range(self.nsets):
            if self.rdata[j].flag != IF_Okay:
                continue
            r = r_fwd(self.rdata[j].value, self.ahat_transform)
            ysum += r
            ysqr += r * r
            n += 1.0

        self.ybar = ysum/n
        self.ybar_inv = r_inv(self.ybar, self.ahat_transform)

        # if no values have been censored ...
        if cnt == 0:
            if n == 1:
                self.scnt = 1
            else:
                self.scnt = n - 1
            self.ssy = (ysqr - n * self.ybar * self.ybar)
            return
        # if some are censored either above or below
        self.scnt, self.ssy = 0.0, 0.0
        if acnt != 0:  # some but not all were above the max

            c = self.rmax
            pstar = acnt / float(self.count)
            theta = phinv(pstar)
            thab = theta*(c-self.ybar)
            sigmar = (thab + sqrt(thab*thab + ((4.0/n)*(ysqr+(n*c-2.0*ysum)*c))))/2.0
            alpha = self.count*nrmden(theta)/float(n)
            self.ybar += alpha*sigmar

            self.ncensor = 3
        elif bcnt != 0:  # some but not all were below the min

            c = self.rmin
            pstar = bcnt / float(self.count)
            theta = phinv(pstar)
            thab = theta*(self.ybar-c)
            sigmar = (thab + sqrt(thab*thab + ((4.0/n)*(ysqr+(n*c-2.0*ysum)*c))))/2.0
            alpha = self.count*nrmden(theta)/float(n)
            self.ybar -= alpha*sigmar

            self.ncensor = 4
        else:  # This is an ERROR condition
            self.ncensor = 7
            self.censored = True
        self.ybar_inv = r_inv(self.ybar, self.ahat_transform)


    def crack_pf_censor(self, tmin):
        self.count = self.above = 0;
        for i in range(self.nsets):
            if self.rdata[i].IsIncluded:
                self.count += 1 
                if self.rdata[i].value>tmin:
                    self.above += 1
                self.rdata[i].flag = IF_Okay
            else:
                self.rdata[i].flag = IF_Missing


class PFData():
    def __init__(self):
        self.crksize = 0.0
        self.crkf = 0.0
        self.prob = 0.0  # // prob = above/count
        self.above = 0
        self.count = 0
        self.sequence = 0


    def __lt__(self,other):
        return self.crksize < other.crksize


    def __le__(self,other):
        return self.crksize <= other.crksize


    def __eq__(self,other):
        return self.crksize == other.crksize


    def __ne__(self,other):
        return self.crksize != other.crksize


    def __gt__(self,other):
        return self.crksize > other.crksize


    def __ge__(self,other):
        return self.crksize >= other.crksize


class CPodDoc():
    'This is the main analysis object'
    def __init__(self):
        self.podfile = None  # use this to hold the filename??
        # self.v OLEVariant - Used this to hold file descriptor
        self.info = []
        self.info_count = 0

        self.name = 'Default'

        # Info Worksheet access
        self.info_row = 0
        self.analysis = AHAT_ANALYSIS  # 0 = ahat, 1 = pass/fail
        self.model = 0  # 0 - linear, 1 = non-linear, 2 - odds
        self.flaw_name = ''
        self.flaw_column = 0
        self.flaw_units = ''
        self.data_column = 0
        self.data_units = ''
        self.signalmin = 0
        self.signalmax = 0

        self.z95 = 1.644853627 #=NORM.S.INV(0.95)
        self.z90 = 1.281551566 #=NORM.S.INV(0.90)

        # list of decision thresholds
        #GetAth(row)
        
        self.showMessages = False
        self.decision_thresholds = []
        self.decision_table_thresholds = []
        self.decision_a50 = []
        self.decision_level = []
        self.decision_confidence = []
        self.decision_V11 = []
        self.decision_V12 = []
        self.decision_V22 = []

        #Data worksheet
        self.titles = []
        #self.nrows = 0
        #self.ncols = 0
        self.flaws = []
        self.flaws_reload = [] #used for doing POD with no points missing
        self.responses_all = [] #used for doing POD with no points missing
        self.responses_missing = [] #used for doing POD with no points missing
        self.nsets = 0
        self.count = 0

        #Flaw range
        self.crkmin = 0
        self.crkmax = 0
        self.xmin = 0
        self.xmax = 0
        self.calcmin = 0 #flaw range to do calculations over like POD
        self.calcmax = 0

        #input to regression routine  (rlonx[nrows], rlony[nrows])
        self.rlonx = []  # transformed known flaw size
        self.rlony = []  # mean transformed measurements
        self.pfdata = []
        self.npts = 0

        # cracks with all inspections above saturation or below threshold
        self.acount = 0  # number of cases with all inspections at maximum
        self.bcount = 0  # number with all at minimum

        #cacks excluded from the analysis
        self.nexcluded = 0

        #POD parameter intial guess
        self.have_guess = False
        self.mu_guess = 0.0
        self.sig_guess = 0.0

        #POD model parameters
        self.sighat = 0.0
        self.muhat = 0.0
        self.mhat = 0.0
        self.a90 = 0.0
        self.m90 = 0.0
        self.m9095 = 0.0

        self.pod_all = 0.0
        self.threshold_all = 0.0
        self.a50_all = 0.0
        self.a90_all = 0.0
        self.a9095_all = 0.0

        #Confidence bounds settings  a9095 = crack size 95% confidence
        #(confidence_level) for 90% pod (pod_level = 0.9)
        self.pod_threshold = 0
        self.pod_level = 0.90
        self.confidence_level = .95
        self.asize = 'a90'  # e.g. a90
        self.alevel = 'a90/95'  # e.g. a90/95
        self.clevel = '95% confidence bound'  # e.g. 95% confidence bound
        
        self.regression = linear_regression()
        self.linfit = {'intercept':0, 'slope':0, 'rms':0}
        self.iret = 0
        self.fnorm = [0.0, 0.0, 0.0]
        self.variance = [[0, 0, 0], [0, 0, 0], [0, 0, 0]]
        self.res_sums = [0, 0, 0]
        self.fit_errors = [0, 0, 0]
        self.repeatability_error = 0
        
        self.pf_covariance = [0.0, 0.0, 0.0]

        self.analyses = []
        self.a_transform = TRANS_NONE
        self.ahat_transform = TRANS_NONE
        ##cbound(npts)
        
        ##Ahat Tests
        self.choices = ["P > 0.1", "0.05 < P <= 0.1", ".025 < P <= 0.05",
                        "0.01 < P <= .025", ".005 < P <= 0.01", "P <= .005", "Undefined"]
        self.astar = 0.0
        self.astar_p = 0.0
        self.astarRating = "" # choices rating for normality
        self.EV_Bartlett = 0.0
        self.EV_Bartlett_p = 0.0
        self.barlettRating = "" # choices rating for equal variance
        self.fcalc = 0.0
        self.fcalc_p = 0.0
        self.lackOfFitRating = "" # choices rating for lack of fit
        self.lackOfFitDegFrdm = 0 # degrees of freedom for the fit test
        self.lackOfFitCalculated = False
        
        self.pod_crksize = []
        self.pod_crksizef = []
        self.pod = []
        self.pod_bounds = []
        self.pod_bounds_crksize = []
        self.pod_bounds_crksizef = []

        self.pf_POD_a = []
        self.pf_POD_af = []
        self.pf_POD_poda = []
        self.pf_POD_conf95 = []
        self.pf_POD_ptxpt = []

        self.newPFGamma = 0.0
        self.newPFBetaodict = 0.0
        self.newPFPODLevel = 0.0

        self.new_pf_a = []
        self.new_pf_gamma = []
        self.new_pf_beta = []
        self.new_pf_pod = []
        self.new_pf_old = []

        self.g0 = []
        self.g1 = []

        self.pf_solve_params = []

        self.PlotTitle = ''

        #// Pass/Fail analysis
        self.pfit = []
        self.diff = []

        self.pf_test_crksum = 0.0
        self.pf_test_phat = 0.0
        self.pf_test_h0_likihood = 0.0
        self.pf_test_ha_likihood = 0.0
        self.pf_censor_test_result = 0.0
        self.pf_censor_test_pass = False

        self.OnNewProgress, self.SetNewProgress = pyevent.make_event()
        self.OnNewStatus, self.SetNewStatus = pyevent.make_event()
        self.OnNewError, self.AddNewError = pyevent.make_event()
        self.OnClearErrors, self.ClearErrors = pyevent.make_event()

        self.SortData()

        start_variance()

    def CountInsp(self, set):
        count = 0
        if set<0 or set>=self.nsets: 
            return 0
        for each in self.flaws:
            if not each.IsIncluded(): 
                continue
            if each.rdata[set].IsIncluded:
                count += 1
        return count


    def get_a_transform(self):
        if len(self.flaws) != 0:
            return self.flaws[0].a_transform
        else:
            return self.a_transform

    def SetAllMissingData(self, flaws, missingResponses, allResponses):
        self.flaws_reload = flaws
        self.responses_all = allResponses
        self.responses_missing = missingResponses

    def SetFlawData(self, flaws=[], transform=TRANS_NONE,  sequence_numbers=[]):  # TRANS_NONE = 1
        
        start_variance()
        
        self.a_transform = transform
        self.flaws = []
        for index in range(len(flaws)):
            crack = CrackData()
            if sequence_numbers == []:
                crack.sequence = index + 1
            else:
                crack.sequence = sequence_numbers[index]
                #print(crack.sequence)
            crack.crksize = float(flaws[index])
            crack.a_transform = transform
            crack.crkf = funcr(crack.crksize, crack.a_transform)
            if not IsNumber(crack.crkf):
                crack.bValid = False
                crack.censored = True
            self.flaws += [crack]


    def SetResponseData(self, responses={}, transform=TRANS_NONE):
        'Sets response data for already existing flaws objects'
        newflaws = []  # Make a new array to hold flaws in case user has turned a crack off
        self.meas = responses
        self.ahat_transform = transform
        self.titles = responses.keys()
        for index in range(len(self.flaws)):
            # Get the flaw with the matching index
            crack = self.flaws[index]
            #clear the measurements
            crack.rdata = []
            # Iterate through the dictionary getting each value for this index
            for key in responses.keys():
                value = float(responses[key][index])
                # Check to see if the value is censored
                if IsNumber(value):
                    flag = IF_Okay
                else:
                    flag = IF_Censored
                    
                an_inspection = CInspection(value, flag, IsNumber(value))
                crack.rdata += [an_inspection]
            crack.ahat_transform = transform
            crack.nsets = len(crack.rdata)
            included = [each for each in crack.rdata if each.IsIncluded]
            crack.count = len(included)
            if IsNumber(crack.crksize) and crack.count != 0:
                newflaws += [crack]

            #we want the biggest set size
            if self.nsets < crack.nsets:
                self.nsets = crack.nsets
            
        self.flaws = newflaws
        self.npts = len(newflaws)


    def SetResponseMin(self, value=float('NaN')):
        'Sets untransformed response minimum to "value"'
        if not IsNumber(value):
            self.signalmin = min(r_inv(each.ybar, self.ahat_transform) for each in self.flaws)
        else:
            self.signalmin = value


    def SetResponseMax(self, value=float('NaN')):
        'Sets untransformed response maximum to "value"'
        if not IsNumber(value):
            self.signalmax = max(r_inv(each.ybar, self.ahat_transform) for each in self.flaws)
        else:
            self.signalmax = value


    def SetThresholdPlotRange(self, minimum=float('NaN'),
        maximum=float('NaN')):
        'I don\'t know what to do with this'
        return -1


    def SetPODThreshold(self, value=float('NaN')):
        'Sets the POD threshold'
        if not IsNumber(value):
            #if have a threshold list
            if len(self.decision_thresholds) != 0:
                self.pod_threshold = self.decision_thresholds[0]
            else:
                # Pick something that makes sense here?
                self.pod_threshold = r_inv(min(self.flaws), self.ahat_transform)
        else:
            self.pod_threshold = value
    

    def SetDecisionThresholds(self, value=[]):
        'Sets the decision threshold'
        if value == []:
            # Pick something that makes sense here?
            self.decision_thresholds = [self.signalmin]
        else:
            self.decision_thresholds = value


    def SetPODLevel(self, value=float('NaN')):
        'Sets the Probability Of Detection Level'
        if not IsNumber(value):
            self.pod_level = 0.9
        else:
            self.pod_level = value


    def SetPODConfidence(self, value=float('NaN')):
        'Sets the Probability Of Detection confidence level'
        if not IsNumber(value):
            self.confidence_level = .95
        else:
            self.confidence_level = value

    def GetFlawRangeMax(self):
        'Returns the max, transformed, uncensored flaw size'
        flawCount = len(self.flaws)

        if flawCount > 0:            
            maxList = [flaw.crksize for flaw in self.flaws if flaw.count > 0]

            if len(maxList) > 0:
                maxValue = max(maxList)
                maxValue = self.GetTransformedFlawValue(maxValue)
            else:
                maxValue = 1.0    

            return maxValue
        else:
            return 1.0

    def SetCalcMin(self, value):

        #only test if values have been set beyound initialization
        if value > self.calcmax and self.calcmax != self.calcmin:
            temp = value
            value = self.calcmax
            self.calcmax = temp

        if value < 0.0:
            value = 0.0

        self.calcmin = value

    def SetCalcMax(self, value):

        #only test if values have been set beyound initialization
        if value < self.calcmin and self.calcmax != self.calcmin:
            temp = value
            value = self.calcmin
            self.calcmin = temp

        if value < 0.0:
            value = 1.0

        self.calcmax = value

    def RegisterUpdateProgressEvent(self, eventholder):

        self.OnNewProgress += eventholder.UpdateProgress
        self.OnNewStatus += eventholder.UpdateStatus
        self.OnClearErrors += eventholder.ClearErrors
        self.OnNewError += eventholder.AddError

    def DeregisterUpdateProgressEvent(self, eventholder):

        self.OnNewProgress -= eventholder.UpdateProgress
        self.OnNewStatus -= eventholder.UpdateStatus
        self.OnClearErrors -= eventholder.ClearErrors
        self.OnNewError -= eventholder.AddError

    def GetUncensoredFlawRangeMax(self):
        'Returns the max, transformed, uncensored flaw size'
        flawCount = len(self.flaws)

        if flawCount > 0:            
            maxList = [flaw.crksize for flaw in self.flaws if not flaw.censored]

            if len(maxList) > 0:
                maxValue = max(maxList)
                maxValue = self.GetTransformedFlawValue(maxValue)
            else:
                maxValue = 1.0    

            return maxValue
        else:
            return 1.0

    def GetFlawRangeMin(self):
        'Returns the min, transformed, uncensored flaw size'
        flawCount = len(self.flaws)

        if flawCount > 0:
            minList = [flaw.crksize for flaw in self.flaws if flaw.count > 0]

            if len(minList) > 0:
                minValue = min(minList)
                minValue = self.GetTransformedFlawValue(minValue)
            else:
                minValue = .000001            

            return minValue
        else:
            return 0.000001

    def GetUncensoredFlawRangeMin(self):
        'Returns the min, transformed, uncensored flaw size'
        flawCount = len(self.flaws)

        if flawCount > 0:
            minList = [flaw.crksize for flaw in self.flaws if not flaw.censored]

            if len(minList) > 0:
                minValue = min(minList)
                minValue = self.GetTransformedFlawValue(minValue)
            else:
                minValue = .000001            

            return minValue
        else:
            return 0.000001


    def GetAnalyzedFlawCount(self):
        'Counts the uncensored flaws'
        return len([flaw for flaw in self.flaws if not flaw.censored])
    
    
    def GetFlawCountPartialBelowResponseMin(self):
        'Counts the uncensored flaws with only some measurements below min'
        return len([flaw for flaw in self.flaws if (flaw.ncensor == 4 and not flaw.censored)])


    def GetFlawCountPartialAboveResponseMax(self):
        'Counts the uncensored flaws with only some measurements below min'
        return len([each for each in self.flaws if (each.ncensor == 3 and not each.censored)])


    def GetFlawCountFullBelowResponseMin(self):
        'Counts the flaws with all measurements at or below min'
        return len([each for each in self.flaws if each.ncensor == 2])


    def GetFlawCountFullAboveResponseMax(self):
        'Counts the flaws with all measurements at or above max'
        return len([each for each in self.flaws if each.ncensor == 1])


    def GetFlawCountUncensored(self):
        'Counts the flaws with all measurements at or above max'
        return len([each for each in self.flaws if each.ncensor == 0])


    def GetModelIntercept(self):
        'intercept of transformed data linear regression'
        if abs(self.linfit['intercept']) > 100000:
            self.linfit['intercept'] = 0.0

        return self.linfit['intercept']


    def GetModelSlope(self):
        'slope of transformed data linear regression'
        if abs(self.linfit['slope']) > 100000:
            self.linfit['slope'] = 0.0

        return self.linfit['slope']


    def GetModelResidual(self):
        'This is the RMS error or Standard Error for the whole fit'
        if abs(self.linfit['rms']) > 100000:
            self.linfit['rms'] = 0.0

        return self.linfit['rms']


    def GetModelInterceptError(self):
        return self.fit_errors[0]  # Need to talk to Pete about this


    def GetModelSlopeError(self):
        return self.fit_errors[1]  # Need to talk to Pete about this


    def GetModelResidualError(self):
        return self.fit_errors[2]  # Need to talk to Pete about this


    def GetRepeatabilityError(self):
        return self.repeatability_error  # Need to talk to Pete about this


    def GetNormality(self):
        return self.astar  # Need to talk to Pete about this

    #do not have the astar p value
    def GetNormality_p(self):
        return self.astar_p  # Need to talk to Pete about this

    def GetNormalityRating(self):
        return self.astarRating  # Need to talk to Pete about this


    def GetEqualVariance(self):
        return self.EV_Bartlett  # Need to talk to Pete about this

    def GetEqualVariance_p(self):
        return self.EV_Bartlett_p  # Need to talk to Pete about this

    def GetEqualVarianceRating(self):
        return self.barlettRating  # Need to talk to Pete about this

    def GetLackOfFit(self):
        return self.fcalc  # Need to talk to Pete about this

    def GetLackOfFit_p(self):
        return self.fcalc_p  # Need to talk to Pete about this

    def GetLackOfFitRating(self):
        return self.lackOfFitRating  # Need to talk to Pete about this

    def GetLackOfDegreesFreedom(self):
        return self.lackOfFitDegFrdm  # Need to talk to Pete about this

    def GetLackOfFitCalculated(self):
        return self.lackOfFitCalculated

    def GetPODSigma(self):
        return self.sighat # Need to talk to Pete about this


    def GetPODMu(self):
        return self.muhat  # Need to talk to Pete about this


    def GetPODFlaw50(self):
        return self.mhat  # Need to talk to Pete about this


    def GetPODFlawLevel(self):
        return self.m90  # Need to talk to Pete about this


    def GetPODFlawConfidence(self):
        return self.m9095  # Need to talk to Pete about this


    def GetRawResidualTable(self): # flaw, t_flaw, response, t_response, t_fit, t_diff

        odict = dict() #OrderedDict()
        
        flawList = []
        t_flawList = []
        respList = []
        t_respList = []
        t_fitList = []
        t_diff = []

        for flaw in self.flaws:
            if not flaw.censored:
                for value in flaw.rdata:

                    #keep resp in range
                    resp =value.value
                    if resp < self.signalmin:
                       resp = self.signalmin
                    elif resp > self.signalmax:
                        resp = self.signalmax

                    #resp =value.value

                    #if resp > self.signalmin and resp < self.signalmax:

                    flawList.append(flaw.crksize)
                    t_flawList.append(flaw.crkf)
                    
                    respList.append(resp)
                    t_resp = r_fwd(resp, self.ahat_transform)
                    t_respList.append(t_resp)
                    t_fit = flaw.crkf * self.linfit['slope'] + self.linfit['intercept']
                    t_fitList.append(t_fit)
                    t_diff.append(t_resp - t_fit)


        odict['list_names'] = ['flaw', 'response', 't_flaw', 't_response', 't_fit', 't_diff']
        odict['flaw'] = flawList
        odict['response'] = respList
        odict['t_flaw'] = t_flawList
        odict['t_response'] = t_respList
        odict['t_fit'] = t_fitList
        odict['t_diff'] = t_diff

        return odict

    def GetResidualTable(self): # flaw, t_flaw, ave_response, t_ave_response, t_fit, t_diff [Residuals A2]

        odict = dict() #OrderedDict()
        
        odict['list_names'] = ['flaw', 'ave_response', 't_flaw', 't_ave_response', 't_fit', 't_diff']
        odict['flaw'] = [flaw.crksize for flaw in self.flaws if not flaw.censored]
        odict['ave_response'] = [flaw.ybar_inv for flaw in self.flaws if not flaw.censored]
        odict['t_flaw'] = [flaw.crkf for flaw in self.flaws if not flaw.censored]        
        odict['t_ave_response'] = [flaw.ybar for flaw in self.flaws if not flaw.censored]
        odict['t_fit'] = [flaw.crkf * self.linfit['slope'] + self.linfit['intercept'] 
            for flaw in self.flaws if not flaw.censored]
        odict['t_diff'] = [flaw.ybar - (flaw.crkf * self.linfit['slope'] + self.linfit['intercept']) 
            for flaw in self.flaws if not flaw.censored]
        #odict['SEmean'] = self.regression.SEmean
        #odict['SEpred'] = self.regression.SEpred

        return odict

    def GetPFResidualTable(self):
        odict = dict() #OrderedDict()
       #[pfd.crksize, pfd.prob, pfd.crkf, pfTest.pfit[i], pfd.prob-pfTest.pfit[i]]
        
        odict['list_names'] = ['flaw', 't_flaw', 'hitrate', 't_fit', 'diff']
        odict['flaw'] = [flaw.crksize for flaw in self.pfdata]
        odict['t_flaw'] = [flaw.crkf for flaw in self.pfdata]
        odict['hitrate'] = [flaw.prob for flaw in self.pfdata]
        odict['t_fit'] = self.pfit
        odict['diff'] = self.diff 
        
        return odict
        

    


    def GetCensoredTable(self): # flaw, t_flaw, ave_response, t_ave_response, t_fit, t_diff [Residuals A2]
        
        odict = dict() #OrderedDict()

        odict['list_names'] = ['flaw', 't_flaw', 'ave_response', 't_ave_response', 't_fit', 't_diff']
        odict['flaw'] = [flaw.crksize for flaw in self.flaws if flaw.censored]
        odict['t_flaw'] = [flaw.crkf for flaw in self.flaws if flaw.censored]
        odict['ave_response'] = [flaw.ybar_inv for flaw in self.flaws if flaw.censored]
        odict['t_ave_response'] = [flaw.ybar for flaw in self.flaws if flaw.censored]
        odict['t_fit'] = [flaw.crkf * self.linfit['slope'] + self.linfit['intercept'] for flaw in self.flaws if flaw.censored]
        odict['t_diff'] = [flaw.ybar - (flaw.crkf * self.linfit['slope'] + self.linfit['intercept']) for flaw in self.flaws if flaw.censored]
        
        return odict

    def GetFullCensoredTable(self): # flaw, t_flaw, ave_response, t_ave_response, t_fit, t_diff
        
        odic = dict() #OrderedDict()

        odic['list_names'] = ['flaw', 't_flaw', 'ave_response', 't_ave_response', 't_fit', 't_diff']
        odic['flaw'] = [flaw.crksize for flaw in self.flaws if (flaw.ncensor == 1 or flaw.ncensor == 2)]
        odic['t_flaw'] = [flaw.crkf for flaw in self.flaws if (flaw.ncensor == 1 or flaw.ncensor == 2)]
        odic['ave_response'] = [flaw.ybar_inv for flaw in self.flaws if (flaw.ncensor == 1 or flaw.ncensor == 2)]
        odic['t_ave_response'] = [flaw.ybar for flaw in self.flaws if (flaw.ncensor == 1 or flaw.ncensor == 2)]
        odic['t_fit'] = [flaw.crkf * self.linfit['slope'] + self.linfit['intercept'] for flaw in self.flaws if (flaw.ncensor == 1 or flaw.ncensor == 2)]
        odic['t_diff'] = [flaw.ybar - (flaw.crkf * self.linfit['slope'] + self.linfit['intercept']) for flaw in self.flaws if (flaw.ncensor == 1 or flaw.ncensor == 2)]
        
        return odic

    def GetPartialCensoredTable(self): # flaw, t_flaw, ave_response, t_ave_response, t_fit, t_diff
        
        odic = dict() #OrderedDict()

        odic['list_names'] = ['flaw', 't_flaw', 'ave_response', 't_ave_response', 't_fit', 't_diff']
        odic['flaw'] = [flaw.crksize for flaw in self.flaws if not flaw.censored and (flaw.ncensor == 4 or flaw.ncensor == 3)]
        odic['t_flaw'] = [flaw.crkf for flaw in self.flaws if not flaw.censored and (flaw.ncensor == 4 or flaw.ncensor == 3)]
        odic['ave_response'] = [flaw.ybar_inv for flaw in self.flaws if not flaw.censored and (flaw.ncensor == 4 or flaw.ncensor == 3)]
        odic['t_ave_response'] = [flaw.ybar for flaw in self.flaws if not flaw.censored and (flaw.ncensor == 4 or flaw.ncensor == 3)]
        odic['t_fit'] = [flaw.crkf * self.linfit['slope'] + self.linfit['intercept'] for flaw in self.flaws if not flaw.censored and (flaw.ncensor == 4 or flaw.ncensor == 3)]
        odic['t_diff'] = [flaw.ybar - (flaw.crkf * self.linfit['slope'] + self.linfit['intercept']) for flaw in self.flaws if not flaw.censored and (flaw.ncensor == 4 or flaw.ncensor == 3)]
        
        return odic
    
    def GetPFEstimatedCovarianceMatrix(self):
        matrix = []

        matrix.append(self.pf_covariance[0])
        matrix.append(self.pf_covariance[1])
        matrix.append(self.pf_covariance[2])

        return matrix

    def GetFitTable(self): #flaw, fit, <name of response col1>, <name of response col  2>,?<name of response column X>
        'Returns the curve fit and (new) bounds'
        mydict = dict() #OrderedDict()

        names = ['flaw', 'fit']
        mydict['flaw'] = [flaw.crksize for flaw in self.flaws if not flaw.censored]
        #mydict['t_fit'] = [flaw.crkf * self.linfit['slope'] + self.linfit['intercept']
        #    for flaw in self.flaws if not flaw.censored]
        
        mydict['fit'] = [r_inv(flaw.crkf * self.linfit['slope']
            + self.linfit['intercept'],self.ahat_transform)
            for flaw in self.flaws if not flaw.censored]

        for key in self.titles:
            names.append(key)
            mydict[key] = [flaw.rdata[self.titles.index(key)].value
            for flaw in self.flaws if not flaw.censored]
            
        mydict['list_names'] = names
        
        return mydict
    

    def GetPODTable(self): #PODTable() #flaw, pod, confidence
        'GetPODTable() #flaw, pod, confidence'
        odict = dict() #OrderedDict()

        odict['list_names'] = ['flaw', 'pod', 'confidence']
        odict['flaw'] = self.pod_crksize
        #odict['t_flaw'] = self.pod_crksizef
        odict['pod'] = self.pod
        #odict['confidenceFlawAtPOD'] = self.pod_bounds_crksize
        odict['confidence'] = self.pod_bounds
        
        return odict

    def GetTotalFlawCount(self):

        sum = 0
        for each in self.flaws:
            #if each.bValid == True and each.censored == False:
            sum += 1

        return sum

    def GetPFPODTable(self):
        odict = dict() #OrderedDict()

        ##NEW TABLE
        ##############################################################################################
        #  TRB - 03-10-2017, removed old confidence flaw and old confidence pod because it doesn't   #
        #  actually match what PODv3 says are the values, right now it is better for                 #
        #  users to run POD v3 themselves and do a manual comparison                                 #
        ##############################################################################################        
        odict['list_names'] = ['flaw', 'pod', 'confidence pod']#, 'old confidence flaw', 'old confidence pod']
        odict['flaw'] = self.pf_POD_a
        odict['pod'] = self.pf_POD_poda
        #odict['confidence flaw'] = self.new_pf_a
        odict['confidence pod'] = self.new_pf_pod
        #odict['gamma'] = self.new_pf_gamma
        #odict['beta'] = self.new_pf_beta
        
        ##OLD TABLE
        ##odict['flaw'] = self.pf_POD_a
        ##odict['pod'] = self.pf_POD_poda

        ##############################################################################################
        #  TRB - 03-10-2017, removed old confidence flaw and old confidence pod because it doesn't   #
        #  actually match what PODv3 says are the values, right now it is better for                 #
        #  users to run POD v3 themselves and do a manual comparison                                 #
        ##############################################################################################        
        #odict['old confidence flaw'] = self.pf_POD_ptxpt
        #odict['old confidence pod'] = self.pf_POD_poda

        #odict['flaw'] = self.pf_POD_a
        #odict['t_flaw'] = self.pf_POD_af
        #odict['pod'] = self.pf_POD_poda
        #odict['confidence flaw'] = self.pf_POD_ptxpt
        #odict['confidence pod'] = self.pf_POD_poda
        #odict['unknown confidence'] = self.pf_POD_conf95
        
        return odict

    def GetNewPFBeta(self):
        return self.newPFBeta

    def GetNewPFGamma(self):
        return self.newPFGamma

    def GetNewPFPODLevel(self):
        return self.newPFPODLevel

    def AddErrorLine(self, line):
        
        if self.showMessages == True:
            self.AddNewError(self, line)

    def GetNewPFTable(self, b=-1, gamma=-1):

        odict = dict() #OrderedDict()       
        odict['list_names'] = ['seq #', 'Depth', 'Ins 1', 'g[0]', 'g[1]']
        odict['seq #'] = [flaw.sequence for flaw in self.flaws]
        odict['Depth'] = [flaw.crksize for flaw in self.flaws]
        odict['Ins 1'] = [flaw.count for flaw in self.flaws]
        odict['g[0]'] = self.g0
        odict['g[1]'] = self.g1

        #Write_Ordered_Dict(odict, "GetNewPFTable", 929)
        
        return odict

    def CalcNewPFPOD(self):

        self.new_pf_a = []
        self.new_pf_gamma = []
        self.new_pf_beta = []
        self.new_pf_pod = []
        self.new_pf_old = []

        self.SetNewProgress(self, int(5.0))

        new_bounds = pf_new_bounds(self)

        gamma = self.x90
        b = 1/(2*self.sighat)
        cov = self.pf_covariance
        #DEBUG = False
        #podpoints = [50.0, 55.0, 60.0, 65.0, 70.0, 75.0, 80.0, 82.0, 84.0, 86.0, 88.0, 90.0, 91.0, 92.0, 93.0, 94.0, 95.0, 96.0, 97.0, 98.0, 99.0, 99.1, 99.2 , 99.3, 99.4, 99.5]
        podpoints = [.5, 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 12.5, 15.0, 17.5, 20.0, 22.5, 25.0, 30.0, 35.0, 40.0, 45.0, 50.0, 55.0, 60.0, 65.0, 70.0, 75.0, 77.5, 80.0, 82.0, 84.0, 86.0, 88.0, 90.0, 91.0, 92.0, 93.0, 94.0, 95.0, 96.0, 97.0, 98.0, 98.5, 99.0, 99.25, 99.5]
        #podpoints = [25.0, 30.0, 35.0, 40.0, 45.0, 50.0, 55.0, 60.0, 65.0, 70.0, 75.0, 77.5, 80.0, 82.0, 84.0, 86.0, 88.0, 90.0, 91.0, 92.0, 93.0, 94.0, 95.0, 96.0, 97.0, 98.0, 98.5, 99.0, 99.25, 99.5]
        #podpoints = [80.0, 85.0, 90.0]
        
        firstPOD = self.pf_POD_poda[1] * 100.0
        lastPOD = self.pf_POD_poda[len(self.pf_POD_poda)-1] * 100.0
        guessed = False

        self.m9095 = 0.0

        if new_bounds.seriousIssue == True:
            self.AddErrorLine('Encountered issue while initializing the 95 curve.')
            self.AddErrorLine('No points calculated.')
            self.AddErrorLine("Consult a statistician if necessary.");
            self.m9095 = 0.0
            return

        for i in podpoints:

            #if the original POD curve didn't have anything don't do anything for the 95 curve
            if i >= firstPOD and i <= lastPOD or i == 90.0:

                self.SetNewStatus(self, 'Calculating POD(a) = {0}'.format(i))
                self.SetNewProgress(self, int(i))

                time_start = time()

                if not guessed:
                    b = 1/(2*self.sighat)
                    #X = self.muhat + new_bounds.zXX(i/100.0)*self.sighat
                    Zk = new_bounds.zXX(i/100.0)
                    X = self.muhat+Zk*self.sighat+self.z95*sqrt(cov[0]+Zk*(2.0*cov[1]+Zk*cov[2]));
                    gamma = X
                    guessed = True
                else:
                    paramCount = len(self.new_pf_gamma)

                    if paramCount >= 2:

                        if i != 99.5:
                            dPOD = (podpoints[paramCount-1] - podpoints[paramCount-2]) / (podpoints[paramCount] - podpoints[paramCount-1])
                        else:
                            dPOD = .5

                        dgamma = (self.new_pf_gamma[paramCount-1] - self.new_pf_gamma[paramCount-2]) / dPOD
                        dbeta = (self.new_pf_beta[paramCount-1] - self.new_pf_beta[paramCount-2]) / dPOD
                        gamma = gamma + dgamma
                        b = b + dbeta    
               
            
                old = a_inv(self.muhat + new_bounds.zXX(i/100.0)*self.sighat,  self.a_transform)
                (gamma, b) = new_bounds.solve(i/100.0,  gamma,  b, self.muhat,  self.sighat)
                a = a_inv(gamma,self.a_transform)
                tries = 0

                

                while a < old:
                    delta = (old - a)
                    delta = delta + (a / (100.0-i) * tries)
                    gammanew = a_fwd(old+delta, self.a_transform)  
                    betanew = b / 2    
                                          
                    (gamma, b) = new_bounds.solve(i/100.0,  gammanew,  betanew, self.muhat,  self.sighat)
                    a = a_inv(gamma,self.a_transform)
                    tries = tries + 1

                    if tries > 10:
                        return

                if new_bounds.seriousIssue == True:
                    self.AddErrorLine('Encountered issue with calculating the 95 curve for the {0} percentile.'.format(i))
                    self.AddErrorLine('Calculated flaw size = {0}.'.format(a))
                    self.AddErrorLine("Consult a statistician if necessary.");
                    self.m9095 = 0.0
                    break

                if i == 90:
                    self.m9095 = a

                time_end = time() - time_start

                #write_list_params([i, time_end], "Calc Time", "CalcNewPFPOD", 1033)

                self.new_pf_a.append(a)
                self.new_pf_gamma.append(gamma)
                self.new_pf_beta.append(b)
                self.new_pf_pod.append(i/100.0)
                self.new_pf_old.append(old)

    #guess using previous beta/gamma
    #def CalcNewPFPOD(self):

    #    self.new_pf_a = []
    #    self.new_pf_gamma = []
    #    self.new_pf_beta = []
    #    self.new_pf_pod = []
    #    self.new_pf_old = []

    #    new_bounds = pf_new_bounds(self)
    #    gamma = self.x90
    #    b = 1/(2*self.sighat)
    #    #DEBUG = False
    #    for i in range(5,100):
    #        if i == 5:
    #            b = 1/(2*self.sighat)
    #            X = self.muhat + new_bounds.zXX(i/100.0)*self.sighat
    #            gamma = X
            
    #        old = a_inv(self.muhat + new_bounds.zXX(i/100.0)*self.sighat,  self.a_transform)
    #        (gamma, b) = new_bounds.solve(i/100.0,  gamma,  b, self.muhat,  self.sighat)
    #        a = a_inv(gamma,self.a_transform)
    #        tries = 0
    #        while a < old:
    #            delta = (old - a)
    #            a = old + delta
    #            delta = delta + (a / (100.0-i) * tries)
    #            gammanew = a_fwd(old+delta, self.a_transform)                        
    #            (gamma, b) = new_bounds.solve(i/100.0,  gammanew,  b/2, self.muhat,  self.sighat)
    #            a = a_inv(gamma,self.a_transform)
    #            tries = tries + 1
    #        #else:
    #            #gammanew = gamma

    #        self.new_pf_a.append(a)
    #        self.new_pf_gamma.append(gamma)
    #        self.new_pf_beta.append(b)
    #        self.new_pf_pod.append(i/100.0)
    #        self.new_pf_old.append(old)

    #always guess with POD
    #def CalcNewPFPOD(self):

    #    self.new_pf_a = []
    #    self.new_pf_gamma = []
    #    self.new_pf_beta = []
    #    self.new_pf_pod = []
    #    self.new_pf_old = []

    #    new_bounds = pf_new_bounds(self)
    #    gamma = self.x90
    #    b = 1/(2*self.sighat)
    #    #DEBUG = False
    #    for i in range(5,100):
    #        #if i == 5:
    #        b = 1/(2*self.sighat)
    #        X = self.muhat + new_bounds.zXX(i/100.0)*self.sighat
    #        gamma = X
            
    #        old = a_inv(self.muhat + new_bounds.zXX(i/100.0)*self.sighat,  self.a_transform)
    #        (gamma, b) = new_bounds.solve(i/100.0,  gamma,  b, self.muhat,  self.sighat)
    #        a = a_inv(gamma,self.a_transform)
    #        tries = 0
    #        while a < old:
    #            delta = (old - a)
    #            a = old + delta
    #            delta = delta + (a / (100.0-i) * tries)
    #            gammanew = a_fwd(old+delta, self.a_transform)                        
    #            (gamma, b) = new_bounds.solve(i/100.0,  gammanew,  b/2, self.muhat,  self.sighat)
    #            a = a_inv(gamma,self.a_transform)
    #            tries = tries + 1
    #        #else:
    #            #gammanew = gamma

    #        self.new_pf_a.append(a)
    #        self.new_pf_gamma.append(gamma)
    #        self.new_pf_beta.append(b)
    #        self.new_pf_pod.append(i/100.0)
    #        self.new_pf_old.append(old)
    
    def GetThresholdTable(self):
        '#threshold value, flaw50, level, confidence'
        odict = dict() #OrderedDict()

        odict['list_names'] = ['threshold', 'level', 'confidence', 'flaw50', 'V11', 'V12', 'V22']
        odict['threshold'] = self.decision_table_thresholds        
        odict['level'] = self.decision_level
        odict['confidence'] = self.decision_confidence
        odict['flaw50'] = self.decision_a50
        odict['V11'] = self.decision_V11
        odict['V12'] = self.decision_V12
        odict['V22'] = self.decision_V22
        return odict
        #return{'threshold':self.decision_table_thresholds, 
        #'flaw50':self.decision_a50, 
        #'level':self.decision_level, 
        #'confidence':self.decision_confidence}

    def GetPFSolveIterationTable(self):
        #trial index, iteration index, mu, sigma, damping 
        odict = dict() #OrderedDict()

        odict['list_names'] = ['trial index', 'iteration index', 'mu', 'sigma', 'fnorm', 'damping']
        odict['trial index'] = [obj.trial for obj in self.pf_solve_params]
        odict['iteration index'] = [obj.iteration for obj in self.pf_solve_params]
        odict['mu'] = [obj.mu for obj in self.pf_solve_params]
        odict['sigma'] = [obj.sigma for obj in self.pf_solve_params]
        odict['fnorm'] = [obj.fnorm for obj in self.pf_solve_params]
        odict['damping'] = [obj.dampValue for obj in self.pf_solve_params]

        return odict
    
    def GetPlotMinFlaw(self): 
        '[Residuals G2]'
        return min([each.crksize for each in self.flaws])


    def GetPlotMaxFlaw(self): 
        '[Residuals G3]'
        return max([each.crksize for each in self.flaws])


    def GetPlotMinFlawMinResponse(self): 
        '[Residuals H2]'
        return self.signalmin


    def GetPlotMinFlawMaxResponse(self): 
        '[Residuals I2]'
        return self.signalmax


    def GetPlotMaxFlawMinResponse(self): 
        '[Residuals H3]'
        return self.signalmin


    def GetPlotMaxFlawMaxResponse(self): 
        '[Residuals I3]'
        return self.signalmax


    def GetFitMinFlaw(self): 
        '[Residuals G6]'
        return min([each.crksize for each in self.flaws if not each.censored])


    def GetFitMaxFlaw(self): 
        '[Residuals G7]'
        return max([each.crksize for each in self.flaws if not each.censored])


    def GetFitMinResponse(self): 
        '[Residuals H6]'
        return min([each.ybar_inv for each in self.flaws if not each.censored])

    def GetFitMaxResponse(self): 
        '[Residuals H7]'
        return max([each.ybar_inv for each in self.flaws if not each.censored])


    def get_ahat_transform(self):
        if len(self.flaws) != 0:
            return self.flaws[0].ahat_transform
        else:
            return self.ahat_transform


    def TransformData(self, list, transformType):
        
        for index in range(len(list)):
            list[index] = funcr(list[index], transformType)


    def GetInvtransformedResponseValue(self, value):

        return r_inv(value, self.ahat_transform)

    def GetInvtransformedFlawValue(self, value):

        return a_inv(value, self.a_transform)

    def GetTransformedResponseValue(self, value):

        return r_fwd(value, self.ahat_transform)

    def GetTransformedValue(self, value, transform):

        return a_fwd(value, transform)

    def GetInvtTransformedValue(self, value, transform):

        return a_inv(value, transform)

    def GetTransformedFlawValue(self, value):

        return a_fwd(value, self.a_transform)


    def AHAT2(self):
        # lcrk = [0.2, 1.1, 1.1, 1.6, 2, 2.2, 2.2, 2.4, 2.4, 2.7, 2.9, 2.9, 2.9,
        #    2.9, 3.1, 3.3, 3.3, 3.3, 3.5, 3.7, 4.6, 4.6, 5.1, 5.1, 5.1, 5.5,
        #    5.5, 4.6, 5.9, 5.9, 5.5, 6.4, 6.4, 6.8, 6.8, 7.3, 7.3, 7.7, 7.7,
        #    8.1, 8.6, 9, 9, 8.8, 9.5, 9.9, 9.9, 9.9, 10.8, 11.2, 11.7, 11.7,
        #    12.5, 12.1, 13, 13.4, 13.9, 14.3, 15.6, 16.1]
        self.SetFlawData([0.2, 1.1, 1.1, 1.6, 2, 2, 2.2, 2.2, 2.4, 2.4, 2.7, 2.9, 2.9, 2.9,
            2.9, 3.1, 3.3, 3.3, 3.3, 3.5, 3.7, 4.6, 4.6, 5.1, 5.1, 5.1, 5.5,
            5.5, 4.6, 5.9, 5.9, 5.5, 6.4, 6.4, 6.8, 6.8, 7.3, 7.3, 7.7, 7.7,
            8.1, 8.6, 9, 9, 8.8, 9.5, 9.9, 9.9, 9.9, 10.8, 11.2, 11.7, 11.7,
            12.5, 12.1, 13, 13.4, 13.9, 14.3, 15.6, 16.1],  XF_LOG)
        
        self.SetFlawData([float('NaN'), float('NaN'), float('NaN'), float('NaN'), float('NaN'), float('NaN'), float('NaN'), 
            2.2, 2.4, 2.4, 2.7, 2.9, 2.9, 2.9,
            2.9, 3.1, 3.3, 3.3, 3.3, 3.5, 3.7, 4.6, 4.6, 5.1, 5.1, 5.1, 5.5,
            5.5, 4.6, 5.9, 5.9, 5.5, 6.4, 6.4, 6.8, 6.8, 7.3, 7.3, 7.7, 7.7,
            8.1, 8.6, 9, 9, 8.8, 9.5, 9.9, 9.9, 9.9, 10.8, 11.2, 11.7, 11.7,
            12.5, 12.1, 13, 13.4, 13.9, 14.3, 15.6, 16.1],  XF_LOG)

        self.SetResponseData({
        'A11': [35, 35, 35, 35, 35, 35, 35,  35, 35, 35, 35, 35, 35, 35, 35, 35, 35,
            35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 41, 35, 35, 60, 57,
            76, 55, 90, 65, 78, 70, 85, 107, 131, 113, 140, 111, 227, 128, 183,
            215, 185, 203, 210, 210, 233, 201, 286, 286, 321, 385, 366],
        'A21':[35, 35, 35, 35, 35, 35,  35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35,
            35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 42, 41, 35, 65, 62,
            80, 55, 95, 67, 78, 77, 92, 121, 136, 112, 145, 112, 231, 124, 189,
            230, 187, 205, 212, 210, 198, 230, 308, 289, 309, 342, 376],
        'A12':[35, 35, 35, 35, 35, 35, 35,  35, 35, 35, 35, 35, 35, 35, 35, 35, 35,
            35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 44, 39, 35, 61, 61,
            81, 55, 92, 65, 80, 86, 88, 114, 135, 113, 142, 111, 228, 128, 184,
            226, 184, 208, 212, 211, 242, 277, 314, 295, 317, 349, 388]}, XF_LOG)
        #self.signalmin = 35
        #self.signalmax = 4095
        #self.amax = 30


    def ideal(self):
        lcrk = [7, 8, 11, 12, 14, 15, 18, 23, 24, 27, 31, 33, 34]
        meas = {
        'A1': [356, 404, 548, 596, 692, 740, 884, 1124, 1172, 1316, 1508,
            1604, 1652]}
        self.flaws = []
        for index in range(len(lcrk)):
            crack = CrackData()
            crack.crksize = float(lcrk[index])
            crack.rdata = []
            for key in meas.keys():
                an_inspection = CInspection(float(meas[key][index]))
                crack.rdata += [an_inspection]
            crack.ahat_transform = self.get_ahat_transform()
            crack.a_transform = self.get_a_transform()
            crack.crkf = funcr(crack.crksize, crack.a_transform)
            if not IsNumber(crack.crkf):
                crack.bValid = False
            crack.nsets = len(crack.rdata)
            included = [each for each in crack.rdata if each.IsIncluded]
            crack.count = len(included)
            r, ysum, ysqr = 0, 0, 0
            n = 0.0
            for each in crack.rdata:
                r = r_fwd(each.value, crack.ahat_transform)
                ysum += r
                ysqr += r * r
                n += 1.0
            #crack.ybar = ysum / n
            #crack.ybar_inv = r_inv(crack.ybar, crack.ahat_transform)
            self.flaws += [crack]

        self.rlonx = [each.crkf for each in self.flaws]
        self.rlony = [each.ybar for each in self.flaws]
        self.signalmin = 10
        self.signalmax = 200


    def example2(self):
        lcrk = [6, 7, 8, 8, 8, 8, 8, 8, 9, 9, 9, 10, 10, 10, 10, 11, 11, 11,
            11, 11, 11, 11, 12, 12, 12, 13, 13, 13, 13, 14, 14, 15, 15, 15, 16,
            16, 17, 18, 18, 20, 20, 21, 25]
        meas = {
        'A11': [130, 104, 118, 115, 147, 143, 154, 95, 138, 181, 218, 154, 216,
            87, 229, 201, 262, 215, 261, 334, 397, 320, 283, 362, 457, 330,
            303, 506, 312, 428, 503, 524, 430, 616, 523, 577, 572, 603, 623,
            1012, 928, 862, 1804],
        'A21': [94, 108, 139, 122, 131, 130, 174, 50, 136, 190, 191, 149, 205,
            220, 273, 157, 305, 186, 210, 345, 323, 324, 282, 331, 358, 282,
            300, 432, 316, 465, 388, 441, 384, 619, 470, 598, 482, 635, 634,
            941, 922, 798, 1547],
        'A31':
            [106, 86, 108, 114, 138, 134, 126, 70, 138, 122, 169, 138, 186,
            223, 214, 132, 251, 169, 205, 291, 304, 283, 273, 333, 333, 302,
            270, 399, 290, 402, 404, 406, 382, 484, 435, 566, 485, 576, 547,
            864, 876, 722, 1543]}
        self.flaws = []
        for index in range(len(lcrk)):
            crack = CrackData()
            crack.crksize = float(lcrk[index])
            crack.rdata = []
            for key in meas.keys():
                an_inspection = CInspection(float(meas[key][index]))
                crack.rdata += [an_inspection]
            crack.ahat_transform = self.get_ahat_transform()
            crack.a_transform = self.get_a_transform()
            crack.crkf = funcr(crack.crksize, crack.a_transform)
            if not IsNumber(crack.crkf):
                crack.bValid = False
            crack.nsets = len(crack.rdata)
            included = [each for each in crack.rdata if each.IsIncluded]
            crack.count = len(included)
            r, ysum, ysqr = 0, 0, 0
            n = 0.0
            for each in crack.rdata:
                r = r_fwd(each.value, crack.ahat_transform)
                ysum += r
                ysqr += r * r
                n += 1.0
            #crack.ybar = ysum / n
           #crack.ybar_inv = r_inv(crack.ybar, crack.ahat_transform)
            self.flaws += [crack]

        self.rlonx = [each.crkf for each in self.flaws]
        self.rlony = [each.ybar for each in self.flaws]
        self.signalmin = 50
        self.signalmax = 4095

    
    def prune_data(self):
        "Gets rid of flaws with NaN's for crack size"
        pruned_flaws = []
        for each in self.flaws:
            if IsNumber(each.crksize):
                pruned_flaws += [each]
        self.flaws = pruned_flaws


    def ahat_censor(self):

        self.SetNewStatus(self, "Starting Censoring Data")

        self.prune_data()  # Get rid of NaN's for the analysis portion
        self.SortData()
        
        self.ssy, self.scnt = 0.0, 0.0
        self.npts = 0
        self.ahat_model = self.model
        self.nrows = len(self.flaws)
        self.rlonx = []
        self.rlony = []
        self.acount = 0
        self.bcount = 0

        for i in range(self.nrows):
            pdata = self.flaws[i]
            pdata.crack_ahat_censor(self)

            if not pdata.IsIncluded():
                continue
            if pdata.count == 0:
                if DEBUG: 
                    print("something stinks")
                if DEBUG:
                    print("flaw " + str(i) + " contains no data!")
                continue
            if pdata.ncensor == 0 \
            or pdata.ncensor == 3 \
            or pdata.ncensor == 4:
                self.rlonx += [pdata.crkf]
                self.rlony += [pdata.ybar]
                self.npts += 1
                self.scnt += pdata.scnt
                self.ssy += pdata.ssy
            elif pdata.ncensor == 1:  # all above max
                self.acount += 1
            elif pdata.ncensor == 2:  # all below min
                self.bcount += 1

        if self.scnt > 0:
            try:
                self.repeatability_error = sqrt(self.ssy/self.scnt)
            except ValueError:
                print('error calculating repeatability error')
                self.repeatability_error = float('NaN')
        else:
            self.repeatability_error = 0.0
        self.SortData()
        npts2 = self.npts
        if self.acount != 0:
            for i in range(self.nrows):
                pdata = self.flaws[i]
                if not pdata.IsIncluded():
                    continue
                if pdata.ncensor != 1:
                    continue
                self.rlonx += [pdata.crkf]
                self.rlony += [pdata.ybar]
                npts2 += 1

        if self.bcount != 0:
            for i in range(self.nrows):
                pdata = self.flaws[i]
                if not pdata.IsIncluded():
                    continue
                if pdata.ncensor != 2:
                    continue
                self.rlonx += [pdata.crkf]
                self.rlony += [pdata.ybar]
                npts2 += 1

        if not npts2 == self.npts + self.acount + self.bcount:
            #make sure we counted every flaw
            if DEBUG:
                print("Something didn't work below")
                print(npts2)
                print("!=")
                print("self.npts +", self.npts)
                print("self.acount", self.acount)
                print("self.bcount", self.bcount)
                print("Something didn't work")
            
        if self.scnt > 0:
            try:
                self.repeatability_error = sqrt(self.ssy/self.scnt)
            except ValueError:
                print('error calculating repeatability error')
                self.repeatability_error = float('NaN')
        else:
            self.repeatability_error = 0.0
        self.SetNewStatus(self, "Finished Censoring Data")


    def pf_censor(self):

        self.SetNewStatus(self, "Starting Censoring Data")

        'This does not really censor, just adds up the identically sized flaw measurements'
        self.pfdata = []
        self.SortData();
        self.count = 0
        for flaw in self.flaws:
            flaw.crack_pf_censor(self.pod_threshold)
            if (flaw.IsIncluded() and flaw.count != 0): 
                self.count += 1;
        # // identify unique cases
        pfd = PFData()
        lastcrack = self.flaws[0]
        pfd.above = lastcrack.above
        pfd.count = lastcrack.count
        pfd.crksize = lastcrack.crksize
        pfd.crkf = lastcrack.crkf
        pfd.prob = float(pfd.above)/pfd.count
        if DEBUG:
            pretty_print(['Flaw', 'Result', 'Above', 'Total',  'Prob'])
            pretty_print([lastcrack.crksize, lastcrack.above, pfd.above, pfd.count,  pfd.prob])            
        for each in self.flaws[1:]:
            if Compare.eq(each.crksize, lastcrack.crksize, .000001):
                pfd.above += each.above
                pfd.count += each.count
                pfd.prob = float(pfd.above)/pfd.count
            else:
                #list = [pfd.crksize, pfd.count]
                #write_list_params(list, 'crack[{0}] '.format(len(self.pfdata)), "pf_censor", 1257);
                self.pfdata += [pfd]
                if DEBUG:
                    print("-- New Crack --")
                pfd = PFData()
                pfd.above = each.above
                #pfd.count = 1.0
                pfd.count = each.count
                pfd.crksize = each.crksize
                pfd.crkf = each.crkf
                pfd.prob = float(pfd.above)/pfd.count
            if DEBUG:
                pretty_print([each.crksize, each.above, pfd.above, pfd.count,  pfd.prob])
                #print("Unique Flaws: " + str(self.npts) )
            lastcrack = each

        #list = [pfd.crksize, pfd.count]
        ##write_list_params(list, 'crack[{0}] '.format(len(self.pfdata)), "pf_censor", 1257);

        self.pfdata += [pfd]
        self.npts = len(self.pfdata)
        if DEBUG:
            for each in self.pfdata:
                print(str(each.crksize) + " " + str(each.count))  

        self.SetNewStatus(self, "Finished Censoring Data")


    def ahat_solve(self):

        self.SetNewStatus(self, "Started Solving aHat Fit")

        self.iret = 0
        self.fnorm = [0.0, 0.0, 0.0]
        lr = self.regression
        npts2 = self.npts + self.acount + self.bcount
        result = lr.regress(self.rlonx, self.rlony, npts2)

        if result != 0:
            self.AddErrorLine("Not enough points to perform a linear regression.")
            self.AddErrorLine("Include additional points in the analysis.")
            return -1    

        self.linfit = {
            'intercept': lr.intercept, 'slope': lr.slope, 'rms': lr.rmse}
        if DEBUG:
            print self.linfit
        self.variance = [[0, 0, 0], [0, 0, 0], [0, 0, 0]]

        #write_variance(self.variance, 'ahat_solve', 1259)

        self.iret = ahat_sysolv(self)
        if self.iret == 0:
            #self.deter = smtxinv(self.variance,self.variance);
            self.SetNewStatus(self, "Finished Solving aHat Fit")
            return 0

        write_msg('started second solve', 'ahat_solve', 1572)

        ##("Pass 1 Ahat analysis error return %d\n", iret)
        lr2 = self.regression
        #// second pass
        #// generate initial guess using only uncensored values
        lr2.regress(self.rlonx, self.rlony, self.npts)
        self.linfit = {
            'intercept': lr2.intercept, 'slope': lr2.slope, 'rms': lr2.rmse}
        if DEBUG:
            print self.linfit
        self.iret = ahat_sysolv(self)

        #self.deter = smtxinv(self.variance,self.variance);
        
        if self.iret == 0:
            self.SetNewStatus(self, "Finished Solving aHat Fit")
            return 0

        ##TRACE("Pass 2 Ahat analysis error return %d\n", iret)
        self.SetNewStatus(self, "Failed to Solve aHat Fit")
        return -1


    def pf_guess(self):
        #i = 0
        #shft = 0.0
        #a, b, arg = 0.0, 0.0, 0.0
        #lna1, lna2 = 0.0, 0.0
        #t1, t2 =0.0, 0.0
        #a,  b = 0.0,  0.0
        
        #if (self.pfdata[0].crksize<1.0): 
        #    shft = -1.1*log(self.pfdata[0].crksize)
        #for i in range(self.npts)[1:]:
        #    lna1 = log(self.pfdata[i].crksize)+shft;
        #    lna2 = log(self.pfdata[i-1].crksize)+shft;
        #    t1 = (lna1-lna2)*(self.pfdata[i].prob+self.pfdata[i-1].prob);
        #    t2 = t1*(lna1+lna2);
        #    a += t1;
        #    b += t2;
        #lna1 = log(self.pfdata[0].crksize)+shft;
        #lna2 = log(self.pfdata[self.npts-1].crksize)+shft;
        #t1 = lna1*self.pfdata[0].prob/2.0;
        #a = lna2 - t1 - a/2.0;
        #t2 = t1*lna1;
        #arg = lna2*lna2-t2-b/2.0-a*a;
        #b = sqrt(fabs(arg));
        #a -= shft;
        #self.muhat = a;
        #self.sighat = b;

        i = 0
        shft = 0.0
        a, b, arg = 0.0, 0.0, 0.0
        lna1, lna2 = 0.0, 0.0
        t1, t2 =0.0, 0.0
        a,  b = 0.0,  0.0
        
        if (self.pfdata[0].crksize<1.0): 
            shft = -1.1*a_fwd(self.pfdata[0].crksize, self.a_transform)
        for i in range(self.npts)[1:]:
            lna1 = a_fwd(self.pfdata[i].crksize, self.a_transform)+shft;
            lna2 = a_fwd(self.pfdata[i-1].crksize, self.a_transform)+shft;
            t1 = (lna1-lna2)*(self.pfdata[i].prob+self.pfdata[i-1].prob);
            t2 = t1*(lna1+lna2);
            a += t1;
            b += t2;
        lna1 = a_fwd(self.pfdata[0].crksize, self.a_transform)+shft;
        lna2 = a_fwd(self.pfdata[self.npts-1].crksize, self.a_transform)+shft;
        t1 = lna1*self.pfdata[0].prob/2.0;
        a = lna2 - t1 - a/2.0;
        t2 = t1*lna1;
        arg = lna2*lna2-t2-b/2.0-a*a;
        b = sqrt(fabs(arg));
        a -= shft;
        self.muhat = a;
        self.sighat = b;


    def pf_solve(self,  have_guess=False):
        
        self.SetNewStatus(self, "Started Solving Hit Miss Fit")

        self.variance = [[0.0, 0.0], [0.0, 0.0]]
        self.pf_solve_params = []

        #write_variance(self.variance, 'pf_solve', 1318)

        mus = [0.0, 0.0, 0.0]
        sigs = [0.0, 0.0, 0.0]
        x = [0.0, 0.0]
        mumom = 0.0
        sigmom = 0.0
        self.iret = False
        #// use method of moments to get starting values for mu and sigma
        self.pf_guess();
        mumom = self.muhat;
        sigmom = self.sighat;
        if (have_guess) :
            mumom = self.mu_guess;
            sigmom = self.sig_guess;
        x[0] = mumom;        
        x[1] = sigmom;
        write_list_params(x, "x", "pf_solve", 1338);
        for n in range(4):
            if n == 3:
                #pDoc = 0
                #// Generate some debug information
                #int i;
                #int row = info_row + 2;
                #xlGetSheet("Results");
                #xlValue(row,1,"Pass/Fail analysis failed to converge");
                #row += 2;
                #xlValue(row,1,"trial");
                #xlValue(row,2,"muhat");
                #xlValue(row,3,"sighat");
                #xlValue(row,4,"fnorm");
                #row++;
                #xlValue(row,1,"guess");
                #xlValue(row,2,mumom);
                #xlValue(row,3,sigmom);
                #for (i=0; i<n; i++) {
                # xlValue(++row,1,i+1);
                # xlValue(row,2,mus[i]);
                # xlValue(row,3,sigs[i]);
                # xlValue(row,4,fnorm[i]);
                #}
                self.AddErrorLine("Pass/Fail analysis failed to converge")
                return False

            if (self.model==LINEAR_MODEL):
                #iret = self.pf_sysolv(x,jb,fnorm[n],row)
                self.iret = pf_sysolv(self,  x, n)
            else:
                self.iret = odds_sysolv(self,  x, n);
            write_list_params(x, "x", "pf_solve", 1345);
            mus[n] = x[0];
            sigs[n] = x[1];
            if not self.iret:
                break
            if (n==0):
                x[0] = mumom;
                x[1] = sigmom/2.0;
            else:
                x[0] = mumom;
                x[1] = 2.0*sigmom;

        self.muhat = x[0];
        self.sighat = x[1];
        self.pf_covariance[0] = self.variance[0][0]  #called v[] in original code
        self.pf_covariance[1] = 0.5*(self.variance[0][1]+self.variance[1][0])
        self.pf_covariance[2] = self.variance[1][1]

        self.SetNewStatus(self, "Finished Solving Hit Miss Fit")

        return True

    def save_pf_solve_iteration(self, trialIndex, iterationIndex, mu, sigma, fnorm, didDamp, dampValue):

        iteration = SolveIteration(trialIndex, iterationIndex, mu, sigma, fnorm, didDamp, dampValue)

        self.pf_solve_params.append(iteration)

    def SortData(self):
        #from qsort import qsort
        #qsort(self.flaws)
        self.flaws.sort()

    #def Kill(self):
    #    _exit(0)

    def OnFitOnlyAnalysis(self):
        ##ahat_model = self.model



        FileLogger.Log_Start("")

        FileLogger.Log_Msg("", self.name, "Starting Analysis", "OnAnalysis")

        self.ClearErrors(self)

        if (self.analysis == AHAT_ANALYSIS):
            FileLogger.Log_Msg("", self.name, "AHat Censoring", "OnAnalysis")
            self.ahat_censor()    # defined in ahat.cpp

            FileLogger.Log_Msg("", self.name, "AHat Getting Crack Ranges", "OnAnalysis")
            self.GetCrackRange()  # defined in CrackData.cpp

            #self.ShowInfo()                 #defined in Info.cpp
            if (self.model == NONLINEAR_MODEL):
                FileLogger.Log_Msg("", self.name, "AHat Non-Linear Solve", "OnAnalysis")
                self.ahat_nonlinear_solve()

            else:
                FileLogger.Log_Msg("", self.name, "AHat Linear Solve", "OnAnalysis")
                self.ahat_solve()  # defined in ahat.cpp

            FileLogger.Log_Msg("", self.name, "AHat Residuals", "OnAnalysis")
            self.ahat_residuals()            

        elif (self.analysis==PF_ANALYSIS):
            FileLogger.Log_Msg("", self.name, "PF Censoring", "OnAnalysis")
            self.pf_censor();

            FileLogger.Log_Msg("", self.name, "PF Getting Crack Ranges", "OnAnalysis")
            self.GetCrackRange();

            #ShowInfo()
            FileLogger.Log_Msg("", self.name, "PF Solve", "OnAnalysis")
            if self.pf_solve() == False:
                FileLogger.Log_Msg("", self.name, "Failed PF Solve Returning with No Output", "OnAnalysis")
                self.pf_clearoutput()

                return
            FileLogger.Log_Msg("", self.name, "PF Residuals", "OnAnalysis")
            self.pf_residuals()        
            
        FileLogger.Log_Msg("", self.name, "Finished Analysis", "OnAnalysis")

        if self.m9095 > 0.0:
                self.SetNewStatus(self, "Finished Performing POD calculations")

    def SavePODOutput(self):
        self.pod_all = self.GetPODTable()
        self.a50_all = self.GetPODFlaw50()
        self.a90_all = self.GetPODFlawLevel()
        self.a9095_all = self.GetPODFlawConfidence()
        self.threshold_all = self.GetThresholdTable()

    def GetPODTable_All(self):

        if self.pod_all == 0.0:
            odict = dict() #OrderedDict()

            odict['list_names'] = ['flaw', 'pod', 'confidence']
            odict['flaw'] = []
            #odict['t_flaw'] = self.pod_crksizef
            odict['pod'] = []
            #odict['confidenceFlawAtPOD'] = self.pod_bounds_crksize
            odict['confidence'] = []

            self.pod_all = odict

        return self.pod_all

    def GetThresholdTable_All(self):

        if self.threshold_all == 0.0:
            '#threshold value, flaw50, level, confidence'
            odict = dict() #OrderedDict()

            odict['list_names'] = ['threshold', 'level', 'confidence', 'flaw50', 'V11', 'V12', 'V22']
            odict['threshold'] = []        
            odict['level'] = []
            odict['confidence'] = []
            odict['flaw50'] = []
            odict['V11'] = []
            odict['V12'] = []
            odict['V22'] = []

            self.threshold_all = odict

        return self.threshold_all

    def GetPODFlaw50_All(self):
        return self.a50_all

    def GetPODFlawLevel_All(self):
        return self.a90_all

    def GetPODFlawConfidence_All(self):
        return self.a9095_all
        

    def OnFullAnalysis(self):

        self.showMessages = False
        self.SetFlawData(self.flaws_reload, self.a_transform)
        self.SetResponseData(self.responses_all, self.ahat_transform)
        self.OnSimpleAnalysis()
        self.SavePODOutput()
        self.showMessages = True
        self.SetFlawData(self.flaws_reload, self.a_transform)
        self.SetResponseData(self.responses_missing, self.ahat_transform)
        self.OnSimpleAnalysis()

    def OnAnalysis(self):
        self.showMessages = True
        self.OnSimpleAnalysis()

    def OnSimpleAnalysis(self):
        ##ahat_model = self.model

        FileLogger.Log_Start("")

        FileLogger.Log_Msg("", self.name, "Starting Analysis", "OnAnalysis")

        self.ClearErrors(self)

        if (self.analysis == AHAT_ANALYSIS):
            FileLogger.Log_Msg("", self.name, "AHat Censoring", "OnAnalysis")
            self.ahat_censor()    # defined in ahat.cpp

            FileLogger.Log_Msg("", self.name, "AHat Getting Crack Ranges", "OnAnalysis")
            self.GetCrackRange()  # defined in CrackData.cpp

            #self.ShowInfo()                 #defined in Info.cpp
            if (self.model == NONLINEAR_MODEL):
                FileLogger.Log_Msg("", self.name, "AHat Non-Linear Solve", "OnAnalysis")
                self.ahat_nonlinear_solve()

            else:
                FileLogger.Log_Msg("", self.name, "AHat Linear Solve", "OnAnalysis")
                self.ahat_solve()  # defined in ahat.cpp

            FileLogger.Log_Msg("", self.name, "AHat Residuals", "OnAnalysis")
            self.ahat_residuals()

            FileLogger.Log_Msg("", self.name, "AHat Tests", "OnAnalysis")
            self.ahat_tests()

            FileLogger.Log_Msg("", self.name, "AHat Print", "OnAnalysis")
            self.ahat_print()

            FileLogger.Log_Msg("", self.name, "AHat POD", "OnAnalysis")
            self.ahat_pod()

            FileLogger.Log_Msg("", self.name, "AHat Decision", "OnAnalysis")
            self.ahat_decision()

        elif (self.analysis==PF_ANALYSIS):
            FileLogger.Log_Msg("", self.name, "PF Censoring", "OnAnalysis")
            self.pf_censor();

            FileLogger.Log_Msg("", self.name, "PF Getting Crack Ranges", "OnAnalysis")
            self.GetCrackRange();

            #ShowInfo()
            FileLogger.Log_Msg("", self.name, "PF Solve", "OnAnalysis")
            if self.pf_solve() == False:
                FileLogger.Log_Msg("", self.name, "Failed PF Solve Returning with No Output", "OnAnalysis")
                self.pf_clearoutput()

                return
            FileLogger.Log_Msg("", self.name, "PF Residuals", "OnAnalysis")
            self.pf_residuals()

            FileLogger.Log_Msg("", self.name, "PF Print", "OnAnalysis")
            self.pf_print()

            FileLogger.Log_Msg("", self.name, "PF POD", "OnAnalysis")
            self.pf_pod()

            
        FileLogger.Log_Msg("", self.name, "Finished Analysis", "OnAnalysis")

        if self.m9095 > 0.0:
                self.SetNewStatus(self, "Finished Performing POD calculations")

    def pf_clearoutput(self):
        self.pf_clearresiduals()
        self.pf_clearprint()
        #self.pf_clearpod()
        self.pf_cleartests()

    def pf_cleartests(self):
        self.fcalc = 0.0
        self.lackOfFitRating = self.choices[5]

    def pf_clearresiduals(self):
        self.pfit = []
        self.diff = []

    def pf_clearprint(self):
        self.muhat = 0.0
        self.sighat = 0.0
        self.pf_covariance = [0.0, 0.0, 0.0]
        self.z90 = 0.0
        self.mhat = 0.0
        self.x90 = 0.0
        self.m90 = 0.0
        self.z95 = 0.0
        self.m9095 = 0.0

    def pf_clearpod(self):
        self.pf_POD_a = []
        self.pf_POD_af = []
        self.pf_POD_poda = []
        self.pf_POD_conf95 = []
        self.pf_POD_ptxpt = []
        self.new_pf_a = []
        self.new_pf_gamma = []
        self.new_pf_beta = []
        self.new_pf_pod = []
        self.new_pf_old = []

    def GetCrackRange(self):

        self.SetNewStatus(self, "Started Getting Crack Ranges")

        first = True
        if len(self.flaws) == 0:
            return
        for crack in self.flaws:
            if not crack.IsIncluded():
                continue
            if first is True:
                self.crkmin = crack.crksize
                self.crkmax = crack.crksize
                first = False
            else:
                if (crack.crksize < self.crkmin):
                    self.crkmin = crack.crksize
                elif (crack.crksize > self.crkmax):
                    self.crkmax = crack.crksize

        first = True
        for crack in self.flaws:
            if not crack.IsIncluded():
                continue
            if crack.count == 0:
                continue
            if first is True:
                self.xmin = crack.crksize
                self.xmax = crack.crksize
                first = False
            else:
                if crack.crksize > self.xmax:
                    self.xmax = crack.crksize
                if crack.crksize < self.xmin:
                    self.xmin = crack.crksize


    def ahat_residuals(self):

        self.SetNewStatus(self, "Started Calculating Residuals")

        #// Calculate extremes of fit
        x1 = a_fwd(self.xmin, self.a_transform)
        y1 = self.linfit['intercept'] + self.linfit['slope'] * x1
        tlimit = r_inv(y1, self.ahat_transform)
        if (tlimit < self.signalmin):
            y1 = r_fwd(self.signalmin, self.ahat_transform)
            if self.linfit['slope'] > 0.0:
                x1 = (y1 - self.linfit['intercept']) / self.linfit['slope']
            else:
                x1 = 0.0
        if (self.signalmax > self.signalmin):
            if (tlimit > self.signalmax):
                y1 = r_fwd(self.signalmax, self.ahat_transform)
                if self.linfit['slope'] > 0.0:
                    x1 = (y1 - self.linfit['intercept']) / self.linfit['slope']
                else:
                    x1 = 0.0
        x2 = a_fwd(self.xmax, self.a_transform)
        y2 = self.linfit['intercept'] + self.linfit['slope'] * x2
        tlimit = r_inv(y2, self.ahat_transform)

        if (tlimit < self.signalmin):
            y2 = r_fwd(self.signalmin, self.ahat_transform)
            if self.linfit['slope'] > 0.0:
                x2 = (y2 - self.linfit['intercept']) / self.linfit['slope']
            else:
                x2 = 1.0
        if (self.signalmax > self.signalmin):
            if (tlimit > self.signalmax):
                y2 = r_fwd(self.signalmax, self.ahat_transform)
                if self.linfit['slope'] > 0.0:
                    x2 = (y2 - self.linfit['intercept']) / self.linfit['slope']
                else:
                    x2 = 1.0
        if (self.get_a_transform() <= XF_LOG):
            x1 = a_inv(x1, self.a_transform)
            x2 = a_inv(x2, self.a_transform)
        if (self.get_ahat_transform() <= XF_LOG):
            y1 = r_inv(y1, self.ahat_transform)
            y2 = r_inv(y2, self.ahat_transform)

        self.analyses = []

        for each in self.flaws:
            if not each.IsIncluded() or each.count == 0:
                continue
            if each.ncensor == 0 \
            or each.ncensor == 3 \
            or each.ncensor == 4:
                self.analyses += [CAnal(each, self)]

        if len(self.analyses) != self.npts:
            if DEBUG:
                print("Something Stinks! len(self.analyses) != self.npts")
                print(str(len(self.analyses)) + " != " + str(self.npts))

        for each in self.flaws:
            if not each.censored:
                continue
            x = each.crkf
            y = self.linfit['intercept'] + self.linfit['slope'] * x
            each.yfit = y
            each.yfit_inv = r_inv(y, self.ahat_transform)

        self.nexcluded = 0
        for each in self.flaws:
            if not each.IsIncluded or each.count == 0:
                continue
            if each.ncensor == 1 \
            or each.ncensor == 2:
                self.analyses += [CAnal(each, self)]
                self.nexcluded += 1

        if DEBUG:
            print("Variance Before")
            print(self.variance)
        #smtxinv(self.variance,self.variance)
        if DEBUG:
            print("Variance After")
            print(self.variance)

        if len(self.analyses) != (self.npts + self.acount + self.bcount):
            if DEBUG:
                print("Something Stinks!")
                print("self.analyses", self.analyses)
                print("!=")
                print("self.npts +", self.npts)
                print("self.acount", self.acount)
                print("self.bcount", self.bcount)

        self.SetNewStatus(self, "Finished Calculating Residuals")

    
    def pf_residuals(self):
        #int i, row;
        #PFData *pfd;
        #double z, pfit;
        #xlGetEmptySheet("Residuals");
        #row = 1;
        #xlValue(row,1,"a");
        #xlValue(row,2,"log(a)");
        #xlValue(row,3,"p");
        #xlValue(row,4,"fit");
        #xlValue(row,5,"diff");
        #pfd = pfdata;
        #xlValue(++row,1,pfd->crksize);
        #xlValue(row,2,pfd->crkf);
        #xlValue(row,3,pfd->prob);

        self.SetNewStatus(self, "Started Calculating Residuals")

        if DEBUG:
            pretty_print(["a", "log(a)", "p",  "fit",  "diff"])
        self.pfit = []
        self.diff = []
        #[self.pfdata[i].prob-self.pfit[i] for i in range(len(self.pfdata))]
        for pfd in self.pfdata:
            z = (pfd.crkf - self.muhat) / self.sighat;
            if self.model==LINEAR_MODEL:
               pfit = mdnord(z)
               diff = pfd.prob - pfit
            else:
                pfit = pod_odds(z)
                diff = pfd.prob - pfit
            self.pfit += [pfit]
            self.diff += [diff]
            if DEBUG:
                pretty_print([pfd.crksize, pfd.crkf, pfd.prob, pfit, diff])
        
            #xlValue(row,4,pfit);
            #xlValue(row,5,pfd->prob-pfit);
        #nexcluded = 0; #// !!!!!!!!!!!!!!!!!!  Add excluded points to residuals worksheet

        self.SetNewStatus(self, "Finished Calculating Residuals")

    def rating_normality(self, aValue):

        ranges = [0.631, 0.752, 0.873, 1.035, 1.159]

        if aValue < 0.0:
            return self.choices[6]

        for i in range(len(ranges)):
            if  aValue <= ranges[i]:
                return self.choices[i]

        return self.choices[len(ranges)]

    def rating_variance(self, vValue):

        ranges = [0.1, 0.05, 0.025, 0.01, 0.005]

        if vValue < 0.0:
            return self.choices[6]

        for i in range(len(ranges)):
            if  vValue > ranges[i]:
                return self.choices[i]

        return self.choices[len(ranges)]

    def rating_fit(self, fValue):

        return self.rating_variance(fValue)

    def statFactorial(self, n, y):

        bottom = (factorial(y)*factorial(n-y)) 

        if bottom != 0.0:
            return factorial(n) / bottom
        else:
            return 0.0

    def pf_tests(self):

        self.pf_test_crksum = 0.0
        self.pf_test_phat = 0.0
        self.pf_test_h0_likihood = 0.0
        self.pf_test_ha_likihood = 0.0
        self.pf_censor_test_result = 0.0
        self.pf_censor_test_pass = False
        crkcount = 0
        t_cracks = []
        crackresults = []

        #build your crack detection list
        for pfd in self.pfdata:
            for i in range(pfd.above):
                crackresults.append(1.0)
            for i in range(pfd.count - pfd.above):
                crackresults.append(0.0)
            self.pf_test_crksum += pfd.above
            crkcount += pfd.count

            for i in range(pfd.count):
                t_cracks.append(pfd.crkf)

        self.pf_test_phat = self.pf_test_crksum / crkcount

        crackTerm = self.pf_test_crksum * log(self.pf_test_phat) + (crkcount - self.pf_test_crksum) * log(1 - self.pf_test_phat)

        #taking out factorial term for now - (06-30-2015)
        #factorialTerm = log(self.statFactorial(crkcount, self.pf_test_crksum))

        self.pf_test_h0_likihood = crackTerm # + factorialTerm 

        result = 0.0
        xi = 0.0
        yi = 0.0

        for i in range(crkcount):
            xi = t_cracks[i]
            yi = crackresults[i]
            zhat = (xi-self.muhat)/self.sighat 
            if (self.model==LINEAR_MODEL):
                pod = mdnord(zhat)
            else:
                pod = pod_odds(zhat)

            self.pf_test_ha_likihood += (yi * log(pod) + (1-yi) * log(1-pod))

        self.pf_censor_test_result = -2.0 * (self.pf_test_h0_likihood - self.pf_test_ha_likihood)

        if self.pf_censor_test_result >= 2.70554:
            self.pf_censor_test_pass = True
            self.lackOfFitRating = self.choices[0]            
        else:
            self.pf_censor_test_pass = False
            self.lackOfFitRating = self.choices[5]
        
        self.fcalc = self.pf_censor_test_result

        #until we get it fixed
        self.fclac = 0.0
        

    def ahat_tests(self):

        self.SetNewStatus(self, "Started Running aHat Tests")

        # Test of normality
        # sort table by deviation
        dstars = [each.dstar for each in self.analyses if not each.censored]
        dstars.sort()
        lim = float(len(dstars))
        asq = 0.0;
        for i in range(int(lim)):
            tmp1 = 2*i+1;
            tmp2 = log(dstars[i]);
            oneMinus = 1.0-dstars[int(lim)-1-i]
            if oneMinus != 0.0:
                tmp3 = log(oneMinus)
            else:
                tmp3 = 0.0
            tmp4 = (tmp1*(tmp2+tmp3))/lim;
            asq += tmp4;
        asq = -(asq+lim);

        if lim > 0.0:
            self.astar = asq * (1.0 + (0.75/lim) + (2.25 / (lim*lim)));
            self.astar_p = 3.5727 * pow(e, -5.767 * self.astar) #equation from fit of points in rating_normality vs rating_variance
            self.astarRating = self.rating_normality(self.astar)
        else:
            self.astar = 0.0
            self.astar_p = 0.0
            self.astarRating = self.rating_normality(-1)

        #////////////////////////////////////////////////////////////////////////
        #// Homogenity of Variance
        ngrps = 2.0
        cnts = []
        avg = []
        sum = []
        vor = []

        #// Sort data by crack size
        anal = [each for each in self.analyses if not each.censored]
        anal.sort()
        #// Divide the data into groups
        intrvl = int(self.npts/ngrps)
        for i in range(int(ngrps)):
            cnts += [intrvl]
        #// If groups can not be equal sizes, add 1 to first few groups
        n = self.npts - ngrps*intrvl
        for i in range(int(n)): 
            cnts[i] += 1

        canCalculate = True

        for val in cnts:
            if val <= 1:
                canCalculate = False

        if canCalculate:

            #// Calculate variance of residuals in each group
            #// "i" is the group index; "j" indexes the values within the group
            n=0
            for i in range(int(ngrps)):
                mean_diff = 0.0
                diff_squared = 0.0
                cnt = cnts[i];
                for j in range(int(cnt)): 
                    mean_diff += anal[int(n+j)].diff;
                mean_diff /= cnt;
                for j in range(int(cnt)):
                    diff = anal[n + j].diff - mean_diff;
                    diff_squared += diff*diff;
                avg += [mean_diff];
                sum += [diff_squared];
                vor += [diff_squared/(cnt-1.0)];
                n += int(cnt);
            dof = float(ngrps-1);
            grpcnt = float(ngrps);
            term1 = 0.0
            term2 = 0.0
            term3 = 0.0
            term4 = 0.0
            for i in range(int(ngrps)):
                cnt = cnts[i];
                term1 += cnt;
                term2 += 1.0/(cnt-1);
                if vor[i] > 0.0:
                    term3 += (cnt-1)*log(vor[i]);
                else:
                    term3 = 0.0
                term4 += (cnt-1)*vor[i];
            term1 -= grpcnt;
            term4 /= term1;
            #double bd, halfbd, halfdof, p;
            try:
                self.EV_Bartlett = (term1*log(term4)-term3)/(1.0+((term2-(1.0/term1))/(3.0*dof)));
            except(ValueError):
                    print('could not calculate EV_Bartlett')
                    self.EV_Bartlett = float('NaN')
                    self.EV_Bartlett_p = float('NaN')

            halfbd = 0.5*self.EV_Bartlett
            halfdof = 0.5*dof

            #// Calculate Equal-Variance Bartlet
            p = 1.0 - gammds(halfbd, halfdof)

            self.EV_Bartlett_p = p

            self.barlettRating = self.rating_variance(p)

        else:
            self.AddErrorLine("Not enough points to perform Equal Variance Test.");
            self.AddErrorLine("Include additional points in the analysis.")
            self.barlettRating = self.rating_variance(-1)
            self.EV_Bartlett = 0.0
        
        

        #/////////////////////////////////////////////////////////////////////////////
        #//  Test of linearity
        #// check for duplicates
        nbrsets = 1  # sets of unique crack sizes

        self.lackOfFitCalculated = False

        if len(anal) > 0:

            lastcrack = anal[0].crksize
            for each in anal:
                if each.crksize != lastcrack:
                    nbrsets += 1
                lastcrack = each.crksize            
        
            #linearity test can be performed
            if nbrsets!=self.npts:
                #// Determine total variance
                totss = 0.0;
                diffs = [each.diff for each in anal]
                for diff in diffs:
                    totss += diff*diff;

                #// process duplicates
                pess = 0.0;
                n=1
                diff = 0.0
                term1 = 0.0
                term2 = 0.0
                i=0
                while i < self.npts:
                    crksize = anal[i].crksize
                    diff = anal[i].diff
                    term1 = diff*diff
                    term2 = diff
                    n=1
                    i += 1
                    while (i<self.npts and anal[i].crksize==crksize) :
                        diff = anal[i].diff
                        i += 1
                        term1 += diff*diff
                        term2 += diff
                        n += 1
                    term2 = (term2 * term2) / n
                    pess += (term1 - term2)
        
                #// LOF means "lack of fit"
                #// PE  means "pure error"
                #// LOFDF = RegressionDF - PureErrorDF
                pedf = float(self.npts-nbrsets);
                lofss = totss - pess;
                lofdf = (self.npts-2) - pedf;
        
                #// Linearity results
                beta = alogam(lofdf/2.0) + alogam(pedf/2.0) - alogam((lofdf+pedf)/2.0)
                self.fcalc = (lofss/lofdf) / (pess/pedf)
                x = pedf / (pedf + (lofdf*self.fcalc))
                paff = betain(x,pedf/2.0,lofdf/2.0,beta)

                self.fcalc_p = paff
                self.lackOfFitRating = self.rating_fit(paff)
                self.lackOfFitDegFrdm = pedf
                self.lackOfFitCalculated = True

        if self.lackOfFitCalculated == False:
            self.lackOfFitDegFrdm = 0.0
            self.fcalc = 0.0
            self.fcalc_p = 0.0
            self.lackOfFitRating = self.rating_fit(-1)

        #smtxinv(self.variance, self.variance)
        
        #need to be in smaller values here

        #got sighat calculation from here and moved to ahat_print

        #smtxinv(self.variance, self.variance)

        self.SetNewStatus(self, "Finished Running aHat Tests")

    def ahat_print(self):

        

        self.SetNewStatus(self, "Started Running Final Calculations")

        #se = [0.0,  0.0,  0.0]
        self.deter = smtxinv(self.variance, self.variance);
        
        

        for i in range(3):
            try:
                self.fit_errors[i] = sqrt(-(self.variance[i][i]))*self.linfit['rms']
            except(ValueError):
                #print('could not calculate fit_errors')
                self.fit_errors[i] = 0.0#float('NaN')


        #calculate sighat
        rn = 0.0
        nstrn = 0;
        for i in range(self.nsets):
            if (self.CountInsp(i) != 0): 
                nstrn += 1
        if self.linfit['slope'] > 0.0:
                self.sighat = self.linfit['rms']/self.linfit['slope'];
        else:
            self.sighat = 0.0
        if nstrn>1:
            rn = float(nstrn-1)/float(nstrn);
            self.sighat = sqrt(self.linfit['rms']*self.linfit['rms']+rn*self.repeatability_error**2.0)
            for i in range(3): 
                if self.sighat != 0.0:
                    self.variance[i][2] *= self.linfit['rms']/self.sighat;
                write_one_variance(self.variance, i, 2, 'ahat_print', 1731)
                self.variance[2][i] = self.variance[i][2];                
                write_one_variance(self.variance, 2, i, 'ahat_print', 1733)
            
            if self.sighat != 0.0:
                self.variance[2][2] *= self.linfit['rms']/self.sighat
            write_one_variance(self.variance, 2, 2, 'ahat_print', 1736)
            if self.scnt != 0: 
                self.variance[2][2] += rn*rn*self.repeatability_error**4.0/(2.0*self.scnt);
                write_one_variance(self.variance, 2, 2, 'ahat_print', 1739)
            if self.linfit['slope'] > 0.0:
                self.sighat = self.sighat/self.linfit['slope']
            else:
                self.sighat = 0.0

        #if len(self.ath > 0):
        #    self.pod_threshold = self.ath[0];
 
        if self.linfit['slope'] > 0.0:
            self.muhat = (r_fwd(self.pod_threshold,  self.ahat_transform) - self.linfit['intercept'])/self.linfit['slope']
        else:
            self.muhat = 0.0
        totnpts = self.npts + self.acount + self.bcount;

        if totnpts > 0:
            chi = cbound(totnpts); #// 5.13822 + 2.09651 / totnpts;
        else:
            chi = cbound(1)
        if DEBUG:
            print("self.pod_level")
            print(self.pod_level)
        
        zp = phinv(self.pod_level); #// 1.282 = phinv(0.9);
        if DEBUG:
            print("zp")
            print(zp)
        xp = zp*self.sighat;
        if DEBUG:
            print("xp")
            print(xp)
        if self.linfit['slope'] > 0.0:
            tmp = (self.linfit['rms']/self.linfit['slope'])**2
        else:
            tmp = 0.0
        if DEBUG:
            print("tmp")
            print(tmp)
            print("self.variance")
            print(self.variance)

        write_variance(self.variance, 'ahat_print', 2290)
        write_one_value('muhat', self.muhat, 'ahat_print', 2290)
        write_one_value('sighat', self.sighat, 'ahat_print', 2290)
        write_one_value('rms', self.linfit['rms'], 'ahat_print', 2290)
        write_one_value('slope', self.linfit['slope'], 'ahat_print', 2290)
        write_one_value('tmp = (rms / slope)^2', tmp, 'ahat_print', 2290)


        v2 = [-(self.variance[0][0]+(2.0*self.variance[0][1]+self.variance[1][1]*self.muhat)*self.muhat)*tmp, 
        -(self.sighat*(self.variance[0][1]+self.muhat*self.variance[1][1])-self.variance[0][2]-self.muhat*self.variance[1][2])*tmp, 
        -(self.variance[1][1]*self.sighat*self.sighat-2.0*self.sighat*self.variance[1][2]+self.variance[2][2])*tmp]
        if DEBUG:
            print("v2")
            print(v2)
        deter = v2[0]*v2[2]-v2[1]*v2[1];
        if DEBUG:
            print("deter")
            print(deter)
        if self.fit_errors[1] != 0.0:
            tstsig = self.linfit['slope'] / self.fit_errors[1] #self.sighat*self.sighat/v2[2];
        else:
            tstsig = self.z95 - 1 #force unable to calculate error
        if DEBUG:
            print("tstsig")
            print(tstsig)
        self.a90 = self.muhat + xp;
        self.m90 = a_inv(self.a90, self.a_transform);
        self.mhat = a_inv(self.muhat,  self.a_transform);

        if DEBUG:
            print("chi")
            print(chi)
        if(v2[0] < 0.0 or v2[2] < 0.0):
            self.m9095 = 0.0
            self.AddErrorLine("Unable to calculate a90/95 due to issues with data values.");
            if(v2[2] < 0.0 and self.repeatability_error > self.linfit['rms']):
                self.AddErrorLine("Because the repeatability error is greater than the residual error,")
                self.AddErrorLine("it is recommended that other axis transforms be considered.")
        elif (tstsig < self.z95):
            self.m9095 = 0.0
            self.AddErrorLine("Unable to calculate a90_95. This is not an adequate fit to the POD model.");
        else:
            try:
                se_a90 = sqrt(v2[0]+(2.0*v2[1]+v2[2]*self.z90)*self.z90)
                self.a9095 = self.a90 + self.z95 * se_a90
                #self.a9095 = self.sighat*(v2[0]+(2.0*v2[1]+v2[2]*zp)*zp);
                #self.a9095 = self.a90 + self.a9095/(sqrt(self.sighat*self.a9095/chi-deter)-v2[1]-v2[2]*zp);
                self.m9095 = a_inv(self.a9095,  self.a_transform);
                self.SetNewStatus(self, "Finished Running Final Calculations")
            except(ValueError):
                print('could not calculate a9095')
                self.a9095 = float('NaN')
                self.m9095 = float('NaN')

        if(self.m9095 == 0.0):
            self.AddErrorLine("Consult a statistician if necessary.");
    
    
    def pf_print(self):

        self.SetNewStatus(self, "Started Running Final Calculations")

        DEBUG = False
        if DEBUG:
            print("Mu-hat: " + str(self.muhat))
            print("Sigma-hat: " + str(self.sighat))
        cov = self.pf_covariance
        determ = cov[0]*cov[2] - cov[1]*cov[1]
        #number of inspections / crack
        # I'm not sure what nstrn means, but setting it to one gives me approximately the right
        # "Estimated Covariance Matrix"
        #nstrn = 1.0  # len([flaw for flaw in self.flaws if flaw.IsIncluded])
        nstrn = self.flaws[0].count
        vtem = cov[0]
        fac = nstrn/determ
        
        cov[0] = -cov[2] * fac
        cov[1] *= fac
        cov[2] = -vtem * fac
        determ = cov[0] * cov[2] - cov[1] * cov[1]
        
        if self.model == ODDS_MODEL:
            self.z90 = odds_inv(self.pod_level)
        else:
            self.z90 = phinv(self.pod_level)

        self.mhat = a_inv(self.muhat,  self.a_transform)
        self.x90 = self.muhat + self.z90*self.sighat
        self.m90 = a_inv(self.x90,  self.a_transform)
        self.z95 = phinv(self.confidence_level);
        self.m9095 = a_inv(self.muhat+self.z90*self.sighat
                     +self.z95*sqrt(cov[0]+self.z90*(2.0*cov[1]+self.z90*cov[2])), self.a_transform);

        if DEBUG:
            pretty_print(["a50",  "a90",  "a9095"])
            pretty_print([self.mhat, self.m90,  self.m9095])
            
            pretty_print(["V11",  "V12",  "V22"])
            pretty_print([cov[0], cov[1],  cov[2]])

        self.SetNewStatus(self, "Finished Running Final Calculations")

    def ahat_pod(self):

        
        self.SetNewStatus(self, "Started Calculating POD Curve")

        length =len(self.flaws)

        if length > 0:
            chi = cbound(length);
        else:
            chi = cbound(1)
        if self.linfit['slope'] > 0.0:
            self.muhat = (r_fwd(self.pod_threshold,  self.ahat_transform) - self.linfit['intercept'])/self.linfit['slope'];
        else:
            self.muhat = 0.0
        if DEBUG:
            print self.muhat
        
        if self.linfit['slope'] > 0.0:
            tmp = (self.linfit['rms'] / self.linfit['slope']) ** 2.0
        else:
            tmp = 0.0

        v = [-(self.variance[0][0]+(2.0*self.variance[0][1]+self.variance[1][1]*self.muhat)*self.muhat)*tmp, 
        -(self.sighat*(self.variance[0][1]+self.muhat*self.variance[1][1])-self.variance[0][2]-self.muhat*self.variance[1][2])*tmp, 
        -(self.variance[1][1]*self.sighat*self.sighat-2.0*self.sighat*self.variance[1][2]+self.variance[2][2])*tmp]
        self.deter = v[0]*v[2]-v[1]*v[1];
        
        self.pod = []
        self.pod_bounds = []
        n = 75
        
        abot = a_inv(self.muhat - 8.0 * self.sighat, self.a_transform)
        atop = a_inv(self.muhat + 4.0 * self.sighat, self.a_transform)

        #if abot<self.calcmin: 
        #    abot = self.calcmin
        #if self.calcmax<atop: 
        #    atop = self.calcmax
        
        
        
        self.pod_crksize = []
        self.pod_crksizef = []
        self.pod = []
        self.pod_bounds = []
        self.pod_bounds_crksize = []
        self.pod_bounds_crksizef = []

        #comment out until we fix POD curve

        ##get back to pod abot instead of 95 curve abot
        #if self.m9095 > 0.0:
            
        #    bound_95_inv = abot + 1
        #    startTestCrack = abot
        #    newPOD = 1.0

        #    while(bound_95_inv > abot and newPOD > .01):
        #        a_t = a_fwd(startTestCrack, self.a_transform)
        #        zhat = (a_t-self.muhat) / self.sighat      
        #        newPOD = mdnord(zhat)      
        #        se_zhat = sqrt(v[0]+(2.0*v[1]+v[2]*zhat)*zhat)
        #        bound_95 = a_t + self.z95 * se_zhat
        #        bound_95_inv = a_inv(bound_95, self.a_transform)

        #        if(bound_95_inv > abot):
        #                startTestCrack /= 2.0

        #    abot = startTestCrack

        #    bound_95_inv = atop - 1
        #    startTestCrack = atop
        #    newPOD = .9

        #    while(bound_95_inv < atop and newPOD < .99):
        #        a_t = a_fwd(startTestCrack, self.a_transform)
        #        zhat = (a_t-self.muhat) / self.sighat                       
        #        se_zhat = sqrt(v[0]+(2.0*v[1]+v[2]*zhat)*zhat)
        #        bound_95 = a_t + self.z95 * se_zhat
        #        bound_95_inv = a_inv(bound_95, self.a_transform)
        #        new_zHat = (bound_95-self.muhat) / self.sighat
        #        newPOD = mdnord(new_zHat)     

        #        if(bound_95_inv < atop):
        #                startTestCrack *= 2.0

        #    atop = startTestCrack

        

        podBottom = 1.0
        tries = 0

        #try to make it so pod at least goes to .01 for the POD curve
        while podBottom > .01 and tries < 25 and self.m9095 > 0.0:
            zHatBottom = (a_fwd(abot, self.a_transform)-self.muhat) / self.sighat
            abot_t = a_fwd(abot, self.a_transform)
            h = chi * (v[0] + (v[2] * zHatBottom + 2.0 * v[1]) * zHatBottom)
            if h > 0.0:
                zcl = zHatBottom - sqrt(h) / self.sighat
            else:
                zcl = 0.0     
                
            se_zhatBottom = sqrt(v[0]+(2.0*v[1]+v[2]*zHatBottom)*zHatBottom)
            bound_95Bottom = abot_t + self.z95 * se_zhatBottom   
        
            zHatBottom = (bound_95Bottom-self.muhat) / self.sighat

            podBottom = mdnord(zHatBottom)
            tries += 1

            if podBottom > .01:
                if abot > 0:
                    abot /= 1.5
                else:
                    abot *= 1.5

                n += 2

        delta = (atop-abot)/(n-1)

        z = [0 for i in range(n)]

        for i in range(n):
            a = abot + delta * i
            if self.sighat > 0.0:
                z[i] = (a_fwd(a, self.a_transform)-self.muhat) / self.sighat
            else:
                z[i] = 0.0

        for i in range(n):
            a = abot + delta * i
            a_t = a_fwd(a, self.a_transform)
            zhat = z[i]
            h = chi * (v[0] + (v[2] * zhat + 2.0 * v[1]) * zhat)
            if h > 0.0 and self.sighat > 0.0:
                zcl = zhat - sqrt(h) / self.sighat
            else:
                zcl = 0.0         
                
            #self.pod_crksize += [a]
            #self.pod_crksizef += [a_t]
            #self.pod += [mdnord(zhat)]   

            if self.m9095 > 0.0 and self.sighat > 0.0:
                #p value we are using for calculating the 95 curve 
                self.pod_bounds += [mdnord(zhat)]              
                se_zhat = sqrt(v[0]+(2.0*v[1]+v[2]*zhat)*zhat)
                bound_95 = a_t + self.z95 * se_zhat
                bound_95_inv = a_inv(bound_95, self.a_transform)
                self.pod_bounds_crksizef += [bound_95]
                self.pod_bounds_crksize += [bound_95_inv]
                #comment out until we fix POD curve

                ##crack size associated with that p value
                self.pod_crksizef += [bound_95]
                self.pod_crksize += [bound_95_inv]
                #p value for the pod curve for the 95 curve's crack size
                new_zHat = (bound_95-self.muhat) / self.sighat
                self.pod += [mdnord(new_zHat)]
            else:
                self.pod_crksize += [a]
                self.pod_crksizef += [a_t]
                self.pod += [mdnord(zhat)]

        #bound on calc min and resonable POD min

        keepDeleting = True

        calculatedBounds = len(self.pod_bounds) > 0

        while(keepDeleting):

            cracksLeft = len(self.pod_crksize)
            if cracksLeft > 0:
                smallcrack = self.pod_crksize[0]
                smallpod = self.pod[0]
            else:
                smallcrack = 0.0
                smallpod = .001

            #want the bigger of pod and bounds as our threshold
            #so we don't prematurely cut off the smaller curve
            if calculatedBounds and smallpod < self.pod_bounds[0]:
                smallpod = self.pod_bounds[0]

            keepDeleting = cracksLeft > 0 and (smallcrack < 0.0 or smallpod < .001)# or smallcrack < self.calcmin)

            if keepDeleting:
                del self.pod_crksize[0]
                del self.pod_crksizef[0]
                del self.pod[0]
                if calculatedBounds:
                    del self.pod_bounds[0]
                    del self.pod_bounds_crksize[0]
                    del self.pod_bounds_crksizef[0]

        keepDeleting = True

        #bound on calc max and resonable POD 95 max
        while keepDeleting:
            cracksLeft = len(self.pod_crksize)
            lastCrackIndex = cracksLeft-1

            if cracksLeft > 0:
                largecrack = self.pod_crksize[lastCrackIndex]
                largepod = self.pod[lastCrackIndex]
            else:
                largecrack = 0.0
                largepod = .999

            #want the smaller of pod and bounds as our threshold
            #so we don't prematurely cut off the bigger curve
            if calculatedBounds and largepod > self.pod_bounds[lastCrackIndex]:
                largepod = self.pod_bounds[lastCrackIndex]

            keepDeleting = cracksLeft > 0 and (largecrack < 0.0 or largepod > .999)# or largecrack > self.calcmax)

            if keepDeleting:
                del self.pod_crksize[lastCrackIndex]
                del self.pod_crksizef[lastCrackIndex]
                del self.pod[lastCrackIndex]
                if calculatedBounds:
                    del self.pod_bounds[lastCrackIndex]
                    del self.pod_bounds_crksize[lastCrackIndex]
                    del self.pod_bounds_crksizef[lastCrackIndex]

        self.SetNewStatus(self, "Finished Calculating POD Curve")

    
    def pf_pod(self):

        self.SetNewStatus(self, "Started Calculating POD Curve")

        chi = cbound(self.npts)
        n = 150
        abot = a_inv(self.muhat - 6.0*self.sighat,  self.a_transform)
        atop = a_inv(self.muhat + 6.0*self.sighat,  self.a_transform)
        if abot<self.calcmin: 
            abot = self.calcmin
        if self.calcmax<atop: 
            atop = self.calcmax
        delta = (atop-abot)/(n-1)
        z95 = phinv(self.confidence_level);
        self.pf_POD_a = []
        self.pf_POD_af = []
        self.pf_POD_poda = []
        self.pf_POD_conf95 = []
        self.pf_POD_ptxpt = []
        
        #calculate here so we know the range to calculate for the 95 curve
        for i in range(n):
            i = float(i)
            a = abot + delta*i
            a_t = a_fwd(a, self.a_transform)
            zhat = (a_t-self.muhat)/self.sighat 
            cov = self.pf_covariance
            h = chi*(cov[0]+(cov[2]*zhat+2.0*cov[1])*zhat)
            if h>0.0:
                zcl = zhat - sqrt(h)/self.sighat
            else: 
                zcl = 0.0
            if (self.model==LINEAR_MODEL):
                pod = mdnord(zhat)
            else:
                pod = pod_odds(zhat)
            pcl = mdnord(zcl)
            zhat = phinv(pod)
            alb = a_inv(self.muhat+zhat*self.sighat
                +z95*sqrt(cov[0]+zhat*(2.0*cov[1]+zhat*cov[2])),  self.a_transform);
            self.pf_POD_a += [a]
            self.pf_POD_af += [a_t]
            self.pf_POD_poda += [pod]
            self.pf_POD_conf95 += [pcl]
            self.pf_POD_ptxpt += [alb]

        #need to do the test here to decide if we want to attempt to do the 95 curve 
        self.pf_tests()

        self.CalcNewPFPOD()

        self.pf_POD_a = []
        self.pf_POD_af = []
        self.pf_POD_poda = []
        self.pf_POD_conf95 = []
        self.pf_POD_ptxpt = []
        
        #recalculating for specific crack sizes so POD and 95 curve share the same x-axis
        for a in self.new_pf_a:
            a_t = a_fwd(a, self.a_transform)
            zhat = (a_t-self.muhat)/self.sighat 
            cov = self.pf_covariance
            h = chi*(cov[0]+(cov[2]*zhat+2.0*cov[1])*zhat)
            if h>0.0:
                zcl = zhat - sqrt(h)/self.sighat
            else: 
                zcl = 0.0
            if (self.model==LINEAR_MODEL):
                pod = mdnord(zhat)
            else:
                pod = pod_odds(zhat)
            pcl = mdnord(zcl)
            zhat = phinv(pod)
            alb = a_inv(self.muhat+zhat*self.sighat
                +z95*sqrt(cov[0]+zhat*(2.0*cov[1]+zhat*cov[2])),  self.a_transform);
            self.pf_POD_a += [a]
            self.pf_POD_af += [a_t]
            self.pf_POD_poda += [pod]
            self.pf_POD_conf95 += [pcl]
            self.pf_POD_ptxpt += [alb]

        self.SetNewStatus(self, "Finished Calculating POD Curve")
        
        #for i in range(n):
        #    i = float(i)
        #    a = abot + delta*i
        #    a_t = a_fwd(a, self.a_transform)
        #    zhat = (a_fwd(a, self.a_transform)-self.muhat)/self.sighat 
        #    cov = self.pf_covariance
        #    h = chi*(cov[0]+(cov[2]*zhat+2.0*cov[1])*zhat)
        #    if h>0.0:
        #        zcl = zhat - sqrt(h)/self.sighat
        #    else: 
        #        zcl = 0.0
        #    if (self.model==LINEAR_MODEL):
        #        pod = mdnord(zhat)
        #    else:
        #        pod = pod_odds(zhat)
        #    pcl = mdnord(zcl)
        #    zhat = phinv(pod)
        #    alb = a_inv(self.muhat+zhat*self.sighat
        #        +z95*sqrt(cov[0]+zhat*(2.0*cov[1]+zhat*cov[2])),  self.a_transform);
        #    self.pf_POD_a += [a]
        #    self.pf_POD_af += [a_t]
        #    self.pf_POD_poda += [pod]
        #    self.pf_POD_conf95 += [pcl]
        #    self.pf_POD_ptxpt += [alb]

    def ahat_decision(self):

        if self.m9095 > 0.0:
            self.SetNewStatus(self, "Started Calculating Threshold Curve")

        self.decision_table_thresholds = []
        self.decision_a50 = []
        self.decision_level = []
        self.decision_confidence = []
        self.decision_V11 = []
        self.decision_V12 = []
        self.decision_V22 = []
        npts2 = self.npts + self.acount + self.bcount

        if npts2 > 0:
            chi = cbound(npts2)
        else:
            chi = cbound(1)

        zp = phinv(self.pod_level)
        xp = zp*self.sighat
        sighat = self.sighat

        #store so you can restore values after calculating decision thresholds
        #tempSigHat = self.sighat
        #tempMuHat = self.muhat
        #tempMHat = self.mhat

        if self.fit_errors[1] != 0.0:
            tstsig = self.linfit['slope'] / self.fit_errors[1]  #sighat*sighat/v2[2];
        else:
            tstsig = self.z95 - 1 #force tstssig error

        for i in range(len(self.decision_thresholds)):
            if i+1 < len(self.decision_thresholds):
                nsteps = 10
                step = (self.decision_thresholds[i+1]-self.decision_thresholds[i])/nsteps
            else:
                nsteps = 1
                step = 0.0
            for j in range(nsteps):
                #threshold = self.decision_thresholds[i] + step * j
                #self.decision_table_thresholds += [threshold]
                #self.muhat = (r_fwd(threshold,  self.ahat_transform) - self.linfit['intercept'])/self.linfit['slope'];
                #tmp = (self.linfit['rms']/self.linfit['slope'])**2;
                #v2 = [-(self.variance[0][0]+(2.0*self.variance[0][1]+self.variance[1][1]*self.muhat)*self.muhat)*tmp, 
                #-(self.sighat*(self.variance[0][1]+self.muhat*self.variance[1][1])-self.variance[0][2]-self.muhat*self.variance[1][2])*tmp, 
                #-(self.variance[1][1]*self.sighat*self.sighat-2.0*self.sighat*self.variance[1][2]+self.variance[2][2])*tmp]
                #deter = v2[0]*v2[2]-v2[1]*v2[1];
                #tstsig = sighat*sighat/v2[2];
                #a90 = self.muhat + xp
                #m90 = a_inv(a90, self.a_transform)
                #mhat = a_inv(self.muhat,  self.a_transform);
                #self.decision_a50 += [mhat]
                #self.decision_level += [m90]
                #if (tstsig < chi):
                #    m9095 = 0.0
                #else:
                #    a9095 = sighat*(v2[0]+(2.0*v2[1]+v2[2]*zp)*zp);
                #    a9095 = a90 + a9095/(sqrt(sighat*a9095/chi-deter)-v2[1]-v2[2]*zp);
                #    m9095 = a_inv(a9095,  self.a_transform);
                #self.decision_confidence += [m9095]
                #self.decision_V11 += [v2[0]]
                #self.decision_V12 += [v2[1]]
                #self.decision_V22 += [v2[2]]
                threshold = self.decision_thresholds[i] + step * j
                self.decision_table_thresholds += [threshold]

                if self.linfit['slope'] > 0.0:
                    muhat = (r_fwd(threshold,  self.ahat_transform) - self.linfit['intercept'])/self.linfit['slope'];
                    tmp = (self.linfit['rms']/self.linfit['slope'])**2;
                else:
                    muhat = 0.0
                    tmp = 0.0

                v2 = [-(self.variance[0][0]+(2.0*self.variance[0][1]+self.variance[1][1]*muhat)*muhat)*tmp, 
                -(sighat*(self.variance[0][1]+muhat*self.variance[1][1])-self.variance[0][2]-muhat*self.variance[1][2])*tmp, 
                -(self.variance[1][1]*sighat*sighat-2.0*sighat*self.variance[1][2]+self.variance[2][2])*tmp]
                deter = v2[0]*v2[2]-v2[1]*v2[1];
                
                a90 = muhat + xp
                m90 = a_inv(a90, self.a_transform)
                mhat = a_inv(muhat,  self.a_transform);
                self.decision_a50 += [mhat]
                self.decision_level += [m90]
                if (tstsig < self.z95 or v2[0] < 0.0 or v2[2] < 0.0):
                    m9095 = 0.0
                else:

                    se_a90 = sqrt(v2[0]+(2.0*v2[1]+v2[2]*self.z90)*self.z90)
                    a9095 = a90 + self.z95 * se_a90
                    m9095 = a_inv(a9095,  self.a_transform);

                    #a9095 = sighat*(v2[0]+(2.0*v2[1]+v2[2]*zp)*zp);
                    #a9095 = a90 + a9095/(sqrt(sighat*a9095/chi-deter)-v2[1]-v2[2]*zp);
                    #m9095 = a_inv(a9095,  self.a_transform);

                self.decision_confidence += [m9095]
                self.decision_V11 += [v2[0]]
                self.decision_V12 += [v2[1]]
                self.decision_V22 += [v2[2]]

        #crack sizes < 0.0 make no sense
        while(len(self.decision_level) > 0 and (self.decision_level[0] < 0.0 or self.decision_a50[0] < 0.0)):
            del self.decision_a50[0]
            del self.decision_level[0]
            del self.decision_confidence[0]
            del self.decision_V11[0]
            del self.decision_V12[0]
            del self.decision_V22[0]

        #restore decision threshold calculation after calculating range of decision thresholds
        #self.sighat = tempSigHat
        #self.muhat = tempMuHat
        #self.mhat = tempMHat

        if self.m9095 > 0.0:
            self.SetNewStatus(self, "Finished Calculating Threshold Curve")

#if __name__ == '__main__':
    
#    import pickle
#    #######################################################
#    if False: # Do the ahat analysis
#        pDoc = CPodDoc()
#        a_trans = TRANS_LOG
#        ahat_trans = TRANS_LOG
###            pDoc.a_transform = a_trans
###            pDoc.ahat_transform = ahat_trans

#        # This is the AHAT2.xls data
#        pDoc.SetFlawData([2.2, 2.2, 2.2, 2.2, 2.2, 2.2, 2.2, 2.2, 2.4, 2.4, 2.7, 2.9, 2.9, 2.9,
#        2.9, 3.1, 3.3, 3.3, 3.3, 3.5, 3.7, 4.6, 4.6, 5.1, 5.1, 5.1, 5.5,
#        5.5, 4.6, 5.9, 5.9, 5.5, 6.4, 6.4, 6.8, 6.8, 7.3, 7.3, 7.7, 7.7,
#        8.1, 8.6, 9, 9, 8.8, 9.5, 9.9, 9.9, 9.9, 10.8, 11.2, 11.7, 11.7,
#        12.5, 12.1, 13, 13.4, 13.9, 14.3, 15.6, 16.1],  XF_LOG)
        
#        pDoc.SetResponseData({
#        'A11': [float('NaN'), float('NaN'), float('NaN'), float('NaN'), float('NaN'), float('NaN'), float('NaN'),   
#            35, 35, 35, 35, 35, 35, 35, 35, 35, 35,
#            35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 41, 35, 35, 60, 57,
#            76, 55, 90, 65, 78, 70, 85, 107, 131, 113, 140, 111, 227, 128, 183,
#            215, 185, 203, 210, 210, 233, 201, 286, 286, 321, 385, 366],
#            #215, 185, 203, 210, 210, 233, 201, 286, 286, 321, 385, float('NaN')],
#        'A21':[float('NaN'), float('NaN'), float('NaN'), float('NaN'), float('NaN'), float('NaN'), float('NaN'),  
#            35, 35, 35, 35, 35, 35, 35, 35, 35, 35,
#            35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 42, 41, 35, 65, 62,
#            80, 55, 95, 67, 78, 77, 92, 121, 136, 112, 145, 112, 231, 124, 189,
#            230, 187, 205, 212, 210, 198, 230, 308, 289, 309, 342, 376],
#            #230, 187, 205, 212, 210, 198, 230, 308, 289, 309, 342, float('NaN')],
#        'A12':[float('NaN'), float('NaN'), float('NaN'), float('NaN'), float('NaN'), float('NaN'), float('NaN'),   
#            35, 35, 35, 35, 35, 35, 35, 35, 35, 35,
#            35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 44, 39, 35, 61, 61,
#            81, 55, 92, 65, 80, 86, 88, 114, 135, 113, 142, 111, 228, 128, 184,
#            226, 184, 208, 212, 211, 242, 277, 314, 295, 317, 349, 388]}, XF_LOG)
#            #226, 184, 208, 212, 211, 242, 277, 314, 295, 317, 349, float('NaN')]}, XF_LOG)
#        #pDoc.SetFlawMaxSize(10.0)
#        pDoc.SetResponseMin(35)
#        pDoc.SetResponseMax(1360)
#        pDoc.SetDecisionThresholds([35,  50,  75,  100,  150,  200,  300,  400])
#        pDoc.SetPODThreshold(55)
#        pDoc.SetPODLevel(0.90)
#        pDoc.SetPODLevel(0.99)
#        pDoc.SetPODLevel(0.95)
#        pDoc.SetPODLevel()
        
#        pDoc.SetPODConfidence(0.90)
#        pDoc.SetPODConfidence(0.99)
#        pDoc.SetPODConfidence(0.95)

#        if a_trans == TRANS_NONE:
#            a_str = "a"
#        else:
#            a_str = "Log a"
#        if ahat_trans == TRANS_NONE:
#            ahat_str = "ahat"
#        else:
#            ahat_str = "Log ahat"

#        analysis_str = a_str + " vs " + ahat_str

#        print(analysis_str + " analysis:")

#        pDoc.OnAnalysis()
        
#        #################################################
#        if False:
#            #Save the results as a pickled python object we can reuse in other files
#            f = open("ahatsavedata.txt",  'w')
#            ahatsavedata = pickle.Pickler(f)
#            ahatsavedata.dump(pDoc)
#            f.close

#    ###################################################
#    if False:  # Print ahat data tables?
#        print("Input Data")
#        print("Seq. #\tLength\tA11\tA21\tA12")
#        for i in range(len(pDoc.flaws)):
#            flaw = pDoc.flaws[i]
#            output = [i,  flaw.crksize] + [int(meas.value) for meas in flaw.rdata]
#            pretty_print(output)
#        R = pDoc.GetResidualTable()
#        C = pDoc.GetCensoredTable()
#        F = pDoc.GetFitTable()
#        P = pDoc.GetPODTable()
#        T = pDoc.GetThresholdTable()
#        print("Residual Table")
#        pretty_print(["a","ln(a)","ahat","ln(ahat)","fit","dif", str(R.keys()[6]),str(R.keys()[7])])
        
#        for i in range(len(R['flaw'])):
##            output = [R['flaw'][i], 
##            R['t_flaw'][i], 
##            R['ave_response'][i],  
##            R['t_ave_response'][i], 
##            R['t_fit'][i],  
##            R['t_diff'][i]]
#            output = [R[key][i] for key in R.keys()]
#            pretty_print(output)
            
#        #print(R)
#        print("Censored Table")
#        print("a\tln(a)\tahat\tln(ahat)")
#        for i in range(len(C['flaw'])):
#            output = [C['flaw'][i], 
#            C['t_flaw'][i], 
#            C['ave_response'][i],  
#            C['t_ave_response'][i]
#            ]
#            pretty_print(output)
#        #print(C)
#        print("Fit Table")
#        print("a\tfit\tA11\tA21\tA12")
#        for i in range(len(F['flaw'])):
#            output = [F['flaw'][i], 
#            F['fit'][i],
#            F['A11'][i],
#            F['A21'][i],  
#            F['A12'][i]]
#            pretty_print(output)
#        #print(F)
#        print("POD Table")
#        print("a\tPOD(a)\t95% bounds")
#        for i in range(len(P['flaw'])):
#            output = [P['flaw'][i],
#            P['pod'][i], 
#            P['confidence'][i]]
#            pretty_print(output)
            
#        print("Inspection Threshold\ta50\ta90\ta90/95")
#        for i in range(len(T['threshold'])):
#            output = [T['threshold'][i], 
#            T['flaw50'][i], 
#            T['level'][i], 
#            T['confidence'][i]
#            ]
#            pretty_print(output)
    
#        ##################################################
#        if False:  # If true, spit out ahat plots
#            import podplot
            
#            podplot.fit((F['flaw'],R['ave_response']),
#            (F['flaw'],F['fit'],pDoc.linfit['rms']),
#            "mean")
            
#            podplot.fit((F['flaw']+F['flaw']+F['flaw'],F['A11']+F['A21']+F['A12']),
#            (F['flaw'],F['fit'],pDoc.linfit['rms']),
#            "plot")
            
#            podplot.resid((F['flaw'], R['t_diff']), "-resid")
            
#            podplot.POD(P['flaw'],  P['pod'],  P['confidence'])
            
#            podplot.Threshold(T['threshold'], T['level'],  T['confidence'])
        
#    #######################################################
#    if False:  # Print aHat parameters
#        print("Max and Min Flaw:")
#        print(pDoc.GetAnalyzedFlawRangeMax())
#        print(pDoc.GetAnalyzedFlawRangeMin())
        
#        print("All Cracks, Analyzed, Partial Below,  Partial Above,  Full Below,  Full Above")
#        print(
#        len(pDoc.flaws), 
#        pDoc.GetAnalyzedFlawCount(), 
#        pDoc.GetFlawCountPartialBelowResponseMin(), 
#        pDoc.GetFlawCountPartialAboveResponseMax(), 
#        pDoc.GetFlawCountFullBelowResponseMin(), 
#        pDoc.GetFlawCountFullAboveResponseMax())
        
#        print("Intercept B, Slope M, and RMS Error")
#        print((
#        pDoc.GetModelIntercept(), 
#        pDoc.GetModelSlope(), 
#        pDoc.GetModelResidual()
#        ))
#        print("Uncertainties:")
#        print((
#        pDoc.GetModelInterceptError(), 
#        pDoc.GetModelSlopeError(), 
#        pDoc.GetModelResidualError()
#        ))
        
#        print("Repeatability Error")
#        print(pDoc.GetRepeatabilityError())
        
#        print("Normality: Anderson-Darling")
#        print(pDoc.GetNormality())
        
#        print("Equal Variance: Bartlett")
#        print(pDoc.GetEqualVariance())
        
#        print("Lack of fit: Pure Error (df=9)")
#        print(pDoc.GetLackOfFit())
        
#        print("Sigma:")
#        print(pDoc.GetPODSigma())
        
#        print("pDoc.GetPlotMinFlaw()")
#        print(pDoc.GetPlotMinFlaw())
        
#        print("pDoc.GetPlotMaxFlaw() ")
#        print(pDoc.GetPlotMaxFlaw() )
        
#        print("pDoc.GetPlotMinFlawMinResponse()")
#        print(pDoc.GetPlotMinFlawMinResponse())
        
#        print("pDoc.GetPlotMinFlawMaxResponse()")
#        print(pDoc.GetPlotMinFlawMaxResponse())
        
#        print("pDoc.GetPlotMaxFlawMinResponse()")
#        print(pDoc.GetPlotMaxFlawMinResponse())
        
#        print("pDoc.GetPlotMaxFlawMaxResponse()")
#        print(pDoc.GetPlotMaxFlawMaxResponse())
        
#        print("pDoc.GetFitMinFlaw()")
#        print(pDoc.GetFitMinFlaw())
        
#        print("pDoc.GetFitMaxFlaw()")
#        print(pDoc.GetFitMaxFlaw())
        
#        print("pDoc.GetFitMinResponse()")
#        print(pDoc.GetFitMinResponse())

#        print("pDoc.GetFitMaxResponse()")
#        print(pDoc.GetFitMaxResponse())
    
#    #########################################################
#    if True:  # Do PF analysis
#        DEBUG = False
#        pfTest = CPodDoc()
#        pfTest.SetFlawData(flaws=[2.5,2.5,2.5,2.5,2.5,2.5,2.5,2.5,2.5,2.5,4.28,4.28,4.28,4.28,5.17,5.17,5.17,5.17,5.17,
#        5.17,5.17,5.17,5.615,5.615,5.615,5.615,5.615,5.615,5.615,5.615,6.06,6.06,6.06,6.06,6.06,6.06,6.06,6.06,6.505,
#        6.505,6.505,6.505,6.505,6.505,6.505,6.505,6.505,6.505,6.95,6.95,6.95,6.95,6.95,7.395,7.395,7.395,7.84,7.84,
#        7.84,8.285,8.285,8.285,8.73,8.73,8.73,8.73,8.73,8.73,9.62,9.62,9.62,9.62,10.065,10.51,10.51,10.51,10.688,10.955,
#        10.955,11.934,11.934,11.934,11.934,11.934,11.934,11.934,12.379,12.468,12.468,12.468,12.557,12.557,13.18,
#        13.625,13.625,13.714,13.803,13.803,13.803,13.803,14.07,14.96,15.1736,15.405,15.939,15.939,15.939,16.562,
#        16.562,16.74,16.74,16.74,17.185,17.541,18.1462,18.1462,18.52,18.52,18.7514,19.2676,19.3744,19.855,20.389,
#        21.19,22.08,22.08,22.6229,22.97,22.97,22.97,23.415,27.8116,27.8116,27.8116,27.8116,27.8116,27.8116,27.8116,
#        27.8116,27.8116,27.8116,27.8116,27.8116,27.8116], 
#        transform=TRANS_LOG,
#        sequence_numbers=[28,29,54,57,65,74,91,92,93,94,30,45,47,56,31,32,37,44,46,58,95,100,9,18,36,43,48,73,99,
#        126,10,12,33,35,38,40,53,122,1,2,3,4,7,8,27,51,66,75,42,67,83,96,121,5,6,39,11,49,97,52,84,144,19,34,50,59,
#        118,120,13,41,55,64,98,89,117,119,68,17,90,60,69,70,72,77,80,82,101,62,63,71,87,123,85,15,76,61,86,88,124,
#        125,14,143,26,24,78,79,81,127,128,20,22,25,16,136,108,109,21,142,135,102,129,130,116,137,23,141,110,103,
#        115,138,114,104,105,106,107,111,112,113,131,132,133,134,139,140])
        
#        pfTest.SetResponseData({'Ins 1':[0,0,1,0,1,1,0,0,0,1,0,0,0,0,0,0,0,1,0,1,1,1,0,0,1,1,1,1,1,1,0,0,0,1,1,1,1,1,0,
#        0,0,0,0,0,0,1,1,1,1,1,1,1,0,0,0,1,0,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
#        1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1]}) 
        
#        pfTest.analysis = PF_ANALYSIS
#        #pfTest.SetFlawMaxSize(30.0)
#        pfTest.OnAnalysis()
        
#        #################################################
#        #Save the results as a pickled python object we can reuse in other files
#        if False:
#            f = open("pfsavedata.txt",  'w')
#            pfsavedata = pickle.Pickler(f)
#            pfsavedata.dump(pfTest)
#            f.close()
        
#        #################################################
#        if False:
#            i=0
#            R = pfTest.GetPFResidualTable()
#            print("flaw \t t_flaw \t hit rate \t t_fit \t diff")
#            for i in range(len(R['flaw'])):
#                output = [R['flaw'][i], 
#                R['t_flaw'][i], 
#                R['hitrate'][i],  
#                R['t_fit'][i],  
#                R['diff'][i]]
#                pretty_print(output)
            
#            P = pfTest.GetPFPODTable()
#            print("POD Table")
#            print("a\tPOD(a)\t95% bounds")
#            for i in range(len(P['flaw'])):
#                output = [P['flaw'][i],
#                P['pod'][i], 
#                P['confidence'][i]]
#                pretty_print(output)
        
#        if False:
#            for each in range(0,100):
#                b=2.0+.01*each
#                print("gamma initial="+str(b))
#                newtable = pfTest.GetNewPFTable(gamma=b)
#                print("gamma " + str(newtable['gamma'][0]) + " b " + str(newtable['b'][0]) )            
#                print("new a90/95 " + str(newtable['new a90/95']))
#        if False:
#            title = ""
#            for key in newtable.keys():
#                title += key
#                title += '\t'
#            print(title)
            
#            for i in range(len(newtable[newtable.keys()[0]])):
#                output = []
#                for key in newtable.keys():
#                    output += [newtable[key][i]]
#                pretty_print(output)
            
#            print("mu")
#            print(pfTest.muhat)
#            print("sigma")
#            print(pfTest.sighat)
            
#        if True:
#            print("POD     gamma           b               a9095")
#            new_bounds = pf_new_bounds(pfTest)
#            for i in range(5,95):
#                gamma = pfTest.x90
#                b = 1/(2*pfTest.sighat)
#                (gamma, b) = new_bounds.solve(i/100.0,  gamma,  b, pfTest.muhat,  pfTest.sighat)
#                print(str(i/100.0) + '\t' + str(gamma) + '\t' + str(b) + '\t' + str(a_inv(gamma,pfTest.a_transform)))
                
            
        
#        #pfTest.model = ODDS_MODEL
#        #pfTest.OnAnalysis()
        
        


