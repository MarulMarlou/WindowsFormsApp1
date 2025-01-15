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
    public partial class MaintenShed : Form
    {
        public MaintenShed()
        {
            InitializeComponent();
            LoadEquipment(); 
            LoadData();
        }

        private void MaintenShed_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "aesDataSet.vw_MaintenanceSchedules". При необходимости она может быть перемещена или удалена.
            this.vw_MaintenanceSchedulesTableAdapter.Fill(this.aesDataSet.vw_MaintenanceSchedules);

        }

        private string connectionString = "Data Source=DESKTOP-Q7010U6\\SQLEXPRESS;Initial Catalog=aes;Integrated Security=True";

        private void LoadData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM vw_MaintenanceSchedules", connection);
                DataTable maintenanceSchedulesTable = new DataTable();
                adapter.Fill(maintenanceSchedulesTable);
                vw_MaintenanceSchedulesDataGridView.DataSource = maintenanceSchedulesTable;
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
                    string.IsNullOrWhiteSpace(textBox2.Text))
                {
                    MessageBox.Show("Пожалуйста, заполните все поля корректно.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO MaintenanceSchedules (EquipmentID, ScheduledDate, MaintenanceType, Frequency) VALUES (@equipmentID, @scheduledDate, @maintenanceType, @frequency)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@equipmentID", comboBox1.SelectedValue);
                    command.Parameters.AddWithValue("@scheduledDate", dateTimePicker1.Value);
                    command.Parameters.AddWithValue("@maintenanceType", textBox1.Text);
                    command.Parameters.AddWithValue("@frequency", textBox2.Text);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();

                }

                LoadData(); // Обновляем данные после добавления
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка при добавлении графика обслуживания: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (vw_MaintenanceSchedulesDataGridView.SelectedRows.Count > 0)
            {
                int selectedRowIndex = vw_MaintenanceSchedulesDataGridView.SelectedRows[0].Index;
                int scheduleID = Convert.ToInt32(vw_MaintenanceSchedulesDataGridView.Rows[selectedRowIndex].Cells[0].Value);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM MaintenanceSchedules WHERE ScheduleID = @id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id", scheduleID);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();

                    LoadData(); // Обновляем данные после удаления
                }
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            (vw_MaintenanceSchedulesDataGridView.DataSource as DataTable).DefaultView.RowFilter =
        string.Format("MaintenanceType LIKE '%{0}%' OR Frequency LIKE '%{0}%'", textBox4.Text);
        }
    }
}
