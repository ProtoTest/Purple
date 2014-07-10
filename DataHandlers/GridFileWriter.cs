using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PurpleLib;


namespace Purple.DataHandlers
{
    class GridFileWriter
    {
        //This class will be used to build files with data contained in a DataGrid
        private List<UIA_ElementInfo> _cachedElements;
        public List<UIA_ElementInfo> ListofElements {set { _cachedElements = value; }}
        private Stream streamer;
        private StreamWriter sw;

        private void BuildFile(string filename)
        {
            
            sw = new StreamWriter(filename, false);
            
            sw.WriteLine("Write this shit to the file fucker");
            sw.Write("Maybe write this?");
            sw.Close();

        }

        public void SaveFile()
        {
            SaveFileDialog svd = new SaveFileDialog();
            svd.Filter = "Text Files (*.txt)|*.txt";
            svd.FilterIndex = 1;
            svd.RestoreDirectory = true;

            if(svd.ShowDialog() == DialogResult.OK)
            {
                
                    BuildFile(svd.FileName);
                    
                
            }


        }
    }
}
