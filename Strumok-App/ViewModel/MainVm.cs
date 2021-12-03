using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using Strumok_App.Util;
using Strumok_App.View;
using Strumok_App.ViewModel;
using StrumokLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StrumokApp.ViewModel
{
    enum eCryptMode { None, Text, File };
    internal class MainVm : BindableBase
    {
        // Діалог вибору файлу для шифрування
        protected readonly OpenFileDialog _openFileDialog = new OpenFileDialog
        {
            Title = "Оберіть файл для шифрування",
            CheckFileExists = true
        };

        protected readonly KeyInputVm keyInputVm = new KeyInputVm();

        public ICommand CryptCommand { get; }
        //public ICommand CryptFileCommand { get; }
        public ICommand KeySetupCommand { get; }
        public ICommand SelectFileCommand {  get; }

        public eCryptMode CryptMode => TextCryptMode ? eCryptMode.Text : FileCryptMode ? eCryptMode.File : eCryptMode.None;

        public bool TextCryptMode { get; set; } = true;
        public bool FileCryptMode { get; set; } = false;

        // Властивість "Ключ шифрування"
        protected ulong[] _strumokKey = new ulong[4];
        public ulong[] StrumokKey {
            get => _strumokKey;
            set => SetProperty(ref _strumokKey, value);
        }

        // Властивість "Вектор ініціалізації"
        protected ulong[] _strumokIv = new ulong[4];
        public ulong[] StrumokIv
        {
            get => _strumokIv;
            set => SetProperty(ref _strumokIv, value);
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

        public MainVm()
        {
            keyInputVm.KeyApplied += OnKeyApplied;

            CryptCommand = new DelegateCommand(Crypt);
            KeySetupCommand = new DelegateCommand(KeySetup);
            SelectFileCommand = new DelegateCommand(SelectFile);
        }

        protected void OnKeyApplied(ulong[] key, ulong[] iv)
        {
            StrumokKey = key;
            StrumokIv = iv;
        }

        protected void Crypt()
        {
            switch (CryptMode)
            {
                case eCryptMode.Text:
                    CryptText();
                    break;
                case eCryptMode.File:
                    CryptFile();
                    break;
                default:
                    break;
            }
        }

        protected void CryptText()
        {
            if (Text == null)
                return;
            using (StrumokCrypter _strumokCrypter = new StrumokCrypter(_strumokKey, _strumokIv))
            {
                CryptedText = _strumokCrypter.Crypt(Text);
            }
            //CryptedText = _strumokCrypter.Crypt(Text);
        }

        protected void CryptFile()
        {
            // Перевірити шлях
            if (!File.Exists(SourceFilePath))
            {
                WarningMessageBox("Помилка: файл не існує", "");
                return;
            }
            try
            {
                byte[] buffer, output = null;
                string destinationPath = destinationPath = ExplorerUtils.ShowSaveFileDialog();
                if (string.IsNullOrEmpty(destinationPath))
                    return;
                using (Stream outputStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    using (Stream stream = new FileStream(SourceFilePath, FileMode.Open))
                    {
                        // Прочитати файл
                        buffer = new byte[stream.Length];
                        stream.Read(buffer, 0, buffer.Length);
                    }
                    // Шифрувати вміст
                    using (StrumokCrypter crypter = new StrumokCrypter(_strumokKey, _strumokIv))
                    {
                        output = crypter.Crypt(buffer);
                    }
                    // Записати до кінцевого файлу
                    outputStream.Write(output, 0, output.Length);
                }
            }
            catch (IOException)
            {
                WarningMessageBox("Не вдалося прочитати файл. Кинуто IOException");
            }
        }

        private static void WarningMessageBox(string text, string caption = "")
        {
            MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        //protected readonly StrumokKeyInputWindow strumokKeyInputWindow = new StrumokKeyInputWindow();
        protected void KeySetup()
        {
            StrumokKeyInputWindow strumokKeyInputWindow = new StrumokKeyInputWindow(keyInputVm);
            strumokKeyInputWindow.ShowDialog();
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
