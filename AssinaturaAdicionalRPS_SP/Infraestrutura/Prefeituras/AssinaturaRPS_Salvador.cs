using System;
using System.Runtime.InteropServices;

namespace MXM.Assinatura.Infraestrutura.Prefeituras
{
    [Guid("5F8FDE21-D0A3-46BD-8D6A-5F234572A53A")]
    public class AssinaturaRPS_Salvador : AssinaRPS_TemplateMethod
    {
        private string XML;

        public AssinaturaRPS_Salvador(String aXML)
        {
            this.XML = aXML;
        }

        protected override string ExecutarProcessoEspecifico()
        {
            String retorno = String.Empty;
            try
            {
                retorno = AssinarXml(XML, "Rps", "InfRps", true);
                retorno = AssinarXml(retorno, "EnviarLoteRpsEnvio", "LoteRps", true);
            }
            catch (Exception erro)
            {
                AddMensagem("Ocorreu erro - ao assinar o XML: " + erro.Message);
            }

            return retorno;
        }

        protected override bool IsDadosValidos()
        {
            Boolean retorno = true;
            if (String.IsNullOrEmpty(XML))
            {
                AddMensagem("Ocorreu erro - XML Vazio");
                retorno = false;
            }

            return retorno;
        }
    }
}