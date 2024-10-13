using System.Diagnostics;

namespace AlphalyBot.Tool;

internal static class AudioCutter
{
    public static async Task RandomClipFromAudioAsync(string audioFile, string outputFile)
    {
        var audio = "audio/" + audioFile + ".mp3";
        // 获取音频的总时长
        var duration = await GetAudioDurationAsync(audio);

        // 生成随机的开始时间（以秒为单位）
        var random = new Random();
        var randomStart = random.Next(0, (int)(duration.TotalSeconds - 20)); // 确保有足够的时间留给20秒

        await RunFFmpegAsync(audio, outputFile, randomStart, 20);
    }

    private static async Task<TimeSpan> GetAudioDurationAsync(string filePath)
    {
        // 获取音频总时长，使用 ffprobe 来实现
        var ffProbe = new Process();
        ffProbe.StartInfo.FileName = "ffprobe"; // 假设ffprobe已添加到系统环境变量中
        ffProbe.StartInfo.Arguments = $"-i \"{filePath}\" -show_entries format=duration -v quiet -of csv=\"p=0\"";
        ffProbe.StartInfo.RedirectStandardOutput = true;
        ffProbe.StartInfo.RedirectStandardError = true;
        ffProbe.StartInfo.UseShellExecute = false;
        ffProbe.StartInfo.CreateNoWindow = true;

        ffProbe.Start();
        var output = await ffProbe.StandardOutput.ReadToEndAsync();
        await ffProbe.WaitForExitAsync();

        if (double.TryParse(output, out var durationInSeconds))
            return TimeSpan.FromSeconds(durationInSeconds);
        throw new Exception("无法获取音频时长。");
    }

    private static async Task RunFFmpegAsync(string inputFile, string outputFile, int startTime, int duration)
    {
        var ffmpeg = new Process();
        ffmpeg.StartInfo.FileName = "ffmpeg"; // 假设ffmpeg已添加到系统环境变量中
        ffmpeg.StartInfo.Arguments =
            $"-i \"{inputFile}\" -ss {startTime} -t {duration} -c copy \"{outputFile}\" -y";
        ffmpeg.StartInfo.RedirectStandardOutput = true;
        ffmpeg.StartInfo.RedirectStandardError = true;
        ffmpeg.StartInfo.UseShellExecute = false;
        ffmpeg.StartInfo.CreateNoWindow = true;

        ffmpeg.Start();
        await ffmpeg.WaitForExitAsync();
    }
}