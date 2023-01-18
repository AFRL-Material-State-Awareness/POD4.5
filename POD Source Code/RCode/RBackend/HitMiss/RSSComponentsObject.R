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
#     along with this program.  If not, see <https://www.gnu.org/licenses>

# class stores the components unique to RSS

# parameters:
# maxResamples = stores the max number of resamples to be inputted into the ranked set sampling analysis
# set_r = the value for r when generating the ranked set samples
# set_m = the value for m when generating the ranked set samples
# regressionType = the type of regression to be applied to the ranked set samples (firth or maximum likelihood)
# excludeNA = boolean flag to determine if NA values are excluded when taking the median (should always be set to true)

RSSComponents <- setRefClass("RSSComponents", fields = list(maxResamples="numeric", set_m="numeric", set_r="numeric", 
                                                            regressionType="character",excludeNA="logical"), #medianAValues="list", 
                             methods=list(
                               initialize=function(maxResamplesInput=30, set_mInput=6, set_rInput=10,
                                                   regressionTypeInput="", excludeNAInput=TRUE){
                                 maxResamples<<-maxResamplesInput
                                 set_m<<-set_mInput
                                 set_r<<-set_rInput
                                 regressionType<<-regressionTypeInput
                                 excludeNA<<-excludeNAInput
                               }
                             ))