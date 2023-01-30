using Microsoft.Extensions.Logging;
using RyzeTelloSDK.Core;
using System;
using System.Diagnostics;
using DigitalTwinOfUAV.TelloSDK.Core;

namespace TelloApplication
{
    public class FFmpeg
    {
        private readonly ProcessStartInfo processStartInfo;

        private Process process;

        public FFmpeg(TelloConnectionSettings connectionSettings)
        {
            processStartInfo = new ProcessStartInfo()
            {
                FileName = "ffmpeg",
                UseShellExecute = false, // legt fest, ob beim Start des Prozesses die Powershell verwenet werden soll. (nur für Windows) 
                Arguments = $"-i udp://0.0.0.0:{connectionSettings.VideoStreamPort} -f sdl Tello",
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
