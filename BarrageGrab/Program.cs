namespace BarrageGrab
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            //注册服务
            ServiceRegistrar.BuildServices();

            //运行主窗体
            Application.Run(ApplicationRuntime.MainForm);

            //Application.ApplicationExit += Application_ApplicationExit;
        }

        private static void Application_ApplicationExit(object? sender, EventArgs e)
        {

        }
    }
}