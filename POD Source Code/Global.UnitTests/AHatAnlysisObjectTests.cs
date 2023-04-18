using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using POD;
using CSharpBackendWithR;
namespace Global.UnitTests
{
    [TestFixture]
    public class AHatAnlysisObjectTests
    {
        private AHatAnalysisObject _ahatAnalysisObject;
        private List<double> _flaws;
        private Dictionary<string, List<double>> _reponseDict;
        [SetUp]
        public void Setup()
        {
            _ahatAnalysisObject = new AHatAnalysisObject("SampleAnalysis");
            _reponseDict = new Dictionary<string, List<double>>();
            
        }
        ///<summary>
        /// GetMaxFlaw() Test
        /// </summary>
        [Test]
        public void GetMaxResponse_OneKeyValuePair_ReturnsMaxInsideDictionary()
        {
            List<double> responseList = new List<double>() { .1, .2, .3, .4, .5 };
            _reponseDict.Add("Responses", responseList);
            _ahatAnalysisObject.Responses_all = _reponseDict;

            var result=_ahatAnalysisObject.GetMaxResponse();

            Assert.That(result, Is.EqualTo(.5));
        }
        [Test]
        public void GetMaxResponse_MulitpleKeyValuePairs_ReturnsMaxAmongAllTheDictionariesKeyValuePairs()
        {
            List<double> responseList = new List<double>() { .1, .2, .3, .4, .5 };
            List<double> responseList2 = new List<double>() { .2, .4, .6, .8, 1.0 };
            List<double> responseList3 = new List<double>() { .1, .15, .2, .25, .3 };
            _reponseDict.Add("Responses", responseList);
            _reponseDict.Add("Responses2", responseList2);
            _reponseDict.Add("Responses3", responseList3);
            _ahatAnalysisObject.Responses_all = _reponseDict;

            var result = _ahatAnalysisObject.GetMaxResponse();

            Assert.That(result, Is.EqualTo(1.0));
        }

    }
}
