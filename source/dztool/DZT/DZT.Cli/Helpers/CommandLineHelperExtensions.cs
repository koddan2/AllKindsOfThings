using System.CommandLine;

static class CommandLineHelperExtensions
{
    public static Option<T> AddOption<T>(
        this Command cmd,
        string name,
        string? description = null,
        bool isRequired = false,
        string[]? aliases = null,
        Func<T>? getDefaultValue = null
    )
    {
        Option<T> opt = getDefaultValue is null
            ? new(name, description: description)
            : new(name, getDefaultValue, description: description);

        opt.IsRequired = isRequired;

        foreach (var alias in aliases ?? Array.Empty<string>())
        {
            opt.AddAlias(alias);
        }
        cmd.Add(opt);
        return opt;
    }
}
