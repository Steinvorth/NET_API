namespace MinimalAPI.Services
{
    public interface I_FileStore
    {
        Task Delete(string? route, string containerName);
        Task<string> Save(IFormFile image, string containerName);
        async Task<string> Edit(string? route, string containerName, IFormFile image)
        {
            await Delete(route, containerName);
            return await Save(image, containerName);
        }
    }
}
