using System.Xml.Linq;

namespace PlayerRegistrationPublisher.Models
{
    public class PlayerAchievements
    {
        public int Year { get; set; }
        public string Title { get; set; } = "";

        public PlayerAchievements(XAttribute year, string title)
        {
            if (year.Value != null)
                Year = Convert.ToInt32(year.Value);
            else
                Console.WriteLine("Year is null");

            if (title != null)
                Title = title;
            else
                Console.WriteLine("Title is null");
        }
    }
}