  PODv4.5 (Probability of Detection version 4.5) is the new and improved version of PODv4 which incorporates advanced statistics and an r backend. There have also been bug fixes related to the original PODv4.

  Unlike PODv4, PODv4.5 is open-source-software licensed under the GNU General Public License version 3 or later (GPLv3+). This means that anyone has the right to change, copy, modify, use, or study all of the code within PODv4.5 under the terms of GPLv3. 

Installation Instructions
  If you wish to download PODv4.5 for your computer, please click on the tag with the latest release (current latest release is PODv4.5.0 beta as of January 2023). From there, simply download the installer (.msi). Once download, open the installer, and the wizard will guide you through the process. A copy of R-4.1.2 with the necessary libraries is included in the installation, so the application is ready to run upon completing the installation wizard. A shortcut for PODv4.5 will be added to the desktop. If you cannot find the .exe, simply type PODv4.5 in the search bar and the application should appear. 
  
Running the Development version in Visual Studio
  PODv4.5 was written in visual studio 2019. In order to clone and run PODv4.5 in visual studio. Clone this github repository and open the 'Winforms POD UI.sln' located in /'POD Source Code'/. Once the solution is open in visual studio, set the startup project to FORMS and run the solution. The configuration should be set to Any CPU. The solution can run in either 32 or 64 bit (prefer 32-bit is set by default). If you wish to run the application in 64 bit, open the FORMS project properties > build > uncheck prefer 32-bit. Do NOT run the program in x86 (some features will cause the program to crash due to bottlenecking the RAM).
