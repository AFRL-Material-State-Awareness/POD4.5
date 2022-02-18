#/* alnorm.cpp
#*
#*   Calculates the lower or upper tail of the standardized normal curve
#*   corresponding to any given argument.
#*
#*   X = the argument value
#*   UPPER = .TRUE. if the area to be computed is "X to infinity"
#*           .FALSE. if the area to be computed is "minus infinity to X"
#*
#*   Algorithm AS 66 Appl. Statist. (1973) Vol.22, P.424
#*
#*
#*
#*---LTONE and UTZERO must be set to suit the particular computer
#*   (see written text)
#*/
#include "stdafx.h"
#include <math.h>
import math

#double alnorm(double x, boolean upper);


#if defined(TEST_PROGRAM)
muk = 0.0008;
sigk = 1.08;

"""
def crk2cum(x):
    x = math.log(x/muk)/sigk;
    print("argument: "+x);
    return alnorm(x,1);

if __name__ == '__main__':
    int i;
    double x, y1, y2;
    for (i=0; i<30; i++) {
        x = (i-15);
        y1 = alnorm(x,1);
        y2 = alnorm(x,0);
        print(x,y1,y2);

	print("\n\n");
	for (i=0; i<2; i++) {
		x = (i? 1.2800001: 1.2799999);
		y1 = alnorm(x,1);
		y2 = alnorm(x,0);
		printf("%8.3f\t %14.6e\t %14.6e\n",x,y1,y2);
	}

	double a_critical = 0.5;
	printf("a_critical = %g, 1 - f_last = %16.6e\n",
		   a_critical, crk2cum(a_critical));
	
	return 0;
}
#endif

double alnorm(double x, boolean upper)
{
	const double a1 = 0.398942280444;
	const double a2 = 0.399903438504;
	const double a3 = 5.75885480458;
	const double a4 = 29.8213557808;
	const double a5 = 2.62433121679;
	const double a6 = 48.6959930692;
	const double a7 = 5.92885724438;
	const double b1 = 0.398942280385;
	const double b2 = 3.8052E-8;
	const double b3 = 1.00000615302;
	const double b4 = 3.98064794E-4;
	const double b5 = 1.98615381364;
	const double b6 = 0.151679116635;
	const double b7 = 5.29330324926;
	const double b8 = 4.8385912808;
	const double b9 = 15.1508972451;
	const double b10 = 0.742380924027;
	const double b11 = 30.789933034;
	const double b12 = 3.99019417011;

	const double con = 1.28;
	const double ltone = 7.0;
	const double utzero = 18.66;
	
	double func;
	double num, den, y, z;
	boolean up;
	z = x;
	up = upper;
	if (z<0.0) {
		up = !upper;
		z = -z;
	}
	if (z <= ltone || (up && z <= utzero) ) {
		y = 0.5*z*z;
		if (z>con) {
			num = b1*exp(-y);
			den = (z - b2 + b3/(z + b4 + b5 / ( z - b6 + b7/(z + b8 -b9 /
				(z + b10 + b11 / (z + b12))))));
			func = num/den;
		}
		else {
			func = 0.5 - z * (a1 - a2*y / ( y + a3 - a4 / (y + a5 + a6 / (y + a7))));
		}
	}
	else func = 0.0;
	if (!up) func = 1.0 - func;
	return func;
}
"""