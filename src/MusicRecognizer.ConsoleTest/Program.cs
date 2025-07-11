namespace MusicRecognizer.ConsoleTest;

internal class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("🎵 Shazam Test Uygulaması 🎵");
        Console.WriteLine("-------------------------------------------------------");

        Console.Write("Lütfen test edilecek ses dosyasının tam yolunu girin (örn: C:\\Muzik\\test.mp3): ");
        string? filePath = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(filePath))
        {
            Console.WriteLine("❌ Geçersiz dosya yolu girdiniz. Program sonlandırılıyor.");
            return;
        }

        await ShazamRecognizer.Recognize(filePath);
    }
}

