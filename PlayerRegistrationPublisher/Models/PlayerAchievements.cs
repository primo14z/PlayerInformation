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
                throw new ArgumentNullException("Year is null");

            if (title != string.Empty)
                Title = title;
            else
                throw new ArgumentNullException("Title is null");
        }
    }
}