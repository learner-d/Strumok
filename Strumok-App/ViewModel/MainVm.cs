using Prism.Commands;
using Prism.Mvvm;
using StrumokApp.Util;
using StrumokLib;
using System.IO;
using System.Threading;
using System.Windows.Input;
using System;
using System.Threading.Tasks;
using StrumokApp.Data;

namespace StrumokApp.ViewModel
{
    public enum eCryptMode { None, Text, File };

    internal class MainVm : BindableBase
    {
        protected readonly KeyInputVm keyInputVm = new KeyInputVm();
        protected readonly MessageContainer MSG_ENCRYPTION_FAILED = new MessageContainer("Помилка шифрування", eMessageLevel.Error);
        protected readonly MessageContainer MSG_ENCRYPTION_CANCELED = new MessageContainer("Шифрування скасовано", eMessageLevel.Info);
        protected readonly MessageContainer MSG_ENCRYPTION_CHOOSE_FILES = new MessageContainer("Оберіть вхідний і вихідний файли для шифрування.", eMessageLevel.Info);
        protected readonly MessageContainer MSG_ENCRYPTION_FILES_READY = new MessageContainer("Файл готовий до шифрування.", eMessageLevel.Success);
        protected readonly MessageContainer MSG_ENCRYPTION_COMPLETE = new MessageContainer("Шифрування завершено", eMessageLevel.Success);

        public DelegateCommand CryptCommand { get; }
        public DelegateCommand KeySetupCommand { get; }
        public DelegateCommand SelectInputFileCommand { get; }
        public DelegateCommand SelectOutputFileCommand { get; }
        public DelegateCommand CancelEncryptionCommand { get; }

        public eCryptMode CryptMode => TextCryptMode ? eCryptMode.Text : FileCryptMode ? eCryptMode.File : eCryptMode.None;

        protected bool _textCryptMode = true;
        public bool TextCryptMode
        {
            get => _textCryptMode; 
            set
            {
                SetProperty(ref _textCryptMode, value);
                CanEncrypt = CheckCanEncrypt();
                ClearEncryptionStatus();
            }
        }
        protected bool _fileCryptMode = false;
        public bool FileCryptMode 
        {
            get => _fileCryptMode;
            set 
            {
                SetProperty(ref _fileCryptMode, value);
                CanEncrypt = CheckCanEncrypt();
                ClearEncryptionStatus();
                if (value)
                    ValidatePathsAndSetStatus();
            }
        }

        // Властивість "Ключ шифрування"
        protected ulong[] _strumokKey = new ulong[4];
        public ulong[] StrumokKey
        {
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

        private bool _inputFilePathIsValid;
        public bool InputFilePathIsValid
        {
            get => _inputFilePathIsValid;
            set
            {
                SetProperty(ref _inputFilePathIsValid, value);
                CanEncrypt = CheckCanEncrypt();
                ValidatePathsAndSetStatus();
            }
        }

        // Властивість "Шлях до файлу"
        protected string _sourceFilePath = "";
        public string InputFilePath
        {
            get => _sourceFilePath;
            set
            {
                SetProperty(ref _sourceFilePath, value);
                InputFilePathIsValid = IsInputFilePathValid(value);
            }
        }

        private bool _outputFilePathIsValid;
        public bool OutputFilePathIsValid
        {
            get => _outputFilePathIsValid;
            set
            {
                SetProperty(ref _outputFilePathIsValid, value);
                CanEncrypt = CheckCanEncrypt();
                ValidatePathsAndSetStatus();
            }
        }

        // Властивість "Шлях до вихідного файлу"
        protected string _outputFilePath = "";
        public string OutputFilePath
        {
            get => _outputFilePath;
            set
            {
                SetProperty(ref _outputFilePath, value);
                OutputFilePathIsValid = IsOutputFilePathValid(value);
            }
        }

        protected bool _canEncrypt = false;
        public bool CanEncrypt
        {
            get => _canEncrypt;
            set
            {
                SetProperty(ref _canEncrypt, value);
                CryptCommand.RaiseCanExecuteChanged();
            }
        }

        protected bool _isEncryptionOngoing = false;
        public bool IsEncryptionOngoing
        {
            get => _isEncryptionOngoing;
            set
            {
                SetProperty(ref _isEncryptionOngoing, value);
                KeySetupCommand.RaiseCanExecuteChanged();
                SelectInputFileCommand.RaiseCanExecuteChanged();
                SelectOutputFileCommand.RaiseCanExecuteChanged();
            }
        }
        protected bool _isLongEncryptionOngoing = false;
        public bool IsLongEncryptionOngoing
        {
            get => _isLongEncryptionOngoing;
            set => SetProperty(ref _isLongEncryptionOngoing, value);
        }

        protected double _encryptionProgress = 0;
        public double EncryptionProgress
        {
            get => _encryptionProgress;
            set => SetProperty(ref _encryptionProgress, value);
        }

        protected MessageContainer _encryptionStatus;
        public MessageContainer EncryptionStatus 
        {
            get => _encryptionStatus;
            set => SetProperty(ref _encryptionStatus, value);
        }

        public event MessageDelegate MessageBroadcast;
        public event Action<KeyInputVm> KeyRequested;

        public MainVm()
        {
            keyInputVm.KeyApplied += OnKeyApplied;

            CryptCommand = new DelegateCommand(Crypt).ObservesCanExecute(()=>CanEncrypt);
            KeySetupCommand = new DelegateCommand(KeySetup);
            SelectInputFileCommand = new DelegateCommand(SelectInputFile);
            SelectOutputFileCommand = new DelegateCommand(SelectOutputFile);
            CancelEncryptionCommand = new DelegateCommand(CancelEncryption);
        }

        protected void OnKeyApplied(StrumokKeyConfiguration keyConfiguration)
        {
            StrumokKey = keyConfiguration.Key;
            StrumokIv = keyConfiguration.IV;
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

        CancellationTokenSource _encryptionCancellationTokenSource;
        protected async void CryptText()
        {
            if (Text == null)
                return;
            try
            {
                using (StrumokCrypter _strumokCrypter = new StrumokCrypter(_strumokKey, _strumokIv))
                using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
                {
                    IsEncryptionOngoing = true;
                    _encryptionCancellationTokenSource = cancellationTokenSource;
                    CryptedText = await Task.Run(() => _strumokCrypter.Crypt(Text), cancellationTokenSource.Token);
                    EncryptionStatus = MSG_ENCRYPTION_COMPLETE;
                }
            }
            catch (TaskCanceledException e)
            {
                EncryptionStatus = MSG_ENCRYPTION_CANCELED;
            }
            catch (Exception e)
            {
                EncryptionStatus = MSG_ENCRYPTION_FAILED;
            }
            finally
            {
                _encryptionCancellationTokenSource = null;
                IsEncryptionOngoing = false;
            }
        }

        protected async void CryptFile()
        {
            if (IsEncryptionOngoing)
            {
                BroadcastMessage("Поточне шифрування ще не завершено", "", eMessageLevel.Warning);
                return;
            }
            // Перевірити шлях
            if (!InputFilePathIsValid)
            {
                BroadcastMessage("Помилка: невірний вхідний шлях", "", eMessageLevel.Warning);
                return;
            }
            if (!OutputFilePathIsValid)
            {
                BroadcastMessage("Помилка: невірний вихідний шлях", "", eMessageLevel.Warning);
                return;
            }
            IsEncryptionOngoing = true;
            EncryptionProgress = 0;
            try
            {
                using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
                {
                    bool canceled = false;
                    bool errorThrown = false;
                    _encryptionCancellationTokenSource = cancellationTokenSource;
                    try
                    {
                        _ = Task.Delay(1000, cancellationTokenSource.Token).ContinueWith(task =>
                        {
                            IsLongEncryptionOngoing = IsEncryptionOngoing;
                        });
                        EncryptionStatus = new MessageContainer("Шифрування в процесі...", eMessageLevel.Info);
                        await StrumokCrypterUtils.DoCryption(InputFilePath, OutputFilePath, _strumokKey, _strumokIv,
                            new Progress<double>(progress =>
                            {
                                EncryptionProgress = progress;
                            }),
                            cancellationTokenSource.Token);
                        BroadcastMessage("Шифрування успішно завершено", "", eMessageLevel.Info);
                        EncryptionStatus = MSG_ENCRYPTION_COMPLETE;
                    }
                    catch (FileNotFoundException e)
                    {
                        errorThrown = true;
                        BroadcastMessage($"Файл {e.FileName} не знайдено!", "", eMessageLevel.Warning);
                    }
                    catch (DirectoryNotFoundException e)
                    {
                        errorThrown = true;
                        BroadcastMessage($"Папку не знайдено!", "", eMessageLevel.Warning);
                    }
                    catch (DriveNotFoundException e)
                    {
                        errorThrown = true;
                        BroadcastMessage($"Невірна буква диску.", "", eMessageLevel.Warning);
                    }
                    catch (PathTooLongException e)
                    {
                        errorThrown = true;
                        BroadcastMessage("Шлях до файлу занадто довгий.", "", eMessageLevel.Warning);
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        errorThrown = true;
                        BroadcastMessage("Відсутній дозвіл оперувати з файлом", "", eMessageLevel.Warning);
                    }
                    catch (IOException e) when ((e.HResult & 0x0000FFFF) == 32)
                    {
                        errorThrown = true;
                        BroadcastMessage("Файл уже зайнято іншим процесом. Спробуйте закрити файл, якщо його було відкрито у іншій програмі",
                            "", eMessageLevel.Warning);
                    }
                    catch (IOException e) when ((e.HResult & 0x0000FFFF) == 80)
                    {
                        errorThrown = true;
                        BroadcastMessage("Такий файл уже існує", "", eMessageLevel.Warning);
                    }
                    catch (IOException e)
                    {
                        errorThrown = true;
                        BroadcastMessage($"Помилка шифрування файлу. Кинуто IOException." +
                            $"\nКод помилки: {e.HResult & 0x0000FFFF}\nТекст помилки: {e.Message}",
                            "", eMessageLevel.Warning);
                    }
                    catch (TaskCanceledException e)
                    {
                        canceled = true;
                        BroadcastMessage("Операцію шифрування скасовано.", "", eMessageLevel.Info);
                    }
                    catch (Exception e)
                    {
                        errorThrown = true;
                        EncryptionStatus = null;
                        throw;
                    }
                    if (errorThrown)
                        EncryptionStatus = MSG_ENCRYPTION_FAILED;
                    else if (canceled)
                        EncryptionStatus = MSG_ENCRYPTION_CANCELED;
                }
            }
            finally
            {
                _encryptionCancellationTokenSource = null;
                IsEncryptionOngoing = false;
                IsLongEncryptionOngoing = false;
                EncryptionProgress = 0;
            }
        }

        protected void CancelEncryption()
        {
            _encryptionCancellationTokenSource?.Cancel();
        }

        protected void BroadcastMessage(string text, string title = "", eMessageLevel messageLevel = eMessageLevel.Default)
        {
            MessageBroadcast?.Invoke(title, text, messageLevel);
        }

        protected void ClearEncryptionStatus()
        {
            EncryptionStatus = null;
        }

        protected void KeySetup()
        {
            KeyRequested?.Invoke(keyInputVm);
        }

        protected bool IsInputFilePathValid(string path)
        {
            return File.Exists(path);
        }

        protected bool IsOutputFilePathValid(string path)
        {
            return !string.IsNullOrEmpty(path);
        }

        protected void ValidatePathsAndSetStatus()
        {
            EncryptionStatus = (InputFilePathIsValid && OutputFilePathIsValid)
                ? MSG_ENCRYPTION_FILES_READY : MSG_ENCRYPTION_CHOOSE_FILES;
        }

        protected bool CheckCanEncrypt()
        {
            if (CryptMode == eCryptMode.Text)
                return true;
            else if (CryptMode == eCryptMode.File)
                return InputFilePathIsValid && OutputFilePathIsValid;
            else
                return false;
        }

        /// <summary>
        /// Обирає файл для шифрування
        /// </summary>
        protected void SelectInputFile()
        {
            string path = ExplorerUtils.ShowOpenFileDialog("Оберіть файл для шифрування");
            if (!string.IsNullOrEmpty(path))
            {
                InputFilePath = path;
            }
        }

        /// <summary>
        /// Обирає шлях для збереження зашифрованого файлу
        /// </summary>
        protected void SelectOutputFile()
        {
            string path = ExplorerUtils.ShowSaveFileDialog("Зберегти шифрований файл");
            if (!string.IsNullOrEmpty(path))
            {
                OutputFilePath = path;
            }
        }
    }
}
