using System.Collections.Generic;
namespace AplicatieLicenta
{
    public static class TemporaryRegistrationStore
    {
        public static Dictionary<string, (string HashedPassword, string TipUtilizator, string CategorieVarsta, string CodConfirmare)> PendingRegistrations
            = new Dictionary<string, (string, string, string, string)>();
    }
}
