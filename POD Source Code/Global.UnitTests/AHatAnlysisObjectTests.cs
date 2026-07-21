// <copyright file="AHatAnlysisObjectTests.cs" company="The University of Dayton">
// Copyright © 2025 The University of Dayton. All rights reserved.
//
// Licensed under the GNU General Public License, Version 3.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.gnu.org/licenses/gpl-3.0.txt
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

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
