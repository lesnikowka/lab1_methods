using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections;
using System.Windows.Forms.VisualStyles;
using System.Numerics;

namespace lab1_dotnet_framework
{
    enum TaskType
    {
        Test,
        Main1,
        Main2
    }


    public partial class Form1 : Form
    {
        private TaskType SelectedTask = TaskType.Main1;

        private Dictionary<Tuple<double, double>, List<Series>> SeriesForStartConditions = new Dictionary<Tuple<double, double>, List<Series>>();

        private DataTable table = new DataTable();

        private DataBase db = null;

        private List<string> columnNames = new List<string>{ "id", "xi", "vi", "v2i", "vi-v2i", "loc_prec", "hi", "c1", "c2", "u", "u-v" };

        string currentTableDB = "main1";

        public Form1()
        {
            InitializeComponent();

            chart1.ChartAreas["ChartArea1"].BackColor = Color.Transparent;
            chart2.ChartAreas["ChartArea1"].BackColor = Color.Transparent;
            chart3.ChartAreas["ChartArea1"].BackColor = Color.Transparent;

            
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.chart1.Series.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string x0Text = textBox1.Text;
            string u0Text = textBox2.Text;
            string startStepText = textBox3.Text;
            string localPrecisionText = textBox4.Text;
            string boundPrecisionText = textBox5.Text;
            string maxStepNumbersText = textBox6.Text;
            string integrationBoundText = textBox7.Text;

            if (x0Text.Length == 0 || u0Text.Length == 0 ||
                startStepText.Length == 0 || localPrecisionText.Length == 0 ||
                boundPrecisionText.Length == 0 || maxStepNumbersText.Length == 0 ||
                integrationBoundText.Length == 0
                )
            {
                MessageBox.Show("Вы не ввели начальные условия", "Ошибка");
                return;
            }

            x0Text = x0Text.Replace('.', ',');
            u0Text = u0Text.Replace('.', ',');
            startStepText = startStepText.Replace('.', ',');
            localPrecisionText = localPrecisionText.Replace('.', ',');
            boundPrecisionText = boundPrecisionText.Replace('.', ',');
            integrationBoundText = integrationBoundText.Replace('.', ',');


            double X0;
            double U0;
            double startStep;
            double localPrecision;
            double boundPrecision;
            double integrationBound;

            int maxStepNumbers;

            try
            {
                X0 = Convert.ToDouble(x0Text);
                U0 = Convert.ToDouble(u0Text);
                startStep = Convert.ToDouble(startStepText);
                localPrecision = Convert.ToDouble(boundPrecisionText);
                boundPrecision = Convert.ToDouble(maxStepNumbersText);  
                maxStepNumbers = Convert.ToInt32(maxStepNumbersText);
                integrationBound = Convert.ToDouble(integrationBoundText);
            }
            catch 
            {
                MessageBox.Show("Неверный формат введенных значений", "Ошибка");
                return;
            }

            Tuple<double, double> x0u0Tuple = new Tuple<double, double>(X0, U0);

            if (SeriesForStartConditions.ContainsKey(x0u0Tuple))
            {

                List<Series> oldSeries = SeriesForStartConditions[x0u0Tuple];

                for (int i = 0; i < oldSeries.Count; i++)
                {
                    this.chart1.Series.Remove(oldSeries[i]);
                }

                SeriesForStartConditions.Remove(x0u0Tuple);

            }

            Series newNumericSeries = new Series();

            List<Series> newSeriesList = new List<Series>();

            newSeriesList.Add(newNumericSeries);
            
            newNumericSeries.Name = "Численное решение при X0 = " + X0.ToString() + " U0 = " + U0.ToString();

            newNumericSeries.ChartType = SeriesChartType.Line;

            newNumericSeries.BorderWidth = 2;

            DrawNumericSolution(newNumericSeries, X0, U0, startStep, localPrecision, boundPrecision, maxStepNumbers, integrationBound);

            this.chart1.Series.Add(newNumericSeries);

            if (SelectedTask == TaskType.Test)
            {
                Series newTrueSeries = new Series();

                newSeriesList.Add(newTrueSeries);

                newTrueSeries.Name = "Истинное решение при X0 = " + X0.ToString() + " U0 = " + U0.ToString();

                newTrueSeries.ChartType = SeriesChartType.Line;

                newTrueSeries.BorderWidth = 2;

                DrawTrueSolution(newTrueSeries, X0, U0, 0.1);

                this.chart1.Series.Add(newTrueSeries);
            }

            SeriesForStartConditions.Add(x0u0Tuple, newSeriesList);

            if (SelectedTask == TaskType.Main2)
            {
                //draw 2 graph planes for mai2
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                db = new DataBase("\\database\\lab1.sqlite3");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }


            table.Columns.Add("id", typeof(string));
            table.Columns.Add("xi", typeof(string));
            table.Columns.Add("vi", typeof(string));
            table.Columns.Add("v2i", typeof(string));
            table.Columns.Add("vi-v2i", typeof(string));
            table.Columns.Add("loc_prec", typeof(string));
            table.Columns.Add("hi", typeof(string));
            table.Columns.Add("C1", typeof(string));
            table.Columns.Add("C2", typeof(string));
            
            dataGridView1.DataSource = table;

            showStartConditions("main1");
        }

        private void showStartConditions(string tableName)
        {
            comboBox1.Items.Clear();

            List<List<string>> startConditions = null;

            try
            {
                startConditions = db.GetAllStartConditions(tableName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }


            for (int i = 0; i < startConditions.Count; i++)
            {
                comboBox1.Items.Add(startConditions[i][0] + ", " + startConditions[i][1]);
            }
        }

        private void тестоваяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            выборТипаЗадачиToolStripMenuItem.Text = "Тестовая";
            SelectedTask = TaskType.Test;
            this.chart1.Series.Clear();

            if (!table.Columns.Contains("u") && !table.Columns.Contains("u-v"))
            {
                table.Columns.Add("u", typeof(string));
                table.Columns.Add("u-v", typeof(string));
            }

            showStartConditions("test");

            currentTableDB = "test";
        }

        private void основнаяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            выборТипаЗадачиToolStripMenuItem.Text = "Основная 1";
            SelectedTask = TaskType.Main1;
            this.chart1.Series.Clear();

            if (table.Columns.Contains("u") && table.Columns.Contains("u-v"))
            {
                table.Columns.Remove("u");
                table.Columns.Remove("u-v");
            }


            showStartConditions("main1");

            currentTableDB = "main1";

        }
        private void основная2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            выборТипаЗадачиToolStripMenuItem.Text = "Основная 2";
            SelectedTask = TaskType.Main2;
            this.chart1.Series.Clear();

            if (table.Columns.Contains("u") && table.Columns.Contains("u-v"))
            {
                table.Columns.Remove("u");
                table.Columns.Remove("u-v");
            }


            showStartConditions("main2");

            currentTableDB = "main2";
        }

        private double TrueSoluitonFunction(double X0, double U0)
        {
            return 3;
        }

        private void DrawTrueSolution(Series series, double X0, double U0, double h)
        {

            for (int i = 0; i < 1000; i++)
            {
                series.Points.AddXY(i, i * i);
            }
        }

        private void DrawNumericSolution(Series series, double X0, double U0, double startStep, double localPrecision, double boundPrecision, int maxStepNumbers, double integrationBound)
        {
            for (int i = 0; i < 1000; i++)
            {
                series.Points.AddXY(i, i * i * 1.5);
            }
        }

        private void ShowDataForStartCondition(string tableName, List<string> startCondition)
        {
            List<List<string>> dataForStartCondition = db.GetDataForStartCondition(tableName, startCondition);

            table.Rows.Clear();

            int columnNamesSize = tableName == "test" ?  columnNames.Count : columnNames.Count - 2; 
            
            for (int i = 0; i < dataForStartCondition.Count; i++)
            {
                DataRow row = table.NewRow();

                for (int j = 0; j < columnNamesSize; j++)
                {
                    row[columnNames[j]] = dataForStartCondition[i][j];
                }

                table.Rows.Add(row);    
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        List<string> stringConditionToList(string startConditionString) // 1,
        {
            List<string> startCondition = new List<string>();

            int commaIndex = startConditionString.IndexOf(',');

            startCondition.Add(startConditionString.Substring(0, commaIndex));
            startCondition.Add(startConditionString.Substring(commaIndex + 2));

            return startCondition;
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            List<string> selectedCondition = stringConditionToList(comboBox1.GetItemText(comboBox1.SelectedItem));

            ShowDataForStartCondition(currentTableDB, selectedCondition);
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }
    }
}
