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
    public class GetCatWiseProductRepository : IGetCategoryWiseProduct
    {

        private readonly IConfiguration _connection;
        public GetCatWiseProductRepository(IConfiguration connection)
        {
            _connection = connection;
        }
        public List<ProductModel> GetCateWiseProduct(int? categoryId)
        {
            using SqlConnection con = new SqlConnection(_connection.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new SqlCommand("sp_product", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@category_id", SqlDbType.Int)
              .Value = categoryId ?? (object)DBNull.Value;

            cmd.Parameters.AddWithValue("@action",3);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<ProductModel> list = new List<ProductModel>();
            while (reader.Read())
            {
                ProductModel model = new ProductModel();
                model.productname = reader["product_name"].ToString();
                model.imagepath = reader["imagepath"].ToString();
                model.price =Convert.ToDecimal(reader["price"].ToString());
                model.product_id =Convert.ToInt32(reader["product_id"].ToString());
                list.Add(model);
            }
            return list;

        }

    }
}
