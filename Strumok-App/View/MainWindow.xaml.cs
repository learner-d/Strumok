using StrumokApp.Data;
using StrumokApp.ViewModel;
using System.ComponentModel;
using System.Windows;

namespace StrumokApp.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal MainVm ViewModel => DataContext as MainVm;
        public MainWindow()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
            DataContext = new MainVm();
        }

        protected void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UnsubscribeVmEvents(e.OldValue);
            SubscribeVmEvents(e.NewValue);
        }

        private void SubscribeVmEvents(object viewModel)
        {
            if (!(viewModel is MainVm mainViewModel))
            {
                MessageBox.Show(this, "ViewModel is missing!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            mainViewModel.MessageBroadcast += OnMainViewModelMessageBroadcast;
            mainViewModel.KeyRequested     += OnMainViewModelKeyRequested;
        }

        protected void UnsubscribeVmEvents(object viewModel)
        {
            if (!(viewModel is MainVm mainViewModel))
            {
                return;
            }

            mainViewModel.MessageBroadcast -= OnMainViewModelMessageBroadcast;
            mainViewModel.KeyRequested     -= OnMainViewModelKeyRequested;
        }

        protected void OnMainViewModelMessageBroadcast(string title, string message, eMessageLevel messageLevel)
        {
            MessageBox.Show(this, message, title, MessageBoxButton.OK, GetMessageBoxImage(messageLevel));
        }

        protected static MessageBoxImage GetMessageBoxImage(eMessageLevel eMessageLevel)
        {
            MessageBoxImage messageBoxImage = MessageBoxImage.None;
            switch (eMessageLevel)
            {
                case eMessageLevel.Warning:
                    messageBoxImage = MessageBoxImage.Warning;
                    break;
                case eMessageLevel.Error:
                    messageBoxImage = MessageBoxImage.Error;
                    break;
                case eMessageLevel.Info:
                    messageBoxImage = MessageBoxImage.Information;
                    break;
                default:
                    break;
            }
            return messageBoxImage;
        }

        protected void OnMainViewModelKeyRequested(KeyInputVm keyInputVm)
        {
            StrumokKeyInputWindow keyInputWindow = new StrumokKeyInputWindow(keyInputVm)
            {
                Owner = this
            };
            keyInputWindow.ShowDialog();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (ViewModel?.IsEncryptionOngoing == true)
                if (MessageBox.Show(this, "Шифрування ще не завершено. Бажаєте вийти?", "",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            
            base.OnClosing(e);
        }
    }
}
