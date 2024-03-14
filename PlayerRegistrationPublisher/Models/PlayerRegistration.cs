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
                throw new ArgumentNullException("Id is null");

            if (name.Value != string.Empty)
                Name = name.Value;
            else
                throw new ArgumentNullException("Name is null");

            if (age.Value != null)
                Age = Convert.ToInt32(age.Value);
            else
                throw new ArgumentNullException("Age is null");

            if (country.Value != string.Empty)
                Country = country.Value;
            else
                throw new ArgumentNullException("Country is null");

            if (position.Value != string.Empty)
                Position = position.Value;
            else
                throw new ArgumentNullException("Position is null");
        }
    }
}