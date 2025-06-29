using System;
using System.Drawing;
using System.Drawing.Imaging;
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
                            SaveAsPDF(saveDialog.FileName);
                            break;
                        case ".png":
                            SaveAsPNG(saveDialog.FileName);
                            break;
                        default:
                            SaveAsText(saveDialog.FileName);
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

        private void SaveAsText(string fileName)
        {
            using StreamWriter writer = new StreamWriter(fileName);
            writer.WriteLine($"Дата и время: {DateTime.Now}");
            writer.WriteLine("=====================================\n");

            writer.WriteLine("Исходный массив (100 элементов от 2 до 22):");
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    writer.Write($"{array[i * 10 + j],4}");
                }
                writer.WriteLine();
            }

            writer.WriteLine("\n=====================================");
            writer.WriteLine("Результаты вычислений:");
            writer.WriteLine(txtResult.Text);
        }

        private void SaveAsPDF(string fileName)
        {
            try
            {
                string htmlFileName = fileName.Replace(".pdf", ".html");
                string html = GenerateHTMLReport();
                File.WriteAllText(htmlFileName, html);
                
                MessageBox.Show(
                    $"Файл сохранен как HTML: {Path.GetFileName(htmlFileName)}\n\n" +
                    "Для конвертации в PDF:\n" +
                    "1. Откройте файл в браузере\n" +
                    "2. Нажмите Ctrl+P\n" +
                    "3. Выберите 'Сохранить как PDF'",
                    "Файл сохранен",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при сохранении: {ex.Message}\n" +
                    "Файл будет сохранен как PNG.",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                    
                SaveAsPNG(fileName.Replace(".pdf", ".png"));
            }
        }

        private void SaveAsPNG(string fileName)
        {
            int width = 850;
            int height = 1100;

            using Bitmap bitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                Font titleFont = new Font("Arial", 20, FontStyle.Bold);
                Font headerFont = new Font("Arial", 14, FontStyle.Bold);
                Font normalFont = new Font("Arial", 12);
                Font arrayFont = new Font("Courier New", 10);

                float y = 50;

                g.DrawString("Результаты вычислений", titleFont, Brushes.Black,
                    width / 2, y, new StringFormat { Alignment = StringAlignment.Center });
                y += 50;

                g.DrawString($"Дата и время: {DateTime.Now}", normalFont, Brushes.Black, 50, y);
                y += 40;

                g.DrawString("Исходный массив (100 элементов от 2 до 22):", headerFont, Brushes.Black, 50, y);
                y += 30;

                for (int i = 0; i < 10; i++)
                {
                    float x = 50;
                    for (int j = 0; j < 10; j++)
                    {
                        int index = i * 10 + j;
                        string value = array[index].ToString();

                        bool isSquared = squaredPositions.Contains(index);

                        if (isSquared)
                        {
                            g.FillRectangle(Brushes.LightGreen, x - 2, y - 2, 30, 20);
                        }

                        g.DrawString(value.PadLeft(3), arrayFont, Brushes.Black, x, y);
                        x += 35;
                    }
                    y += 25;
                }

                y += 30;

                g.DrawString("Результаты вычислений:", headerFont, Brushes.Black, 50, y);
                y += 30;

                string[] lines = txtResult.Text.Split(new[] { "\r\n" }, StringSplitOptions.None);
                foreach (string line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        g.DrawString(line, normalFont, Brushes.Black, 50, y);
                        y += 25;
                    }
                }

                y = height - 100;
                g.FillRectangle(Brushes.LightGreen, 50, y, 20, 15);
                g.DrawString("- элементы на позициях 1², 2², 3²... 9²", normalFont, Brushes.Black, 75, y - 2);
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

            PrintViaHTML();
        }

        private void PrintViaHTML()
        {
            try
            {
                string html = GenerateHTMLReport();

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

        private string GenerateHTMLReport()
        {
            StringBuilder html = new StringBuilder();

            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.AppendLine("<meta charset='UTF-8'>");
            html.AppendLine("<title>Результаты вычислений массива</title>");
            html.AppendLine("<style>");
            html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; }");
            html.AppendLine("h1 { color: #333; }");
            html.AppendLine("table { border-collapse: collapse; margin: 20px 0; }");
            html.AppendLine("td { border: 1px solid #ddd; padding: 8px; text-align: center; width: 40px; }");
            html.AppendLine(".highlight { background-color: #90EE90; font-weight: bold; }");
            html.AppendLine(".result { background-color: #f0f0f0; padding: 15px; margin: 20px 0; }");
            html.AppendLine("@media print { body { margin: 0; } }");
            html.AppendLine("</style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");

            html.AppendLine("<h1>Результаты вычислений массива</h1>");
            html.AppendLine($"<p><strong>Дата и время:</strong> {DateTime.Now}</p>");
            html.AppendLine($"<p><strong>Выполнил:</strong> Мокан Константин, 24 ИС</p>");

            html.AppendLine("<h2>Исходный массив (100 элементов от 2 до 22)</h2>");
            html.AppendLine("<table>");

            for (int i = 0; i < 10; i++)
            {
                html.AppendLine("<tr>");
                for (int j = 0; j < 10; j++)
                {
                    int index = i * 10 + j;
                    bool isHighlight = squaredPositions.Contains(index);
                    string cssClass = isHighlight ? " class='highlight'" : "";
                    html.AppendLine($"<td{cssClass}>{array[index]}</td>");
                }
                html.AppendLine("</tr>");
            }

            html.AppendLine("</table>");
            html.AppendLine("<p><span style='background-color: #90EE90; padding: 2px 8px;'>Зелёным</span> выделены элементы на позициях 1², 2², 3²... 9²</p>");

            html.AppendLine("<div class='result'>");
            html.AppendLine("<h2>Результаты вычислений</h2>");

            string[] lines = txtResult.Text.Split(new[] { "\r\n" }, StringSplitOptions.None);
            foreach (string line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    html.AppendLine($"<p>{line}</p>");
                }
            }

            html.AppendLine("</div>");

            html.AppendLine("</body>");
            html.AppendLine("</html>");

            return html.ToString();
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
