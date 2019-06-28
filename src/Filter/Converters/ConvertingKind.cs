namespace RimDev.Filter.Range
{
    public enum ConvertingKind
    {
        /// <summary>
        /// Unspecified conversion
        /// </summary>
        Unspecified,
        /// <summary>
        /// Minimum Inclusive conversion (includes the value)
        /// </summary>
        MinInclusive,
        /// <summary>
        /// Maximum Inclusive conversion (includes the value)
        /// </summary>
        MaxInclusive,
        /// <summary>
        /// Minimum Exclusive conversion (excludes the value)
        /// </summary>
        MinExclusive,
        /// <summary>
        /// Maximum Exclusive conversion (excludes the value)
        /// </summary>
        MaxExclusive
    }
}