Random

# sportsbet-code-test

As a senior engineer, my first course of action was to ask questions and seek clarity regarding the tech test requirements during the cultural interview. However I was informed that they were deliberately vague. As such I decided to err on the side of caution and ignore the 2-3hr recommended effort in order to implement something more comprehensive whilst showcasing my capabilities.

## Problem interpretation
I'm building a webhook service for an external vendor to send race data. This service transforms the data, persist it, and finally publishes it to any downstream service that wish to consume it.

### webhook
It’s not clear what types of race updates we will get from the external vendor and how the data will be structured. Most webhooks have 1 endpoint and all requests have the same structure. Even though in the sample input the XML root object is called “RaceUpdate", I have assumed this object will be responsible for either Updating or Creating races. 

### data persistence
There are no explicit requirements to persist the race data and I had to choose between being Lean or assume we are following some good design principles. I have opted for the latter and persisted the race data in a simple key-value datastore. My reasons are
- race data seems important and should be persisted somewhere in our system, but where?
- there should be a single source of truth for this data (other stores are considered copies/projections)
- the requirements state that the service needs to PUBLISH race data to multiple systems
- according to CQRS concepts we publish EVENTS which reflect that something has happened to our domain
- if there is only 1 webhook service between the ingestion of external data and the publication of raced updated events, then this service must own the source of truth for race data and therefore should persist it

In the interest of time I have created a fake in memory static object to simulate the data store. In reality this will probably by an AWS DynamoDB.

### publish/subscript message brokers
In the interest of time I have used MediatR to simulate a message broker. In reality this will probably be AWS SNS.

### Integration tests
My integration tests were designed to test that the API, DB and MessageBroker were all wired up correctly. However since I was using fake DB stores and in memory message brokers, the test implementations are somewhat un-conventional, particularly with MediatR. The key concept I wanted to demonstrate was that in order to test that messages were correctly sent in an integration test, you may have to create a fake queue and fake handler to intercept the test message. 

### Concurrency and scaling
Here are a few things I considered when it comes to scaling
- the service does not persist any state and can be run in parallel, so this api can be horizontally scaled behind a load balancer
- the output events should be sent to durable message queues so consumers should easily be scalable
- the DB is a potential bottleneck which is why I have opted for a performant key-value DB
- I have also implemented an optimistic concurrency check when updating races. In this particular case, it may not make much difference but I wanted to demonstrate the concept

## Further TODOs
If I had more time I would work on the following
- add request middleware to generate messageId and corrolationId for every request
- add logging middleware to create logging context so that all logs will have corrolationId, messageId, applicationName etc....
- think about observability and possibly add counters and timers
- think about security and possibly enforce webhook authentication, data hashing, ip checks/whitelists
- you can't assume you will get all webhook requests, so whats the external vendor's retry strategy, maybe I have to build a tool to poll for updates as a backup.

## Build and run
Project was written in VS Code. Simple clone the solution and use "dotnet run" to run the API, or "dotnet test" to run either unit or integration tests

