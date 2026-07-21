// <copyright file="AHatVsARegressionChartTests.cs" company="The University of Dayton">
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
using NUnit.Framework;
using POD.Controls;
using POD;
using System.Windows.Forms.DataVisualization.Charting;
using System.Linq;

namespace Controls.UnitTests
{
    [TestFixture]
    public class AHatVsARegressionChartTests
    {
        private FakeAHatVsARegressionChart _regressionChart;
        [SetUp]
        public void SetUp()
        {
            _regressionChart = new FakeAHatVsARegressionChart();
        }
        [Test]
        public void UpdateEquation_EquationIsNull_NoEquationAssigned()
        {
            //Arrange
            _regressionChart.Equation = null;
            //Act
            _regressionChart.UpdateEquation(TransformTypeEnum.None, TransformTypeEnum.None);
            //Assert
            Assert.IsNull(_regressionChart.Equation);
        }
        [Test]
        [TestCase(TransformTypeEnum.Linear, TransformTypeEnum.Linear, "y" + " = " + "m·x + b")]
        [TestCase(TransformTypeEnum.Log, TransformTypeEnum.Linear, "y" + " = " + "m·ln(x) + b")]
        [TestCase(TransformTypeEnum.Linear, TransformTypeEnum.Log, "ln(y)" + " = " + "m·x + b")]
        [TestCase(TransformTypeEnum.Log, TransformTypeEnum.Log, "ln(y)" + " = " + "m·ln(x) + b")]
        [TestCase(TransformTypeEnum.Exponetial, TransformTypeEnum.Linear, "y" + " = " + "m(e^x) + b")]
        [TestCase(TransformTypeEnum.Linear, TransformTypeEnum.Exponetial, "e^y" + " = " + "m·x + b")]
        [TestCase(TransformTypeEnum.Exponetial, TransformTypeEnum.Exponetial, "e^y" + " = " + "m(e^x) + b")]
        [TestCase(TransformTypeEnum.Inverse, TransformTypeEnum.Linear, "y" + " = " + "m(1/x) + b")]
        [TestCase(TransformTypeEnum.Linear, TransformTypeEnum.Inverse, "1/y" + " = " + "m·x + b")]
        [TestCase(TransformTypeEnum.Inverse, TransformTypeEnum.Inverse, "1/y" + " = " + "m(1/x) + b")]
        [TestCase(TransformTypeEnum.Linear, TransformTypeEnum.BoxCox, "(y^(lambda)-1)/lambda" + " = " + "m·x + b")]
        [TestCase(TransformTypeEnum.Log, TransformTypeEnum.BoxCox, "(y^(lambda)-1)/lambda" + " = " + "m·ln(x) + b")]
        [TestCase(TransformTypeEnum.Exponetial, TransformTypeEnum.BoxCox, "(y^(lambda)-1)/lambda" + " = " + "m(e^x) + b")]
        [TestCase(TransformTypeEnum.Inverse, TransformTypeEnum.BoxCox, "(y^(lambda)-1)/lambda" + " = " + "m(1/x) + b")]
        [TestCase(TransformTypeEnum.None, TransformTypeEnum.BoxCox, "(y^(lambda)-1)/lambda" + " = " + "Custom")]
        [TestCase(TransformTypeEnum.Linear, TransformTypeEnum.None, "Custom" + " = " + "m·x + b")]
        [TestCase(TransformTypeEnum.None, TransformTypeEnum.Linear, "y" + " = " + "Custom")]
        [TestCase(TransformTypeEnum.None, TransformTypeEnum.None, "Custom" + " = " + "Custom")]
        [TestCase(TransformTypeEnum.Log, TransformTypeEnum.None, "Custom" + " = " + "m·ln(x) + b")]
        [TestCase(TransformTypeEnum.None, TransformTypeEnum.Log, "ln(y)" + " = " + "Custom")]
        [TestCase(TransformTypeEnum.Linear, TransformTypeEnum.None, "Custom" + " = " + "m·x + b")]
        [TestCase(TransformTypeEnum.None, TransformTypeEnum.Exponetial, "e^y" + " = " + "Custom")]
        [TestCase(TransformTypeEnum.Exponetial, TransformTypeEnum.None, "Custom" + " = " + "m(e^x) + b")]
        [TestCase(TransformTypeEnum.None, TransformTypeEnum.Inverse, "1/y" + " = " + "Custom")]
        [TestCase(TransformTypeEnum.Inverse, TransformTypeEnum.None, "Custom" + " = " + "m(1/x) + b")]
        public void UpdateEquation_ValidTransform_AssignsEquationAsString(TransformTypeEnum XTrans, TransformTypeEnum YTrans, string eq)
        {
            //Arrange
            _regressionChart.Equation = new TextAnnotation();
            //Act
            _regressionChart.UpdateEquation(XTrans, YTrans);
            //Assert
            Assert.That(RemoveWhitespace(_regressionChart.Equation.Text), Is.EqualTo(RemoveWhitespace(eq)));
        }
        public string RemoveWhitespace(string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }

    }
    public class FakeAHatVsARegressionChart : AHatVsARegressionChart
    {
        public TextAnnotation Equation
        {
            set { _equation = value; }
            get { return _equation; }
        }
    }
}
