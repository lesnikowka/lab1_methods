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
using System.Diagnostics;


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
        private DataBase db = null;
        private DataTable table = new DataTable();
        private DataTable table2 = new DataTable();

        private List<string> columnNames = new List<string> { "id", "xi", "vi", "v2i", "vi-v2i", "loc_prec", "hi", "c1", "c2", "u", "u-v" };
        private List<string> columnNamesForDerivative = new List<string> { "id", "xi", "u'i", "v2i", "vi-v2i", "loc_prec", "hi", "c1", "c2" };

        private Dictionary<Tuple<double, double>, List<Series>> SeriesForStartConditions = new Dictionary<Tuple<double, double>, List<Series>>();

        private TaskType selectedTask = TaskType.Main1;
        private string currentTableDB = "main1";

        public Form1()
        {
            InitializeComponent();

            chart1.ChartAreas["ChartArea1"].BackColor = Color.Transparent;
            chart2.ChartAreas["ChartArea1"].BackColor = Color.Transparent;
            chart3.ChartAreas["ChartArea1"].BackColor = Color.Transparent;

            
        }

        private int catchParams(ref double X0, ref double U0, ref double startStep, ref double localPrecision, ref double boundPrecision, ref double integrationBound, ref int maxStepNumbers, ref bool withControl, ref double a, ref double b, ref double c)
        {
            string x0Text = textBox1.Text;
            string u0Text = textBox2.Text;
            string startStepText = textBox3.Text;
            string localPrecisionText = textBox4.Text;
            string boundPrecisionText = textBox5.Text;
            string maxStepNumbersText = textBox6.Text;
            string integrationBoundText = textBox7.Text;
            string aText = textBox8.Text;
            string bText = textBox9.Text;
            string cText = textBox10.Text;

            if (x0Text.Length == 0 || u0Text.Length == 0 ||
                startStepText.Length == 0 || localPrecisionText.Length == 0 ||
                boundPrecisionText.Length == 0 || maxStepNumbersText.Length == 0 ||
                integrationBoundText.Length == 0 || aText.Length == 0 || bText.Length == 0 || 
                cText.Length == 0
                )
            {
                MessageBox.Show("Вы не ввели начальные условия", "Ошибка");
                return -1;
            }

            pointsToCommas(ref x0Text);
            pointsToCommas(ref u0Text);
            pointsToCommas(ref startStepText);
            pointsToCommas(ref localPrecisionText);
            pointsToCommas(ref boundPrecisionText);
            pointsToCommas(ref integrationBoundText);
            pointsToCommas(ref aText);
            pointsToCommas(ref bText);
            pointsToCommas(ref cText);

            try
            {
                X0 = Convert.ToDouble(x0Text);
                U0 = Convert.ToDouble(u0Text);
                startStep = Convert.ToDouble(startStepText);
                localPrecision = Convert.ToDouble(boundPrecisionText);
                boundPrecision = Convert.ToDouble(boundPrecisionText);
                maxStepNumbers = Convert.ToInt32(maxStepNumbersText);
                integrationBound = Convert.ToDouble(integrationBoundText);
                a = Convert.ToDouble(aText);
                b = Convert.ToDouble(bText);
                c = Convert.ToDouble(cText);
            }
            catch
            {
                MessageBox.Show("Неверный формат введенных значений", "Ошибка");
                return -1;
            }

            if (X0 + startStep > integrationBound)
            {
                MessageBox.Show("Некорректная граница интегрирования", "Ошибка");
                return -1;
            }

            withControl = checkBox1.Checked;

            return 0;
        }
        
        private void executeMethod()
        {
            double X0 = 0, U0 = 0, startStep = 0, localPrecision = 0, boundPrecision = 0, integrationBound = 0, a = 0, b = 0, c = 0;
            int maxStepNumbers = 0;
            bool withControl = true;

            string tableName;
            if (selectedTask == TaskType.Test) tableName = "test";
            else if (selectedTask == TaskType.Main1) tableName = "main1";
            else tableName = "main2";

            int valid = catchParams(ref X0, ref U0, ref startStep, ref localPrecision, ref boundPrecision, ref integrationBound, ref maxStepNumbers, ref withControl, ref a, ref b, ref c);

            if (valid != 0)
            {
                return;
            }

            string args = "";

            args += X0.ToString().Replace(",", ".") + " ";
            args += U0.ToString().Replace(",", ".") + " ";
            args += startStep.ToString().Replace(",", ".") + " ";
            args += maxStepNumbers.ToString() + " ";
            args += localPrecision.ToString().Replace(",", ".") + " ";
            args += boundPrecision.ToString().Replace(",", ".") + " ";
            args += (withControl ? 1 : 0).ToString() +  " ";
            args += a.ToString().Replace(",", ".") + " ";
            args += b.ToString().Replace(",", ".") + " ";
            args += c.ToString().Replace(",", ".") + " ";
            args += tableName + " ";
            args += integrationBound.ToString().Replace(",", ".") + " ";

            //args += " > C:\\sqlite\\log222.txt";


            ProcessStartInfo infoStartProcess = new ProcessStartInfo();

            Process methodProcess = new Process();

            infoStartProcess.WorkingDirectory = "C:\\Users\\lesni\\lab1_methods\\script";
            infoStartProcess.FileName = "main.py";
            infoStartProcess.Arguments = args;

            methodProcess.StartInfo = infoStartProcess;

            methodProcess.Start();

            methodProcess.WaitForExit();

            ShowDataForStartCondition(new List<string> { X0.ToString().Replace(",", "."), U0.ToString().Replace(",", ".") });

            drawGraphs(X0, U0);

            showStartConditions(tableName);


            // exec python 
            // show data for st cond (x0, u0)
            // draw graphs
        }

        private void drawGraphs(double X0, double U0)
        {
            Tuple<double, double> x0u0Tuple = new Tuple<double, double>(X0, U0);
            deleteOldSeries(X0, U0);

            List<Series> newSeriesList = new List<Series>();


            Series newNumericSeries = new Series();
            newNumericSeries.Name = "Численное решение при X0 = " + X0.ToString() + " U0 = " + U0.ToString();
            newNumericSeries.ChartType = SeriesChartType.Line;
            newNumericSeries.BorderWidth = 2;
            this.chart1.Series.Add(newNumericSeries);
            newSeriesList.Add(newNumericSeries);


            if (selectedTask == TaskType.Test)
            {
                Series newTrueSeries = new Series();
                newTrueSeries.Name = "Истинное решение при X0 = " + X0.ToString() + " U0 = " + U0.ToString();
                newTrueSeries.ChartType = SeriesChartType.Line;
                newTrueSeries.BorderWidth = 2;
                this.chart1.Series.Add(newTrueSeries);
                newSeriesList.Add(newTrueSeries);

                DrawTrueSolution(newTrueSeries, X0, U0, 0.1); // написать нормально 
            }

            if (selectedTask == TaskType.Test || selectedTask == TaskType.Main1)
            {
                DrawNumericSolution(newNumericSeries, null, null, X0, U0);
            }
            else
            {
                Series newDerivativeSeries = new Series();
                newDerivativeSeries.Name = "Производная при X0 = " + X0.ToString() + " U0 = " + U0.ToString();
                newDerivativeSeries.ChartType = SeriesChartType.Line;
                newDerivativeSeries.BorderWidth = 2;
                this.chart3.Series.Add(newDerivativeSeries);
                newSeriesList.Add(newDerivativeSeries);


                Series newPhaseSeries = new Series();
                newPhaseSeries.Name = "Фазовая траектория при X0 = " + X0.ToString() + " U0 = " + U0.ToString();
                newPhaseSeries.ChartType = SeriesChartType.Line;
                newPhaseSeries.BorderWidth = 2;
                this.chart2.Series.Add(newPhaseSeries);
                newSeriesList.Add(newPhaseSeries);

                DrawNumericSolution(newNumericSeries, newDerivativeSeries, newPhaseSeries, X0, U0);
            }

            SeriesForStartConditions.Add(x0u0Tuple, newSeriesList);

        }

        private void deleteOldSeries(double X0, double U0)
        {

            Tuple<double, double> x0u0Tuple = new Tuple<double, double>(X0, U0);

            if (SeriesForStartConditions.ContainsKey(x0u0Tuple))
            {

                List<Series> oldSeries = SeriesForStartConditions[x0u0Tuple];

                for (int i = 0; i < oldSeries.Count; i++)
                {
                    this.chart1.Series.Remove(oldSeries[i]);
                    this.chart2.Series.Remove(oldSeries[i]);
                    this.chart3.Series.Remove(oldSeries[i]);
                }

                SeriesForStartConditions.Remove(x0u0Tuple);

            }
        }

        private void pointsToCommas(ref string s)
        {
            s = s.Replace('.', ',');
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

        double constant(double x0, double v0)
        {
            return v0 / Math.Exp(2 * x0);
        }

        private double TrueSoluitonFunction(double x0, double v0, double x)
        {
            return Math.Exp(2 * x) * constant(x0, v0);
        }

        private void DrawTrueSolution(Series series, double x0, double u0, double h)
        {

            //for (int i = 0; i < 5; i++)
            //{
            //    series.Points.AddXY(i, TrueSoluitonFunction(x0, u0, i));
            //}
        }

        private void DrawNumericSolution(Series mainSeries, Series derSeries, Series phaseSeries, double X0, double U0)
        {
            string tableName;
            if (selectedTask == TaskType.Test) tableName = "test";
            else if (selectedTask == TaskType.Main1) tableName = "main1";
            else tableName = "main2";

            List<List<string>> data = db.GetDataForStartCondition(tableName, new List<string> { X0.ToString(), U0.ToString() });

            if (selectedTask == TaskType.Test || selectedTask == TaskType.Main1)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    mainSeries.Points.AddXY(Convert.ToDouble(data[i][1].Replace(".", ",")), Convert.ToDouble(data[i][2].Replace(".", ",")));
                }
            }
            else
            {
                for (int i = 0; i < data.Count; i++)
                {
                    mainSeries.Points.AddXY(Convert.ToDouble(data[i][2].Replace(".", ",")), Convert.ToDouble(data[i][3].Replace(".", ",")));
                }

                for (int i = 0; i < data.Count; i++)
                {
                    derSeries.Points.AddXY(Convert.ToDouble(data[i][2].Replace(".", ",")), Convert.ToDouble(data[i][0].Replace(".", ",")));
                }

                for (int i = 0; i < data.Count; i++)
                {
                    phaseSeries.Points.AddXY(Convert.ToDouble(data[i][3].Replace(".", ",")), Convert.ToDouble(data[i][0].Replace(".", ",")));
                }
            }
        }

        private void ShowDataForStartCondition(List<string> startCondition)
        {
            string tableName;
            if (selectedTask == TaskType.Test) tableName = "test";
            else if (selectedTask == TaskType.Main1) tableName = "main1";
            else tableName = "main2";

            List<List<string>> dataForStartCondition = db.GetDataForStartCondition(tableName, startCondition);

            table.Rows.Clear();
            table2.Rows.Clear();

            int columnNamesSize = tableName == "test" ? columnNames.Count : columnNames.Count - 2;

            for (int i = 0; i < dataForStartCondition.Count; i++)
            {
                DataRow row = table.NewRow();

                for (int j = 0; j < columnNamesSize; j++)
                {
                    int j_index = tableName == "main2" ? j + 1 : j;

                    row[columnNames[j]] = dataForStartCondition[i][j_index];
                }

                table.Rows.Add(row);
            }
        }

        List<string> stringConditionToList(string startConditionString)
        {
            List<string> startCondition = new List<string>();

            int commaIndex = startConditionString.IndexOf(',');

            startCondition.Add(startConditionString.Substring(0, commaIndex));
            startCondition.Add(startConditionString.Substring(commaIndex + 2));

            return startCondition;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.chart1.Series.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                db = new DataBase("/../../../database/lab1.sqlite3");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }


            for (int i = 0; i < columnNames.Count - 2; i++)
            {
                table.Columns.Add(columnNames[i], typeof(string));
            }

            for (int i = 0; i < columnNamesForDerivative.Count; i++)
            {
                table2.Columns.Add(columnNamesForDerivative[i], typeof(string));
            }

            dataGridView1.DataSource = table;
            dataGridView2.DataSource = table2;

            showStartConditions("main1");

            textBox8.Enabled = false;
            textBox9.Enabled = false;
            textBox10.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            executeMethod();
        }

        private void тестоваяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            выборТипаЗадачиToolStripMenuItem.Text = "Тестовая";
            selectedTask = TaskType.Test;
            this.chart1.Series.Clear();
            this.chart2.Series.Clear();
            this.chart3.Series.Clear();

            if (!table.Columns.Contains("u") && !table.Columns.Contains("u-v"))
            {
                table.Columns.Add("u", typeof(string));
                table.Columns.Add("u-v", typeof(string));
            }

            table.Rows.Clear();

            showStartConditions("test");

            currentTableDB = "test";

            table.Rows.Clear();

            textBox8.Enabled = false;
            textBox9.Enabled = false;
            textBox10.Enabled = false;
        }

        private void основнаяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            выборТипаЗадачиToolStripMenuItem.Text = "Основная 1";
            selectedTask = TaskType.Main1;
            this.chart1.Series.Clear();
            this.chart2.Series.Clear();
            this.chart3.Series.Clear();

            if (table.Columns.Contains("u") && table.Columns.Contains("u-v"))
            {
                table.Columns.Remove("u");
                table.Columns.Remove("u-v");
            }


            table.Rows.Clear();

            showStartConditions("main1");

            currentTableDB = "main1";

            textBox8.Enabled = false;
            textBox9.Enabled = false;
            textBox10.Enabled = false;

        }
        private void основная2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            выборТипаЗадачиToolStripMenuItem.Text = "Основная 2";
            selectedTask = TaskType.Main2;
            this.chart1.Series.Clear();
            this.chart2.Series.Clear();
            this.chart3.Series.Clear();

            if (table.Columns.Contains("u") && table.Columns.Contains("u-v"))
            {
                table.Columns.Remove("u");
                table.Columns.Remove("u-v");
            }


            table.Rows.Clear();

            showStartConditions("main2");

            currentTableDB = "main2";

            textBox8.Enabled = true;
            textBox9.Enabled = true;
            textBox10.Enabled = true;
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            List<string> selectedCondition = stringConditionToList(comboBox1.GetItemText(comboBox1.SelectedItem));

            ShowDataForStartCondition(selectedCondition);

            drawGraphs(Convert.ToDouble(selectedCondition[0].Replace(".", ",")), Convert.ToDouble(selectedCondition[1].Replace(".", ",")));
        }
    }
}
