using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using System.Data.OleDb; 
namespace FacilityDocLaptop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public class conn
    {
        public SqlConnection condb()
        {
            SqlConnection conn = new SqlConnection("Data Source=YASHASVI;  Initial Catalog=FacilityDocu;  Integrated Security=TRUE");

            return conn;
        }

    }
    public class TempSaveData
    {
        static DataSet userTable;
        static DataSet moduleTable;
        static DataSet stepTable;
        static DataSet actionTable;
        static DataSet toolTable;
        static DataSet resourceTable;
        static DataSet imagesTable;
        static DataSet moduleStepTable;
        static DataSet rigTypeTable;
        static DataSet projectTable;
        static DataSet projectDetailTable;
        static DataSet imageDetailTable;
        static DataSet stepActionTable;
        static DataSet projectActionToolTable;
        static DataSet projectModuleResourceTable;
        static DataSet projectActionDimensionTable;
        static DataSet projectActionImageTable;
        static DataSet projectActionRiskTable;
        public void SetUserTable(DataSet tbl)
        {
            userTable = tbl;
        }
        public DataSet GetUserTable()
        {
            return userTable;
        }



    }
 
    public partial class MainWindow : Window
    {
        conn connectiondb = new conn();
        DataSet ds = new DataSet();
        SqlDataAdapter da = new SqlDataAdapter();


        public MainWindow()
        {
            InitializeComponent();
            HomePage hp = new HomePage();
            hp.Show();
            main.Hide();

            
        }

       

        

        private void login_Click(object sender, RoutedEventArgs e)
        {

            da.SelectCommand =  new SqlCommand("Select * from User Where Password='" + password.Password + "' and UserName= '" + userName.Text + "'", connectiondb.condb());

            //da.SelectCommand = new SqlCommand("SELECT* FROM Users", conn);
            try
            {
                da.Fill(ds);
                ds.Tables[0].Rows[0][0].ToString();
                TempSaveData cal = new TempSaveData();
                cal.SetUserTable(ds);

                HomePage hp = new HomePage();
                hp.Show();
                main.Hide();


            }
            catch
            {
                password.Password = "";
                MessageBox.Show("Invalid Username Or Password");
            }
            //MessageBox.Show(ds.Tables[0].ToString());
        }

     
    }
}
