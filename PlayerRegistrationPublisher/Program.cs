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
            var exchangeName = "player.information";
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
                var playerId = 0;
                try
                {
                    var registrationEventModel = GetPlayerRegistration(player, eventId);
                    playerId = registrationEventModel.Id;

                    //Create registration event
                    var registrationEvent = new
                    {
                        event_type = "player_registration",
                        player = registrationEventModel
                    };

                    //Parsing into JSON
                    var registrationMessageBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(registrationEvent));

                    //Creating header properties
                    IBasicProperties properties = channel.CreateBasicProperties();
                    properties.Headers = CreateHeader(fileName, eventId, "achievements");

                    //Event publish
                    channel.BasicPublish(exchange: exchangeName, routingKey: "", basicProperties: properties, body: registrationMessageBody);
                    Console.WriteLine($"The event was succesfully published on exchange: {exchangeName}, with Id: {eventId} and body: {JsonConvert.SerializeObject(registrationEvent)}");
                }
                catch (ArgumentNullException e)
                {
                    throw new ArgumentNullException($"Event with id: {eventId}", e.InnerException);
                }
                catch (Exception e)
                {
                    throw new Exception($"Event with id: {eventId}", e.InnerException);
                }

                //Looping through achievements
                foreach (var achievement in player.Descendants("achievements"))
                {
                    var eventIdAchievement = achievement.GetHashCode();

                    var achievementList = GetPlayerAchievements(achievement, eventIdAchievement);

                    //Creating achievement event
                    var achievementEvent = new
                    {
                        event_type = "player_achievements",
                        player_id = playerId,
                        achievements = achievementList
                    };

                    try
                    {
                        var achievementMessageBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(achievementEvent));

                        //Creating headers
                        IBasicProperties achievementProperties = channel.CreateBasicProperties();
                        achievementProperties.Headers = CreateHeader(fileName, eventIdAchievement, "achievements");

                        //Event publish
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

    private static PlayerRegistration GetPlayerRegistration(XElement? player, int eventId)
    {
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
            throw new ArgumentNullException($"The XML schema for registration is wrong for event with id: {eventId}");
        }

        return new PlayerRegistration(playerId!, playerName!, playerAge!, playerCountry!, playerPosition!);
    }
    private static List<PlayerAchievements> GetPlayerAchievements(XElement achievement, int eventId)
    {
        var achievementList = new List<PlayerAchievements>();
        foreach (var item in achievement.Descendants("achievement"))
        {
            var achievementYear = item.Attribute("year");
            var achievementTitle = item.Value;

            if (achievementYear == null)
            {
                Console.WriteLine($"The XML schema for achievement is wrong for event with id: {eventId}");
                continue;
            }

            achievementList.Add(new PlayerAchievements(achievementYear, achievementTitle));
        }

        return achievementList;
    }
    private static Dictionary<string, object> CreateHeader(string fileName, int eventId, string eventType)
        => new Dictionary<string, object>() {
            { "fileName", fileName },
            { "eventId",  eventId},
            { "type", eventType} };
}