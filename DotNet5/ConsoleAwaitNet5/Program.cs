using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsoleAwaitNet5
{
    class Program
    {
        static string imgPath = @"c:\workImage";
        static async Task Main(string[] args)
        {
            var url = "https://luhacovice.cz/wp-content/uploads/2020/10/u_X8A3077.jpg";
            var fileName = Path.GetFileName(url);
            var originalImageBytes = await DownloadImage(url);
            Directory.CreateDirectory(imgPath);
            var origImagePath = Path.Combine(imgPath, fileName);
            await SaveImage(originalImageBytes, origImagePath);
            var bluredImageBytes = await BlureImage(origImagePath);
            var bluredImagePath = Path.Combine(imgPath, $"{Path.GetFileNameWithoutExtension(fileName)}_blured{Path.GetExtension(fileName)}");
            await SaveImage(originalImageBytes, bluredImagePath);
        }
        static void Log(string msg)
        {
            Console.WriteLine(msg);
        }
        static Task<byte[]> DownloadImage(string url)
        {
            Log("Downloading img.");
            var client = new HttpClient();
            return client.GetByteArrayAsync(url);
        }

        static async Task SaveImage(byte[] bytes, string imagePath)
        {
            Log($"Save img:'{Path.GetFileName(imagePath)}'");
            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await fileStream.WriteAsync(bytes, 0, bytes.Length);
            }
        }
        /// <summary>
        ///  launching an operation on a separate thread via Task.Run is mainly useful for CPU-bound operations
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        static async Task<byte[]> BlureImage(string imagePath)
        {
            return await Task.Run(() =>
            {
                Image image = Image.Load(imagePath);
                image.Mutate(ctx => ctx.GaussianBlur());
                using(var memoryStream = new MemoryStream())
                {
                    image.SaveAsJpeg(memoryStream);
                    return memoryStream.ToArray();
                }
            });
        }
    }
}
