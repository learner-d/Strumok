using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using StrumokLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StrumokApp.ViewModel
{
    internal class MainVm : BindableBase
    {
        // Діалог вибору файлу для шифрування
        protected readonly OpenFileDialog _openFileDialog = new OpenFileDialog
        {
            Title = "Оберіть файл для шифрування",
            CheckFileExists = true
        };

        public ICommand CryptCommand { get; }
        public ICommand SelectFileCommand {  get; }

        // Властивість "Ключ шифрування"
        protected StrumokKey _strumokKey = new StrumokKey();
        protected StrumokKey StrumokKey { 
            get => _strumokKey;
            set { 
                _strumokKey = value;
                // TODO: Remove
                RaisePropertyChanged(nameof(EncryptionKeyStr));
            }
        }

        public string Text { get; set; }

        // Властивість "Шифротекст"
        protected string _cryptedText = "";
        public string CryptedText
        {
            get => _cryptedText;
            set => SetProperty(ref _cryptedText, value);
        }

        // Властивість "Шлях до файлу"
        protected string _sourceFilePath = "";
        public string SourceFilePath
        {
            get => _sourceFilePath;
            set => SetProperty(ref _sourceFilePath, value);
        }

        public string EncryptionKeyStr
        {   
            get
            {
                if (_strumokKey == null)
                    return "";
                return _strumokKey.Key.ToString();
            } 
        }

        public MainVm()
        {
            //_strumokCrypter = new StrumokCrypter(_strumokKey);
            CryptCommand = new DelegateCommand(Crypt);
            SelectFileCommand = new DelegateCommand(SelectFile);
        }

        protected void Crypt()
        {
            using (StrumokCrypter _strumokCrypter = new StrumokCrypter(_strumokKey))
            {
                CryptedText = _strumokCrypter.Crypt(Text);
            }
            //CryptedText = _strumokCrypter.Crypt(Text);
        }

        /// <summary>
        /// Обирає файл для шифрування
        /// </summary>
        protected void SelectFile()
        {
            if (_openFileDialog.ShowDialog() ?? false)
            {
                SourceFilePath = _openFileDialog.FileName;
                // TODO: перевірити доступність файлу
            }
        }
    }
}
