public class ImageService
{
    private readonly string _imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

    public ImageService()
    {
        if (!Directory.Exists(_imagePath))
        {
            Directory.CreateDirectory(_imagePath);
        }
    }

    public async Task<List<string>> SaveImagesAsync(List<IFormFile> files)
    {
        var urls = new List<string>();

        foreach (var file in files)
        {
            var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(_imagePath, uniqueName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            urls.Add($"/uploads/{uniqueName}"); // URL relative à exposer côté frontend
        }

        return urls;
    }
}
