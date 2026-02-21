namespace GooseShop.Models
{
    public class NPCity { public string Description { get; set; } = ""; public string Ref { get; set; } = ""; }
    
    public class NPResponse<T> { public bool Success { get; set; } public List<T> Data { get; set; } = new(); }

}