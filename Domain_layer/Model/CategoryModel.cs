using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_layer.Model
{
    public class CategoryModel
    {
        public string? categoryname { get; set; }

        public string? imagepath { get; set; }
        public IFormFile? image { get; set; }
    }
}
