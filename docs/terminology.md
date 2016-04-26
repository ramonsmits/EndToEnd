# Terminology

The permutation meeting agreed on a way on how to run the most value benchmark results and defined some terminology:

## Variable

An item that  has multiple values. Examples are:

- Transport
- Persistence
- DTC
- Serialization
- NServiceBus verion

In code this could be an `enum`

## Value

The possible values for a variable. Examples are:

- On, Off
- NServiceBus.RabbitMQ, NServiceBus.MSMQ
- 1,2,3,4,5
- Tiny, Small, Medium, Large
- 0%, 50%, 80%
- 5.2.17, 6.0.0

## Mode

The configuration of a transport. Examples are:

- Local, Remote
- Default, No ACK
- Connection Encryption, Large connection pool

## Environment

The identification of the environment. Examples are:

- On-Premise
- Azure
- Amazon

## Instance type

The type of machine on which the benchmark is run. Example are:

- A0
- D2
- G5
- t2.nano
- c4.2xlarge
- i2.2xlarge


## Category

A specific set of variable values. The following examples do not mention the defaults, only the values that influence the number of permutations

PAAS vs IAAS

- Mode: SQL Azure, SQL Remote
- Version: 5.2.x, 6.0.x

Outbox vs DTC

- Version: 5.2.x, 6.0.x
- DTC: On, Off
- Outbox: On, Off

Publish vs Send

- Version: 5.2.x, 6.0.x
- Transport: RabbitMQ, MSMQ, SQL, ASB, ASQ
- Persistence: InMemory, NHibernate, RavenDB

## Permutation

A specific combination of Value, Mode and Environment. This basically identies a specific benchmark configuration.

## Scenario

A specific sort test that can be run against a permutation. For example receiving messages, publishing messages or creating a Saga.
