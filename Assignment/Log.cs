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

namespace Assignment25
{
    public partial class Log : Form
    {
        SqlConnection con = new SqlConnection("Data Source=DESKTOP-LQ48K27;Initial Catalog=Assignment25;Integrated Security=true;");
        SqlCommand cmd;
        SqlDataAdapter adapt;
        String staffID = "";
        String customerID = "";
        Boolean isUpdate = false;

        public Log()
        {
            InitializeComponent();
            generateID();
            loadForm();
        }

        private void generateID()
        {
            con.Open();
            string sqlQuery = "SELECT TOP 1 ID from tblLog order by ID desc";
            SqlCommand cmd = new SqlCommand(sqlQuery, con);
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                string input = dr["ID"].ToString();
                string angka = input.Substring(1, 4);
                int number = Convert.ToInt32(angka);
                number += 1;
                string str = number.ToString("D4");

                txtID.Text = "L" + str;
            }
            else
            {
                txtID.Text = "L0001";
            }
            con.Close();
        }

        private void loadForm()
        {
            dtpDate.CustomFormat = "dd/MM/yyyy";
            dtpT1.CustomFormat = "HH:mm:ss tt";
            dtpT2.CustomFormat = "HH:mm:ss tt";
            loadCustomer();
            loadStaff();
            DisplayData();
            calDurtion();
        }

        private void calDurtion()
        {
            TimeSpan duration = new TimeSpan(Math.Abs(dtpT2.Value.Ticks - dtpT1.Value.Ticks));
            txtDuration.Text = duration.Hours.ToString() + ":" + duration.Minutes.ToString() + ":" + duration.Seconds.ToString();
        }

        public class ComboboxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        private void loadCustomer()
        {
            con.Open();
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from Customer";
            cmd.ExecuteNonQuery();
            con.Close();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            foreach (DataRow dr in dt.Rows)
            {
                ComboboxItem item = new ComboboxItem();
                item.Text = dr["Name"].ToString();
                item.Value = dr["ID"].ToString();
                cboCustomer.Items.Add(item);
                //cboCustomer.Items.Add(dr["Name"].ToString());
            }
        }

        private void loadStaff()
        {
            con.Open();
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from Staff";
            cmd.ExecuteNonQuery();
            con.Close();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            foreach (DataRow dr in dt.Rows)
            {
                ComboboxItem item = new ComboboxItem();
                item.Text = dr["Name"].ToString();
                item.Value = dr["ID"].ToString();
                cboStaff.Items.Add(item);
            }
        }

        private void dtpT1_ValueChanged(object sender, EventArgs e)
        {
            TimeSpan duration = new TimeSpan(Math.Abs(dtpT2.Value.Ticks - dtpT1.Value.Ticks));
            txtDuration.Text = duration.Hours.ToString() + ":" + duration.Minutes.ToString() + ":" + duration.Seconds.ToString();
        }

        private void dtpT2_ValueChanged(object sender, EventArgs e)
        {
            TimeSpan duration = new TimeSpan(Math.Abs(dtpT2.Value.Ticks - dtpT1.Value.Ticks));
            txtDuration.Text = duration.Hours.ToString() + ":" + duration.Minutes.ToString() + ":" + duration.Seconds.ToString();
        }

        private void DisplayData()
        {
            con.Open();
            DataTable dt = new DataTable();
            adapt = new SqlDataAdapter("select l.ID, l.lDate Date, l.StartTime, l.EndTime, l.Duration, s.Name Staff, c.Name Customer, l.Remark from tblLog l "
                                       + " inner join Customer c on l.CustomerID=c.ID "
                                       + " inner join Staff s on l.StaffID=s.ID", con);
            adapt.Fill(dt);
            dataGridView1.DataSource = dt;
            con.Close();
        }

        private void ClearData()
        {
            txtID.Text = "";
            dtpT1.ResetText();
            dtpT2.ResetText();
            dtpDate.ResetText();
            txtDuration.Text = "";
            cboCustomer.SelectedIndex = -1;
            cboStaff.SelectedIndex = -1;
            txtRemark.Text = "";
            calDurtion();
            isUpdate = false;
            generateID();
        }

        private void cboStaff_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            if (cmb.SelectedItem != null)
            {
                int selectedIndex = cmb.SelectedIndex;
                string selectedText = this.cboStaff.Text;
                staffID = ((ComboboxItem)cmb.SelectedItem).Value.ToString();
            }
        }

        private void cboCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            if (cmb.SelectedItem != null)
            {
                int selectedIndex = cmb.SelectedIndex;
                string selectedText = this.cboCustomer.Text;
                customerID = ((ComboboxItem)cmb.SelectedItem).Value.ToString();
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            ClearData();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!isUpdate)
            {
                if (txtID.Text != "" && txtDuration.Text != "" && cboCustomer.SelectedIndex != -1 && cboStaff.SelectedIndex != -1)
                {
                    cmd = new SqlCommand("insert into tblLog(ID,LDate,StartTime,EndTime,Duration,StaffID,CustomerID,Remark) values(@id,@ldate,@starttime,@endtime,@duration,@staffid,@customerid,@remark)", con);
                    con.Open();
                    cmd.Parameters.AddWithValue("@id", txtID.Text);
                    cmd.Parameters.AddWithValue("@ldate", dtpDate.Text);
                    cmd.Parameters.AddWithValue("@starttime", dtpT1.Text);
                    cmd.Parameters.AddWithValue("@endtime", dtpT2.Text);
                    cmd.Parameters.AddWithValue("@duration", txtDuration.Text);
                    cmd.Parameters.AddWithValue("@staffid", staffID);
                    cmd.Parameters.AddWithValue("@customerid", customerID);
                    cmd.Parameters.AddWithValue("@remark", txtRemark.Text);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    MessageBox.Show("Saved Successfully");
                    DisplayData();
                    ClearData();
                }
                else
                {
                    MessageBox.Show("Please Provide Details!");
                }
            }else{
                if (txtID.Text != "" && txtDuration.Text != "" && cboCustomer.SelectedIndex != -1 && cboStaff.SelectedIndex != -1)
            {
                cmd = new SqlCommand("update tblLog set ID = @id,lDate = @ldate,StartTime=@starttime,EndTime=@endtime,Duration=@duration,StaffID=@staffid,CustomerID=@customerid,Remark=@remark where ID=@id", con);
                con.Open();
                cmd.Parameters.AddWithValue("@id", txtID.Text);
                cmd.Parameters.AddWithValue("@ldate", dtpDate.Text);
                cmd.Parameters.AddWithValue("@starttime", dtpT1.Text);
                cmd.Parameters.AddWithValue("@endtime", dtpT2.Text);
                cmd.Parameters.AddWithValue("@duration", txtDuration.Text);
                cmd.Parameters.AddWithValue("@staffid", staffID);
                cmd.Parameters.AddWithValue("@customerid", customerID);
                cmd.Parameters.AddWithValue("@remark", txtRemark.Text);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Updated Successfully");
                con.Close();
                DisplayData();
                ClearData();
            }
            else
            {
                MessageBox.Show("Please Select Record to Update");
            }
            }
            
        }


        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (txtID.Text != "" && txtDuration.Text != "" && cboCustomer.SelectedIndex != -1 && cboStaff.SelectedIndex != -1)
            {
                cmd = new SqlCommand("delete tblLog where ID=@id", con);
                con.Open();
                cmd.Parameters.AddWithValue("@id", txtID.Text);
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Deleted Successfully!");
                DisplayData();
                ClearData();
            }
            else
            {
                MessageBox.Show("Please Select Record to Delete");
            }  
        }

        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            isUpdate = true;
            txtID.Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            dtpDate.Text = DateTime.ParseExact(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString(), "dd/MM/yyyy", null).ToString();
            dtpT1.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
            dtpT2.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
            txtDuration.Text = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();
            cboStaff.SelectedIndex = cboStaff.FindString(dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString());
            cboCustomer.SelectedIndex = cboCustomer.FindString(dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString());
            txtRemark.Text = dataGridView1.Rows[e.RowIndex].Cells[7].Value.ToString();
        }
       
    }
}
