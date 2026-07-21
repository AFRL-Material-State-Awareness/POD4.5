// <copyright file="EventRaisingStreamWriter.cs" company="The University of Dayton">
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
using System.IO;

namespace POD
{
    public class MyEvtArgs<T> : EventArgs
    {
        public T Value
        {
            get;
            private set;
        }
        public MyEvtArgs(T value)
        {
            this.Value = value;
        }
    }


    public class EventRaisingStreamWriter : StreamWriter
    {
        #region Event
        public event EventHandler<MyEvtArgs<string>> StringWritten;
        #endregion

        #region CTOR
        public EventRaisingStreamWriter(Stream s)
            : base(s)
        { }
        #endregion

        #region Private Methods
        private void LaunchEvent(string txtWritten)
        {
            if (StringWritten != null)
            {
                StringWritten(this, new MyEvtArgs<string>(txtWritten));
            }
        }
        #endregion


        #region Overrides

        public override void Write(string value)
        {
            base.Write(value);
            LaunchEvent(value);
        }
        public override void Write(bool value)
        {
            base.Write(value);
            LaunchEvent(value.ToString());
        }
        // here override all writing methods...

        #endregion
    }
}
