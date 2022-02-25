# Import needed packages
library("RSSampling")
library(MASS)
library(pracma)
library(mcprofile)  
library(glmnet)
library(logistf)


# Set folder where functions are stored
codeLocation="c:/Users/chris/OneDrive/Documents/Research/R_Code/PODv4/CodeForStandardWald/"
# Load the functions into R's memory
source(paste(codeLocation,"getFirthResults.R",sep=""))
source(paste(codeLocation,"getLassoResults.R",sep=""))
source(paste(codeLocation,"getLogitResults.R",sep=""))
source(paste(codeLocation,"getRSSindices.R",sep=""))
source(paste(codeLocation,"getRSSmedians.R",sep=""))
source(paste(codeLocation,"lasso_se.R",sep=""))
source(paste(codeLocation,"RSS.fn.R",sep=""))
source(paste(codeLocation,"setupData.R",sep=""))

# Set folder where data exists
inputLocation="c:/Users/chris/OneDrive/Documents/Research/R_Code/SimulatedHitMiss/Completed/Sim 60/"
inputName = "simulation 60 0.28 2 "
data<-read.csv(paste(inputLocation, inputName,".csv",sep=""), 
               col.names=c("Order","y","x","SimNum","insp","n","case","uneven",
                           "uneven_lower","overlap","missDist","hitDist"),
               header=FALSE)
# Choose a simulation number from the 3000 simulations. 
data = subset(data, SimNum==1)

# Set folder where output files write
folderLocation = "C:/Users/chris/OneDrive/Documents/Research/R_Code/SimulatedHitMiss/Completed/StdWald_Points/"
fileName=paste(folderLocation, inputName,"_results",".csv",sep="")


# For each inspector, fit the models and write results to file.
  # Initiate the Ranked Set Sampling index since it should be the same for each
  # inspector
  index_insp1 = NULL
for(Inspector in unique(data$insp)){
  # Call main function which sets up the data frame for one inspector
  df_1_insp = setupData(subset(data,insp==Inspector), folderLocation)
  
  # Model with Logit
    results_Logit = getLogitResults(df_1_insp, folderLocation)
    # Write results to output file
      # To write without the header, change: col.names = FALSE
      write.table(results_Logit, file = fileName, row.names = FALSE, 
              append = TRUE, col.names = TRUE, sep = ", ")
  
  # Model with Firth
    results_Firth = getFirthResults(df_1_insp, folderLocation)
    # Write results to output file
    write.table(results_Firth, file = fileName, row.names = FALSE, 
                append = TRUE, col.names = FALSE, sep = ", ")
  
  # Model with Lasso
    results_Lasso = getLassoResults(df_1_insp, folderLocation) 
    # Write results to output file
    write.table(results_Lasso, file = fileName, row.names = FALSE, 
              append = TRUE, col.names = FALSE, sep = ", ")
  
  # Model with RSS  
    # NOTE: If more than 1 inpector exists, all the inspectors should use the same
    # "index_insp1" values. Index is set to null until it is set by the first 
    # inspector. Once it is set, it is reused for the remaining inspectors.
    if(is.null(index_insp1)){
      # More samples takes longer, but may be more accurate. 
      # Little change observed in testing for more than 30 resamples.
      index_insp1 = getRSSindices(df_1_insp,maxResamples = 30)
    }
    results_RSS = getRSSmedians(df_1_insp,index_insp1)
    # Write results to output file
    write.table(results_RSS[[2]], file = fileName, row.names = FALSE, 
              append = TRUE, col.names = FALSE, sep = ", ")
    # NOTE: results_RSS[[1]] variable contains RSS estimates where cases that 
    # had NA values were allowed to be included.  
}



