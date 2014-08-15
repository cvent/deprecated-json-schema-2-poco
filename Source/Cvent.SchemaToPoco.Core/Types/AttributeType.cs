namespace Cvent.SchemaToPoco.Core.Types
{
    /// <summary>
    ///     Determine type of annotation to generate.
    /// </summary>
    public enum AttributeType
    {
        /// <summary>
        ///     Compatible with ASP.net WebAPI.
        /// </summary>
        SystemDefault = 1,

        /// <summary>
        ///     Compatible with JSON.net deserialization.
        /// </summary>
        JsonDotNet = 2,
    }
}
