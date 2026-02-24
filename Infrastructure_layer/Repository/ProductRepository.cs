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
    public class ProductRepository : IProductRepository
    {
        private readonly IConfiguration _connection;
        public ProductRepository(IConfiguration connection)
        {
            _connection = connection;
        }
        public int AddProduct(ProductModel product)
        {
            using SqlConnection con = new SqlConnection(_connection.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new SqlCommand("sp_product", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@product_name", product.productname);
            cmd.Parameters.AddWithValue("@price", product.price);
            cmd.Parameters.AddWithValue("@imagepath",product.imagepath);
            cmd.Parameters.AddWithValue("@category_id",product.categoryId);
            cmd.Parameters.AddWithValue("@action",1);



            con.Open();
            int res = cmd.ExecuteNonQuery();
            return res;
        }
    }
}
