namespace BarrageGrab
{
    partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            label1 = new Label();
            btnReBoot_LocalWebSocket = new Button();
            groupBox1 = new GroupBox();
            lblLocalWebSocket_Location = new Label();
            lblLocalWebSocket_Status = new Label();
            label2 = new Label();
            label5 = new Label();
            groupBox2 = new GroupBox();
            radio_huya = new RadioButton();
            radio_tiktok = new RadioButton();
            radio_acfun = new RadioButton();
            radio_douyu = new RadioButton();
            radio_bilibili = new RadioButton();
            radio_kuaishou = new RadioButton();
            radio_douyin = new RadioButton();
            txtConsole = new RichTextBox();
            txtLiveUrl = new TextBox();
            label4 = new Label();
            label3 = new Label();
            btnGrab = new Button();
            toolStrip1 = new ToolStrip();
            tsbtnAbout = new ToolStripButton();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(50, 29);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(68, 17);
            label1.TabIndex = 0;
            label1.Text = "监听地址：";
            // 
            // btnReBoot_LocalWebSocket
            // 
            btnReBoot_LocalWebSocket.Font = new Font("Microsoft YaHei UI", 8F);
            btnReBoot_LocalWebSocket.Location = new Point(182, 53);
            btnReBoot_LocalWebSocket.Margin = new Padding(2);
            btnReBoot_LocalWebSocket.Name = "btnReBoot_LocalWebSocket";
            btnReBoot_LocalWebSocket.Size = new Size(38, 21);
            btnReBoot_LocalWebSocket.TabIndex = 2;
            btnReBoot_LocalWebSocket.Text = "重启";
            btnReBoot_LocalWebSocket.UseVisualStyleBackColor = true;
            btnReBoot_LocalWebSocket.Click += btnReBoot_LocalWebSocket_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(lblLocalWebSocket_Location);
            groupBox1.Controls.Add(lblLocalWebSocket_Status);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(btnReBoot_LocalWebSocket);
            groupBox1.Dock = DockStyle.Top;
            groupBox1.Location = new Point(10, 32);
            groupBox1.Margin = new Padding(2);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(2);
            groupBox1.Size = new Size(547, 87);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Text = "本地WebSocket服务";
            // 
            // lblLocalWebSocket_Location
            // 
            lblLocalWebSocket_Location.AutoSize = true;
            lblLocalWebSocket_Location.ForeColor = Color.Red;
            lblLocalWebSocket_Location.Location = new Point(121, 29);
            lblLocalWebSocket_Location.Margin = new Padding(2, 0, 2, 0);
            lblLocalWebSocket_Location.Name = "lblLocalWebSocket_Location";
            lblLocalWebSocket_Location.Size = new Size(104, 17);
            lblLocalWebSocket_Location.TabIndex = 0;
            lblLocalWebSocket_Location.Text = "ws://0.0.0.0:8888";
            // 
            // lblLocalWebSocket_Status
            // 
            lblLocalWebSocket_Status.AutoSize = true;
            lblLocalWebSocket_Status.ForeColor = Color.Green;
            lblLocalWebSocket_Status.Location = new Point(121, 55);
            lblLocalWebSocket_Status.Margin = new Padding(2, 0, 2, 0);
            lblLocalWebSocket_Status.Name = "lblLocalWebSocket_Status";
            lblLocalWebSocket_Status.Size = new Size(44, 17);
            lblLocalWebSocket_Status.TabIndex = 0;
            lblLocalWebSocket_Status.Text = "监听中";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(50, 55);
            label2.Margin = new Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new Size(68, 17);
            label2.TabIndex = 0;
            label2.Text = "服务状态：";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.ForeColor = Color.Fuchsia;
            label5.Location = new Point(290, 29);
            label5.Margin = new Padding(2, 0, 2, 0);
            label5.Name = "label5";
            label5.Size = new Size(206, 17);
            label5.TabIndex = 0;
            label5.Text = "* 感谢点小心心的你，好人一生平安~";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(radio_huya);
            groupBox2.Controls.Add(radio_tiktok);
            groupBox2.Controls.Add(radio_acfun);
            groupBox2.Controls.Add(radio_douyu);
            groupBox2.Controls.Add(radio_bilibili);
            groupBox2.Controls.Add(radio_kuaishou);
            groupBox2.Controls.Add(radio_douyin);
            groupBox2.Controls.Add(txtConsole);
            groupBox2.Controls.Add(txtLiveUrl);
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(btnGrab);
            groupBox2.Dock = DockStyle.Top;
            groupBox2.Location = new Point(10, 119);
            groupBox2.Margin = new Padding(2);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(2);
            groupBox2.Size = new Size(547, 402);
            groupBox2.TabIndex = 4;
            groupBox2.TabStop = false;
            groupBox2.Text = "弹幕抓取服务";
            // 
            // radio_huya
            // 
            radio_huya.AutoSize = true;
            radio_huya.Enabled = false;
            radio_huya.Location = new Point(446, 25);
            radio_huya.Margin = new Padding(2);
            radio_huya.Name = "radio_huya";
            radio_huya.Size = new Size(50, 21);
            radio_huya.TabIndex = 6;
            radio_huya.Tag = "7";
            radio_huya.Text = "虎牙";
            radio_huya.UseVisualStyleBackColor = true;
            // 
            // radio_tiktok
            // 
            radio_tiktok.AutoSize = true;
            radio_tiktok.Enabled = false;
            radio_tiktok.Location = new Point(386, 25);
            radio_tiktok.Margin = new Padding(2);
            radio_tiktok.Name = "radio_tiktok";
            radio_tiktok.Size = new Size(62, 21);
            radio_tiktok.TabIndex = 5;
            radio_tiktok.Tag = "6";
            radio_tiktok.Text = "Tiktok";
            radio_tiktok.UseVisualStyleBackColor = true;
            // 
            // radio_acfun
            // 
            radio_acfun.AutoSize = true;
            radio_acfun.Enabled = false;
            radio_acfun.Location = new Point(329, 25);
            radio_acfun.Margin = new Padding(2);
            radio_acfun.Name = "radio_acfun";
            radio_acfun.Size = new Size(58, 21);
            radio_acfun.TabIndex = 4;
            radio_acfun.Tag = "5";
            radio_acfun.Text = "Acfun";
            radio_acfun.UseVisualStyleBackColor = true;
            // 
            // radio_douyu
            // 
            radio_douyu.AutoSize = true;
            radio_douyu.Enabled = false;
            radio_douyu.Location = new Point(280, 25);
            radio_douyu.Margin = new Padding(2);
            radio_douyu.Name = "radio_douyu";
            radio_douyu.Size = new Size(50, 21);
            radio_douyu.TabIndex = 3;
            radio_douyu.Tag = "4";
            radio_douyu.Text = "斗鱼";
            radio_douyu.UseVisualStyleBackColor = true;
            // 
            // radio_bilibili
            // 
            radio_bilibili.AutoSize = true;
            radio_bilibili.Enabled = false;
            radio_bilibili.Location = new Point(219, 25);
            radio_bilibili.Margin = new Padding(2);
            radio_bilibili.Name = "radio_bilibili";
            radio_bilibili.Size = new Size(60, 21);
            radio_bilibili.TabIndex = 2;
            radio_bilibili.Tag = "3";
            radio_bilibili.Text = "bilibili";
            radio_bilibili.UseVisualStyleBackColor = true;
            // 
            // radio_kuaishou
            // 
            radio_kuaishou.AutoSize = true;
            radio_kuaishou.Enabled = false;
            radio_kuaishou.Location = new Point(170, 25);
            radio_kuaishou.Margin = new Padding(2);
            radio_kuaishou.Name = "radio_kuaishou";
            radio_kuaishou.Size = new Size(50, 21);
            radio_kuaishou.TabIndex = 1;
            radio_kuaishou.Tag = "2";
            radio_kuaishou.Text = "快手";
            radio_kuaishou.UseVisualStyleBackColor = true;
            // 
            // radio_douyin
            // 
            radio_douyin.AutoSize = true;
            radio_douyin.Checked = true;
            radio_douyin.Location = new Point(121, 25);
            radio_douyin.Margin = new Padding(2);
            radio_douyin.Name = "radio_douyin";
            radio_douyin.Size = new Size(50, 21);
            radio_douyin.TabIndex = 0;
            radio_douyin.TabStop = true;
            radio_douyin.Tag = "1";
            radio_douyin.Text = "抖音";
            radio_douyin.UseVisualStyleBackColor = true;
            // 
            // txtConsole
            // 
            txtConsole.BackColor = SystemColors.InfoText;
            txtConsole.Font = new Font("Microsoft YaHei UI", 8F);
            txtConsole.ForeColor = Color.White;
            txtConsole.Location = new Point(4, 79);
            txtConsole.Margin = new Padding(2);
            txtConsole.Name = "txtConsole";
            txtConsole.ReadOnly = true;
            txtConsole.Size = new Size(597, 321);
            txtConsole.TabIndex = 4;
            txtConsole.TabStop = false;
            txtConsole.Text = "";
            // 
            // txtLiveUrl
            // 
            txtLiveUrl.Location = new Point(121, 51);
            txtLiveUrl.Margin = new Padding(2);
            txtLiveUrl.Name = "txtLiveUrl";
            txtLiveUrl.Size = new Size(311, 23);
            txtLiveUrl.TabIndex = 0;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(64, 53);
            label4.Margin = new Padding(2, 0, 2, 0);
            label4.Name = "label4";
            label4.Size = new Size(54, 17);
            label4.TabIndex = 0;
            label4.Text = "LiveId：";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(50, 26);
            label3.Margin = new Padding(2, 0, 2, 0);
            label3.Name = "label3";
            label3.Size = new Size(68, 17);
            label3.TabIndex = 0;
            label3.Text = "抓取平台：";
            // 
            // btnGrab
            // 
            btnGrab.Font = new Font("Microsoft YaHei UI", 8F);
            btnGrab.Location = new Point(436, 50);
            btnGrab.Margin = new Padding(2);
            btnGrab.Name = "btnGrab";
            btnGrab.Size = new Size(60, 25);
            btnGrab.TabIndex = 1;
            btnGrab.Tag = "Start";
            btnGrab.Text = "开始";
            btnGrab.UseVisualStyleBackColor = true;
            btnGrab.Click += btnGrab_Click;
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new ToolStripItem[] { tsbtnAbout });
            toolStrip1.Location = new Point(10, 7);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(547, 25);
            toolStrip1.TabIndex = 5;
            toolStrip1.Text = "toolStrip1";
            // 
            // tsbtnAbout
            // 
            tsbtnAbout.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsbtnAbout.Image = (Image)resources.GetObject("tsbtnAbout.Image");
            tsbtnAbout.ImageTransparentColor = Color.Magenta;
            tsbtnAbout.Name = "tsbtnAbout";
            tsbtnAbout.Size = new Size(79, 22);
            tsbtnAbout.Text = "关于(&About)";
            tsbtnAbout.Click += tsbtnAbout_Click;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(567, 534);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(toolStrip1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(2);
            MaximizeBox = false;
            Name = "MainWindow";
            Padding = new Padding(10, 7, 10, 7);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "抖音快手Tiktok视频号WSS弹幕助手(v1.8.0) by 吴所畏惧 VX：xhhdqq";
            FormClosed += MainWindow_FormClosed;
            Load += MainWindow_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Button btnReBoot_LocalWebSocket;
        private GroupBox groupBox1;
        private Label lblLocalWebSocket_Location;
        private Label label2;
        private Label lblLocalWebSocket_Status;
        private GroupBox groupBox2;
        private Label label3;
        private Label label4;
        private TextBox txtLiveUrl;
        private Button btnGrab;
        private RichTextBox txtConsole;
        private Label label5;
        private RadioButton radio_douyin;
        private RadioButton radio_kuaishou;
        private RadioButton radio_bilibili;
        private RadioButton radio_douyu;
        private RadioButton radio_acfun;
        private RadioButton radio_tiktok;
        private RadioButton radio_huya;
        private ToolStrip toolStrip1;
        private ToolStripButton tsbtnAbout;
    }
}
