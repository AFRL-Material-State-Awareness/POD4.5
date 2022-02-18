from __future__ import division
from scipy import stats

for i in range(10000):
    #input = (i-5000)/1000
    #output = stats.norm(0,1).cdf(input)
    #output = stats.norm(0,1).pdf(input)
    input = (i/10000)
    output = stats.chi2.isf(input, 1)
    print('('+str(input)+','+str(output)+'),')
