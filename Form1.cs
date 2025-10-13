using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace FTPc
{
    public partial class Tela : Form
    {
        private FileInfo ArqEsc;
        private INI MeuIni;
        private FTP cFPT;
        private DateTime UltData;
        private DateTime UltDt;
        private int PassoTimer = 0;
        private int Transferencias = 0;        
        private float TempoAtual = 2000;
        private string camLocal = "";
        private string PastaBaseFTP = "";
        private string host = ""; 
        private string ftpAtu = "";
        private string UltNome = "";

        private void btConfig_Click(object sender, EventArgs e)
        {
            this.Label1.Text = "";
            this.timer1.Enabled = false;
            Config FConfig = new Config();
            FConfig.ShowDialog();
            int numeroFtps = this.MeuIni.ReadInt("Config", "ftp_count", 0);
            if (numeroFtps == 0)
            {
                MessageBox.Show("Não foi configurado", "Não foi configurado");
            } else {
                this.ftpAtu = this.MeuIni.ReadString("Config", "ftpAtu", "1");
                string Nome = this.MeuIni.ReadString(this.ftpAtu, "nome", "");
                this.Text = "FTPeia " + Nome;
                Inicializa();
            }
        }

        private void Credenciais()
        {
            string user = MeuIni.ReadString(this.ftpAtu, "user", "");
            string pass = MeuIni.ReadString(this.ftpAtu, "pass", "");
            this.camLocal = MeuIni.ReadString(this.ftpAtu, "CamLocal", "");
            this.PastaBaseFTP = MeuIni.ReadString(this.ftpAtu, "PastaBaseFTP", "");
            int Porta = this.MeuIni.ReadInt(ftpAtu, "Porta", 21);
            this.cFPT.Credeciais(this.host, user, pass, Porta);
        }

        private void Inicializa()
        {
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.PassoTimer = 0;            
            this.cFPT = new FTP();
            Credenciais();
            this.cFPT.setBarra(ref ProgressBar1);
        }

        private void Tela_Shown(object sender, EventArgs e)
        {
            int TamVert = Screen.PrimaryScreen.Bounds.Height;
            this.Top = TamVert - 147;
            UltDt = new DateTime(2001, 1, 1);
            int numeroFtps = this.MeuIni.ReadInt("Config", "ftp_count", 0);
            if (numeroFtps == 0)
            {
                Config FConfig = new Config();
                FConfig.ShowDialog();
                this.host = MeuIni.ReadString("Config", "host", "");
                if (this.host.Length == 0)
                {
                    MessageBox.Show("Não foi configurado", "Configure");
                }
            }
            else
            {
                this.ftpAtu = this.MeuIni.ReadString("Config", "ftpAtu", "1");
                this.host = MeuIni.ReadString(this.ftpAtu, "host", "");
                timer1.Enabled = true;
            }
                
        }

        #region Inicialização

        public Tela()
        {
            InitializeComponent();
        }

        private void Tela_Load(object sender, EventArgs e)
        {
            MeuIni = new INI();
            this.ftpAtu = this.MeuIni.ReadString("Config", "ftpAtu", "1");
            string Nome = this.MeuIni.ReadString(this.ftpAtu, "nome", "");
            this.Text = "FTPeia " + Nome;
        }

        #endregion

        #region Obtem Arquivo a atualizar

        private void UltAtualizado(String Pasta)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(Pasta);
            BuscaArquivos(dirInfo);
        }

        private void BuscaArquivos(DirectoryInfo diret)
        {
            DirectoryInfo objDirectoryInfo = new System.IO.DirectoryInfo(diret.FullName);
            SearchFiles(objDirectoryInfo);
            SearchDirectories(objDirectoryInfo);
        }

        private void SearchDirectories(DirectoryInfo objDirectoryInfo)
        {
            foreach (DirectoryInfo DirectorioInfo in objDirectoryInfo.GetDirectories())
            {
                if ((DirectorioInfo.Exists == true) && (DirectorioInfo.Name != "System Volume Information") && (DirectorioInfo.Name != "RECYCLER"))
                {
                    SearchFiles(DirectorioInfo);
                    SearchDirectories(DirectorioInfo);
                }
            }
        }

        private void SearchFiles(DirectoryInfo info)
        {
            List<string> extensoesPermitidas = new List<string> { ".php", ".js", ".css" , ".html", ".apk" };
            FileInfo[] arquivos = info.GetFiles()
                .Where(arquivo => extensoesPermitidas.Contains(arquivo.Extension.ToLower()))
                .OrderByDescending(arquivo => arquivo.CreationTime)
                .ToArray();
            foreach (FileInfo arquivo in arquivos)
            {
                DateTime EssaData = arquivo.LastWriteTime;
                if (EssaData > UltDt)
                {
                    UltDt = EssaData;
                    ArqEsc = arquivo;
                }
            }
        }

        #endregion

        #region Operações do Usuário
        private bool Atualiza(bool forcado = false)
        {
            UltAtualizado(this.camLocal);
            this.Text = "Ftpeia : " + ArqEsc.Name;
            Label1.Text = ArqEsc.FullName;
            Console.WriteLine(ArqEsc.FullName);
            string ese = ArqEsc.FullName;
            DateTime DtGrv = ArqEsc.LastWriteTime;
            if ((this.UltNome != ese) || (this.UltData != DtGrv) || forcado)
            {
                this.UltNome = ese;
                this.UltData = DtGrv;
                int pos = ArqEsc.FullName.IndexOf(this.PastaBaseFTP.Replace("/", "\\"));
                string CamfTP = ArqEsc.FullName.Substring(pos);
                string NmArq = ArqEsc.Name;
                if (CamfTP.EndsWith(NmArq))
                {
                    CamfTP = CamfTP.Remove(CamfTP.Length - NmArq.Length);
                }
                if (this.cFPT.Upload(ese, CamfTP))
                {
                    lbErro.Visible = false;
                    // Gambiarra pra mostrar um Progress fake, na primeira vez, não sei pq não aparece o Progress na primeira faz
                    if (this.Transferencias == 0)
                        pictureBox1.Visible = true;

                    this.Transferencias++;
                    timer1.Enabled = true;
                    Console.WriteLine("Upload realizado");
                    ProgressBar1.Value = ProgressBar1.Maximum;
                    ProgressBar1.Refresh();
                    return true;
                }
                else
                {
                    string Erro = this.cFPT.getErro();
                    Console.WriteLine("Erro: " + Erro);
                    Console.WriteLine("ProgressBar1.Visible = false");
                    ProgressBar1.Visible = false;
                    lbErro.Text = Erro;
                    lbErro.Visible = true;
                    timer1.Enabled = false;
                    return false;
                }
            }
            else
            {
                ProgressBar1.Visible = false;
                btInicio.Text = "Enviar denovo";
                btInicio.Visible = true;
                btConfig.Visible = true;
                Label1.Text = Label1.Text+ " já enviado"; 
                timer1.Interval = (int)this.TempoAtual;
                timer1.Enabled = true;
                return false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.Transferencias == 0)
            {
                this.PassoTimer++;
                int Tmp = 6 - this.PassoTimer;
                if (Tmp == 0)
                {
                    this.Transferencias = 1;
                    Label1.Text = "";
                    this.ClicouInicio();
                }
                else
                {
                    Label1.Text = Tmp.ToString();
                }
            }
            else
            {
                this.PassoTimer++;
                switch (PassoTimer)
                {
                    case 1: // Desabilit
                        ProgressBar1.Value = 0;
                        break;
                    case 2: // Invisivel
                        ProgressBar1.Visible = false;

                        // Gambiarra pra mostrar um Progress fake, na primeira vez, não sei pq não aparece o Progress na primeira faz
                        pictureBox1.Visible = false;

                        break;
                    default:
                        timer1.Enabled = false;
                        this.WindowState = FormWindowState.Minimized;
                        this.PassoTimer = 0;
                        int Tmp = (int)(this.TempoAtual * (float).97);
                        if (Tmp > 100)
                        {
                            this.TempoAtual = Tmp;
                            timer1.Interval = Tmp;
                        }
                        break;
                }
            }
        }

        private void ClicouInicio()
        {
            btInicio.Visible = false;
            btConfig.Visible = false;
            this.Refresh();
            Inicializa();
            Atualiza(true);
            if (ArqEsc != null)
                Label1.Text = ArqEsc.FullName;
        }

        private void btInicio_Click(object sender, EventArgs e)
        {
            this.ClicouInicio();
        }

        private void Tela_Resize_1(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                // Gambiarra pra mostrar um Progress fake, na primeira vez, não sei pq não aparece o Progress na primeira faz
                pictureBox1.Visible = false;

                Label1.Text = "Procurando arquivo a atualizar";
                Label1.Refresh();
                Atualiza();
            }
        }
        #endregion

    }
}
