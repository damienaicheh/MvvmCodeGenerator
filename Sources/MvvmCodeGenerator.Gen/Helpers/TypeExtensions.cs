namespace MvvmCodeGenerator.Gen
{
    using System;

    /// <summary>
    /// Manage the Type of the different class and properties.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Finds the type of the object defined in the MvvmCodeGenMapper.xml file as a string.
        /// </summary>
        /// <returns>The complete namespace with the type of the object for example : System.Boolean.</returns>
        /// <param name="name">The type name.</param>
        public static string FindType(this string name)
        { 
            switch (name)
            {
                case "string":
                    return typeof(string).ToString();
                case "int":
                    return typeof(int).ToString();
                case "long":
                    return typeof(long).ToString();
                case "bool":
                    return typeof(bool).ToString();
                case "float":
                    return typeof(float).ToString();
                case "object":
                    return typeof(object).ToString();
                case "double":
                    return typeof(double).ToString();
                case "DateTime":
                    return typeof(DateTime).ToString();
                case "DateTimeOffset":
                    return typeof(DateTimeOffset).ToString();
                case "TimeSpan":
                    return typeof(TimeSpan).ToString();
                case var list when name.StartsWith("list ", StringComparison.InvariantCulture):
                    var paramType = FindType(list.Substring("list ".Length).Trim());
                    return $"System.Collections.Generic.IList<{paramType}>";
                default:
                    return name;
            }
        }
    }
}