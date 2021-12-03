using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;

namespace Strumok_App.ViewModel
{
    public class KeyInputVm : BindableBase
    {
        protected readonly Random random = new Random();
        private ulong keyPart1 = 0;
        private ulong keyPart2 = 0;
        private ulong keyPart3 = 0;
        private ulong keyPart4 = 0;
        private ulong keyPart5 = 0;
        private ulong keyPart6 = 0;
        private ulong keyPart7 = 0;
        private ulong keyPart8 = 0;
        private ulong iVPart1 = 0;
        private ulong iVPart2 = 0;
        private ulong iVPart3 = 0;
        private ulong iVPart4 = 0;

        protected bool _isStrumok256 = true;
        public bool IsStrumok256
        {
            get => _isStrumok256;
            set => SetProperty(ref _isStrumok256, value);
        }

        protected bool _isStrumok512 = false;

        public bool IsStrumok512
        {
            get => _isStrumok512;
            set => SetProperty(ref _isStrumok512, value);
        }

        public ulong[] Key { get; } = new ulong[16];

        private void SetKeyPart(ulong keyPart, int index)
        {
            Key[index] = keyPart;
            RaisePropertyChanged(nameof(Key));
        }

        public ulong KeyPart1 { get => keyPart1; set => SetProperty(ref keyPart1, value); }
        public ulong KeyPart2 { get => keyPart2; set => SetProperty(ref keyPart2, value); }
        public ulong KeyPart3 { get => keyPart3; set => SetProperty(ref keyPart3, value); }
        public ulong KeyPart4 { get => keyPart4; set => SetProperty(ref keyPart4, value); }
        public ulong KeyPart5 { get => keyPart5; set => SetProperty(ref keyPart5, value); }
        public ulong KeyPart6 { get => keyPart6; set => SetProperty(ref keyPart6, value); }
        public ulong KeyPart7 { get => keyPart7; set => SetProperty(ref keyPart7, value); }
        public ulong KeyPart8 { get => keyPart8; set => SetProperty(ref keyPart8, value); }

        // Вектор ініціалізації
        public ulong IVPart1 { get => iVPart1; set => SetProperty(ref iVPart1, value); }
        public ulong IVPart2 { get => iVPart2; set => SetProperty(ref iVPart2, value); }
        public ulong IVPart3 { get => iVPart3; set => SetProperty(ref iVPart3, value); }
        public ulong IVPart4 { get => iVPart4; set => SetProperty(ref iVPart4, value); }

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

            ulong[] key = _isStrumok512? new ulong[] { KeyPart1, KeyPart2, KeyPart3, KeyPart4, KeyPart5, KeyPart6, KeyPart7, KeyPart8 }
                                       : new ulong[] { KeyPart1, KeyPart2, KeyPart3, KeyPart4 };

            ulong[] iv = { IVPart1, IVPart2, IVPart3, IVPart4 };

            KeyApplied?.Invoke(key, iv);
        }
        protected ulong GetRandomKeyPart()
        {
            byte[] buffer = new byte[8];
            random.NextBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }
    }
}
