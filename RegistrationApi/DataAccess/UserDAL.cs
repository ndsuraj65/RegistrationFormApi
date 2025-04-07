
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

        public async Task CreateUserAsync(UserModel user)
        {
            using var con = new NpgsqlConnection(_connectionString);
            using var cmd = new NpgsqlCommand("sp_create_user", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("p_fullname", user.FullName);
            cmd.Parameters.AddWithValue("p_email", user.Email);
            cmd.Parameters.AddWithValue("p_phone", user.Phone);
            cmd.Parameters.AddWithValue("p_password", user.Password);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<UserModel>> GetAllUsersAsync()
        {
            var users = new List<UserModel>();
            using var con = new NpgsqlConnection(_connectionString);
            using var cmd = new NpgsqlCommand("SELECT * FROM sp_get_all_users()", con);

            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
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

        public async Task<UserModel> GetUserByIdAsync(int id)
        {
            UserModel user = null;
            using var con = new NpgsqlConnection(_connectionString);
            using var cmd = new NpgsqlCommand("SELECT * FROM sp_get_user_by_id(@p_id)", con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("p_id", id);

            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
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

        public async Task UpdateUserAsync(UserModel user)
        {
            using var con = new NpgsqlConnection(_connectionString);
            using var cmd = new NpgsqlCommand("sp_update_user", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("p_id", user.Id);
            cmd.Parameters.AddWithValue("p_fullname", user.FullName);
            cmd.Parameters.AddWithValue("p_email", user.Email);
            cmd.Parameters.AddWithValue("p_phone", user.Phone);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            using var con = new NpgsqlConnection(_connectionString);
            using var cmd = new NpgsqlCommand("SELECT sp_delete_user(@p_id)", con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("p_id", id);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
