using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Printing;
using System.Drawing.Imaging;

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
            this.Text = "Калькулятор суммы элементов массива";
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
            using (StreamWriter writer = new StreamWriter(fileName))
            {
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
        }

        private void SaveAsPDF(string fileName)
        {
            try
            {
                PrintDocument printDoc = new PrintDocument();
                printDoc.PrinterSettings.PrinterName = "Microsoft Print to PDF";
                printDoc.PrinterSettings.PrintToFile = true;
                printDoc.PrinterSettings.PrintFileName = fileName;
                printDoc.DefaultPageSettings.Landscape = false;
                printDoc.PrintPage += PrintDocument_PrintPage;
                
                // Проверяем, установлен ли PDF принтер
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
                    // Если PDF принтер не найден, сохраняем как PNG
                    MessageBox.Show(
                        "PDF принтер не найден. Файл будет сохранен в формате PNG.\n" +
                        "Для сохранения в PDF используйте функцию печати.",
                        "Информация",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    
                    SaveAsPNG(fileName.Replace(".pdf", ".png"));
                }
            }
            catch (Exception)
            {
                // В случае ошибки сохраняем как PNG
                SaveAsPNG(fileName.Replace(".pdf", ".png"));
            }
        }

        private void SaveAsPNG(string fileName)
        {
            int width = 850;
            int height = 1100;
            
            using (Bitmap bitmap = new Bitmap(width, height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.White);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    
                    Font titleFont = new Font("Arial", 20, FontStyle.Bold);
                    Font headerFont = new Font("Arial", 14, FontStyle.Bold);
                    Font normalFont = new Font("Arial", 12);
                    Font arrayFont = new Font("Courier New", 10);
                    
                    float y = 50;
                    
                    // Заголовок
                    g.DrawString("Результаты вычислений", titleFont, Brushes.Black, 
                        width / 2, y, new StringFormat { Alignment = StringAlignment.Center });
                    y += 50;
                    
                    // Дата
                    g.DrawString($"Дата и время: {DateTime.Now}", normalFont, Brushes.Black, 50, y);
                    y += 40;
                    
                    // Массив
                    g.DrawString("Исходный массив (100 элементов от 2 до 22):", headerFont, Brushes.Black, 50, y);
                    y += 30;
                    
                    // Рисуем массив с выделением
                    for (int i = 0; i < 10; i++)
                    {
                        float x = 50;
                        for (int j = 0; j < 10; j++)
                        {
                            int index = i * 10 + j;
                            string value = array[index].ToString();
                            
                            // Проверяем, является ли позиция квадратом числа
                            bool isSquared = squaredPositions.Contains(index);
                            
                            if (isSquared)
                            {
                                // Рисуем зеленый фон для выделенных элементов
                                g.FillRectangle(Brushes.LightGreen, x - 2, y - 2, 30, 20);
                            }
                            
                            g.DrawString(value.PadLeft(3), arrayFont, Brushes.Black, x, y);
                            x += 35;
                        }
                        y += 25;
                    }
                    
                    y += 30;
                    
                    // Результаты
                    g.DrawString("Результаты вычислений:", headerFont, Brushes.Black, 50, y);
                    y += 30;
                    
                    // Разбиваем текст результата на строки
                    string[] lines = txtResult.Text.Split(new[] { "\r\n" }, StringSplitOptions.None);
                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            g.DrawString(line, normalFont, Brushes.Black, 50, y);
                            y += 25;
                        }
                    }
                    
                    // Легенда
                    y = height - 100;
                    g.FillRectangle(Brushes.LightGreen, 50, y, 20, 15);
                    g.DrawString("- элементы на позициях 1², 2², 3²... 9²", normalFont, Brushes.Black, 75, y - 2);
                }
                
                // Сохраняем изображение
                bitmap.Save(fileName, ImageFormat.Png);
            }
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
                    
                    foreach (string line in lines)
                    {
                        if (line.Contains("Исходный массив"))
                        {
                            for (int i = 0; i < 10 && index < lines.Length; i++)
                            {
                                string dataLine = lines[Array.IndexOf(lines, line) + i + 1];
                                if (dataLine.Contains("=")) break;
                                
                                string[] values = dataLine.Split(new[] { ' ' }, 
                                    StringSplitOptions.RemoveEmptyEntries);
                                
                                foreach (string val in values)
                                {
                                    if (int.TryParse(val, out int num) && index < 100)
                                    {
                                        array[index++] = num;
                                    }
                                }
                            }
                            break;
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
                        throw new Exception("Неверный формат файла");
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

            try
            {
                PrintDocument printDoc = new PrintDocument();
                PrintPreviewDialog previewDialog = new PrintPreviewDialog();
                
                printDoc.PrintPage += PrintDocument_PrintPage;
                previewDialog.Document = printDoc;
                previewDialog.WindowState = FormWindowState.Maximized;
                
                previewDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при печати: {ex.Message}", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font font = new Font("Arial", 12);
            Font titleFont = new Font("Arial", 16, FontStyle.Bold);
            float y = 50;
            
            g.DrawString("Результаты вычислений", titleFont, Brushes.Black, 50, y);
            y += 40;
            
            g.DrawString($"Дата: {DateTime.Now}", font, Brushes.Black, 50, y);
            y += 30;
            
            g.DrawString("Массив:", font, Brushes.Black, 50, y);
            y += 25;
            
            for (int i = 0; i < 10; i++)
            {
                string line = "";
                for (int j = 0; j < 10; j++)
                {
                    line += $"{array[i * 10 + j],4}";
                }
                g.DrawString(line, new Font("Courier New", 10), Brushes.Black, 50, y);
                y += 20;
            }
            
            y += 20;
            g.DrawString("Результаты:", font, Brushes.Black, 50, y);
            y += 25;
            
            string[] resultLines = txtResult.Text.Split(new[] { "\r\n" }, 
                StringSplitOptions.RemoveEmptyEntries);
            
            foreach (string line in resultLines)
            {
                g.DrawString(line, font, Brushes.Black, 50, y);
                y += 25;
            }
        }

        private void ShowAbout()
        {
            MessageBox.Show(
                "Калькулятор суммы элементов массива\n\n" +
                "Версия: 1.0.0\n" +
                "Разработчик: Константин Мокан\n\n" +
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
    }
}
