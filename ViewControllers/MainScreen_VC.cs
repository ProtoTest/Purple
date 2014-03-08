using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Forms;
using MouseKeyboardLibrary;
using Purple.DataHandlers;
using Binding = System.Windows.Data.Binding;
using DataGrid = System.Windows.Controls.DataGrid;
using MessageBox = System.Windows.MessageBox;

namespace Purple.ViewControllers
{
    public class MainScreen_VC 
    {
        //The purpose of this class is to handle all interactions from the DataHandler Classes that need to be either updated from the UI or stored back in the data classes.
        //The UI classes should NEVER directly interact with the data storeage classes except through classes like this.  More screens need more view controllers but not more data classes.
        //Variables for mouse positions
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(uint x, uint y);

        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, int dx, int dy, uint cButtons, uint dwExtraInfo);

        //Thought these two might be needed for sending text to textboxes, but Forms.SendKeys is way easier - these might be useful for something later
        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);


        //need to gather constants for the rest of these
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

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
        }

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
                    _foundElement = new UIA_ElementInfo(_ElementLocList.Last(), element);
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

        public void AttemptClick()
        {
            if (elementFound)
            {
                uint X = (uint) _foundElement.ElementLocation.X;
                uint Y = (uint) _foundElement.ElementLocation.Y;
                SetCursorPos(X, Y);
                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            }
        }

        public void AttemptKeyboard()
        {
            if (elementFound)
            {   _foundElement.setfocus();
                SendKeys.SendWait("Here's a message");
            }
        }









       
    }
}
