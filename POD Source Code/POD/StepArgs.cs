// <copyright file="StepArgs.cs" company="The University of Dayton">
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
using System.Windows.Forms;

namespace POD
{
    public class StepArgs : EventArgs
    {
    
        public StepArgs(int myIndex)
        {
            Index = myIndex;
            HelpFile = string.Empty;
        }

        public StepArgs(int myIndex, string myHelpFile)
        {
            Index = myIndex;
            HelpFile = myHelpFile;
        }

        public StepArgs(int myIndex, string myHelpFile, TreeNode myNode, PODObjectTypeEnum objectType)
        {
            Index = myIndex;
            HelpFile = myHelpFile;
            ListNode = myNode;
            Type = objectType;
        }

        /// <summary>
        /// An index of a Wizard Step.
        /// </summary>
        public int Index;

        public PODObjectTypeEnum Type = PODObjectTypeEnum.None;

        /// <summary>
        /// the name of the help file to load into RTF viewer
        /// </summary>
        public string HelpFile;

        /// <summary>
        /// Node who's children make up a list of step for the wizard
        /// </summary>
        public TreeNode ListNode;
    }
}
