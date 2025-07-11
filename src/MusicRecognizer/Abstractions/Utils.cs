namespace MusicRecognizer.Abstraction;

internal static class Utils
{
    private static readonly Dictionary<string, Tuple<byte[], int>> _audioFileSignatures = new Dictionary<string, Tuple<byte[], int>>()
    {
        // MP3 (ID3v2 etiketi)
        { "mp3", Tuple.Create(new byte[] { 0x49, 0x44, 0x33 }, 0) },
        { "mp3-mpeg", Tuple.Create(new byte[] { 0xFF, 0xFB }, 0) },
        { "mp3_mpeg_frame", Tuple.Create(new byte[] { 0xFF, 0xF0 }, 0) },

        // WAV
        // WAV için hem RIFF (offset 0) hem de WAVE (offset 8) kontrol edilmeli
        { "wav-riff", Tuple.Create(new byte[] { 0x52, 0x49, 0x46, 0x46 }, 0) }, // RIFF
        { "wav-wave", Tuple.Create(new byte[] { 0x57, 0x41, 0x56, 0x45 }, 8) }, // WAVE

        // AIFF
        // AIFF için hem FORM (offset 0) hem de AIFF/AIFC (offset 8) kontrol edilmeli
        { "aiff-form", Tuple.Create(new byte[] { 0x46, 0x4F, 0x52, 0x4D }, 0) }, // FORM
        { "aiff-aiff", Tuple.Create(new byte[] { 0x41, 0x49, 0x46, 0x46 }, 8) }, // AIFF
        { "aiff-aifc", Tuple.Create(new byte[] { 0x41, 0x49, 0x46, 0x43 }, 8) }, // AIFC (Sıkıştırılmış AIFF)

        // WMA (ASF Header Object GUID)
        { "wma", Tuple.Create(new byte[] { 0x30, 0x26, 0xB2, 0x75, 0x8E, 0x66, 0xCF, 0x11, 0xA6, 0xD9, 0x00, 0xAA, 0x00, 0x62, 0xCE, 0x6C }, 0) },

        // M4A (MP4 ftyp kutusu)
        // M4A için ftyp (offset 4) ve M4A  (offset 8) kontrol edilmeli
        { "m4a-ftyp", Tuple.Create(new byte[] { 0x66, 0x74, 0x79, 0x70 }, 4) }, // ftyp
        { "m4a-id", Tuple.Create(new byte[] { 0x4D, 0x34, 0x41, 0x20 }, 8) }, // M4A  (boşluk karakterine dikkat)
        // M4B (sesli kitaplar için)
        { "m4b-id", Tuple.Create(new byte[] { 0x4D, 0x34, 0x42, 0x20 }, 8) }, // M4B

        // AAC (ADTS Header - ham akışlar için)
        // Bu daha az kesin olabilir, çünkü ADTS başlıkları dosya boyunca tekrar eder.
        { "aac", Tuple.Create(new byte[] { 0xFF, 0xF1 }, 0) }, // Genellikle MPEG-4 AAC LC
        { "aac-mpeg2", Tuple.Create(new byte[] { 0xFF, 0xF9 }, 0) }, // Genellikle MPEG-2 AAC LC
    };

    private static float[] ComputeGaussianKernel1D(int radius, float sigma)
    {
        var kernel = new float[radius * 2 + 1];
        float sum = 0;
        for (var i = 0; i < kernel.Length; i++) sum += kernel[i] = (float)Math.Exp(-.5 * Math.Pow((i - radius) / sigma, 2));
        for (var i = 0; i < kernel.Length; i++) kernel[i] /= sum;
        return kernel;
    }

    public static string FileExtensionFromStream(Stream stream)
    {
        static bool CheckSignature(byte[] header, Tuple<byte[], int> signatureInfo)
        {
            byte[] signature = signatureInfo.Item1;
            int offset = signatureInfo.Item2;

            if (header.Length < offset + signature.Length)
            {
                return false;
            }

            for (int i = 0; i < signature.Length; i++)
            {
                if (header[offset + i] != signature[i])
                {
                    return false;
                }
            }
            return true;
        }

        if (stream == null || !stream.CanRead)
        {
            return null;
        }

        // Akışın başına git. Okuma işleminden sonra orijinal pozisyona dönülebilir,
        // ancak bu metodun tek sorumluluğu uzantıyı bulmak olduğundan, şimdilik basit tutuyoruz.
        stream.Seek(0, SeekOrigin.Begin);

        byte[] header = new byte[16];
        int bytesRead = stream.Read(header, 0, header.Length);

        if (bytesRead == 0)
        {
            return null; // Boş akış
        }

        // WAV kontrolü
        bool isWavRiff = CheckSignature(header, _audioFileSignatures["wav-riff"]);
        bool isWavWave = CheckSignature(header, _audioFileSignatures["wav-wave"]);
        if (isWavRiff && isWavWave)
        {
            return "wav";
        }

        // AIFF kontrolü
        bool isAiffForm = CheckSignature(header, _audioFileSignatures["aiff-form"]);
        bool isAiffAiff = CheckSignature(header, _audioFileSignatures["aiff-aiff"]);
        bool isAiffAifc = CheckSignature(header, _audioFileSignatures["aiff-aifc"]);
        if (isAiffForm && (isAiffAiff || isAiffAifc))
        {
            return "aiff";
        }

        // M4A kontrolü
        bool isM4aFtyp = CheckSignature(header, _audioFileSignatures["m4a-ftyp"]);
        bool isM4aId = CheckSignature(header, _audioFileSignatures["m4a-id"]);
        bool isM4bId = CheckSignature(header, _audioFileSignatures["m4b-id"]);
        if (isM4aFtyp && (isM4aId || isM4bId))
        {
            return isM4aId ? "m4a" : "m4b";
        }

        if (CheckSignature(header, _audioFileSignatures["aac"]) || CheckSignature(header, _audioFileSignatures["aac-mpeg2"]))
        {
            return "aac";
        }

        if (CheckSignature(header, _audioFileSignatures["mp3"]))
        {
            return "mp3";
        }

        foreach (var signatureEntry in _audioFileSignatures)
        {

            if (signatureEntry.Key.StartsWith("wav-") || signatureEntry.Key.StartsWith("aiff-") || signatureEntry.Key.StartsWith("m4a-"))
            {
                continue;
            }

            if (CheckSignature(header, signatureEntry.Value))
            {
                if (signatureEntry.Key == "mp3-mpeg")
                {
                    return "mp3";
                }
                return signatureEntry.Key;
            }
        }

        return null; // Bilinmeyen uzantı
    }

}
