using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purple.DataHandlers
{
    class UIA_ElementCacher
    {
        private List<UIA_ElementInfo> _ElementsSelected;

        public UIA_ElementCacher()
        {
            _ElementsSelected = new List<UIA_ElementInfo>();
        }

        public void addElement(UIA_ElementInfo theElement)
        {
            _ElementsSelected.Add(theElement);
        }
    }
}
