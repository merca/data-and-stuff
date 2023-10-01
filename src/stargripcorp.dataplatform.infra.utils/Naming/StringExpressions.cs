using System.Text.RegularExpressions;

namespace stargripcorp.dataplatform.infra.utils.Naming;

public static partial class StringExpressions
{
    [GeneratedRegex("[^a-z0-9]+")]
    public static partial Regex NoSpecialCharactersLowerCase();

    [GeneratedRegex("[^a-zA-Z0-9-]")]
    public static partial Regex AllowDashes();
}