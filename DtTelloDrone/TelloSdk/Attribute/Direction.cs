namespace DtTelloDrone.RyzeSDK.Attribute
{
    /// <summary>
    /// The enum represents the direction drone can move.
    /// </summary>
    public enum MoveDirection
    {
        Stop,
        Rise, 
        Sink, 
        Left, 
        Right, 
        Forward, 
        Back
    }

    /// <summary>
    /// The enum represents the direction drone can rotate.
    /// </summary>
    public enum RotationDirection
    {
        Clockwise,
        CounterClockwise,
    }

    /// <summary>
    /// Flip direction.
    /// </summary>
    public enum FlipDirection
    {
        Left, 
        Right, 
        Forward, 
        Back
    }
}
