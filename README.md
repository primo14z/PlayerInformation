1. terminal run the following command:
   `docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.13-management`
2. When RabbitMQ is running open a terminal in the directory named ___PlayerRegistrationSubscriber___
and run the application with: `dotnet run`

3. Go to the directory ___PlayerRegistrationPublisher___ and run the same command in a terminal: `dotnet run`
