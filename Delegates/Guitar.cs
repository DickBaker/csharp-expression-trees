namespace Delegates;

/// <summary>
/// The pickup situation for the guitar
/// </summary>
public enum PickupType
{
    Acoustic,
    AcousticElectric,
    Electric
}

/// <summary>
/// What kind of strings the guitar has
/// </summary>
public enum StringType
{
    Steel,
    Nylon
}

/// <summary>
/// Guitars because they're cool
/// </summary>
/// <remarks>
/// Constructor which sets all the properties
/// </remarks>
/// <param name="pickup">The broad pickup type</param>
/// <param name="strings">Steel string or nylon</param>
/// <param name="name">The name of the guitar</param>
public class Guitar(PickupType pickup, StringType strings, string name)
{
    /// <summary>
    /// Model Name of the guitar
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// The pickup situation for the guitar
    /// </summary>
    public PickupType Pickup { get; set; } = pickup;

    /// <summary>
    /// What kind of strings the guitar has
    /// </summary>
    public StringType Strings { get; set; } = strings;
}
