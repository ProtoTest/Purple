using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using MouseKeyboardLibrary;
using Purple.DataHandlers;

using MessageBox = System.Windows.MessageBox;

namespace Purple.ViewControllers
{
    public class MainScreen_VC 
    {
        //Variables for mouse positions
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
                    _foundElement = new UIA_ElementInfo(_ElementLocList.Last(), element.Current.Name, element.Current.AutomationId, element.Current.LocalizedControlType);
                    
                }
                else
                {
                    MessageBox.Show(String.Format("Could not find element at: {0},{1}", _ElementLocList.Last().X, _ElementLocList.Last().Y));
                }
            }
            else
            {
                MessageBox.Show("There are no element locations available.");
            }
        }

        public void FoundElement_AddRow(ref DataGrid dataGrid)
        {
            DataGridTextColumn columnHeader = new DataGridTextColumn();
            if (_foundElement != null)
            {
                dataGrid.Columns.Clear();

                string[] headerText = _foundElement.Headers();
                int headercount = headerText.Count();
                for (int x = 0; x < headercount; x++)
                {
                    columnHeader = new DataGridTextColumn();
                    columnHeader.Header = headerText[x];
                    columnHeader.Binding = new Binding(string.Format("[{0}]", x));
                    dataGrid.Columns.Add(columnHeader);
                }
                AddFoundElement(ref dataGrid);
            }
            else
            {
                columnHeader.Header = "UI Elements found will be listed here.";
                dataGrid.Columns.Add(columnHeader);
            }
        }

        public void AddFoundElement(ref DataGrid dataGrid)
        {
            List<object> rows = new List<object>();
            rows.Add(_foundElement.elementData());
            dataGrid.ItemsSource = rows;
        }

        public void AddElementToCache(UIA_ElementInfo element)
        {
            if (element != null)
            {
                _CachefileBuilder.addElement(element);
            }
        }











       
    }
}
