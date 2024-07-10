namespace Toolbox.Invoice;

public class TaxableItemModel
{
    public string taxType { get; set; }
    public string subType { get; set; }
    public decimal amount { get; set; }
    public decimal rate { get; set; }
}