using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace MXM.Infraestrutura
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
                String uri = "InfRps";

                //XmlSerializer ser = new XmlSerializer(typeof(EnviarLoteRpsEnvio));
                //EnviarLoteRpsEnvio enviarLoteRPSEnvio = ser.Deserialize(new FileStream(XML, FileMode.Open)) as EnviarLoteRpsEnvio;

                //foreach (var rps in enviarLoteRPSEnvio.LoteRps.ListaRps)
                //{
                //    if (rps.Signature == null)
                //    {
                //        rps.Signature = new SignatureType();
                //    }

                //    rps.Signature.SignedInfo = new SignedInfoType();

                //    rps.Signature.SignedInfo.CanonicalizationMethod = new CanonicalizationMethodType();
                //    rps.Signature.SignedInfo.CanonicalizationMethod.Algorithm = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";

                //    rps.Signature.SignedInfo.SignatureMethod = new SignatureMethodType();
                //    rps.Signature.SignedInfo.SignatureMethod.Algorithm = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";

                //}

                XmlDocument docXML = new XmlDocument();
                docXML.Load(XML);

                SignedXml signedXml = new SignedXml(docXML);
                signedXml.SigningKey = certificado.PrivateKey;
                signedXml.SignedInfo.CanonicalizationMethod = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";
                signedXml.SignedInfo.SignatureMethod = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";

                Reference reference = new Reference();

                // Pega a URI para ser assinada
                XmlAttributeCollection _Uri = docXML.GetElementsByTagName(uri).Item(0).Attributes;
                foreach (XmlAttribute _atributo in _Uri)
                {
                    if (_atributo.Name == "id")
                        reference.Uri = "#" + _atributo.InnerText;
                }

                // Adiciona o envelope à referência
                XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
                reference.AddTransform(env);

                // Atribui o método do Hash
                reference.DigestMethod = "http://www.w3.org/2000/09/xmldsig#sha1";

                // Adiciona a referencia ao XML assinado
                signedXml.AddReference(reference);

                // Cria o objeto keyInfo
                KeyInfo keyInfo = new KeyInfo();

                // Carrega a informação da KeyInfo
                KeyInfoClause rsaKeyVal = new RSAKeyValue((System.Security.Cryptography.RSA)certificado.PrivateKey);
                KeyInfoX509Data x509Data = new KeyInfoX509Data(certificado);
                x509Data.AddSubjectName(certificado.SubjectName.Name.ToString());
                keyInfo.AddClause(x509Data);
                keyInfo.AddClause(rsaKeyVal);

                signedXml.KeyInfo = keyInfo;
                signedXml.Signature.Id = "Assigned" + uri;
                signedXml.ComputeSignature();

                XmlElement xmlDigitalSignature = signedXml.GetXml();

                docXML.DocumentElement.AppendChild(docXML.ImportNode(xmlDigitalSignature, true));

                retorno = docXML.InnerXml;
            }
            catch (Exception erro)
            {
                AddMensagem("Ocorreu um erro ao assinar o XML: " + erro.Message);
            }

            return retorno;
        }

        protected override bool IsDadosValidos()
        {
            return true;
        }

        private void AssinarXml(string arquivo, string tagAssinatura, string tagAtributoId)
        {
            StreamReader SR = null;

            try
            {
                SR = File.OpenText(arquivo);
                string xmlString = SR.ReadToEnd();
                SR.Close();
                SR = null;

                // Create a new XML document.
                XmlDocument doc = new XmlDocument();

                // Format the document to ignore white spaces.
                doc.PreserveWhitespace = false;

                // Load the passed XML file using it’s name.
                doc.LoadXml(xmlString);

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
                    XmlDocument XMLDoc;

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

                    XMLDoc = new XmlDocument();
                    XMLDoc.PreserveWhitespace = false;
                    XMLDoc = doc;

                    string conteudoXMLAssinado = XMLDoc.OuterXml;

                    using (StreamWriter sw = File.CreateText(arquivo))
                    {
                        sw.Write(conteudoXMLAssinado);
                        sw.Close();
                    }
                }
            }
            finally
            {
                if (SR != null)
                    SR.Close();
            }
        }
    }
}