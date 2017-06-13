using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace MXM.Assinatura.Infraestrutura
{
    public abstract class AssinaRPS_TemplateMethod
    {
        private string numeroSerieCertificado;

        protected abstract Boolean IsDadosValidos();

        protected abstract String ExecutarProcessoEspecifico();

        protected List<String> Mensagens;
        protected X509Certificate2 certificado;

        public AssinaRPS_TemplateMethod()
        {
            Mensagens = new List<string>();
            certificado = null;
        }

        public String Assinar(string sNumeroSerieCert)
        {
            this.numeroSerieCertificado = sNumeroSerieCert.ToUpper();

            String retorno = String.Empty;

            try
            {
                if (IsCertificadoExistente())
                {
                    if (IsDadosValidos())
                    {
                        retorno = ExecutarProcessoEspecifico();
                    }
                }
            }
            catch (Exception erro)
            {
                AddMensagem("Ocorreu erro ao executar o processo: " + erro.ToString());
            }
            finally
            {
                retorno += String.Concat(Mensagens.ToArray());
            }
            return retorno;
        }

        private bool IsCertificadoExistente()
        {
            if (String.IsNullOrEmpty(numeroSerieCertificado))
            {
                AddMensagem("Ocorreu erro - Certificado digital não parametrizado.");
            }
            else
            {
                certificado = FindCertificate(StoreLocation.CurrentUser, StoreName.My, X509FindType.FindBySerialNumber, numeroSerieCertificado);

                if (certificado == null)
                {
                    AddMensagem("Ocorreu erro - Certificado digital não instalado.");
                }
            }

            return (certificado != null);
        }

        protected X509Certificate2 FindCertificate(StoreLocation location, StoreName name, X509FindType findType, string findValue)
        {
            X509Certificate2 retorno = null;
            X509Store store = new X509Store(name, location);
            try
            {
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

                foreach (X509Certificate2 col in store.Certificates)
                {
                    if (string.Compare(col.SerialNumber, findValue) == 0)
                    {
                        retorno = col;
                        break;
                    }
                }
            }
            catch (Exception erro)
            {
                AddMensagem("Ocorreu erro ao localizar o certificado: " + erro.ToString());
            }
            finally
            {
                store.Close();
            }

            return retorno;
        }

        protected void AddMensagem(String descricao)
        {
            this.Mensagens.Add(descricao);
        }

        protected String AssinarXml(string aXML, string tagAssinatura, string tagAtributoId, Boolean IsSalvador = false)
        {
            string XMLAssinado = String.Empty;

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = false;
                doc.LoadXml(aXML);

                if (isDocumentoXmlValidoParaAssinar(doc, tagAssinatura, tagAtributoId))
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

                            XmlElement xmlNodeSignature = GerarAssinatura(IsSalvador, doc, childNodes);

                            if (xmlNodeSignature != null)
                            {
                                nodes.AppendChild(doc.ImportNode(xmlNodeSignature, true));
                            }
                        }
                    }

                    XMLAssinado = doc.OuterXml;
                }
            }
            catch (Exception erro)
            {
                AddMensagem("Ocorreu erro ao assinar. " + erro.ToString());
            }

            return XMLAssinado;
        }

        private bool isDocumentoXmlValidoParaAssinar(XmlDocument doc, string tagAssinatura, string tagAtributoId)
        {
            Boolean retorno = true;
            if (doc.GetElementsByTagName(tagAssinatura).Count == 0)
            {
                AddMensagem("Ocorreu erro na tag de assinatura " + tagAssinatura.Trim() + " não existe no XML.");
                retorno = false;
            }
            else if (doc.GetElementsByTagName(tagAtributoId).Count == 0)
            {
                AddMensagem("Ocorreu erro na tag de assinatura " + tagAtributoId.Trim() + " não existe no XML.");
                retorno = false;
            }

            return retorno;
        }

        private XmlElement GerarAssinatura(bool IsSalvador, XmlDocument doc, XmlNode childNodes)
        {
            XmlElement retorno = null;
            try
            {
                Reference reference = new Reference();
                reference.Uri = "";

                XmlElement childElemen = (XmlElement)childNodes;
                if (childElemen.GetAttributeNode("Id") != null)
                {
                    reference.Uri = "#" + childElemen.GetAttributeNode("Id").Value;
                }
                else if (childElemen.GetAttributeNode("id") != null)
                {
                    reference.Uri = "#" + childElemen.GetAttributeNode("id").Value;
                }

                RSACryptoServiceProvider privateKeyProvider = (RSACryptoServiceProvider)certificado.PrivateKey;

                SignedXml signedXml = new SignedXml(doc);
                signedXml.SigningKey = privateKeyProvider;

                XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
                reference.AddTransform(env);

                if (!IsSalvador)
                {
                    XmlDsigC14NTransform c14 = new XmlDsigC14NTransform();
                    reference.AddTransform(c14);
                }

                signedXml.AddReference(reference);

                KeyInfo keyInfo = new KeyInfo();
                KeyInfoX509Data x509Data = new KeyInfoX509Data(certificado);
                if (IsSalvador)
                {
                    KeyInfoClause rsaKeyVal = new RSAKeyValue((RSA)privateKeyProvider);
                    keyInfo.AddClause(rsaKeyVal);

                    x509Data.AddSubjectName(certificado.SubjectName.Name.ToString());
                }

                keyInfo.AddClause(x509Data);

                signedXml.KeyInfo = keyInfo;

                signedXml.ComputeSignature();

                retorno = signedXml.GetXml();
            }
            catch (Exception erro)
            {
                AddMensagem("Ocorreu erro ao assinar. " + erro.InnerException.ToString());
            }
            return retorno;
        }
    }
}