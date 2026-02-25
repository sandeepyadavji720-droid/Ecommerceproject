using Domain_layer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application_layer.Interface
{
    public interface IProductRepository
    {
        int AddProduct(ProductModel product);
        int UpdateProduct(ProductModel model);
        int DeleteProduct(int id);
        ProductModel GetProductById(int id);
    }
}
