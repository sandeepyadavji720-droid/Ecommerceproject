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
    public class GetAllCategoryRepository: IGetAllCategoryRepository
    {
        private readonly IConfiguration _connection;
        public GetAllCategoryRepository(IConfiguration connection)
        {
            _connection = connection;
        }
        public List<ProductModel> GetAllCategory( ProductModel category)
        {
            using SqlConnection con = new SqlConnection(_connection.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new SqlCommand("sp_category", con);
            cmd.CommandType = CommandType.StoredProcedure;
           
                        
            cmd.Parameters.AddWithValue("@action", 2);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<ProductModel> list = new List<ProductModel>();
            while(reader.Read())
            {
                ProductModel model = new ProductModel();
                model.categoryId = Convert.ToInt32(reader["category_id"]);
                model.CategoryName = reader["category_name"].ToString();
                model.imagepath = reader["imagepath"].ToString();
                list.Add(model);
            }
            return list;

        }

    }
}
