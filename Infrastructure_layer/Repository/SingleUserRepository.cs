using Application_layer.Interface;
using Domain_layer.Model;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure_layer.Repository
{
    public class SingleUserRepository:ISingleUserRepository
    {
        private readonly IConfiguration _connection;
        public SingleUserRepository(IConfiguration connection)
        {
            _connection = connection;
        }
        public UserModel GetSingleUser(string email)
        {
            using SqlConnection con = new SqlConnection(_connection.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new SqlCommand("sp_Users", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@action", 2); 
            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new UserModel
                {
                    email = reader["email"].ToString(),
                    name = reader["name"].ToString()
                };
            }

            return null; 
        }
    }
    }

