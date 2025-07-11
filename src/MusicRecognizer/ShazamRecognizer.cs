using System.Text.Json;
using System.Text.Json.Serialization;

namespace MusicRecognizer;

public static class ShazamRecognizer
{
    public static async Task Recognize(string audioFilePath)
    {
        try
        {
            Console.WriteLine("Identifying song from file...");

            var result = await ShazamService.IdentifyAsync(audioFilePath, CancellationToken.None);

            if (result != null)
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase, 
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull 
                };
                Console.WriteLine($"Found: " + JsonSerializer.Serialize(result, options));
            }
            else
            {
                Console.WriteLine("No match found");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public static async Task Recognize(Stream stream)
    {
        try
        {
            Console.WriteLine("Identifying song from file...");

            var result = await ShazamService.IdentifyAsync(stream, CancellationToken.None);

            if (result != null)
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };
                Console.WriteLine($"Found: " + JsonSerializer.Serialize(result, options));
            }
            else
            {
                Console.WriteLine("No match found");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
