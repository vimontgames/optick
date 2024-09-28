using System;
using System.Windows;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace Profiler
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{        

        static App()
        {

		}

        protected override void OnStartup(StartupEventArgs e)
        {
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			base.OnStartup(e);
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Exception ex = e.ExceptionObject as Exception;
			ReportError(ex);
		}

		private void Optick_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			e.Handled = ReportError(e.Exception);
		}

		bool ReportError(Exception ex)
		{
			Exception rootException = ex;

			while (rootException.InnerException != null)
				rootException = rootException.InnerException;
			
			MessageBox.Show(rootException.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}
	}
}
