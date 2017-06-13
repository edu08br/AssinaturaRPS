using System;
using System.Security.Cryptography;
using System.Text;

namespace MXM.Assinaturas.Infraestrutura.Prefeituras
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
            if (String.IsNullOrEmpty(sAssinatura))
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
                RSACryptoServiceProvider rsa = certificado.PrivateKey as RSACryptoServiceProvider;

                RSAPKCS1SignatureFormatter rsaf = new RSAPKCS1SignatureFormatter(rsa);
                SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();

                byte[] sAssinaturaByte = new ASCIIEncoding().GetBytes(sAssinatura);
                byte[] hash = sha1.ComputeHash(sAssinaturaByte);

                rsaf.SetHashAlgorithm("SHA1");
                sAssinaturaByte = rsaf.CreateSignature(hash);

                retorno = Convert.ToBase64String(sAssinaturaByte);
                //Substituir para codigo abaixo, porém adaptado para framework 2.0
                //string.Join("", hash.Select(b => b.ToString("x2")).ToArray());
            }
            catch (Exception ex)
            {
                AddMensagem(ex.Message);
            }
            return retorno;
        }
    }
}