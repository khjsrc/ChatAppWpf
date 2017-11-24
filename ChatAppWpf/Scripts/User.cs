using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp
{
    [Serializable]
    class User
    {
        internal int ID { get; set; }
        public string UserName { get; set; }
        public DateTime RegistrationDay { get; set; }
        public int Age { get; set; }

        public User(string _name, int _age)
        {
            if (Encoding.UTF8.GetBytes(_name).Length <= 32)
            {
                UserName = _name;
            }
            else
            {
                UserName = _name.Substring(0,32);
            }
            Age = _age;
            RegistrationDay = DateTime.Now;
        }
    }
}
