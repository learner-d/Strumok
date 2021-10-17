using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Strumok_App.Extentions
{
    internal class KeyInputTextBox : TextBox
    {
        const string DEFAULT_PLACEHOLDER = "0000-0000-0000-0000";
        public KeyInputTextBox()
        {
            Text = DEFAULT_PLACEHOLDER;
            EnableOvertypeMode();
        }

        protected void EnableOvertypeMode()
        {
            // fetch TextEditor from myTextBox
            PropertyInfo textEditorProperty = typeof(TextBox).GetProperty("TextEditor", BindingFlags.NonPublic | BindingFlags.Instance);
            object textEditor = textEditorProperty.GetValue(this, null);

            // set _OvertypeMode on the TextEditor
            PropertyInfo overtypeModeProperty = textEditor.GetType().GetProperty("_OvertypeMode", BindingFlags.NonPublic | BindingFlags.Instance);
            overtypeModeProperty.SetValue(textEditor, true, null);
        }
    }
}
