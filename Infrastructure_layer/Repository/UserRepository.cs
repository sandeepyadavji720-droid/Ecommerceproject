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
        public void Register(UserModel user)
        {
            using SqlConnection con = new SqlConnection(_connection.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new SqlCommand("sp_Users", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@name", user.name);
            cmd.Parameters.AddWithValue("@password", user.password);
            cmd.Parameters.AddWithValue("@email", user.email);
            cmd.Parameters.AddWithValue("@role", user.role);

           
            con.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
