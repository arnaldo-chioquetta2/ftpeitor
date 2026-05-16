
namespace FTPc
{
    partial class Config
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txLocal = new System.Windows.Forms.TextBox();
            this.txCamFTP = new System.Windows.Forms.TextBox();
            this.txSenha = new System.Windows.Forms.TextBox();
            this.txUser = new System.Windows.Forms.TextBox();
            this.txHost = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btTeste = new System.Windows.Forms.Button();
            this.txPorta = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.cbFTP = new System.Windows.Forms.ComboBox();
            this.btNovo = new System.Windows.Forms.Button();
            this.btLog = new System.Windows.Forms.Button();
            this.chkLog = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Host:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "User:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "Senha:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 127);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(105, 17);
            this.label4.TabIndex = 3;
            this.label4.Text = "Caminho Local:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(12, 156);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(97, 17);
            this.label5.TabIndex = 4;
            this.label5.Text = "Caminho FTP:";
            // 
            // txLocal
            // 
            this.txLocal.Location = new System.Drawing.Point(112, 126);
            this.txLocal.Name = "txLocal";
            this.txLocal.Size = new System.Drawing.Size(273, 20);
            this.txLocal.TabIndex = 4;
            // 
            // txCamFTP
            // 
            this.txCamFTP.Location = new System.Drawing.Point(112, 156);
            this.txCamFTP.Name = "txCamFTP";
            this.txCamFTP.Size = new System.Drawing.Size(273, 20);
            this.txCamFTP.TabIndex = 5;
            // 
            // txSenha
            // 
            this.txSenha.Location = new System.Drawing.Point(112, 97);
            this.txSenha.Name = "txSenha";
            this.txSenha.Size = new System.Drawing.Size(273, 20);
            this.txSenha.TabIndex = 3;
            // 
            // txUser
            // 
            this.txUser.Location = new System.Drawing.Point(112, 68);
            this.txUser.Name = "txUser";
            this.txUser.Size = new System.Drawing.Size(273, 20);
            this.txUser.TabIndex = 2;
            // 
            // txHost
            // 
            this.txHost.Location = new System.Drawing.Point(112, 39);
            this.txHost.Name = "txHost";
            this.txHost.Size = new System.Drawing.Size(273, 20);
            this.txHost.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(216, 208);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btTeste
            // 
            this.btTeste.Location = new System.Drawing.Point(111, 208);
            this.btTeste.Name = "btTeste";
            this.btTeste.Size = new System.Drawing.Size(75, 23);
            this.btTeste.TabIndex = 7;
            this.btTeste.Text = "Testar";
            this.btTeste.UseVisualStyleBackColor = true;
            this.btTeste.Click += new System.EventHandler(this.btTeste_Click);
            // 
            // txPorta
            // 
            this.txPorta.Location = new System.Drawing.Point(112, 182);
            this.txPorta.Name = "txPorta";
            this.txPorta.Size = new System.Drawing.Size(273, 20);
            this.txPorta.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(12, 182);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 17);
            this.label6.TabIndex = 12;
            this.label6.Text = "Porta:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(12, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(34, 17);
            this.label7.TabIndex = 14;
            this.label7.Text = "FTP";
            // 
            // cbFTP
            // 
            this.cbFTP.FormattingEnabled = true;
            this.cbFTP.Location = new System.Drawing.Point(112, 12);
            this.cbFTP.Name = "cbFTP";
            this.cbFTP.Size = new System.Drawing.Size(216, 21);
            this.cbFTP.TabIndex = 0;
            this.cbFTP.SelectedIndexChanged += new System.EventHandler(this.cbFTP_SelectedIndexChanged);
            // 
            // btNovo
            // 
            this.btNovo.Location = new System.Drawing.Point(334, 12);
            this.btNovo.Name = "btNovo";
            this.btNovo.Size = new System.Drawing.Size(51, 21);
            this.btNovo.TabIndex = 15;
            this.btNovo.Text = "Novo";
            this.btNovo.UseVisualStyleBackColor = true;
            this.btNovo.Click += new System.EventHandler(this.btNovo_Click);
            // 
            // btLog
            // 
            this.btLog.Location = new System.Drawing.Point(310, 208);
            this.btLog.Name = "btLog";
            this.btLog.Size = new System.Drawing.Size(75, 23);
            this.btLog.TabIndex = 9;
            this.btLog.Text = "Ver Log";
            this.btLog.UseVisualStyleBackColor = true;
            this.btLog.Click += new System.EventHandler(this.btLog_Click);
            // 
            // chkLog
            // 
            this.chkLog.AutoSize = true;
            this.chkLog.Location = new System.Drawing.Point(15, 212);
            this.chkLog.Name = "chkLog";
            this.chkLog.Size = new System.Drawing.Size(72, 17);
            this.chkLog.TabIndex = 16;
            this.chkLog.Text = "Gerar log";
            this.chkLog.UseVisualStyleBackColor = true;
            // 
            // Config
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(397, 240);
            this.Controls.Add(this.chkLog);
            this.Controls.Add(this.btLog);
            this.Controls.Add(this.btNovo);
            this.Controls.Add(this.cbFTP);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txPorta);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btTeste);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txHost);
            this.Controls.Add(this.txUser);
            this.Controls.Add(this.txSenha);
            this.Controls.Add(this.txCamFTP);
            this.Controls.Add(this.txLocal);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Config";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configuração";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Config_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txLocal;
        private System.Windows.Forms.TextBox txCamFTP;
        private System.Windows.Forms.TextBox txSenha;
        private System.Windows.Forms.TextBox txUser;
        private System.Windows.Forms.TextBox txHost;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btTeste;
        private System.Windows.Forms.TextBox txPorta;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cbFTP;
        private System.Windows.Forms.Button btNovo;
        private System.Windows.Forms.Button btLog;
        private System.Windows.Forms.CheckBox chkLog;
    }
}
