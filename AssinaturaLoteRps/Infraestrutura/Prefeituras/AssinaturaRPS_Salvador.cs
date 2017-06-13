using System;

namespace MXM.Assinaturas.Infraestrutura.Prefeituras
{
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