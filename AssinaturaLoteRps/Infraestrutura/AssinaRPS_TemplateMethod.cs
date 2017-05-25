using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace MXM.Assinatura.Infraestrutura
{
    [Guid("B94BE863-F063-4BE6-B019-0F901A9671CB")]
    public abstract class AssinaRPS_TemplateMethod
    {
        protected abstract Boolean IsDadosValidos();

        protected abstract String ExecutarProcessoEspecifico();

        protected List<String> Mensagens;
        protected X509Certificate2 certificado;
        protected string numeroSerieCertificado;

        public AssinaRPS_TemplateMethod()
        {
            Mensagens = new List<string>();
            certificado = null;
        }

        public String Assinar(string sNumeroSerieCert)
        {
            this.numeroSerieCertificado = sNumeroSerieCert;

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
            if (string.IsNullOrWhiteSpace(numeroSerieCertificado))
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
                store.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection col = store.Certificates.Find(findType, findValue, false);

                if ((col != null) && (col.Count > 0))
                {
                    retorno = col[0];
                }
            }
            catch (Exception erro)
            {
                AddMensagem("Ocorreu um erro ao localizar o certificado: " + erro.Message);
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

        protected T ObterTagSignatureAssinada<T>(String xml, String idSignature, Boolean IsSalvador = false) where T : class
        {
            T retorno = null;
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream, System.Text.Encoding.UTF8);
            try
            {
                string xmlSignature = GerarTagSignature(xml, idSignature, IsSalvador);

                if (!String.IsNullOrEmpty(xmlSignature))
                {
                    XmlSerializer serSignatureType = new XmlSerializer(typeof(T));
                    writer.Write(xmlSignature);
                    writer.Flush();
                    stream.Position = 0;

                    retorno = serSignatureType.Deserialize(stream) as T;
                }
            }
            catch (Exception erro)
            {
                AddMensagem("Ocorreu erro ao obter o objeto assinado: " + erro.ToString());
            }
            finally
            {
                stream.Close();
                writer.Close();
            }

            return retorno;
        }

        private String GerarTagSignature(String xml, String idSignature, Boolean IsSalvador = false)
        {
            String retorno = String.Empty;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = false;
                doc.LoadXml(xml);

                Reference reference = new Reference();
                reference.Uri = "";
                if (!String.IsNullOrEmpty(idSignature))
                {
                    reference.Uri = "#" + idSignature;
                }

                SignedXml signedXml = new SignedXml(doc);
                signedXml.SigningKey = certificado.PrivateKey;

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
                    KeyInfoClause rsaKeyVal = new RSAKeyValue((System.Security.Cryptography.RSA)certificado.PrivateKey);
                    keyInfo.AddClause(rsaKeyVal);

                    x509Data.AddSubjectName(certificado.SubjectName.Name.ToString());
                }

                keyInfo.AddClause(x509Data);

                signedXml.KeyInfo = keyInfo;
                signedXml.ComputeSignature();

                retorno = signedXml.GetXml().OuterXml;
            }
            catch (Exception erro)
            {
                AddMensagem("Ocorreu erro ao gerar assinatura: " + erro.ToString());
            }

            return retorno;
        }

        protected String ConverterDataBindEmStringXml<T>(T aDataBind) where T : class
        {
            String retorno = String.Empty;
            StringWriter stringWriter = new StringWriter();
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Encoding = Encoding.UTF8;
                settings.Indent = false;
                settings.OmitXmlDeclaration = false;

                using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, settings))
                {
                    xmlSerializer.Serialize(xmlWriter, aDataBind);
                }

                retorno = stringWriter.ToString();
                retorno = XElement.Parse(retorno).ToString(SaveOptions.DisableFormatting);
            }
            catch (Exception erro)
            {
                AddMensagem("Ocorreu um erro ao converter databind em xml: " + erro.ToString());
            }
            finally
            {
                stringWriter.Close();
            }

            return retorno;
        }

        protected T ConverterStringXmlEmDataBind<T>(String xml) where T : class
        {
            T retorno = null;

            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                writer.Write(xml);
                writer.Flush();
                stream.Position = 0;

                retorno = serializer.Deserialize(stream) as T;
            }
            catch (Exception erro)
            {
                AddMensagem("Ocorreu erro ao converter xml em databind: " + erro.ToString());
            }
            finally
            {
                stream.Close();
                writer.Close();
            }
            return retorno;
        }
    }
}