using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Xml.Linq;
using MouseKeyboardLibrary;
using Purple.DataHandlers;
using PurpleLib;
using Binding = System.Windows.Data.Binding;
using CheckBox = System.Windows.Controls.CheckBox;
using DataGrid = System.Windows.Controls.DataGrid;
using MessageBox = System.Windows.MessageBox;
using TextBox = System.Windows.Controls.TextBox;
using TreeView = System.Windows.Controls.TreeView;

namespace Purple.ViewControllers
{
    public class MainScreen_VC 
    {
        //The purpose of this class is to handle all interactions from the DataHandler Classes that need to be either updated from the UI or stored back in the data classes.
        //The UI classes should NEVER directly interact with the data storeage classes except through classes like this.  More screens need more view controllers but not more data classes.
        //Variables for mouse positions -- Mouse position stuff added to Golem PurpleBaseElement
        
        //General Options for PurpleUI
        private StartOptions _Options = new StartOptions();
        private PurplePath _purpleLocator = new PurplePath();
        private AutomationElement parentElement;
        public List<UIA_ElementInfo> ElementInfos;

        private int _PreviousXLoc;
        private int _PreviousYLoc;
        private bool _TrackerRunning;
        private Point _Location;
        private List<Point> _ElementLocList;

        //Variables for DataGrids
        private string[] DataGridHeaders;
        private string[] DataGridRows;

        //Variables for UIAElements
        private UIA_ElementInfo _foundElement;
        private UIA_ElementCacher _CachefileBuilder;
        private bool elementFound = false;

        //Constructor
        public MainScreen_VC()
        {
            _ElementLocList = new List<Point>();
            _Location = new Point();
            _CachefileBuilder = new UIA_ElementCacher();
            ElementInfos = new List<UIA_ElementInfo>();
            ConfigurePurpleLocator();
        }

        public void ConfigurePurpleLocator()
        {
            _purpleLocator.DefaultWindowName = ConfigurationManager.AppSettings["Purple_WindowTitle"];
            _purpleLocator.BlankValue = ConfigurationManager.AppSettings["Purple_BlankValue"];
            _purpleLocator.Delimiter = ConfigurationManager.AppSettings["Purple_Delimiter"];
            _purpleLocator.ValueDelimiterEnd = ConfigurationManager.AppSettings["Purple_ValueDelimiterEnd"];
            _purpleLocator.ValueDelimiterStart = ConfigurationManager.AppSettings["Purple_ValueDelimiterStart"];
        }
        #region DataGrid functions for finding elements and displaying paths 
        public void AddPoint(Point value)
        {
            _Location.X = value.X;
            _Location.Y = value.Y;
            _ElementLocList.Add(_Location);
            FindUIElement();
        }

        private void FindUIElement()
        {
            if (_ElementLocList.Count > 0)
            {
                AutomationElement element = AutomationElement.FromPoint(_ElementLocList.Last());
                if (element != null)
                {
                    _foundElement = new UIA_ElementInfo(_ElementLocList.Last(), element, _purpleLocator);
                    //setting this for use with other functions
                    elementFound = true;
                }
                else
                {
                    MessageBox.Show(String.Format("Could not find element at: {0},{1}", _ElementLocList.Last().X, _ElementLocList.Last().Y));
                }
            }
            else
            {
                MessageBox.Show("\"This should never be displayed.\"\n\t --Every Developer Ever");
            }
            
        }
       
        public void FoundElement_AddRow(ref DataGrid dataGrid)
        {
            BuildElementDG_Headers(ref dataGrid);
            AddFoundElement(ref dataGrid);
        }

        private void BuildElementDG_Headers(ref DataGrid dataGrid)
        {
            DataGridTextColumn columnHeader = new DataGridTextColumn();
            dataGrid.Columns.Clear();
            if (_foundElement != null)
            {
                string[] headerText = _foundElement.Headers();
                int headercount = headerText.Count();
                for (int x = 0; x < headercount; x++)
                {
                    columnHeader = new DataGridTextColumn();
                    columnHeader.Header = headerText[x];
                    columnHeader.Binding = new Binding(string.Format("[{0}]", x));
                    dataGrid.Columns.Add(columnHeader);
                }
            }
        }

        private void AddFoundElement(ref DataGrid dataGrid)
        {
            List<object> rows = new List<object>();
            rows.Add(_foundElement.elementData());
            dataGrid.ItemsSource = rows;
        }

        public void SelectedElements_AddRow(ref DataGrid dataGrid)
        {
            if (_foundElement != null)
            {
                if (_CachefileBuilder.CachedElements)
                {
                    _CachefileBuilder.addElement(_foundElement);
                    AddCachedElements(ref dataGrid);
                }
                else
                {
                    BuildElementDG_Headers(ref dataGrid);
                    _CachefileBuilder.addElement(_foundElement);
                    AddCachedElements(ref dataGrid);
                }
            }
        }

        private void AddCachedElements(ref DataGrid dataGrid)
        {
            List<object> rows = new List<object>();
            for (int x = 0; x < _CachefileBuilder.ElementsInCache.Count; x++)
            {
                rows.Add(_CachefileBuilder.ElementsInCache[x].elementData());
            }
            dataGrid.ItemsSource = rows;
        }
        #endregion

        #region ConfigurationOptions

        public void SetOptions(ref CheckBox useDefault, ref TextBox defName)
        {
            _Options.UseDefaultWindow = Convert.ToBoolean(ConfigurationManager.AppSettings["UseDefaultStartScreen"]);
            _Options.DefaultWindow = ConfigurationManager.AppSettings["DefaultStartScreen"];

            useDefault.IsChecked = _Options.UseDefaultWindow;
            defName.Text = _Options.DefaultWindow;
        }

        public void PersistOptions(bool enabled, string name)
        {
            _Options.UseDefaultWindow = enabled;
            _Options.DefaultWindow = name;
        }
       

        public void SaveSettings_OnExit()
        {
            //Can't save appSettings only user settings
            ConfigurationManager.AppSettings["UseDefaultStartScreen"] = _Options.UseDefaultWindow.ToString();
            ConfigurationManager.AppSettings["DefaultStartScreen"] = _Options.DefaultWindow;
        }

        public void ReadOptionVals(ref CheckBox useDefault, ref TextBox defName)
        {
            useDefault.IsChecked = _Options.UseDefaultWindow;
            defName.Text = _Options.DefaultWindow;
        }

        #endregion

        #region TreeViewBuilder

        public List<UIA_ElementInfo> BuildApplicationTree(UIA_ElementInfo parent = null)
        {
            //I hate recursive functions
            TreeWalker walker = TreeWalker.RawViewWalker;
            if (parent == null)
            {
                parentElement = _purpleLocator.FindElement(_purpleLocator.Delimiter + _purpleLocator.DefaultWindowName);
                parent = new UIA_ElementInfo(parentElement, _purpleLocator);
            }
            else
            {
                parentElement = parent.AElement;
                parent.Children.Clear();
            }
            AutomationElement node = parentElement;
            
            
            //Get all the children of the first node
            node = walker.GetFirstChild(parentElement);
            while (node != null)
            {
                UIA_ElementInfo child = new UIA_ElementInfo(node, _purpleLocator);
                child.BuildNextLevel();
                parent.Children.Add(child);
                node = walker.GetNextSibling(node);
            }
            
            ElementInfos.Add(parent);
            
            return ElementInfos;
        }
        
        public void BuildChildTree(UIA_ElementInfo item, object sender, RoutedEventArgs e)
        {
            item.Children = BuildApplicationTree(item);
        }

        #endregion


        #region TestFunctions
        public void AttemptClick()
        {
            if (elementFound)
            {
                uint X = (uint) _foundElement.ElementLocation.X;
                uint Y = (uint) _foundElement.ElementLocation.Y;
                //SetCursorPos(X, Y);
                //mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                //mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            }
        }

        public void AttemptKeyboard()
        {
            if (elementFound)
            {   _foundElement.setfocus();
                SendKeys.SendWait("Here's a message");
            }
        }

        public void testpatterns()
        {
            if (elementFound)
            {
                _foundElement.patterns();
            }
        } 
        public void testPath()
        {
            AutomationElement thing = _purpleLocator.FindElement("/LifeQuest™ Pipeline/!BLANK!/RLF2013TestFile.qig [2D]/LifeQuestBaseView_ViewBar/ViewBar_OptionsButton");
            if (thing != null)
            {
                MessageBox.Show("Element Found!");
            }
        }
        #endregion

       
        









    }
}
