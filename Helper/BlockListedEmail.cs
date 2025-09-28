namespace AllOkEmail.Helper
{
    public static class BlockListedEmail
    {
        public static Lazy<HashSet<string>> EmailBlockList = new Lazy<HashSet<string>>(  () =>
        {
            var lines = File.ReadLines("Assets\\disposable_email_blocklist.conf")
                .Where(line => !string.IsNullOrWhiteSpace(line) && !line.TrimStart().StartsWith("//"));
            return new HashSet<string>(lines, StringComparer.OrdinalIgnoreCase);
        });

    }
}
