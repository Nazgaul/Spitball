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
        [ProtoMember(1)]
        public string Name { get; private set; }
        [ProtoMember(2)]
        public string Phone { get; set; }
        [ProtoMember(3)]
        public string University { get; set; }
        [ProtoMember(4)]
        public string Email { get; set; }
        [ProtoMember(5)]
        public string Text { get; set; }

    }
}