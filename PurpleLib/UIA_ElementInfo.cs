using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        private PurplePath _locator;
        private List<UIA_ElementInfo> _children = new List<UIA_ElementInfo>();

        public List<UIA_ElementInfo> Children
        {
            get { return _children; }
            set { _children = value; }
        }

        public String Name
        {
            get
            {
                if (_ElementName == "")
                {
                    _ElementName = "<Blank>";
                }
                return _ElementName;
            }
        }

        public AutomationElement AElement
        {
            get { return _uiaElement; }
        }

        public String Purplepath
        {
            get
            {
                _PurplePath = _locator.getPurplePath(_uiaElement);
                return _PurplePath;
            }
        }

        public Point ElementLocation{get { return _ElementLocation; }}

        public UIA_ElementInfo(Point loc, AutomationElement element, PurplePath locator)
        {
            _uiaElement = element;
            _ElementLocation = loc;
            _ElementName = _uiaElement.Current.Name;
            _ElementAutomationID = _uiaElement.Current.AutomationId;
            _ElementType = _uiaElement.Current.LocalizedControlType;
            _PurplePath = locator.getPurplePath(element);
            _locator = locator;
            
        }

        public UIA_ElementInfo(AutomationElement element, PurplePath locator)
        {
            _uiaElement = element;
            _ElementName = element.Current.Name;
            //_PurplePath = locator.getPurplePath(element);
            _locator = locator;
            //BuildNextLevel();
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

        public void BuildNextLevel()
        {
            if (_locator.HasChildren(_uiaElement))
            {
                List<AutomationElement> childelements = _locator.GetChildren(_uiaElement);
                for (int x = 0; x < childelements.Count; x++)
                {
                    _children.Add(new UIA_ElementInfo(childelements[x], _locator));
                }
            }
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
