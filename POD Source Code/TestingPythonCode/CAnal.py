
from PODglobals import *
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

