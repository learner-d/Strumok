using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using Strumok_App.Binding;
namespace Strumok_App.ViewModel
{
    public class KeyInputVm : BindableBase
    {
        const string DEFAULT_KEYPART = "0000-0000-0000-0000";
        protected readonly Random random = new Random();
        protected readonly StrumokKeyConverter strumokKeyConverter = new StrumokKeyConverter();
        private string keyPart1 = DEFAULT_KEYPART;
        private string keyPart2 = DEFAULT_KEYPART;
        private string keyPart3 = DEFAULT_KEYPART;
        private string keyPart4 = DEFAULT_KEYPART;
        private string keyPart5 = DEFAULT_KEYPART;
        private string keyPart6 = DEFAULT_KEYPART;
        private string keyPart7 = DEFAULT_KEYPART;
        private string keyPart8 = DEFAULT_KEYPART;
        private string iVPart1 = DEFAULT_KEYPART;
        private string iVPart2 = DEFAULT_KEYPART;
        private string iVPart3 = DEFAULT_KEYPART;
        private string iVPart4 = DEFAULT_KEYPART;

        public bool IsStrumok256 { get; set; }
        public bool IsStrumok512 { get; set; }
        
        public string KeyPart1 { get => keyPart1; set => SetProperty(ref keyPart1, value); }
        public string KeyPart2 { get => keyPart2; set => SetProperty(ref keyPart2, value); }
        public string KeyPart3 { get => keyPart3; set => SetProperty(ref keyPart3, value); }
        public string KeyPart4 { get => keyPart4; set => SetProperty(ref keyPart4, value); }
        public string KeyPart5 { get => keyPart5; set => SetProperty(ref keyPart5, value); }
        public string KeyPart6 { get => keyPart6; set => SetProperty(ref keyPart6, value); }
        public string KeyPart7 { get => keyPart7; set => SetProperty(ref keyPart7, value); }
        public string KeyPart8 { get => keyPart8; set => SetProperty(ref keyPart8, value); }
        public string[] KeyParts => new string[] { keyPart1 ?? "", keyPart2 ?? "", keyPart3 ?? "", keyPart4 ?? "",
                                                   keyPart5 ?? "", keyPart6 ?? "", keyPart7 ?? "", keyPart8 ?? ""};
        // Вектор ініціалізації
        public string IVPart1 { get => iVPart1; set => SetProperty(ref iVPart1, value); }
        public string IVPart2 { get => iVPart2; set => SetProperty(ref iVPart2, value); }
        public string IVPart3 { get => iVPart3; set => SetProperty(ref iVPart3, value); }
        public string IVPart4 { get => iVPart4; set => SetProperty(ref iVPart4, value); }
        public string[] IVParts => new string[] {iVPart1 ?? "", iVPart2 ?? "", iVPart3 ?? "", iVPart4 ?? "" };
        
        // Команди
        public ICommand ApplyKeyCommand { get; }
        public ICommand RandomKeyCommand { get; }
        public ICommand LoadFromFileCommand { get; }
        public ICommand SaveToFileCommand { get; }

        // Події
        public event Action<ulong[], ulong[]> KeyApplied;

        public KeyInputVm()
        {
            ApplyKeyCommand = new DelegateCommand(ApplyKey);
            RandomKeyCommand = new DelegateCommand(GenerateRandomKey);
        }
        protected void GenerateRandomKey()
        {
            KeyPart1 = GetRandomKeyPart();
            KeyPart2 = GetRandomKeyPart();
            KeyPart3 = GetRandomKeyPart();
            KeyPart4 = GetRandomKeyPart();
            KeyPart5 = GetRandomKeyPart();
            KeyPart6 = GetRandomKeyPart();
            KeyPart7 = GetRandomKeyPart();
            KeyPart8 = GetRandomKeyPart();
            IVPart1 = GetRandomKeyPart();
            IVPart2 = GetRandomKeyPart();
            IVPart3 = GetRandomKeyPart();
            IVPart4 = GetRandomKeyPart();
        }
        protected void ApplyKey()
        {
            ulong[] key = strumokKeyConverter.ConvertBack(KeyParts);
            if (key == null)
                throw new Exception("couldn't convert the key");

            ulong[] iv = strumokKeyConverter.ConvertBack(IVParts);
            if (iv == null)
                throw new Exception("couldn't convert the initialization vector");

            KeyApplied?.Invoke(key, iv);
        }
        protected string GetRandomKeyPart()
        {
            byte[] buffer = new byte[8];
            random.NextBytes(buffer);
            return $"{buffer[7]:X2}{buffer[6]:X2}-{buffer[5]:X2}{buffer[4]:X2}-{buffer[3]:X2}{buffer[2]:X2}-{buffer[1]:X2}{buffer[0]:X2}";
        }
    }
}
