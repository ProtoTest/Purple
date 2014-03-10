
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
        #endregion

        private void Purple_MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //This function fires when the window is first loaded
            OptionsExpander.BorderBrush.Opacity = 0;
        }
        
        private void Cursor_Button_Click(object sender, RoutedEventArgs e)
        {
            Found_Element_Grid.ItemsSource = null;
            mouseHook.Start();
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


        #region Options Code to handle options expander
        private void PurpleStartingPath_Checked(object sender, RoutedEventArgs e)
        {
            Options_StartingWindow.Visibility = Visibility.Visible;
        }

        private void PurpleStartingPath_Unchecked(object sender, RoutedEventArgs e)
        {
            Options_StartingWindow.Visibility = Visibility.Collapsed;
        }

        private void OptionsExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            //for some stupid reason i have to hid the fucking border of the thing when it's collapsed
            OptionsExpander.BorderBrush.Opacity = 0;
        }
        private void OptionsExpander_Expanded(object sender, RoutedEventArgs e)
        {
            OptionsExpander.BorderBrush.Opacity = 100;
        }
        
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mainScreenVc.testPath();
        }
    }
}
