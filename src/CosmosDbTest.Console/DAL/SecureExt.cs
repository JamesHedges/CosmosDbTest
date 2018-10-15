using System.Security;

namespace CosmosDbTest.DAL
{
    public static class SecureExt
    {
        public static SecureString SecureString(this string key)
        {
            var secureString = new SecureString();
            foreach (char ch in key)
            {
                secureString.AppendChar(ch);
            }

            return secureString;
        }
    }
}