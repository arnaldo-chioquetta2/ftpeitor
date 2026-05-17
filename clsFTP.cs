using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FTPc
{
    public class FTP
    {
        private int _tamanhoConteudo = 0;
        private int Tot = 0;
        string ftpIPServidor = "";
        string ftpUsuarioID = "";
        string ftpSenha = "";
        private int Porta;
        private string Erro = "";
        private ProgressBar ProgressBar1= null;

        public int tamanhoConteudo
        {
            get
            {
                return _tamanhoConteudo;
            }
            set
            {
                _tamanhoConteudo = value;
                Tot += value;
                this.ProgressBar1.Value = Tot;
                //Console.WriteLine("ProgressBar1.Value = " + Tot.ToString());                
            }
        }

        public FTP()
        {

        }

        public void Credeciais(string ftpIPServidor, string ftpUsuarioID, string ftpSenha, int porta = 21)
        {
            this.ftpIPServidor = ftpIPServidor ?? "";
            this.ftpUsuarioID = ftpUsuarioID;
            this.ftpSenha = ftpSenha;
            this.Porta = porta;
        }

        private Uri CriarUriFtp(string caminhoRemoto)
        {
            string servidor = (this.ftpIPServidor ?? "").Trim();
            if (servidor.Length == 0)
                throw new InvalidOperationException("Servidor FTP não configurado.");

            if (!servidor.Contains("://"))
                servidor = "ftp://" + servidor;

            Uri baseUri = new Uri(servidor);
            UriBuilder builder = new UriBuilder(baseUri);

            if (builder.Scheme.Length == 0)
                builder.Scheme = Uri.UriSchemeFtp;

            if (builder.Port <= 0)
                builder.Port = this.Porta;

            string caminho = (caminhoRemoto ?? "").Replace("\\", "/").Trim();
            if (caminho.Length == 0)
                caminho = "/";
            if (!caminho.StartsWith("/"))
                caminho = "/" + caminho;

            builder.Path = caminho;
            return builder.Uri;
        }

        public bool Upload(string _nomeArquivo, string Caminho)
        {
            this.Tot = 0;
            this.Erro = "";

            string Cam = Caminho.Replace(@"\", @"/").Trim('/');
            FileInfo _arquivoInfo = new FileInfo(_nomeArquivo);

            // ✅ Garantir que o diretório remoto exista ANTES do upload
            if (!CriarDiretorioRecursivo(Cam))
            {
                this.Erro = "Falha ao criar diretório no FTP: " + Cam;
                return false;
            }

            Uri uriUpload = CriarUriFtp(Cam + "/" + _arquivoInfo.Name);
            RemoverArquivoSeExistir(Cam + "/" + _arquivoInfo.Name);

            FtpWebRequest requisicaoFTP;
            requisicaoFTP = (FtpWebRequest)FtpWebRequest.Create(uriUpload);
            requisicaoFTP.Credentials = new NetworkCredential(this.ftpUsuarioID, this.ftpSenha);
            requisicaoFTP.KeepAlive = false;
            requisicaoFTP.Method = WebRequestMethods.Ftp.UploadFile;
            requisicaoFTP.UsePassive = true;
            requisicaoFTP.UseBinary = true;
            requisicaoFTP.ContentLength = _arquivoInfo.Length;
            this.ProgressBar1.Visible = true;
            Console.WriteLine("ProgressBar1.Visible = true");
            this.ProgressBar1.Maximum = (int)_arquivoInfo.Length;
            Console.WriteLine("ProgressBar1.Maximum = "+ this.ProgressBar1.Maximum.ToString());
            this.ProgressBar1.Enabled = true;
            FileStream fs = _arquivoInfo.OpenRead();
            bool sair = false;
            bool bReturn = false;
            while (sair==false) {
                string ret = this.UploadEmSi(requisicaoFTP, fs);
                if (ret=="")
                {
                    bReturn = true;
                    sair = true;
                } else
                {
                    if (ret.IndexOf("553") > 0)
                    {
                        FtpWebRequest requestCD = (FtpWebRequest)FtpWebRequest.Create(CriarUriFtp(Cam));
                        requestCD.Credentials = new NetworkCredential(this.ftpUsuarioID, this.ftpSenha);
                        requestCD.KeepAlive = false;
                        requestCD.Method = WebRequestMethods.Ftp.MakeDirectory;
                        requestCD.UsePassive = true;
                        try
                        {
                            using (var resp = (FtpWebResponse)requestCD.GetResponse())
                            {
                                Console.WriteLine(resp.StatusCode);
                            }
                        }
                        catch (Exception ex)
                        {
                            this.Erro = ex.Message;
                            MessageBox.Show("Não foi possivel enviar arquivo", "É necessário criar o diretório");
                            bReturn = false;
                            sair = true;
                        }
                    }
                    else
                    {
                        this.Erro = ret;
                        MessageBox.Show(ret, "Erro não tratado");
                        bReturn = false;
                        sair = true;
                    }
                } 
            }
            return bReturn;
        }

        public bool Download(string caminhoRemoto, string pastaLocalBase)
        {
            this.Erro = "";

            try
            {
                string remoto = (caminhoRemoto ?? "").Replace("\\", "/").Trim();
                if (remoto.Length == 0)
                {
                    this.Erro = "Caminho remoto vazio.";
                    return false;
                }

                string relativo = remoto.TrimStart('/');
                string local = Path.Combine(pastaLocalBase, relativo.Replace("/", "\\"));
                string diretorioLocal = Path.GetDirectoryName(local);
                if (!string.IsNullOrEmpty(diretorioLocal))
                    Directory.CreateDirectory(diretorioLocal);

                FtpWebRequest req = (FtpWebRequest)WebRequest.Create(CriarUriFtp(remoto));
                req.Credentials = new NetworkCredential(this.ftpUsuarioID, this.ftpSenha);
                req.Method = WebRequestMethods.Ftp.DownloadFile;
                req.KeepAlive = false;
                req.UsePassive = true;
                req.UseBinary = true;

                using (FtpWebResponse response = (FtpWebResponse)req.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (FileStream fileStream = new FileStream(local, FileMode.Create, FileAccess.Write))
                {
                    responseStream.CopyTo(fileStream);
                }

                return true;
            }
            catch (WebException ex)
            {
                var response = ex.Response as FtpWebResponse;
                if (response != null)
                    this.Erro = response.StatusDescription;
                else
                    this.Erro = ex.Message;

                return false;
            }
            catch (Exception ex)
            {
                this.Erro = ex.Message;
                return false;
            }
        }

        private void RemoverArquivoSeExistir(string caminhoArquivo)
        {
            try
            {
                FtpWebRequest req = (FtpWebRequest)WebRequest.Create(CriarUriFtp(caminhoArquivo));
                req.Credentials = new NetworkCredential(this.ftpUsuarioID, this.ftpSenha);
                req.Method = WebRequestMethods.Ftp.DeleteFile;
                req.KeepAlive = false;
                req.UsePassive = true;
                using (var resp = (FtpWebResponse)req.GetResponse())
                {
                }
            }
            catch (WebException ex)
            {
                var response = ex.Response as FtpWebResponse;
                if (response == null || response.StatusCode != FtpStatusCode.ActionNotTakenFileUnavailable)
                    this.Erro = ex.Message;
            }
        }

        private bool CriarDiretorioRecursivo(string caminhoRemoto)
        {
            // Normaliza caminho: remove barras extras e converte para Unix-style
            caminhoRemoto = caminhoRemoto.Trim('/').Replace("\\", "/");
            string[] niveis = caminhoRemoto.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            string caminhoAtual = "";

            for (int i = 0; i < niveis.Length; i++)
            {
                string nomeProximo = niveis[i];
                string caminhoPai = caminhoAtual; // Diretório pai (vazio = raiz)
                caminhoAtual += "/" + nomeProximo;

                // ✅ Verifica se o diretório já existe SEM lançar exceção
                if (!DiretorioExiste(caminhoPai, nomeProximo))
                {
                    // ❌ Não existe → cria
                    if (!CriarDiretorioUnico(caminhoAtual))
                    {
                        this.Erro = $"Falha ao criar diretório: {caminhoAtual}";
                        return false;
                    }
                    Console.WriteLine($"✓ Criado: {caminhoAtual}");
                }
                else
                {
                    // ✅ Já existe → apenas avança
                    Console.WriteLine($"→ Existente: {caminhoAtual}");
                }
            }
            return true;
        }

        /// <summary>
        /// Verifica se um subdiretório existe dentro de um diretório pai
        /// usando ListDirectory (sem lançar exceções para decisão lógica)
        /// </summary>
        private bool DiretorioExiste(string caminhoPai, string nomeDiretorio)
        {
            try
            {
                // Monta URL do diretório PAI para listar seu conteúdo
                FtpWebRequest req = (FtpWebRequest)WebRequest.Create(CriarUriFtp(caminhoPai));
                req.Credentials = new NetworkCredential(this.ftpUsuarioID, this.ftpSenha);
                req.Method = WebRequestMethods.Ftp.ListDirectory; // LIST command
                req.KeepAlive = false;
                req.UsePassive = true; // Melhor compatibilidade com firewalls

                using (var resp = (FtpWebResponse)req.GetResponse())
                using (var stream = resp.GetResponseStream())
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string linha;
                    while ((linha = reader.ReadLine()) != null)
                    {
                        // Normaliza: remove whitespace e compara case-insensitive
                        if (linha.Trim().Equals(nomeDiretorio, StringComparison.OrdinalIgnoreCase))
                            return true;
                    }
                    return false; // Não encontrou na lista
                }
            }
            catch (WebException ex)
            {
                // ⚠️ Só tratamos exceções reais (conexão, autenticação)
                // Diretórios inexistentes NÃO devem chegar aqui — ListDirectory retorna lista vazia ou erro 550
                var response = ex.Response as FtpWebResponse;
                if (response != null && response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    // 550 = diretório pai não existe → filho também não existe
                    return false;
                }
                // Outros erros = falha real
                this.Erro = $"Erro ao listar diretório '{caminhoPai}': {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Cria um único diretório (não recursivo)
        /// </summary>
        private bool CriarDiretorioUnico(string caminhoCompleto)
        {
            try
            {
                FtpWebRequest req = (FtpWebRequest)WebRequest.Create(CriarUriFtp(caminhoCompleto));
                req.Credentials = new NetworkCredential(this.ftpUsuarioID, this.ftpSenha);
                req.Method = WebRequestMethods.Ftp.MakeDirectory;
                req.KeepAlive = false;
                req.UsePassive = true;

                using (var resp = (FtpWebResponse)req.GetResponse())
                {
                    return resp.StatusCode == FtpStatusCode.PathnameCreated ||
                           resp.StatusCode == FtpStatusCode.CommandOK;
                }
            }
            catch (WebException ex)
            {
                var response = ex.Response as FtpWebResponse;
                if (response != null &&
                    (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable || // 550
                     response.StatusCode == FtpStatusCode.ActionNotTakenFilenameNotAllowed)) // 553
                {
                    // Diretório já existe (alguns servidores retornam erro em vez de sucesso)
                    return true;
                }
                this.Erro = $"Falha ao criar '{caminhoCompleto}': {ex.Message}";
                return false;
            }
        }

        private string UploadEmSi(FtpWebRequest requisicaoFTP, FileStream fs)
        {
            try
            {
                // Stream  para o qual o arquivo a ser enviado será escrito
                Stream strm = requisicaoFTP.GetRequestStream();

                int buffLength = 2048;
                byte[] buff = new byte[buffLength];

                // Lê a partir do arquivo stream, 2k por vez
                this.tamanhoConteudo = fs.Read(buff, 0, buffLength);

                // ate o conteudo do stream terminar
                while (this.tamanhoConteudo != 0)
                {
                    // Escreve o conteudo a partir do arquivo para o stream FTP 
                    strm.Write(buff, 0, this.tamanhoConteudo);
                    this.tamanhoConteudo = fs.Read(buff, 0, buffLength);
                }

                // Fecha o stream a requisição
                strm.Close();
                fs.Close();
                using (var response = (FtpWebResponse)requisicaoFTP.GetResponse())
                {
                    this.Erro = response.StatusDescription;
                }
                return "";
            }
            catch (WebException ex)
            {
                var response = ex.Response as FtpWebResponse;
                if (response != null)
                    return response.StatusDescription;

                return ex.Message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public bool Testa()
        {
            string StringTeste = "Teste do FtpTeitor";
            Uri uriTeste = CriarUriFtp("/Teste.tst");
            FtpWebRequest requisicaoFTP;
            requisicaoFTP = (FtpWebRequest)FtpWebRequest.Create(uriTeste);
            requisicaoFTP.Credentials = new NetworkCredential(this.ftpUsuarioID, this.ftpSenha);            
            requisicaoFTP.KeepAlive = false;
            requisicaoFTP.Method = WebRequestMethods.Ftp.UploadFile;
            requisicaoFTP.UsePassive = true;
            requisicaoFTP.UseBinary = true;
            requisicaoFTP.ContentLength = 9;
            //int buffLength = 2048;
            byte[] buff = Encoding.ASCII.GetBytes(StringTeste);
            bool ret = false;
            try
            {
                Stream strm = requisicaoFTP.GetRequestStream();
                strm.Write(buff, 0, StringTeste.Length);                
                using (var respUp = (FtpWebResponse)requisicaoFTP.GetResponse())
                {
                }
                FtpWebRequest redDown = (FtpWebRequest)WebRequest.Create(uriTeste);
                redDown.Method = WebRequestMethods.Ftp.DownloadFile;
                redDown.Credentials = new NetworkCredential(this.ftpUsuarioID, this.ftpSenha);
                redDown.UsePassive = true;
                FtpWebResponse respDown = (FtpWebResponse)redDown.GetResponse();
                Stream responseStream = respDown.GetResponseStream();
                StreamReader readerD = new StreamReader(responseStream);
                string resposta = readerD.ReadToEnd();
                strm.Close();
                readerD.Close();
                respDown.Close();
                ret = true;
            }
            catch (Exception ex)
            {
                ret= false;
            }
            if (ret)
            {
                // Deleção do arquivo de testes, se der erro na deleção ainda assim a conexão é valida, porque será utilizado para upload
                FtpWebRequest redDel = (FtpWebRequest)WebRequest.Create(uriTeste);
                redDel.Method = WebRequestMethods.Ftp.DeleteFile;
                redDel.Credentials = new NetworkCredential(this.ftpUsuarioID, this.ftpSenha);
                redDel.UsePassive = true;
                FtpWebResponse response = (FtpWebResponse)redDel.GetResponse();
                response.Close();
            }
            return ret;
        }
        public string getErro()
        {
            return this.Erro;
        }

        public void setBarra(ref ProgressBar ProgressBar1)
        {
            this.ProgressBar1 = ProgressBar1;
            Console.WriteLine("this.ProgressBar1 = ProgressBar1");
        }

        public async Task DownloadFileAsync(string caminhoRemoto, string caminhoLocal)
        {
            if (string.IsNullOrWhiteSpace(caminhoRemoto))
                throw new ArgumentException("Caminho remoto nao informado.", nameof(caminhoRemoto));
            if (string.IsNullOrWhiteSpace(caminhoLocal))
                throw new ArgumentException("Caminho local nao informado.", nameof(caminhoLocal));

            this.Erro = "";

            string dirLocal = Path.GetDirectoryName(caminhoLocal);
            if (!string.IsNullOrEmpty(dirLocal))
                Directory.CreateDirectory(dirLocal);

            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(CriarUriFtp(caminhoRemoto));
            req.Credentials = new NetworkCredential(this.ftpUsuarioID, this.ftpSenha);
            req.Method = WebRequestMethods.Ftp.DownloadFile;
            req.KeepAlive = false;
            req.UsePassive = true;
            req.UseBinary = true;

            try
            {
                using (var resp = (FtpWebResponse)await req.GetResponseAsync().ConfigureAwait(false))
                using (var responseStream = resp.GetResponseStream())
                using (var fs = new FileStream(caminhoLocal, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    if (responseStream != null)
                        await responseStream.CopyToAsync(fs).ConfigureAwait(false);
                }
            }
            catch (WebException ex)
            {
                var response = ex.Response as FtpWebResponse;
                if (response != null)
                    this.Erro = response.StatusDescription;
                else
                    this.Erro = ex.Message;

                throw;
            }
            catch (Exception ex)
            {
                this.Erro = ex.Message;
                throw;
            }
        }

    }
}


