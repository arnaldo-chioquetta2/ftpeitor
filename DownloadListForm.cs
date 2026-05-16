using System;
using System.Windows.Forms;
using System.Linq;

namespace FTPc
{
    public partial class DownloadListForm : Form
    {
        private RichTextBox rtbDownloadPaths; // Declarar como campo

        public DownloadListForm()
        {
            InitializeComponent();
            this.Text = "Lista de Arquivos para Download";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new System.Drawing.Size(500, 400);

            // RichTextBox para a lista de arquivos
            rtbDownloadPaths = new RichTextBox(); // Instanciar o campo
            rtbDownloadPaths.Multiline = true;
            rtbDownloadPaths.ScrollBars = RichTextBoxScrollBars.Both; // Alterado para RichTextBoxScrollBars
            //rtbDownloadPaths.AcceptsReturn = true;
            rtbDownloadPaths.Dock = DockStyle.Fill;
            rtbDownloadPaths.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            rtbDownloadPaths.Name = "rtbDownloadPaths"; // Renomeado
            rtbDownloadPaths.Tag = "Lista de arquivos remotos, um por linha.";
            this.Controls.Add(rtbDownloadPaths);

            // Painel para os botões na parte inferior
            Panel pnlButtons = new Panel();
            pnlButtons.Dock = DockStyle.Bottom;
            pnlButtons.Height = 40;
            this.Controls.Add(pnlButtons);

            // Botão Baixar
            Button btnBaixar = new Button();
            btnBaixar.Text = "Baixar";
            btnBaixar.DialogResult = DialogResult.OK;
            btnBaixar.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
            btnBaixar.Location = new System.Drawing.Point(this.Width - 200, 8);
            btnBaixar.Click += btnBaixar_Click; // Adicionar manipulador de eventos
            pnlButtons.Controls.Add(btnBaixar);

            // Botão Cancelar
            Button btnCancel = new Button();
            btnCancel.Text = "Cancelar";
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
            btnCancel.Location = new System.Drawing.Point(this.Width - 110, 8);
            pnlButtons.Controls.Add(btnCancel);

            // Ajustar o Anchor dos botões com o redimensionamento do form
            this.Resize += (sender, e) =>
            {
                btnBaixar.Location = new System.Drawing.Point(this.ClientSize.Width - 200, 8);
                btnCancel.Location = new System.Drawing.Point(this.ClientSize.Width - 110, 8);
            };
        }

        /// <summary>
        /// Obtém a lista de caminhos de arquivos para download, um por linha, ignorando linhas vazias.
        /// </summary>
        public System.Collections.Generic.List<string> DownloadPaths
        {
            get
            {
                if (rtbDownloadPaths == null)
                {
                    return new System.Collections.Generic.List<string>();
                }
                // Usar System.Linq para Where e ToList
                return rtbDownloadPaths.Lines
                                       .Where(line => !string.IsNullOrWhiteSpace(line))
                                       .ToList();
            }
        }

        // Manipulador de eventos para o botão Baixar - será implementado na Etapa 4
        private void btnBaixar_Click(object sender, EventArgs e)
        {
            // Lógica de download será implementada aqui na Etapa 4
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            //
            // DownloadListForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 361);
            this.Name = "DownloadListForm";
            this.Text = "DownloadListForm";
            this.ResumeLayout(false);

        }
    }
}
