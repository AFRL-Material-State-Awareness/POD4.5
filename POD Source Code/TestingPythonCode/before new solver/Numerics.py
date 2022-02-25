import clr
import sys

clr.AddReference('MathNet.Numerics')

from MathNet.Numerics.Distributions import ContinuousUniform

class Numerics(object):
    """description of class"""

    def GetDistribution(self):

        d1=ContinuousUniform(0.0, 1.0)

        return d1

if __name__ == '__main__':

    num = Numerics()

    d1 = num.GetDistribution()

    print d1.Maximum - d1.Minimum