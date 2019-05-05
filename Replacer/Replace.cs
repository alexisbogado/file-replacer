namespace Replacer
{
    public struct Replace
    {
        public string Search;
        public string Replacement;

        public Replace(string search, string replacement)
        {
            Search = search;
            Replacement = replacement;
        }
    }
}