using System.Text.Json.Serialization;

namespace MusicRecognizer.Models;
internal class ShazamRequest
{
    [JsonPropertyName("timezone")]
    public string TimeZone { get; set; }
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    [JsonPropertyName("geolocation")]
    public object Geolocation { get; set; }

    [JsonPropertyName("context")]
    public object Context { get; set; }

    [JsonPropertyName("signature")]
    public ShazamSignature Signature { get; set; }

}
internal class ShazamSignature
{

    [JsonPropertyName("uri")]
    public string Uri { get; set; }

    [JsonPropertyName("samplems")]
    public int SampleMs { get; set; }
}
