using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.ServiceModel;
using System.Xml;

namespace MXM.Assinatura.Infraestrutura.Prefeituras
{
    public class AssinaturaRPS_Salvador : AssinaRPS_TemplateMethod
    {
        private string XML;

        public AssinaturaRPS_Salvador(String aXML)
        {
            this.XML = aXML;
        }

        //[PermissionSetAttribute(SecurityAction.PermitOnly, Name = "FullTrust")]
        //[OperationBehavior(Impersonation = ImpersonationOption.Required)]
        protected override string ExecutarProcessoEspecifico()
        {
            String retorno = String.Empty;

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = false;
                doc.LoadXml(XML);

                //try
                //{
                //    retorno = AssinarXml(doc, "Rps", "InfRps", true);
                //}
                //catch
                //{
                //    Mensagens.Clear();
                //    throw;
                //}

                retorno = AssinarXml(doc, "Rps", "InfRps", true);
                retorno = AssinarXml(doc, "Rps", "InfRps", true);
                retorno = AssinarXml(doc, "EnviarLoteRpsEnvio", "LoteRps", true);

                retorno = retorno.Replace("<", "&lt;");
                retorno = retorno.Replace(">", "&gt;");
            }
            catch (Exception erro)
            {
                AddMensagem("Ocorreu erro na assinatura do XML: " + erro.ToString());
            }
            
            return retorno;
        }

        protected override bool IsDadosValidos()
        {
            Boolean retorno = true;
            if (String.IsNullOrEmpty(XML))
            {
                AddMensagem("Ocorreu erro com XML Vazio");
                retorno = false;
            }

            return retorno;
        }
    }
}