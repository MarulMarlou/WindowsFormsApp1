using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Empl : Form
    {
        public Empl()
        {
            InitializeComponent();
            LoadData();
        }

        private void Empl_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "aesDataSet.vw_Employees". При необходимости она может быть перемещена или удалена.
            this.vw_EmployeesTableAdapter.Fill(this.aesDataSet.vw_Employees);

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private string connectionString = "Data Source=DESKTOP-Q7010U6\\SQLEXPRESS;Initial Catalog=aes;Integrated Security=True";

        private void LoadData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM vw_Employees", connection);
                DataTable employeesTable = new DataTable();
                adapter.Fill(employeesTable);
                vw_EmployeesDataGridView.DataSource = employeesTable;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                     string.IsNullOrWhiteSpace(textBox2.Text))
                {
                    MessageBox.Show("Пожалуйста заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Employees (EmployeeName, Position) VALUES (@name, @position)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@name", textBox1.Text);
                command.Parameters.AddWithValue("@position", textBox2.Text);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

                LoadData(); // Обновляем данные после добавления
            }
        }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка при добавлении сотрудника: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (vw_EmployeesDataGridView.SelectedRows.Count > 0)
            {
                int selectedRowIndex = vw_EmployeesDataGridView.SelectedRows[0].Index;
                int employeeID = Convert.ToInt32(vw_EmployeesDataGridView.Rows[selectedRowIndex].Cells[0].Value);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Employees WHERE EmployeeID = @id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id", employeeID);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();

                    LoadData(); 
                }
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            (vw_EmployeesDataGridView.DataSource as DataTable).DefaultView.RowFilter =
        string.Format("EmployeeName LIKE '%{0}%' OR Position LIKE '%{0}%'", textBox4.Text);
        }
    }
}
