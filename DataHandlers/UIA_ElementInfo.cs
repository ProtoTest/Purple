using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Purple.DataHandlers
{
    public class UIA_ElementInfo
    {
        private Point _ElementLocation;
        private String _ElementName;
        private String _ElementParent;
        private String _ElementAutomationID;
        private String _ElementType;

        public UIA_ElementInfo(Point loc, String name, String AID, String Type)
        {
            _ElementLocation = loc;
            _ElementName = name;
            //Need to find a good way to get the parent
            _ElementAutomationID = AID;
            _ElementType = Type;
        }

        public string[] Headers()
        {
            string[] headerRow = new string[5];
            headerRow[0] = "X";
            headerRow[1] = "Y";
            headerRow[2] = "Name";
            headerRow[3] = "Automation ID";
            headerRow[4] = "Type";

            return headerRow;
        }

        public string[] elementData()
        {
            string[] data = new string[5];
            data[0] = _ElementLocation.X.ToString();
            data[1] = _ElementLocation.Y.ToString();
            data[2] = _ElementName;
            data[3] = _ElementAutomationID;
            data[4] = _ElementType;

            return data;
        }


    }
}
