# sportsbet-code-test

add messageId and corrolationId in request header
add these headers to logging context

concurrency
- If the service is scalable then rely on load balancers to horizontally scale
- Publishing messages to queues is scalable
- Only real bottleneck here is the DB, but if we're using a fast Key-Value pair store thats optomised for speed then it should not be a problem.
- I've implemented optomistic concurrency check in the UpdateRace just to demo the concept, in this particular case it will not make any difference.

there is no explicit requirement to persist the race data, if I was to be lean I'd just transform and publish the data.
however, the requirement specified the PUBLICATION of EVENTS which reflect that something has already happened in our system. This is usually a result of an action performed on our domain which is responsible for persisting the source of truth for our domain data.
in the case this domain must be the source of truth for race data, and such it should persist all the state of the races.

given this ambiguity, you would normally seek clarification about the design of the overall system (for which is clearly just a small part) and find out where the source of truth should persist. so I asked about it duing the interview and was told that they left this problem deliberately vague and I would not get any clarity. 

so I would have to err on the side of caution and assume this service is the source of truth and persist the data.