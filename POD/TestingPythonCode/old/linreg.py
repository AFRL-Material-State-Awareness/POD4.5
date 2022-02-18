from mdnord import mdnord
from math import sqrt

class linear_regression():
    'This does what it says.'
    def __init__(self):
        self.slope=0.0
        self.intercept=0.0
        self.rms=0.0
        self.xbar=0.0 
        self.ybar=0.0
        self.des = [0 for each in range(5)];
        self.anova = [0 for each in range(14)];
        self.stat = [0 for each in range(9)];
    
    def regress(self, x, y, npts):
        sumx = sumy = sumx2 = sumy2 = sumxy = 0.0
        for i in range(npts) :
            xv = x[i];
            yv = y[i];
            sumx += xv;
            sumy += yv;
            sumx2 += xv*xv;
            sumy2 += yv*yv;
            sumxy += xv*yv;

        self.xbar = sumx/npts;
        xbar=self.xbar
        self.ybar = sumy/npts;
        ybar = self.ybar
        axx = sumx2/npts;
        axy = sumxy/npts;
        ayy = sumy2/npts;
        cxx = axx-xbar*xbar;
        cxy = axy-xbar*ybar;
        cyy = ayy-ybar*ybar;
        self.slope = cxy/cxx;
        slope = self.slope
        self.intercept = ybar-slope*xbar;
        intercept = self.intercept
        rsqd = ((intercept*ybar)+(slope*xbar)-ybar*ybar)/cyy;
        self.rms=sqrt(cyy-slope*cxy);

        self.des[0] = xbar;
        self.des[1] = ybar;
        varx= cxx*npts/(npts-1.0);
        vary= cyy*npts/(npts-1.0);
        self.des[2] = sqrt(varx);
        self.des[3] = sqrt(vary);

        syx=sqrt((vary-(slope*slope*varx))*(npts-1.)/(npts-2.));
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



