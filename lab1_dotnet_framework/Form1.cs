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

        Dictionary<Tuple<double, double>, List<Series>> SeriesForStartConditions = new Dictionary<Tuple<double, double>, List<Series>>();

        public Form1()
        {
            InitializeComponent();

            chart1.ChartAreas["ChartArea1"].BackColor = Color.Transparent;
            chart2.ChartAreas["ChartArea1"].BackColor = Color.Transparent;
            chart3.ChartAreas["ChartArea1"].BackColor = Color.Transparent;

            try
            {
                DataBase db = new DataBase("\\database\\lab1.sqlite3");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }
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

            DrawNumericSolution(newNumericSeries, X0, U0, startStep, localPrecision, boundPrecision, maxStepNumbers, integrationBound);

            this.chart1.Series.Add(newNumericSeries);

            if (SelectedTask == TaskType.Test)
            {
                Series newTrueSeries = new Series();

                newSeriesList.Add(newTrueSeries);

                newTrueSeries.Name = "Истинное решение при X0 = " + X0.ToString() + " U0 = " + U0.ToString();

                newTrueSeries.ChartType = SeriesChartType.Line;

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
        }

        private void тестоваяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            выборТипаЗадачиToolStripMenuItem.Text = "Тестовая";
            SelectedTask = TaskType.Test;
            this.chart1.Series.Clear();
        }

        private void основнаяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            выборТипаЗадачиToolStripMenuItem.Text = "Основная 1";
            SelectedTask = TaskType.Main1;
            this.chart1.Series.Clear();
        }
        private void основная2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            выборТипаЗадачиToolStripMenuItem.Text = "Основная 2";
            SelectedTask = TaskType.Main2;
            this.chart1.Series.Clear();
        }

        private double TrueSoluitonFunction(double X0, double U0)
        {
            return 3;
        }

        private void DrawTrueSolution(Series series, double X0, double U0, double h)
        {
            series.Points.Add(X0, U0);
            series.Points.Add(3 * X0, 3 * U0);
        }

        private void DrawNumericSolution(Series series, double X0, double U0, double startStep, double localPrecision, double boundPrecision, int maxStepNumbers, double integrationBound)
        {
            series.Points.Add(X0, U0);
            series.Points.Add(2 * X0, 2 * U0);
        }


        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }
    }
}
