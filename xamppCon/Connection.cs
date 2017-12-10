using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;

namespace xamppCon
{
    public class Connection
    {
        public MySqlConnection conn = new MySqlConnection();
        MySqlTransaction trans;
        public String connect(String db)
        {
            try
            {
                conn.Close();
                conn.ConnectionString = "Server=localhost; Database="+db+"; Uid=root; Pwd=;";
                conn.Open();
                return "connect";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void endConnection() { conn.Close(); }

        public DataSet selectData(String query)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd = conn.CreateCommand();
            cmd.CommandText = query;
            MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adap.Fill(ds);
            return ds;
        }

        public void manipulateData(String query)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd = conn.CreateCommand();
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
        }

        public int countRow(String tableCondition)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd = conn.CreateCommand();
            cmd.CommandText = "select count(*) from " + tableCondition;
            return Convert.ToInt32(cmd.ExecuteScalar().ToString());
        }

        public String returnOne(String query)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd = conn.CreateCommand();
            cmd.CommandText = query;
            return cmd.ExecuteScalar().ToString();
        }

        public String convertDate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        public String convertTime(DateTime date)
        {
            return date.ToString("HH:mm:ss");
        }

        public void beginTransaction()
        {
            trans = conn.BeginTransaction();
        }

        public void doTransaction(string query)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.Transaction = trans;
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
        }

        public void commit()
        {
            trans.Commit();
        }

        public void rollback()
        {
            trans.Rollback();
        }

        public bool transaction(DataTable input, String[] column, String[] type, String table)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.Transaction = trans;
            try
            {
                foreach (DataRow row in input.Rows)
                {
                    String col = "", val = "";
                    for (int i = 0; i < column.Length; i++)
                    {
                        if (type[i] == "int")
                        {
                            val += row[i].ToString();
                        }
                        else if (type[i] == "string")
                        {
                            val += "'" + row[i].ToString() + "'";
                        }

                        col += column[i];
                        if (i < column.Length - 1)
                        {
                            val += ", ";
                            col += ", ";
                        }
                    } cmd.CommandText = "insert into " + table + "("+col+")" + "values ("+val+")";
                } trans.Commit();
                return true;
            }
            catch (Exception)
            {
                trans.Rollback();
                return false;
                throw;
            }
        }
    }
}
