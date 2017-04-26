﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace MXM.Infraestrutura
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

            if (IsCertificadoExistente() && IsDadosValidos())
            {
                retorno = ExecutarProcessoEspecifico();

                if (Mensagens.Count > 0)
                {
                    retorno += String.Concat(Mensagens.ToArray());
                }
            }
            else
            {
                retorno = Mensagens.ToString();
            }

            return retorno;
        }

        private bool IsCertificadoExistente()
        {
            if (string.IsNullOrWhiteSpace(numeroSerieCertificado))
            {
                AddMensagem("CODERRO0 - Certificado digital não parametrizado.");
            }
            else
            {
                certificado = FindCertificate(StoreLocation.CurrentUser, StoreName.My, X509FindType.FindBySerialNumber, numeroSerieCertificado);

                if (certificado == null)
                {
                    AddMensagem("CODERRO1 - Certificado digital não instalado.");
                }
            }

            return Mensagens.Count == 0;
        }

        protected X509Certificate2 FindCertificate(StoreLocation location, StoreName name, X509FindType findType, string findValue)
        {
            X509Store store = new X509Store(name, location);
            try
            {
                store.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection col = store.Certificates.Find(findType, findValue, false);

                if ((col != null) && (col.Count > 0))
                {
                    return col[0];
                }

                return null;
            }
            finally
            {
                store.Close();
            }
        }

        protected void AddMensagem(String descricao)
        {
            this.Mensagens.Add(descricao);
        }

        protected String GerarTagSignature(String xml, String IDRps, Boolean IsSalvador = false)
        {            
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = false;
            doc.LoadXml(xml);

            Reference reference = new Reference();
            reference.Uri = "";
            if (String.IsNullOrEmpty(IDRps))
            {
                reference.Uri = ""; // "#" + childElemen.GetAttributeNode("Id").Value;
            }
            else
            {
                reference.Uri = "#" + IDRps;
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

            if (IsSalvador)
            {
                KeyInfoClause rsaKeyVal = new RSAKeyValue((System.Security.Cryptography.RSA)certificado.PrivateKey);
                KeyInfoX509Data x509Data = new KeyInfoX509Data(certificado);
                x509Data.AddSubjectName(certificado.SubjectName.Name.ToString());
                keyInfo.AddClause(x509Data);
                keyInfo.AddClause(rsaKeyVal);
            }

            signedXml.KeyInfo = keyInfo;
            signedXml.ComputeSignature();

            return signedXml.GetXml().OuterXml;
        }
    }
}