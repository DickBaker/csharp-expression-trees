using System.IO;
using Xunit;

namespace TitanicExplorer.Data.Tests;

public class DataTests
{
    public DataTests()
    {
        SampleDataPath = Path.GetTempFileName();

        File.WriteAllText(SampleDataPath, Resource.passengers);
    }

    public string SampleDataPath { get; }

    [Fact]
    public void LoadData()
    {
        System.Collections.Generic.List<Passenger> passengers = Passenger.LoadFromFile(SampleDataPath);

        Assert.Equal(887, passengers.Count);

        Assert.Equal("Mr. Owen Harris Braund", passengers[0].Name);
    }
}