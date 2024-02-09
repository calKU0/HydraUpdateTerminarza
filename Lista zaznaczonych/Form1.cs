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
using static System.Windows.Forms.AxHost;

namespace UpdateTerminRealizacjiTerminarz
{
    public partial class TerminRealizacjiForm : Form
    {
        private readonly long ZadId;
        private readonly string ConnectionString;
        private DataTable dt { get; set; }
        public TerminRealizacjiForm(long ZadId, string ConnectionString)
        {
            InitializeComponent();
            this.ZadId = ZadId;
            this.ConnectionString = ConnectionString;
        }

        private void TerminRealizacjiForm_Load(object sender, EventArgs e)
        {
            try
            {
                using(SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    string query = "Select Dateadd(Second, zad_TerminOd, '1990-01-01') as [Termind Od], Dateadd(Second, zad_TerminDo, '1990-01-01') as [Termin Do] from cdn.Zadania where Zad_ID = @ZadID";
                    SqlCommand command = new SqlCommand(query, conn);
                    command.Parameters.AddWithValue("@ZadID", ZadId);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    
                    dt = new DataTable();
                    adapter.Fill(dt);
                }
                dataGridView1.DataSource = dt;
                dataGridView1.Refresh();
                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime TerminOd;
                DateTime TerminDo;
                if (DateTime.TryParse(dataGridView1.Rows[0].Cells[0].Value.ToString(), out TerminOd) && DateTime.TryParse(dataGridView1.Rows[0].Cells[1].Value.ToString(), out TerminDo))
                {
                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    {
                        conn.Open();
                        string query = "UPDATE CDN.Zadania SET Zad_TerminOd = DATEDIFF(SECOND, '1990-01-01', @TerminOd), Zad_TerminDo = DATEDIFF(SECOND, '1990-01-01', @TerminDo) WHERE Zad_ID = @ZadID";
                        SqlCommand command = new SqlCommand(query, conn);
                        command.Parameters.AddWithValue("@ZadID", ZadId);
                        command.Parameters.AddWithValue("@TerminOd", dataGridView1.Rows[0].Cells[0].Value);
                        command.Parameters.AddWithValue("@TerminDo", dataGridView1.Rows[0].Cells[1].Value);
                        int wynik = command.ExecuteNonQuery();

                        if (wynik == 1)
                        {
                            MessageBox.Show("Zaktualizowano", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Nie zaktualizowano", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Błędny format daty", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DateTime Termin;
            if (!DateTime.TryParse(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out Termin))
            {
                MessageBox.Show("Błędny format daty", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dataGridView1.Rows[0].Cells[0].Value = string.Empty;
            }
        }
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs anError)
        {
            MessageBox.Show("Błędny format daty", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            dataGridView1.Rows[0].Cells[0].Value = string.Empty;
        }
    }
}
