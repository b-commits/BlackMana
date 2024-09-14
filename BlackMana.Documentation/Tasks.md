### Tasks

- [X] **BM-1**: Add the ability to select active interactable *aka* player or party member (will be useful for turn based combat)
    - Create an outlined texture variant
    - When a player is select, outline variant appears
    - Sets the active selectable
    - From now on, every mouse click on the grid affects the active selectable
- [ ] **BM-2**: Change the hue of a tile on hover, so that it is easier to
  determine what the affected selectable is
    - Tile hue should be changed programatically to avoid having to add
      too many alternative textures
    - Add the ability to double-click on a tile to confirm movement
- [X] **BM-3**: Implement scrollable camera that follows active selectable and move it to separate
scene
- [ ] **BM-4**: Implement a rudimentary dialogue box
- [X] **BM-5**: Add documentation project
- [X] **BM-6**: Rename project files
- [ ] **BM-7**: Implement debug mode (additional logging). Can use some kind of interceptor
- [ ] **BM-8**: Add a tween to smooth camera movement
- [X] **BM-9**: Implement physics-based movement | <span style="color:red">**P1**
- [X] **BM-10**: Implement outside-combat collisions | <span style="color:red">**P1**
- [X] **BM-11**</span>: Switch between characters with a button
- [ ] **BM-12**: Reset map state
- [X] **BM-13**: Move assets to separate folders
- [X] **BM-14**: Prevent switching selectables while another selectable is moving
- [ ] **BM-15**: Review whether I can put `MoveByPath` and `Move` code to `IMovable` from `Player` 
- [ ] **BM-16**: `GetAnimation(Vector2 nextMapPosition)`: Move this code to some kind of an `AnimationResolver`; does not belong to the player class
---

### Bugs

- [X] **BM-B-1**: Fix the resolution of movement animation
- [X] **BM-B-2**: Fix being able to have two active characters at the same time
- [X] **BM-B-3**: Fix coordinate resolution after movement re-work
- [X] **BM-B-4**: Fix character movement (incorrect tile getting selected)
- [X] **BM-B-5**: Fix an issue with character not being able to trace back
- [ ] **BM-B-6**: Fix selected export not being applied from editor