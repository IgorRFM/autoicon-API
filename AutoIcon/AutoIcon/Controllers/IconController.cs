using Microsoft.AspNetCore.Mvc;
using AutoIcon.Contracts;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace AutoIcon.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IconController : ControllerBase
{
    private readonly Cloudinary _cloudinary;

    public IconController(Cloudinary cloudinary)
    {
        _cloudinary = cloudinary;
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadIcon([FromForm] IconUploadRequest request)
    {
        if (request.Image == null || request.Image.Length == 0)
        {
            return BadRequest(new { success = false, error = "Imagem não enviada." });
        }

        // Envia para o Cloudinary
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(request.Image.FileName, request.Image.OpenReadStream()),
            PublicId = request.Uuid.ToString(),
            Folder = "autoicon",
            // Crop quadrado centralizado
            Transformation = new Transformation()
                .Height(300)
                .Width(300)
                .Gravity("center")
                .Crop("fill")
        };

        // Envia a imagem para o Cloudinary com as transformações aplicadas
        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return StatusCode((int)uploadResult.StatusCode, new { error = uploadResult.Error?.Message });
        }


        return Ok(new
        {
            success = true,
            uuid = request.Uuid,
            url = uploadResult.SecureUrl.ToString(),
            public_id = uploadResult.PublicId
        });
    }
}
