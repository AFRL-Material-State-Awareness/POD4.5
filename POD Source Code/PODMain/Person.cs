using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POD
{
    [Serializable]
    public class Person
    {
        string _name = "";

        public Person()
        {
            _name = "";
            _company = "";
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        string _company = "";

        public string Company
        {
            get { return _company; }
            set { _company = value; }
        }
    }
}
