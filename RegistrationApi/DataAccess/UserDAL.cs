using Npgsql;
using RegistrationApi.Models;
using System.Data;

namespace RegistrationApi.DataAccess
{
    public class UserDAL
    {
        private readonly string _connectionString;

        public UserDAL(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public void CreateUser(UserModel user)
        {
            using var con = new NpgsqlConnection(_connectionString);
            using var cmd = new NpgsqlCommand("sp_create_user", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("p_fullname", user.FullName);
            cmd.Parameters.AddWithValue("p_email", user.Email);
            cmd.Parameters.AddWithValue("p_phone", user.Phone);
            cmd.Parameters.AddWithValue("p_password", user.Password);

            con.Open();
            cmd.ExecuteNonQuery();
        }

        /* public List<UserModel> GetAllUsers()
         {
             var users = new List<UserModel>();
             using var con = new NpgsqlConnection(_connectionString);
             using var cmd = new NpgsqlCommand("sp_get_all_users", con);
             cmd.CommandType = CommandType.StoredProcedure;

             con.Open();
             using var reader = cmd.ExecuteReader();
             while (reader.Read())
             {
                 users.Add(new UserModel
                 {
                     Id = Convert.ToInt32(reader["id"]),
                     FullName = reader["fullname"].ToString(),
                     Email = reader["email"].ToString(),
                     Phone = reader["phone"].ToString(),
                     CreatedAt = Convert.ToDateTime(reader["createdat"])
                 });
             }

             return users;
         }
 */
        public List<UserModel> GetAllUsers()
        {
            var users = new List<UserModel>();
            using var con = new NpgsqlConnection(_connectionString);

            // ✅ Use SELECT instead of CommandType.StoredProcedure
            using var cmd = new NpgsqlCommand("SELECT * FROM sp_get_all_users()", con);

            con.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                users.Add(new UserModel
                {
                    Id = Convert.ToInt32(reader["id"]),
                    FullName = reader["fullname"].ToString(),
                    Email = reader["email"].ToString(),
                    Phone = reader["phone"].ToString(),
                    CreatedAt = Convert.ToDateTime(reader["createdat"])
                });
            }

            return users;
        }

        /*public UserModel GetUserById(int id)
        {
            using var con = new NpgsqlConnection(_connectionString);
            using var cmd = new NpgsqlCommand("sp_get_user_by_id", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("p_id", id);

            con.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new UserModel
                {
                    Id = Convert.ToInt32(reader["id"]),
                    FullName = reader["fullname"].ToString(),
                    Email = reader["email"].ToString(),
                    Phone = reader["phone"].ToString(),
                    CreatedAt = Convert.ToDateTime(reader["createdat"])
                };
            }

            return null;
        }*/
        public UserModel GetUserById(int id)
        {
            UserModel user = null;

            using var con = new NpgsqlConnection(_connectionString);
            // ✅ Call it like a SQL SELECT
            using var cmd = new NpgsqlCommand("SELECT * FROM sp_get_user_by_id(@p_id)", con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("p_id", id);

            con.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                user = new UserModel
                {
                    Id = Convert.ToInt32(reader["id"]),
                    FullName = reader["fullname"].ToString(),
                    Email = reader["email"].ToString(),
                    Phone = reader["phone"].ToString(),
                    CreatedAt = Convert.ToDateTime(reader["createdat"])
                };
            }

            return user;
        }


        public void UpdateUser(UserModel user)
        {
            using var con = new NpgsqlConnection(_connectionString);
            using var cmd = new NpgsqlCommand("sp_update_user", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("p_id", user.Id);
            cmd.Parameters.AddWithValue("p_fullname", user.FullName);
            cmd.Parameters.AddWithValue("p_email", user.Email);
            cmd.Parameters.AddWithValue("p_phone", user.Phone);

            con.Open();
            cmd.ExecuteNonQuery();
        }


        public void DeleteUser(int id)
        {
            using var con = new NpgsqlConnection(_connectionString);
            using var cmd = new NpgsqlCommand("SELECT sp_delete_user(@p_id)", con); // ✅ SELECT function
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("p_id", id);

            con.Open();
            cmd.ExecuteNonQuery();
        }

    }
}
