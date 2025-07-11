using MusicRecognizer.Abstraction;
using MusicRecognizer.Models;
using NAudio.Wave;
using System.Text;
using System.Text.Json;

namespace MusicRecognizer;
public class ShazamService
{
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(5) };
    private static readonly string DeviceId = Guid.NewGuid().ToString();
    public static async Task<SoundMatch> IdentifyAsync(string filePath, CancellationToken cancel)
    {
        using var audioFile = OpenAudioFile(filePath);

        return await Identify(audioFile, cancel);
    }
    public static async Task<SoundMatch> IdentifyAsync(Stream stream, CancellationToken cancel)
    {
        using var audioFile = OpenAudioStream(stream);
        if(audioFile is null)
            throw new NotSupportedException("Unsupported audio format");

        return await Identify(audioFile, cancel);
    }

    private static async Task<SoundMatch> Identify(WaveStream audioFile, CancellationToken cancel)
    {
        using var resampler = new MediaFoundationResampler(audioFile, new WaveFormat(16000, 16, 1));
        var samples = resampler.ToSampleProvider();

        var analyser = new Analyser();
        var landmarker = new Landmarker(analyser);

        var retryMs = 3000;

        while (true)
        {
            analyser.ReadChunk(samples);

            if (analyser.StripeCount > 2 * Landmarker.RADIUS_TIME) landmarker.Find(analyser.StripeCount - Landmarker.RADIUS_TIME - 1);
            if (analyser.ProcessedMs < retryMs) continue;

            var body = new ShazamRequest
            {
                Signature = new ShazamSignature
                {
                    Uri = "data:audio/vnd.shazam.sig;base64," + Convert.ToBase64String(Signature.Create(Analyser.SAMPLE_RATE, analyser.ProcessedSamples, landmarker)),
                    SampleMs = analyser.ProcessedMs,
                },
                TimeZone = "Europe/Moscow",
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Context = new { },
                Geolocation = new { }
            };

            var uri = $"https://amp.shazam.com/discovery/v5/en/US/android/-/tag/{DeviceId}/{Guid.NewGuid()}";
            //var uri = $"https://amp.shazam.com/discovery/v5/ru/RU/iphone/-/tag/{Guid.NewGuid()}/{Guid.NewGuid()}?sync=true&webv3=true&sampling=true&connected=&shazamapiversion=v3&sharehub=true&hubv5minorversion=v5.1&hidelb=true&video=v3";

            using var request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(body),
                    Encoding.UTF8,
                    "application/json"
                ),
                Headers = {
                    { "User-Agent", "Shazam/15.0.0 (iPhone; iOS 17.0.3; Scale/3.00)" },
                    { "Accept", "application/json" },
                    { "Accept-Language", "ru" },
                    { "X-Shazam-Platform", "IPHONE" },
                    { "X-Shazam-AppVersion", "14.1.0" }
                }
            };

            using var res = await Http.SendAsync(request, cancel);

            var response = await res.Content.ReadAsStringAsync(cancel);
            var data = JsonSerializer.Deserialize<ShazamResponseLight>(response);

            if (data.RetryMs > 0)
            {
                retryMs = (int)data.RetryMs;
                continue;
            }

            if (data.Track == null) return null;

            //return new SoundMatch
            //{
            //    Title = data.Track.Title,
            //    Artist = data.Track.Subtitle,
            //    Link = data.Track.Url,
            //    Cover = data.Track?.Images?.CoverHQ ?? data.Track?.Images?.Cover ?? data.Track.Share.Image
            //};

            return new SoundMatch
            {
                Title = data.Track.Title,
                Artist = data.Track.Subtitle,
                Link = data.Track.Share.Link,
                Cover = data.Track?.Images?.CoverHQ ?? data.Track?.Images?.Cover ?? data.Track.Share.Image
            };
        }
    }

    private static WaveStream OpenAudioFile(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();

        return extension switch
        {
            ".mp3" => new Mp3FileReader(filePath),
            ".wav" => new WaveFileReader(filePath),
            ".aiff" => new AiffFileReader(filePath),
            ".wma" => new MediaFoundationReader(filePath),
            ".m4a" => new MediaFoundationReader(filePath),
            ".aac" => new MediaFoundationReader(filePath),
            _ => new AudioFileReader(filePath) 
        };
    }

    private static WaveStream? OpenAudioStream(Stream file)
    {
        var extension = Utils.FileExtensionFromStream(file);

        return extension switch
        {
            "mp3" => new StreamMediaFoundationReader(file),
            "wav" => new WaveFileReader(file),
            "aiff" => new AiffFileReader(file),
            "wma" => new StreamMediaFoundationReader(file),
            "m4a" => new StreamMediaFoundationReader(file),
            "aac" => new StreamMediaFoundationReader(file),
            _ => null
        };
    }
}
