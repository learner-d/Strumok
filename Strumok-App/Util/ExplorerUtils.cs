using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrumokApp.Util
{
    public static class ExplorerUtils
    {
        private static readonly OpenFileDialog s_openFileDialog = new OpenFileDialog();
        private static readonly SaveFileDialog s_saveFileDialog = new SaveFileDialog();
        
        public static string ShowOpenFileDialog(string title, string filter = "", bool checkFileExists = true)
        {
            s_openFileDialog.Title = title;
            s_openFileDialog.Filter = filter;
            s_openFileDialog.CheckFileExists = checkFileExists;
            bool dlgResult = s_openFileDialog.ShowDialog() == true;
            return dlgResult ? s_openFileDialog.FileName : "";
        }

        public static string ShowSaveFileDialog(string title, string filter = "")
        {
            s_saveFileDialog.Title = title;
            s_saveFileDialog.Filter = filter;
            bool dlgResult = s_saveFileDialog.ShowDialog() == true;
            return dlgResult ? s_saveFileDialog.FileName : "";
        }
    }
}
