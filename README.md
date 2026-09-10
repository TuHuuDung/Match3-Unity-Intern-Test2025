# Match3-Unity-Intern-Test2025

# Unity version

Unity 2020.3.38f1

------------------------------------------------------------------------------------------------------------------------------------------------------------

# TASK 1: RE-SKIN

The original item sprites were replaced with the new fish asset pack.

## New Assets

The new fish assets are located in:

`Assets/Textures/Fish/`

The asset pack contains:

- fish_1
- fish_2
- fish_3
- fish_4
- fish_5
- fish_6
- rainbow_fish

## Asset Mapping

| Original Item | New Asset    |
|---------------|--------------|
| itemNormal01  | fish_1       |
| itemNormal02  | fish_2       |
| itemNormal03  | fish_3       |
| itemNormal04  | fish_4       |
| itemNormal05  | fish_5       |
| itemNormal06  | fish_6       |
| itemNormal07  | rainbow_fish |

The original prefab names were kept unchanged. Only the Sprite Renderer references were changed to use the new fish sprites.

--------------------------------------------------------------------------------------------------------------------------------------------------------------

# TASK 2: CHANGE THE GAMEPLAY

## Goal

The original game used a drag-to-swap Match-3 gameplay system.

The gameplay was changed to a tap-to-collect system.

The player now taps an item on the board to move it into a 5-slot bottom tray.

When 3 identical fish are collected in the tray, those 3 fish are cleared.

The player wins when the entire board is cleared.

The player loses when all 5 bottom tray cells are occupied without making a valid triple.

## Requirement 1: Tap an Item to Move It to the Bottom

The original drag and swap system was removed.

The player can now tap an item on the board.

When an item is tapped:

1. The item is removed from its board cell.
2. The item moves to the first available bottom tray cell.
3. The item cannot be moved back to the board in the normal game mode.

## Requirement 2: Bottom Tray

The game now contains 5 bottom cells.

Example:

[ Fish ] [ Fish ] [ Empty ] [ Empty ] [ Empty ]

Items are placed from left to right into the first available cell.

## Requirement 3: Match Three Items

When 3 identical fish are present in the bottom tray, the 3 fish are automatically cleared.

Example:

[ Fish 1 ] [ Fish 2 ] [ Fish 1 ] [ Fish 1 ] [ Empty ]

The three Fish 1 items are cleared.

## Requirement 4: Win Condition

The player wins when all items have been removed from the board.

The existing win UI is used to display the result.

## Requirement 5: Lose Condition

The player loses when all 5 bottom tray cells are occupied and no group of 3 identical items can be cleared.

The lose/game-over UI is displayed when this condition occurs.

## Requirement 6: Initial Board

The initial board is generated so that the number of each fish type is divisible by 3.

For example:

- Fish 1 x 3
- Fish 2 x 3
- Fish 3 x 3
- Fish 4 x 3
- Fish 5 x 3
- Fish 6 x 3
- Rainbow Fish x 3

This allows every fish type to be completely cleared in groups of three.

## Autoplay

An Autoplay mode was added to the Home screen.

When Autoplay is selected, the game automatically chooses items and moves them to the bottom tray.

There is a 0.5 second delay between automatic actions.

The Autoplay mode continues until the board is completely cleared and the player wins.

## Auto Lose

An Auto Lose mode was also added.

The automatic player intentionally chooses different fish types so that the 5 bottom tray cells become occupied.

When the tray becomes full, the game ends with a loss.

There is a 0.5 second delay between automatic actions.

--------------------------------------------------------------------------------------------------------------------------------------------------------------------

# TASK 3: IMPROVE THE GAMEPLAY

## Requirement 1: All Fish Types Must Appear

The initial board was improved so that every available fish type appears on the board.

Each fish type is placed in a group of three.

The board then fills any remaining cells using additional valid groups.

This guarantees that all fish types are represented when the board has enough cells.

## Requirement 2: Move and Clear Animations

Animations were added to improve the gameplay experience.

### Move Animation

When the player taps a fish:

`Board -> Bottom Tray`

The fish smoothly moves to its bottom tray position.

### Clear Animation

When three identical fish are matched, they shrink smoothly until they disappear.

The clear animation scales the fish down to zero before destroying it.

## Requirement 3: Time Attack Mode

A new Time Attack mode was added to the Home screen.

The player has 60 seconds to clear the board.

### Time Attack Rules

- The game uses the same tap-to-collect gameplay.
- The bottom tray contains 5 cells.
- Items can be moved from the board to the tray.
- In Time Attack mode, the player can tap an item in the tray to return it to its original board position.
- Filling all 5 tray cells does not immediately cause a loss.
- The player loses when the 60-second timer expires.
- The player wins by clearing the entire board before the timer expires.

## Home Screen

The Home screen now contains the following game modes:

- Normal Game
- Autoplay
- Auto Lose
- Time Attack

## Files Changed

| File                 | Purpose                                                              |
|----------------------|----------------------------------------------------------------------|
| `GameSettings.cs`    | Added bottom tray count and Time Attack duration                     |
| `Cell.cs`            | Added tray cell identification                                       |
| `Board.cs`           | Added new board generation and board state checking                  |
| `Item.cs`            | Updated clear animation                                              |
| `BottomTray.cs`      | New bottom tray management                                           |
| `BoardController.cs` | New tap gameplay, matching, win/lose, autoplay and Time Attack logic |
| `GameManager.cs`     | Added separate WIN/LOSE states and new game modes                    |
| `UIPanelWin.cs`      | Win screen                                                           |
| `UIPanelLose.cs`     | Lose screen                                                          |
| `UIPanelMain.cs`     | Added Home screen buttons                                            |
| `UIMainManager.cs`   | Connected the new game mode buttons                                  |


------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

## Task Status

### Task 1 - Reskin

- [x] Import fish asset pack
- [x] Replace original sprites
- [x] Keep original prefab structure

### Task 2 - Change Gameplay

- [x] Tap items to move them to the bottom
- [x] Prevent items from returning to the board
- [x] Clear 3 identical items
- [x] Win when the board is cleared
- [x] Lose when the 5-slot tray is full
- [x] Initial item counts are divisible by 3
- [x] Add Autoplay
- [x] Add Auto Lose
- [x] Add 0.5 second autoplay delay

### Task 3 - Improve Gameplay

- [x] Include all fish types on the initial board
- [x] Add move animation
- [x] Add clear animation
- [x] Add Time Attack mode
- [x] Add 60 second Time Attack timer
- [x] Allow tray items to return to the board in Time Attack mode