using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strumok_App.Util
{
    public static class ExplorerUtils
    {
        private static readonly SaveFileDialog saveFileDialog = new SaveFileDialog();
        public static string ShowSaveFileDialog()
        {
            bool dlgResult = saveFileDialog.ShowDialog() == true;
            return dlgResult? saveFileDialog.FileName : "";
        }
    }
}
