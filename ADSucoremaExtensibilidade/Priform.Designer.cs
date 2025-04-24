namespace ADSucoremaExtensibilidade
{
    partial class Priform
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Priform));
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.KGFornecedor = new System.Windows.Forms.NumericUpDown();
            this.PrecoKGFornecedor = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.TotalFornecedor = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.TotalSucorema = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.PrecoKGSucorema = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.KGSucorema = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.TotalFornecedor_TEXT = new System.Windows.Forms.TextBox();
            this.TotalSucorema_TEXT = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.KGFornecedor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PrecoKGFornecedor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TotalFornecedor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TotalSucorema)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PrecoKGSucorema)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.KGSucorema)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.Control;
            this.button1.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(116, 196);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(133, 32);
            this.button1.TabIndex = 0;
            this.button1.Text = "Aplicar distribuição";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(53, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "KG/UN";
            // 
            // KGFornecedor
            // 
            this.KGFornecedor.Location = new System.Drawing.Point(32, 82);
            this.KGFornecedor.Name = "KGFornecedor";
            this.KGFornecedor.Size = new System.Drawing.Size(87, 21);
            this.KGFornecedor.TabIndex = 2;
            this.KGFornecedor.ValueChanged += new System.EventHandler(this.KGFornecedor_ValueChanged);
            // 
            // PrecoKGFornecedor
            // 
            this.PrecoKGFornecedor.Location = new System.Drawing.Point(141, 82);
            this.PrecoKGFornecedor.Name = "PrecoKGFornecedor";
            this.PrecoKGFornecedor.Size = new System.Drawing.Size(87, 21);
            this.PrecoKGFornecedor.TabIndex = 4;
            this.PrecoKGFornecedor.ValueChanged += new System.EventHandler(this.PrecoKGFornecedor_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(147, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Preço KG/UN";
            // 
            // TotalFornecedor
            // 
            this.TotalFornecedor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TotalFornecedor.Enabled = false;
            this.TotalFornecedor.Location = new System.Drawing.Point(12, 283);
            this.TotalFornecedor.Name = "TotalFornecedor";
            this.TotalFornecedor.Size = new System.Drawing.Size(87, 21);
            this.TotalFornecedor.TabIndex = 6;
            this.TotalFornecedor.Visible = false;
            this.TotalFornecedor.ValueChanged += new System.EventHandler(this.TotalFornecedor_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(274, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Total";
            // 
            // TotalSucorema
            // 
            this.TotalSucorema.Enabled = false;
            this.TotalSucorema.InterceptArrowKeys = false;
            this.TotalSucorema.Location = new System.Drawing.Point(-4, 283);
            this.TotalSucorema.Name = "TotalSucorema";
            this.TotalSucorema.Size = new System.Drawing.Size(87, 21);
            this.TotalSucorema.TabIndex = 12;
            this.TotalSucorema.Visible = false;
            this.TotalSucorema.ValueChanged += new System.EventHandler(this.TotalSucorema_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(274, 134);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Total";
            // 
            // PrecoKGSucorema
            // 
            this.PrecoKGSucorema.Enabled = false;
            this.PrecoKGSucorema.Location = new System.Drawing.Point(141, 150);
            this.PrecoKGSucorema.Name = "PrecoKGSucorema";
            this.PrecoKGSucorema.Size = new System.Drawing.Size(87, 21);
            this.PrecoKGSucorema.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(147, 134);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Preço KG/UN";
            // 
            // KGSucorema
            // 
            this.KGSucorema.Enabled = false;
            this.KGSucorema.Location = new System.Drawing.Point(32, 150);
            this.KGSucorema.Name = "KGSucorema";
            this.KGSucorema.Size = new System.Drawing.Size(87, 21);
            this.KGSucorema.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(53, 134);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "KG/UN";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(68, 18);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(255, 21);
            this.comboBox1.TabIndex = 13;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(29, 21);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(37, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Artigo:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(323, 22);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(18, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "Kg";
            // 
            // TotalFornecedor_TEXT
            // 
            this.TotalFornecedor_TEXT.Location = new System.Drawing.Point(249, 82);
            this.TotalFornecedor_TEXT.Name = "TotalFornecedor_TEXT";
            this.TotalFornecedor_TEXT.Size = new System.Drawing.Size(87, 21);
            this.TotalFornecedor_TEXT.TabIndex = 16;
            this.TotalFornecedor_TEXT.TextChanged += new System.EventHandler(this.TotalFornecedor_TEXT_TextChanged);
            // 
            // TotalSucorema_TEXT
            // 
            this.TotalSucorema_TEXT.Location = new System.Drawing.Point(249, 150);
            this.TotalSucorema_TEXT.Name = "TotalSucorema_TEXT";
            this.TotalSucorema_TEXT.Size = new System.Drawing.Size(87, 21);
            this.TotalSucorema_TEXT.TabIndex = 17;
            this.TotalSucorema_TEXT.TextChanged += new System.EventHandler(this.TotalSucorema_TEXT_TextChanged);
            // 
            // Priform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(366, 251);
            this.Controls.Add(this.TotalSucorema_TEXT);
            this.Controls.Add(this.TotalFornecedor_TEXT);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.TotalSucorema);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.PrecoKGSucorema);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.KGSucorema);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.TotalFornecedor);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.PrecoKGFornecedor);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.KGFornecedor);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Priform";
            this.Text = "Calculadora de Preço por KG e UN ";
            this.Load += new System.EventHandler(this.Priform_Load);
            ((System.ComponentModel.ISupportInitialize)(this.KGFornecedor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PrecoKGFornecedor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TotalFornecedor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TotalSucorema)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PrecoKGSucorema)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.KGSucorema)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown KGFornecedor;
        private System.Windows.Forms.NumericUpDown PrecoKGFornecedor;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown TotalFornecedor;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown TotalSucorema;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown PrecoKGSucorema;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown KGSucorema;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox TotalFornecedor_TEXT;
        private System.Windows.Forms.TextBox TotalSucorema_TEXT;
    }
}