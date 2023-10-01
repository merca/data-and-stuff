namespace stargripcorp.dataplatform.infra.utils.Naming;

internal class ResourceTypeAbbreviations
{
    private readonly Dictionary<string, string> abbreviations = new();
    public void Add(string resourceType, string abbreviation)
    {
        abbreviations[resourceType] = abbreviation;
    }

    public bool Contains(string resourceType)
    {
        return abbreviations.ContainsKey(resourceType);
    }

    public string GetAbbreviation(string resourceType)
    {
        return abbreviations[resourceType];
    }
}
