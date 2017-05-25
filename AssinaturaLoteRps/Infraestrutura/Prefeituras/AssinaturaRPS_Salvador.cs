using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace MXM.Assinatura.Infraestrutura.Prefeituras
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
                string xmlAssinado;

                foreach (var rps in enviarLoteRPSEnvio.LoteRps.ListaRps)
                {
                    SignatureType signatureTypeRps = null;

                    xmlAssinado = ConverterDataBindEmStringXml<EnviarLoteRpsEnvio>(enviarLoteRPSEnvio);
                    if (!String.IsNullOrEmpty(xmlAssinado))
                    {
                        signatureTypeRps = ObterTagSignatureAssinada<SignatureType>(xmlAssinado, rps.InfRps.id, true);

                        if (signatureTypeRps != null)
                        {
                            rps.Signature = signatureTypeRps;
                        }
                    }
                }

                SignatureType signatureTypeLote = null;

                xmlAssinado = ConverterDataBindEmStringXml<EnviarLoteRpsEnvio>(enviarLoteRPSEnvio);

                if (!String.IsNullOrEmpty(xmlAssinado))
                {
                    signatureTypeLote = ObterTagSignatureAssinada<SignatureType>(xmlAssinado, enviarLoteRPSEnvio.LoteRps.id, true);

                    if (signatureTypeLote != null)
                    {
                        enviarLoteRPSEnvio.Signature = signatureTypeLote;
                    }
                }
                retorno = ConverterDataBindEmStringXml(enviarLoteRPSEnvio);
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

        private String AssinarXml(string arquivo, string tagAssinatura, string tagAtributoId)
        {
            // Create a new XML document.
            XmlDocument doc = new XmlDocument();

            // Format the document to ignore white spaces.
            doc.PreserveWhitespace = false;

            // Load the passed XML file using it’s name.
            doc.LoadXml(arquivo);

            if (doc.GetElementsByTagName(tagAssinatura).Count == 0)
            {
                throw new Exception("A tag de assinatura " + tagAssinatura.Trim() + " não existe no XML. (Código do Erro: 5)");
            }
            else if (doc.GetElementsByTagName(tagAtributoId).Count == 0)
            {
                throw new Exception("A tag de assinatura " + tagAtributoId.Trim() + " não existe no XML. (Código do Erro: 4)");
            }
            else
            {
                XmlNodeList lists = doc.GetElementsByTagName(tagAssinatura);
                foreach (XmlNode nodes in lists)
                {
                    foreach (XmlNode childNodes in nodes.ChildNodes)
                    {
                        if (!childNodes.Name.Equals(tagAtributoId))
                            continue;

                        if (childNodes.NextSibling != null && childNodes.NextSibling.Name.Equals("Signature"))
                            continue;

                        // Create a reference to be signed
                        Reference reference = new Reference();
                        reference.Uri = "";

                        XmlElement childElemen = (XmlElement)childNodes;
                        if (childElemen.GetAttributeNode("Id") != null)
                        {
                            reference.Uri = ""; // "#" + childElemen.GetAttributeNode("Id").Value;
                        }
                        else if (childElemen.GetAttributeNode("id") != null)
                        {
                            reference.Uri = "#" + childElemen.GetAttributeNode("id").Value;
                        }

                        // Create a SignedXml object.
                        SignedXml signedXml = new SignedXml(doc);

                        // Add the key to the SignedXml document
                        signedXml.SigningKey = certificado.PrivateKey;

                        // Add an enveloped transformation to the reference.
                        XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
                        reference.AddTransform(env);

                        XmlDsigC14NTransform c14 = new XmlDsigC14NTransform();
                        reference.AddTransform(c14);

                        // Add the reference to the SignedXml object.
                        signedXml.AddReference(reference);

                        // Create a new KeyInfo object
                        KeyInfo keyInfo = new KeyInfo();

                        // Load the certificate into a KeyInfoX509Data object
                        // and add it to the KeyInfo object.
                        keyInfo.AddClause(new KeyInfoX509Data(certificado));

                        // Add the KeyInfo object to the SignedXml object.
                        signedXml.KeyInfo = keyInfo;
                        signedXml.ComputeSignature();

                        // Get the XML representation of the signature and save
                        // it to an XmlElement object.
                        XmlElement xmlDigitalSignature = signedXml.GetXml();

                        nodes.AppendChild(doc.ImportNode(xmlDigitalSignature, true));
                    }
                }

                return doc.OuterXml;
            }
        }
    }
}