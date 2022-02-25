
import clr

clr.CompileModules("..\\WinForms POD UI\\bin\\Debug\\ahat.dll", "ahat.py")

'''
/* ahat.cpp
*
*/
#include "stdafx.h"
#include <math.h>
#include <search.h>
#include "PodDoc.h"
#include "XlControl.h"
#include "excel8.h"
#include "linreg.h"
#include "matrix.h"
'''
from math import *
'''
CString funcstr(CString v, int iopt);
bool IsCustom();
CString a_str();
CString r_str();
double a_fwd(double v);
double a_inv(double v);
double r_fwd(double v);
double r_inv(double v);
'''
from funcr import *
'''
// defined in alogam.cpp
double alogam(double x);
// defined in betain.cpp
double betain(double x, double p, double q, double beta);
// defined in phinv.cpp
double phinv(double p);
'''
from alogam import *
from betain import *
from phinv import *
'''
// defined in Info.cpp
void PutValue(int row, int col, double value);

// defined in mdnord.cpp
double mdnord(double x);
'''
from mdnord import *
'''
double leqslv(CMatrix &rj, double *dd);
'''
from leqslv import *
'''

// defined in smtxinv.cpp
double smtxinv(CMatrix &x, CMatrix &xi);
'''
from smtxinv import *
'''
// defined in sysolv.cpp
int ahat_sysolv(double *x, CMatrix &jb, double *fnorm);
'''
from sysolv import *

from CPodDoc import *
'''


// defined in nonlinear.cpp
int ahat_nonlinear_sysolv(double *x, CMatrix &jb, double *fnorm);

extern CPodDoc *pDoc;
'''
'''
def ahat_tests(self):

        i, j, n = 0

        #Test of normality
        lim = 0.0
        asq, astar = 0.0
        tmp1, tmp2, tmp3, tmp4 = 0.0

        #sort table by deviation
        qsort(anal,npts,sizeof(CAnal),compare1)

        lim = (double) npts;
        asq = 0.0;
        for (i=0; i<npts; i++) {
	        tmp1 = (double)(2*i+1);
	        tmp2 = log(anal[i].dstar);
	        tmp3 = log(1.0-anal[npts-1-i].dstar);
	        tmp4 = (tmp1*(tmp2+tmp3))/lim;
	        asq += tmp4;
        }
        asq = -(asq+lim);

        astar = asq * (1.0 + (0.75/lim) + (2.25 / (lim*lim)));

        xlValue(data_row,1,"Normality: Anderson-Darling");
        xlValue(data_row,4,"A* = ");
        xlHAlign(data_row,4,xlRight);
        PutValue(data_row,5,astar);
        xlValue(data_row,6,rngfindr(astar));
        data_row++;

        ////////////////////////////////////////////////////////////////////////
        // Homogenity of Variance
        const int ngrps = 2;
        int cnts[ngrps];
        double avg[ngrps],sum[ngrps],vor[ngrps];
        double av, sv, diff;
        int cnt, intrvl;

        // Sort data by crack size
        qsort(anal,npts,sizeof(CAnal),compare2);

        // Divide the data into groups
        intrvl = npts/ngrps;
        for (i=0; i<ngrps; i++) cnts[i] = intrvl;
        // If groups can not be equal sizes, add 1 to first few groups
        n = npts - ngrps*intrvl;
        for (i=0; i<n; i++) cnts[i]++;

        // Calculate variance of residuals in each group
        // "i" is the group index; "j" indexes the values within the group
        n=0;
        for (i=0; i<ngrps; i++) {
	        av = sv = 0.0;
	        cnt = cnts[i];
	        for (j=0; j<cnt; j++) av += anal[n+j].diff;
	        av /= cnt;
	        for (j=0; j<cnt; j++) {
		        diff = anal[n+j].diff-av;
		        sv += diff*diff;
	        }
	        avg[i] = av;
	        sum[i] = sv;
	        vor[i] = sv/(cnt-1.0);
	        n += cnt;
        }
        double term1, term2, term3, term4, dof, grpcnt;
        term1 = term2 = term3 = term4 = 0.0;
        dof = (double)(ngrps-1);
        grpcnt = (double)(ngrps);
        for (i=0; i<ngrps; i++) {
	        cnt = cnts[i];
	        term1 += cnt;
	        term2 += 1.0/(cnt-1);
	        term3 += (cnt-1)*log(vor[i]);
	        term4 += (cnt-1)*vor[i];
        }
        term1 -= grpcnt;
        term4 /= term1;
        double bd, halfbd, halfdof, p;
        bd = (term1*log(term4)-term3)/(1.0+((term2-(1.0/term1))/(3.0*dof)));
        halfbd = 0.5*bd;
        halfdof = 0.5*dof;

        // Calculate Equal-Variance Bartlet
        p = 1.0 - gammds(halfbd, halfdof);

        TRACE("Equal Variance:  Barlett P = %g B = %g\n",p,bd);

        Range range;
        Characters chars;
        Font font;

        xlValue(data_row,1,"Equal Variance: Bartlett");
        xlValue(data_row,4,"c2 = ");
        range = xl.GetRange(data_row,4);
        range.SetHorizontalAlignment(COleVariant((long)xlRight));
        chars = range.GetCharacters(COleVariant((long)1),COleVariant((long)1));
        font = chars.GetFont();
        font.SetName(COleVariant("Symbol"));
        chars = range.GetCharacters(COleVariant((long)2),COleVariant((long)1));
        font = chars.GetFont();
        font.SetSuperscript(vTrue);
        PutValue(data_row,5,bd);
        xlValue(data_row,6,probrng(p));
        data_row++;

        /////////////////////////////////////////////////////////////////////////////
        //  Test of linearity

        CString msg;
        double crksize;
        double totss, pess;
        double pedf; // pure error degrees of freedom
        double lofss;
        double lofdf;
        double beta, fcalc, paff, x;

        // check for duplicates

        int nbrsets = 0;  // # sets of unique crack sizes

        for (i=0; i<npts; ) {
	        crksize = anal[i].crksize;
	        nbrsets++;
	        i++;
	        while (i<npts && anal[i].crksize==crksize) i++;
        }
        if (nbrsets==npts) goto done;  // linearity test can not be performed

        // Determine total variance
        totss = 0.0;
        for (i=0; i<npts; i++) {
	        diff = anal[i].diff;
	        totss += diff*diff;
        }

        // process duplicates
        pess = 0.0;
        for (i=0; i<npts; ) {
	        crksize = anal[i].crksize;
	        diff = anal[i].diff;
	        term1 = diff*diff;
	        term2 = diff;
	        n=1;
	        i++;
	        while (i<npts && anal[i].crksize==crksize) {
		        diff = anal[i++].diff;
		        term1 += diff*diff;
		        term2 += diff;
		        n++;
	        }
	        term2 = (term2*term2)/n;
	        pess += (term1-term2);
        }

        // LOF means "lack of fit"
        // PE  means "pure error"
        // LOFDF = RegressionDF - PureErrorDF
        pedf = (double)(npts-nbrsets);
        lofss = totss - pess;
        lofdf = (npts-2) - pedf;

        // Linearity results
        beta = alogam(lofdf/2.0) + alogam(pedf/2.0) - alogam((lofdf+pedf)/2.0);
        fcalc = (lofss/lofdf) / (pess/pedf);
        x = pedf / (pedf + (lofdf*fcalc));
        paff = betain(x,pedf/2.0,lofdf/2.0,beta);
        TRACE("Linearity test: fcalc %g paff %g\n",fcalc,paff);

        msg.Format("Lack of fit: Pure Error (df=%d)",(int)pedf);
        xlValue(data_row,1,msg);
        //xlValue(data_row,1,"Lack of fit: Pure error");
        xlValue(data_row,4,"F = ");
        range = xl.GetRange(data_row,4);
        range.SetHorizontalAlignment(COleVariant((long)xlRight));
        PutValue(data_row,5,fcalc);
        xlValue(data_row,6,probrng(paff));
        data_row++;

done:
	delete [] anal;
	anal = 0;
}
'''

if __name__ == '__main__':

    doc = CPodDoc()

    doc.rlonx = tuple[(0.0, 1.0, 2.0)]
    doc.rlony = tuple[(0.0, 1.0, 2.0)]
    #doc.rlonx = tuple[(0.0, 1.0, 2.0)]
    doc.npts = 3

    doc.ahat_solve()

    print('alogam(23.2) '+str(alogam(23.2)))
    print('betain(.1,.2,.3,.4) '+str(betain(.1,.2,.3,.4)))
    print('phint test')
    for i in range(11) :
        if i == 0:
            x = 0.0001;
        elif i ==  10:
            x = 0.9999;
        else:
            x = 0.1*i;
        y = phinv(x);
        print("%9.4f\t %18.12f" % (x, y));
    print('mdnord(23.2) '+str(mdnord(23.2)))
    matrix = [[4,2],[6,1]]
    dd = [2,1]
    print ['matrix'+str(matrix)]
    print ['vector'+str(dd)]
    print ['tdleq(matrix,vector)'+str(tdleq(matrix,dd))]
    
    x = [[2,2,3],[4,5,6],[7,8,9]]
    print ("x: "+ str(x))

    xi = [[2,2,3],[4,5,6],[7,8,9]]
    print ("xi: "+ str(xi))

    deter, xi = smtxinv(x, xi)

    print ("x = [[1,2,3],[4,5,6],[7,8,9]]")
    print ("deter, xi = smtxinv(x)")
    print ("deter: " + str(deter))
    print ("xi: " + str(xi))
    
    print ['leqslv(x,[3,8,4.2])'+str(leqslv(x,[3,8,4.2]))]
    
    
    
    
    
    

