using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Automation;
using Condition = System.Windows.Automation.Condition;


namespace PurpleLib
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

        public UIA_ElementInfo(Point loc, AutomationElement element, string windowName = "")
        {
            _uiaElement = element;
            _ElementLocation = loc;
            _ElementName = _uiaElement.Current.Name;
            _ElementAutomationID = _uiaElement.Current.AutomationId;
            _ElementType = _uiaElement.Current.LocalizedControlType;
            if (windowName != "")
            {
                PurplePath locator = new PurplePath();
                locator.DefaultWindowName = windowName;
                _PurplePath = locator.getPurplePath(_uiaElement);
            }
            else
            {
                _PurplePath = new PurplePath().getPurplePath(_uiaElement);
            }
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

        public void setfocus()
        {
            _uiaElement.SetFocus();
        }

        public void patterns()
        {
            AutomationProperty[] props = _uiaElement.GetSupportedProperties();
            AutomationProperty prop = props[1];

            AutomationPattern[] patterns = _uiaElement.GetSupportedPatterns();
            AutomationPattern pattern = patterns[0];
        }

    }
}
