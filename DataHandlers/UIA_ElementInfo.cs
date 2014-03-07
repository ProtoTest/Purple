using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Automation;
using Condition = System.Windows.Automation.Condition;


namespace Purple.DataHandlers
{
    public class UIA_ElementInfo
    {
        private AutomationElement _uiaElement;
        private Point _ElementLocation;
        private String _ElementName;
        private String _ElementParent;
        private String _ElementAutomationID;
        private String _ElementType;
        private String _PurplePath;
        

        public UIA_ElementInfo(Point loc, AutomationElement element)
        {
            _uiaElement = element;
            _ElementLocation = loc;
            _ElementName = _uiaElement.Current.Name;
            //Need to find a good way to get the parent
            _ElementAutomationID = _uiaElement.Current.AutomationId;
            _ElementType = _uiaElement.Current.LocalizedControlType;
            _PurplePath = new PurplePath().getPurplePath(_uiaElement);

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
