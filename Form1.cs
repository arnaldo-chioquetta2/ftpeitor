using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

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
            List<string> extensoesPermitidas = new List<string> { ".php", ".js", ".css" , ".html", ".apk" , ".jpg"};
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
                btDownload.Visible = true;
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
            btDownload.Visible = false;
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

        private async void btDownload_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            using (DownloadListForm downloadListForm = new DownloadListForm())
            {
                var result = downloadListForm.ShowDialog(this);
                if (result != DialogResult.OK)
                    return;

                try
                {
                    // Garante que credenciais/camLocal estejam atualizados
                    if (this.cFPT == null)
                        this.cFPT = new FTP();

                    Credenciais();

                    if (string.IsNullOrWhiteSpace(this.camLocal))
                    {
                        lbErro.Text = "Caminho Local (CamLocal) nao configurado.";
                        lbErro.Visible = true;
                        ExecutionLog.Write("Download abortado: CamLocal nao configurado.");
                        return;
                    }

                    var paths = downloadListForm.DownloadPaths;
                    if (paths == null || paths.Count == 0)
                    {
                        ExecutionLog.Write("Download cancelado: lista vazia.");
                        return;
                    }

                    int ok = 0;
                    int erros = 0;
                    var errosAcumulados = new StringBuilder();

                    lbErro.Visible = false;
                    Label1.Text = "Iniciando downloads...";
                    Label1.Refresh();

                    foreach (string remotoOriginal in paths)
                    {
                        string remoto = (remotoOriginal ?? string.Empty).Trim();
                        if (remoto.Length == 0)
                            continue;

                        string destinoLocal;
                        try
                        {
                            destinoLocal = MapearCaminhoRemotoParaLocalSeguro(remoto, this.camLocal);
                        }
                        catch (Exception ex)
                        {
                            erros++;
                            string msg = "Download ignorado (path invalido): '" + remoto + "' - " + ex.Message;
                            ExecutionLog.Write(msg);
                            errosAcumulados.AppendLine(msg);
                            continue;
                        }

                        try
                        {
                            Label1.Text = "Baixando: " + remoto;
                            Label1.Refresh();

                            await this.cFPT.DownloadFileAsync(remoto, destinoLocal);
                            ok++;
                            ExecutionLog.Write("Download OK: '" + remoto + "' -> '" + destinoLocal + "'");
                        }
                        catch (Exception ex)
                        {
                            erros++;
                            string erroFtp = (this.cFPT != null) ? this.cFPT.getErro() : "";
                            string msg = "Download ERRO: '" + remoto + "' -> '" + destinoLocal + "' - " + ex.Message + (string.IsNullOrWhiteSpace(erroFtp) ? "" : (" | FTP: " + erroFtp));
                            ExecutionLog.Write(msg);
                            errosAcumulados.AppendLine(msg);
                        }
                    }

                    Label1.Text = "Downloads concluidos. OK: " + ok + " / Erros: " + erros;
                    Label1.Refresh();

                    if (erros > 0)
                    {
                        lbErro.Text = "Concluido com " + erros + " erro(s).";
                        lbErro.Visible = true;

                        string texto = errosAcumulados.ToString().Trim();
                        if (!string.IsNullOrWhiteSpace(texto))
                            ShowErrorMessageOnUiThread(texto, "Erros no Download");
                    }
                    else
                    {
                        lbErro.Visible = false;
                    }
                }
                catch (Exception ex)
                {
                    ExecutionLog.Write("Falha geral no btDownload: " + ex.Message);
                    lbErro.Text = "Falha no download.";
                    lbErro.Visible = true;
                    ShowErrorMessageOnUiThread(ex.Message, "Erro no Download");
                }
            }
        }

        private static string MapearCaminhoRemotoParaLocalSeguro(string caminhoRemoto, string raizLocal)
        {
            if (string.IsNullOrWhiteSpace(raizLocal))
                throw new ArgumentException("Raiz local nao informada.", nameof(raizLocal));

            if (string.IsNullOrWhiteSpace(caminhoRemoto))
                throw new ArgumentException("Caminho remoto nao informado.", nameof(caminhoRemoto));

            string rel = caminhoRemoto.Trim();
            rel = rel.Replace('\\', '/');

            while (rel.StartsWith("./"))
                rel = rel.Substring(2);

            rel = rel.TrimStart('/');

            if (rel.Length == 0)
                throw new ArgumentException("Caminho remoto vazio apos normalizacao.");

            // Bloqueios básicos contra escape
            if (rel.Contains(".."))
                throw new InvalidOperationException("Path traversal ('..') nao permitido.");
            if (rel.Contains(":") || rel.StartsWith("//"))
                throw new InvalidOperationException("Caminho remoto com ':' ou UNC nao permitido.");

            rel = rel.Replace('/', Path.DirectorySeparatorChar);

            string raizFull = Path.GetFullPath(raizLocal);
            string destinoFull = Path.GetFullPath(Path.Combine(raizFull, rel));

            // Garante que destino está dentro da raiz
            if (!destinoFull.StartsWith(raizFull.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(destinoFull, raizFull, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Destino fora da raiz local configurada.");
            }

            return destinoFull;
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

        private async void ProcessarDownloadsAsync(System.Collections.Generic.List<string> remotePaths)
        {
            if (remotePaths == null || remotePaths.Count == 0)
                return;

            if (this.cFPT == null)
            {
                Inicializa();
            }

            if (string.IsNullOrWhiteSpace(this.camLocal))
            {
                // Sem prompts intrusivos por arquivo; aqui e um bloqueio de configuracao.
                lbErro.Text = "Caminho Local (raiz) nao configurado.";
                lbErro.Visible = true;
                ExecutionLog.Write("[Download] Abortado: CamLocal (raiz local) nao configurado.");
                return;
            }

            int ok = 0;
            int erro = 0;
            var errosAcumulados = new StringBuilder();

            lbErro.Visible = false;
            Label1.Text = "Iniciando download...";
            Label1.Refresh();

            string raiz = this.camLocal;
            ExecutionLog.Write("[Download] Inicio. Itens=" + remotePaths.Count + " RaizLocal='" + raiz + "'.");

            foreach (var raw in remotePaths)
            {
                string remoto = raw;
                try
                {
                    string local;
                    string motivo;
                    if (!TryMapRemoteToLocal(raiz, remoto, out local, out motivo))
                    {
                        erro++;
                        string msg = "[Download] Ignorado (path invalido). Remoto='" + (remoto ?? "") + "' Motivo='" + motivo + "'.";
                        ExecutionLog.Write(msg);
                        errosAcumulados.AppendLine(msg);
                        continue;
                    }

                    Label1.Text = "Baixando: " + remoto;
                    Label1.Refresh();

                    await this.cFPT.DownloadFileAsync(remoto, local);
                    ok++;
                    ExecutionLog.Write("[Download] OK. Remoto='" + remoto + "' Local='" + local + "'.");
                }
                catch (Exception ex)
                {
                    erro++;
                    string ftpErro = (this.cFPT != null) ? this.cFPT.getErro() : "";
                    string msg = "[Download] ERRO. Remoto='" + (remoto ?? "") + "' FTP='" + (ftpErro ?? "") + "' Ex='" + ex.Message + "'.";
                    ExecutionLog.Write(msg);
                    errosAcumulados.AppendLine(msg);
                }
            }

            string resumo = "Download concluido. OK=" + ok + " Erros=" + erro;
            Label1.Text = resumo;
            Label1.Refresh();

            if (erro > 0)
            {
                lbErro.Text = resumo;
                lbErro.Visible = true;

                string texto = errosAcumulados.ToString().Trim();
                if (!string.IsNullOrWhiteSpace(texto))
                    ShowErrorMessageOnUiThread(texto, "Erros no Download");
            }

            ExecutionLog.Write("[Download] Fim. " + resumo);
        }

        private bool TryMapRemoteToLocal(string raizLocal, string remotePath, out string localPath, out string reason)
        {
            localPath = null;
            reason = null;

            if (string.IsNullOrWhiteSpace(raizLocal))
            {
                reason = "raiz_local_vazia";
                return false;
            }

            string raizFull;
            try
            {
                raizFull = Path.GetFullPath(raizLocal);
            }
            catch
            {
                reason = "raiz_local_invalida";
                return false;
            }

            string rel = NormalizeRemotePath(remotePath);
            if (string.IsNullOrWhiteSpace(rel))
            {
                reason = "remote_vazio";
                return false;
            }

            // Bloqueios basicos antes de combinar
            if (rel.Contains(".."))
            {
                reason = "path_traversal";
                return false;
            }
            if (rel.IndexOf(':') >= 0)
            {
                reason = "drive_letter";
                return false;
            }
            if (rel.StartsWith("\\\\"))
            {
                reason = "unc";
                return false;
            }

            // Converter / para \ para caminho do Windows
            rel = rel.Replace('/', '\\');

            string combined = Path.Combine(raizFull, rel);
            string combinedFull;
            try
            {
                combinedFull = Path.GetFullPath(combined);
            }
            catch
            {
                reason = "path_local_invalido";
                return false;
            }

            if (!IsSubPathOf(combinedFull, raizFull))
            {
                reason = "fora_da_raiz";
                return false;
            }

            localPath = combinedFull;
            return true;
        }

        private string NormalizeRemotePath(string remotePath)
        {
            string p = (remotePath ?? "").Trim();
            if (p.Length == 0)
                return p;

            p = p.Replace('\\', '/');

            while (p.StartsWith("./"))
                p = p.Substring(2);

            if (p.StartsWith("/"))
                p = p.Substring(1);

            return p.Trim();
        }

        private bool IsSubPathOf(string candidatePath, string basePath)
        {
            if (string.IsNullOrEmpty(candidatePath) || string.IsNullOrEmpty(basePath))
                return false;

            string b = basePath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
            return candidatePath.StartsWith(b, StringComparison.OrdinalIgnoreCase);
        }

        private void ShowErrorMessageOnUiThread(string message, string title)
        {
            if (this.IsDisposed)
                return;

            Action show = () =>
            {
                if (!this.IsDisposed)
                    MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            if (this.InvokeRequired)
                this.BeginInvoke(show);
            else
                show();
        }
        #endregion

    }
}
