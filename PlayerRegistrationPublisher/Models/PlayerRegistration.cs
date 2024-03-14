using System.Xml.Linq;

namespace PlayerRegistrationPublisher.Models
{
    public class PlayerRegistration
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int Age { get; set; }
        public string Country { get; set; } = "";
        public string Position { get; set; } = "";

        public PlayerRegistration(XElement id, XElement name, XElement age, XElement country, XElement position)
        {
            if (id.Value != null)
                Id = Convert.ToInt32(id.Value);
            else
                Console.WriteLine("Id is null");

            if (name.Value != null)
                Name = name.Value;
            else
                Console.WriteLine("Name is null");

            if (age.Value != null)
                Age = Convert.ToInt32(age.Value);
            else
                Console.WriteLine("Age is null");

            if (country.Value != null)
                Country = country.Value;
            else
                Console.WriteLine("Country is null");

            if (position.Value != null)
                Position = position.Value;
            else
                Console.WriteLine("Position is null");
        }
    }
}