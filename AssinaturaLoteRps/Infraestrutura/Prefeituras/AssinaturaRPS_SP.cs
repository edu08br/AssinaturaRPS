using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace MXM.Assinatura.Infraestrutura.Prefeituras
{
    public class AssinaturaRPS_SP : AssinaRPS_TemplateMethod
    {
        private string sAssinatura;

        public AssinaturaRPS_SP(string sAssinatura)
        {
            this.sAssinatura = sAssinatura;
        }

        protected override bool IsDadosValidos()
        {
            if (string.IsNullOrWhiteSpace(sAssinatura))
            {
                AddMensagem("CODERRO0 - Informações de RPS não informada para assinatura.");
            }

            return Mensagens.Count == 0;
        }

        protected override string ExecutarProcessoEspecifico()
        {
            string retorno = String.Empty;
            try
            {
                //recebe o certificado e a string a ser assinada
                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();

                //cria o array de bytes e realiza a conversao da string em array de bytes
                byte[] sAssinaturaByte = enc.GetBytes(sAssinatura);

                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

                //pega a chave privada do certificado digital
                rsa = certificado.PrivateKey as RSACryptoServiceProvider;

                RSAPKCS1SignatureFormatter rsaf = new RSAPKCS1SignatureFormatter(rsa);
                SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();

                //cria a variavel hash que armazena o resultado do sha1
                byte[] hash;
                hash = sha1.ComputeHash(sAssinaturaByte);

                //definimos o metodo a ser utilizado na criptografia e assinamos
                rsaf.SetHashAlgorithm("SHA1");
                sAssinaturaByte = rsaf.CreateSignature(hash);

                //por fim fazemos a conversao do array de bytes para string
                retorno = Convert.ToBase64String(sAssinaturaByte);
            }
            catch (Exception ex)
            {
                AddMensagem(ex.Message);
            }
            return retorno;
        }
    }
}