using MXM.Assinatura.Processos;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace TestarAssinaturaRPS
{
    public partial class Princip : Form
    {
        public Princip()
        {
            InitializeComponent();

            PreencherComoboxUF();
            PreencherComoboxComCertificados();
        }

        private void PreencherComoboxUF()
        {
            cmbUF.Items.Clear();

            cmbUF.Items.Add("Salvador");
            cmbUF.Items.Add("São Paulo");
        }

        private void PreencherComoboxComCertificados()
        {
            cmbCertificados.Items.Clear();

            X509Store store = new X509Store("My");
            store.Open(OpenFlags.ReadOnly);
            foreach (var certificado in store.Certificates)
            {
                cmbCertificados.Items.Add(certificado.SerialNumber);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFieldDialog = new OpenFileDialog();

            if (openFieldDialog.ShowDialog() == DialogResult.OK)
            {
                txbPathArquivoSelecionado.Text = openFieldDialog.FileName;
            }
        }

        private void btnProcessar_Click(object sender, EventArgs e)
        {
            AssinarXML();
        }

        private void AssinarXML()
        {
            String UF = cmbUF.Text;
            String serialCertificado = cmbCertificados.Text;
            String pathXML = txbPathArquivoSelecionado.Text;

            if ((!String.IsNullOrEmpty(UF)) && (!String.IsNullOrEmpty(serialCertificado)) && (!String.IsNullOrEmpty(pathXML)))
            {
                var assinaturaAdicional = new AssinaturaAdicionalRPS_SP();
                String retorno = String.Empty;

                if (UF == "Salvador")
                {
                    retorno = assinaturaAdicional.AssinaLoteRPS_Salvador(serialCertificado, pathXML);
                }
                else
                {
                    retorno = assinaturaAdicional.AssinaRPS_SP(serialCertificado, pathXML);
                }

                txbRetorno.Clear();
                txbRetorno.Text = retorno;
            }
        }
    }
}