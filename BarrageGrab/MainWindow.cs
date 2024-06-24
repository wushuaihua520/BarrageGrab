using BarrageGrab.Entity.Enums;
using BarrageGrab.Framework;
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
            this.Text = $"抖音快手Tiktok视频号WSS弹幕助手({GlobalConfigs.Version}) by 吴所畏惧 VX：xhhdqq";

            this.lblLocalWebSocket_Location.Text = GlobalConfigs.LocalWebSocketServer_Location;

            #region Platform
            var platformList = new List<KeyValuePair<string, int>>();
            platformList.Add(new KeyValuePair<string, int>("抖音", 1));

            #endregion


        }

        public void PrintConsole(string message)
        {
            this.Invoke(new Action(() =>
            {
                this.txtConsole.AppendText(message + "\r\n");
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
                    MessageBox.Show("LiveId不能为空!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                ApplicationRuntime.BarrageGrabService?.Start(liveUrl);


                this.txtLiveUrl.Enabled = false;
                this.btnGrab.Text = "停止";
                this.btnGrab.Tag = "Stop";
            }
            else
            {
                ApplicationRuntime.BarrageGrabService?.Stop();

                this.txtLiveUrl.Enabled = true;
                this.btnGrab.Text = "开始";
                this.btnGrab.Tag = "Start";
            }

        }


        private void tsbtnAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("本程序只用作学习交流，请勿用作非法用途。如有违背，责任自行承担。\r\nThis program is only for learning and communication purposes, please do not use it for illegal purposes. If there is any violation, the responsibility shall be borne by oneself.", "警告/Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
