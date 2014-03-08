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
        
        public Point ElementLocation{get { return _ElementLocation; }}

        public UIA_ElementInfo(Point loc, AutomationElement element)
        {
            _uiaElement = element;
            _ElementLocation = loc;
            _ElementName = _uiaElement.Current.Name;
            _ElementAutomationID = _uiaElement.Current.AutomationId;
            _ElementType = _uiaElement.Current.LocalizedControlType;
            _PurplePath = new PurplePath().getPurplePath(_uiaElement);

        }

        public string[] Headers()
        {
            string[] headerRow = new string[6];
            headerRow[0] = "X";
            headerRow[1] = "Y";
            headerRow[2] = "Name";
            headerRow[3] = "Automation ID";
            headerRow[4] = "Type";
            headerRow[5] = "PurplePath";

            return headerRow;
        }

        public string[] elementData()
        {
            string[] data = new string[6];
            data[0] = _ElementLocation.X.ToString();
            data[1] = _ElementLocation.Y.ToString();
            data[2] = _ElementName;
            data[3] = _ElementAutomationID;
            data[4] = _ElementType;
            data[5] = _PurplePath;
            return data;
        }

    }
}
