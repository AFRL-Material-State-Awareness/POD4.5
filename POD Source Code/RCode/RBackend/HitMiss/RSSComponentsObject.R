#class stores the components unique to RSS
RSSComponents <- setRefClass("RSSComponents", fields = list(maxResamples="numeric", set_m="numeric", set_r="numeric", 
                                                            regressionType="character",excludeNA="logical"), #medianAValues="list", 
                             #logitResultsListRSS="list", RankedSetResults="list"),
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