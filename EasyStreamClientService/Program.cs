using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyStreamClientService
{
    class Program
    {
        static List<EasyStreamClientLive.EasyLiveClient> clients = new List<EasyStreamClientLive.EasyLiveClient>();

        static void Main(string[] args)
        {
            for (int i = 0; i < 10; i++)
            {
                EasyStreamClientLive.EasyLiveClient liveClient = new EasyStreamClientLive.EasyLiveClient();
                clients.Add(liveClient);

                liveClient.RtspUrl = "rtsp://admin:abc123456@10.115.99.11:554/cam/realmonitor?channel=1&subtype=1";
                liveClient.RtmpUrl = "rtmp://192.168.0.107:1935/live/demo" + i.ToString();
                liveClient.Start();
            }
            

            Console.ReadLine();
        }
    }
}
