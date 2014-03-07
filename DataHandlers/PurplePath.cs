using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Documents;

namespace Purple.DataHandlers
{
    public class PurplePath
    {
        //This class is used to build and interpret purplepaths to find AutomationElements
        private const String DELINIATOR = "//";
        private const String BLANK = "<<BLANK>>";
        private String _deliniator = DELINIATOR;
        private String _blankValue = BLANK;
        
        public PurplePath(String deliniator = DELINIATOR, String blank = BLANK)
        {
            _deliniator = deliniator;
            _blankValue = blank;
        }

        public String getPurplePath(AutomationElement element)
        {
            //I was curious how UI automation would handle having more than one panel with a blank name from Inspect.exe when building this path
            //It was surpriseing to know that the TreeWalker handles that for us when we walk up the tree, and conversly down the tree.
            //TODO: handle title bars like LQP where the title name changes based on the file opened
            TreeWalker walker = TreeWalker.ContentViewWalker;
            bool parentExists = true; //need to assume that there's a parent
            String path = element.Current.Name + _deliniator;
            AutomationElement parent;
            AutomationElement node = element;
            String purplePath = "";

            while (parentExists)
            {
                parent = walker.GetParent(node);
                if (parent != null)
                {
                    string parentName = parent.Current.Name;
                    if (parentName == "")
                    {
                        //We need to put something for blanks
                        parentName = _blankValue;
                    }
                    path += parentName + _deliniator;
                    node = parent;
                }
                else
                {
                    parentExists = false;
                }
            }
            //now try to build the path in the proper order
            string[] pathStrings = path.Split(_deliniator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            Array.Reverse(pathStrings); //Reverse the order of the strings in the array so elements appear in top to bottom
            //this trims off the first level, since that's the root element or Desktop
            for (int x = 1; x < pathStrings.Count(); x++)
            {
                purplePath += _deliniator + pathStrings[x];
            }
            return purplePath;
        }

        public AutomationElement FindElement(String purplePath)
        {
            List<String> pathStrings = new List<string>(purplePath.Split(_deliniator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
            

            AutomationElement element = AutomationElement.RootElement;
            TreeWalker walker = TreeWalker.ContentViewWalker;
            AutomationElement node = element;
            Condition findCondition;

            for (int i = 0; i < pathStrings.Count(); i++)
            {
                if (pathStrings[i].Equals(_blankValue))
                {
                    pathStrings[i] = "";
                }
                findCondition = new PropertyCondition(AutomationElement.NameProperty, pathStrings[i], PropertyConditionFlags.IgnoreCase);
                node = element.FindFirst(TreeScope.Children, findCondition);
                if (node != null)
                {
                    element = node;
                }
            }
            return node;
        }
    }
}
