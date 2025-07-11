using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MusicRecognizer.Test;

public class ShazamTest
{
    [Fact]
    public async Task StreamRecognize()
    {
        var Expected = @"{
  ""title"": ""Kuzu Kuzu"",
  ""artist"": ""Tarkan"",
  ""link"": ""https://www.shazam.com/track/86019790/kuzu-kuzu"",
  ""cover"": ""https://is1-ssl.mzstatic.com/image/thumb/Music124/v4/35/8d/22/358d22cf-8b9c-1e7d-996d-85aad739a255/dj.nixigvoo.jpg/400x400cc.jpg""
}";
        try
        {
            string resourceName = "MusicRecognizer.Test.Source.media_3.aac";
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException($"Gömülü kaynak '{resourceName}' bulunamadý.", resourceName);
                }

                var result = await ShazamService.IdentifyAsync(stream, CancellationToken.None);
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };
                var jsonData = JsonSerializer.Serialize(result, options);

                Assert.Equal(jsonData, Expected);
            }
        }
        catch (FileNotFoundException ex)
        {
            Assert.True(false, ex.Message);
        }
    }
}