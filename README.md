# NoSingletons #

Allows multiple instances of singletons to be in an election for HA, only one processes at a time.

Update to .NET 6

## Prerequisites

1. Have docker desktop or similar running
2. Invoke `Scripts\Redis-Start.ps1` to get REDIS running in your docker
3. Set start up project to `BlitzLib.RedisElector` and run

## Reason ##

In the cloud or enterprise, singleton processes are bad. Even if they are started automatically using something like a PCF Process Health Check it is still less than ideal.

So, this uses Redis to keep track of which instance of a process is the master.

## Sequence ##

*Setup*

1. Get REDIS info for this library
2. Create REDIS provider instance to leverage
3. Create `ElectorInfo` using the same Application Name, and a Unique instance name

*Loop*

* Before doing a unit of work, check to see if you are the master, if not, do nothing (return)


## About ##

* Stuart Williams
* <a href="mailto:Stuart.T.Williams@outlook.com" target="_blank">Stuart.T.Williams@outlook.com</a> (e-mail)
* LinkedIn: <a href="http://lnkd.in/P35kVT" target="_blank">http://lnkd.in/P35kVT</a> 
* YouTube: <a href="https://www.youtube.com/channel/UCO88zFRJMTrAZZbYzhvAlMg" target="_blank">https://www.youtube.com/channel/UCO88zFRJMTrAZZbYzhvAlMg</a> 
