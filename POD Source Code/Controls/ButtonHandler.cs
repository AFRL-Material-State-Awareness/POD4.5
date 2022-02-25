using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POD.Controls
{
    public class ButtonHandler
    {
        public PODButton Button;
        public List<EventHandler> Handler;

        public ButtonHandler(PODButton myButton, EventHandler myHandler)
        {
            Button = myButton;
            Handler = new List<EventHandler>();
            Handler.Add(myHandler);            
        }       

        public void AddEvent(EventHandler myEvent)
        {
            //always insert at beginning of list so children class get their events called first
            Handler.Insert(0, myEvent);
        }
    }
}
