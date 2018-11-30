namespace DelTool.Views
{
    using System;
    using System.Windows;
    using ViewModels;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        private readonly MainWindowVm _mainWindowVm;

        public MainWindow(MainWindowVm mainWindowVm)
        {
            this._mainWindowVm = mainWindowVm;
            DataContext = mainWindowVm;
            Closing += (sender, e) => { Dispose(); };
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        public void Dispose()
        {
            _mainWindowVm.Dispose();
        }
    }
}
