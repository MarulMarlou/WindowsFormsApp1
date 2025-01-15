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
    public partial class Suppl : Form
    {
        public Suppl()
        {
            InitializeComponent();
            LoadData();
        }

        private void Suppl_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "aesDataSet.vw_Supplies". При необходимости она может быть перемещена или удалена.
            this.vw_SuppliesTableAdapter.Fill(this.aesDataSet.vw_Supplies);

        }

        private string connectionString = "Data Source=DESKTOP-Q7010U6\\SQLEXPRESS;Initial Catalog=aes;Integrated Security=True";

        private void LoadData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM vw_Supplies", connection);
                DataTable suppliesTable = new DataTable();
                adapter.Fill(suppliesTable);
                vw_SuppliesDataGridView.DataSource = suppliesTable;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                    !int.TryParse(textBox2.Text, out int quantity) ||
                    !decimal.TryParse(textBox3.Text, out decimal unitPrice))
                {
                    MessageBox.Show("Пожалуйста, заполните все поля корректно.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Supplies (SupplyName, Quantity, UnitPrice) VALUES (@name, @quantity, @unitPrice)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@name", textBox1.Text);
                    command.Parameters.AddWithValue("@quantity", Convert.ToInt32(textBox2.Text));
                    command.Parameters.AddWithValue("@unitPrice", Convert.ToDecimal(textBox3.Text));

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();

                }

                LoadData(); // Обновляем данные после добавления
            }
                 catch (SqlException ex)
            {
                MessageBox.Show("Ошибка при добавлении запаса: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (vw_SuppliesDataGridView.SelectedRows.Count > 0)
            {
                int selectedRowIndex = vw_SuppliesDataGridView.SelectedRows[0].Index;
                int supplyID = Convert.ToInt32(vw_SuppliesDataGridView.Rows[selectedRowIndex].Cells[0].Value);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Supplies WHERE SupplyID = @id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id", supplyID);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();

                    LoadData(); // Обновляем данные после удаления
                }
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            (vw_SuppliesDataGridView.DataSource as DataTable).DefaultView.RowFilter =
        string.Format("SupplyName LIKE '%{0}%' OR Quantity LIKE '%{0}%' OR UnitPrice LIKE '%{0}%'", textBox4.Text);
        }
    }
}
