namespace CoworkingGes.Helpers
{
    public static class AbonnementTypeHelper
    {
        // ✅ Dictionary: Type name -> Price
        public static readonly Dictionary<string, decimal> AbonnementTypes = new()
        {
            { "Mensuel", 150m },
            { "Trimestrel", 400m },
            { "Semestriel", 700m },
            { "Annuel", 1200m }
        };

        // Get price by name
        public static decimal GetPrice(string typeName)
        {
            return AbonnementTypes.TryGetValue(typeName, out var price) ? price : 0m;
        }

        // Get all types as list
        public static List<string> GetAllTypes()
        {
            return AbonnementTypes.Keys.ToList();
        }

        // Get all types with prices
        public static Dictionary<string, decimal> GetAllTypesWithPrices()
        {
            return AbonnementTypes;
        }
    }
}
