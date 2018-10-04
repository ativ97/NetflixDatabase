using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace NetflixApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //this.clearForm();

        }

        private bool fileExists(string filename)
        {
            if (!System.IO.File.Exists(filename))
            {
                string msg = string.Format("Input file not found: '{0}'",
                  filename);

                MessageBox.Show(msg);
                return false;
            }

            // exists!
            return true;
        }

        /* private void clearForm()
         {
             this.textBox1.;
             this.chart.Titles.Clear();
             this.chart.Legends.Clear();
         }*/


        private void button1_Click_1(object sender, EventArgs e)
        {
            //
            // Check to make sure database filename in text box actually exists:
            //
            this.listBox1.Items.Clear();
            this.listBox2.Items.Clear();

            this.Cursor = Cursors.WaitCursor;


            String dbfilename = this.textBox1.Text;

            BusinessTier.Business biztier = new BusinessTier.Business(dbfilename);
            if (!biztier.TestConnection())
                return;

            IReadOnlyList<BusinessTier.Movie> movies = biztier.GetAllMovies();

            foreach (var name in movies)
            {
                listBox1.Items.Add(name.MovieName);
            }
            /*if (!fileExists(filename))
                return;


            //clearForm();

            SqlConnection db;
            String version = "MSSQLLocalDB";
            //filename = "CrimeDB.mdf";
            String connectionInfo = String.Format(@"Data Source=(LocalDB)\{0};AttachDbFilename=|DataDirectory|\{1};Integrated Security=True;", version, filename);
            db = new SqlConnection(connectionInfo);
            db.Open();
            string sql = string.Format(@"SELECT MovieName FROM Movies ORDER BY MovieName ASC;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = db;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            cmd.CommandText = sql;
            adapter.Fill(ds);

            String movienames;

            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {
                movienames = string.Format("{0}", (string)(row["MovieName"]));
                listBox1.Items.Add(movienames);
            }

            db.Close();*/



            getallusers();

            this.Cursor = Cursors.Default;



        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            String dbfilename = this.textBox1.Text;

           /* if (!fileExists(filename))
                return;*/

            this.Cursor = Cursors.WaitCursor;

            //clearForm();

            /*SqlConnection db;
            String version = "MSSQLLocalDB";
            //filename = "CrimeDB.mdf";
            String connectionInfo = String.Format(@"Data Source=(LocalDB)\{0};AttachDbFilename=|DataDirectory|\{1};Integrated Security=True;", version, filename);
            db = new SqlConnection(connectionInfo);
            db.Open();
            string Name = this.listBox1.Text;
            Name = Name.Replace("'", "''");
            string sql = string.Format(@"SELECT MovieID FROM Movies where Movies.MovieName= '{0}';", Name);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = db;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            cmd.CommandText = sql;
            object result = cmd.ExecuteScalar();*/
            BusinessTier.Business biztier = new BusinessTier.Business(dbfilename);
            if (!biztier.TestConnection())
                return;
            //if (textBox5.Text.Length == 0) return;
            //int movieID = Convert.ToInt32(textBox5.Text);

            string Name = this.listBox1.Text;

            BusinessTier.Movie details = biztier.GetMovie(Name);

            //var reviews = details.Reviews;


            int id = Convert.ToInt32(details.MovieID);
            textBox5.Text = Convert.ToString(id);

            /*sql = string.Format(@"SELECT Rating FROM Reviews, Movies where Movies.MovieName= '{0}' And Movies.MovieID= Reviews.MovieID;", Name);

            cmd = new SqlCommand();
            cmd.Connection = db;
            adapter = new SqlDataAdapter(cmd);
            ds = new DataSet();

            cmd.CommandText = sql;
            adapter.Fill(ds);
            List<Double> numbers = new List<double>();
            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {
                Double rate = Convert.ToDouble(row["Rating"]);
                numbers.Add(rate);
                //movienames = string.Format("{0}", (string)(row["MovieName"]));
                //listBox1.Items.Add(movienames);
            }
            if (numbers.Count == 0)
            {
                this.Cursor = Cursors.Default;

                return;
            }*/

            BusinessTier.MovieDetail movieDetail = biztier.GetMovieDetail(id);


            Double average = movieDetail.AvgRating;

            textBox2.Text = average.ToString();
            //db.Close();

            getmoviereviews();

            this.Cursor = Cursors.Default;

            /*int UN= 233;

            BusinessTier.Business biztier = new BusinessTier.Business(filename);
            BusinessTier.Movie m = biztier.GetMovie(UN);

            textBox16.Text = m.MovieName;*/

        }

        private void getallusers()
        {
            string dbfilename = this.textBox1.Text;

            BusinessTier.Business biztier = new BusinessTier.Business(dbfilename);
            if (!biztier.TestConnection())
                return;

            IReadOnlyList<BusinessTier.User> users = biztier.GetAllNamedUsers();

            foreach (var name in users)
            {
                listBox2.Items.Add(name.UserName);
            }

        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string dbfilename = this.textBox1.Text;

            BusinessTier.Business biztier = new BusinessTier.Business(dbfilename);
            if (!biztier.TestConnection())
                return;
            string selected = listBox2.Text;

            BusinessTier.User user = biztier.GetNamedUser(selected);

            textBox7.Text = user.Occupation;
            textBox6.Text = Convert.ToString(user.UserID);
            getuserreviews();
        }

        private void getmoviereviews()
        {
            this.listBox3.Items.Clear();
            this.listBox5.Items.Clear();


            string dbfilename = this.textBox1.Text;

            BusinessTier.Business biztier = new BusinessTier.Business(dbfilename);
            if (!biztier.TestConnection())
                return;
            if (textBox5.Text.Length == 0) return;
            int movieID = Convert.ToInt32(textBox5.Text);

            BusinessTier.MovieDetail details = biztier.GetMovieDetail(movieID);

            var reviews = details.Reviews;

            int rate5 = 0; int rate4 = 0; int rate3 = 0; int rate2 = 0; int rate1 = 0;


            foreach (var review in reviews)
            {
                string line1 = string.Format("{0}: {1}", review.UserID, review.Rating);
                listBox3.Items.Add(line1);

                if (review.Rating == 5)
                    rate5++;
                else if (review.Rating == 4)
                    rate4++;
                else if (review.Rating == 3)
                    rate3++;
                else if (review.Rating == 2)
                    rate2++;
                else if (review.Rating == 1)
                    rate1++;
            }

            string line = string.Format("5: {0}", rate5);
            listBox5.Items.Add(line);

            line = string.Format("4: {0}", rate4);
            listBox5.Items.Add(line);
            line = string.Format("3: {0}", rate3);
            listBox5.Items.Add(line);
            line = string.Format("2: {0}", rate2);
            listBox5.Items.Add(line);

            line = string.Format("1: {0}", rate1);
            listBox5.Items.Add(line);




        }

        private void getuserreviews()
        {
            this.listBox4.Items.Clear();

            string dbfilename = this.textBox1.Text;

            BusinessTier.Business biztier = new BusinessTier.Business(dbfilename);
            if (!biztier.TestConnection())
                return;
            if (textBox6.Text.Length == 0) return;
            int userID = Convert.ToInt32(textBox6.Text);

            BusinessTier.UserDetail details = biztier.GetUserDetail(userID);

            var reviews = details.Reviews;

            foreach (var review in reviews)
            {
                string moviename = biztier.GetMovie(review.MovieID).MovieName;
                string line = string.Format("{0}-> {1}", moviename, review.Rating);
                listBox4.Items.Add(line);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string dbfilename = this.textBox1.Text;

            BusinessTier.Business biztier = new BusinessTier.Business(dbfilename);
            if (!biztier.TestConnection())
                return;
            if (textBox5.Text.Length == 0) return;

            int movieID = Convert.ToInt32(textBox5.Text);
            if (textBox6.Text.Length == 0) return;

            int userID = Convert.ToInt32(textBox6.Text);
            if (comboBox1.Text.Length == 0) return;

            int rating = Convert.ToInt32(comboBox1.Text);

            BusinessTier.Review review = biztier.AddReview(movieID, userID, rating);

            if (review == null)
                MessageBox.Show("Failed");
            else
                MessageBox.Show("Success");

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.listBox1.Items.Clear();
            if (textBox10.Text.Length == 0) return;
            int top = Convert.ToInt32(textBox10.Text);
            string dbfilename = this.textBox1.Text;

            BusinessTier.Business biztier = new BusinessTier.Business(dbfilename);
            if (!biztier.TestConnection())
                return;
            IReadOnlyList<BusinessTier.Movie> topmovieAvg = biztier.GetTopMoviesByAvgRating(top);
            BusinessTier.MovieDetail movies;
            foreach (var name in topmovieAvg)
            {
                movies = biztier.GetMovieDetail(name.MovieID);
                string line = string.Format("{0}: {1}", name.MovieName, movies.AvgRating);
                listBox1.Items.Add(line);
            }

        }
    }
}