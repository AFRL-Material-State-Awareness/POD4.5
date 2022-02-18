#from random import random
from linreg import linear_regression
from sysolv import ahat_sysolv
from funcr import *
from qsort import *
#import xlrd

def IsString(cell): return str(cell).split(':')[0]=='text'
def IsNumber(cell): return str(cell).split(':')[0]=='number'



class CInfo():
    'A stub for the CInfo class'
    def __init__(self):
        self.name = ""
        self.row = 0
        self.value = None
        self.namecell = None
        self.valuecell  = None

    def IsNumber(self):
        return IsNumber(self.valuecell)
        
    def IsString(self):
        return IsString(self.valuecell)
    
    def __str__(self):
        return str(self.name)+" "+str(self.value)

    def SayHello(self):
        return str("hello world")+" "+str(self.row)
    

AHAT_ANALYSIS = 1
PF_ANALYSIS = 2

LINEAR_MODEL    = 0
NONLINEAR_MODEL  = 1
ODDS_MODEL       = 2

XF_NONE    = 1
XF_LOG     = 2
XF_EXP     = 3
XF_INVERSE = 4
XF_CUSTOM  = 5

MAXINFO = 32

max_ath = 30

class CDocument():
    'A stub for the CDocument class.  Dunno what this does.'

class CInspection():
	def __init__(self, value=0, ColorIndex=0, flag=False):
	    self.value = value
	    self.ColorIndex = ColorIndex
	    self.flag = flag
	
	def IsIncluded(self) : return self.ColorIndex

class CrackData():
    'Holds info about a single crack'
    def __init__(self):
        self.data_row = 0          #// Spreadsheet row
        self.size = 0 #// Crack Size
        self.ColorIndex = 0
        self.crksize = 0
        self.crkf = 0    #// transformed crack size
        self.bValid = True
        self.rdata = []
        self.nsets = 0
        self.count = 0 #// number of inspections showing (uncolored) numeric results
        #// PassFail analysis
        self.above = 0
        #// Ahat analysis
        self.ybar = 0      #// mean transformed ahat
        self.ybar_inv = 0  #// mean untransformed ahat
        self.yfit = 0      #// predicted transformed ahat
        self.yfit_inv = 0  #// predicted untransformed ahat
        self.scnt = 0
        self.ssy = 0
        self.ncensor = 0
    
    def Parse(self, row, data_column, flaw_column, pDoc) :
        j = 0
        interior = 0
        self.data_row = row
        #cell = xl.GetRange(row,flaw_column);
        mysheet = pDoc.podfile.sheet_by_name('Data')
        row = mysheet.row_values(row,0)
        self.size = row[flaw_column]
        myformat = mysheet.cell_xf_index(row,flaw_column)
        print("myformat")
        print(myformat)
        #interior = range.GetInterior();
        #ColorIndex = interior.GetColorIndex();
        #bValid = V_VT(&size)==VT_R8;
        
        if self.bValid:
	        crkf = 0.0
	        #// invalidate cracks with non-positive crack sizes
	        self.bValid = crksize>0.0
        #// read inspection data
        count = 0
        for j in range(nsets):
	        thisrange = pDoc.sheet_by_name('Data').row_values(row,j+data_column)
	        rdata += [CInspection()]
	        #interior = range.GetInterior();
	        #rdata[j].ColorIndex = interior.GetColorIndex();
	        #if (rdata[j].IsIncluded()) count++
	            
    #def ahat_censor(double tmin, double tmax);
    #def pf_censor(double tmin);
    #def ahat_write(int row, int col);
    #def IsNumber() { return (V_VT(&size)==VT_R8); }
    #def IsIncluded() { return (bValid && V_I4(&ColorIndex)==xlNone); }
    
class CPodDoc(CDocument):
    'A dummy CPodDoc class for testing purposes'
    def __init__(self):
        # GetInfo()
        self.podfile = None #use this to hold the filename??
        #self.v OLEVariant - Used this to hold file descriptor
        self.info = []
        self.info_count = 0

        # Info Worksheet access
        self.info_row = 0
        self.analysis = 0 # 0 = ahat,  1 = pass/fail
        self.model = 0 # 0 - linear, 1 = rational
        self.flaw_name = ''
        self.flaw_column = 0
        self.flaw_units = ''
        self.data_column = 0
        self.data_units = ''
        self.tmin = 0
        self.tmax = 0
        self.amax = 0

        # list of decision thresholds
        #GetAth(row)
        self.nath = 0
        self.ath = []
        self.pod_threshold = 0

        #Data worksheet
        self.titles = []
        self.nrows = 0
        self.ncols = 0
        self.flaws = []
        self.nsets = 0
        self.count = 0

        #Flaw range
        self.crkmin = 0
        self.crkmax = 0
        self.xmin = 0
        self.xmax = 0

        #input to regression routine  (rlonx[nrows], rlony[nrows])
        self.rlonx = []
        self.rlony = []
        self.pfdata = []
        self.npts = 0

        # cracks with all inspections above saturation or below threshold
        self.acount = 0 #number of cases with all inspections at maximum
        self.bcount = 0 #number with all at minimum 

        #cacks excluded from the analysis
        nexcluded = 0

        #POD parameter intial guess
        self.have_guess = False;
        self.mu_guess = 0.0
        self.sig_guess = 0.0

        #POD model parameters
        self.sighat = 0.0
        self.muhat = 0.0
        
        #Confidence bounds settings  a9095 = crack size 95% confidence (confidence_level) for 90% pod (pod_level = 0.9) 
        self.pod_level = 0
        self.confidence_level = 0
        self.asize = ''  # e.g. a90
        self.alevel = ''# e.g. a90/95
        self.clevel = '' # e.g. 95% confidence bound
        
        self.a_opt = 0
        self.r_opt = 0
        self.a_row = 0
        

        #cbound(npts)

        self.PlotTitle = ''
        #BuildTitle();
        #ResidualsChart();
        #FitChart();
        #ThresholdChart();
        #PODChart();
        #AhatChart();
        
        #XlOpen(CString filepath);
        #OnFileNew();
        #OnFileImport();

        #// Ahat analysis
        #ahat_residuals();
        #ahat_censor();
        #ahat_solve();
        #ahat_nonlinear_solve();
        #ahat_tests();
        #ahat_print();
        #ahat_decision();
        #ahat_pod();

        #// Pass/Fail analysis
        #pf_censor();
        #pf_guess();
        #pf_residuals();
        #pf_solve();
        #pf_print();
        #pf_pod();

        #OnNewDocument();
        #OnOpenDocument(LPCTSTR lpszPathName);
        #OnSaveDocument(LPCTSTR lpszPathName);
        #DeleteContents();

        #OnFileSave();
        #OnAnalysis();
        #OnUpdateTasksRecalculate(CCmdUI* pCmdUI);
        #OnTasksSortbysize();
        #OnUpdateTasksSortbysize(CCmdUI* pCmdUI);
        #OnChart2();
        #OnUpdateChart2(CCmdUI* pCmdUI);
        #OnChart1();
        #OnUpdateChart1(CCmdUI* pCmdUI);
        #OnChart3();
        #OnUpdateChart3(CCmdUI* pCmdUI);
        #OnChart4();
        #OnUpdateChart4(CCmdUI* pCmdUI);
        #OnChart5();
        #OnUpdateChart5(CCmdUI* pCmdUI);
        #self.dummyfill()
        
        # Data Worksheet
        self.GetData()
        #GetCensorCounts(counts)
        #GetCrackRange()
        #HaveData()
        self.SortData()
        #CountInsp(set)
        self.data_row = 0
    
    def GetData(self):
        i,j,n = 0,0,0
        myrange = 0
        string = ''
        
        n = 1;
              
        # cracks with all inspections above saturation or below threshold
        self.acount = 0 #number of cases with all inspections at maximum
        self.bcount = 0 #number with all at minimum
        if self.podfile:
            self.rlonx = self.podfile.sheet_by_name('Data').col_values(2,1)    
            self.rlony = self.podfile.sheet_by_name('Data').col_values(3,1)     
            self.npts = len(self.rlony)
            self.nrows = len(self.rlony)
	        
            self.row = 0
            self.col = 0
            self.rdata = [] 
            
            self.titles = self.podfile.sheet_by_name('Data').row_values(0,0)
            self.ncols = len(self.titles) - 1
            
            if self.flaw_column != 0 :
                flaw_name = titles[self.flaw_column-1]
            else:
                for i in range(len(self.titles)) :
                    if self.titles[i].lower().strip() == self.flaw_name.lower():
                        flaw_column = i
            
            self.nsets = self.ncols-self.data_column+1
            self.flaws = []
            for i in xrange(self.nrows):
                self.flaws += [CrackData()]
                self.flaws[i].Parse(i+2,self.flaw_column,self.data_column,self)
                if not self.flaw[i].bValid : continue
                self.flaw[i].crkf = a_fwd(pflaw.crksize)
            return True
            
            
        
        
        #self.dummyfill()
        

    
    def ahat_do_censor(self,tmin, tmax):
        i, j, n = 0,0,0
        acnt, bcnt, count = 0,0,0
        value = 0.0
        ybar = 0.0
        ncensor = 0
        '''
        for i in range(nsets):
            
        '''
        '''
            if (!rdata[i].IsIncluded()) {
	            rdata[i].flag = IF_Missing;
	            continue;
            }
        '''
        '''
            value = rdata[i].value();
            count++;
            if (value<=tmin) {
	            bcnt++;
	            rdata[i].flag = IF_Below;
            }
            else if (value>=tmax && tmax>tmin) {
	            acnt++;
	            rdata[i].flag = IF_Above;
            }
            else rdata[i].flag = IF_Okay;
        }
        /*
        *   If there are no inspection result values, set a flag and return
        */
        if (count==0) {
            ncensor = 6;
            return;
        }
        /*
        * If there are inspection result values both above the saturation
        * and below the recording threshold for the same flaw, notify the
        * user and continue.
        */
        if (acnt && bcnt) {
            ncensor = 5;
            return;
        }
        int cnt = acnt+bcnt;
        double rmin, rmax;
        rmin = r_fwd(tmin);
        rmax = r_fwd(tmax);

        /*
        * if all the inspection results were censored ...
        */
        if (cnt==count) {
            if (acnt) {
	            ybar = rmax;
	            ybar_inv = tmax;
	            ncensor = 1;
            }
            else if (bcnt) {
	            ybar = rmin;
	            ybar_inv = tmin;
	            ncensor = 2;
            }
            return;
        }

        double r, ysum, ysqr;
        ysum = 0.0;
        ysqr = 0.0;
        for (j=n=0; j<nsets; j++) {
            if (rdata[j].flag!=IF_Okay) continue;
            r = r_fwd(rdata[j].value());
            ysum += r;
            ysqr += r*r;
            n++;
        }
        ybar = ysum/n;
        ybar_inv = r_inv(ybar);

        /*
        *  if no values have been censored ...
        */
        if (!cnt) {
            scnt = (n==1? 1: (n-1));
            ssy = (ysqr - n*ybar*ybar);
            return;
        }

        /*
        *   if some are censored either above or below
        */
        extern double phinv(double x);
        extern double nrmden(double x);
        double pstar, theta, thab, sigmar;
        double alpha, c;

        scnt = ssy = 0.0;
        if (acnt) {
            c = rmax;
            pstar = (double) acnt / (double) count;
            theta = phinv(pstar);
            thab = theta*(c-ybar);
            sigmar = (thab + sqrt(thab*thab + ((4.0/n)*(ysqr+(n*c-2.0*ysum)*c))))/2.0;
            alpha = count*nrmden(theta)/n;
            ybar += alpha*sigmar;
            ncensor = 3;
        }
        else if (bcnt) {
            CString msg;
            TRACE("row %d crack size %g ",data_row, crksize);
            c = rmin;
            pstar = (double) bcnt / (double) count;
            theta = phinv(pstar);
            thab = theta*(ybar-c);
            TRACE("pstar %g theta %g thab %g ",pstar,theta,thab);
            sigmar = (thab + sqrt(thab*thab + ((4.0/n)*(ysqr+(n*c-2.0*ysum)*c))))/2.0;
            alpha = count*nrmden(theta)/n;
            TRACE("sigmar %g alpha %g\n",sigmar,alpha);
            ybar -= alpha*sigmar;
            ncensor = 4;
        }
        else ncensor = 7; // should never be here (since cnt = acnt+bcnt is nonzero
        ybar_inv = r_inv(ybar);
        }
        '''
    def ahat_censor(self):
        i = 0
        pdata = []

        ahat_model = self.model;
        self.rlonx = []
        self.rlony = []

        self.SortData();
        '''
        pdata = flaws;
        ssy = scnt = 0.0;
        npts = acount = bcount = 0;
        for (i=0; i<nrows; i++, pdata++) {
            pdata->ahat_censor(tmin,tmax);
            if (!pdata->IsIncluded() || !pdata->count) continue;
            switch (pdata->ncensor) {
            case 0:
            case 3:
            case 4:
	            rlonx[npts] = pdata->crkf;
	            rlony[npts] = pdata->ybar;
	            npts++;
	            scnt += pdata->scnt;
	            ssy += pdata->ssy;
	            break;
            case 1:
	            acount++;
	            break;
            case 2:
	            bcount++;
	            break;
            }
        }
        sw = sqrt(ssy/scnt);

        npts2 = npts;

        // Append censored cracks above threshold
        if (acount) {
            pdata = flaws;
            for (i=0; i<nrows; i++, pdata++) {
	            if (!pdata->IsIncluded()) continue;
	            if (pdata->ncensor!=1) continue;
	            rlonx[npts2] = pdata->crkf;
	            rlony[npts2] = pdata->ybar;
	            npts2++;
            }
        }

        // Append censored cracks below threshold
        if (bcount) {
            pdata = flaws;
            for (i=0; i<nrows; i++, pdata++) {
	            if (!pdata->IsIncluded()) continue;
	            if (pdata->ncensor!=2) continue;
	            rlonx[npts2] = pdata->crkf;
	            rlony[npts2] = pdata->ybar;
	            npts2++;
            }
        }

        ASSERT(npts2 == npts+acount+bcount);
        '''
        
        
	    
    def value(self):
        return xlrd.sheet.Cell(self.row,self.col).value #{ return V_R8(&v); }

    def GetInfo(self):
        i,j = 0,0
        podstr = ''
        xl = self.podfile
        global POD_Version, POD_Build
        if xl.nsheets <= 0 : return False
        #print("xl.nsheets: ",str(xl.nsheets))
        #print("xl.sheet_names(): ",str(xl.sheet_names()))
        if not u'Info' in set(xl.sheet_names()): 
            print("No Excel worksheet called \"Info\"")
            return False
        
        self.ath = []
        
        thiscolumn = [cell for cell in xl.sheet_by_name('Info').col(0) ]
        assert len(thiscolumn)<32             
        #print(thiscolumn)
        i = -1
        for cell in thiscolumn :
            i+=1
            if not IsString(cell):continue
            newinfo = CInfo()
            newinfo.row = i
            newinfo.name = cell.value
            newinfo.namecell = cell
            newinfo.valuecell = xl.sheet_by_name('Info').row(i)[1]
            newinfo.value = xl.sheet_by_name('Info').row(i)[1].value
            
            if newinfo.value != "" : self.info += [newinfo]
            
            if cell.value.strip().lower() == "thresholds:" : 
                self.GetAth(i)
            if cell.value.strip().lower() == "version:" :
                #BWH Write in a version number here!
                #Version in column 2
                #Build in column 3
                continue
        self.info_count = len(self.info)
        return True
    
    def GetInfo_by_name(self, name):
        name = name.strip().lower()
        for each in self.info :
            if each.name.strip().lower() == name : 
                #print("We found "+str(name)+" "+str(each.value))
                return each
        return 0
            
    def GetAth(self, row):       
        info = self.podfile.sheet_by_name('Info')
        #print("ath row "+str(row))
        for cell in info.row(row)[1:] :
            #print (str(cell))
            if not IsNumber(cell): break;
            self.ath += [cell.value]
        nath = len(self.ath)
        #print(nath)
    
    def ParseInfo(self):
        pinfo = CInfo()
        msg = ""
        
        self.analysis = AHAT_ANALYSIS
        pinfo = self.GetInfo_by_name("analysis:")
        if pinfo != 0 :
            name = pinfo.name.strip().lower()
            if name == "ahat": self.analysis = AHAT_ANALYSIS
            elif name == "pass/fail": self.analysis = PF_ANALYSIS
        
        self.model = LINEAR_MODEL
        pinfo = self.GetInfo_by_name("model:")
        if pinfo != 0 :
            name = pinfo.name.strip().lower()
            if name == "nonlinear" : self.model = NONLINEAR_MODEL
            elif name == "odds" : self.model = ODDS_MODEL
        
        self.have_guess = False
        pinfo = self.GetInfo_by_name("guess:")
        if pinfo != 0 :
            self.mu_guess = pinfo.value
            sig_cell = self.podfile.sheet_by_name('Info').row(pinfo.row)[2]
            if IsNumber(sig_cell):
                self.sig_guess = sig_cell.value
                have_guess = True
            
        self.a_opt = self.r_opr = XF_LOG
        pinfo = self.GetInfo_by_name("flaw transform:")
        if pinfo != 0 :
            name = pinfo.value.strip().lower()
            if name == "none" : self.a_opt = XF_NONE
            elif name == "inverse" : self.a_opt = XF_INVERSE
            elif name == "custom" : self.a_opt = XF_CUSTOM
            self.a_row = pinfo.row
        
        if self.a_opt==XF_CUSTOM :
            test1 = 10.0
            test2 = a_fwd(test1,doc)
            fail1 = fabs(a_inv(test2,doc)-test1)>1e-8
            test2 = a_inv(test1,doc)
            fail2 = fabs(a_fwd(test2,doc)-test1)>1e-8
            if (fail1 or fail2) :
                #AfxMessageBox("Round-trip test failed for Flaw Transform");
                return FALSE;
        
        pinfo = self.GetInfo_by_name("insp transform:")
        if pinfo != 0 :
            name = pinfo.value.strip().lower()
            if name == "none" : self.r_opt = XF_NONE
            elif name == "inverse" : self.r_opt = XF_INVERSE
            elif name == "custom" : self.r_opt = XF_CUSTOM
            self.r_row = pinfo.row
        '''
        if (a_opt==XF_CUSTOM) {
            test1 = 10.0;
            test2 = r_fwd(test1);
            fail1 = fabs(r_inv(test2)-test1)>1e-8;
            test2 = r_inv(test1);
            fail2 = fabs(r_fwd(test2)-test1)>1e-8;
            if (fail1 || fail2) {
	            AfxMessageBox("Round-trip test failed for Insp Transform");
	            return FALSE;
            }
        }
        '''
        pinfo = self.GetInfo_by_name("flaw column:")
        self.flaw_column = 0;
        if pinfo != 0 :
            if pinfo.IsNumber() : self.flaw_column = int(pinfo.value)
            elif pinfo.IsString() :
                col = pinfo.value.strip().upper()
                n = int(ord(col[0])-ord('A'))
                if n >= 0 : self.flaw_column = n

        pinfo = self.GetInfo_by_name("Flaw Name:")
        self.flaw_name = "Length"
        if pinfo != 0 :
            if pinfo.IsString() : self.flaw_name = pinfo.value
            
        pinfo = self.GetInfo_by_name("Flaw Units:")
        self.flaw_units = "mil"
        if pinfo != 0 :
            if (pinfo.IsString()) : self.flaw_units = pinfo.value
            
        pinfo = self.GetInfo_by_name("Insp Start:")
        self.data_column = 6;
        if pinfo != 0 :
            if pinfo.IsNumber() : self.data_column = int(pinfo.value)
            elif pinfo.IsString() :
                col = pinfo.value.strip().upper()
                n = int(ord(col[0])-ord('A'))
                if n >= 0 : self.data_column = n
        
        pinfo = self.GetInfo_by_name("Insp Units:")
        self.flaw_units = "counts"
        if pinfo != 0 :
            if (pinfo.IsString()) : self.data_units = pinfo.value
        
        pinfo = self.GetInfo_by_name("Signal Min:")
        self.tmin = 0.0
        if pinfo != 0 :
            if pinfo.IsNumber() : self.tmin = pinfo.value
        
        pinfo = self.GetInfo_by_name("Signal Max:")
        self.tmax = 0.0
        if pinfo != 0 :
            if pinfo.IsNumber() : self.tmax = pinfo.value
        
        pinfo = self.GetInfo_by_name("Thresholds:")
        if pinfo == 0 and self.tmin>0.0:
            self.nath = 1
            self.ath = [self.tmin]
        
        self.pod_threshold = self.ath[0]
        pinfo = self.GetInfo_by_name("POD Threshold:")
        if pinfo != 0 :
            if pinfo.IsNumber() : self.pod_threshold = pinfo.value
        
        self.level = 90
        pinfo = self.GetInfo_by_name("POD Level:")
        if pinfo != 0 :
            if pinfo.IsNumber() : self.level = pinfo.value
        self.pod_level = 0.01*self.level
        self.asize = "a"+str(self.level)
        
        self.confidence_level = 95
        pinfo = self.GetInfo_by_name("Confidence:")
        if pinfo != 0 :
            if pinfo.IsNumber() : self.confidence_level = pinfo.value
        if self.confidence_level == 90: pass
        elif self.confidence_level == 95: pass
        elif self.confidence_level == 99: pass
        else : self.confidence_level = 95
        
        self.alevel = "a"+str(int(self.level))+"/"+str(self.confidence_level)
        self.clevel = str(self.confidence_level)+"% confidence bound"
        
        return True
        
    def ShowInfo(self):
        i = j = ncomments = 0
        
        pass
          
    def OnNewDocument(self):
        return True
        
    def OnOpenDocument(self,PathName):
        try:
            f = open(PathName, 'r')
            f.close()
            return True
        except(IOError):
            return False
        finally:
            return False
    
    def OnSaveDocument(self,Pathname):
        try:
            f = open(PathName, 'r+')
            f.close()
        except(IOError):
            return False
        finally:
            return False
            
    def ahat_solve(self):


        #print("HELLO!")

        iret = 0
        fnorm = 0;
        lr = linear_regression()

        #// first pass - 
        #// generate initial guess using all data, including values with censored y's
        npts2 = self.npts+self.acount+self.bcount
        lr.regress(self.rlonx,self.rlony,npts2);  
        parm = [lr.intercept, lr.slope, lr.rms];
        var = [[0,0,0],[0,0,0],[0,0,0]]
        iret = ahat_sysolv(parm, var, fnorm, self);
        if (not iret) : return 0;
        #TRACE("Pass 1 Ahat analysis error return %d\n",iret);

        #// second pass
        #// generate initial guess using only uncensored values
        lr.regress(self.rlonx,self.rlony,self.npts);
        parm = [lr.intercept, lr.slope, lr.rms];
        iret = ahat_sysolv(parm, var, fnorm, self);
        if (not iret) :return 0;

        #TRACE("Pass 2 Ahat analysis error return %d\n",iret);
        return -1;
       
    def SortData(self):
        from qsort import qsort 
        qsort(self.flaws)
        
    def OnAnalysis(self):
        iret, n = 0, 0
        pdata = []
        
        ahat_model = self.model
        self.rlonx = []
        self.rlony = []

    

    
"""        
            
if __name__ == '__main__':
    pDoc = CPodDoc()
    pDoc.podfile = xlrd.open_workbook('../example2.xls',on_demand=True)
    
    pDoc.GetInfo()
    pDoc.ParseInfo()
    pDoc.GetData()
    #pDoc.ahat_solve()
    #pDoc.ahat_censor()

    #print(pDoc.__class__)
    pdict = pDoc.__dict__
    plist = list(pdict)
    plist.sort()
    for key in plist : 
        if type(pdict[key]) == type([]) and len(pdict[key])>3 :
            print(key+" "+str(type(pdict[key]))+" "+str(pdict[key][:3])+ " ...")
        else : print(key+" "+str(type(pdict[key])))+" "+str(pdict[key])
        
    xldict = pDoc.podfile.sheet_by_name("Data").__dict__
    xllist = list(xldict)
    xllist.sort()
    for each in range(5): print ("")
    #for key in xllist:
    #    print(key+" "+str(type(xldict[key])))+" "+str(xldict[key])
    """
