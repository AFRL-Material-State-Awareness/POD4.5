// <copyright file="TableRangePair.cs" company="The University of Dayton">
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
    /// <summary>
    /// Holds a set of related Table Ranges. One specifying how the table is going to be copied from a table.
    /// And the other specifying how it will copied into the new table.
    /// </summary>
    [Serializable]
    public class TableRangePair
    {
        #region Fields
        /// <summary>
        /// Specifies how data will be copied FROM the table to be imported.
        /// </summary>
        public TableRange From;
        /// <summary>
        /// Specifies where the data will be copied TO the new table.
        /// </summary>
        public TableRange To;
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new Table Range Pair.
        /// </summary>
        /// <param name="myFrom"></param>
        /// <param name="myTo"></param>
        public TableRangePair(TableRange myFrom, TableRange myTo)
        {
            From = myFrom;
            To = myTo;
        }
        #endregion

        #region Properties
        #endregion

        #region Methods
        #endregion

        #region Event Handling
        #endregion

    }
}
