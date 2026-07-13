using BarrageGrab.Entity.Enums;

namespace BarrageGrab
{
    public partial class MainWindow : Form
    {
        private const int MaxConsoleLines = 10000;
        private int _consoleLineCount;
        private bool _isGrabbing;

        public MainWindow()
        {
            InitializeComponent();
            ApplicationRuntime.MainWindow = this;
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            Text = $"抖音快手Tiktok视频号WSS弹幕助手({GlobalConfigs.Version}) by 吴所畏惧 VX：xhhdqq";
            lblLocalWebSocket_Location.Text = GlobalConfigs.LocalWebSocketServer_Location;
            UpdateWebSocketStatus(running: true);

            txtLiveUrl.KeyDown += TxtLiveUrl_KeyDown;
            WireGrabServiceEvents();

            ApplicationRuntime.LivePlatform = GetSelectedPlatform();
            PrintConsole($"BarrageGrab {GlobalConfigs.Version} 已启动");
            PrintConsole($"本地 WebSocket 监听地址：{GlobalConfigs.LocalWebSocketServer_Location}");
            PrintConsole("请输入直播间地址，点击「开始」或按 Enter 开始抓取");
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_isGrabbing)
            {
                StopGrabbing();
            }
        }

        public void PrintConsole(string message)
        {
            if (IsDisposed)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(PrintConsole, message);
                return;
            }

            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            txtConsole.AppendText($"[{timestamp}] {message}{Environment.NewLine}");
            txtConsole.ScrollToCaret();

            if (++_consoleLineCount > MaxConsoleLines)
            {
                _consoleLineCount = 0;
                txtConsole.Clear();
            }
        }

        private void WireGrabServiceEvents()
        {
            if (ApplicationRuntime.BarrageGrabService == null)
            {
                return;
            }

            ApplicationRuntime.BarrageGrabService.OnOpen += (_, _) =>
            {
                PrintConsole("[系统] 直播间连接成功");
            };

            ApplicationRuntime.BarrageGrabService.OnClose += (_, _) =>
            {
                PrintConsole("[系统] 直播间连接已断开");
                if (_isGrabbing)
                {
                    BeginInvoke(ResetGrabUi);
                }
            };

            ApplicationRuntime.BarrageGrabService.OnError += (_, _) =>
            {
                if (_isGrabbing)
                {
                    BeginInvoke(ResetGrabUi);
                }
            };
        }

        private void TxtLiveUrl_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                btnGrab.PerformClick();
            }
        }

        private void btnReBoot_LocalWebSocket_Click(object sender, EventArgs e)
        {
            try
            {
                ApplicationRuntime.LocalWebSocketServer?.ReStart();
                UpdateWebSocketStatus(running: true);
                PrintConsole("[系统] 本地 WebSocket 服务已重启");
            }
            catch (Exception ex)
            {
                UpdateWebSocketStatus(running: false);
                PrintConsole($"[系统] WebSocket 重启失败：{ex.Message}");
            }
        }

        private void btnGrab_Click(object sender, EventArgs e)
        {
            if (_isGrabbing)
            {
                StopGrabbing();
                return;
            }

            StartGrabbing();
        }

        private void StartGrabbing()
        {
            var liveUrl = txtLiveUrl.Text.Trim();
            if (string.IsNullOrEmpty(liveUrl))
            {
                MessageBox.Show("直播间地址不能为空。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtLiveUrl.Focus();
                return;
            }

            var platform = GetSelectedPlatform();
            ApplicationRuntime.LivePlatform = platform;
            if (platform != PlatformTypeEnum.Douyin)
            {
                MessageBox.Show("当前开源版仅支持抖音平台，其他平台请使用技术支持版。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                ApplicationRuntime.BarrageGrabService?.Start(liveUrl);
                _isGrabbing = true;
                txtLiveUrl.Enabled = false;
                btnGrab.Text = "停止";
                btnGrab.Tag = "Stop";
                PrintConsole($"[系统] 开始抓取：{liveUrl}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"启动抓取失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetGrabUi();
            }
        }

        private void StopGrabbing()
        {
            ApplicationRuntime.BarrageGrabService?.Stop();
            ResetGrabUi();
            PrintConsole("[系统] 已停止抓取");
        }

        private void ResetGrabUi()
        {
            _isGrabbing = false;
            txtLiveUrl.Enabled = true;
            btnGrab.Text = "开始";
            btnGrab.Tag = "Start";
        }

        private PlatformTypeEnum GetSelectedPlatform()
        {
            if (radio_tiktok.Checked)
            {
                return PlatformTypeEnum.Tiktok;
            }

            if (radio_kuaishou.Checked)
            {
                return PlatformTypeEnum.Kuaishou;
            }

            if (radio_bilibili.Checked)
            {
                return PlatformTypeEnum.Bilibili;
            }

            return PlatformTypeEnum.Douyin;
        }

        private void UpdateWebSocketStatus(bool running)
        {
            lblLocalWebSocket_Status.Text = running ? "监听中" : "未启动";
            lblLocalWebSocket_Status.ForeColor = running ? Color.Green : Color.Red;
        }

        private void tsbtnAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "本程序只用作学习交流，请勿用作非法用途。如有违背，责任自行承担。\r\n" +
                "This program is only for learning and communication purposes, please do not use it for illegal purposes. " +
                "If there is any violation, the responsibility shall be borne by oneself.",
                "警告 / Warning",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }
}
