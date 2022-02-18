import pylab as p

def CDF((xdata,ydata),(xdata2,ydata2,rms1),(xdata3,ydata3,rms2)):
    #Get the filename without the (!3 digit!) extension and create a picture name based on that

    picturename = 'CDF.png'
    #!!! 2011-07-23 delete f line after a few days f = open(f.textfile, 'rU') #!!! f is never referenced. 
    p.title('a90/95')# + '\n' + os.path.basename(f.filebase))
    p.plot(xdata, ydata, 'b-', label='First Function')
    p.ylabel('Probability of Detection, POD | a')
    p.xlabel('Size, a (inches)')
    #p.legend(loc=2)
    p.axhline(0, color='k')
    v = p.axis()
    #p.axis((v[0],v[1],v[2],v[3]))
    p.axis((0,2,0,4))
    #(xmin_pct * v[0], xmax_pct * v[1], ymin_pct * v[2], ymax_pct * v[3]))
    p.twinx()
    
    p.plot(xdata2,ydata2, 'r--', label='Regression '+str(rms1)+' rms')
    p.plot(xdata3,ydata3, 'k--', label='Inverse Regression '+str(rms2)+' rms')
    #v = p.axis()
    #p.axis((xmin_pct * v[0], xmax_pct * v[1], ymin_pct * v[2], ymax_pct * v[3]))
    #p.xlabel('Time [sec]')
    #p.ylabel('Disp [in]')
    p.legend(loc=1)
    p.axis((0,2,0,4))
    p.savefig(picturename)
    p.close('all')
    return picturename
    
def fit((xdata,ydata),(xdata2,ydata2,rms1), suffix=''):
    #Get the filename without the (!3 digit!) extension and create a picture name based on that

    picturename = 'fit'+suffix+'.png'
    #!!! 2011-07-23 delete f line after a few days f = open(f.textfile, 'rU') #!!! f is never referenced. 
    p.title('Linear Fit')# + '\n' + os.path.basename(f.filebase))
    p.loglog(xdata, ydata, 'x')
    p.ylabel('Signal (Counts)')
    p.xlabel('Size, a (mils)')
    #p.legend(loc=2)
    p.axhline(0, color='k')
    v = p.axis()
    #p.axis((0,v[1],0,v[3]))
    #p.axis((0,2,0,4))
    #(xmin_pct * v[0], xmax_pct * v[1], ymin_pct * v[2], ymax_pct * v[3]))
    #p.twinx()
    
    p.loglog(xdata2,ydata2, 'r--', label='Regression '+'{0:.6f}'.format(rms1)+' rms')
    #p.plot(xdata3,ydata3, 'k--', label='Inverse Regression '+str(rms2)+' rms')
    #v = p.axis()
    #p.axis((xmin_pct * v[0], xmax_pct * v[1], ymin_pct * v[2], ymax_pct * v[3]))
    #p.xlabel('Time [sec]')
    #p.ylabel('Disp [in]')
    p.legend(loc=1)
    #p.axis((0,2,0,4))
    p.savefig(picturename)
    p.close('all')
    return picturename  
    
    
    
def resid((xdata,ydata), suffix=''):
    #Get the filename without the (!3 digit!) extension and create a picture name based on that

    picturename = 'resid'+suffix+'.png'
    #!!! 2011-07-23 delete f line after a few days f = open(f.textfile, 'rU') #!!! f is never referenced. 
    p.title('Residuals')# + '\n' + os.path.basename(f.filebase))
    p.ylabel('Residuals (log Counts)')
    p.xlabel('Size, a (mils)')
    #p.legend(loc=2)
    p.axhline(0, color='k')
    
    p.plot(xdata,ydata, 'o')
    
    p.savefig(picturename)
    p.close('all')
    return picturename
