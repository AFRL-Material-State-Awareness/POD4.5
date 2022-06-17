import math
class Compare ():
    @staticmethod
    def eq(a, b, tol):
        return math.fabs(a - b) < tol

    @staticmethod
    def lteq(a, b, tol):
        return math.fabs(a - b) < tol or a < b

    @staticmethod
    def gteq(a, b, tol):
        return math.fabs(a - b) < tol or a > b

    @staticmethod
    def lt(a, b, tol):
        return (not (math.fabs(a - b) < tol)) and a < b

    @staticmethod
    def gt(a, b, tol):
        return (not (math.fabs(a - b) < tol)) and a > b
