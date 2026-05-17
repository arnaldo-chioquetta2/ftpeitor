namespace FTPc
{
    partial class Tela
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Tela));
            this.Label1 = new System.Windows.Forms.Label();
            this.ProgressBar1 = new System.Windows.Forms.ProgressBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lbErro = new System.Windows.Forms.Label();
            this.btInicio = new System.Windows.Forms.Button();
            this.btConfig = new System.Windows.Forms.Button();
            this.btDownload = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // Label1
            // 
            this.Label1.Location = new System.Drawing.Point(2, 9);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(418, 22);
            this.Label1.TabIndex = 0;
            this.Label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ProgressBar1
            // 
            this.ProgressBar1.Location = new System.Drawing.Point(12, 34);
            this.ProgressBar1.Name = "ProgressBar1";
            this.ProgressBar1.Size = new System.Drawing.Size(402, 23);
            this.ProgressBar1.TabIndex = 1;
            this.ProgressBar1.Visible = false;
            // 
            // timer1
            // 
            this.timer1.Interval = 2000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lbErro
            // 
            this.lbErro.Location = new System.Drawing.Point(2, 27);
            this.lbErro.Name = "lbErro";
            this.lbErro.Size = new System.Drawing.Size(418, 22);
            this.lbErro.TabIndex = 2;
            this.lbErro.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbErro.Visible = false;
            // 
            // btInicio
            // 
            this.btInicio.Location = new System.Drawing.Point(12, 26);
            this.btInicio.Name = "btInicio";
            this.btInicio.Size = new System.Drawing.Size(112, 23);
            this.btInicio.TabIndex = 3;
            this.btInicio.Text = "Iniciar";
            this.btInicio.UseVisualStyleBackColor = true;
            this.btInicio.Click += new System.EventHandler(this.btInicio_Click);
            // 
            // btConfig
            // 
            this.btConfig.Location = new System.Drawing.Point(160, 26);
            this.btConfig.Name = "btConfig";
            this.btConfig.Size = new System.Drawing.Size(112, 23);
            this.btConfig.TabIndex = 4;
            this.btConfig.Text = "Configurar";
            this.btConfig.UseVisualStyleBackColor = true;
            this.btConfig.Click += new System.EventHandler(this.btConfig_Click);
            // 
            // btDownload
            // 
            this.btDownload.Location = new System.Drawing.Point(308, 26);
            this.btDownload.Name = "btDownload";
            this.btDownload.Size = new System.Drawing.Size(112, 23);
            this.btDownload.TabIndex = 6;
            this.btDownload.Text = "Download";
            this.btDownload.UseVisualStyleBackColor = true;
            this.btDownload.Click += new System.EventHandler(this.btDownload_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Lime;
            this.pictureBox1.Location = new System.Drawing.Point(12, 34);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(402, 23);
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            // 
            // Tela
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(429, 76);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btDownload);
            this.Controls.Add(this.btConfig);
            this.Controls.Add(this.btInicio);
            this.Controls.Add(this.lbErro);
            this.Controls.Add(this.ProgressBar1);
            this.Controls.Add(this.Label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(597, 752);
            this.MaximizeBox = false;
            this.Name = "Tela";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "FTPeia";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Tela_Load);
            this.Shown += new System.EventHandler(this.Tela_Shown);
            this.Resize += new System.EventHandler(this.Tela_Resize_1);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label Label1;
        private System.Windows.Forms.ProgressBar ProgressBar1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lbErro;
        private System.Windows.Forms.Button btInicio;
        private System.Windows.Forms.Button btConfig;
        private System.Windows.Forms.Button btDownload;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

