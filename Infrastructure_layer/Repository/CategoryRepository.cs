using Application_layer.Interface;
using Domain_layer.Model;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure_layer.Repository
{
    public class CategoryRepository:ICategoryRepository
    {
        private readonly IConfiguration _connection;
        public CategoryRepository(IConfiguration connection)
        {
            _connection = connection;
        }
        public int AddCategory(CategoryModel category)
        {
            using SqlConnection con = new SqlConnection(_connection.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new SqlCommand("sp_category", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@category_name", category.categoryname);
            cmd.Parameters.AddWithValue("@imagepath", category.imagepath);
            cmd.Parameters.AddWithValue("@action",1);
           


            con.Open();
           int res= cmd.ExecuteNonQuery();
            return res;
        }
        
    }
}
