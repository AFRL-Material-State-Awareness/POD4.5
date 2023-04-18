using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using CSharpBackendWithR;
using POD.Analyze;
using POD;
namespace Analyze.UnitTests
{
    [TestFixture]
    public class AnalysisListTests
    {
        private AnalysisList _analysisList;
        [SetUp]
        public void SetUp()
        {
            _analysisList = new AnalysisList();
            Analysis analysis1 = new Analysis();
            analysis1.Name = "My POD Analysis Name 1";
            Analysis analysis2 = new Analysis();
            analysis1.Name = "My POD Analysis Name 2";
            analysis2.SourceName = "mySourceName";
            Analysis analysis3 = new Analysis();
            analysis3.Name = "My POD Analysis Name 3";
            //Add to list
            _analysisList.Add(analysis1);
            _analysisList.Add(analysis2);
            _analysisList.Add(analysis3);
        } 
        [Test]
        public void UsingDataSource_AnalysesListIsEmpty_ReturnsFalse()
        {
            AnalysisList emptyAnalysisList = new AnalysisList();
            //Act
            var result = emptyAnalysisList.UsingDataSource("mySourceName");
            //Assert
            Assert.That(result, Is.False);
        }
        [Test]
        public void UsingDataSource_AnalysesListDoesNotContainSourceName_ReturnsFalse()
        {
            //Act
            var result = _analysisList.UsingDataSource("NotASourceName");
            //Assert
            Assert.That(result, Is.False);
        }
        [Test]
        public void UsingDataSource_AnalysesListContainsSourceName_ReturnsTrue()
        {
            //Act
            var result = _analysisList.UsingDataSource("mySourceName");
            //Assert
            Assert.That(result, Is.True);
        }  
    }
}
