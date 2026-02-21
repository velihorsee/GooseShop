namespace GooseShop.Models
{
    public class NPWarehouse
    {
        public string Description { get; set; } = "";
        public string Ref { get; set; } = "";
        public string CityRef { get; set; } = ""; // Обов'язково додайте це!
    }
    public class CachedWarehouse
    {
        public int Id { get; set; }
        public string CityRef { get; set; } = "";
        public string Ref { get; set; } = "";
        public string Description { get; set; } = "";
        public string Type { get; set; } = ""; // "Warehouse" або "Postomat"
        public int Number { get; set; } // Для точного сортування
    }

    // Також додамо модель для логів оновлення
    public class AppConfig
    {
        public int Id { get; set; }
        public string Key { get; set; } = "";
        public string Value { get; set; } = "";
    }
}
