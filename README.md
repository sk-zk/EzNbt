# EzNbt

A minimal NBT and region reader using `dynamic`.

Thanks to [wiki.vg](https://wiki.vg/Main_Page) for their documentation and sample files.

## Examples

### NBT
Parse level.dat and display the player's inventory:
```csharp
var nbt = NbtReader.Open(@"(path)/level.dat");
var player = nbt["Data"]["Player"];
foreach (var entry in player["Inventory"])
{
   Console.WriteLine(entry["Count"] + "\t" + entry["id"]);
}
```

### Region
Open a region file and display InhabitedTime of each chunk if greater than 0:
```csharp
var region = RegionFile.Open(@"(path)/region/r.-1.0.mca");
foreach (var chunk in region.GetAllChunks())
{
    var level = chunk["Level"];
    if (level["InhabitedTime"] > 0)
    {
        Console.WriteLine(level["xPos"] + ";" + level["zPos"]
            + ":\t" + level["InhabitedTime"]);
    }
}
```

Note that this library just gives you the raw NBT data and therefore 
has no block data parser. Implementing that is up to the user.

## Dependencies
* [Microsoft.CSharp](https://www.nuget.org/packages/Microsoft.CSharp/)
* [Ionic.Zlib.Core](https://www.nuget.org/packages/Ionic.Zlib.Core/)
