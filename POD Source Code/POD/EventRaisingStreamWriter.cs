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
