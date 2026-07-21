# GetBoxCoxParameters() is intended to evaluate the need for a Box Cox transformation and compute the necessary parameters.
# It is designed to be called from C# and return a list of parameters for C# to use.
# NOTE: This script requires the 'MASS' library.

GetBoxCoxParameters <- function(df_nc) {
  # Ensure the MASS library is loaded
  library(MASS)

  # Stop execution if the input is not a data frame or is empty
  if (!is.data.frame(df_nc) || nrow(df_nc) == 0) {
    stop("Input must be a non-empty data frame.")
  }

  # Ensure the required columns 'Size' and 'Ahat' exist
  if (!all(c("Size", "Ahat") %in% names(df_nc))) {
    stop("Input data frame must contain 'Size' and 'Ahat' columns.")
  }

  # Create a local copy for calculations to avoid altering the original data frame
  data_local_nc <- df_nc
  data_local_nc$x <- data_local_nc$Size
  data_local_nc$y <- data_local_nc$Ahat

  # Initialize parameters
  K1 <- 0
  K2 <- 0
  K_2.1 <- 0
  optimal_lambda <- NULL

  # The boxcox function requires strictly positive values.
  
    # --- SHIFTED PATH for data with zeros or negative values ---
    if (any(data_local_nc$y <= 0, na.rm = TRUE)) {
        min_val <- min(data_local_nc$y, na.rm = TRUE)
        y_shifted <- data_local_nc$y - min_val + 0.000001
    }
    else
    {
        y_shifted <- data_local_nc$y
    }
    
    # Calculate lambda using shifted data
    bc <- boxcox(y_shifted ~ x, data = data_local_nc, na.action = na.omit, lambda = seq(-2, 2, by = 0.01), plotit = FALSE)
    lambda <- bc$x[which.max(bc$y)]
    optimal_lambda <- round(lambda, digits = 3)
    
    # Calculate K's using SHIFTED data to avoid prod(0)
    N <- length(y_shifted)
    K_2.1 <- prod(y_shifted)
    K2 <- (K_2.1)^(1 / N)
    K1 <- optimal_lambda * (K2)^(optimal_lambda - 1)

  # Return a list containing all the calculated parameters
  return(list(lambda = optimal_lambda, K1 = K1, K2 = K2, K_2.1 = K_2.1))
}



# <--- Original Code Start --->
#
# # BoxCox() is intended to evaluate the need for a Box Cox transformation and compute one
# # Also have the option to compute linear models as well
# #NOTE: need to include library(MASS) for this to work
# #df = data frame you are using
# #df_nc is a subset of you data that has only data that does not require censoring
# #y_dec is your decision threshold for your NDI system
# #ZeroInData is TRUE if you data that contains zeros
# #KDenominator is TRUE if you want to use the K1 and K2 terms to perform your transformation
# #set equal to FALSE if you want the denominator to be just lambda
# #This transformation is NOT designed to handle data that contains negatives
# #If you have negatives in your data, ask a Statistician what other transformations may be helpful
# 
# BoxCox <- function(df,df_nc, y_dec, ZeroInData = TRUE, KDenominator = TRUE){
#   
#   options(warning = 1)
#   
#   #Set up some copies
#   df_obs = df
#   df_obs$x = df$Size
#   df_obs$y = df$Ahat
#   
#   # Check for negative values and provide helpful error
#   if (any(df_obs$y < 0, na.rm = TRUE)) {
#     stop("Error: The data frame contains negative values and Box-Cox does not allow for negative values.")
#   }
#   
#   #Set up no censor copies
#   df_no_censor = df_nc
#   df_no_censor$x = df_nc$Size
#   df_no_censor$y = df_nc$Ahat
#   
#   #Set up temporary data frame so that we do not alter the original data
#   data_obs_local = df_obs
#   data_obs_local_nc = df_no_censor
#   
#   #Ensuring only positive values for the data
#   if(any(data_obs_local$y<=0)){
#     data_obs_local$y = data_obs_local$y-min(data_obs_local$y)+0.000001
#   }
#   
#   #Ensuring only positive values for the data
#   if(any(data_obs_local_nc$y<=0)){
#     data_obs_local_nc$y = data_obs_local_nc$y-min(data_obs_local_nc$y)+0.000001
#   }
#   
#   #Now to perform the box cox analysis
#   bc<-boxcox(y ~ x, data=data_obs_local_nc, na.action=na.omit, lambda = seq(-2,2,by=0.01),
#              plotit = FALSE, eps = .Machine$double.eps/2, 
#              xlab = expression(lambda), ylab = "LL")
#   
#   #Providing best evaluation for lambda
#   lambda <- bc$x[which.max(bc$y)]
#   print(paste("Box Cox Lambda is ",lambda))
#   
#   #Now to specify which one we will use
#   optimal_lambda <- round(lambda, digits = 3)
#   print(paste("Using the Box Cox transformation at lambda =", optimal_lambda))
#   
#   #Need to find our K1 and K2 for the transformation
#   #K2 = (product of all y values)^1/N where N is number of data points
#   N = dim(df_no_censor)[1]
#   K_2.1 = prod(df_no_censor$y)
#   K2 = (K_2.1)^(1/N)
#   #K1 = lambda times K2 raised to the lambda -1
#   K1 = optimal_lambda * (K2)^(optimal_lambda -1)
#   
#   
#   if(ZeroInData == TRUE){
#     #This is for data that is >= 0
#     #Our approach of shifting the data by 1 comes from Yeo-Johnson Transformation for non-negative data
#     print("Using the shifted Box Cox")
#     if(KDenominator == TRUE){
#       print("using K denominator")
#       if(optimal_lambda !=0){
#         df_obs$y.trans = ((df_obs$y + 1)^optimal_lambda - 1) / K1 #All the data
#         df_no_censor$y.trans = ((df_no_censor$y + 1)^optimal_lambda - 1) / K1 #No Censor Required data
#         y_dec_trans = ((y_dec + 1)^optimal_lambda - 1) / K1 #y_dec
#       }else{
#         df_obs$y.trans = log(df_obs$y + 1)  #All the data
#         df_no_censor$y.trans = log(df_no_censor$y + 1)  #No Censor Required data
#         y_dec_trans = log(y_dec + 1)  #y_dec
#       }
#       
#     }else{
#       print("lambda denominator")
#       if(optimal_lambda !=0){
#         df_obs$y.trans = ((df_obs$y + 1)^optimal_lambda - 1) / optimal_lambda #All the data
#         df_no_censor$y.trans = ((df_no_censor$y + 1)^optimal_lambda - 1) / optimal_lambda #No Censor Required data
#         y_dec_trans = ((y_dec + 1)^optimal_lambda - 1) / optimal_lambda #y_dec
#       }else{
#         df_obs$y.trans = log(df_obs$y + 1)  #All the data
#         df_no_censor$y.trans = log(df_no_censor$y + 1)  #No Censor Required data
#         y_dec_trans = log(y_dec + 1)  #y_dec
#       }
#     }
#   }else{
#     #This is for the data that is >0
#     print("Using the non-shifted Box Cox")
#     if(optimal_lambda !=0){
#       df_obs$y.trans = ((df_obs$y)^optimal_lambda - 1) /optimal_lambda #All the data
#       df_no_censor$y.trans = ((df_no_censor$y)^optimal_lambda - 1) /optimal_lambda #No Censor Required data
#       y_dec_trans = ((y_dec)^optimal_lambda - 1) /optimal_lambda #y_dec
#     }else{
#       df_obs$y.trans = log(df_obs$y)  #All the data
#       df_no_censor$y.trans = log(df_no_censor$y) #No Censor Required data
#       y_dec_trans = log(y_dec) #y_dec
#     }
#   }
#   
#   return(list(df_trans = df_obs, y_dec_trans = y_dec_trans, lambda = optimal_lambda))   
# }
# 
# BoxCoxInverse <- function(df_trans, df_nc, lambda, ZeroInData = TRUE, KDenominator = TRUE){
#   
#   options(warning = 1)
#   
#   # Set up some copies
#   df_obs = df_trans
#   
#   # Recalculate K1 and K2 if needed
#   if (KDenominator == TRUE) {
#     #Set up no censor copies
#     df_no_censor = df_nc
#     df_no_censor$x = df_nc$Size
#     df_no_censor$y = df_nc$Ahat
#     
#     #Need to find our K1 and K2 for the transformation
#     #K2 = (product of all y values)^1/N where N is number of data points
#     N = dim(df_no_censor)[1]
#     K_2.1 = prod(df_no_censor$y)
#     K2 = (K_2.1)^(1/N)
#     #K1 = lambda times K2 raised to the lambda -1
#     K1 = lambda * (K2)^(lambda -1)
#   }
#   
#   if(ZeroInData == TRUE){
#     #This is for data that is >= 0
#     print("Using the inverse shifted Box Cox")
#     if(KDenominator == TRUE){
#       print("using K denominator")
#       if(lambda !=0){
#         df_obs$y.inv.trans = ((df_obs$y.trans * K1) + 1)^(1/lambda) - 1
#       }else{
#         df_obs$y.inv.trans = exp(df_obs$y.trans) - 1
#       }
#       
#     }else{
#       print("lambda denominator")
#       if(lambda !=0){
#         df_obs$y.inv.trans = ((df_obs$y.trans * lambda) + 1)^(1/lambda) - 1
#       }else{
#         df_obs$y.inv.trans = exp(df_obs$y.trans) - 1
#       }
#     }
#   }else{
#     #This is for the data that is >0
#     print("Using the inverse non-shifted Box Cox")
#     if(lambda !=0){
#       df_obs$y.inv.trans = ((df_obs$y.trans * lambda) + 1)^(1/lambda)
#     }else{
#       df_obs$y.inv.trans = exp(df_obs$y.trans)
#     }
#   }
#   
#   return(df_obs)   
# }
#
# <--- Original Code End --->