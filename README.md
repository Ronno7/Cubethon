# Cubethon

Cubethon is a Unity obstacle-avoidance game **based on Brackeys' "How to make a Video Game" tutorial series**. The series provided the foundation: a cube that moves forward, sideways steering, obstacles, a following camera, distance scoring, and a level/menu flow. We then expanded the project with replay recording, design patterns, event-driven statistics and visual feedback, and revised end-of-run controls.

[Play the WebGL demo](https://ronno7.github.io/Cubethon/)

## Core game

Steer the cube around obstacles and reach the end of the road. Hitting an obstacle or falling off the road ends the attempt. The game includes a main menu, three levels, and a credits screen.

- **Movement:** `PlayerMovement` applies forward force and sideways steering to a Rigidbody during `FixedUpdate`.
- **Camera:** `FollowPlayer` follows the player's position at a fixed offset in `LateUpdate`.
- **Score:** `Score` displays the player's distance along the Z axis.
- **Level completion:** `EndTrigger` detects the player at the finish. The level sequence is `Level01` → `Level02` → `Level03` → `Credits`.

## Features and systems added afterward

### Replay recording — Command pattern

Each live physics step creates a `MoveCommand` containing the steering input. Commands implement `MovementCommand.Execute` and are stored in a list, so the same movement instructions can be executed again later.

After a win or loss, press **V** to watch the attempt. `PlayerMovement.BeginReplay` restores the player's starting position, rotation, velocity, and angular velocity before playing the commands back through the same movement code. `GameManager` preserves the original win/loss result and displays it again when playback finishes. Replay collisions and finish triggers do not create new results or add to the crash count.

This records movement instructions in memory, rather than saving video or a position for every frame. The recording belongs to the current attempt and is discarded when the scene reloads. You can replay it repeatedly before retrying or leaving the level.

### Live/replay switching — Strategy pattern

[`MovementStrategies.cs`](Cubethon/Assets/Scripts/MovementStrategies.cs) separates the two ways of supplying movement commands behind `IMovementStrategy.TryGetCommand`:

| Strategy | How it works |
| --- | --- |
| `LiveMovementStrategy` | Creates a command from the current steering input and adds it to the recording. |
| `ReplayMovementStrategy` | Returns recorded commands in order, ignores live steering, and signals when playback is finished. |

`PlayerMovement` acts as the context. It selects the live strategy in `Awake` and switches to a fresh replay strategy in `BeginReplay`. Both use the same command execution path in `FixedUpdate`; creating a fresh replay strategy resets playback to the first command.

The patterns work together: **Strategy chooses where the next command comes from; Command represents the movement to execute.** No extra scene or Inspector setup is needed for the strategies.

### Statistics and visual feedback — Observer pattern

`GameManager` publishes C# events for `RunStarted`, `PlayerCrashed`, `RunEnded`, and `ReplayStateChanged`. Separate observer components subscribe to the events they need, keeping their display and tracking logic out of the game manager.

| Feature | Basic implementation |
| --- | --- |
| Session collision counter | `CrashCounterObserver` listens for `PlayerCrashed` and increments a static counter that survives level changes and retries within the session. Falls and replay collisions do not count. |
| Attempt timer | `LevelTimerObserver` starts on `RunStarted` and stops on `RunEnded`. It displays the live attempt duration and preserves that value during replay. |
| Replay color | `ReplayTintObserver` listens for `ReplayStateChanged`, gives the player a blue tint during playback using a `MaterialPropertyBlock`, and restores the original appearance afterward. |

`ObserverSetup` automatically attaches missing observers to each scene's game manager when the scene loads. Observers subscribe in `OnEnable` and unsubscribe in `OnDisable`.

### End-of-run UI and manual navigation

The original automatic end screens are replaced by a centered result panel drawn by `GameManager.OnGUI`. It shows **LEVEL COMPLETE**, **GAME OVER**, or **REPLAY**, along with the controls available in that state. Wins use a green panel; losses use red.

At the end of an attempt, movement stops and the score freezes. The player chooses whether to replay, retry, return to the menu, or advance after a win. `GameManager` guards against processing an attempt's result more than once and only allows next-level navigation after a completed win.

### Input, scene loading, and platform support

- **Keyboard backend support:** `GameInput` and `GameManager` use conditional compilation to support Unity's newer Input System or the legacy Input Manager, preferring the newer system when both are enabled. `UIInputSetup` supplies the corresponding input module for menu buttons.
- **Scene loading:** `SceneLoader` loads scenes by name, checks that the requested scene is available in the build, and resets the time scale before loading.
- **Platform-aware quit:** `QuitApplication` stops Play Mode in the Unity Editor, returns to the menu in WebGL, and quits standalone builds.
- **Browser build:** The repository includes the WebGL build and page used for the hosted demo. Source changes require a new WebGL build to appear in the demo.

## Controls

| Key | Action |
| --- | --- |
| A / Left Arrow | Steer left |
| D / Right Arrow | Steer right |
| V | Replay a finished attempt when a recording is available |
| R | Restart the current level |
| Esc | Return to the main menu |
| Enter / Numpad Enter | Advance after winning a level |

## Credits

Thanks to **Brackeys** for the "How to make a Video Game" series that this project is based on. The replay system, Command/Observer/Strategy implementations, statistics, replay feedback, and revised result flow described above are extensions built on that foundation.
