using Domain_layer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application_layer.Interface
{
    public interface ISingleUserRepository
    {
        UserModel GetSingleUser(string email);
        void UpdateUser(string name, string email);
        void DeleteUser(string email);
    }
}
