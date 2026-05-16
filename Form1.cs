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
        private const string LogVersao = "LOGSCAN_V5_20260421_0153";
        private FileInfo ArqEsc;
        private INI MeuIni;
        private FTP cFPT;
        private DateTime UltData;
        private DateTime UltDt;
        private int PastasVisitadas;
        private int ArquivosElegiveisEncontrados;
        private int ArquivosAnalisados;
        private int PassoTimer = 0;
        private int Transferencias = 0;        
        private float TempoAtual = 2000;
        private string camLocal = "";
        private string PastaBaseFTP = "";
        private string host = ""; 
        private string ftpAtu = "";
        private string UltNome = "";

        private void Log(string message)
        {
            ExecutionLog.Write(message);
        }

        private static string NormalizarCaminhoLocal(string caminho)
        {
            if (string.IsNullOrWhiteSpace(caminho))
                return "";

            return Path.GetFullPath(caminho).TrimEnd('\\');
        }

        private string MontarCaminhoRemoto(FileInfo arquivo)
        {
            string raizLocal = NormalizarCaminhoLocal(this.camLocal);
            string diretorioArquivo = arquivo.DirectoryName ?? "";
            string diretorioNormalizado = NormalizarCaminhoLocal(diretorioArquivo);
            string relativo = "";

            if (!string.IsNullOrEmpty(raizLocal) &&
                diretorioNormalizado.StartsWith(raizLocal, StringComparison.OrdinalIgnoreCase))
            {
                relativo = diretorioNormalizado.Substring(raizLocal.Length).TrimStart('\\');
            }

            string baseRemota = this.PastaBaseFTP ?? "";
            if (Path.IsPathRooted(baseRemota))
                baseRemota = "";

            baseRemota = baseRemota.Replace("\\", "/").Trim('/');
            relativo = relativo.Replace("\\", "/").Trim('/');

            if (baseRemota.Length > 0 && relativo.Length > 0)
                return "/" + baseRemota + "/" + relativo;

            if (baseRemota.Length > 0)
                return "/" + baseRemota;

            if (relativo.Length > 0)
                return "/" + relativo;

            return "/";
        }

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
            UltDt = DateTime.MinValue;
            ArqEsc = null;
            PastasVisitadas = 0;
            ArquivosElegiveisEncontrados = 0;
            ArquivosAnalisados = 0;
            DirectoryInfo dirInfo = new DirectoryInfo(Pasta);
            string msgInicio = "[" + LogVersao + "][UltAtualizado] Inicio da busca em: " + Pasta;
            Log(msgInicio);
            BuscaArquivos(dirInfo);
            string msgResumo = "[" + LogVersao + "][UltAtualizado] Resumo: PastasVisitadas=" + PastasVisitadas + " | ArquivosElegiveis=" + ArquivosElegiveisEncontrados + " | ArquivosAnalisados=" + ArquivosAnalisados;
            Log(msgResumo);
            if (ArqEsc != null)
            {
                string msgEscolhido = "[" + LogVersao + "][UltAtualizado] Arquivo escolhido: " + ArqEsc.FullName + " | LastWriteTime=" + ArqEsc.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
                Log(msgEscolhido);
            }
            else
            {
                string msgVazio = "[" + LogVersao + "][UltAtualizado] Nenhum arquivo elegivel encontrado.";
                Log(msgVazio);
            }
        }

        private void BuscaArquivos(DirectoryInfo diret)
        {
            DirectoryInfo objDirectoryInfo = new System.IO.DirectoryInfo(diret.FullName);
            string msgRaiz = "[" + LogVersao + "][BuscaArquivos] Pasta raiz: " + objDirectoryInfo.FullName;
            Log(msgRaiz);
            SearchFiles(objDirectoryInfo);
            SearchDirectories(objDirectoryInfo);
        }

        private void SearchDirectories(DirectoryInfo objDirectoryInfo)
        {
            PastasVisitadas++;
            DirectoryInfo[] diretorios;
            try
            {
                diretorios = objDirectoryInfo.GetDirectories();
            }
            catch (Exception ex)
            {
                Log("[" + LogVersao + "][SearchDirectories] Erro ao listar subpastas de " + objDirectoryInfo.FullName + ": " + ex.Message);
                return;
            }

            foreach (DirectoryInfo DirectorioInfo in diretorios)
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
            List<string> extensoesPermitidas = new List<string> { ".php", ".js", ".css" , ".html", ".apk" , ".jpg"};
            FileInfo[] todosArquivos;
            try
            {
                todosArquivos = info.GetFiles();
            }
            catch (Exception ex)
            {
                //Log("[" + LogVersao + "][SearchFiles] Erro ao listar arquivos de " + info.FullName + ": " + ex.Message);
                return;
            }

            FileInfo[] arquivos = todosArquivos
                .Where(arquivo => extensoesPermitidas.Contains(arquivo.Extension.ToLower()))
                .OrderByDescending(arquivo => arquivo.LastWriteTime)
                .ToArray();

            ArquivosElegiveisEncontrados += arquivos.Length;
            if (arquivos.Length > 0)
            {
                //Log("[" + LogVersao + "][SearchFiles] " + info.FullName + " | Total arquivos=" + todosArquivos.Length + " | Elegiveis=" + arquivos.Length + " | Mais recente na pasta=" + arquivos[0].Name + " | LastWriteTime=" + arquivos[0].LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"));
            }

            foreach (FileInfo arquivo in arquivos)
            {
                ArquivosAnalisados++;
                DateTime EssaData = arquivo.LastWriteTime;
                if (EssaData > UltDt)
                {
                    UltDt = EssaData;
                    ArqEsc = arquivo;
                    //Log("[" + LogVersao + "][SearchFiles] Novo escolhido: " + arquivo.FullName + " | LastWriteTime=" + arquivo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"));
                }
            }
        }

        #endregion

        #region Operações do Usuário
        private bool Atualiza(bool forcado = false)
        {
            UltAtualizado(this.camLocal);
            Log("[" + LogVersao + "][Atualiza] Resultado apos busca: " + (ArqEsc != null ? ArqEsc.FullName : "<null>"));
            if (ArqEsc == null)
            {
                Log("[" + LogVersao + "][Atualiza] Nenhum arquivo encontrado para upload.");
                return false;
            }
            this.Text = "Ftpeia : " + ArqEsc.Name;
            Label1.Text = ArqEsc.FullName;
            Console.WriteLine(ArqEsc.FullName);
            string ese = ArqEsc.FullName;
            DateTime DtGrv = ArqEsc.LastWriteTime;
            if ((this.UltNome != ese) || (this.UltData != DtGrv) || forcado)
            {
                this.UltNome = ese;
                this.UltData = DtGrv;
                string CamfTP = MontarCaminhoRemoto(ArqEsc);
                Log("[" + LogVersao + "][Atualiza] Caminho remoto calculado: " + CamfTP);
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
