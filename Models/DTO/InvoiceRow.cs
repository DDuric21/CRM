namespace Models.DTO
{
    public class InvoiceRow
    {
        public long BillId { get; set; }

        public string BillingProfileId { get; set; }
        
        public DateTime DateCreated { get; set; }
        
        public bool HasPdf { get; set; }
    }
}
