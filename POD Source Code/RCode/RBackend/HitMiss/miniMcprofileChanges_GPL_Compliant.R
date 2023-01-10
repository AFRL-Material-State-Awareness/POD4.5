# The follow document states the changes made to the code provided in the mcprofile library (https://cran.r-project.org/web/packages/mcprofile/index.html).
# The library is licensed under GPLv2 or GPLv3. In order to be compliant with GPL, it is required that any derivative work created must state the changes made to the program.
# The purpose of this document to is provide brief description of the changes made to the mcprofile library written by Daniel Gerhard. Changes to the library were made in order
# remove the Mvtnorm(licensed as GPLv2-only) and ggplot2 packages from the installation of PODv4.5 and apply parallelization to generation of the linear combination matrix.
# The changes are listed below:
# ~ The function mcprofile was changed to the name minimcprofile. In addition, this function now has a return value (returns an mcprofile s3 class). Finally, parallelization using cpu clusters 
# was added to the generation of the linear combination matrix (K matrix). There is also an identical function to this named just mcprofile for using different cluster sizes
# ~ The function mcprofile.confint had many if statements that were used for adjustment (single-step","none","bonferroni") and confint sides ("two.sided","less","greater"). Some of these options 
# relied on mvtnorm and PODv4.5 never utilizes them. Thus, the if statement for everything besides "none" and "greater" were removed so that the mvtnorm packages was not included in the final distribution
# ~The wald function in mcprofile was renamed to wald.mlr
# ~The functions hoa, control, orglm.fit, and glm_profiling were simply just copied and pasted
# ~any functions related to ggplot2 were removed.
# ~All functions are included in an R file called minimcprofile.R and a copy of the GPLv3 license is provided at the top
