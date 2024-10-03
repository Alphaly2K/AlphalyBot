using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AlphalyBot.Tool
{
    internal class AudioCutter
    {
        public static async Task RandomClipFromAudioUrlAsync(string audioUrl, string outputFile)
        {
            string tempFile = Guid.NewGuid().ToString()+".mp3"; // 下载的临时音频文件

            // 异步下载音频文件
            using (WebClient webClient = new WebClient())
            {
                Console.WriteLine("正在下载音频文件...");
                await webClient.DownloadFileTaskAsync(new Uri(audioUrl), tempFile);
            }

            // 获取音频的总时长
            TimeSpan duration = await GetAudioDurationAsync(tempFile);

            // 确保音频长度足够
            if (duration.TotalSeconds <= 20)
            {
                throw new Exception("音频长度小于或等于20秒，无法随机剪切。");
            }

            // 生成随机的开始时间（以秒为单位）
            Random random = new Random();
            int randomStart = random.Next(0, (int)(duration.TotalSeconds - 20)); // 确保有足够的时间留给20秒

            // 使用 FFmpeg 随机剪切音频
            Console.WriteLine($"正在从 {randomStart} 秒开始剪切20秒的音频...");
            await RunFFmpegAsync(tempFile, outputFile, randomStart, 20);

            // 删除临时文件
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }

        public static async Task<TimeSpan> GetAudioDurationAsync(string filePath)
        {
            // 获取音频总时长，使用 ffprobe 来实现
            Process ffprobe = new Process();
            ffprobe.StartInfo.FileName = "ffprobe";  // 假设ffprobe已添加到系统环境变量中
            ffprobe.StartInfo.Arguments = $"-i \"{filePath}\" -show_entries format=duration -v quiet -of csv=\"p=0\"";
            ffprobe.StartInfo.RedirectStandardOutput = true;
            ffprobe.StartInfo.RedirectStandardError = true;
            ffprobe.StartInfo.UseShellExecute = false;
            ffprobe.StartInfo.CreateNoWindow = true;

            ffprobe.Start();
            string output = await ffprobe.StandardOutput.ReadToEndAsync();
            ffprobe.WaitForExit();

            if (double.TryParse(output, out double durationInSeconds))
            {
                return TimeSpan.FromSeconds(durationInSeconds);
            }
            else
            {
                throw new Exception("无法获取音频时长。");
            }
        }

        public static async Task RunFFmpegAsync(string inputFile, string outputFile, int startTime, int duration)
        {
            Process ffmpeg = new Process();
            ffmpeg.StartInfo.FileName = "ffmpeg";  // 假设ffmpeg已添加到系统环境变量中
            ffmpeg.StartInfo.Arguments = $"-i \"{inputFile}\" -ss {startTime} -t {duration} -c copy \"{outputFile}\" -y";
            ffmpeg.StartInfo.RedirectStandardOutput = true;
            ffmpeg.StartInfo.RedirectStandardError = true;
            ffmpeg.StartInfo.UseShellExecute = false;
            ffmpeg.StartInfo.CreateNoWindow = true;

            ffmpeg.Start();
            await ffmpeg.WaitForExitAsync();
        }
    }
}
