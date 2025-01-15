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
    public partial class Equip : Form
    {
        public Equip()
        {
            InitializeComponent();
            LoadData();
        }

        private void Equip_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "aesDataSet.vw_Equipment". При необходимости она может быть перемещена или удалена.
            this.vw_EquipmentTableAdapter.Fill(this.aesDataSet.vw_Equipment);

        }

        private string connectionString = "Data Source=DESKTOP-Q7010U6\\SQLEXPRESS;Initial Catalog=aes;Integrated Security=True";

        private void LoadData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM vw_Equipment", connection);
                DataTable equipmentTable = new DataTable();
                adapter.Fill(equipmentTable);
                vw_EquipmentDataGridView.DataSource = equipmentTable;
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                    string.IsNullOrWhiteSpace(textBox2.Text) ||
                    string.IsNullOrWhiteSpace(textBox3.Text))
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Equipment (EquipmentName, EquipmentType, Status, LastMaintenanceDate, NextMaintenanceDate) VALUES (@name, @type, @status, @lastDate, @nextDate)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@name", textBox1.Text);
                command.Parameters.AddWithValue("@type", textBox2.Text);
                command.Parameters.AddWithValue("@status", textBox3.Text);
                command.Parameters.AddWithValue("@lastDate", dateTimePicker1.Value);
                command.Parameters.AddWithValue("@nextDate", dateTimePicker2.Value);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

                LoadData(); // Обновляем данные после добавления
            }

            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка при добавлении оборудования: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (vw_EquipmentDataGridView.SelectedRows.Count > 0)
            {
                int selectedRowIndex = vw_EquipmentDataGridView.SelectedRows[0].Index;
                int equipmentID = Convert.ToInt32(vw_EquipmentDataGridView.Rows[selectedRowIndex].Cells[0].Value);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Equipment WHERE EquipmentID = @id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id", equipmentID);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();

                    LoadData(); // Обновляем данные после удаления
                }
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            (vw_EquipmentDataGridView.DataSource as DataTable).DefaultView.RowFilter =
        string.Format("EquipmentName LIKE '%{0}%' OR EquipmentType LIKE '%{0}%'", textBox4.Text);
        }
    }
}
