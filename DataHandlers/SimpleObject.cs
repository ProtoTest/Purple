using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purple.DataHandlers
{
    public class SimpleObject
    {
        public string item1 { get; set; }
        public string item2 { get; set; }

        public List<SimpleObject> Children { get; set; } 
    }
}
