using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ess;
using ProDesk.Domain.Model.Invoice;

namespace Toolbox;

public class SignInvoice
{
    private const string DllLibPath = "eps2003csp11.dll";

    private SignInvoiceDto _dto;
    // private const string TokenPin = "60897619";
    // private const string TokenCertificate = "Egypt Trust CA G6";

    
    
    public async Task<string> Sign(long id, SignInvoiceDto dto)
    {
        _dto = dto;
        InvoiceModel? invoice = await GetInvoice(id);

        if (invoice is null) return "Failed to get Invoice.";

        return await ProcessJson(id, invoice.Document);
    }

    private async Task<InvoiceModel?> GetInvoice(long id)
    {
        var client = new HttpClient();
        
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:44365/api/Invoice/{id}");
        // request.Headers.Add("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJQcm9EZXNrVGVzdCIsImp0aSI6IjNhNjExNjk2LWEyNWItNGJhMi1iODAwLTljYmI0NDllOTQ4OCIsImVtYWlsIjoicHJlcHJvZC5wcm9kZXNrQGNvbW1hdGVjcy5jb20iLCJ1aWQiOiI3MWY3MzI1NC1hOTIwLTRmMmMtYjhhNS1jYTBiZWU5NmU5N2EiLCJjaWQiOiIxMiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6WyJSZWNlaXB0cyIsIlNoaWZ0cyIsIk1hbmFnZW1lbnQiLCJDbGllbnRzIiwiU2FsZXMiLCJFdGEiLCJTdG9yYWdlIiwiVGlja2V0Il0sImV4cCI6MTcyMDU3MTgzOSwiaXNzIjoiQ29tbWF0ZWNzIiwiYXVkIjoiQ29tbWF0ZWNzQ2xpZW50In0.jva3gReY9wGi5LMYZMANmI8CHVwHKTf-guR9Aes7BkI");
        
        // var content = new StringContent("", null, "application/json");
        // request.Content = content;
        
        var response = await client.SendAsync(request);
        
        // Console.WriteLine(response.ToString());
        // Console.WriteLine(await response.Content.ReadAsStringAsync());

        if (response.StatusCode != HttpStatusCode.OK)
            return null;
        
        var data = await response.Content.ReadAsStringAsync();

        
        return JsonConvert.DeserializeObject<InvoiceModel>(data) ;
    }
    
    private async Task<bool> UpdateInvoiceSignature(long id, List<SignatureModel> signatures)
    {
        var client = new HttpClient();
        
        var request = new HttpRequestMessage(HttpMethod.Put,  $"https://localhost:44365/api/Invoice/sign/{id}");
        // request.Headers.Add("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJQcm9EZXNrVGVzdCIsImp0aSI6IjNhNjExNjk2LWEyNWItNGJhMi1iODAwLTljYmI0NDllOTQ4OCIsImVtYWlsIjoicHJlcHJvZC5wcm9kZXNrQGNvbW1hdGVjcy5jb20iLCJ1aWQiOiI3MWY3MzI1NC1hOTIwLTRmMmMtYjhhNS1jYTBiZWU5NmU5N2EiLCJjaWQiOiIxMiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6WyJSZWNlaXB0cyIsIlNoaWZ0cyIsIk1hbmFnZW1lbnQiLCJDbGllbnRzIiwiU2FsZXMiLCJFdGEiLCJTdG9yYWdlIiwiVGlja2V0Il0sImV4cCI6MTcyMDU3MTgzOSwiaXNzIjoiQ29tbWF0ZWNzIiwiYXVkIjoiQ29tbWF0ZWNzQ2xpZW50In0.jva3gReY9wGi5LMYZMANmI8CHVwHKTf-guR9Aes7BkI");
        
        var content = new StringContent(JsonConvert.SerializeObject(signatures), null, "application/json");
        request.Content = content;
        
        var response = await client.SendAsync(request);

        if (response.StatusCode != HttpStatusCode.OK)
            return false;
        
        return true;
    }
    
    private async Task<string> ProcessJson(long id, InvoiceDocumentModel model)
    {
        string jsonToPost = JsonConvert.SerializeObject(model,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                FloatFormatHandling = FloatFormatHandling.String,
                FloatParseHandling = FloatParseHandling.Decimal,
                DateFormatString = "yyyy-MM-ddTHH:mm:ss",
                DateParseHandling = DateParseHandling.None,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new IgnorePropertiesResolver(["signatures"]),
                Converters = new List<JsonConverter> { new NumberJsonConverter() }
            }
        );
        JObject? request = JsonConvert.DeserializeObject<JObject>(jsonToPost, new JsonSerializerSettings
        {
            FloatFormatHandling = FloatFormatHandling.String,
            FloatParseHandling = FloatParseHandling.Decimal,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateParseHandling = DateParseHandling.None,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new IgnorePropertiesResolver(["signatures"])
        });

        if (request is null) return "Failed to deserialize invoice.";
        
        //Start serialize
        string canonicalString = SerializeJson(request);
        Console.WriteLine(canonicalString);

        
        // retrieve cades
        var (success, result) = SignWithCms(canonicalString);

        if (success is false) return result;
        
        SignatureModel signature = new SignatureModel()
        {
            signatureType = "I",
            value = result
        };
        List<SignatureModel> signatures = [signature];

        success = await UpdateInvoiceSignature(id, signatures);

        return success ? string.Empty : "Failed to sign invoice.";
    }

    private string SerializeJson(JObject request)
    {
        return SerializeJsonToken(request);
    }
    
    private string SerializeJsonToken(JToken request)
    {
        string serialized = "";
        
        if (request.Parent is null)
        {
            SerializeJsonToken(request.First);
        }
        else
        {
            if (request.Type == JTokenType.Property)
            {
                string name = ((JProperty)request).Name.ToUpper();
                serialized += "\"" + name + "\"";
                foreach (var property in request)
                {
                    if (property.Type == JTokenType.Object)
                    {
                        serialized += SerializeJsonToken(property);
                    }
                    if (property.Type == JTokenType.Boolean || property.Type == JTokenType.Integer || property.Type == JTokenType.Float || property.Type == JTokenType.Date)
                    {
                        serialized += "\"" + property.Value<string>() + "\"";
                    }
                    if(property.Type == JTokenType.String)
                    {
                        serialized +=  JsonConvert.ToString(property.Value<string>()) ;
                    }
                    if (property.Type == JTokenType.Date)
                    {
                        serialized += "\"" + property.Value<DateTime>().ToString("yyyy-MM-ddTHH:mm:ss") + "\"";
                    }
                    if (property.Type == JTokenType.Array)
                    {
                        foreach (var item in property.Children())
                        {
                            serialized += "\"" + ((JProperty)request).Name.ToUpper() + "\"";
                            serialized += SerializeJsonToken(item);
                        }
                    }
                }
            }
            // Added to fix "References"
            if (request.Type == JTokenType.String)
            {
                serialized += JsonConvert.ToString(request.Value<string>());
            }
        }
        
        if (request.Type == JTokenType.Object)
        {
            foreach (var property in request.Children())
            {

                if (property.Type == JTokenType.Object || property.Type == JTokenType.Property)
                {
                    serialized += SerializeJsonToken(property);
                }
            }
        }

        return serialized;
    }
    
    private byte[] HashBytes(byte[] input)
    {
        using SHA256 sha = SHA256.Create();
        var output = sha.ComputeHash(input);
        return output;
    }
    
    private (bool, string) SignWithCms(String serializedText)
    {
        
        byte[] data = Encoding.UTF8.GetBytes(serializedText);
        Pkcs11InteropFactories factories = new Pkcs11InteropFactories();
        using IPkcs11Library pkcs11Library = factories.Pkcs11LibraryFactory.LoadPkcs11Library(factories, DllLibPath, AppType.MultiThreaded);
        ISlot? slot = pkcs11Library.GetSlotList(SlotsType.WithTokenPresent).FirstOrDefault();

        if (slot is null)
        {
            return (false, "No slots found");
        }

        // ITokenInfo tokenInfo = slot.GetTokenInfo();
        //
        // ISlotInfo slotInfo = slot.GetSlotInfo();

        using var session = slot.OpenSession(SessionType.ReadWrite);
        session.Login(CKU.CKU_USER, Encoding.UTF8.GetBytes(_dto.TokenPin));

        var certificateSearchAttributes = new List<IObjectAttribute>()
        {
            session.Factories.ObjectAttributeFactory.Create(CKA.CKA_CLASS, CKO.CKO_CERTIFICATE),
            session.Factories.ObjectAttributeFactory.Create(CKA.CKA_TOKEN, true),
            session.Factories.ObjectAttributeFactory.Create(CKA.CKA_CERTIFICATE_TYPE, CKC.CKC_X_509)
        };

        IObjectHandle? certificate = session.FindAllObjects(certificateSearchAttributes).FirstOrDefault();

        if (certificate is null)
        {
            return (false, "Certificate not found.");
        }

        X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        store.Open(OpenFlags.MaxAllowed);

        // find cert by thumbprint
        var foundCerts = store.Certificates.Find(X509FindType.FindByIssuerName, _dto.CertificateIssuer, false);

        //var foundCerts = store.Certificates.Find(X509FindType.FindBySerialNumber, "2b1cdda84ace68813284519b5fb540c2", true);

        if (foundCerts.Count == 0)
        {
            return (false, "No device detected.");
        }

        var certForSigning = foundCerts[0];
        store.Close();

        ContentInfo content = new ContentInfo(new Oid("1.2.840.113549.1.7.5"), data);
        SignedCms cms = new SignedCms(content, true);
                
        EssCertIDv2 bouncyCertificate = new EssCertIDv2(
            new Org.BouncyCastle.Asn1.X509.AlgorithmIdentifier(new DerObjectIdentifier("1.2.840.113549.1.9.16.2.47")), 
            this.HashBytes(certForSigning.RawData)
        );
                
        SigningCertificateV2 signerCertificateV2 = new SigningCertificateV2([bouncyCertificate]);
        CmsSigner signer = new CmsSigner(certForSigning)
        {
            DigestAlgorithm = new Oid("2.16.840.1.101.3.4.2.1")
        };

        signer.SignedAttributes.Add(new Pkcs9SigningTime(DateTime.UtcNow));
        signer.SignedAttributes.Add(new AsnEncodedData(new Oid("1.2.840.113549.1.9.16.2.47"), signerCertificateV2.GetEncoded()));
        cms.ComputeSignature(signer);
        var output = cms.Encode();

        return (true, Convert.ToBase64String(output));
    }
}