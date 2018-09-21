# NoSingletons #
Allows multiple instances of singletons to be in an election for HA, only one processes at a time.

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

> Stuart Williams

* Cloud/DevOps Practice Lead
 
* Magenic Technologies Inc.
* Office of the CTO, National Markets
 
* <a href="mailto:stuartw@magenic.com" target="_blank">stuartw@magenic.com</a> (e-mail)
 
* Blog: <a href="http://blitzkriegsoftware.net/Blog" target="_blank">http://blitzkriegsoftware.net/Blog</a> 
* LinkedIn: <a href="http://lnkd.in/P35kVT" target="_blank">http://lnkd.in/P35kVT</a> 
* YouTube: <a href="https://www.youtube.com/channel/UCO88zFRJMTrAZZbYzhvAlMg" target="_blank">https://www.youtube.com/channel/UCO88zFRJMTrAZZbYzhvAlMg</a> 
