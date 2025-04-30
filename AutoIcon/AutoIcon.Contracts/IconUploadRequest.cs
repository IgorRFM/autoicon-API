using Microsoft.AspNetCore.Http;

namespace AutoIcon.Contracts;

public class IconUploadRequest
{
    public Guid Uuid { get; set;}

    public IFormFile Image { get; set;}
}
