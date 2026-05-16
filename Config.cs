using System;
using System.IO;
using System.Windows.Forms;

namespace FTPc
{
    public partial class Config : Form
    {
        private INI MeuIni;
        private int iftpAtu = 0;
        private int numeroFtps = 0;
        private bool carregando = false;

        private void button1_Click(object sender, EventArgs e)
        {
            if (cbFTP.SelectedIndex == -1)
            {
                iftpAtu = numeroFtps + 1;
                iftpAtu = numeroFtps;
                numeroFtps++;
                iftpAtu = numeroFtps;
                this.MeuIni.WriteInt("Config", "ftp_count", iftpAtu);
                this.MeuIni.WriteInt("Config", "ftpAtu", iftpAtu);
                this.MeuIni.WriteString(iftpAtu.ToString(), "nome", cbFTP.Text);
                this.MeuIni.WriteInt("Config", "ftpAtu", iftpAtu);
                this.MeuIni.WriteInt("Config", "ftp_count", numeroFtps);
            }
            this.MeuIni.WriteInt("Config", "ftpAtu", iftpAtu);
            this.MeuIni.WriteString(iftpAtu.ToString(), "host", txHost.Text);
            this.MeuIni.WriteString(iftpAtu.ToString(), "user", txUser.Text);
            this.MeuIni.WriteString(iftpAtu.ToString(), "pass", txSenha.Text);
            this.MeuIni.WriteString(iftpAtu.ToString(), "CamLocal", txLocal.Text);
            this.MeuIni.WriteString(iftpAtu.ToString(), "PastaBaseFTP", txCamFTP.Text);
            this.MeuIni.WriteString(iftpAtu.ToString(), "Porta", txPorta.Text);
            this.MeuIni.WriteBool("Config", "LogAtivo", chkLog.Checked);
            ExecutionLog.SetEnabled(chkLog.Checked);
            Close();
        }

        private void cbFTP_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!carregando)
            {
                iftpAtu = cbFTP.SelectedIndex + 1;
                string ftpAtu = iftpAtu.ToString();
                txHost.Text = this.MeuIni.ReadString(ftpAtu, "host", "");
                txUser.Text = this.MeuIni.ReadString(ftpAtu, "user", "");
                txSenha.Text = this.MeuIni.ReadString(ftpAtu, "pass", "");
                txLocal.Text = this.MeuIni.ReadString(ftpAtu, "CamLocal", "");
                txCamFTP.Text = this.MeuIni.ReadString(ftpAtu, "PastaBaseFTP", "");
                txPorta.Text = this.MeuIni.ReadString(ftpAtu, "Porta", "21");                        
            }
        }

        private void Config_Load(object sender, EventArgs e)
        {
            carregando = true;
            this.MeuIni = new INI();
            numeroFtps = this.MeuIni.ReadInt("Config", "ftp_count", 0);
            for (int i = 1; i <= numeroFtps; i++)
            {
                string sftp = this.MeuIni.ReadString(i.ToString(), "nome", "");
                cbFTP.Items.Add(sftp);
            }
            if (numeroFtps > 0)
            {
                iftpAtu = this.MeuIni.ReadInt("Config", "ftpAtu", 0);
                cbFTP.SelectedIndex = iftpAtu - 1;
                string ftpAtu = iftpAtu.ToString();
                txHost.Text = this.MeuIni.ReadString(ftpAtu, "host", "");
                txUser.Text = this.MeuIni.ReadString(ftpAtu, "user", "");
                txSenha.Text = this.MeuIni.ReadString(ftpAtu, "pass", "");
                txLocal.Text = this.MeuIni.ReadString(ftpAtu, "CamLocal", "");
                txCamFTP.Text = this.MeuIni.ReadString(ftpAtu, "PastaBaseFTP", "");
                txPorta.Text = this.MeuIni.ReadString(ftpAtu, "Porta", "21");
            }
            chkLog.Checked = this.MeuIni.ReadBool("Config", "LogAtivo", false);
            carregando = false; ;
        }

        private void btNovo_Click(object sender, EventArgs e)
        {
            txHost.Clear();
            txUser.Clear();
            txSenha.Clear();
            txLocal.Clear();
            txCamFTP.Clear();
            txPorta.Clear();
            cbFTP.Text = "";
            cbFTP.Focus();
        }

        public Config()
        {
            InitializeComponent();
        }

        private void btTeste_Click(object sender, EventArgs e)
        {
            btTeste.Enabled = false;            
            FTP cFPT;
            int Porta = Convert.ToInt32(txPorta.Text);
            cFPT = new FTP();
            cFPT.Credeciais(txHost.Text, txUser.Text, txSenha.Text, Porta);
            if (cFPT.Testa())
                MessageBox.Show("Teste realizado com sucesso", "Credenciais válidas");
            else
                MessageBox.Show("Impossível conectar", "Erro na configuração");
            btTeste.Enabled = true;
        }

        private void btLog_Click(object sender, EventArgs e)
        {
            string logPath = ExecutionLog.CurrentLogPath;
            if (string.IsNullOrEmpty(logPath) || !File.Exists(logPath))
            {
                MessageBox.Show("Nenhum log da execucao atual foi encontrado.", "Log indisponivel");
                return;
            }

            if (!ExecutionLog.OpenCurrentLog())
                MessageBox.Show("Nao foi possivel abrir o log atual.", "Erro ao abrir log");
        }

    }
}
