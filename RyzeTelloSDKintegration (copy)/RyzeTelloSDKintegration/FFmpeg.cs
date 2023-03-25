using Microsoft.Extensions.Logging;
using RyzeTelloSDK.Core;
using System;
using System.Diagnostics;

namespace TelloTestApp
{
    public class FFmpeg
    {
        private readonly ProcessStartInfo processStartInfo;

        private Process process;

        public FFmpeg(TelloSettings settings)
        {
            processStartInfo = new ProcessStartInfo()
            {
                FileName = "ffmpeg",
                UseShellExecute = true,
                Arguments = $"-i udp://0.0.0.0:{TelloSettings.VideoStreamPort} -f sdl Tello",
                WindowStyle = ProcessWindowStyle.Minimized
            };
        }

        public void Spawn()
        {
            if (process != null && !process.HasExited) return;
            process = Process.Start(processStartInfo);
        }

        public void Close()
        {
            if (process == null || process.HasExited) return;
            process.Kill();
        }
    }
}
