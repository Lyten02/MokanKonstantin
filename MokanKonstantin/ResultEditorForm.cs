using System;
using System.Drawing;
using System.Windows.Forms;

namespace MokanKonstantin
{
    public partial class ResultEditorForm : Form
    {
        private TextBox txtTemplate;
        private TextBox txtPreview;
        private Button btnSave;
        private Button btnPrint;
        private Button btnCancel;
        private Label lblInstructions;
        private Label lblTemplate;
        private Label lblPreview;
        private SplitContainer splitContainer;

        private string arrayData;
        private string calculationResults;
        
        public string EditedTemplate { get; private set; }
        public DialogResult Result { get; private set; }

        public ResultEditorForm(string arrayData, string calculationResults)
        {
            this.arrayData = arrayData;
            this.calculationResults = calculationResults;
            
            InitializeComponent();
            InitializeDefaultTemplate();
        }

        private void InitializeDefaultTemplate()
        {
            txtTemplate.Text = @"Результаты вычислений массива
Дата и время: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + @"

Исходный массив (100 элементов от 2 до 22):
{ARRAY}

Результаты вычислений:
{RESULTS}

Выполнил: Мокан Константин, 24 ИС";
            
            UpdatePreview();
        }

        private void InitializeComponent()
        {
            this.txtTemplate = new TextBox();
            this.txtPreview = new TextBox();
            this.btnSave = new Button();
            this.btnPrint = new Button();
            this.btnCancel = new Button();
            this.lblInstructions = new Label();
            this.lblTemplate = new Label();
            this.lblPreview = new Label();
            this.splitContainer = new SplitContainer();
            
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();

            // Form settings
            this.Text = "Редактор результатов";
            this.ClientSize = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(700, 500);

            // lblInstructions
            this.lblInstructions.Text = "Используйте {ARRAY} для вставки массива и {RESULTS} для вставки результатов:";
            this.lblInstructions.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblInstructions.Location = new Point(12, 12);
            this.lblInstructions.Size = new Size(876, 20);
            this.lblInstructions.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // splitContainer
            this.splitContainer.Location = new Point(12, 35);
            this.splitContainer.Size = new Size(876, 515);
            this.splitContainer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.splitContainer.SplitterDistance = 430;

            // lblTemplate
            this.lblTemplate.Text = "Шаблон:";
            this.lblTemplate.Location = new Point(3, 3);
            this.lblTemplate.Size = new Size(100, 20);

            // txtTemplate
            this.txtTemplate.Multiline = true;
            this.txtTemplate.ScrollBars = ScrollBars.Both;
            this.txtTemplate.WordWrap = true;
            this.txtTemplate.Font = new Font("Consolas", 10F);
            this.txtTemplate.Location = new Point(3, 26);
            this.txtTemplate.Size = new Size(424, 450);
            this.txtTemplate.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.txtTemplate.TextChanged += TxtTemplate_TextChanged;

            // lblPreview
            this.lblPreview.Text = "Предпросмотр:";
            this.lblPreview.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblPreview.Location = new Point(3, 3);
            this.lblPreview.Size = new Size(100, 20);

            // txtPreview
            this.txtPreview.Multiline = true;
            this.txtPreview.ReadOnly = true;
            this.txtPreview.ScrollBars = ScrollBars.Both;
            this.txtPreview.WordWrap = true;
            this.txtPreview.Font = new Font("Consolas", 10F);
            this.txtPreview.BackColor = SystemColors.Control;
            this.txtPreview.Location = new Point(3, 26);
            this.txtPreview.Size = new Size(439, 450);
            this.txtPreview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // btnSave
            this.btnSave.Text = "Сохранить";
            this.btnSave.Location = new Point(650, 556);
            this.btnSave.Size = new Size(80, 30);
            this.btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnSave.Click += BtnSave_Click;

            // btnPrint
            this.btnPrint.Text = "Печать";
            this.btnPrint.Location = new Point(736, 556);
            this.btnPrint.Size = new Size(80, 30);
            this.btnPrint.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnPrint.Click += BtnPrint_Click;

            // btnCancel
            this.btnCancel.Text = "Отмена";
            this.btnCancel.Location = new Point(822, 556);
            this.btnCancel.Size = new Size(80, 30);
            this.btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.btnCancel.Click += BtnCancel_Click;

            // Add controls to panels
            this.splitContainer.Panel1.Controls.Add(this.lblTemplate);
            this.splitContainer.Panel1.Controls.Add(this.txtTemplate);
            this.splitContainer.Panel2.Controls.Add(this.lblPreview);
            this.splitContainer.Panel2.Controls.Add(this.txtPreview);

            // Add controls to form
            this.Controls.Add(this.lblInstructions);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.btnCancel);

            // Tab order
            this.txtTemplate.TabIndex = 0;
            this.btnSave.TabIndex = 1;
            this.btnPrint.TabIndex = 2;
            this.btnCancel.TabIndex = 3;

            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private void TxtTemplate_TextChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            string template = txtTemplate.Text;
            
            // Заменяем все возможные варианты заполнителей
            string finalOutput = template
                .Replace("{ARRAY}", arrayData)
                .Replace("{RESULTS}", calculationResults)
                .Replace("{array}", arrayData)
                .Replace("{results}", calculationResults)
                .Replace("{printed array}", arrayData)
                .Replace("{printed result}", calculationResults);
            
            txtPreview.Text = finalOutput;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            EditedTemplate = txtTemplate.Text;
            this.Result = DialogResult.OK;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            EditedTemplate = txtTemplate.Text;
            this.Result = DialogResult.Yes;
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Result = DialogResult.Cancel;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public string GetFinalOutput()
        {
            return txtPreview.Text;
        }
    }
}