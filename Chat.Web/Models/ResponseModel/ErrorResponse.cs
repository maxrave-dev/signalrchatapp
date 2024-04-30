namespace Chat.Web.Models.ResponseModel;

public class ErrorResponse
{
    public ErrorResponse(string message)
    {
        Message = message;
        Errors = true;
    }
    public ErrorResponse(string message, bool errors)
    {
        Message = message;
        Errors = errors;
    }
    public string Message { get; set; }
    public bool Errors { get; set; }
}