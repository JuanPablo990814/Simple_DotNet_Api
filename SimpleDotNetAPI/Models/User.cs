namespace SimpleDotNetAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Document {get; set;}
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime ModifyDate { get; set; }

        public User(int id, string phoneNumber, string name,
         string email, DateTime modifyDate)
        {
            Id = id;
            PhoneNumber = phoneNumber;
            Name = name;
            Email = email;
            ModifyDate = modifyDate;
        }
    }
}