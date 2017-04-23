namespace TestarAssinaturaRPS
{
    partial class Princip
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmbUF = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.txbPathArquivoSelecionado = new System.Windows.Forms.TextBox();
            this.btnProcessar = new System.Windows.Forms.Button();
            this.cmbCertificados = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txbRetorno = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // cmbUF
            // 
            this.cmbUF.FormattingEnabled = true;
            this.cmbUF.Items.AddRange(new object[] {
            "Salvador",
            "São Paulo"});
            this.cmbUF.Location = new System.Drawing.Point(58, 18);
            this.cmbUF.Name = "cmbUF";
            this.cmbUF.Size = new System.Drawing.Size(183, 21);
            this.cmbUF.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(21, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "UF";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(15, 45);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(226, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Selecionar arquivo";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txbPathArquivoSelecionado
            // 
            this.txbPathArquivoSelecionado.Enabled = false;
            this.txbPathArquivoSelecionado.Location = new System.Drawing.Point(15, 74);
            this.txbPathArquivoSelecionado.Name = "txbPathArquivoSelecionado";
            this.txbPathArquivoSelecionado.Size = new System.Drawing.Size(310, 20);
            this.txbPathArquivoSelecionado.TabIndex = 4;
            // 
            // btnProcessar
            // 
            this.btnProcessar.Location = new System.Drawing.Point(15, 165);
            this.btnProcessar.Name = "btnProcessar";
            this.btnProcessar.Size = new System.Drawing.Size(75, 23);
            this.btnProcessar.TabIndex = 5;
            this.btnProcessar.Text = "Processar";
            this.btnProcessar.UseVisualStyleBackColor = true;
            this.btnProcessar.Click += new System.EventHandler(this.btnProcessar_Click);
            // 
            // cmbCertificados
            // 
            this.cmbCertificados.FormattingEnabled = true;
            this.cmbCertificados.Location = new System.Drawing.Point(15, 126);
            this.cmbCertificados.Name = "cmbCertificados";
            this.cmbCertificados.Size = new System.Drawing.Size(310, 21);
            this.cmbCertificados.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "label2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 210);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Retorno";
            // 
            // txbRetorno
            // 
            this.txbRetorno.Location = new System.Drawing.Point(10, 241);
            this.txbRetorno.Multiline = true;
            this.txbRetorno.Name = "txbRetorno";
            this.txbRetorno.ReadOnly = true;
            this.txbRetorno.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txbRetorno.Size = new System.Drawing.Size(315, 282);
            this.txbRetorno.TabIndex = 9;
            // 
            // Princip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(337, 526);
            this.Controls.Add(this.txbRetorno);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbCertificados);
            this.Controls.Add(this.btnProcessar);
            this.Controls.Add(this.txbPathArquivoSelecionado);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbUF);
            this.Name = "Princip";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Testar Assinatura NFSe";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbUF;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txbPathArquivoSelecionado;
        private System.Windows.Forms.Button btnProcessar;
        private System.Windows.Forms.ComboBox cmbCertificados;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txbRetorno;
    }
}

