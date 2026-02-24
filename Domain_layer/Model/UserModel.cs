using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_layer.Model
{
    public class UserModel
    {
        public string name { set; get; }
        public string email { set; get; }
        public string password { set; get; }
        public string role { set; get; }
        public string? profilePic { set; get; }
    }
}
