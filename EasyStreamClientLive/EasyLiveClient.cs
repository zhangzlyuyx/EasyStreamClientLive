using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyStreamClientLive
{
    /// <summary>
    /// 直播客户端
    /// </summary>
    public class EasyLiveClient
    {
        /// <summary>
        /// 获取或设置输入rtsp地址
        /// </summary>
        public string RtspUrl { get; set; }

        /// <summary>
        /// 获取或设置输出rtmp地址
        /// </summary>
        public string RtmpUrl { get; set; }

        private EasyStreamClientAPI streamClient;

        private EasyStreamClientAPI.EasyStreamClientCallBack streamClientCallBack;

        private EasyRTMPAPI rtmpClient;

        private EasyRTMPAPI.EasyRTMPCallBack rtmpCallback;

        private EasyRTMPAPI.EASY_MEDIA_INFO_T mediaInfo;

        public EasyLiveClient()
        {
            this.streamClient = new EasyStreamClientLive.EasyStreamClientAPI();
            this.streamClientCallBack = new EasyStreamClientAPI.EasyStreamClientCallBack(this.EasyStreamClientCallBack);

            this.rtmpClient = new EasyStreamClientLive.EasyRTMPAPI();
            this.rtmpCallback = new EasyRTMPAPI.EasyRTMPCallBack(this.EasyRTMPCallBack);
        }

        /// <summary>
        /// 启动
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<bool,string> Start()
        {
            //激活rtsp
            if (!EasyStreamClientAPI.Activated)
            {
                var retActivate = EasyStreamClientAPI.Activate();
                if (!retActivate.Key)
                {
                    return retActivate;
                }
            }
            //激活rtmp
            if (!EasyRTMPAPI.Activated)
            {
                var retActivate = EasyRTMPAPI.Activate();
                if (!retActivate.Key)
                {
                    return retActivate;
                }
            }
            //初始化rtsp
            var ret = this.streamClient.Init();
            if (!ret.Key)
            {
                this.Stop();
                return ret;
            }
            //启用音频
            streamClient.SetAudioEnable(true);
            //设置rtsp回调
            ret = this.streamClient.SetCallback(this.streamClientCallBack);
            if (!ret.Key)
            {
                this.Stop();
                return ret;
            }
            //打开rtsp网络
            ret = this.streamClient.OpenStream(new StringBuilder(this.RtspUrl), IntPtr.Zero);
            if (!ret.Key)
            {
                this.Stop();
                return ret;
            }
            return new KeyValuePair<bool, string>(true, "启动成功!");
        }


        public KeyValuePair<bool,string> Stop()
        {
            return new KeyValuePair<bool, string>();
        }

        private int EasyStreamClientCallBack(IntPtr channelPtr, int frameType, IntPtr pBuf, IntPtr frameInfoPtr)
        {
            if (frameType == EASY_SDK_VIDEO_FRAME_FLAG || frameType == EASY_SDK_AUDIO_FRAME_FLAG)//视频或音频帧
            {
                if(frameInfoPtr == IntPtr.Zero)
                {
                    return 0;
                }
                var frameInfo = EasyStreamClientAPI.EASY_FRAME_INFO.Parse(frameInfoPtr);
                if(frameInfo.length > 0)
                {
                    if (!this.rtmpClient.IsInit)
                    {
                        //初始化rtmp
                        var ret = this.rtmpClient.Create();
                        if (!ret.Key)
                        {
                            return -1;
                        }
                        //设置rtmp回调
                        ret = this.rtmpClient.SetCallback(this.rtmpCallback, IntPtr.Zero);
                        if (!ret.Key)
                        {
                            return -1;
                        }
                        //连接
                        ret = this.rtmpClient.Connect(this.RtmpUrl);
                        if (!ret.Key)
                        {
                            //TODO:
                        }
                        //媒体信息
                        ret = this.rtmpClient.InitMetadata(this.mediaInfo);
                        if (!ret.Key)
                        {

                        }
                    }

                    //
                    if (this.rtmpClient.IsInit)
                    {
                        EasyRTMPAPI.EASY_AV_Frame avFrame = new EasyRTMPAPI.EASY_AV_Frame();
                        avFrame.u32AVFrameFlag = (uint)frameType;
                        avFrame.u32AVFrameLen = frameInfo.length;
                        avFrame.pBuffer = pBuf;
                        avFrame.u32VFrameType = frameInfo.type;
                        avFrame.u32TimestampSec = frameInfo.timestamp_sec;
                        avFrame.u32TimestampUsec = frameInfo.timestamp_usec;

                        var ret = this.rtmpClient.SendPacket(avFrame);
                        if (!ret.Key)
                        {
                            //TODO:
                        }
                    }
                }
            }
            else if (frameType == EASY_SDK_MEDIA_INFO_FLAG)//回调出媒体信息
            {
                this.mediaInfo = EasyStreamClientLive.EasyRTMPAPI.EASY_MEDIA_INFO_T.Parse(pBuf);
            }
            else if (frameType == EASY_SDK_EVENT_FRAME_FLAG)//事件帧
            {
                var frameInfo = EasyStreamClientAPI.EASY_FRAME_INFO.Parse(frameInfoPtr);

                if (frameInfo.codec == (uint)EasyStreamClientAPI.EASY_STREAM_CLIENT_STATE_T.EASY_STREAM_CLIENT_STATE_DISCONNECTED)
                {
                    Console.WriteLine("----------------------channel source stream disconnected ----------------------");
                }
                else if (frameInfo.codec == (uint)EasyStreamClientAPI.EASY_STREAM_CLIENT_STATE_T.EASY_STREAM_CLIENT_STATE_CONNECTED)
                {
                    Console.WriteLine("---------------------- channel source stream connected ----------------------");
                }
                else if (frameInfo.codec == (uint)EasyStreamClientAPI.EASY_STREAM_CLIENT_STATE_T.EASY_STREAM_CLIENT_STATE_EXIT)
                {
                    Console.WriteLine("---------------------- channel source stream exit ----------------------");
                }
            }
            else if (frameType == EASY_SDK_SNAP_FRAME_FLAG)//快照帧
            {

            }
            return 0;
        }

        private int EasyRTMPCallBack(int frameType, IntPtr pBuf, EasyRTMPAPI.EASY_RTMP_STATE_T state, IntPtr userPtr)
        {
            if(state == EasyRTMPAPI.EASY_RTMP_STATE_T.EASY_RTMP_STATE_CONNECTING)
            {
                Console.WriteLine("----------------------rtmp connecting------------------------");
            }
            else if (state == EasyRTMPAPI.EASY_RTMP_STATE_T.EASY_RTMP_STATE_CONNECTED)
            {
                Console.WriteLine("----------------------rtmp connected------------------------");
            }
            else if (state == EasyRTMPAPI.EASY_RTMP_STATE_T.EASY_RTMP_STATE_CONNECT_FAILED)
            {
                Console.WriteLine("----------------------rtmp connect failed ------------------------");
            }
            else if (state == EasyRTMPAPI.EASY_RTMP_STATE_T.EASY_RTMP_STATE_CONNECT_ABORT)
            {
                Console.WriteLine("----------------------rtmp connect abort ------------------------");
            }
            else if (state == EasyRTMPAPI.EASY_RTMP_STATE_T.EASY_RTMP_STATE_DISCONNECTED)
            {
                Console.WriteLine("----------------------rtmp disconnected ------------------------");
            }
            return 0;
        }

        #region 常量/枚举

        /* 视频编码 */
        public const int EASY_SDK_VIDEO_CODEC_H264 = 0x1C;		/* H264  */
        public const int EASY_SDK_VIDEO_CODEC_H265 = 0xAE;	  /* H265 */
        public const int EASY_SDK_VIDEO_CODEC_MJPEG = 0x08;		/* MJPEG */
        public const int EASY_SDK_VIDEO_CODEC_MPEG4 = 0x0D;		/* MPEG4 */

        /* 音频编码 */
        public const int EASY_SDK_AUDIO_CODEC_AAC = 0x15002;		/* AAC */
        public const int EASY_SDK_AUDIO_CODEC_G711U = 0x10006;		/* G711 ulaw*/
        public const int EASY_SDK_AUDIO_CODEC_G711A = 0x10007;		/* G711 alaw*/
        public const int EASY_SDK_AUDIO_CODEC_G726 = 0x1100B;		/* G726 */

        public const int EASY_SDK_EVENT_CODEC_ERROR = 0x63657272;	/* ERROR */
        public const int EASY_SDK_EVENT_CODEC_EXIT = 0x65786974;	/* EXIT */
        public const int EASY_SDK_EVENT_CODEC_FAIL = 0x7265636F;/* Fail */

        /* 音视频帧标识 */
        public const int EASY_SDK_VIDEO_FRAME_FLAG = 0x00000001;		/* 视频帧标志 */
        public const int EASY_SDK_AUDIO_FRAME_FLAG = 0x00000002;		/* 音频帧标志 */
        public const int EASY_SDK_EVENT_FRAME_FLAG = 0x00000004;		/* 事件帧标志 */
        public const int EASY_SDK_RTP_FRAME_FLAG = 0x00000008;		/* RTP帧标志 */
        public const int EASY_SDK_SDP_FRAME_FLAG = 0x00000010;		/* SDP帧标志 */
        public const int EASY_SDK_MEDIA_INFO_FLAG = 0x00000020;		/* 媒体类型标志*/
        public const int EASY_SDK_SNAP_FRAME_FLAG = 0x00000040;		/* 图片标志*/

        /* 视频关键字标识 */
        public const int EASY_SDK_VIDEO_FRAME_I = 0x01;		/* I帧 */
        public const int EASY_SDK_VIDEO_FRAME_P = 0x02;		/* P帧 */
        public const int EASY_SDK_VIDEO_FRAME_B = 0x03;		/* B帧 */
        public const int EASY_SDK_VIDEO_FRAME_J = 0x04;		/* JPEG */

        public enum Easy_Error
        {
            Easy_NoErr = 0,
            Easy_RequestFailed = -1,
            Easy_Unimplemented = -2,
            Easy_RequestArrived = -3,
            Easy_OutOfState = -4,
            Easy_NotAModule = -5,
            Easy_WrongVersion = -6,
            Easy_IllegalService = -7,
            Easy_BadIndex = -8,
            Easy_ValueNotFound = -9,
            Easy_BadArgument = -10,
            Easy_ReadOnly = -11,
            Easy_NotPreemptiveSafe = -12,
            Easy_NotEnoughSpace = -13,
            Easy_WouldBlock = -14,
            Easy_NotConnected = -15,
            Easy_FileNotFound = -16,
            Easy_NoMoreData = -17,
            Easy_AttrDoesntExist = -18,
            Easy_AttrNameExists = -19,
            Easy_InstanceAttrsNotAllowed = -20,
            Easy_InvalidSocket = -21,
            Easy_MallocError = -22,
            Easy_ConnectError = -23,
            Easy_SendError = -24
        }

        public enum EASY_ACTIVATE_ERR_CODE_ENUM
        {
            EASY_ACTIVATE_INVALID_KEY = -1,			/* 无效Key */
            EASY_ACTIVATE_TIME_ERR = -2,			/* 时间错误 */
            EASY_ACTIVATE_PROCESS_NAME_LEN_ERR = -3,			/* 进程名称长度不匹配 */
            EASY_ACTIVATE_PROCESS_NAME_ERR = -4,			/* 进程名称不匹配 */
            EASY_ACTIVATE_VALIDITY_PERIOD_ERR = -5,			/* 有效期校验不一致 */
            EASY_ACTIVATE_PLATFORM_ERR = -6,			/* 平台不匹配 */
            EASY_ACTIVATE_COMPANY_ID_LEN_ERR = -7,			/* 授权使用商不匹配 */
            EASY_ACTIVATE_SUCCESS = 9999,		/* 激活成功 */
        }

        #endregion
    }
}
