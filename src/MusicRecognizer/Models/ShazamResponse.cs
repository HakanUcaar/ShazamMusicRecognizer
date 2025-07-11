using System.Text.Json.Serialization;
using static System.Collections.Specialized.BitVector32;

namespace MusicRecognizer.Models;
public class ShazamResponse
{
    [JsonPropertyName("matches")]
    public List<Match> Matches { get; set; }

    [JsonPropertyName("location")]
    public Location Location { get; set; }

    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    [JsonPropertyName("timezone")]
    public string Timezone { get; set; }

    [JsonPropertyName("track")]
    public Track Track { get; set; }

    [JsonPropertyName("tagid")]
    public string TagId { get; set; }

    [JsonPropertyName("retryms")]
    public int? RetryMs { get; set; }

}

public class Match
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("offset")]
    public double Offset { get; set; }

    [JsonPropertyName("timeskew")]
    public double Timeskew { get; set; }

    [JsonPropertyName("frequencyskew")]
    public double Frequencyskew { get; set; }

}

public class Location
{
    [JsonPropertyName("accuracy")]
    public double Accuracy { get; set; }

}

public class Track
{
    [JsonPropertyName("layout")]
    public string Layout { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("key")]
    public string Key { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("subtitle")]
    public string Subtitle { get; set; }

    [JsonPropertyName("share")]
    public Share Share { get; set; }

    [JsonPropertyName("explicit")]
    public bool Explicit { get; set; }

    [JsonPropertyName("displayname")]
    public string Displayname { get; set; }

    [JsonPropertyName("sections")]
    public List<Section> Sections { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("urlparams")]
    public Dictionary<string, string> Urlparams { get; set; }

    [JsonPropertyName("highlightsurls")]
    public Dictionary<string, object> Highlightsurls { get; set; }

    [JsonPropertyName("relatedtracksurl")]
    public string Relatedtracksurl { get; set; }

}

public class Share
{
    [JsonPropertyName("subject")]
    public string Subject { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; }

    [JsonPropertyName("href")]
    public string Href { get; set; }

    [JsonPropertyName("twitter")]
    public string Twitter { get; set; }

    [JsonPropertyName("html")]
    public string Html { get; set; }

    [JsonPropertyName("snapchat")]
    public string Snapchat { get; set; }

}
