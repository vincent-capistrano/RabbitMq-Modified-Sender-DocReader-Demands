<<<<<<< HEAD
# RabbitMq-Modified-Local-Sender-DocReader-Demands
RabbitMq sender program DocReader/ Demands
=======
# LocalSender Module
The LocalSender module is at its core, a very light sender module intended to be a simple "plug-and-play" tool created for locally testing other modules that contain methods and functions that consume messages from a queue.

To be able to get the LocalSender to properly communicate with your consumer module and be able to test them, they both need to be written using the required libraries listed below.<br><br>

## Requirements:
- MassTransit
    * The LocalSender is built to make use of **MassTransit**. It will do all the necessary handling and management of the messages that get sent to and from all the queues inside the bus. What this means is that if you are going to use the LocalSender to test a consumer inside your project, it will need to be written to publish/subscribe to queues using the MassTransit library too, or else the messages will not be sent to the proper bus.

- RabbitMQ
    * **RabbitMQ** is essentially a message and streaming broker. This is what we will be running for local testing. Although it is also meant to handle message delivery and reception like MassTransit, it will not be used like so in this scenario. We will only be using RabbitMQ to act as the 'server host' where all the messages will be sent and processed, similar to a client-server setup.
    The RabbitMQ server will run on your host locally, which is done to emulate the 'client' sending a message (in this case, the LocalSender) to a particular queue, where a 'server' is waiting to read and process the messages in the same queue (this is the project with consumer module you are going to test).
    MassTransit is capable of using RabbitMQ as the host for the message brokering, and as such has an optional RabbitMQ integration package which we will be using.

- Serilog
    * **Serilog** is a useful tool that allows structured logging of everything in our module. It's what we're going to use to log the status of a message. 
    The main reason we use Serilog is because it is capable of also writing the logs to an Elasticsearch sink, which is what we will be using to log and better track everything the program will be doing. The logging is mainly for monitoring the 'journey' of the messages we will be sending, from the moment of its creation all the way until it reaches its last destination, but it is also used for identifying the causes of errors. This way we can understand the ***what***, ***where***, and ***why*** of everything whenever it works as intended, or whenever something goes wrong.<br><br>
    In hindsight, logging is an indispensable tool that helps us in debugging whenever a message *goes missing* or whenever exceptions occur.<br><br>

## Importing the LocalSender Module:

If the requirements above have all been satisfied, we can move on to setting up the LocalSender module. You can get started importing the LocalSender by downloading its entire project folder via a browser, or by simply cloning its repository to your local machine. The repository is named ***PracticeAI/BackendModule-LocalSender***.<br><br>

## Configuring the LocalSender Module:
You are going to need do two things here: 
+ Configuring the `queueName` variable, and
+ Matching the object type received by the consumer

Before you can run it, you need to do some minor coding by making changes to a variable inside the `Program.cs` file. This variable is named `queueName`, which is a string that will contain the name of the queue the consumer project you’re working on is subscribed to. Go ahead and open the `Program.cs` file and look for the specific line below. It has a big block of comments above it, so you definitely won't miss it:

```
string queueName = "YourQueueName";
```

You need to replace `"YourQueueName"` with the right queue name the consumer module you're testing is referencing. So, if your consumer is subscribed to a queue named `"DocumentsQueue"` for example, the `queueName` variable should look something like this:

```
string queueName = "DocumentQueue";
```

***(NOTE: Queue names are case-sensitive. The sender needs to subscribe to the <u>exact same queue name</u> as the consumer or else the messages will get lost and be sent to a different queue than the one your consumer is expecting.)***

The last thing you need to do for this part is to just make sure you match the object type sent by the sender module to the object type received in the consumer module.<br><br>
The sample object type to be sent is defined inside the `/Sender/Models/Message.cs` inside the LocalSender repository and it looks like this:<br>
```
namespace DocumentApi.Models
{
    public class Message
    {
        public string? Content { get; set; }
    }
}
```
This existing class file inside the LocalSender module is a very simple object class named `Message` with one attribute- a string named `Content` that will serve as the container for the messages that will be inputted from the console.<br><br>
This model may differ from the model you intend to use on the consumer model. If so, since we are only testing if they properly connect to the queues anyway, I advise temporarily modifying the object on the consumer class instead, as the LocalSender module can only read inputs from the console and can only parse strings, and be sure to also import the namespace `DocumentApi.Models` at the top of the consumer class file.<br><br>
Locate the consume function somewhere within the class file in your consumer module project. It should look similar to the code below, just with different object names in the parameters:
```
public Task Consume(ConsumeContext<MessageContent> message)
        {
            // Some Other Logic
            Console.WriteLine("Received message: {0}", message.MessageContent.ID);
            return Task.CompletedTask;
        }
```
Change the expected objects in the code to be of the sample object class `Message` above. It should now look similar to the one below after modifying it:
```
using DocumentApi.Models;

public Task Consume(ConsumeContext<Message> message)
        {
            // Some Other Logic
            Console.WriteLine("Received message: {0}", message.Message.Content);
            return Task.CompletedTask;
        }
```
Afterwards, you can just revert the changes you made to the object class in the consumer module when you finish testing, so just verify that the consumer application is building properly after making the changes.<br><br>
The hard part is pretty much over, and all that's left to do now is test the consumer and see if it's getting your messages and processing them properly.<br>

One last thing before moving on to the next part, you need to ensure that the RabbitMQ Service is running. This is the `RabbitMQ Service-start.exe` if you used an installer on Windows. (Verify that the RabbitMQ image is actively running if you are using a docker instead.) Remember to consult with the appropriate RabbitMQ documentation on the proper installation of whichever method you're going for should you encounter issues. You can determine if the RabbitMQ Service is running by quickly checking whether the RabbitMQ Interface is accessible.


## The RabbitMQ Interface:
To check if the service is actively running, open a browser and type in ***localhost:15672*** in the address bar. This is the default port for the RabbitMQ service, so if you changed the default assigned port for the RabbitMQ service during installation to a different one, be sure to also modify the port number accordingly. <br><br>
Once it is actively running, you should see the RabbitMQ interface. If the RabbitMQ Service is not running, this page will be unreachable. After verifying that you can reach the page, you can now move on to running the sender module.<br><br>
***NOTE: You do not explicitly need the RabbitMQ interface to test your sender or consumer and is entirely **optional**. If the logging is properly set up in your consumer module, you can just monitor whether or not the message got consumed via the message logs in the terminal.***<br><br> 
However, the RabbitMQ interface is an extremely useful tool to see the details and status of the messages and can be utilized to monitor the queues we are referencing when running sender or consumer modules. <br><br>
You can access it by logging in to the interface using ***guest*** for both username and password. The **Queues and Streams** tab will display all the active and inactive queues present in your local machine, so when you run either a sender or a consumer module, the queue they reference will automatically show up here. This interface is primarily used to verify that the queue exists and if it’s properly receiving and consuming the messages you will be sending with the LocalSender module.<br><br>
You could also view the messages inside these queues individually and even view their contents. Keep in mind that *getting* the messages this way <u>**consumes them from the queue**</u>, and they will **no longer be received by any consumers** subscribed to that particular queue, so be careful.  <br><br>
## Running the LocalSender Module:
Once all set, you are going to run the LocalSender via the terminal so go ahead and open one and change the current target directory in the terminal to the `/Sender/` folder inside of where the LocalSender project was downloaded (if you did it manually). If you cloned the repository instead, opening the repository with any IDE of your choice and running a terminal inside the IDE will automatically set the target directory to the current folder containing the LocalSender project. Simply change the directory to the `/Sender/` folder, which contains the `Sender.csproj` file required to run the sender module.

To run the module, type `dotnet run` in the terminal and the LocalSender should start to build and run. Take note that at this point, the consumer model you will be testing should already be running in the background waiting for messages.

After it finishes compiling, you should see a similar line in the terminal:

```
PS \BackendModule-LocalSender\Sender> dotnet run
Enter message to send to the queue:

```

The LocalSender is now running and is waiting for inputs. LocalSender is made to send the messages by inputting them via the console, so simply type the message you wish to send on the terminal directly and hit the Enter key:

```
PS \BackendModule-LocalSender\Sender> dotnet run
Enter message to send to the queue:
Hello World
```

After sending, the logger should display the message itself and that it was sent successfully, along with a date and timestamp of the message:

```
PS \BackendModule-LocalSender\Sender> dotnet run
Enter message to send to the queue:
Hello World
2024-01-01 00:00:00.00 +08:00 [INF] Sent message to queue: Hello World
Enter message to send to the queue:
```

The verification of whether the message you just sent was processed correctly is entirely dependent on the way the consumer module you are testing is written. If the consumer module creates its own log messages upon consuming, then you should verify them there.<br><br>

## Closing the LocalSender Module:
A MassTransit bus instance is operating so long as the sender module is running, so don’t forget to close it once you are done using it to free up system memory. <br>
To close the sender module, you can simply kill the terminal. Alternatively, you can hit the Enter key again without typing any messages to terminate the LocalSender and still have the terminal open.<br><br>


## Info:
Info and module written by<br> ***Pau Intia***, on 2024 April <br>
**PracticeAI - Backend Team** <br>

For questions, clarifications, or help about this module, contact me on **Slack** or E-mail me at **pau@mylawfirm.ai**<br><br><br>
>>>>>>> ae7fb6f (Initial commit)
