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
    public partial class Repa : Form
    {
        public Repa()
        {
            InitializeComponent();
            LoadEquipment();
            LoadData();
        }

        private void Repa_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "aesDataSet.vw_Repairs". При необходимости она может быть перемещена или удалена.
            this.vw_RepairsTableAdapter.Fill(this.aesDataSet.vw_Repairs);

        }

        private string connectionString = "Data Source=DESKTOP-Q7010U6\\SQLEXPRESS;Initial Catalog=aes;Integrated Security=True";

        private void LoadData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM vw_Repairs", connection);
                DataTable repairsTable = new DataTable();
                adapter.Fill(repairsTable);
                vw_RepairsDataGridView.DataSource = repairsTable;
            }
        }

        private void LoadEquipment()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT EquipmentID, EquipmentName FROM Equipment", connection);
                DataTable equipmentTable = new DataTable();
                adapter.Fill(equipmentTable);

                comboBox1.DataSource = equipmentTable;
                comboBox1.DisplayMember = "EquipmentName";
                comboBox1.ValueMember = "EquipmentID";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedValue == null ||
                    string.IsNullOrWhiteSpace(textBox1.Text) ||
                    !decimal.TryParse(textBox2.Text, out decimal cost))
                {
                    MessageBox.Show("Пожалуйста, заполните все поля корректно.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Repairs (EquipmentID, RepairDate, RepairType, Cost) VALUES (@equipmentID, @repairDate, @repairType, @cost)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@equipmentID", comboBox1.SelectedValue);
                    command.Parameters.AddWithValue("@repairDate", dateTimePicker1.Value);
                    command.Parameters.AddWithValue("@repairType", textBox1.Text);
                    command.Parameters.AddWithValue("@cost", Convert.ToDecimal(textBox2.Text));

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();

                }

                LoadData(); // Обновляем данные после добавления
            }
                catch (SqlException ex)
            {
                MessageBox.Show("Ошибка при добавлении ремонта: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (vw_RepairsDataGridView.SelectedRows.Count > 0)
            {
                int selectedRowIndex = vw_RepairsDataGridView.SelectedRows[0].Index;
                int repairID = Convert.ToInt32(vw_RepairsDataGridView.Rows[selectedRowIndex].Cells[0].Value);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Repairs WHERE RepairID = @id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id", repairID);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();

                    LoadData(); // Обновляем данные после удаления
                }
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            (vw_RepairsDataGridView.DataSource as DataTable).DefaultView.RowFilter =
        string.Format("RepairType LIKE '%{0}%' OR Cost LIKE '%{0}%'", textBox4.Text);
        }
    }
}
