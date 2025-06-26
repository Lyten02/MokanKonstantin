using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MokanKonstantin
{
    public partial class Form1 : Form
    {
        private int[] array;
        private int[] squaredPositions = { 0, 3, 8, 15, 24, 35, 48, 63, 80 };
        private int sum = 0;

        public Form1()
        {
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Мокан Константин 24 ИС: Калькулятор суммы элементов массива";
            this.BackColor = Color.WhiteSmoke;
            this.MinimumSize = new Size(900, 600);

            lblStatus.Text = "Готов к работе";
        }

        private void GenerateArray()
        {
            array = new int[100];
            Random rand = new Random();

            for (int i = 0; i < 100; i++)
            {
                array[i] = rand.Next(2, 23);
            }

            DisplayArray();
            lblStatus.Text = "Массив сгенерирован успешно";
            btnCalculate.Enabled = true;
        }

        private void DisplayArray()
        {
            dgvArray.Rows.Clear();
            dgvArray.ColumnCount = 10;

            for (int i = 0; i < 10; i++)
            {
                dgvArray.Columns[i].Width = 50;
                dgvArray.Columns[i].HeaderText = i.ToString();
            }

            for (int row = 0; row < 10; row++)
            {
                object[] rowData = new object[10];
                for (int col = 0; col < 10; col++)
                {
                    int index = row * 10 + col;
                    rowData[col] = array[index];
                }
                dgvArray.Rows.Add(rowData);
            }

            dgvArray.RowHeadersVisible = true;
            for (int i = 0; i < dgvArray.Rows.Count; i++)
            {
                dgvArray.Rows[i].HeaderCell.Value = (i * 10).ToString();
            }

            HighlightSquaredPositions();
        }

        private void HighlightSquaredPositions()
        {
            foreach (int pos in squaredPositions)
            {
                int row = pos / 10;
                int col = pos % 10;
                dgvArray.Rows[row].Cells[col].Style.BackColor = Color.LightGreen;
                dgvArray.Rows[row].Cells[col].Style.Font = new Font(dgvArray.Font, FontStyle.Bold);
            }
        }

        private void CalculateSum()
        {
            if (array == null)
            {
                MessageBox.Show("Сначала необходимо сгенерировать массив!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            sum = 0;
            txtResult.Clear();
            txtResult.AppendText("Вычисление суммы элементов на позициях:\r\n\r\n");

            for (int i = 1; i <= 9; i++)
            {
                int position = i * i - 1;
                int value = array[position];
                sum += value;

                txtResult.AppendText($"Позиция {i}² = {i * i}: array[{position}] = {value}\r\n");
            }

            txtResult.AppendText($"\r\n\r\nИтоговая сумма: {sum}");
            lblStatus.Text = $"Сумма вычислена: {sum}";

            btnSave.Enabled = true;
            btnPrint.Enabled = true;
        }

        private void SaveToFile()
        {
            if (array == null)
            {
                MessageBox.Show("Сначала необходимо сгенерировать массив!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Подготавливаем данные для редактора
            string arrayText = GetArrayText();
            string resultsText = txtResult.Text;
            
            // Показываем редактор
            var editorForm = new ResultEditorForm(arrayText, resultsText);
            DialogResult editorResult = editorForm.ShowDialog();

            if (editorResult == DialogResult.OK || editorResult == DialogResult.Yes)
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "PDF files (*.pdf)|*.pdf|PNG Image (*.png)|*.png|Text files (*.txt)|*.txt|All files (*.*)|*.*";
                saveDialog.DefaultExt = "pdf";
                saveDialog.FileName = $"array_sum_{DateTime.Now:yyyyMMdd_HHmmss}";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string extension = Path.GetExtension(saveDialog.FileName).ToLower();

                        switch (extension)
                        {
                            case ".pdf":
                                SaveAsPDF(saveDialog.FileName, editorForm);
                                break;
                            case ".png":
                                SaveAsPNG(saveDialog.FileName, editorForm);
                                break;
                            default:
                                SaveAsText(saveDialog.FileName, editorForm);
                                break;
                        }

                        MessageBox.Show("Результат успешно сохранен!", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        lblStatus.Text = "Результат сохранен в файл";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private string GetArrayText()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    sb.Append(array[i * 10 + j].ToString().PadLeft(4));
                }
                if (i < 9) sb.AppendLine();
            }
            return sb.ToString();
        }
        
        private string GetFullResultsText(bool includeArray = true, bool includeCalculations = true)
        {
            StringBuilder sb = new StringBuilder();
            
            sb.AppendLine($"Дата и время: {DateTime.Now}");
            sb.AppendLine("=====================================");
            sb.AppendLine();

            if (includeArray && array != null)
            {
                sb.AppendLine("Исходный массив (100 элементов от 2 до 22):");
                sb.Append(GetArrayText());
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine("=====================================");
            }

            if (includeCalculations && !string.IsNullOrEmpty(txtResult.Text))
            {
                sb.AppendLine("Результаты вычислений:");
                sb.AppendLine(txtResult.Text);
            }

            return sb.ToString();
        }

        private void SaveAsText(string fileName, ResultEditorForm editorForm = null)
        {
            string content = editorForm != null ? editorForm.GetFinalOutput() : GetFullResultsText();
            File.WriteAllText(fileName, content);
        }

        private void SaveAsPDF(string fileName, ResultEditorForm editorForm = null)
        {
            try
            {
                PrintDocument printDoc = new PrintDocument();
                printDoc.PrinterSettings.PrinterName = "Microsoft Print to PDF";
                printDoc.PrinterSettings.PrintToFile = true;
                printDoc.PrinterSettings.PrintFileName = fileName;
                printDoc.DefaultPageSettings.Landscape = false;
                
                if (editorForm != null)
                {
                    printDoc.PrintPage += (sender, e) => PrintDocument_PrintPageWithEditor(sender, e, editorForm);
                }
                else
                {
                    printDoc.PrintPage += PrintDocument_PrintPage;
                }

                bool pdfPrinterFound = false;
                foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                {
                    if (printer.Contains("PDF"))
                    {
                        pdfPrinterFound = true;
                        break;
                    }
                }

                if (pdfPrinterFound)
                {
                    printDoc.Print();
                }
                else
                {
                    MessageBox.Show(
                        "PDF принтер не найден. Файл будет сохранен в формате PNG.\n" +
                        "Для сохранения в PDF используйте функцию печати.",
                        "Информация",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    SaveAsPNG(fileName.Replace(".pdf", ".png"), editorForm);
                }
            }
            catch (Exception)
            {
                SaveAsPNG(fileName.Replace(".pdf", ".png"), editorForm);
            }
        }

        private void SaveAsPNG(string fileName, ResultEditorForm editorForm = null)
        {
            string content = editorForm != null ? editorForm.GetFinalOutput() : GetFullResultsText();
            string[] lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            
            // Вычисляем размер изображения
            int lineHeight = 20;
            int width = 850;
            int height = Math.Max(1100, (lines.Length + 10) * lineHeight);

            using Bitmap bitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                Font font = new Font("Consolas", 11);
                float y = 20;
                float x = 20;

                foreach (string line in lines)
                {
                    g.DrawString(line, font, Brushes.Black, x, y);
                    y += lineHeight;
                }
            }

            bitmap.Save(fileName, ImageFormat.Png);
        }

        private void LoadFromFile()
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string[] lines = File.ReadAllLines(openDialog.FileName);
                    array = new int[100];
                    int index = 0;
                    bool foundArray = false;

                    for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
                    {
                        if (lines[lineIndex].Contains("Исходный массив"))
                        {
                            foundArray = true;
                            for (int row = 0; row < 10 && lineIndex + row + 1 < lines.Length; row++)
                            {
                                string dataLine = lines[lineIndex + row + 1];

                                if (dataLine.Contains("=====")) break;

                                string[] values = dataLine.Split(new[] { ' ', '\t' },
                                    StringSplitOptions.RemoveEmptyEntries);

                                foreach (string val in values)
                                {
                                    if (int.TryParse(val.Trim(), out int num) && index < 100)
                                    {
                                        array[index++] = num;
                                    }
                                }
                            }
                            break;
                        }
                    }

                    if (!foundArray)
                    {
                        index = 0;
                        foreach (string line in lines)
                        {
                            if (line.Contains("=") || line.Contains("Дата") || line.Contains("Результат"))
                                continue;

                            string[] values = line.Split(new[] { ' ', '\t', ',' },
                                StringSplitOptions.RemoveEmptyEntries);

                            foreach (string val in values)
                            {
                                if (int.TryParse(val.Trim(), out int num) && index < 100)
                                {
                                    if (num >= 2 && num <= 22)
                                    {
                                        array[index++] = num;
                                    }
                                }
                            }
                        }
                    }

                    if (index == 100)
                    {
                        DisplayArray();
                        btnCalculate.Enabled = true;
                        lblStatus.Text = "Данные загружены из файла";
                        MessageBox.Show("Данные успешно загружены!", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        throw new Exception($"Неверный формат файла. Найдено {index} чисел из 100 необходимых.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке файла: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void PrintResults()
        {
            if (array == null)
            {
                MessageBox.Show("Сначала необходимо сгенерировать массив!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Подготавливаем данные для редактора
            string arrayText = GetArrayText();
            string resultsText = txtResult.Text;
            
            // Показываем редактор
            var editorForm = new ResultEditorForm(arrayText, resultsText);
            DialogResult editorResult = editorForm.ShowDialog();
            
            if (editorResult == DialogResult.Cancel)
                return;

            var printMethod = MessageBox.Show(
                "Выберите метод печати:\n\n" +
                "ДА - Печать через браузер (рекомендуется)\n" +
                "НЕТ - Стандартная печать Windows\n\n" +
                "Печать через браузер работает надёжнее!",
                "Выбор метода печати",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (printMethod == DialogResult.Cancel)
                return;

            if (printMethod == DialogResult.Yes)
            {
                PrintViaHTML(editorForm);
            }
            else
            {
                try
                {
                    PrintDocument printDoc = new PrintDocument();
                    PrintPreviewDialog previewDialog = new PrintPreviewDialog();

                    printDoc.DocumentName = "Результаты вычислений массива";
                    printDoc.DefaultPageSettings.Margins = new System.Drawing.Printing.Margins(50, 50, 50, 50);

                    printDoc.PrintPage += (sender, e) => PrintDocument_PrintPageWithEditor(sender, e, editorForm);

                    previewDialog.Document = printDoc;
                    previewDialog.WindowState = FormWindowState.Maximized;
                    previewDialog.UseAntiAlias = true;

                    previewDialog.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Ошибка при печати: {ex.Message}\n\n" +
                        "Попробуйте печать через браузер (рекомендуется)!",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    if (MessageBox.Show("Попробовать печать через браузер?", "Альтернативный способ",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        PrintViaHTML(editorForm);
                    }
                }
            }
        }

        private void PrintViaHTML(ResultEditorForm editorForm = null)
        {
            try
            {
                string html = GenerateHTMLReport(editorForm);

                string tempFile = Path.Combine(Path.GetTempPath(), $"array_report_{DateTime.Now:yyyyMMddHHmmss}.html");
                File.WriteAllText(tempFile, html);

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = tempFile,
                    UseShellExecute = true
                });

                MessageBox.Show(
                    "Документ открыт в браузере.\n\n" +
                    "Для печати используйте:\n" +
                    "• Ctrl+P или\n" +
                    "• Меню браузера → Печать\n\n" +
                    "Это самый надёжный способ печати!",
                    "Печать через браузер",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании HTML: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GenerateHTMLReport(ResultEditorForm editorForm = null)
        {
            string content = editorForm != null ? editorForm.GetFinalOutput() : GetFullResultsText();
            
            StringBuilder html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.AppendLine("<meta charset='UTF-8'>");
            html.AppendLine("<title>Результаты вычислений массива</title>");
            html.AppendLine("<style>");
            html.AppendLine("body { font-family: 'Consolas', 'Courier New', monospace; margin: 20px; }");
            html.AppendLine("pre { white-space: pre-wrap; word-wrap: break-word; }");
            html.AppendLine("@media print { body { margin: 10px; } }");
            html.AppendLine("</style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine("<pre>");
            html.AppendLine(System.Web.HttpUtility.HtmlEncode(content));
            html.AppendLine("</pre>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            
            return html.ToString();
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (array == null || array.Length == 0)
            {
                e.Cancel = true;
                return;
            }

            Graphics g = e.Graphics;
            Font font = new Font("Arial", 12);
            Font titleFont = new Font("Arial", 16, FontStyle.Bold);
            Font arrayFont = new Font("Courier New", 10);

            Rectangle bounds = e.MarginBounds;
            float y = bounds.Top;
            float x = bounds.Left;

            g.DrawString("Результаты вычислений", titleFont, Brushes.Black, x, y);
            y += 40;

            g.DrawString($"Дата: {DateTime.Now}", font, Brushes.Black, x, y);
            y += 30;

            g.DrawString("Массив (100 элементов от 2 до 22):", font, Brushes.Black, x, y);
            y += 25;

            for (int i = 0; i < 10; i++)
            {
                string line = "";
                for (int j = 0; j < 10; j++)
                {
                    int index = i * 10 + j;
                    line += array[index].ToString().PadLeft(4);
                }
                g.DrawString(line, arrayFont, Brushes.Black, x, y);
                y += 20;
            }

            y += 20;

            g.DrawString("Результаты вычислений:", font, Brushes.Black, x, y);
            y += 25;

            if (!string.IsNullOrEmpty(txtResult.Text))
            {
                string[] resultLines = txtResult.Text.Split(new[] { "\r\n" }, StringSplitOptions.None);

                foreach (string line in resultLines)
                {
                    if (y > bounds.Bottom - 50)
                    {
                        e.HasMorePages = true;
                        return;
                    }

                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        g.DrawString(line, font, Brushes.Black, x, y);
                        y += 25;
                    }
                }
            }
            e.HasMorePages = false;

            string footer = $"Страница 1 - Программа: Калькулятор суммы элементов массива";
            g.DrawString(footer, new Font("Arial", 8), Brushes.Gray,
                x, bounds.Bottom - 20);
        }

        private void PrintDocument_PrintPageWithEditor(object sender, PrintPageEventArgs e, ResultEditorForm editorForm)
        {
            string content = editorForm.GetFinalOutput();
            string[] lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            
            Graphics g = e.Graphics;
            Font font = new Font("Consolas", 10);
            
            Rectangle bounds = e.MarginBounds;
            float y = bounds.Top;
            float x = bounds.Left;
            
            foreach (string line in lines)
            {
                if (y > bounds.Bottom - 50)
                {
                    e.HasMorePages = true;
                    return;
                }
                
                g.DrawString(line, font, Brushes.Black, x, y);
                y += 18;
            }
            
            e.HasMorePages = false;
        }

        private void ShowAbout()
        {
            MessageBox.Show(
                "Калькулятор суммы элементов массива\n\n" +
                "Версия: 1.0.0\n" +
                "Разработчик: Мокан Константин\n\n" +
                "Программа вычисляет сумму элементов массива\n" +
                "на позициях 1², 2², 3²... 9²",
                "О программе",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void ShowHelp()
        {
            string helpText =
                "ИНСТРУКЦИЯ ПО ИСПОЛЬЗОВАНИЮ\n\n" +
                "1. Нажмите 'Сгенерировать массив' для создания массива из 100 случайных чисел от 2 до 22\n\n" +
                "2. Зеленым цветом выделены элементы на позициях 1², 2², 3²... 9²\n\n" +
                "3. Нажмите 'Вычислить сумму' для расчета суммы выделенных элементов\n\n" +
                "4. Результат можно сохранить в файл или распечатать\n\n" +
                "5. Можно загрузить ранее сохраненный массив из файла";

            MessageBox.Show(helpText, "Инструкция", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ExitApplication()
        {
            if (MessageBox.Show("Вы уверены, что хотите выйти?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void loadMenuItem_Click(object sender, EventArgs e) => LoadFromFile();
        private void saveMenuItem_Click(object sender, EventArgs e) => SaveToFile();
        private void printMenuItem_Click(object sender, EventArgs e) => PrintResults();
        private void exitMenuItem_Click(object sender, EventArgs e) => ExitApplication();
        private void aboutMenuItem_Click(object sender, EventArgs e) => ShowAbout();
        private void versionMenuItem_Click(object sender, EventArgs e) =>
            MessageBox.Show("Версия: 1.0.0", "Версия программы", MessageBoxButtons.OK, MessageBoxIcon.Information);
        private void instructionMenuItem_Click(object sender, EventArgs e) => ShowHelp();
        private void btnGenerate_Click(object sender, EventArgs e) => GenerateArray();
        private void btnCalculate_Click(object sender, EventArgs e) => CalculateSum();
        private void btnSave_Click(object sender, EventArgs e) => SaveToFile();
        private void btnPrint_Click(object sender, EventArgs e) => PrintResults();
    }
}
