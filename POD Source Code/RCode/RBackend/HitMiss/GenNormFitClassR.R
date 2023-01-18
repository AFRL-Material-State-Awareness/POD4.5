#     Probability of Detection Version 4.5 (PODv4.5)
#     Copyright (C) 2022  University of Dayton Research Institute (UDRI)
# 
#     This program is free software: you can redistribute it and/or modify
#     it under the terms of the GNU General Public License as published by
#     the Free Software Foundation, either version 3 of the License, or
#     (at your option) any later version.
# 
#     This program is distributed in the hope that it will be useful,
#     but WITHOUT ANY WARRANTY; without even the implied warranty of
#     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
#     GNU General Public License for more details.
# 
#     You should have received a copy of the GNU General Public License
#     along with this program.  If not, see <https://www.gnu.org/licenses/>.

#this class is used to create a normally distributed set of crack sizes given a sample size of 'x'
#its primary purpose is for modfied wald

# Parameters:
# cracks = The array of crack sizes from the original dataframe
# sampleSize = The sample size of the input dataframe
# Nsample = number of normally distributed cracks to generate
# simCrackSizesArray = The array of simulated crack sizes (this gets returned)

GenNormFit <- setRefClass("GenNormFit", fields = list(cracks="numeric", sampleSize="numeric", 
                                                      Nsample="numeric", simCrackSizesArray="numeric"), 
                                  methods = list(
                                    getSimCrackSizesArray=function(){
                                      return(simCrackSizesArray)
                                    },
                                    genNormalFit=function(){
                                      #Fit a normal distribution to the flaw sizes so that you get a
                                      #smooth curve (500 points?)
                                      normalFit_x <- fitdistr(cracks, "normal")
                                      ### This is where the mean and standard deviation are calculated
                                      a=rnorm(mean= normalFit_x$estimate[1], sd = normalFit_x$estimate[2], 
                                              n=max(sampleSize, Nsample))
                                      #this for loop is used to may all the points positive when generating 
                                      #the normal distribution points, only do this if it isn't a log transformation
                                      if(isLog==FALSE){
                                        for (idx in 1:length(a)){
                                          while(a[idx] <= 0){
                                            a[idx] = rnorm(mean=normalFit_x$estimate[1], sd=normalFit_x$estimate[2], n=1)
                                          }
                                        }
                                      }
                                      simCrackSizesArray<<-a
                                    }
                                  ))