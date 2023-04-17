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
    public class PODObjectTests
    {
        private PODObject _podObject;
        [SetUp]
        public void Setup()
        {
            _podObject = new PODObject();
        }

        /// No tests here, but need to seriously refactor the Get Accessror for WIzardType (many nested switch statements and almost impossible to read
    }
}
