namespace JsonToObjectConverter.Runtime
{
    /// <summary>
    /// Represents the type of a JSON token.
    /// </summary>
    public enum JsonTokenType
    {
        /// <summary>No token (initial state)</summary>
        None = 0,

        /// <summary>Start of an object: {</summary>
        ObjectStart,

        /// <summary>End of an object: }</summary>
        ObjectEnd,

        /// <summary>Start of an array: [</summary>
        ArrayStart,

        /// <summary>End of an array: ]</summary>
        ArrayEnd,

        /// <summary>Property name in an object</summary>
        PropertyName,

        /// <summary>String value</summary>
        String,

        /// <summary>Number value (integer or floating-point)</summary>
        Number,

        /// <summary>Boolean true value</summary>
        True,

        /// <summary>Boolean false value</summary>
        False,

        /// <summary>Null value</summary>
        Null,

        /// <summary>Colon separator: :</summary>
        Colon,

        /// <summary>Comma separator: ,</summary>
        Comma,
    }
}
