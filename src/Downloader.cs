using System;
using FFmpeg.NET;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace ytdlvids_v2
{
    public class Downloader
    {
        private static string Url { get; set; }
        private static string Path { get; set; }

        private static string _newUrl = null;
        private static YoutubeClient _youtubeClient = new YoutubeClient();
        public static async Task Welcome()
        {
            Console.WriteLine("paste the url of the video:");
            Url = Console.ReadLine();
            Console.WriteLine("paste the path:");
            Path = Console.ReadLine();

            await Task.Run(() => DownloadVideoAudio());
        }  

        private static async Task DownloadVideoAudio()
        {
            Manifest(Url);

            var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(_newUrl);

            var videoInfo = streamManifest.GetVideoOnly()
                .Where(s => s.Container == Container.Mp4).WithHighestVideoQuality();

            var audioInfo = streamManifest.GetAudioOnly().WithHighestBitrate();

            if (videoInfo != null && audioInfo != null)
            {
                var test1 = _youtubeClient.Videos.Streams.DownloadAsync(audioInfo, $"{Path}\audio.wav");
                var test2 = _youtubeClient.Videos.Streams.DownloadAsync(videoInfo, $"{Path}\video.mp4");
                await Task.WhenAll(new[] { test1, test2 }).ContinueWith(r => MergeAudioVideo(Path));
            }
        }

        private static async Task MergeAudioVideo(string path)
        {
            var ffmpeg = new Engine($@"{Environment.CurrentDirectory}\Engine\ffmpeg.exe");
            await ffmpeg.ExecuteAsync($@"-i {Environment.CurrentDirectory}\Temp\video.mp4  -i {Environment.CurrentDirectory}\Temp\audio.wav -c:v copy -c:a aac -map 0:v:0 -map 1:a:0 -shortest {path}\video.mp4");
        }

        private static void Manifest(string Url)
        {
            var pos = Url.LastIndexOf('=') + 1;
            _newUrl = Url.Substring(pos);
        }
    }
}
