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
            label1 = new Label();
            btnReBoot_LocalWebSocket = new Button();
            groupBox1 = new GroupBox();
            lblLocalWebSocket_Location = new Label();
            lblLocalWebSocket_Status = new Label();
            label2 = new Label();
            label5 = new Label();
            groupBox2 = new GroupBox();
            txtConsole = new RichTextBox();
            txtLiveUrl = new TextBox();
            cbxPlatformType = new ComboBox();
            label4 = new Label();
            label3 = new Label();
            btnGrab = new Button();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 44);
            label1.Name = "label1";
            label1.Size = new Size(100, 24);
            label1.TabIndex = 0;
            label1.Text = "监听地址：";
            // 
            // btnReBoot_LocalWebSocket
            // 
            btnReBoot_LocalWebSocket.Font = new Font("Microsoft YaHei UI", 8F);
            btnReBoot_LocalWebSocket.Location = new Point(214, 77);
            btnReBoot_LocalWebSocket.Name = "btnReBoot_LocalWebSocket";
            btnReBoot_LocalWebSocket.Size = new Size(60, 30);
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
            groupBox1.Location = new Point(15, 10);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(948, 123);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Text = "本地WebSocket服务";
            // 
            // lblLocalWebSocket_Location
            // 
            lblLocalWebSocket_Location.AutoSize = true;
            lblLocalWebSocket_Location.ForeColor = Color.Red;
            lblLocalWebSocket_Location.Location = new Point(118, 44);
            lblLocalWebSocket_Location.Name = "lblLocalWebSocket_Location";
            lblLocalWebSocket_Location.Size = new Size(156, 24);
            lblLocalWebSocket_Location.TabIndex = 0;
            lblLocalWebSocket_Location.Text = "ws://0.0.0.0:8888";
            // 
            // lblLocalWebSocket_Status
            // 
            lblLocalWebSocket_Status.AutoSize = true;
            lblLocalWebSocket_Status.ForeColor = Color.Green;
            lblLocalWebSocket_Status.Location = new Point(118, 80);
            lblLocalWebSocket_Status.Name = "lblLocalWebSocket_Status";
            lblLocalWebSocket_Status.Size = new Size(64, 24);
            lblLocalWebSocket_Status.TabIndex = 0;
            lblLocalWebSocket_Status.Text = "监听中";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(6, 80);
            label2.Name = "label2";
            label2.Size = new Size(100, 24);
            label2.TabIndex = 0;
            label2.Text = "服务状态：";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.ForeColor = Color.Fuchsia;
            label5.Location = new Point(570, 44);
            label5.Name = "label5";
            label5.Size = new Size(306, 24);
            label5.TabIndex = 0;
            label5.Text = "* 感谢点小心心的你，好人一生平安~";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(txtConsole);
            groupBox2.Controls.Add(txtLiveUrl);
            groupBox2.Controls.Add(cbxPlatformType);
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(btnGrab);
            groupBox2.Dock = DockStyle.Top;
            groupBox2.Location = new Point(15, 133);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(948, 568);
            groupBox2.TabIndex = 4;
            groupBox2.TabStop = false;
            groupBox2.Text = "弹幕抓取服务";
            // 
            // txtConsole
            // 
            txtConsole.BackColor = SystemColors.InfoText;
            txtConsole.Font = new Font("Microsoft YaHei UI", 8F);
            txtConsole.ForeColor = Color.White;
            txtConsole.Location = new Point(6, 111);
            txtConsole.Name = "txtConsole";
            txtConsole.ReadOnly = true;
            txtConsole.Size = new Size(936, 451);
            txtConsole.TabIndex = 4;
            txtConsole.TabStop = false;
            txtConsole.Text = "";
            // 
            // txtLiveUrl
            // 
            txtLiveUrl.Location = new Point(118, 75);
            txtLiveUrl.Name = "txtLiveUrl";
            txtLiveUrl.Size = new Size(758, 30);
            txtLiveUrl.TabIndex = 0;
            // 
            // cbxPlatformType
            // 
            cbxPlatformType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxPlatformType.FormattingEnabled = true;
            cbxPlatformType.Location = new Point(118, 37);
            cbxPlatformType.Name = "cbxPlatformType";
            cbxPlatformType.Size = new Size(824, 32);
            cbxPlatformType.TabIndex = 3;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(28, 78);
            label4.Name = "label4";
            label4.Size = new Size(78, 24);
            label4.TabIndex = 0;
            label4.Text = "LiveId：";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 40);
            label3.Name = "label3";
            label3.Size = new Size(100, 24);
            label3.TabIndex = 0;
            label3.Text = "抓取平台：";
            // 
            // btnGrab
            // 
            btnGrab.Font = new Font("Microsoft YaHei UI", 8F);
            btnGrab.Location = new Point(882, 75);
            btnGrab.Name = "btnGrab";
            btnGrab.Size = new Size(60, 30);
            btnGrab.TabIndex = 1;
            btnGrab.Tag = "Start";
            btnGrab.Text = "开始";
            btnGrab.UseVisualStyleBackColor = true;
            btnGrab.Click += btnGrab_Click;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(978, 714);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainWindow";
            Padding = new Padding(15, 10, 15, 10);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "抖音快手bilibili直播弹幕wss直连（v0.5.0 - gitee/github:吴所畏惧）";
            FormClosed += MainWindow_FormClosed;
            Load += MainWindow_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);
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
        private ComboBox cbxPlatformType;
        private TextBox txtLiveUrl;
        private Button btnGrab;
        private RichTextBox txtConsole;
        private Label label5;
    }
}
