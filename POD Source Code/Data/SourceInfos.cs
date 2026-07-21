// <copyright file="SourceInfos.cs" company="The University of Dayton">
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

namespace POD.Data
{
    [Serializable]
    public class SourceInfos : List<SourceInfo>
    {
        public bool ContainsOriginal(string myName)
        {
            return GetFromOriginalName(myName) != null;
        }

        public bool ContainsNew(string myName)
        {
            return GetFromNewName(myName) != null;
        }

        public SourceInfo GetFromOriginalName(string myName)
        {
            SourceInfo info = null;

            foreach(SourceInfo item in this)
            {
                if (item.OriginalName == myName)
                    info = item;
            }

            return info;
        }

        public SourceInfo GetFromNewName(string myName)
        {
            SourceInfo info = null;

            foreach (SourceInfo item in this)
            {
                if (item.NewName == myName)
                    info = item;
            }

            return info;
        }

        public void AddCopiedInfo(SourceInfo currentInfo, string newName)
        {
            SourceInfo newInfo = new SourceInfo(newName, newName, currentInfo);

            Add(newInfo);
        }
    }
}
