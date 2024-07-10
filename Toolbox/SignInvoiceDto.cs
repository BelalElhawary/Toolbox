namespace Toolbox;

public class SignInvoiceDto
{
    public required string CertificateIssuer { get; set; }
    public required string TokenPin { get; set; }
}