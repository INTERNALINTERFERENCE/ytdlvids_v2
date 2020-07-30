using System;
using System.Threading.Tasks;

namespace ytdlvids_v2
{
    class Program
    {
        async static Task Main(string[] args)
            => await Task.Run(() => Downloader.Welcome());
    }
}
