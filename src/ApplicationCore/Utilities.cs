namespace Microsoft.eShopWeb.ApplicationCore;

internal class Utilities
{
    public static string ReplaceBetween(string where, string s1, string s2, string replaceWith)
    {
        int pos = where.IndexOf(s1);
        if (pos > -1)
        {
            var pos2 = where.IndexOf(s2, pos + 1);
            if (pos2 > -1)
            {
                return where.Remove(pos, pos2 + s2.Length + 1 - pos).Insert(pos, replaceWith);
            }
        }
        return where;
    }
}
