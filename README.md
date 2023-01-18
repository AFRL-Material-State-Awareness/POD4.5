# Probability of Detection version 4.5 (PODv4.5)                                                        
![alt text](https://github.com/gohmann174442049/POD4.5/blob/PODv4.5.0BackupBranch/POD4.5DependencyLicenses/POD%20icons.ico?raw=true)

  PODv4.5 (Probability of Detection version 4.5) is the new and improved version of PODv4 which incorporates advanced statistics and an r backend. There have also been bug fixes related to the original PODv4.

### License

  Unlike PODv4, PODv4.5 is open-source-software licensed under the GNU General Public License version 3 or later (**GPLv3+**). This means that anyone has the right to change, copy, modify, use, or study all of the code within PODv4.5 under the terms of GPLv3. For full license text, see [https://www.gnu.org/licenses/gpl-3.0.en.html].

### Installation Instructions

  If you wish to download PODv4.5 for your computer, please click on the tag with the latest release (current latest release is PODv4.5.0 beta as of January 2023). From there, simply download the installer (.msi). Once download, open the installer, and the wizard will guide you through the process. A copy of R-4.1.2 with the necessary libraries is included in the installation, so the application is ready to run upon completing the installation wizard. A shortcut for PODv4.5 will be added to the desktop. If you cannot find the .exe, simply type PODv4.5 in the search bar and the application should appear. 
  
### Running the Development version in Visual Studio

  PODv4.5 was written in visual studio 2019. In order to clone and run PODv4.5 in visual studio. Clone this github repository and open the 'Winforms POD UI.sln' located in /'POD Source Code'/. Once the solution is open in visual studio, set the startup project to **FORMS** and run the solution. The configuration should be set to Any CPU. The solution can run in either 32 or 64 bit (prefer 32-bit is set by default). If you wish to run the application in 64 bit, open the **FORMS** project properties > build > uncheck prefer 32-bit. Do NOT run the program in x86 (some features will cause the program to crash due to bottlenecking the 32-bit RAM allocation).
  
### New Features in PODv4.5.0

  PODv4.5.0 is the first release of PODv4.5. It offers a wide range of features not previously included in PODv4. In addition, the backend analysis for PODv4.5 is written in the statistical programming langauge R in order to more easily accomadate the new statistical methods for POD through the use of CRAN libraries. The features specific to PODv4.5.0 are listed below:

###### Hit/Miss Analysis

There have been many features and drastic improvements in hit/miss analysis for PODv4.5.0:

* **Plotting Confidence Interval Curve**: In PODv4, the python algorithms often struggled to plot the confidence _a<sup>9095</sup>_ curve even when the logistic regression algorithm converges easily. PODv4.5.0 can correctly generate and plot the wald _a<sup>9095</sup>_ curve for the user in the analyis wizard -> full regression analysis.

* **New Regression Technique**: Hit/Miss POD has traditionally been calculated using maximum likelihood logistic regression. However, in the cases, this algorithm may fail to converge when the data is separated (i.e. the 0s and 1s don't overlap), or when there are outliers present. To accomodate for this, PODv4.5.0 offers _Firth's Bias-Reduced Logistic Regression_ . This new regression method is capable of producing an _a<sup>90</sup>_ curve for tricky datasets.

* **New Confidence Interval Techniques**: PODv4.5.0 has three new statistical methods to general both the _a<sup>9095</sup>_ value and the _a<sup>9095</sup>_ curve. These methods include _Modified Wald_, _Likelihood Ratio confidence interval_, and _Modified Likelihood Ratio Confidence Interval_. More information can be found about these new confidence interval techniques in /'POD Documents'/

* **Alternative Sampling Method**: As an alternative to simple random sampling, PODv4.5.0 offers Ranked Set sampling to assist in creating both the POD curve and the  _a<sup>9095</sup>_ curve. This feature is especially useful when the sample size is relatively small. 


