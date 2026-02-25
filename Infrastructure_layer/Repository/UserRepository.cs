using Application_layer.Interface;
using Domain_layer.Model;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Extensions.Configuration;

namespace Infrastructure_layer.Repository
{
    public class UserRepository:IUserRepository
    {
        private readonly IConfiguration _connection;
        public UserRepository(IConfiguration connection)
        {
            _connection = connection;
        }
        public int Register(UserModel user)
        {
            using SqlConnection con = new SqlConnection(_connection.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new SqlCommand("sp_Users", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@name", user.name);
            cmd.Parameters.AddWithValue("@password", user.password);
            cmd.Parameters.AddWithValue("@email", user.email);
            cmd.Parameters.AddWithValue("@role", user.role);
            cmd.Parameters.AddWithValue("@action", 1);

           
            con.Open();
           int res= cmd.ExecuteNonQuery();
            return res;
        }
        public List<UserModel> GetAllUsers()
        {
            List<UserModel> list = new List<UserModel>();

            using SqlConnection con = new SqlConnection(_connection.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new SqlCommand("sp_Users", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", 5);

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new UserModel
                {
                    name = dr["name"].ToString(),
                    email = dr["email"].ToString(),
                    role = dr["role"].ToString(),
                    password = dr["password"].ToString(),
                   
                });
            }

            return list;
        }


        public int UpdateUser(UserModel user)
        {
            using SqlConnection con = new SqlConnection(_connection.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new SqlCommand("sp_Users", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@name", user.name);
            cmd.Parameters.AddWithValue("@email", user.email);
            cmd.Parameters.AddWithValue("@password", user.password);
            cmd.Parameters.AddWithValue("@role", user.role);
            cmd.Parameters.AddWithValue("@action", 6);

            con.Open();
            return cmd.ExecuteNonQuery();
        }

        public int DeleteUser(string email)
        {
            using SqlConnection con = new SqlConnection(_connection.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new SqlCommand("sp_Users", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@action", 7);

            con.Open();
            return cmd.ExecuteNonQuery();
        }
    }
}
