using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;


namespace MyEchoBot.Models
{   
    public class UserProfile
    {    

        // state property...
        // Added new properties
        public string Name { get; set; }
         public string Order { set; get; }
        public string Location{ set; get; }
        // public DateTime CallbackTime { set; get; }
        public string Instruction { get; set; }
    }
}