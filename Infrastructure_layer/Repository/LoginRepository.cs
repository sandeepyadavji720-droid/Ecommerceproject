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
    public class LoginRepository:ILoginRepository
    {

        private readonly IConfiguration _connection;
        public LoginRepository( IConfiguration connection)
        {
            _connection = connection;
        }

        public List<LoginModel> Login(LoginModel login)
        {
            using SqlConnection con = new SqlConnection(_connection.GetConnectionString("DefaultConnection")) ;
           
                 using SqlCommand command = new SqlCommand("sp_login",con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@email", login.email);
                command.Parameters.AddWithValue("@password", login.password);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                List<LoginModel> list = new List<LoginModel>();
                while(reader.Read())
                {
                    LoginModel model = new LoginModel();
                    model.email = reader["email"].ToString();
                    model.password = reader["password"].ToString();
                    model.role = reader["role"].ToString();
                    list.Add(model);
                }
                con.Close();
                return list;
               
               
            }
        }
    }

