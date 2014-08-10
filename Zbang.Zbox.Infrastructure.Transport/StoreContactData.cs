using ProtoBuf;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    public class StoreContactData : StoreData
    {
        protected StoreContactData()
        {
            
        }

        public StoreContactData(string name, string phone, string university, string email, string text)
        {
            Name = name;
            Phone = phone;
            University = university;
            Email = email;
            Text = text;
        }

        public string Name { get; private set; }
        public string Phone { get; set; }
        public string University { get; set; }
        public string Email { get; set; }
        public string Text { get; set; }

    }
}