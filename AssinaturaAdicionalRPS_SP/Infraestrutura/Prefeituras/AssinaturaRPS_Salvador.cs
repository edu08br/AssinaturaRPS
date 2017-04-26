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
                    rps.Signature = ObterTagSignatureAssinada<SignatureType>(XML, rps.InfRps.id, true);
                }

                enviarLoteRPSEnvio.Signature = ObterTagSignatureAssinada<SignatureType>(XML, enviarLoteRPSEnvio.LoteRps.id, true);

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

        private string AssinarXml(string tagAssinatura, string tagAtributoId)
        {
            {
                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = false;
                doc.LoadXml(XML);

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

                            SignedXml signedXml = new SignedXml(doc);
                            signedXml.SigningKey = certificado.PrivateKey;

                            XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
                            reference.AddTransform(env);

                            //XmlDsigC14NTransform c14 = new XmlDsigC14NTransform();
                            //reference.AddTransform(c14);

                            signedXml.AddReference(reference);

                            // Create a new KeyInfo object
                            KeyInfo keyInfo = new KeyInfo();

                            KeyInfoClause rsaKeyVal = new RSAKeyValue((System.Security.Cryptography.RSA)certificado.PrivateKey);
                            KeyInfoX509Data x509Data = new KeyInfoX509Data(certificado);
                            x509Data.AddSubjectName(certificado.SubjectName.Name.ToString());
                            keyInfo.AddClause(x509Data);
                            keyInfo.AddClause(rsaKeyVal);

                            // Add the KeyInfo object to the SignedXml object.
                            signedXml.KeyInfo = keyInfo;
                            signedXml.ComputeSignature();

                            // Get the XML representation of the signature and save
                            // it to an XmlElement object.
                            XmlElement xmlDigitalSignature = signedXml.GetXml();

                            nodes.AppendChild(doc.ImportNode(xmlDigitalSignature, true));
                        }
                    }

                    XmlDocument XMLDoc = new XmlDocument();
                    XMLDoc.PreserveWhitespace = false;
                    XMLDoc = doc;

                    return doc.OuterXml;

                    //using (StreamWriter sw = File.CreateText(arquivo))
                    //{
                    //    sw.Write(conteudoXMLAssinado);
                    //    sw.Close();
                    //}
                }
            }
        }
    }
}