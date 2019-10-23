# Networking architecture

Client and server work independently with their own entity model and **reconcile** their predictions. Server is an **authorative central** server.

## Message types

### Input (client -> server)

```
Input {
    speed: none/forward/backward (SpeedState enum)
    fire: on/off
    direction: none/left/right (TurnState enum)
    turretDirection: float (0-2PI)
}
```

### Connect (client -> server)

```
Connect {
    name: string
}
```

### Connect response (client <- server)

```
ConnectResponse {
    approved: bool (will be true almost always, maybe implement room limit)
    playerEntityId: string
}
```

### GameState (client <- server)

```
GameState {
    playerEntityId: string <- why not
    entities: GameObject[] <- list
        GameObject representation minimum required (id:guid, position: Vector2, speed: float, direction: float, type: Player/Bullet/Asteroid etc. [enum or string or whatever])
        Player :GameObject representation (turretDirection, turnState, firing, speedState, powerUpState [enum can be added later])
        Asteroid: GameObject (shape, rotationSpeed) <- not needed yet, just mentioning
    createdEntities: string[] <- id list, the created entity details are in the entities list though
    destroyedEntities: string[] <- id list, they are not in the entities list anymore
}
```

## Implementation notes
- Enums can be represented **as integers** (use a single byte for them as it's unlikely will hit more than 256)
- Other types should be encoded **according to C#'s liking** if asked use **BIG ENDIAN** (as that's the internet standard)
- Vector2 should be encoded as 2 floats
- If I did not include something **only send data absolutely necessary, favor local computation** for things like matrices
- Guid should probably be serialized as string
