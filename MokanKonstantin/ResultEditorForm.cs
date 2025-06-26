using System;
using System.Drawing;
using System.Windows.Forms;

namespace MokanKonstantin
{
    public partial class ResultEditorForm : Form
    {
        private TextBox txtResults;
        private CheckBox chkIncludeArray;
        private CheckBox chkIncludeCalculations;
        private TextBox txtHeader;
        private TextBox txtFooter;
        private Button btnSave;
        private Button btnPrint;
        private Button btnCancel;
        private Label lblHeader;
        private Label lblFooter;
        private Label lblResults;
        private Panel pnlButtons;
        private Panel pnlOptions;

        public string EditedResults { get; private set; }
        public bool IncludeArray { get; private set; }
        public bool IncludeCalculations { get; private set; }
        public string CustomHeader { get; private set; }
        public string CustomFooter { get; private set; }
        public DialogResult Result { get; private set; }

        public ResultEditorForm(string currentResults, bool includeArray = true, bool includeCalculations = true)
        {
            InitializeComponent();


            txtResults.Text = currentResults;
            chkIncludeArray.Checked = includeArray;
            chkIncludeCalculations.Checked = includeCalculations;


            this.Text = "Edit Results";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void InitializeComponent()
        {

            this.txtResults = new TextBox();
            this.chkIncludeArray = new CheckBox();
            this.chkIncludeCalculations = new CheckBox();
            this.txtHeader = new TextBox();
            this.txtFooter = new TextBox();
            this.btnSave = new Button();
            this.btnPrint = new Button();
            this.btnCancel = new Button();
            this.lblHeader = new Label();
            this.lblFooter = new Label();
            this.lblResults = new Label();
            this.pnlButtons = new Panel();
            this.pnlOptions = new Panel();

            this.SuspendLayout();


            this.ClientSize = new Size(600, 500);


            this.lblResults.Text = "Results:";
            this.lblResults.Location = new Point(12, 12);
            this.lblResults.Size = new Size(100, 20);


            this.txtResults.Multiline = true;
            this.txtResults.ScrollBars = ScrollBars.Both;
            this.txtResults.WordWrap = false;
            this.txtResults.Font = new Font("Consolas", 9F);
            this.txtResults.Location = new Point(12, 35);
            this.txtResults.Size = new Size(576, 250);
            this.txtResults.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;


            this.pnlOptions.Location = new Point(12, 291);
            this.pnlOptions.Size = new Size(576, 120);
            this.pnlOptions.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;


            this.chkIncludeArray.Text = "Include Array in Output";
            this.chkIncludeArray.Location = new Point(0, 0);
            this.chkIncludeArray.Size = new Size(200, 24);
            this.chkIncludeArray.CheckedChanged += ChkIncludeArray_CheckedChanged;


            this.chkIncludeCalculations.Text = "Include Calculations in Output";
            this.chkIncludeCalculations.Location = new Point(220, 0);
            this.chkIncludeCalculations.Size = new Size(200, 24);
            this.chkIncludeCalculations.CheckedChanged += ChkIncludeCalculations_CheckedChanged;


            this.lblHeader.Text = "Custom Header:";
            this.lblHeader.Location = new Point(0, 30);
            this.lblHeader.Size = new Size(100, 20);

            this.txtHeader.Location = new Point(0, 50);
            this.txtHeader.Size = new Size(576, 20);
            this.txtHeader.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;


            this.lblFooter.Text = "Custom Footer:";
            this.lblFooter.Location = new Point(0, 75);
            this.lblFooter.Size = new Size(100, 20);

            this.txtFooter.Location = new Point(0, 95);
            this.txtFooter.Size = new Size(576, 20);
            this.txtFooter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;


            this.pnlOptions.Controls.Add(this.chkIncludeArray);
            this.pnlOptions.Controls.Add(this.chkIncludeCalculations);
            this.pnlOptions.Controls.Add(this.lblHeader);
            this.pnlOptions.Controls.Add(this.txtHeader);
            this.pnlOptions.Controls.Add(this.lblFooter);
            this.pnlOptions.Controls.Add(this.txtFooter);


            this.pnlButtons.Location = new Point(12, 417);
            this.pnlButtons.Size = new Size(576, 40);
            this.pnlButtons.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;


            this.btnSave.Text = "Save";
            this.btnSave.Location = new Point(326, 5);
            this.btnSave.Size = new Size(80, 30);
            this.btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnSave.Click += BtnSave_Click;


            this.btnPrint.Text = "Print";
            this.btnPrint.Location = new Point(412, 5);
            this.btnPrint.Size = new Size(80, 30);
            this.btnPrint.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnPrint.Click += BtnPrint_Click;


            this.btnCancel.Text = "Cancel";
            this.btnCancel.Location = new Point(498, 5);
            this.btnCancel.Size = new Size(80, 30);
            this.btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnCancel.Click += BtnCancel_Click;


            this.pnlButtons.Controls.Add(this.btnSave);
            this.pnlButtons.Controls.Add(this.btnPrint);
            this.pnlButtons.Controls.Add(this.btnCancel);


            this.Controls.Add(this.lblResults);
            this.Controls.Add(this.txtResults);
            this.Controls.Add(this.pnlOptions);
            this.Controls.Add(this.pnlButtons);


            this.txtResults.TabIndex = 0;
            this.chkIncludeArray.TabIndex = 1;
            this.chkIncludeCalculations.TabIndex = 2;
            this.txtHeader.TabIndex = 3;
            this.txtFooter.TabIndex = 4;
            this.btnSave.TabIndex = 5;
            this.btnPrint.TabIndex = 6;
            this.btnCancel.TabIndex = 7;

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void ChkIncludeArray_CheckedChanged(object sender, EventArgs e)
        {
            UpdateResultsPreview();
        }

        private void ChkIncludeCalculations_CheckedChanged(object sender, EventArgs e)
        {
            UpdateResultsPreview();
        }

        private void UpdateResultsPreview()
        {


        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            PrepareResults();
            this.Result = DialogResult.OK;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            PrepareResults();
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

        private void PrepareResults()
        {
            EditedResults = txtResults.Text;
            IncludeArray = chkIncludeArray.Checked;
            IncludeCalculations = chkIncludeCalculations.Checked;
            CustomHeader = txtHeader.Text;
            CustomFooter = txtFooter.Text;
        }

        public string GetFinalOutput()
        {
            string output = "";


            if (!string.IsNullOrWhiteSpace(CustomHeader))
            {
                output += CustomHeader + Environment.NewLine + Environment.NewLine;
            }


            output += EditedResults;


            if (!string.IsNullOrWhiteSpace(CustomFooter))
            {
                output += Environment.NewLine + Environment.NewLine + CustomFooter;
            }

            return output;
        }
    }
}