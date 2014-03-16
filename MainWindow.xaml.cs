
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using MouseKeyboardLibrary;
using Purple.DataHandlers;
using System.Windows.Forms;
using Purple.ViewControllers;
using PurpleLib;
using Control = System.Windows.Forms.Control;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace Purple
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitMouseEventHandlers();
        }

        //Initialize View Controllers
        private MainScreen_VC mainScreenVc = new MainScreen_VC();
        public List<UIA_ElementInfo> Elements; 

        #region MouseHandlers Code to handle mouse driven events on the MainScreen
        //For some reason i couldn't pass around MouseEventArgs properly.  Bah! --Had to include it in the main form class.  
        private MouseHook mouseHook = new MouseHook();
        private int _mouseXLoc;
        private int _mouseYLoc;

        private void InitMouseEventHandlers()
        {
            mouseHook.MouseMove += mouseHook_MouseMove;
            mouseHook.MouseDown += mouseHook_MouseDown;
            mouseHook.MouseUp += mouseHook_MouseUp;
        }

        private void mouseHook_MouseUp(object sender, MouseEventArgs e)
        {
            //Stub for mouse up action if needed.
        }

        public void mouseHook_MouseDown(object sender, MouseEventArgs e)
        {
            mouseHook.Stop();
            GatherElementDetail();
        }

        private void mouseHook_MouseMove(object sender, MouseEventArgs e)
        {
            Xcord.Text = e.X.ToString();
            YCord.Text = e.Y.ToString();
        }
        #endregion

        #region FormLoad and Exit events
        private void Purple_MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //This function fires when the window is first loaded
            Elements = mainScreenVc.BuildApplicationTree();
            ApplicationTree.ItemsSource = Elements;
            //ApplicationTree.AddHandler(TreeViewItem.ExpandedEvent, new RoutedEventHandler(mainScreenVc.BuildChildTree));
            
        }

        private void Purple_MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            mainScreenVc.SaveSettings_OnExit();
        }
        #endregion
        private void Cursor_Button_Click(object sender, RoutedEventArgs e)
        {
            purplepathtextbox.Text = "PurplePath";
            mouseHook.Start();
        }

        private void GatherElementDetail()
        {
            //This function is called from the mouseHook_MouseDown() function
            if (!mouseHook.IsStarted)
            {
                mainScreenVc.AddPoint(new Point(double.Parse(Xcord.Text), double.Parse(YCord.Text)));
                mainScreenVc.GetElementInfo(ref purplepathtextbox);
            }
        }

        private void Add_Element_Selected_Click(object sender, RoutedEventArgs e)
        {
            if (!mouseHook.IsStarted)
            {
                //mainScreenVc.SelectedElements_AddRow(ref Selected_Elements_Grid);
            }
        }


        #region Options Code to handle options expander
        
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mainScreenVc.testPath();
        }
        #region MotherFuckingTreeView Event handlers for the goddamn treeview
        private void ApplicationTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            UIA_ElementInfo thing = (UIA_ElementInfo) ApplicationTree.SelectedItem;
            var what = ApplicationTree.Items.CurrentPosition;
            ApplicationTree_OnExpanded(sender, e);

        }
       
        private void ApplicationTree_OnExpanded(object sender, RoutedEventArgs e)
        {
            mainScreenVc.BuildChildTree((UIA_ElementInfo)ApplicationTree.SelectedValue, sender, e);
        }

        private void TreeItem_GetInfo(object sender, RoutedEventArgs e)
        {
            UIA_ElementInfo thing = (UIA_ElementInfo)ApplicationTree.SelectedItem;
            purplepathtextbox.Text = thing.Purplepath;
        }
        #endregion

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Clipboard.SetText(purplepathtextbox.Text);
        }
    }
}
