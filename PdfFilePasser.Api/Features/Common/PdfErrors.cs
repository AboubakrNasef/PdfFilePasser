namespace PdfFilePasser.Api.Features.Common;

public class PdfNotFoundException : Exception
{
    public PdfNotFoundException(string message) : base(message) { }
}

public class PdfValidationException : Exception
{
    public PdfValidationException(string message) : base(message) { }
}
