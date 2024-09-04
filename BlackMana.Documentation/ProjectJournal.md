### Project Journal


#### <span style="color: dodgerblue">  Sunday, August 11th, 2024

Completed `BM-2` and `BM-7`.

Today, I also resolved the auto-load issue where I've tried to reference another auto-load within `ctor` instead of `_Ready`
which meant that the auto-load didn't get enough time to spawn in the scene tree.

#### <span style="color: dodgerblue">   Wednesday, September 5th, 2024

Today I've fixed to pesky bugs related to pathfinding. `aStarGrid` needs to have its `Update` method called to 
re-calculate its internal state. This was causing an issue with characters not being able to trace back previously 
trodden path.

I've also learned that it's best to have `SelectableManager` inherit from `Node` because otherwise it cannot use _Input 
override to listen for user inputs (and it's best that it does, because it makes sense for this class to handle 
selectable switching)

#### <span style="color: dodgerblue">   Wednesday, September 6th, 2024