
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MouseKeyboardLibrary;
using Purple.ViewControllers;
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
        

        private void Cursor_Button_Click(object sender, RoutedEventArgs e)
        {
            mouseHook.Start();
        }
        #endregion

        private void Purple_MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Load events should go here
        }

        private void GatherElementDetail()
        {
            //This function is called from the mouseHook_MouseDown() function
            if (!mouseHook.IsStarted)
            {
                mainScreenVc.AddPoint(new Point(double.Parse(Xcord.Text), double.Parse(YCord.Text)));
                mainScreenVc.FoundElement_AddRow(ref Found_Element_Grid);
            }
        }

        private void Add_Element_Selected_Click(object sender, RoutedEventArgs e)
        {
            if (!mouseHook.IsStarted)
            {
                mainScreenVc.SelectedElements_AddRow(ref Selected_Elements_Grid);
            }
        }
        
       
    }
}
