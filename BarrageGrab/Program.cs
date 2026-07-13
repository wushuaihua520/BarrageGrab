namespace BarrageGrab
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (_, args) => ShowFatalError(args.Exception);
            AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            {
                if (args.ExceptionObject is Exception ex)
                {
                    ShowFatalError(ex);
                }
            };

            ApplicationConfiguration.Initialize();

            try
            {
                ServiceRegistrar.BuildServices();
                Application.Run(new MainWindow());
            }
            finally
            {
                ApplicationRuntime.Shutdown();
            }
        }

        private static void ShowFatalError(Exception exception)
        {
            var message = exception?.Message ?? "未知错误";
            MessageBox.Show(
                $"程序发生未处理异常：{message}",
                "BarrageGrab",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
