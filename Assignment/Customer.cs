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
    public partial class Customer : Form
    {
        SqlConnection con = new SqlConnection("Data Source=DESKTOP-LQ48K27;Initial Catalog=Assignment25;Integrated Security=true;");
        SqlCommand cmd;
        SqlDataAdapter adapt;
        Boolean isUpdate = false;

        public Customer()
        {
            InitializeComponent();
            generateID();
            rdoMale.Checked = true;
            DisplayData();
        }

        private void generateID()
        {
            con.Open();
            string sqlQuery = "SELECT TOP 1 ID from Customer order by ID desc";
            SqlCommand cmd = new SqlCommand(sqlQuery, con);
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                string input = dr["ID"].ToString();
                string angka = input.Substring(1, 4);
                int number = Convert.ToInt32(angka);
                number += 1;
                string str = number.ToString("D4");

                txtID.Text = "C" + str;
            }
            else
            {
                txtID.Text = "C0001";
            }
            con.Close();
        }

        private void ClearData()
        {
            txtID.Text = "";
            txtName.Text = "";
            txtNrc.Text = "";
            txtAddress.Text = "";
            txtPhno.Text = "";
            txtRemark.Text = "";
            rdoMale.Checked = true;
            isUpdate = false;
            generateID();
        }

        private void DisplayData()
        {
            con.Open();
            DataTable dt = new DataTable();
            adapt = new SqlDataAdapter("select ID,Name,(case when Gender=2 then 'Female' else 'Male' end) Gender, NRC, Address, Phno, Remark from Customer", con);
            adapt.Fill(dt);
            dataGridView1.DataSource = dt;
            con.Close();
        }  

        private void btnSave_Click(object sender, EventArgs e)
        {
            int gen = 1;
            if (!isUpdate)
            {
                if (txtID.Text != "" && txtName.Text != "" && txtNrc.Text != "" && txtAddress.Text != "" && txtPhno.Text != "")
                {
                    if (rdoMale.Checked) gen = 1;
                    else gen = 2;
                    cmd = new SqlCommand("insert into Customer(id,name,gender,nrc,address,phno,remark) values(@id,@name,@gender,@nrc,@address,@phno,@remark)", con);
                    con.Open();
                    cmd.Parameters.AddWithValue("@id", txtID.Text);
                    cmd.Parameters.AddWithValue("@name", txtName.Text);
                    cmd.Parameters.AddWithValue("@gender", gen);
                    cmd.Parameters.AddWithValue("@nrc", txtNrc.Text);
                    cmd.Parameters.AddWithValue("@address", txtAddress.Text);
                    cmd.Parameters.AddWithValue("@phno", txtPhno.Text);
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
            }
            else
            {
                if (rdoMale.Checked) gen = 1;
                else gen = 2;
                if (txtID.Text != "" && txtName.Text != "" && txtNrc.Text != "" && txtAddress.Text != "" && txtPhno.Text != "")
                {
                    cmd = new SqlCommand("update Customer set ID=@id,Name=@name,Gender=@gender,NRC=@nrc,Address=@address,Phno=@phno,Remark=@remark where ID=@id", con);
                    con.Open();
                    cmd.Parameters.AddWithValue("@id", txtID.Text);
                    cmd.Parameters.AddWithValue("@name", txtName.Text);
                    cmd.Parameters.AddWithValue("@gender", gen);
                    cmd.Parameters.AddWithValue("@nrc", txtNrc.Text);
                    cmd.Parameters.AddWithValue("@address", txtAddress.Text);
                    cmd.Parameters.AddWithValue("@phno", txtPhno.Text);
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

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (txtID.Text != "" && txtName.Text != "" && txtNrc.Text != "" && txtAddress.Text != "" && txtPhno.Text != "")
            {
                cmd = new SqlCommand("delete Customer where ID=@id", con);
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

        private void dataGridView1_RowHeaderMouseClick_1(object sender, DataGridViewCellMouseEventArgs e)
        {
            isUpdate = true;
            txtID.Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            txtName.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            if (dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString() == "Male") rdoMale.Checked = true;
            else rdoFemale.Checked = true;
            txtNrc.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
            txtAddress.Text = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();
            txtPhno.Text = dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString();
            txtRemark.Text = dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            ClearData();
        }
    }
}
