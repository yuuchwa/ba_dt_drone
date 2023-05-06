namespace DtTelloDrone.RyzeSDK.Attribute
{
    /// <summary>
    /// Move directions.
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
