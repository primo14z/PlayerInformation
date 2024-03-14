Firstly in a terminal run the following command:

'docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.13-management'

Then when RabbitMQ is running open a terminal in the directory named: PlayerRegistrationSubscriber
and run the application with: 'dotnet run'
Lastly go to the directory PlayerRegistrationPublisher and run the same command in a terminal: 'dotnet run'
