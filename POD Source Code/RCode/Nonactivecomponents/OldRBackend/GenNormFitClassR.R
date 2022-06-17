#this class is used to create a normally distributed set of crack sizes given a sample size of 'x'
GenNormFit <- setRefClass("GenNormFit", fields = list(cracks="numeric", sampleSize="numeric", Nsample="numeric", simCrackSizesArray="numeric"), 
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
                                      #the normal distribution points
                                      for (idx in 1:length(a)){
                                        while(a[idx] <= 0){
                                          a[idx] = rnorm(mean=normalFit_x$estimate[1], sd=normalFit_x$estimate[2], n=1)
                                        }
                                      }
                                      simCrackSizesArray<<-a
                                    }
                                  ))