# TempoInfiniteRun

Project uses Addressables and Unity Input System packages. 

## How to Play

- **Open** the `Start.unity` scene and press **Play**.

### Input Controls

- **Mobile:**  
  Swipe left/right to change lane, swipe up to jump.
- **Standalone:**  
  Press **A**, **D**, **Left Arrow**, or **Right Arrow** to change lane, and press **Space** to jump.
- **Consoles:**  
  Use the left stick to change lanes and the south button (X or A) to jump.

Collect coins as you run. If you hit an obstacle, the game is over, and you can restart the game.

## Tech Details

- The game starts from the `Start.unity` scene, which hides behind a loading screen that initializes the **PoolManager** and asynchronously loads the addressable `Gameplay.unity` scene.
- The codebase uses `async/await` extensively in C#.
- The **PoolManager** can be configured with an initial pool size.

![image](https://github.com/user-attachments/assets/3e10735e-a883-41d6-888d-34b289bfe52b)

## Game State Machine

- The game uses a simple game state machine implemented in `GameManager.cs`.
- Some states are implemented as partial classes of `GameManager`. In a more mature project, each state would be a separate class.

## Input

![image](https://github.com/user-attachments/assets/639b5a7d-6244-48ff-9fec-f1f2d7a955f6)

- The game uses the new Unity Input Package, with input handling logic in `PlayerController`.
- Game design parameters for lane changes, jumping, and running can be modified via the `PlayerControllerConfig` ScriptableObject. Since it's a ScriptableObject, you can change these values in the Editor and have them persist after you stop playing.

![image](https://github.com/user-attachments/assets/f28fa41e-eeb0-4ab9-8700-554c41bf304b)


## Level Generation

- The `LevelGenerator` script generates levels.
- Level configuration values are exposed via the `LevelConfig` ScriptableObject.

![image](https://github.com/user-attachments/assets/02b93afd-aa13-447f-af23-5c99af535603)
