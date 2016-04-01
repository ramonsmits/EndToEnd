# Variables

## Environments

- [ ] Azure
- [ ] AWS (https://aws.amazon.com/ec2/instance-types/)

RavenDB

- [ ] Local
- [x] Remote (3.x)

SQL Server

- [ ] Local
- [x] Remote (SQL2012, because of enterprise adoption)
- [ ] SQL Azure

PostgreSQL

- [ ] Local
- [ ] Remote

Oracle

- [ ] Local
- [ ] Remote

RabbitMQ

- [ ] Local
- [x] Remote 

## Configurations

Logging

- [x] Lowest level

NServiceBus

- [ ] 4.x
- [x] 5.x
- [x] 6.x

Transports:

- [x] MSMQ
- [x] RabbitMQ
- SqlServer
 - [ ] 1.x
 - [x] 2.x
 - [x] 3.x
- [x] ASQ
- [x] ASB

Persistence Multiple major versions per core major version e.g. NServiceBus V5 - SQL Transport 1.x &  NServiceBus V5 - SQL Transport 2.x

(which versions?)

- [x] NHibernate 
- [x] RavenDB
- [x] InMemory
- [ ] MSMQ

Serialization

- [x] XML
- [x] JSON
- [ ] Binary??

Auditing

- [x] On
- [x] Off

Encryption

- [ ] On
- [x] Off

Message size:

- [x] Tiny (Headers only, empty message)
- [x] Small (1kb)
- [x] Medium (10kb)
- [x] Large (64kb)

Outbox

- [x] On
- [x] Off

Transactional modes

http://docs.particular.net/nservicebus/messaging/transactions

**All combinations**

- [x] DTC
- [x] No transactions
- [x] Native
- [x] Receive only (Transport transaction)
- [x] Unreliable (Transactions Disabled)
- [x] Sends atomic with Receive

Persistence TransactionScope

**Is this supported in V6?**

- [ ] On
- [ ] Off

Concurrency

- [ ] 1
- [x] Environment CPU count?
- [ ] 4 * Environment CPU count?

Operations

- [x] Publish
- [x] SendLocal
- [x] Send (same endpoint)
- [ ] Send (other endpoint)
- [ ] Send with destination
- [ ] Durable messages (Express?)
- [ ] Defer ?

Sagas

- [ ] Concurrency

FLR

- [ ] On
- [x] Off

SLR

- [ ] On
- [x] Off

Forward to error queue

- [x] On
- [ ] Off

Hosting

- [ ] AppDomain
- [ ] External process

.NET Architecture

- [x] x86
- [x] x64

gcServer

- [x] On
- [x] Off

Disk speed

- [ ] HDD (Max 250 IOPS)
- [ ] SSD (Min 15,000 IOPS)
- [ ] SSD M.2 (Min. 75,000 IOPS)
- [ ] SAN  

## Out of scope?

- Timeouts
- Containers
- Distributor
- Gateway
- DataBus
- Callbacks (synchronous request / response)
- NGen / JIT


