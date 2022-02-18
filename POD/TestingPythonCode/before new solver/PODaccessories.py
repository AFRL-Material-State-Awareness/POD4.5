from math import isnan

def pretty_print(mylist):
    output = [str(each) for each in mylist]
    outputstring = output[0]
    for each in output[1:]:
        outputstring += '\t'
        outputstring += each
    print(outputstring)

def IsNumber(x):
    return not isnan(x)
    
def mean(my_array):
    if len(my_array) > 0:
        return sum(my_array) / len(my_array)
    else:
        return 0.0


def cbound(npts,  confidence_level=0.95):
    npts = float(npts)
    if confidence_level == 0.9:
        return 3.8077 + 1.9039 /npts;
    if confidence_level == 0.99:
        return 8.2740 + 2.5495 / npts;
    #Assume default 95% confidence level if not 90 or 99
    return  5.13822 + 2.0965 / npts;
