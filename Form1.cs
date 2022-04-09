using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;

namespace DT1
{
    public partial class Form1 : Form
    {
        private SqlConnection Connection = null;

        public Form1()
        {
            InitializeComponent();
            notifyIcon1.Visible = false;
            this.Resize += new EventHandler(Form1_Resize);
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e) // если гланая форма развернута, то иконка в трее не видна, иконка в панели видна
        {
            notifyIcon1.Visible = false;
            this.ShowInTaskbar = true;
            WindowState = FormWindowState.Normal;
        }

        private void Form1_Resize(object sender, EventArgs e) // если гавная форма свернута,то ее иконка имчезает из панели и появлятся иконка в трее
        {
            if(WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                notifyIcon1.Visible = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e) // события при загрузке формы
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "database1DataSet1.Postpone". При необходимости она может быть перемещена или удалена.
            this.postponeTableAdapter.Fill(this.database1DataSet1.Postpone);
            Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Database1"].ConnectionString);
            Connection.Open();

            dataMyContent();
            dataOnFuture();
        }

        private void button1_Click(object sender, EventArgs e) // кнопка записать в "мой контент"
        {
            SqlCommand Add = new SqlCommand(String.Format("INSERT INTO [Content] (Name, Category, Genre, Autor, CreationYear, Rating, [Check]) VALUES (N'{0}',N'{1}',N'{2}',N'{3}',N'{4}',N'{5}',N'{6}')", 
                                                                                                                                         textBoxWatermark1.Text, comboBox1.Text, 
                                                                                                                                         textBoxWatermark2.Text, textBoxWatermark3.Text, 
                                                                                                                                         textBoxWatermark4.Text, textBoxWatermark5.Text, checkBox1.Checked ? 1 : 0), Connection);
            Add.ExecuteNonQuery();
            textBoxWatermark1.Clear();
            comboBox1.SelectedIndex = -1;
            textBoxWatermark2.Clear();
            textBoxWatermark3.Clear();
            textBoxWatermark4.Clear();
            textBoxWatermark5.Clear();
            checkBox1.Checked = false;
            dataMyContent();
            colorRow(dataGridView1);
        }

        private void textBoxWatermark6_TextChanged(object sender, EventArgs e) // фильтр по имени
        {
            filters("Name",textBoxWatermark6.Text);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) // фильтр по категории
        {
            filters("Category", comboBox2.Text);
        }

        private void textBoxWatermark7_TextChanged(object sender, EventArgs e) // филтр по жанру
        {
            filters("Genre", textBoxWatermark7.Text);
        }

        private void textBoxWatermark8_TextChanged(object sender, EventArgs e) // фильтр по автору
        {
            filters("Autor", textBoxWatermark8.Text);
        }

        private void textBoxWatermark9_TextChanged(object sender, EventArgs e) // фильтр по году выпуска
        {
            filters("CreationYear", textBoxWatermark9.Text);
        }

        private void textBoxWatermark10_TextChanged(object sender, EventArgs e) // фильтр по рейтингу
        {
            filters("Convert(Rating, System.String)", textBoxWatermark10.Text);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) //когда форма закрывается
        {
            Connection.Close();
        }

        private void button4_Click(object sender, EventArgs e) //нажатие на кнопку "Запомнить"
        {
            SqlCommand Add = new SqlCommand(String.Format("INSERT INTO [Postpone] (Name, Autor, CreationYear) VALUES (N'{0}',N'{1}',N'{2}')", textBoxWatermark11.Text,
                                                                                                       textBoxWatermark12.Text, textBoxWatermark13.Text), Connection);

            Add.ExecuteNonQuery();
            textBoxWatermark11.Clear();
            textBoxWatermark12.Clear();
            textBoxWatermark13.Clear();
            dataOnFuture();
        }

        private void button5_Click(object sender, EventArgs e) // кнопка удалить в "На будущее"
        {
            removeRow(dataGridView2, "Postpone");
        }

        private void button6_Click(object sender, EventArgs e) // кнопка удалить в "Мой контент"
        {
            removeRow(dataGridView1, "Content");
        }

        private void textBoxWatermark5_KeyPress(object sender, KeyPressEventArgs e) // только цифры в оценке
        {
            onlyNumbers(e);
        }

        private void textBoxWatermark4_KeyPress(object sender, KeyPressEventArgs e) // только цифры в году
        {
            onlyNumbers(e);
        }

        private void textBoxWatermark10_KeyPress(object sender, KeyPressEventArgs e) // только цифры
        {
            onlyNumbers(e);
        }

        private void textBoxWatermark9_KeyPress(object sender, KeyPressEventArgs e) // только цифры
        {
            onlyNumbers(e);
        }

        private void textBoxWatermark13_KeyPress(object sender, KeyPressEventArgs e) // только цифры
        {
            onlyNumbers(e);
        }

        private void Form1_Activated(object sender, EventArgs e) //при активации формы
        {
            colorRow(dataGridView1);
        }

        private void Form1_Shown(object sender, EventArgs e) //при первой видимости формы
        {
            colorRow(dataGridView1);
        }
        #region HelperMethods
        private void dataOnFuture() // заполнение Grid в "На будущее"
        {
            SqlDataAdapter dataAdapter2 = new SqlDataAdapter("SELECT ID, Name, Autor, CreationYear FROM Postpone", Connection);
            DataSet dataSet2 = new DataSet();
            dataAdapter2.Fill(dataSet2);
            dataGridView2.DataSource = dataSet2.Tables[0];
            dataGridView2.Columns["ID"].Visible = false;
        }

        private void dataMyContent() // заполнение Grid в "Мой контент"
        {
            SqlDataAdapter dataAdapter1 = new SqlDataAdapter("SELECT ID, Name, Category, Genre, Autor, CreationYear, Rating, [Check] FROM Content", Connection); // отображение всех данных в гриде
            DataSet dataSet1 = new DataSet();
            dataAdapter1.Fill(dataSet1);
            dataGridView1.DataSource = dataSet1.Tables[0];
            dataGridView1.Columns["ID"].Visible = false;
        }

        private void removeRow(DataGridView grid, string table) //удаление строки из grid и БД
        {
            try
            {
                int row = (Int32)grid.SelectedRows[0].Cells[0].Value;
                grid.Rows.RemoveAt(grid.SelectedRows[0].Index);
                SqlCommand Del = new SqlCommand(String.Format("DELETE FROM {0} WHERE Id = @RowId", table), Connection);
                Del.Parameters.AddWithValue("@RowId", row);
                Del.ExecuteNonQuery();
            }
            catch(ArgumentOutOfRangeException e)
            {
                MessageBox.Show("Выделите всю строку");
            }
        }

        private void onlyNumbers(KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8) // цифры и клавиша BackSpace
            {
                e.Handled = true;
            }
        }

        private void filters(string sqlName, string field) // фильтрация первого grid
        {
            (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = $"{sqlName} LIKE '%{field}%'";
        }

        private void colorRow(DataGridView grid) // окрашивает строку в гриде если этот контент надо пересмотреть
        {
            foreach(DataGridViewRow i in grid.Rows)
            {
                if (i.Cells[7].Value is int)
                {
                    if (Convert.ToInt32(i.Cells[7].Value) == 1)
                    {
                        i.DefaultCellStyle.BackColor = Color.Aqua;
                    }
                    else i.DefaultCellStyle.BackColor = Color.Empty;
                }
                
            }
        }
        #endregion
    }
}
