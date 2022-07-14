from sklearn.linear_model import Lasso
from sklearn.linear_model import LassoCV
from numpy import arange
from sklearn.model_selection import RepeatedKFold
from sklearn.model_selection import train_test_split
import pandas as pd
#temporarily used for testing
from sklearn import datasets
#df=pd.open_csv()
#df=pd.read_csv(r"C:\Users\gohmancm\Desktop\Experiments\R_Source_Code_dummy_2\simulation_60_0.28_2.csv")
#data=df.values
def calcLassoRegression(df):
    #define predictor and response vars (crack size and hit miss)
    X = df[["x"]]
    y = df["y"]
    #cross validation method
    cv=RepeatedKFold(n_splits=10, n_repeats=3, random_state=1)
    #define model
    model=LassoCV(alphas=arange(0, 1, 0.005), cv=cv, n_jobs=-1)
    #model=Lasso(alpha=1, alphas=arange(0, 3, 0.01))
    #model=LassoCV(alphas=arange(0, 3, 0.01))

    #fit model
    model.fit(X,y)
    #model.fit_intercept(X,y)
    #print lambda(also known as alpha
    #print(model.alpha_)
    print(model.coef_)
    return model