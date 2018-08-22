using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp
{
    [Serializable]
    class Message
    {
        public static ulong MessagesAmount { get; private set; }

        public string Content { get; set; }
        public ulong ID { get; set; }
        public User Author { get; set; }
        public DateTime TimeOfCreation { get; set; }

        //public Message(string _message)
        //{
        //    ID = MessagesAmount++;
        //    Content = _message;
        //}

        public Message(string message, string username)
        {
            ID = MessagesAmount++;
            Content = message;
            Author = new User(username);
            TimeOfCreation = DateTime.Now;
        }

        public byte[] GetBytes()
        {
            return Encoding.UTF8.GetBytes(this.Content);
        }
    }
}
