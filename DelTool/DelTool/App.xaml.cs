using System;
using System.Diagnostics;
using System.Reflection;
using log4net;

namespace DelTool
{
    using System.Windows;
    using ViewModels;
    using Views;

    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public void ApplicationStartup(object sender, StartupEventArgs e)
        {
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            var mainWindowVm = new MainWindowVm();
            var mainWindow = new MainWindow(mainWindowVm);

            mainWindow.Show();
        }


        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //记录日志

            string exceptionMessage = e.Exception.Message;

            // 记录日志
            ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            log.Error("程序发生异常" + e.Exception.Message);

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (Application.Current.MainWindow == null || Application.Current.MainWindow.Visibility != Visibility.Visible)
                {
                    if (MessageBox.Show("程序发生异常,请重试！") == MessageBoxResult.OK)
                    {
                        Process.GetCurrentProcess().Kill();
                    }
                }
            }));
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            string errorMsg = "捕获全局异常:【非WPF窗体线程异常】 : \n\n";

            // 记录日志
            ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            if (ex != null)
                log.Error(errorMsg + ex.Message + Environment.NewLine + ex.StackTrace);

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (Application.Current.MainWindow == null || Application.Current.MainWindow.Visibility != Visibility.Visible)
                {
                    if (MessageBox.Show("程序发生异常,请重试！") == MessageBoxResult.OK)
                    {
                        Process.GetCurrentProcess().Kill();
                    }
                }
            }));
        }
    }
}
