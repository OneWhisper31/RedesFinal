using Fusion;

public struct NetworkInputData : INetworkInput
{
    public float horizontalInput;
    public float verticalInput;
    public NetworkBool isJumpPressed;
    public NetworkBool isGrabPressed;
}
