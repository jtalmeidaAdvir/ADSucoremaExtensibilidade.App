
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ADSucoremaExtensibilidade
{
    public partial class ProgressForm : Form
    {
        private ProgressBar progressBar;
        private Label lblStatus;
        private Label lblTitle;

        public ProgressForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.progressBar = new ProgressBar();
            this.lblStatus = new Label();
            this.lblTitle = new Label();
            this.SuspendLayout();

            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new Point(12, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new Size(200, 15);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "🔄 Processando artigos...";

            // 
            // progressBar
            // 
            this.progressBar.Location = new Point(15, 45);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new Size(350, 23);
            this.progressBar.Style = ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 1;
            this.progressBar.Value = 0;

            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new Point(12, 80);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new Size(100, 13);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Iniciando...";

            // 
            // ProgressForm
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.ClientSize = new Size(380, 110);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Processamento em Andamento";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        public void UpdateProgress(int percentage, string status)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int, string>(UpdateProgress), percentage, status);
                return;
            }

            // Garantir que o valor está dentro dos limites
            percentage = Math.Max(0, Math.Min(100, percentage));

            this.progressBar.Value = percentage;
            this.lblStatus.Text = status;

            // Atualizar título baseado no progresso
            if (percentage == 100)
            {
                this.lblTitle.Text = "✅ Processamento Concluído!";
                this.lblTitle.ForeColor = Color.Green;
            }
            else if (percentage >= 80)
            {
                this.lblTitle.Text = "🔄 Finalizando processamento...";
                this.lblTitle.ForeColor = Color.Blue;
            }
            else if (percentage >= 50)
            {
                this.lblTitle.Text = "🔄 Processando artigos...";
                this.lblTitle.ForeColor = Color.DarkBlue;
            }
            else
            {
                this.lblTitle.Text = "🔄 Preparando processamento...";
                this.lblTitle.ForeColor = Color.DarkOrange;
            }

            // Forçar atualização da interface
            this.Refresh();
            Application.DoEvents();
        }

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(value);

            if (value)
            {
                // Centralizar no formulário pai se possível
                if (this.Owner != null)
                {
                    this.Location = new Point(
                        this.Owner.Location.X + (this.Owner.Width - this.Width) / 2,
                        this.Owner.Location.Y + (this.Owner.Height - this.Height) / 2
                    );
                }
            }
        }
    }
}
