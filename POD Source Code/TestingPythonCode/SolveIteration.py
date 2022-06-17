class SolveIteration ():

    def __init__(self, trial, iteration, mu, sigma, fnorm, didDamp, dampValue):
        self.trial = trial
        self.iteration = iteration
        self.mu = mu
        self.sigma = sigma
        self.fnorm = fnorm
        if didDamp == True:
            self.dampValue = dampValue
        else:
            self.dampValue = 1.0
