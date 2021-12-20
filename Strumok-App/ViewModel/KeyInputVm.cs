using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using Newtonsoft.Json;
using StrumokApp.Util;
using System.IO;
using System.Linq;
using StrumokApp.Data;

namespace StrumokApp.ViewModel
{
    public class KeyInputVm : BindableBase
    {
        protected readonly Random random = new Random();
        private StrumokKeyConfiguration keyConfiguration;

        protected bool _isStrumok256 = true;
        public bool IsStrumok256
        {
            get => _isStrumok256;
            set
            {
                if (value == _isStrumok256)
                    return;
                SetProperty(ref _isStrumok256, value);
                IsStrumok512 = !value;
            }
        }

        protected bool _isStrumok512 = false;

        public bool IsStrumok512
        {
            get => _isStrumok512;
            set
            {
                if (value == _isStrumok512)
                    return;
                SetProperty(ref _isStrumok512, value);
                RecreateKeyConfiguration();
            }
        }

        public ulong KeyPart1
        {
            get => keyConfiguration.GetKeyPartOrDefault(0, 0);
            set
            {
                keyConfiguration.TrySetKeyPart(0, value);
                RaisePropertyChanged();
            }
        }
        public ulong KeyPart2 {
            get => keyConfiguration.GetKeyPartOrDefault(1, 0);
            set
            {
                keyConfiguration.TrySetKeyPart(1, value);
                RaisePropertyChanged();
            }
        }
        public ulong KeyPart3 {
            get => keyConfiguration.GetKeyPartOrDefault(2, 0);
            set
            {
                keyConfiguration.TrySetKeyPart(2, value);
                RaisePropertyChanged();
            }
        }
        public ulong KeyPart4 {
            get => keyConfiguration.GetKeyPartOrDefault(3, 0);
            set
            {
                keyConfiguration.TrySetKeyPart(3, value);
                RaisePropertyChanged();
            }
        }
        public ulong KeyPart5 {
            get => keyConfiguration.GetKeyPartOrDefault(4, 0);
            set
            {
                keyConfiguration.TrySetKeyPart(4, value);
                RaisePropertyChanged();
            }
        }
        public ulong KeyPart6 {
            get => keyConfiguration.GetKeyPartOrDefault(5, 0);
            set
            {
                keyConfiguration.TrySetKeyPart(5, value);
                RaisePropertyChanged();
            }
        }
        public ulong KeyPart7 {
            get => keyConfiguration.GetKeyPartOrDefault(6, 0);
            set
            {
                keyConfiguration.TrySetKeyPart(6, value);
                RaisePropertyChanged();
            }
        }
        public ulong KeyPart8 {
            get => keyConfiguration.GetKeyPartOrDefault(7, 0);
            set
            {
                keyConfiguration.TrySetKeyPart(7, value);
                RaisePropertyChanged();
            }
        }

        // Вектор ініціалізації
        public ulong IVPart1 {
            get => keyConfiguration.GetIVPartOrDefault(0, 0);
            set {
                keyConfiguration.TrySetIVPart(0, value);
                RaisePropertyChanged();
            }
        }
        public ulong IVPart2 {
            get => keyConfiguration.GetIVPartOrDefault(1, 0);
            set {
                keyConfiguration.TrySetIVPart(1, value);
                RaisePropertyChanged();
            }
        }
        public ulong IVPart3 {
            get => keyConfiguration.GetIVPartOrDefault(2, 0);
            set {
                keyConfiguration.TrySetIVPart(2, value);
                RaisePropertyChanged();
            }
        }
        public ulong IVPart4 {
            get => keyConfiguration.GetIVPartOrDefault(3, 0);
            set {
                keyConfiguration.TrySetIVPart(3, value);
                RaisePropertyChanged();
            }
        }

        // Команди
        public ICommand ApplyKeyCommand { get; }
        public ICommand RandomKeyCommand { get; }
        public ICommand LoadFromFileCommand { get; }
        public ICommand SaveToFileCommand { get; }

        // Події
        public event Action<StrumokKeyConfiguration> KeyApplied;

        public event MessageDelegate MessageBroadcast;

        public KeyInputVm()
        {
            ApplyKeyCommand = new DelegateCommand(ApplyKey);
            RandomKeyCommand = new DelegateCommand(GenerateRandomKey);
            LoadFromFileCommand = new DelegateCommand(LoadFromFile);
            SaveToFileCommand = new DelegateCommand(SaveToFile);
            RecreateKeyConfiguration();
        }

        protected void RecreateKeyConfiguration()
        {
            keyConfiguration = new StrumokKeyConfiguration(_isStrumok512, keyConfiguration);
            RaiseKeyConfigurationChanged();
        }

        protected void GenerateRandomKey()
        {
            if (!_isStrumok256 && !_isStrumok512)
                return;

            IVPart1 = GetRandomKeyPart();
            IVPart2 = GetRandomKeyPart();
            IVPart3 = GetRandomKeyPart();
            IVPart4 = GetRandomKeyPart();

            KeyPart1 = GetRandomKeyPart();
            KeyPart2 = GetRandomKeyPart();
            KeyPart3 = GetRandomKeyPart();
            KeyPart4 = GetRandomKeyPart();

            if (!_isStrumok512)
                return;
            KeyPart5 = GetRandomKeyPart();
            KeyPart6 = GetRandomKeyPart();
            KeyPart7 = GetRandomKeyPart();
            KeyPart8 = GetRandomKeyPart();
        }
        protected void ApplyKey()
        {
            if (!_isStrumok256 && !_isStrumok512)
                return;

            KeyApplied?.Invoke(keyConfiguration);
        }
        protected ulong GetRandomKeyPart()
        {
            byte[] buffer = new byte[8];
            random.NextBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }
        protected void LoadFromFile()
        {
            if (keyConfiguration == null)
                return;
            string path = ExplorerUtils.ShowOpenFileDialog("", "JSON|*.json");
            if (!File.Exists(path))
                return;
            string fileContents = File.ReadAllText(path);
            StrumokKeyConfiguration deserialized = JsonConvert.DeserializeObject<StrumokKeyConfiguration>(fileContents);
            if (deserialized == null)
                return;
            keyConfiguration = deserialized;
            _isStrumok256 = !deserialized.Strumok512;
            _isStrumok512 =  deserialized.Strumok512;
            RaiseKeyConfigurationChanged();
        }
        protected void SaveToFile()
        {
            if (keyConfiguration == null)
                return;
            string path = ExplorerUtils.ShowSaveFileDialog("", "JSON|*.json");
            File.WriteAllText(path, JsonConvert.SerializeObject(keyConfiguration));
        }

        protected void RaiseKeyConfigurationChanged()
        {
            RaisePropertyChanged(nameof(IsStrumok256));
            RaisePropertyChanged(nameof(IsStrumok512));
            RaisePropertyChanged(nameof(KeyPart1));
            RaisePropertyChanged(nameof(KeyPart2));
            RaisePropertyChanged(nameof(KeyPart3));
            RaisePropertyChanged(nameof(KeyPart4));
            RaisePropertyChanged(nameof(KeyPart5));
            RaisePropertyChanged(nameof(KeyPart6));
            RaisePropertyChanged(nameof(KeyPart7));
            RaisePropertyChanged(nameof(KeyPart8));
            RaisePropertyChanged(nameof(IVPart1));
            RaisePropertyChanged(nameof(IVPart2));
            RaisePropertyChanged(nameof(IVPart3));
            RaisePropertyChanged(nameof(IVPart4));
        }
    }
}
