using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Replacer
{
    public partial class Form1 : Form
    {
        private string Path;
        private List<Replace> ReplacementList;
        private bool EditMode;
        private int Row;
        private int CompletedReplacements;

        public Form1()
        {
            InitializeComponent();
            ReplacementList = new List<Replace>();
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                Path = folderBrowserDialog1.SelectedPath;
                textBox4.Text = Path;
                button2.Enabled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var search = textBox2.Text;
            var replacement = textBox3.Text;
            if (string.IsNullOrEmpty(search) || string.IsNullOrEmpty(replacement)) return;
            ReplacementList.Add(new Replace(search, replacement));
            dataGridView1.Rows.Add(search, replacement);
            dataGridView1.ClearSelection();
            textBox2.Text = string.Empty;
            textBox3.Text = string.Empty;
            EditMode = false;
            Row = 0;
            button4.Enabled = false;
            button5.Enabled = false;
        }

        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count <= 0) return;
            var index = dataGridView1.SelectedCells[0].RowIndex;
            var selectedRow = dataGridView1.Rows[index];
            textBox2.Text = selectedRow.Cells["Column1"].Value.ToString();
            textBox3.Text = selectedRow.Cells["Column2"].Value.ToString();
            EditMode = true;
            Row = index;
            button4.Enabled = true;
            button5.Enabled = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (!EditMode) return;
            var search = textBox2.Text;
            var replacement = textBox3.Text;
            if (string.IsNullOrEmpty(search) || string.IsNullOrEmpty(replacement)) return;
            dataGridView1.ClearSelection();
            var replace = ReplacementList[Row];
            replace.Search = search;
            replace.Replacement = replacement;
            ReplacementList[Row] = replace;
            dataGridView1.Rows[Row].Cells["Column1"].Value = search;
            dataGridView1.Rows[Row].Cells["Column2"].Value = replacement;
            textBox2.Text = string.Empty;
            textBox3.Text = string.Empty;
            button4.Enabled = false;
            button5.Enabled = false;
            EditMode = false;
            Row = 0;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!EditMode) return;
            var search = textBox2.Text;
            var replacement = textBox3.Text;
            if (string.IsNullOrEmpty(search) || string.IsNullOrEmpty(replacement)) return;
            var replace = ReplacementList[Row];
            ReplacementList.Remove(replace);
            var row = dataGridView1.Rows[Row];
            dataGridView1.Rows.Remove(row);
            dataGridView1.ClearSelection();
            textBox2.Text = string.Empty;
            textBox3.Text = string.Empty;
            button4.Enabled = false;
            button5.Enabled = false;
            EditMode = false;
            Row = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Path) || ReplacementList.Count <= 0 || !Directory.Exists(Path)) return;
            var files = new List<string>();
            foreach (var file in Directory.GetFiles(Path))
            {
                if (!comboBox2.Text.Equals(".*") && !file.EndsWith(comboBox2.Text)) continue;
                files.Add(file);
            }

            CompletedReplacements = 0;
            foreach (var file in files)
            {
                var content = string.Empty;
                using (var reader = new StreamReader(file))
                {
                    content = reader.ReadToEnd();
                    reader.Dispose();
                }

                foreach (var replacement in ReplacementList)
                    content = content.Replace(replacement.Search, replacement.Replacement);
                
                using (var writer = new StreamWriter(file))
                {
                    writer.Write(content);
                    writer.Dispose();
                }

                CompletedReplacements++;
                progressBar1.Value = CompletedReplacements * 100 / files.Count;
            }

            MessageBox.Show("DONE");
        }
    }
}