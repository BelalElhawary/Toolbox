namespace Toolbox.Invoice
{
    public class InvoiceModel
    {
        public string Type { get; set; }
        public InvoiceDocumentModel Document { get; set; }
        public object Client { get; set; }
        public object Company { get; set; }
        public object User { get; set; }
        public string State { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
        public bool Deleted { get; set; } = false;
    }
}
