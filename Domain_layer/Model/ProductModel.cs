using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_layer.Model
{
    public class ProductModel
    {
       
        public string? productname { get; set; }

        public decimal? price { get; set; }
        public int product_id { get; set; }
        public int ProductId { get; set; }

        public int? categoryId { get; set; }

        public string? imagepath { get; set; }  

        public IFormFile? image { get; set; }
        public string? CategoryName { get; set; }
    }
}

