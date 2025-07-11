using System.Text.Json.Serialization;

namespace MusicRecognizer.Models;
internal class ShazamResponseLight
{

    [JsonPropertyName("track")]
    public ShazamTrackLight Track { get; set; }

    [JsonPropertyName("retryms")]
    public int? RetryMs { get; set; }

}

internal class ShazamTrackLight
{

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("subtitle")]
    public string Subtitle { get; set; }

    [JsonPropertyName("images")]
    public ShazamImagesLight Images { get; set; }

    [JsonPropertyName("share")]
    public ShazamShareLight Share { get; set; }

}
internal class ShazamImagesLight
{

    [JsonPropertyName("coverart")]
    public string Cover { get; set; }

    [JsonPropertyName("coverarthq")]
    public string CoverHQ { get; set; }

}
internal class ShazamShareLight
{

    [JsonPropertyName("href")]
    public string Link { get; set; }

    [JsonPropertyName("image")]
    public string Image { get; set; }

}
