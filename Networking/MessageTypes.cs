namespace NeonTDS
{
    public enum MessageTypes
    {
        Unknown,
        Clock,
        EntityCreate,
        EntityDestroy,
        PlayerState,
        Health,
        PlayerRespawned,
        PlayerPoweredUp,
        Connect,
        ConnectResponse,
        PlayerInput,
        PlayerInputAck,
        Disconnect,
        Ping
    }
}
