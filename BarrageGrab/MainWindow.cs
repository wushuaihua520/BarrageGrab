using BarrageGrab.Entity.Enums;
using Google.Protobuf.WellKnownTypes;

namespace BarrageGrab
{
    public partial class MainWindow : Form
    {
        #region 属性&字段

        /// <summary>
        /// 打印的行数
        /// </summary>
        static int printCount = 0;

        #endregion


        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            this.lblLocalWebSocket_Location.Text = GlobalConfig.LocalWebSocketServer_Location;

            #region Platform
            var platformList = new List<KeyValuePair<string, int>>();
            platformList.Add(new KeyValuePair<string, int>("抖音", 1));

            this.cbxPlatformType.DataSource = platformList;
            this.cbxPlatformType.DisplayMember = "Key";
            this.cbxPlatformType.SelectedIndex = 0;
            #endregion


        }

        public void PrintConsole(string message)
        {
            this.Invoke(new Action(() =>
            {
                this.txtConsole.AppendText(message); // + "\n"
                this.txtConsole.ScrollToCaret();

                if (++printCount > 10000)
                {
                    printCount = 0;
                    this.txtConsole.Clear();
                }
            }));

        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void btnReBoot_LocalWebSocket_Click(object sender, EventArgs e)
        {
            ApplicationRuntime.LocalWebSocketServer?.ReStart();
        }

        private void btnGrab_Click(object sender, EventArgs e)
        {
            object? tag = this.btnGrab.Tag;

            if (tag == null || "start".Equals(tag.ToString()?.ToLower()))
            {
                string liveUrl = this.txtLiveUrl.Text.Trim();
                if (string.IsNullOrEmpty(liveUrl))
                {
                    MessageBox.Show("直播地址不能为空!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                ApplicationRuntime.BarrageGrabService?.Start(liveUrl);

                this.cbxPlatformType.Enabled = false;
                this.txtLiveUrl.Enabled = false;
                this.btnGrab.Text = "停止";
                this.btnGrab.Tag = "Stop";
            }
            else
            {
                ApplicationRuntime.BarrageGrabService?.Stop();

                this.cbxPlatformType.Enabled = true;
                this.txtLiveUrl.Enabled = true;
                this.btnGrab.Text = "开始";
                this.btnGrab.Tag = "Start";
            }

        }
    }
}
