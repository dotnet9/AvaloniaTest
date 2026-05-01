using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Avalonia.MusicStore.Models;

public partial class Album
{
    private static readonly HttpClient HttpClient = new();

    public Album(string artist, string title, string coverUrl)
    {
        Artist = artist;
        Title = title;
        CoverUrl = coverUrl;
    }

    public string Artist { get; set; }

    public string Title { get; set; }

    public string CoverUrl { get; set; }

    private string CachePath => $"./Cache/{Artist} - {Title}";

    public async Task<Stream> LoadCoverBitmapAsync()
    {
        if (File.Exists(CachePath + ".bmp"))
        {
            return File.OpenRead(CachePath + ".bmp");
        }

        var data = await HttpClient.GetByteArrayAsync(CoverUrl);
        return new MemoryStream(data);
    }

    public async Task SaveAsync()
    {
        if (!Directory.Exists("./Cache"))
        {
            Directory.CreateDirectory("./Cache");
        }

        await using var fs = File.OpenWrite(CachePath);
        await SaveToStreamAsync(this, fs);
    }

    public Stream SaveCoverBitmapStream()
    {
        return File.OpenWrite(CachePath + ".bmp");
    }

    private static async Task SaveToStreamAsync(Album data, Stream stream)
    {
        await JsonSerializer.SerializeAsync(stream, data, AlbumJsonSerializerContext.Default.Album).ConfigureAwait(false);
    }

    public static async Task<Album> LoadFromStream(Stream stream)
    {
        return (await JsonSerializer.DeserializeAsync(stream, AlbumJsonSerializerContext.Default.Album).ConfigureAwait(false))!;
    }

    public static async Task<IEnumerable<Album>> LoadCachedAsync()
    {
        if (!Directory.Exists("./Cache"))
        {
            Directory.CreateDirectory("./Cache");
        }

        var results = new List<Album>();

        foreach (var file in Directory.EnumerateFiles("./Cache"))
        {
            if (!string.IsNullOrWhiteSpace(new DirectoryInfo(file).Extension))
            {
                continue;
            }

            await using var fs = File.OpenRead(file);
            results.Add(await LoadFromStream(fs).ConfigureAwait(false));
        }

        return results;
    }

    public static async Task<IEnumerable<Album>> SearchAsync(string searchTerm)
    {
        var requestUri = $"https://itunes.apple.com/search?term={System.Uri.EscapeDataString(searchTerm)}&entity=album";
        await using var stream = await HttpClient.GetStreamAsync(requestUri).ConfigureAwait(false);
        var query = await JsonSerializer.DeserializeAsync(stream, AlbumJsonSerializerContext.Default.ITunesSearchResponse).ConfigureAwait(false);

        return (query?.Results ?? [])
            .Where(x => !string.IsNullOrWhiteSpace(x.ArtistName)
                && !string.IsNullOrWhiteSpace(x.CollectionName)
                && !string.IsNullOrWhiteSpace(x.ArtworkUrl100))
            .Select(x => new Album(
                x.ArtistName!,
                x.CollectionName!,
                x.ArtworkUrl100!.Replace("100x100bb", "600x600bb")));
    }

    private sealed class ITunesSearchResponse
    {
        [JsonPropertyName("results")]
        public List<ITunesAlbumResult> Results { get; set; } = [];
    }

    private sealed class ITunesAlbumResult
    {
        [JsonPropertyName("artistName")]
        public string? ArtistName { get; set; }

        [JsonPropertyName("collectionName")]
        public string? CollectionName { get; set; }

        [JsonPropertyName("artworkUrl100")]
        public string? ArtworkUrl100 { get; set; }
    }

    [JsonSerializable(typeof(Album))]
    [JsonSerializable(typeof(ITunesSearchResponse))]
    private sealed partial class AlbumJsonSerializerContext : JsonSerializerContext
    {
    }
}
