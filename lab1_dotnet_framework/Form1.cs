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

namespace lab1_dotnet_framework
{
    enum TaskType
    {
        Test,
        Main,
        NotSelected
    }


    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
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

            if (x0Text.Length == 0 || u0Text.Length == 0 || 
                startStepText.Length == 0 || localPrecisionText.Length == 0 ||
                boundPrecisionText.Length == 0 || maxStepNumbersText.Length == 0)
            {
                MessageBox.Show("Вы не ввели начальные условия", "Ошибка");
                return;
            }

            x0Text.Replace('.', ',');
            u0Text.Replace('.', ',');
            startStepText.Replace(".", ",");
            localPrecisionText.Replace(".", ",");   
            boundPrecisionText.Replace('.', ',');


            double X0;
            double U0;
            double startStep;
            double localPrecision;
            double boundPrecision;

            int maxStepNumbers;

            try
            {
                X0 = Convert.ToDouble(x0Text);
                U0 = Convert.ToDouble(u0Text);
                startStep = Convert.ToDouble(startStepText);
                localPrecision = Convert.ToDouble(boundPrecisionText);
                boundPrecision = Convert.ToDouble(maxStepNumbersText);  
                maxStepNumbers = Convert.ToInt32(maxStepNumbersText);
            }
            catch 
            {
                MessageBox.Show("Неверный формат введенных значений", "Ошибка");
                return;
            }

            Tuple<double, double> x0u0Tuple = new Tuple<double, double>(X0, U0);

            List<Series> oldSeries = SeriesForStartConditions[x0u0Tuple];

            for (int i = 0; i < oldSeries.Count; i++)
            {
                this.chart1.Series.Remove(oldSeries[i]);
            }

            SeriesForStartConditions.Remove(x0u0Tuple);

            Series newSeries = new Series();

            switch (SelectedTask)
            {
                case TaskType.Test:


                    break;
            }

            newSeries.ChartType = SeriesChartType.Line;

            newSeries.Points.Add(X0, U0);
            newSeries.Points.Add(2*X0, 2*U0);

            this.chart1.Series.Add(newSeries);

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
            выборТипаЗадачиToolStripMenuItem.Text = "Основная";
            SelectedTask = TaskType.Main;
            this.chart1.Series.Clear();
        }

        private double TrueSoluitonFunction(double X0, double U0)
        {
            return 3;
        }

        private TaskType SelectedTask = TaskType.NotSelected;

        Dictionary<Tuple<double, double>, List<Series>> SeriesForStartConditions;

    }
}
