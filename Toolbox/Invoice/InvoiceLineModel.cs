using ProDesk.Domain.Table.EInvoice;

namespace Toolbox.Invoice;

public class InvoiceLineModel
{
    public string description { get; set; }
    public string itemType {get;set;}
    public string itemCode {get;set;}
    public string unitType {get;set;}
    public decimal quantity {get;set;}
    public ValueModel UnitValueModel {get;set;}
    public decimal salesTotal {get;set;}
    public decimal total {get;set;}
    public decimal valueDifference {get;set;}
    public decimal totalTaxableFees {get;set;}
    public decimal netTotal {get;set;}
    public decimal itemsDiscount {get;set;}
    public DiscountModel? discount {get;set;}
    public required List<TaxableItemModel> taxableItems {get;set;}
    public string internalCode {get;set;}
}