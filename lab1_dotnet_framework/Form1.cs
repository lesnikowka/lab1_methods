﻿using System;
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
            if (SelectedTask == TaskType.NotSelected)
            {
                MessageBox.Show("Вы не выбрали задачу", "Ошибка");
                return;
            }

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

            SeriesForStartConditions.Add(x0u0Tuple, newSeriesList);

            newNumericSeries.Name = "Численное решение при X0 = " + X0.ToString() + " U0 = " + U0.ToString();


            newNumericSeries.ChartType = SeriesChartType.Line;

            newNumericSeries.Points.Add(X0, U0);
            newNumericSeries.Points.Add(2*X0, 2*U0);

            this.chart1.Series.Add(newNumericSeries);

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

        private TaskType SelectedTask = TaskType.Main;

        Dictionary<Tuple<double, double>, List<Series>> SeriesForStartConditions = new Dictionary<Tuple<double, double>, List<Series>>();

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
