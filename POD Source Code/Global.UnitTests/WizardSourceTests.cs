using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Data;
using NUnit.Framework;
using POD;
using CSharpBackendWithR;
namespace Global.UnitTests
{
    [TestFixture]
    public class WizardSourceTests
    {
        //private Mock<WizardSource> _wizaardSource;
        [SetUp]
        public void Setup()
        {
           // _wizaardSource = new  Mock<WizardSource>();

            
        }
        [Test]
        public void SwitchHelpView_NeedSwitchHelpViewIsNull_NoInvocationThrown()
        {
            
        }
        /// TODO: Figure out how to make the methods in this abastract class testable
    }
}
