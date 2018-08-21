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

        public User(string name)
        {
            if (Encoding.UTF8.GetBytes(name).Length <= 32)
            {
                UserName = name;
            }
            else
            {
                UserName = name.Substring(0,32);
            }
            RegistrationDay = DateTime.Now;
        }
    }
}
