@johnsimons raised a question regarding the memory usage of NSB v6. I did a quick run under medium load

## Setup

Azure Standard DS4 (8 cores, 28 GB memory)
795487 messages in roughly 41 minutes ~ 323 msgs/s
sent in bursts up to 10000 message (randomly generated) in intervals of 10 up to 30 seconds
Sender is using file based routing, routing table is updated every 5 seconds to trigger the file watcher
Receiver side is scaled out
A really small amount of messages failing
Using 3 senders this time
Msmq, Standard Concurrency of 100, XmlSerializer, InMemoryPersistence, only message handling

[Code](https://drive.google.com/open?id=0Bxf5pxyNKtQVc003QzJsNTNUZm8)

![795487messages](https://cloud.githubusercontent.com/assets/174258/14024240/9616b540-f1e8-11e5-8458-228800aaad11.png)


All results, screenshots etc. uploaded to [GDrive](https://drive.google.com/drive/u/0/folders/0Bxf5pxyNKtQVQ1pPMHV4UWFLWWs).

## Sender

![sender cpu](https://cloud.githubusercontent.com/assets/174258/14024298/cc732768-f1e8-11e5-9403-9d564f747826.png)
![sender memory](https://cloud.githubusercontent.com/assets/174258/14024301/cc785eb8-f1e8-11e5-9ce8-cdfebb81f748.png)
![sender performancecharts](https://cloud.githubusercontent.com/assets/174258/14024300/cc7803aa-f1e8-11e5-91ad-64ac5e3bf8a3.png)
![sender summary](https://cloud.githubusercontent.com/assets/174258/14024299/cc73f36e-f1e8-11e5-8d0e-74b6768f0526.png)

## Receiver 1

![receiver 1 cpu](https://cloud.githubusercontent.com/assets/174258/14024314/dac3ce6c-f1e8-11e5-87ec-6d6adeea74cf.png)
![receiver 1 memory](https://cloud.githubusercontent.com/assets/174258/14024315/dac44234-f1e8-11e5-98ed-0dc204e5f2b0.png)
![receiver 1 performancecharts](https://cloud.githubusercontent.com/assets/174258/14024317/dac5d982-f1e8-11e5-9d7d-c0defca79464.png)
![receiver 1 summary](https://cloud.githubusercontent.com/assets/174258/14024316/dac53644-f1e8-11e5-9dc5-45787d50cc73.png)

## Receiver 2

![receiver 2 summary](https://cloud.githubusercontent.com/assets/174258/14024326/e1a27e54-f1e8-11e5-9858-03b383b395d4.png)
![receiver 2 cpu](https://cloud.githubusercontent.com/assets/174258/14024328/e247c13e-f1e8-11e5-869a-33c35b7abbbe.png)
![receiver 2 memory](https://cloud.githubusercontent.com/assets/174258/14024329/e2683770-f1e8-11e5-94b2-26920409936b.png)
![receiver 2 performancecharts](https://cloud.githubusercontent.com/assets/174258/14024330/e2869c1a-f1e8-11e5-9742-4f6cbf467a49.png)
