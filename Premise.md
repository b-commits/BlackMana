### General premise:

Isometric 2D narrative-heavy game inspired by DE with occasional turn and grid based tactics combat.
- Smooth CRPG like movement outside combat;
- Players are able to interact with the objects to get some more detailed information about them (shown in the dialogue box)
- Dialogue box occupies the right hand side of the screen;
- Players are able to enter in dialogue with other characters in the scene and accept quests;
- Players are able to manage the skills they can use during and outside combat;
- Players are able to interact with the inventory;
- Players can switch between party members **but** during exploration only one character is visible on the screen
- Whenever player enters combat, a grid and an action bar appears and the game is now turn-based
- Players can select other enemy (interactable) 
- Players can switch scenes (location) animated by a scene

*In combat:* 
- Selecting party member, selecting a skill and then selecting another interactable object
- Skills are displayed on the action 
- Only one character can move at a time 

---

### To do list:

- [x] **BM-1**: Fix character movement (incorrect tile getting selected).
- [ ] **BM-2**: Add the ability to select active interactable *aka* player or party member (will be useful for turn based combat)
  - Create an outlined texture variant
  - When a player is select, outline variant appears
  - Sets the active selectable
  - From now on, every mouse click on the grid affects the active selectable
- [ ] **BM-3**: Change the hue of a tile on hover, so that it is easier to
determine what the affected selectable is
  - Tile hue should be changed programatically to avoid having to add 
too many alternative textures 
- [ ] **BM-4**: 
- [ ] **BM-5**: 
- [ ] **BM-6**: 


