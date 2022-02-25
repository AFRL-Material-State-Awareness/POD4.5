
import pylab as p

def XY(xdata,ydata,title):
    picturename = title + '.png'
    p.title(title)
    p.plot(xdata, ydata, 'b-')
    p.savefig(picturename)
    p.close('all')
    return picturename

def POD(xdata, ydata, ydata2):
    picturename = "POD.png"
    p.title('a90/95')  # + '\n' + os.path.basename(f.filebase))
    p.plot(xdata, ydata, 'b-', label='POD(a)')
    p.plot(xdata, ydata2, 'r--', label='95% Confidence')
    p.ylabel('Probability of Detection, POD | a')
    p.xlabel('Size, a (mils)')
    #p.legend(loc=2)
    #v = p.axis()
    #p.axis((v[0],v[1],v[2],v[3]))
    #(xmin_pct * v[0], xmax_pct * v[1], ymin_pct * v[2], ymax_pct * v[3]))
    p.legend(loc=0)
    p.savefig(picturename)
    p.close('all')
    return picturename
    
def Threshold(xdata, ydata, ydata2):
    picturename = "Threshold.png"
    p.title('Threshold Plot')  # + '\n' + os.path.basename(f.filebase))
    p.plot(xdata, ydata, 'k', label='a90')
    p.plot(xdata, ydata2, 'k--', label='a90/95')
    p.ylabel('Flaw Depth | (mils)')
    p.xlabel('Decision Threshold (counts)')
    p.legend(loc=0)
    #p.legend(loc=2)
    #v = p.axis()
    #p.axis((v[0],v[1],v[2],v[3]))
    #(xmin_pct * v[0], xmax_pct * v[1], ymin_pct * v[2], ymax_pct * v[3]))
    p.savefig(picturename)
    p.close('all')
    return picturename

def CDF((xdata, ydata),
    (xdata2, ydata2, rms1),
    (xdata3, ydata3, rms2)):
    # Get the filename without the
    # (!3 digit!) extension and create a picture name based on that

    picturename = 'CDF.png'
    #!!! 2011-07-23 delete f line after a few days
    p.title('a90/95')  # + '\n' + os.path.basename(f.filebase))
    p.plot(xdata, ydata, 'b-', label='First Function')
    p.ylabel('Probability of Detection, POD | a')
    p.xlabel('Size, a (inches)')
    #p.legend(loc=2)
    p.axhline(0, color='k')
    #v = p.axis()
    #p.axis((v[0],v[1],v[2],v[3]))
    p.axis((0, 2, 0, 4))
    #(xmin_pct * v[0], xmax_pct * v[1], ymin_pct * v[2], ymax_pct * v[3]))
    p.twinx()

    p.plot(xdata2, ydata2, 'r--',
        label='Regression ' + str(rms1) + ' rms')
    p.plot(xdata3, ydata3, 'k--',
        label='Inverse Regression ' + str(rms2) + ' rms')
    # v = p.axis()
    # p.axis((xmin_pct * v[0], xmax_pct
    # * v[1], ymin_pct * v[2], ymax_pct * v[3]))
    #p.xlabel('Time [sec]')
    #p.ylabel('Disp [in]')
    p.legend(loc=1)
    p.axis((0, 2, 0, 4))
    p.savefig(picturename)
    p.close('all')
    return picturename


def fit((xdata, ydata), (xdata2, ydata2, rms1), suffix=''):
    # Get the filename without the
    # (!3 digit!) extension and create a picture name based on that

    picturename = 'fit' + suffix + '.png'
    p.title('Linear Fit')  # + '\n' + os.path.basename(f.filebase))
    p.loglog(xdata, ydata, 'x')
    #p.plot(xdata, ydata, 'x')
    p.ylabel('Signal (Counts)')
    p.xlabel('Size, a (mils)')
    #p.legend(loc=2)
    #p.axhline(0, color='k')
    #v = p.axis()
    #p.axis((0,v[1],0,v[3]))
    #p.axis((0,2,0,4))
    #(xmin_pct * v[0], xmax_pct * v[1], ymin_pct * v[2], ymax_pct * v[3]))
    #p.twinx()

    # p.loglog(xdata2,ydata2, 'r--',
    # label='Regression '+'{0:.6f}'.format(rms1)+' rms')
    p.loglog(xdata2, ydata2, 'r--', label='Regression ' + str(rms1) + ' rms')
    #p.plot(xdata2, ydata2, 'r--', label='Regression ' + str(rms1) + ' rms')
    # v = p.axis()
    # p.axis((xmin_pct * v[0], xmax_pct
    # * v[1], ymin_pct * v[2], ymax_pct * v[3]))
    # p.xlabel('Time [sec]')
    # p.ylabel('Disp [in]')
    p.legend(loc=1)
    #p.axis((0,2,0,4))
    p.savefig(picturename)
    p.close('all')
    return picturename


def resid((xdata, ydata), suffix=''):
    # Get the filename without the
    # (!3 digit!) extension and create a picture name based on that

    picturename = 'resid' + suffix + '.png'
    p.title('Residuals')  # + '\n' + os.path.basename(f.filebase))
    p.ylabel('Residuals (log Counts)')
    p.xlabel('Size, a (mils)')
    #p.legend(loc=2)
    p.axhline(0, color='k')

    p.plot(xdata, ydata, 'o')

    p.savefig(picturename)
    p.close('all')
    return picturename
