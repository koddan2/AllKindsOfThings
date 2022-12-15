using System.Reflection;

namespace SAK.MetaProgramming;

public static class MpReflectExtensions
{
    /// <summary>
    /// Gets all properties on the type of the target object reference.
    /// </summary>
    /// <typeparam name="T">The type of the object reference.</typeparam>
    /// <param name="_">The reference. Does not matter if it is null or not.</param>
    /// <param name="maybeBindingFlags">Optional parameter to select a subset of properties.</param>
    /// <returns>The array of matching <see cref="PropertyInfo"/>s.</returns>
    public static PropertyInfo[] GetProperties<T>(this T _, BindingFlags? maybeBindingFlags = null)
    {
        var typ = typeof(T);
        if (maybeBindingFlags is BindingFlags bindingFlags)
        {
            return typ.GetProperties(bindingFlags);
        }
        else
        {
            return typ.GetProperties();
        }
    }

    /// <summary>
    /// <seealso cref="https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/attributes/creating-custom-attributes"/>
    /// <seealso cref="https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/attributes/accessing-attributes-by-using-reflection"/>
    /// </summary>
    /// <typeparam name="T">The type of the object reference.</typeparam>
    /// <param name="_">The reference. Does not matter if it is null or not.</param>
    /// <returns>The array of <see cref="Attribute"/>s.</returns>
    public static Attribute[] GetCustomAttributes<T>(this T _, bool includeInherited = true)
    {
        Attribute[] result = Attribute.GetCustomAttributes(typeof(T), includeInherited);
        return result;
    }
}
