// <copyright file="WizardSourceTests.cs" company="The University of Dayton">
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
