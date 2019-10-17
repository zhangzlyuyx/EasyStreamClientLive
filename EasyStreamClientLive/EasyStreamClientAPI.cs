using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace EasyStreamClientLive
{
    /// <summary>
    /// EasyStreamClient
    /// </summary>
    public class EasyStreamClientAPI
    {
        /// <summary>
        /// 激活状态
        /// </summary>
        public static bool Activated { get; private set; }

        /// <summary>
        /// StreamClient 句柄
        /// </summary>
        public IntPtr StreamClientHandle { get; private set; }

        /// <summary>
        /// StreamClient 回调
        /// </summary>
        public EasyStreamClientCallBack StreamClientCallBack { get; private set; }

        /// <summary>
        /// 获取是否初始化
        /// </summary>
        public bool IsInit { get { return this.StreamClientHandle != IntPtr.Zero; } }

        /// <summary>
        /// 连接方式
        /// </summary>
        public EASY_RTP_CONNECT_TYPE ConnectType { get; set; } = EASY_RTP_CONNECT_TYPE.EASY_RTP_OVER_TCP;

        /// <summary>
        /// 连接超时时间(秒)
        /// </summary>
        public int ConnectTimeout { get; set; } = 5;

        #region API函数

        const string EasyStreamClient_LIB = "libEasyStreamClient.dll";

        const string EasyStreamClient_KEY = "4638674F756F69576B5A73416C354A646F6D316F772B744659584E35553352795A574674513278705A5735304C6D56345A536C58444661672F38502B4947566863336B3D";

        /// <summary>
        /// EasyStreamClient回调委托
        /// </summary>
        /// <param name="_channelPtr"> 通道信息 </param>
        /// <param name="_frameType"> EASY_SDK_VIDEO_FRAME_FLAG/EASY_SDK_AUDIO_FRAME_FLAG/EASY_SDK_EVENT_FRAME_FLAG </param>
        /// <param name="pBuf"> 回调的数据部分 </param>
        /// <param name="_frameInfo"> 帧结构数据 </param>
        /// <returns></returns>
        public delegate int EasyStreamClientCallBack(IntPtr _channelPtr, int _frameType, IntPtr pBuf, IntPtr _frameInfo);

        /// <summary>
        /// 激活 EasyStreamClient
        /// </summary>
        /// <param name="license"> 激活码 </param>
        /// <returns></returns>
        [DllImport(EasyStreamClient_LIB, EntryPoint = "EasyStreamClient_Activate")]
        private static extern int EasyStreamClient_Activate(string license);

        /// <summary>
        /// 创建 EasyStreamClient 句柄
        /// </summary>
        /// <param name="handle"> EasyStreamClient句柄 </param>
        /// <param name="loglevel">日志级别(0quiet,1debug)</param>
        /// <returns> 返回0表示成功，返回非0表示失败 </returns>
        [DllImport(EasyStreamClient_LIB, EntryPoint = "EasyStreamClient_Init")]
        private static extern int EasyStreamClient_Init(ref IntPtr handle, int loglevel);

        /// <summary>
        /// 释放EasyStreamClient
        /// </summary>
        /// <param name="handle"> EasyStreamClient句柄 </param>
        /// <returns></returns>
        [DllImport(EasyStreamClient_LIB, EntryPoint = "EasyStreamClient_Deinit")]
        private static extern int EasyStreamClient_Deinit(IntPtr handle);

        /// <summary>
        /// 设置数据回调
        /// </summary>
        /// <param name="handle"> EasyStreamClient句柄 </param>
        /// <param name="callback"></param>
        /// <returns></returns>
        [DllImport(EasyStreamClient_LIB, EntryPoint = "EasyStreamClient_SetCallback")]
        private static extern int EasyStreamClient_SetCallback(IntPtr handle, EasyStreamClientCallBack callback);

        /// <summary>
        /// 打开网络流
        /// </summary>
        /// <param name="handle"> EasyStreamClient句柄 </param>
        /// <param name="url"></param>
        /// <param name="connType"></param>
        /// <param name="userPtr"></param>
        /// <param name="reconn"></param>
        /// <param name="timeout"></param>
        /// <param name="useExtraData"></param>
        /// <returns></returns>
        [DllImport(EasyStreamClient_LIB, EntryPoint = "EasyStreamClient_OpenStream")]
        private static extern int EasyStreamClient_OpenStream(IntPtr handle, StringBuilder url, EASY_RTP_CONNECT_TYPE connType, IntPtr userPtr, int reconn, int timeout, int useExtraData);

        /// <summary>
        /// 获取输入流的context
        /// </summary>
        /// <param name="handle"> EasyStreamClient句柄 </param>
        /// <param name="avFormatContext"></param>
        /// <param name="avCodecContext"></param>
        /// <returns></returns>
        [DllImport(EasyStreamClient_LIB, EntryPoint = "EasyStreamClient_GetStreamContext")]
        private static extern int EasyStreamClient_GetStreamContext(IntPtr handle, IntPtr avFormatContext, IntPtr avCodecContext);

        /// <summary>
        /// 获取快照
        /// </summary>
        /// <param name="handle"> EasyStreamClient句柄 </param>
        /// <returns></returns>
        [DllImport(EasyStreamClient_LIB, EntryPoint = "EasyStreamClient_GetSnap")]
        private static extern int EasyStreamClient_GetSnap(IntPtr handle);

        /// <summary>
        /// 获取音频是否启用
        /// </summary>
        /// <param name="handle"> EasyStreamClient句柄 </param>
        /// <returns></returns>
        [DllImport(EasyStreamClient_LIB, EntryPoint = "EasyStreamClient_GetAudioEnable")]
        private static extern int EasyStreamClient_GetAudioEnable(IntPtr handle);

        /// <summary>
        /// 设置音频是否启用
        /// </summary>
        /// <param name="handle"> EasyStreamClient句柄 </param>
        /// <param name="enable"> 是否启用 </param>
        /// <returns></returns>
        [DllImport(EasyStreamClient_LIB, EntryPoint = "EasyStreamClient_SetAudioEnable")]
        private static extern int EasyStreamClient_SetAudioEnable(IntPtr handle, int enable);

        /// <summary>
        /// 连接类型
        /// </summary>
        public enum EASY_RTP_CONNECT_TYPE
        {
            /// <summary>
            /// RTP Over TCP
            /// </summary>
            EASY_RTP_OVER_TCP = 0x01,
            /// <summary>
            /// RTP Over UDP
            /// </summary>
            EASY_RTP_OVER_UDP,
            /// <summary>
            /// RTP Over MULTICAST
            /// </summary>
            EASY_RTP_OVER_MULTICAST
        }

        /// <summary>
        /// 推送事件类型定义
        /// </summary>
        public enum EASY_STREAM_CLIENT_STATE_T
        {
            /// <summary>
            /// 连接中
            /// </summary>
            EASY_STREAM_CLIENT_STATE_CONNECTING = 1,
            /// <summary>
            /// 连接成功
            /// </summary>
            EASY_STREAM_CLIENT_STATE_CONNECTED,
            /// <summary>
            /// 连接失败
            /// </summary>
            EASY_STREAM_CLIENT_STATE_CONNECT_FAILED,
            /// <summary>
            /// 连接中断
            /// </summary>
            EASY_STREAM_CLIENT_STATE_CONNECT_ABORT,
            /// <summary>
            /// 推流中
            /// </summary>
            EASY_STREAM_CLIENT_STATE_PUSHING,
            /// <summary>
            /// 断开连接
            /// </summary>
            EASY_STREAM_CLIENT_STATE_DISCONNECTED,
            /// <summary>
            /// 退出连接
            /// </summary>
            EASY_STREAM_CLIENT_STATE_EXIT,
            /// <summary>
            /// 错误
            /// </summary>
            EASY_STREAM_CLIENT_STATE_ERROR
        }

        /// <summary>
        /// 帧信息 
        /// </summary>
        public struct EASY_FRAME_INFO
        {
            /// <summary>
            /// 音视频格式
            /// </summary>
            public uint codec;
            /// <summary>
            /// 视频帧类型
            /// </summary>
            public uint type;
            /// <summary>
            /// 视频帧率
            /// </summary>
            public byte fps;
            /// <summary>
            /// 视频宽
            /// </summary>
            public ushort width;
            /// <summary>
            /// 视频高
            /// </summary>
            public ushort height;
            /// <summary>
            /// 保留参数1
            /// </summary>
            public uint reserved1;
            /// <summary>
            /// 保留参数2
            /// </summary>
            public uint reserved2;
            /// <summary>
            /// 音频采样率
            /// </summary>
            public uint sample_rate;
            /// <summary>
            /// 音频声道数
            /// </summary>
            public uint channels;
            /// <summary>
            /// 音频采样精度
            /// </summary>
            public uint bits_per_sample;
            /// <summary>
            /// 音视频帧大小
            /// </summary>
            public uint length;
            /// <summary>
            /// 时间戳,微妙
            /// </summary>
            public uint timestamp_usec;
            /// <summary>
            /// 时间戳 秒
            /// </summary>
            public uint timestamp_sec;
            /// <summary>
            /// 比特率
            /// </summary>
            public float bitrate;
            /// <summary>
            /// 丢包率
            /// </summary>
            public float losspacket;

            /// <summary>
            /// 指针转帧结构
            /// </summary>
            /// <param name="frameInfoPtr"></param>
            /// <returns></returns>
            public static EASY_FRAME_INFO Parse(IntPtr frameInfoPtr)
            {
                return (EASY_FRAME_INFO)Marshal.PtrToStructure(frameInfoPtr, typeof(EASY_FRAME_INFO));
            }
        }

        #endregion

        /// <summary>
        /// 激活 EasyStreamClient
        /// </summary>
        /// <returns></returns>
        public static KeyValuePair<bool, string> Activate()
        {
            if (Activated)
            {
                return new KeyValuePair<bool, string>(true, "EasyStreamClient 已激活!");
            }
            try
            {
                int ret = EasyStreamClient_Activate(EasyStreamClient_KEY);
                if (ret < 0)
                {
                    return new KeyValuePair<bool, string>(false, string.Format("EasyStreamClient 激活失败,错误代码：{0}", ret));
                }
                else
                {
                    Activated = true;
                    return new KeyValuePair<bool, string>(true, "EasyStreamClient 激活成功!");
                }
            }
            catch (Exception ex)
            {
                return new KeyValuePair<bool, string>(false, string.Format("EasyStreamClient 激活失败,异常消息：{0}", ex.Message));
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="loglevel">日志级别(0 - quiet,1 - debug)</param>
        /// <returns></returns>
        public KeyValuePair<bool, string> Init(int loglevel = 1)
        {
            if (!Activated)
            {
                return new KeyValuePair<bool, string>(false, "EasyStreamClient 未激活!");
            }
            try
            {
                IntPtr handle = IntPtr.Zero;

                int ret = EasyStreamClient_Init(ref handle, loglevel);

                if (ret != 0)
                {
                    return new KeyValuePair<bool, string>(false, string.Format("EasyStreamClient 初始化失败,错误代码：{0}", ret));
                }
                else
                {
                    this.StreamClientHandle = handle;

                    return new KeyValuePair<bool, string>(true, "EasyStreamClient 初始化成功!");
                }
            }
            catch (Exception ex)
            {
                return new KeyValuePair<bool, string>(false, string.Format("EasyStreamClient 初始化失败,异常消息：{0}", ex.Message));
            }
        }

        /// <summary>
        /// 释放
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<bool, string> Deinit()
        {
            if (!this.IsInit)
            {
                return new KeyValuePair<bool, string>(true, "EasyStreamClient 未始化!");
            }
            try
            {
                EasyStreamClient_Deinit(this.StreamClientHandle);

                this.StreamClientHandle = IntPtr.Zero;

                return new KeyValuePair<bool, string>(true, "EasyStreamClient 释放成功!");
            }
            catch (Exception ex)
            {
                return new KeyValuePair<bool, string>(false, string.Format("EasyStreamClient 释放失败,异常消息：{0}", ex.Message));
            }
        }

        /// <summary>
        /// 设置是否启用音频
        /// </summary>
        /// <param name="enable"> 是否启用 </param>
        /// <returns></returns>
        public KeyValuePair<bool, string> SetAudioEnable(bool enable)
        {
            if (!this.IsInit)
            {
                return new KeyValuePair<bool, string>(true, "EasyStreamClient 未始化!");
            }
            try
            {
                int ret = EasyStreamClient_SetAudioEnable(this.StreamClientHandle, enable ? 1 : 0);

                if (ret != 0)
                {
                    return new KeyValuePair<bool, string>(false, string.Format("设置音频失败,错误代码：{0}", ret));
                }
                else
                {
                    return new KeyValuePair<bool, string>(true, "设置音频成功!");
                }
            }
            catch (Exception ex)
            {
                return new KeyValuePair<bool, string>(false, string.Format("EasyStreamClient 设置音频失败,异常消息：{0}", ex.Message));
            }
        }

        /// <summary>
        /// 设置回调
        /// </summary>
        /// <param name="streamClientCallBack"> 回调委托 </param>
        /// <returns></returns>
        public KeyValuePair<bool, string> SetCallback(EasyStreamClientCallBack streamClientCallBack)
        {
            try
            {
                this.StreamClientCallBack = streamClientCallBack;

                int ret = EasyStreamClient_SetCallback(this.StreamClientHandle, this.StreamClientCallBack);

                if (ret != 0)
                {
                    return new KeyValuePair<bool, string>(false, string.Format("设置回调失败,错误代码：{0}", ret));
                }
                else
                {
                    return new KeyValuePair<bool, string>(true, "设置回调成功!");
                }
            }
            catch (Exception ex)
            {
                return new KeyValuePair<bool, string>(false, string.Format("EasyStreamClient 设置回调失败,异常消息：{0}", ex.Message));
            }
        }

        /// <summary>
        /// 打开网络流
        /// </summary>
        /// <param name="url"> 网络地址 </param>
        /// <param name="userPtr"> 用户信息 </param>
        /// <returns></returns>
        public KeyValuePair<bool, string> OpenStream(StringBuilder url, IntPtr userPtr)
        {
            try
            {
                int ret = EasyStreamClient_OpenStream(this.StreamClientHandle, url, this.ConnectType, userPtr, 1000, this.ConnectTimeout, 1);

                if (ret != 0)
                {
                    return new KeyValuePair<bool, string>(false, string.Format("打开网络流失败,错误代码：{0}", ret));
                }
                else
                {
                    return new KeyValuePair<bool, string>(true, "打开网络流成功!");
                }
            }
            catch (Exception ex)
            {
                return new KeyValuePair<bool, string>(false, string.Format("EasyStreamClient 打开网络流失败,异常消息：{0}", ex.Message));
            }
        }
    }
}
