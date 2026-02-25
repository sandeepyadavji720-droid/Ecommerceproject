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

        public ProductModel GetProductById(int id)
        {
            ProductModel product = new ProductModel();

            using SqlConnection con =
                new SqlConnection(_connection.GetConnectionString("DefaultConnection"));

            using SqlCommand cmd = new SqlCommand("sp_product", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@product_id", id);
            cmd.Parameters.AddWithValue("@action", 6);

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                product.product_id = Convert.ToInt32(dr["product_id"]);
                product.productname = dr["product_name"].ToString();
                product.price = Convert.ToDecimal(dr["price"]);
                product.imagepath = dr["imagepath"].ToString();
            }

            return product;
        }

        public int UpdateProduct(ProductModel product)
        {
            using SqlConnection con =
                new SqlConnection(_connection.GetConnectionString("DefaultConnection"));

            using SqlCommand cmd = new SqlCommand("sp_product", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@product_id", product.product_id);
            cmd.Parameters.AddWithValue("@product_name", product.productname);
            cmd.Parameters.AddWithValue("@price", product.price);
            cmd.Parameters.AddWithValue("@imagepath", product.imagepath);
            cmd.Parameters.AddWithValue("@category_id", product.categoryId);
            cmd.Parameters.AddWithValue("@action", 4);

            con.Open();
            return cmd.ExecuteNonQuery();
        }

        public int DeleteProduct(int id)
        {
            using SqlConnection con =
                new SqlConnection(_connection.GetConnectionString("DefaultConnection"));

            using SqlCommand cmd = new SqlCommand("sp_product", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@product_id", id);
            cmd.Parameters.AddWithValue("@action", 5);

            con.Open();
            return cmd.ExecuteNonQuery();
        }

    }
}
