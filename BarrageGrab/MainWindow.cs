using BarrageGrab.Entity.Enums;
using BarrageGrab.Framework;
using Google.Protobuf.WellKnownTypes;

namespace BarrageGrab
{
    public partial class MainWindow : Form
    {
        #region ����&�ֶ�

        /// <summary>
        /// ��ӡ������
        /// </summary>
        static int printCount = 0;

        #endregion


        public MainWindow()
        {
            ApplicationRuntime.MainWindow = this;

            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            this.Text = $"��������Tiktok��Ƶ��WSS��Ļ����({GlobalConfigs.Version}) by ����η�� VX��xhhdqq";

            this.lblLocalWebSocket_Location.Text = GlobalConfigs.LocalWebSocketServer_Location;

            #region Platform
            var platformList = new List<KeyValuePair<string, int>>();
            platformList.Add(new KeyValuePair<string, int>("����", 1));

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
                    MessageBox.Show("LiveId����Ϊ��!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                ApplicationRuntime.BarrageGrabService?.Start(liveUrl);


                this.txtLiveUrl.Enabled = false;
                this.btnGrab.Text = "ֹͣ";
                this.btnGrab.Tag = "Stop";
            }
            else
            {
                ApplicationRuntime.BarrageGrabService?.Stop();

                this.txtLiveUrl.Enabled = true;
                this.btnGrab.Text = "��ʼ";
                this.btnGrab.Tag = "Start";
            }

        }


        private void tsbtnAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("������ֻ����ѧϰ���������������Ƿ���;������Υ�����������ге���\r\nThis program is only for learning and communication purposes, please do not use it for illegal purposes. If there is any violation, the responsibility shall be borne by oneself.", "����/Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
