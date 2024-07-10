namespace Toolbox.Invoice
{
    public class InvoiceDocumentModel
    {
        public IssuerModel issuer { get; set; }
        public ReceiverModel receiver { get; set; }
        public string documentType { get; set; }
        public string documentTypeVersion { get; set; }
        public DateTime dateTimeIssued { get; set; }
        public string taxpayerActivityCode { get; set; }
        public string internalId { get; set; }
        public string? purchaseOrderReference { get; set; }
        public string? purchaseOrderDescription { get; set; }
        public string? salesOrderReference { get; set; }
        public string? salesOrderDescription { get; set; }
        public string? proformaInvoiceNumber { get; set; }
        public PaymentModel? payment { get; set; }
        public DeliveryModel? delivery { get; set; }
        public List<InvoiceLineModel> invoiceLine { get; set; }
        public decimal totalSalesAmount { get; set; }
        public decimal totalDiscountAmount { get; set; }
        public decimal netAmount { get; set; }
        public List<TaxTotalModel> taxTotals { get; set; }
        public decimal extraDiscountAmount { get; set; }
        public decimal totalItemsDiscountAmount { get; set; }
        public decimal totalAmount { get; set; }
        public List<SignatureModel> signatures { get; set; }
    }
}
