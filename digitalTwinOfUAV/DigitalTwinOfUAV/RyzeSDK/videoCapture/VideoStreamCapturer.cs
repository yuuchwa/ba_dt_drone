/*
using Emgu;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using RyzeTelloSDK.Core;

namespace TelloTestApp;

public class VideoStreamCapturer
{
    private VideoCapture _capture;

    public VideoStreamCapturer()
    {
        _capture = new VideoCapture($"upd://{TelloSettings.IpAddress}:{TelloSettings.VideoStreamPort}");
        OpenWindow();
    }

    public void Capure()
    {
        while (true)
        {
            // Rufe das nächste Frame aus dem Stream ab
            _capture.Grab();

            // Extrahiere das Frame aus dem Stream
            Mat frame = new Mat();
            _capture.Retrieve(frame);

            // Zeige das Frame an
            Image<Bgr, byte> imageFrame = frame.ToImage<Bgr, byte>();
            CvInvoke.Imshow("Video", imageFrame);

            // Überprüfe, ob die Benutzerin das Fenster schließt
            if (CvInvoke.WaitKey(1) >= 0)
                break;
        }
    }

    public void OpenWindow()
    {
        CvInvoke.NamedWindow("Video", WindowFlags.Normal);
    }

    public void Close()
    {
        _capture.Dispose();
        CvInvoke.DestroyWindow("Video");
    }
}*/