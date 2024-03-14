using Newtonsoft.Json;
using PlayerRegistrationPublisher.Models;
using RabbitMQ.Client;
using System.Text;
using System.Xml.Linq;

// TODO: Add comments

public class Program
{
    private static void Main(string[] args)
    {
        var fileName = "players.xml";
        XDocument doc = XDocument.Load(fileName);

        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            //Creating exchange and Queue for player registration and achievements
            var exchangeName = "PlayerInformation";
            channel.ExchangeDeclare(exchange: exchangeName, type: "headers", durable: true);
            channel.QueueDeclare("player.information.registration");
            channel.QueueDeclare("player.information.achievements");

            channel.QueueBind("player.information.registration", exchangeName, "", new Dictionary<string, object>
            {
                { "type", "registration"}
            });

            channel.QueueBind("player.information.achievements", exchangeName, "", new Dictionary<string, object>
            {
                { "type", "achievements"}
            });

            //Looping through the xml
            foreach (var player in doc.Descendants("player_registration"))
            {
                var eventId = player.GetHashCode();

                var playerId = player.Element("id");
                var playerName = player.Element("name");
                var playerAge = player.Element("age");
                var playerCountry = player.Element("country");
                var playerPosition = player.Element("position");

                if (playerId == null ||
                playerName == null ||
                playerAge == null ||
                playerCountry == null ||
                playerPosition == null)
                {
                    Console.WriteLine($"The XML schema for registration is wrong for event with id: {eventId}");
                    continue;
                }

                var registrationEventModel = new PlayerRegistration(playerId!, playerName!, playerAge!, playerCountry!, playerPosition!);

                //Create registration event
                var registrationEvent = new
                {
                    event_type = "player_registration",
                    player = registrationEventModel
                };
                
                try
                {
                    //Parsing into JSON
                    var registrationMessageBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(registrationEvent));

                    //Creating header properties
                    IBasicProperties properties = channel.CreateBasicProperties();
                    properties.Headers = new Dictionary<string, object>
                    {
                        { "fileName", fileName },
                        { "eventId", eventId },
                        { "type", "registration"}
                    };

                    channel.BasicPublish(exchange: exchangeName, routingKey: "", basicProperties: properties, body: registrationMessageBody);
                    Console.WriteLine($"The event was succesfully published on exchange: {exchangeName}, with Id: {eventId} and body: {JsonConvert.SerializeObject(registrationEvent)}");
                }
                catch (Exception e)
                {
                    throw new Exception($"Event with id: {eventId}", e.InnerException);
                }

                //Looping through achievements
                foreach (var achievement in player.Descendants("achievements"))
                {
                    var eventIdAchievement = achievement.GetHashCode();

                    var achievementList = new List<PlayerAchievements>();
                    foreach (var item in achievement.Descendants("achievement"))
                    {
                        var achievementYear = item.Attribute("year");
                        var achievementTitle = item.Value;

                        if (achievementYear == null)
                        {
                            Console.WriteLine($"The XML schema for achievement is wrong for event with id: {eventIdAchievement}");
                            continue;
                        }

                        achievementList.Add(new PlayerAchievements(achievementYear, achievementTitle));
                    }

                    //Creating achievement event
                    var achievementEvent = new
                    {
                        event_type = "player_achievements",
                        player_id = playerId.Value,
                        achievements = achievementList
                    };               

                    try
                    {
                        var achievementMessageBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(achievementEvent));

                        //Creating headers
                        IBasicProperties achievementProperties = channel.CreateBasicProperties();
                        achievementProperties.Headers = new Dictionary<string, object>
                        {
                            { "fileName", fileName },
                            { "eventId",  eventIdAchievement},
                            { "type", "achievements"}
                        };
                        channel.BasicPublish(exchange: exchangeName, routingKey: "", basicProperties: achievementProperties, body: achievementMessageBody);
                        Console.WriteLine($"The event was succesfully published on exchange: {exchangeName}, with Id: {eventIdAchievement} and with body: {JsonConvert.SerializeObject(achievementEvent)}");
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Event with id: {eventIdAchievement}", e.InnerException);
                    }
                }
            }
        }
    }
}