using CaesarsAPI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Data;

namespace CaesarsAPI.Services
{
    public class DB_manager
    {
        private SqlConnection conn;


        private void CreateConnection()
        {
            IConfiguration config = new ConfigurationBuilder()
            .AddUserSecrets<DB_manager>()
            .Build();

            string connectionString = "Server=tcp:caesars-db.database.windows.net,1433;Initial Catalog=db1;Persist Security Info=False;User ID=caesar_api_user;Password=ChangedPassword321*;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private void CloseConnection() { conn.Close(); }
        public Guest GetGuestbyId(int id)
        {
            Guest guest = new Guest();
            CreateConnection();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = conn;
                command.CommandType = CommandType.Text;
                command.CommandText = @"SELECT * FROM dbo.guest where id = @id";
                command.Parameters.AddWithValue("id", id);

                try
                {
                    using SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        guest.id = (int)reader["id"];
                        guest.title = reader["title"] as string;
                        guest.name_first = reader["name_first"] as string;
                        guest.name_last = reader["name_last"] as string;
                        guest.suffix = reader["suffix"] as string;
                        guest.dob = (DateTime)reader["dob"];
                        guest.email = reader["email"] as string;
                        guest.addr_street = reader["addr_street"] as string;
                        guest.addr_city = reader["addr_city"] as string;
                        guest.addr_state = reader["addr_state"] as string;
                        guest.addr_zip = reader["addr_zip"] as string;
                        guest.created = (DateTime)reader["created"];
                    }
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.ToString());
                }
                finally
                {
                    CloseConnection();
                }

            };

            return guest;
        }
        public List<Guest> WildSearchGuests(string wildcard, int max, int offset)
        {
            if(max <= 0) { max = 5; }
            if (max > 100) { max = 100; }
            if (offset < 0) { offset = 0; }
            List<Guest> results = new List<Guest>();

            

            CreateConnection();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = conn;
                command.CommandType = CommandType.Text;
                command.CommandText = @"SELECT * FROM dbo.guest ";


                DateTime? dobDT = CaesarUtils.getDate(wildcard);
                if(dobDT != null)
                {
                    command.CommandText += @"where dob = @dob ";
                    command.Parameters.AddWithValue("@dob", dobDT);
                } else
                {
                    wildcard = "%" + wildcard + "%";
                    command.CommandText += @"where name_first like @name_first OR name_last like @name_last OR email like @email OR addr_street like @addr_street OR addr_city like @addr_city OR addr_state like @addr_state OR addr_zip like @addr_zip ";
                    command.Parameters.AddWithValue("@name_first", wildcard);
                    command.Parameters.AddWithValue("@name_last", wildcard);
                    command.Parameters.AddWithValue("@email", wildcard);
                    command.Parameters.AddWithValue("@addr_street", wildcard);
                    command.Parameters.AddWithValue("@addr_city", wildcard);
                    command.Parameters.AddWithValue("@addr_state", wildcard);
                    command.Parameters.AddWithValue("@addr_zip", wildcard);
                }

                command.CommandText += @"ORDER BY id OFFSET @offset ROWS FETCH NEXT @max ROWS ONLY";
                command.Parameters.AddWithValue("@offset", offset);
                command.Parameters.AddWithValue("@max", max);

                try
                {
                    using SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read()) 
                        {
                            Guest guest = new Guest();
                            guest.id = (int)reader["id"];
                            guest.title = reader["title"] as string;
                            guest.name_first = reader["name_first"] as string;
                            guest.name_last = reader["name_last"] as string;
                            guest.suffix = reader["suffix"] as string;
                            guest.dob = (DateTime)reader["dob"];
                            guest.email = reader["email"] as string;
                            guest.addr_street = reader["addr_street"] as string;
                            guest.addr_city = reader["addr_city"] as string;
                            guest.addr_state = reader["addr_state"] as string;
                            guest.addr_zip = reader["addr_zip"] as string;
                            guest.created = (DateTime)reader["created"];
                            results.Add(guest);
                        }
                        
                    }
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.ToString());
                }
                finally
                {
                    CloseConnection();
                }
            };
            return results;
        }

        public void InsertNewGuest(Guest guest)
        {
            CreateConnection();
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = conn;
                command.CommandType = CommandType.Text;
                command.CommandText = @"insert into dbo.guest(title, name_first, name_last, suffix, dob, email, addr_street, addr_city, addr_state, addr_zip) values (@title, @name_first, @name_last, @suffix, @dob, @email, @addr_street, @addr_city, @addr_state, @addr_zip)";

                command.Parameters.AddWithValue("@title", guest.title);
                command.Parameters.AddWithValue("@name_first", guest.name_first);
                command.Parameters.AddWithValue("@name_last", guest.name_last);
                command.Parameters.AddWithValue("@suffix", guest.suffix);
                command.Parameters.AddWithValue("@dob", guest.dob);
                command.Parameters.AddWithValue("@email", guest.email);
                command.Parameters.AddWithValue("@addr_street", guest.addr_street);
                command.Parameters.AddWithValue("@addr_city", guest.addr_city);
                command.Parameters.AddWithValue("@addr_state", guest.addr_state);
                command.Parameters.AddWithValue("@addr_zip", guest.addr_zip);

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.ToString());
                }
                finally
                {
                    CloseConnection();
                }
            }
        }

        public int UpdateGuest(Guest guest)
        {
            int affectedRows = 0;
            CreateConnection();
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = conn;
                command.CommandType = CommandType.Text;
                command.CommandText = @"update dbo.guest set title=@title, name_first=@name_first, name_last=@name_last, suffix=@suffix, dob=@dob, email=@email, addr_street=@addr_street, addr_city=@addr_city, addr_state=@addr_state, addr_zip=@addr_zip where id=@id";

                command.Parameters.AddWithValue("@title", guest.title);
                command.Parameters.AddWithValue("@name_first", guest.name_first);
                command.Parameters.AddWithValue("@name_last", guest.name_last);
                command.Parameters.AddWithValue("@suffix", guest.suffix);
                command.Parameters.AddWithValue("@dob", guest.dob);
                command.Parameters.AddWithValue("@email", guest.email);
                command.Parameters.AddWithValue("@addr_street", guest.addr_street);
                command.Parameters.AddWithValue("@addr_city", guest.addr_city);
                command.Parameters.AddWithValue("@addr_state", guest.addr_state);
                command.Parameters.AddWithValue("@addr_zip", guest.addr_zip);
                command.Parameters.AddWithValue("@id", guest.id);

                try
                {
                    affectedRows = command.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.ToString());
                }
                finally
                {
                    CloseConnection();
                }
            }
            return affectedRows;
        }

        public int DeleteGuest(int id)
        {
            int affectedRows = 0;
            CreateConnection();
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = conn;
                command.CommandType = CommandType.Text;
                command.CommandText = @"delete from dbo.guest where id=@id";
                command.Parameters.AddWithValue ("id", id);

                try
                {
                    affectedRows = command.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.ToString());
                }
                finally
                {
                    CloseConnection();
                }
            }
            return affectedRows;
        }
        public bool InsertNewUser(string username, string pass)
        {
            int affectedRows = 0;
            CreateConnection();
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = conn;
                command.CommandType = CommandType.Text;
                command.CommandText = @"insert into dbo.users (username, pass) values (@username, PWDENCRYPT(@pass))";

                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@pass", pass + CaesarUtils.Salt());

                try
                {
                    affectedRows = command.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.ToString());
                }
                finally
                {
                    CloseConnection();
                }
            }

            if(affectedRows == 1)
            {
                return true;
            }

            return false;
        }

        public void InsertNewLogin(int id)
        {
            CreateConnection();
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = conn;
                command.CommandType = CommandType.Text;
                command.CommandText = @"insert into dbo.user_logins(user_id) values (@id)";

                command.Parameters.AddWithValue("@id", id);

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.ToString());
                }
                finally
                {
                    CloseConnection();
                }
            }
        }

        public bool VerifyLogin(string username, string pass)
        {
            bool loggedInFlag = false;
            int loggedInId = 0;
            CreateConnection();
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = conn;
                command.CommandType = CommandType.Text;
                command.CommandText = @"select id from dbo.users where username = @username AND PWDCOMPARE(@pass, pass) = 1 ";

                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@pass", pass + CaesarUtils.Salt());

                try
                {
                    using SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            loggedInFlag = reader["id"] != null && (int)reader["id"] > 0;
                            loggedInId = Convert.ToInt32(reader["id"]);
                        }
                    }
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.ToString());
                }
                finally
                {
                    CloseConnection();
                }
            }

            if (loggedInFlag)
            {
                InsertNewLogin(loggedInId);
            }

            return loggedInFlag;
        }
    }
}