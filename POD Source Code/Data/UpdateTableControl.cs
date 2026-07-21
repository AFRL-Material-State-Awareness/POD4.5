// <copyright file="UpdateTableControl.cs" company="The University of Dayton">
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

using POD.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    [Serializable]
    public class UpdateTableControl : IUpdateTableControl
    {
        private IAnalysisData _data;
        public UpdateTableControl(IAnalysisData dataIn)
        {
            _data = dataIn;
        }
        public void UpdateTable(int rowIndex, int colIndex, Flag bounds)
        {
            switch (bounds)
            {
                case Flag.InBounds:
                    _data.TurnOnPoint(colIndex, rowIndex);
                    break;
                case Flag.OutBounds:
                    _data.TurnOffPoint(colIndex, rowIndex);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("bounds must be either InBounds or OutBounds");
            }
        }
    }
    public interface IUpdateTableControl
    {
        void UpdateTable(int rowIndex, int colIndex, Flag bounds);
    }
}
