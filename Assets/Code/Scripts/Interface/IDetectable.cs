public interface IDetectable
{
    /// <summary>
    /// Called when the object is detected by the player.
    /// Triggers visual effects like outlines or highlights.
    /// </summary>
    void OnDetected();

    /// <summary>
    /// Called when the object is no longer detected by the player.
    /// Disables visual effects like outlines or highlights.
    /// </summary>
    void OnDetectionLost();
}
