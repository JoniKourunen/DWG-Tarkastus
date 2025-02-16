using OfficeOpenXml;
using System.Text;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using OfficeOpenXml;
using ClosedXML.Excel;

namespace DWG_Tarkastus
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) // scannatta kansion valinta
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Valitse kansio, jossa DWG-tiedostot sijaitsevat";
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e) // tarkistaa DWG:t
        {
            string hakemisto = textBox1.Text;

            foreach (string tiedosto in Directory.GetFiles(hakemisto, "*.dwg"))
            {
                try
                {
                    using (FileStream fs = new FileStream(tiedosto, FileMode.Open, FileAccess.Read))
                    using (BinaryReader reader = new BinaryReader(fs))
                    {
                        byte[] buffer = new byte[6];
                        reader.Read(buffer, 0, 6);
                        string versio = Encoding.ASCII.GetString(buffer);

                        if (versio == "AC1032" || versio == "AC1033" || versio == "AC1034") // 2016 tai uudempi
                        {
                            string tiedostonimi = Path.GetFileName(tiedosto);
                            listBox1.Items.Add(tiedostonimi);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Virhe luettaessa tiedostoa {tiedosto}: {ex.Message}");
                }
            }

            MessageBox.Show("Tarkistus valmis.");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)  // tallenna exceliin
        {
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Excel Files|*.xlsx";
                saveFileDialog.Title = "Tallenna Excel-tiedosto";
                saveFileDialog.FileName = "tulokset.xlsx";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string outputTiedosto = saveFileDialog.FileName;

                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Tarkistetut DWG-tiedostot");
                        worksheet.Cell(1, 1).Value = "Tiedostonimi";
                        int row = 2;

                        foreach (string tiedosto in listBox1.Items)
                        {
                            worksheet.Cell(row, 1).Value = tiedosto;
                            row++;
                        }

                        workbook.SaveAs(outputTiedosto);
                        MessageBox.Show($"Tiedosto tallennettu: {outputTiedosto}");
                    }
                }
            }
        }
    }
}







