from mdnord import mdnord
from math import sqrt

DEBUG = False

class linear_regression():
    'This does what it says.'
    def __init__(self):
        self.slope=0.0
        self.intercept=0.0
        self.rmse=0.0
        self.xbar=0.0
        self.ybar=0.0
        self.des = [0 for each in range(5)];
        self.anova = [0 for each in range(14)];
        self.stat = [0 for each in range(9)];
        self.SEmean = []
        self.SEpred = []

    def regress(self, x, y, npts):
        sumx = 0.0
        sumy = 0.0
        sumx2 = 0.0
        sumy2 = 0.0
        sumxy = 0.0
        for i in range(npts) :
            xv = x[i];  # Get each x value
            yv = y[i];  # Get each y value
            sumx += xv;  # sum up each x
            sumy += yv;  # sum up each y
            sumx2 += xv*xv;  # sum up each x^2=
            sumy2 += yv*yv;  # sum up each y^2
            sumxy += xv*yv;  # sum up each x*y

        self.xbar = sumx/npts;  # mean of x
        xbar=self.xbar               #   "
        self.ybar = sumy/npts;  # mean of y
        ybar = self.ybar             #   "
        axx = sumx2/npts;         
        axy = sumxy/npts;
        ayy = sumy2/npts;
        cxx = axx-xbar*xbar;
        cxy = axy-xbar*ybar;
        cyy = ayy-ybar*ybar;
        ssx = sumx2 - sumx * sumx / npts  # Pete equation 1
        ssy = sumy2 - sumy * sumy / npts  # Pete equation 2
        ssxy = sumxy - sumx * sumy / npts  # Pete equation 3
        b1 = ssxy / ssx  # Pete slope
        b0 = ybar - b1 * xbar  # Pete intercept
        
        self.slope = cxy/cxx;
        slope = self.slope
        self.intercept = ybar-slope*xbar;
        intercept = self.intercept
        if DEBUG:
            print("Old slope: " + str(slope))
            print("New slope: " + str(b1))
            
            print("Old intercept: " + str(intercept))
            print("New intercept; " + str(b0))
        #self.rsqd = ((intercept*ybar)+(slope*xbar)-ybar*ybar)/cyy;
        self.rmse=sqrt(cyy-slope*cxy);
        SSres = ssy - b1 * ssx
        MSres = SSres / (npts - 2)
        
        if DEBUG:
            print ("Old RMSE: " + str(self.rmse))
            print("New SSres: " + str(SSres))
            print("New MSres: " + str(MSres))
        
        self.SEmean = []
        self.SEpred = []
        #prevI = -100000.0
        if DEBUG:
            print("x\ty\tSEmean\tSEpred ")
        for i in range(len(x)):
        #    if i <= prevI:
            thisSEmean = sqrt(abs(MSres * (1/npts + (x[i] - xbar) ** 2 / npts)))
            self.SEmean += [thisSEmean]
            thisSEpred = sqrt(abs(MSres * (1 + 1/npts + (x[i] - xbar) ** 2 / npts)))
            self.SEpred += [thisSEpred]
        #    prevI = i
            if DEBUG:
                print(str(x[i]) + "\t" + str(y[i]) 
                    + "\t" + str(thisSEmean) 
                    + "\t" + str(thisSEpred))

        self.des[0] = xbar;
        self.des[1] = ybar;
        varx= cxx*npts/(npts-1.0);
        vary= cyy*npts/(npts-1.0);
        self.des[2] = sqrt(varx);
        self.des[3] = sqrt(vary);

        #syx=sqrt((vary-(slope*slope*varx))*(npts-1.)/(npts-2.));
        self.stat[0] = slope;
        self.stat[4] = intercept;

        self.anova[0]=1.;
        self.anova[1]=npts-2.;
        self.anova[2]=npts-1.;
        self.anova[3]=slope*slope*npts*cxx;  #// npts*cxy*cxy/cxx
        self.anova[5]=npts*cyy;
        self.anova[4]=self.anova[5]-self.anova[3];     #// npts*(cxx*cyy-cxy*cxy)/cxx
        self.anova[6]=self.anova[3]/self.anova[0];
        self.anova[7]=self.anova[4]/self.anova[1];
        self.anova[8]=self.anova[6]/self.anova[7];

        term0=(2.*self.anova[1])-1.;
        term1=(2.*self.anova[0])-1.;
        term2=self.anova[0]*self.anova[8]/self.anova[1];
        temp=(sqrt(term0*term2)-sqrt(term1))/sqrt(1.+term2);
        self.anova[9] = mdnord(temp);
        #//CALL MDNORD(TEMP,ANOVA(10))
        self.anova[9]=1.-self.anova[9];

        self.stat[5]=sqrt(self.anova[7]*sumx2/((npts*sumx2)-(sumx*sumx)));
        self.stat[1]=sqrt(self.anova[7]*npts/((npts*sumx2)-(sumx*sumx)));


if __name__ == '__main__':
    DEBUG = True
#    x = [4.6, 5.9, 5.9, 5.5, 6.4, 6.4, 6.8, 6.8, 7.3, 7.3, 7.7, 7.7,
#        8.1, 8.6, 9, 9, 8.8, 9.5, 9.9, 9.9, 9.9, 10.8, 11.2, 11.7, 11.7,
#        12.5, 12.1, 13, 13.4, 13.9, 14.3, 15.6, 16.1]
#        
#    y = [35, 41, 35, 35, 60, 57,
#        76, 55, 90, 65, 78, 70, 85, 107, 131, 113, 140, 111, 227, 128, 183,
#        215, 185, 203, 210, 210, 233, 201, 286, 286, 321, 385, 366]
        
    x = [4.6,  5.9,  5.5,  6.4,  6.8,  7.3,  7.7,  8.1,  8.6,  9,  8.8,  9.5,  9.9,  10.8,  11.2, 
        11.7,  12.5,  12.1,  13,  13.4,  13.9,  14.3,  15.6,  16.1]
    
    y = [35, 38,  35,  58.5,  65.5,  77.5,  74,  85,  107,  122,  140,  111,  179.3333333333, 
    215,  185,  206.5,  210,  233,  201,  286,  286,  321,  385,  366]

        
    for i in range(len(x)):
        print(str(x[i]) + " " + str(y[i]))
        
    lr = linear_regression()
    lr.regress(x, y, len(x))
    
    if DEBUG:
        print("Done")
    
    
