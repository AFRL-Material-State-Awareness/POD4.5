options( warn = -1 )
#update.packages(ask = FALSE)
Sys.setenv(PATH = paste("C:/Rtools/bin", Sys.getenv("PATH"), sep=";"))
#used for folder location debugging scripts
install.packages('rstudioapi')

#establish a path for Rtools to run properly run this first line and then restart r
writeLines('PATH="${RTOOLS40_HOME}\\usr\\bin;${PATH}"', con = "~/.Renviron")
#writeLines('PATH="${RTOOLS_HOME}\\bin;${PATH}"', con = "~/.Renviron")
#writeLines('PATH="${RTOOLS_HOME}\\usr\\bin;${PATH}"', con = "~/.Renviron")
#alternative if rtools is still not working properly (takes a few minutes to install devtools)
#install.packages('devtools')
#Sys.setenv(PATH = paste("C:/Rtools/bin", Sys.getenv("PATH"), sep=";"))


#after r is restarted run the rest of the code
packageurl <- "https://cran.r-project.org/src/contrib/Archive/remotes/remotes_2.4.0.tar.gz"
install.packages(packageurl, repos=NULL, type="source")
#install.packages('remotes')
library(remotes)  
install_version('glue', version = '1.6.2', repos = 'http://cran.us.r-project.org',  upgrade= "never")  
install_version('rlang', version = '1.0.2', repos = 'http://cran.us.r-project.org',  upgrade= "never")
#will ask for an update--- just press enter to skip
install_version('lifecycle', version = '1.0.1', repos = 'http://cran.us.r-project.org',  upgrade= "never")  

#ggplot2
install_version('colorspace', version = '2.0.3', repos = 'http://cran.us.r-project.org',  upgrade= "never")  
install_version('cli', version = '3.2.0', repos = 'http://cran.us.r-project.org',  upgrade= "never")  
install_version('utf8', version = '1.2.2', repos = 'http://cran.us.r-project.org',  upgrade= "never")  
install_version('farver', version = '2.1.0', repos = 'http://cran.us.r-project.org',  upgrade= "never") 
install_version('labeling', version = '0.4.2', repos = 'http://cran.us.r-project.org',  upgrade= "never")  
install_version('munsell', version = '0.5.0', repos = 'http://cran.us.r-project.org',  upgrade= "never")  
install_version('R6', version = '2.5.1', repos = 'http://cran.us.r-project.org',  upgrade= "never")  
install_version('RColorBrewer', version = '1.1.3', repos = 'http://cran.us.r-project.org',  upgrade= "never")  
install_version('viridisLite', version = '0.4.0', repos = 'http://cran.us.r-project.org',  upgrade= "never")  
install_version('fansi', version = '1.0.3', repos = 'http://cran.us.r-project.org')  
install_version('magrittr', version = '2.0.3', repos = 'http://cran.us.r-project.org',  upgrade= "never")  
install_version('pkgconfig', version = '2.0.3', repos = 'http://cran.us.r-project.org',  upgrade= "never")  
install_version('vctrs', version = '0.4.1', repos = 'http://cran.us.r-project.org',  upgrade= "never")  
install_version('digest', version = '0.6.29', repos = 'http://cran.us.r-project.org',  upgrade= "never")  
install_version('gtable', version = '0.3.0', repos = 'http://cran.us.r-project.org',  upgrade= "never")  
install_version('isoband', version = '0.2.5', repos = 'http://cran.us.r-project.org',  upgrade= "never")
install_version('scales', version = '1.2.0', repos = 'http://cran.us.r-project.org',  upgrade= "never")
install_version('pillar', version = '1.8.0', repos = 'http://cran.us.r-project.org',  upgrade= "never")  
install_version('tibble', version = '3.1.6', repos = 'http://cran.us.r-project.org',  upgrade= "never")  
install_version('withr', version =  '2.5.0', repos = 'http://cran.us.r-project.org',  upgrade= "never")  

install_version('ellipsis', version = '0.3.2', repos = 'http://cran.us.r-project.org',  upgrade= "never")  

install_version('ggplot2', version = '3.3.4', repos = 'http://cran.us.r-project.org',  upgrade= "never")  
install.packages('foreach')
### Logistf dependencies
install_version('broom', version =  '1.0.0', repos = 'http://cran.us.r-project.org',  upgrade= "never") 
install_version('dplyr', version =  '1.0.9', repos = 'http://cran.us.r-project.org',  upgrade= "never") 
install_version('generics', version =  '0.1.3', repos = 'http://cran.us.r-project.org',  upgrade= "never") 
install_version('logistf', version =  '1.22', repos = 'http://cran.us.r-project.org',  upgrade= "never") 
#######
install_version('methods', version =  '4.0.0', repos = 'http://cran.us.r-project.org',  upgrade= "never") 
install_version('MASS', version =  '7.3.51.5', repos = 'http://cran.us.r-project.org')
install_version('mcprofile', version =  '1.0.0', repos = 'http://cran.us.r-project.org',  upgrade= "never") 
install_version('gridExtra', version =  '2.3', repos = 'http://cran.us.r-project.org',  upgrade= "never") 
install_version('nlme', version =  '3.1.158', repos = 'http://cran.us.r-project.org',  upgrade= "never") 
install_version('pracma', version =  '2.3.8', repos = 'http://cran.us.r-project.org',  upgrade= "never") 
install_version('carData', version =  '3.0.5', repos = 'http://cran.us.r-project.org',  upgrade= "never") 
#used for the car package
install_version('sp', version =  '1.5-0', repos = 'http://cran.us.r-project.org',  upgrade= "never") 
install_version('MatrixModels', version =  '0.4-0', repos = 'http://cran.us.r-project.org',  upgrade= "never") #package updated needed to install older version-October 10, 2022
install_version('RcppEigen', version =  '0.3.3.5.0', repos = 'http://cran.us.r-project.org',  upgrade= "never") #package updated needed to install older version-October 10, 2022
install_version('nloptr', version =  '1.2.1', repos = 'http://cran.us.r-project.org',  upgrade= "never")  
install_version('quantreg', version =  '5.94', repos = 'http://cran.us.r-project.org',  upgrade= "never") 
install_version('lme4', version =  '1.1-30', repos = 'http://cran.us.r-project.org',  upgrade= "never")  
install_version('car', version =  '3.1-0', repos = 'http://cran.us.r-project.org',  upgrade= "never")  
#survival package and dependencies
install_version('Matrix', version =  '1.4-1', repos = 'http://cran.us.r-project.org',  upgrade= "never") 
install_version('survival', version =  '3.3-1', repos = 'http://cran.us.r-project.org',  upgrade= "never") 
###ggresidpanel library and dependencies
install_version('cowplot', version =  '1.1.1', repos = 'http://cran.us.r-project.org',  upgrade= "never")  
install_version('plotly', version =  '4.10.0', repos = 'http://cran.us.r-project.org',  upgrade= "never") 
install_version('qqplotr', version =  '0.0.5', repos = 'http://cran.us.r-project.org',  upgrade= "never")  
install_version('stringr', version =  '1.4.0', repos = 'http://cran.us.r-project.org',  upgrade= "never")  
install_version('ggResidpanel', version =  '0.3.0', repos = 'http://cran.us.r-project.org',  upgrade= "never") 
#####
install_version('corrplot', version =  '0.92', repos = 'http://cran.us.r-project.org',  upgrade= "never") 
#### Used for firth approximation
install_version('roxygen2', version =  '7.1.2', repos = 'http://cran.us.r-project.org',  upgrade= "never") 



