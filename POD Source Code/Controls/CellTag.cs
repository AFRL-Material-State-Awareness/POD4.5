// <copyright file="CellTag.cs" company="The University of Dayton">
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

namespace POD.Controls
{
    public enum DefinitionMode
    {
        None,
        ID,
        MetaData,
        Flaw,
        Response,
        ClearAll,
        ClearOne
    }

    public enum CellStatus
    {
        Valid,
        Invalid,
        InvalidWholeRow,
    }

    public enum RowStatus
    {
        Valid,
        Invalid
    }

    public class CellTag
    {
        public CellStatus Status;
        public string AutoValue;

        public CellTag(CellStatus myStatus)
        {
            Status = myStatus;
        }
    }

    public class RowTag
    {
        public RowStatus Status;

        public RowTag(RowStatus myStatus)
        {
            Status = myStatus;
        }
    }

    public class ColTag
    {
        public DefinitionMode Mode;

        public ColTag(DefinitionMode myMode)
        {
            Mode = myMode;
        }
    }

}
