using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.Xml.Serialization;

namespace MXM.Infraestrutura.Prefeituras
{
    [Guid("5F8FDE21-D0A3-46BD-8D6A-5F234572A53A")]
    public class AssinaturaRPS_Salvador : AssinaRPS_TemplateMethod
    {
        private string XML;
        private EnviarLoteRpsEnvio enviarLoteRPSEnvio;

        public AssinaturaRPS_Salvador(String aXML)
        {
            this.XML = aXML;
        }

        protected override string ExecutarProcessoEspecifico()
        {
            String retorno = String.Empty;
            try
            {
                foreach (var rps in enviarLoteRPSEnvio.LoteRps.ListaRps)
                {
                    SignatureType signatureTypeRps = null;

                    String infRpsXml = ConverterDataBindEmStringXml<tcInfRps>(rps.InfRps);
                    if (!String.IsNullOrEmpty(infRpsXml))
                    {
                        signatureTypeRps = ObterTagSignatureAssinada<SignatureType>(infRpsXml, rps.InfRps.id);

                        if (signatureTypeRps != null)
                        {
                            rps.Signature = signatureTypeRps;
                        }
                    }
                }

                SignatureType signatureTypeLote = null;

                String loteRpsXml = ConverterDataBindEmStringXml<tcLoteRps>(enviarLoteRPSEnvio.LoteRps);

                if (!String.IsNullOrEmpty(loteRpsXml))
                {
                    signatureTypeLote = ObterTagSignatureAssinada<SignatureType>(loteRpsXml, enviarLoteRPSEnvio.LoteRps.id);

                    if (signatureTypeLote != null)
                    {
                        enviarLoteRPSEnvio.Signature = signatureTypeLote;
                    }
                }
                retorno = ConverterDataBindEmStringXml(enviarLoteRPSEnvio, true);
            }
            catch (Exception erro)
            {
                AddMensagem("Ocorreu um erro ao assinar o XML: " + erro.Message);
            }

            return retorno;
        }

        protected override bool IsDadosValidos()
        {
            if (!String.IsNullOrEmpty(XML))
            {
                enviarLoteRPSEnvio = ConverterStringXmlEmDataBind<EnviarLoteRpsEnvio>(XML);

                if (enviarLoteRPSEnvio == null)
                {
                    AddMensagem("Ocorreu erro ao converter XML em Classe");
                }
            }
            else
            {
                AddMensagem("XML vazio");
            }

            return Mensagens.Count == 0;
        }
    }
}