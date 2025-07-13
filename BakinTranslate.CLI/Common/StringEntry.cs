namespace BakinTranslate.CLI.Common
{
    internal struct StringEntry
    {
        public long Offset { get; set; }
        public int Length { get; set; }
        public string Value { get; set; }
        public string NewValue { get; set; }
        public StringEntry(long offset, int length, string value, string newValue)
        {
            Offset = offset;
            Length = length;
            Value = value;
            NewValue = newValue;
        }
    }
}
