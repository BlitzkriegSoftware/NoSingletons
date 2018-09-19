# NoSingletons #
Allows multiple instances of singletons to be in an election for HA, only one processes at a time.

## Reason ##

In the cloud or enterprise, singleton processes are bad. Even if they are started automatically using something like a PCF Process Health Check it is still less than ideal.

So, this uses Redis to keep track of which instance of a process is the master.

Sequence:

1. Get REDIS info for this library
2. Create REDIS provider instance to leverage
3. Create `ElectorInfo` using the same Application Name, and a Unique instance name
4. Before doing a unit of work, check to see if you are the master, if not, do nothing (return)