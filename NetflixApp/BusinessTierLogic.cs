//
// BusinessTier:  business logic, acting as interface between UI and data store.
//

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


namespace BusinessTier
{

  //
  // Business:
  //
  public class Business
  {
    //
    // Fields:
    //
    private string _DBFile;
    private DataAccessTier.Data dataTier;


    //
    // Constructor:
    //
    public Business(string DatabaseFilename)
    {
      _DBFile = DatabaseFilename;

      dataTier = new DataAccessTier.Data(DatabaseFilename);
    }


    //
    // TestConnection:
    //
    // Returns true if we can establish a connection to the database, false if not.
    //
    public bool TestConnection()
    {
      return dataTier.TestConnection();
    }


    //
    // GetNamedUser:
    //
    // Retrieves User object based on USER NAME; returns null if user is not
    // found.
    //
    // NOTE: there are "named" users from the Users table, and anonymous users
    // that only exist in the Reviews table.  This function only looks up "named"
    // users from the Users table.
    //
    public User GetNamedUser(string UserName)
    {
            UserName = UserName.Replace("'", "''");
            string sql = string.Format(@"SELECT UserID, Occupation FROM Users where Users.Username = '{0}';", UserName);
            DataSet ds = dataTier.ExecuteNonScalarQuery(sql);
            String Occ=null;
            int uid=0;

            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {
                   Occ = Convert.ToString(row["Occupation"]);
                uid = Convert.ToInt32(row["UserID"]);
            }

            User user = new User(uid, UserName, Occ);
                return user;

    }


    //
    // GetAllNamedUsers:
    //
    // Returns a list of all the users in the Users table ("named" users), sorted 
    // by user name.
    //
    // NOTE: the database also contains lots of "anonymous" users, which this 
    // function does not return.
    //
    public IReadOnlyList<User> GetAllNamedUsers()
    {
      List<User> users = new List<User>();

            string sql = string.Format(@"SELECT UserID, UserName, Occupation FROM Users Order by UserName ASC;");
            DataSet ds = dataTier.ExecuteNonScalarQuery(sql);


            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {
              User user = new User(Convert.ToInt32(row["UserID"]), Convert.ToString(row["UserName"]), Convert.ToString(row["Occupation"]));
                users.Add(user);
            }

            return users;
    }


    //
    // GetMovie:
    //
    // Retrieves Movie object based on MOVIE ID; returns null if movie is not
    // found.
    //
    public Movie GetMovie(int MovieID)
    {
            //MovieID = 1000;
            string sql = string.Format(@"SELECT MovieName FROM Movies where Movies.MovieID = {0};", MovieID);
            object mname = dataTier.ExecuteScalarQuery(sql);


            if (mname != null)
            {
                Movie movie = new Movie(MovieID, Convert.ToString(mname));

                return movie;
            }
            else
            return null;      
    }


    //
    // GetMovie:
    //
    // Retrieves Movie object based on MOVIE NAME; returns null if movie is not
    // found.
    //
    public Movie GetMovie(string MovieName)
    {
            MovieName = MovieName.Replace("'", "''");

            string sql = string.Format(@"SELECT MovieID FROM Movies where MovieName = '{0}';", MovieName);
            object mID = dataTier.ExecuteScalarQuery(sql);


            if (mID != null)
            {
                Movie movie = new Movie(Convert.ToInt32(mID), MovieName);

                return movie;
            }
            else
                return null;

            //return null;
    }


    //
    // AddReview:
    //
    // Adds review based on MOVIE ID, returning a Review object containing
    // the review, review's id, etc.  If the add failed, null is returned.
    //
    public Review AddReview(int MovieID, int UserID, int Rating)
    {
            string sql = string.Format(@" INSERT INTO Reviews(MovieID,UserID,Rating) VALUES({0},{1},{2}); 
                                        SELECT ReviewID FROM Reviews WHERE ReviewID = SCOPE_IDENTITY(); ", MovieID, UserID, Rating);
            object reviewID = dataTier.ExecuteScalarQuery(sql);

            if (reviewID != null)
            {
                Review rev = new Review(Convert.ToInt32(reviewID), MovieID, UserID, Rating);

                return rev;
            }
            else
                return null;

        }


    //
    // GetMovieDetail:
    //
    // Given a MOVIE ID, returns detailed information about this movie --- all
    // the reviews, the total number of reviews, average rating, etc.  If the 
    // movie cannot be found, null is returned.
    //
    public MovieDetail GetMovieDetail(int MovieID)
    {
            Movie m = GetMovie(MovieID);

            string sql = string.Format(@"SELECT AVG(cast(Rating as float)) FROM Reviews where MovieID = {0};", MovieID);
            object average = dataTier.ExecuteScalarQuery(sql);
            Double avg = 0;
            if (average!= DBNull.Value)
                avg = Convert.ToDouble(average);

            string sql1 = string.Format(@"SELECT Count(*) FROM Reviews where MovieID = {0};", MovieID);
            int noOfR = Convert.ToInt32(dataTier.ExecuteScalarQuery(sql1));

            List<Review> reviews = new List<Review>();

            string sql2 = string.Format(@"SELECT * FROM Reviews where MovieID = {0} Order by Rating Desc, UserID ASC;", MovieID);
            DataSet ds = dataTier.ExecuteNonScalarQuery(sql2);


            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {
                Review review = new Review(Convert.ToInt32(row["ReviewID"]), Convert.ToInt32(row["MovieID"]), Convert.ToInt32(row["UserID"]), Convert.ToInt32(row["Rating"]));
                reviews.Add(review);
            }

            MovieDetail movieDetail = new MovieDetail(m, avg, noOfR, reviews);

            return movieDetail;
    }


    //
    // GetUserDetail:
    //
    // Given a USER ID, returns detailed information about this user --- all
    // the reviews submitted by this user, the total number of reviews, average 
    // rating given, etc.  If the user cannot be found, null is returned.
    //
    public UserDetail GetUserDetail(int UserID)
    {
            //Movie m = GetMovie(MovieID);
            //UserName = UserName.Replace("'", "''");
            string sql0 = string.Format(@"SELECT UserName, Occupation FROM Users where Users.UserID = {0};", UserID);
            DataSet ds0 = dataTier.ExecuteNonScalarQuery(sql0);
            String Occ = null;
            String Uname = null;

            foreach (DataRow row in ds0.Tables["TABLE"].Rows)
            {
                Occ = Convert.ToString(row["Occupation"]);
                Uname = Convert.ToString(row["UserName"]);
            }

            User user = new User(UserID, Uname, Occ);
           // return user;

            string sql = string.Format(@"SELECT AVG(cast(Rating as float)) FROM Reviews where UserID = {0};", UserID);
            Double avg=0;
            object average = dataTier.ExecuteScalarQuery(sql);
            if (average != DBNull.Value)
            avg = Convert.ToDouble(average);

            string sql1 = string.Format(@"SELECT Count(*) FROM Reviews where UserID = {0};", UserID);
            int noOfR = Convert.ToInt32(dataTier.ExecuteScalarQuery(sql1));

            List<Review> reviews = new List<Review>();

            string sql2 = string.Format(@"SELECT * FROM Reviews 
                                        Inner Join Movies on Movies.MovieID = Reviews.MovieID
                                        where UserID = {0}
                                        Order by MovieName ASC, Rating Desc;", UserID);
            DataSet ds = dataTier.ExecuteNonScalarQuery(sql2);


            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {
                Review review = new Review(Convert.ToInt32(row["ReviewID"]), Convert.ToInt32(row["MovieID"]), Convert.ToInt32(row["UserID"]), Convert.ToInt32(row["Rating"]));
                reviews.Add(review);
            }

            UserDetail userDetail = new UserDetail(user, avg, noOfR, reviews);

            return userDetail;

            //return null;
    }


    //
    // GetTopMoviesByAvgRating:
    //
    // Returns the top N movies in descending order by average rating.  If two
    // movies have the same rating, the movies are presented in ascending order
    // by name.  If N < 1, an EMPTY LIST is returned.
    //
    public IReadOnlyList<Movie> GetTopMoviesByAvgRating(int N)
    {
      List<Movie> movies = new List<Movie>();

            string sql = string.Format(@"SELECT Top {0} MovieID, AVG(cast(Rating as float)) FROM Reviews group by MovieID Order by AVG(cast(Rating as float)) DESC;", N);
            DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {
                Movie movie = GetMovie(Convert.ToInt32(row["MovieID"]));
                movies.Add(movie);
            }

               return movies;
    }

        public IReadOnlyList<Movie> GetAllMovies()
        {
            List<Movie> movies = new List<Movie>();

            string sql = string.Format(@"SELECT MovieID, MovieName FROM Movies Order by MovieName ASC;");
            DataSet ds = dataTier.ExecuteNonScalarQuery(sql);


            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {
                Movie user = new Movie(Convert.ToInt32(row["MovieID"]), Convert.ToString(row["MovieName"]));
                movies.Add(user);
            }

            return movies;
        }

    }//class
}//namespace
