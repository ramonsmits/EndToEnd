@johnsimons raised a question regarding the memory usage of NSB v6. I did a quick run under small load

## Setup

Azure Standard DS4 (8 cores, 28 GB memory)
199820 messages in roughly 2 h 20 minutes ~ 24 msgs/s
sent in bursts up to 10000 message (randomly generated) in intervals of 30 up to 300 seconds
Sender is using file based routing, routing table is updated every 5 seconds to trigger the file watcher
Receiver side is scaled out
A really small amount of messages failing
Msmq, Standard Concurrency of 100, XmlSerializer, InMemoryPersistence, only message handling

[Code](https://drive.google.com/open?id=0Bxf5pxyNKtQVNThsSVpfZGNmbHM)

![199820messages](https://cloud.githubusercontent.com/assets/174258/14021758/2ece899a-f1dd-11e5-9808-e62aa1e52c34.png)

Memory Dumps can be found [here](https://drive.google.com/open?id=0Bxf5pxyNKtQVUWtxTnhaXzZRams)

All results, screenshots etc. uploaded to [GDrive](https://drive.google.com/drive/u/0/folders/0Bxf5pxyNKtQVc19ZcTUxM1dXeG8).

## Sender

![sender cpu](https://cloud.githubusercontent.com/assets/174258/14021898/aefaa34c-f1dd-11e5-866b-c21eb1fa53a1.png)
![sender memory](https://cloud.githubusercontent.com/assets/174258/14021896/aef90fdc-f1dd-11e5-9cdd-7eb3aef097c2.png)
![sender performancecharts](https://cloud.githubusercontent.com/assets/174258/14021897/aefaadf6-f1dd-11e5-8ad1-b260a0945d77.png)
![sender summary](https://cloud.githubusercontent.com/assets/174258/14021899/aeff3114-f1dd-11e5-8509-a0d969946a9c.png)

## Receiver 1

![receiver 1 cpu](https://cloud.githubusercontent.com/assets/174258/14021909/bfd139b0-f1dd-11e5-8816-050c61116cb7.png)
![receiver 1 memory](https://cloud.githubusercontent.com/assets/174258/14021910/bfd49c7c-f1dd-11e5-9543-2181381c8370.png)
![receiver 1 performancecharts](https://cloud.githubusercontent.com/assets/174258/14021911/c00f8c2e-f1dd-11e5-99ca-9fa14b7ea4c0.png)
![receiver 1 summary](https://cloud.githubusercontent.com/assets/174258/14021912/c018e6ac-f1dd-11e5-884d-a150a6175d48.png)


## Receiver 2

![receiver 2performancecharts](https://cloud.githubusercontent.com/assets/174258/14021927/c9d896a6-f1dd-11e5-9bc4-60760d4ca85f.png)
![receiver 2 cpu](https://cloud.githubusercontent.com/assets/174258/14021928/c9d90ec4-f1dd-11e5-881c-04559fb279fd.png)
![receiver 2 memory](https://cloud.githubusercontent.com/assets/174258/14021929/c9dd69ce-f1dd-11e5-8cd9-ffd79d0dffa9.png)
![receiver 2 summary](https://cloud.githubusercontent.com/assets/174258/14021926/c9d72348-f1dd-11e5-98ac-13d222e399fb.png)
