using ProDesk.Domain.Table.EInvoice;

namespace Toolbox.Invoice;

public class IssuerModel
{
    public string type {get;set;} = "";
    public string id {get;set;} = "";
    public string name {get;set;} = ""; 
    public AddressModel? address {get;set;}
}