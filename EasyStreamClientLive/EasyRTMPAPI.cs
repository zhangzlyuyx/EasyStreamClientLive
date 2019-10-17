using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace EasyStreamClientLive
{
    /// <summary>
    /// RTMP推流客户端
    /// </summary>
    public class EasyRTMPAPI
    {
        #region API函数

#if LINUX
        const string EasyRTMP_LIB = "libeasyrtmp.so";
        const string EasyRTMP_KEY = "79736C36655A4F576B597141725370636F39565245764E6C59584E35636E52746346396F6157736A567778576F5036532F69426C59584E35";
#else
        const string EasyRTMP_LIB = "libeasyrtmp.dll";
        const string EasyRTMP_KEY = "79736C36655969576B5A734154526C646F756179532B394659584E35556C524E55463949535573755A58686C4B56634D5671442F532F34675A57467A65513D3D";
#endif

        /// <summary>
        /// RTMP 回调
        /// typedef int (*EasyRTMPCallBack)(int _frameType, char *pBuf, EASY_RTMP_STATE_T _state, void *_userPtr);
        /// </summary>
        /// <param name="frameType"> EASY_SDK_VIDEO_FRAME_FLAG/EASY_SDK_AUDIO_FRAME_FLAG/EASY_SDK_EVENT_FRAME_FLAG/... </param>
        /// <param name="pBuf"> 回调的数据部分 </param>
        /// <param name="state"></param>
        /// <param name="userPtr"> 用户自定义数据 </param>
        /// <returns></returns>
        public delegate int EasyRTMPCallBack(int frameType, IntPtr pBuf, EASY_RTMP_STATE_T state, IntPtr userPtr);

        /// <summary>
        /// rtmp 激活
        /// </summary>
        /// <param name="license"></param>
        /// <returns></returns>
        [DllImport(EasyRTMP_LIB, EntryPoint = "EasyRTMP_Activate")]
        private extern static int EasyRTMP_Activate(string license);

        /// <summary>
        /// 创建RTMP推送Session 返回推送句柄
        /// </summary>
        /// <returns></returns>
        [DllImport(EasyRTMP_LIB, EntryPoint = "EasyRTMP_Create")]
        private extern static IntPtr EasyRTMP_Create();

        /// <summary>
        /// 设置数据回调
        /// </summary>
        /// <param name="handle"> rtmp 句柄 </param>
        /// <param name="callBack"> 回调委托 </param>
        /// <param name="userptr"> 用户自定义数据 </param>
        /// <returns></returns>
        [DllImport(EasyRTMP_LIB, EntryPoint = "EasyRTMP_SetCallback")]
        private extern static int EasyRTMP_SetCallback(IntPtr handle, EasyRTMPCallBack callBack, IntPtr userptr);

        /// <summary>
        /// 创建RTMP推送的参数信息
        /// </summary>
        /// <param name="handle"> rtmp 句柄 </param>
        /// <param name="pstruStreamInfo"> 流媒体信息 </param>
        /// <param name="bufferKSize"> 缓冲区大小 </param>
        /// <returns></returns>
        [DllImport(EasyRTMP_LIB, EntryPoint = "EasyRTMP_InitMetadata")]
        private extern static int EasyRTMP_InitMetadata(IntPtr handle, EASY_MEDIA_INFO_T pstruStreamInfo, int bufferKSize);

        /// <summary>
        /// 连接RTMP服务器
        /// </summary>
        /// <param name="handle"> rtmp 句柄 </param>
        /// <param name="url"> rtmp url </param>
        /// <returns></returns>
        [DllImport(EasyRTMP_LIB, EntryPoint = "EasyRTMP_Connect")]
        private extern static bool EasyRTMP_Connect(IntPtr handle, string url);


        /// <summary>
        /// 推送H264或AAC流
        /// </summary>
        /// <param name="handle"> rtmp 句柄 </param>
        /// <param name="frame"> 侦信息 </param>
        /// <returns></returns>
        [DllImport(EasyRTMP_LIB, EntryPoint = "EasyRTMP_SendPacket")]
        private extern static int EasyRTMP_SendPacket(IntPtr handle, EASY_AV_Frame frame);

        /// <summary>
        /// 获取缓冲区大小
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="usedSize"></param>
        /// <param name="totalSize"></param>
        /// <returns></returns>
        [DllImport(EasyRTMP_LIB, EntryPoint = "EasyRTMP_GetBufInfo")]
        private extern static int EasyRTMP_GetBufInfo(IntPtr handle, out int usedSize, out int totalSize);

        /// <summary>
        /// 停止RTMP推送，释放句柄
        /// </summary>
        /// <param name="handle"> rtmp 句柄 </param>
        [DllImport(EasyRTMP_LIB, EntryPoint = "EasyRTMP_Release")]
        private extern static void EasyRTMP_Release(IntPtr handle);

        /// <summary>
        /// 推送事件类型定义
        /// </summary>
        public enum EASY_RTMP_STATE_T
        {
            /// <summary>
            /// 连接中
            /// </summary>
            EASY_RTMP_STATE_CONNECTING = 1,
            /// <summary>
            /// 连接成功
            /// </summary>
            EASY_RTMP_STATE_CONNECTED,
            /// <summary>
            /// 连接失败
            /// </summary>
            EASY_RTMP_STATE_CONNECT_FAILED,
            /// <summary>
            /// 连接异常中断
            /// </summary>
            EASY_RTMP_STATE_CONNECT_ABORT,
            /// <summary>
            /// 推流中
            /// </summary>
            EASY_RTMP_STATE_PUSHING,
            /// <summary>
            /// 断开连接
            /// </summary>
            EASY_RTMP_STATE_DISCONNECTED,
            /// <summary>
            /// 错误
            /// </summary>
            EASY_RTMP_STATE_ERROR
        }

        /// <summary>
        /// 媒体信息
        /// </summary>
        public struct EASY_MEDIA_INFO_T
        {
            public System.UInt32 u32VideoCodec;				/* 视频编码类型 */
            public System.UInt32 u32VideoFps;				/* 视频帧率 */
            public System.UInt32 u32AudioCodec;				/* 音频编码类型 */
            public System.UInt32 u32AudioSamplerate;		/* 音频采样率 */
            public System.UInt32 u32AudioChannel;			/* 音频通道数 */
            public System.UInt32 u32AudioBitsPerSample;		/* 音频采样精度 */
            public System.UInt32 u32VpsLength;
            public System.UInt32 u32SpsLength;
            public System.UInt32 u32PpsLength;
            public System.UInt32 u32SeiLength;
            //char	 u8Vps[256];
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public System.Byte[] u8Vps;
            //char	 u8Sps[256];
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public System.Byte[] u8Sps;
            //char	 u8Pps[128];
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public System.Byte[] u8Pps;
            //char	 u8Sei[128];
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public System.Byte[] u8Sei;

            public static EASY_MEDIA_INFO_T Parse(IntPtr ptr)
            {
                return (EASY_MEDIA_INFO_T)Marshal.PtrToStructure(ptr, typeof(EASY_MEDIA_INFO_T));
            }
        }

        /// <summary>
        /// 视频/音频侦信息
        /// </summary>
        public struct EASY_AV_Frame
        {
            /// <summary>
            /// 帧标志  视频 or 音频
            /// </summary>
            public System.UInt32 u32AVFrameFlag;

            /// <summary>
            /// 帧的长度
            /// </summary>
            public System.UInt32 u32AVFrameLen;

            /// <summary>
            /// 视频的类型，I帧或P帧
            /// </summary>
            public System.UInt32 u32VFrameType;

            /// <summary>
            /// 数据
            /// </summary>
            public IntPtr pBuffer;

            /// <summary>
            /// 时间戳(秒)
            /// </summary>
            public System.UInt32 u32TimestampSec;

            /// <summary>
            /// 时间戳(微秒) 
            /// </summary>
            public System.UInt32 u32TimestampUsec;
        }

        #endregion

        /// <summary>
        /// 获取激活状态
        /// </summary>
        /// <value></value>
        public static bool Activated { get; private set; }

        /// <summary>
        /// RTMP 句柄
        /// </summary>
        /// <value></value>
        public IntPtr RtmpHandle { get; private set; }

        /// <summary>
        /// 获取是否初始化
        /// </summary>
        /// <value></value>
        public bool IsInit
        {
            get { return this.RtmpHandle != IntPtr.Zero; }
        }

        /// <summary>
        /// RTMP 激活
        /// </summary>
        /// <returns></returns>
        public static KeyValuePair<bool, string> Activate()
        {
            if(Activated)
            {
                return new KeyValuePair<bool, string>(true, "RTSP 已激活!");
            }
            try
            {
                int ret = EasyRTMP_Activate(EasyRTMP_KEY);
                if(ret > 0)
                {
                    Activated = true;
                    Console.WriteLine("RTMP 激活成功!");
                    return new KeyValuePair<bool, string>(true, "RTMP 激活成功!");
                }
                else
                {
                    Activated = false;
                    Console.WriteLine(string.Format("RTMP 激活失败，错误代码：{0}", ret));
                    return new KeyValuePair<bool, string>(false, string.Format("RTMP 激活失败，错误代码：{0}", ret));
                }
            }
            catch (System.Exception e)
            {
                return new KeyValuePair<bool, string>(false, e.Message);
            }
        }

        /// <summary>
        /// 回调委托
        /// </summary>
        /// <value></value>
        public EasyRTMPCallBack RTMPCallBack { get; private set; }

        /// <summary>
        /// 创建RTMP推送Session
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<bool, string> Create()
        {
            //rtmp 释放
            this.Release();
            try
            {
                IntPtr ret = EasyRTMP_Create();
                if(ret == IntPtr.Zero)
                {
                    this.RtmpHandle = IntPtr.Zero;
                    return new KeyValuePair<bool, string>(false, "RTMP 创建失败!");
                }
                else
                {
                    this.RtmpHandle = ret;
                    return new KeyValuePair<bool, string>(true, "RTMP 创建成功!");
                }
            }
            catch (System.Exception e)
            {
                return new KeyValuePair<bool, string>(false, e.Message);
            }
        }

        /// <summary>
        /// 设置回调委托
        /// </summary>
        /// <param name="callBack"> 回调委托 </param>
        /// <param name="userptr"> 用户自定义数据 </param>
        /// <returns></returns>
        public KeyValuePair<bool, string> SetCallback(EasyRTMPCallBack callBack, IntPtr userptr)
        {
            if(!this.IsInit)
            {
                return new KeyValuePair<bool, string>(false, "RTMP 未创建!");
            }
            try
            {
                int ret = EasyRTMP_SetCallback(this.RtmpHandle, callBack, userptr);
                if(ret == 0)
                {
                    this.RTMPCallBack = callBack;
                    return new KeyValuePair<bool, string>(true, "RTMP 设置回调成功!");
                }
                else
                {
                    return new KeyValuePair<bool, string>(false, string.Format("RTMP 设置回调失败，错误代码：{0}", ret));
                }
            }
            catch (System.Exception e)
            {
                return new KeyValuePair<bool, string>(false, e.Message);
            }
        }

        /// <summary>
        /// 创建RTMP推送的参数信息
        /// </summary>
        /// <param name="mediaInfo"> 流媒体信息 </param>
        /// <param name="bufferKSize"> 缓冲区大小 </param>
        /// <returns></returns>
        public KeyValuePair<bool, string> InitMetadata(EASY_MEDIA_INFO_T mediaInfo, int bufferKSize = 1024)
        {
            if(!this.IsInit)
            {
                return new KeyValuePair<bool, string>(false, "RTMP 未创建!");
            }
            try
            {
                int ret = EasyRTMP_InitMetadata(this.RtmpHandle, mediaInfo, bufferKSize);
                if(ret == 0)
                {
                    return new KeyValuePair<bool, string>(true, "RTMP 创建媒体成功!");
                }
                else
                {
                    return new KeyValuePair<bool, string>(false, string.Format("RTMP 创建媒体失败，错误代码：{0}", ret));
                }
            }
            catch (System.Exception e)
            {
                return new KeyValuePair<bool, string>(false, e.Message);
            }
        }

        /// <summary>
        /// 连接RTMP服务器
        /// </summary>
        /// <param name="url"> rtmp 地址 </param>
        /// <returns></returns>
        public KeyValuePair<bool, string> Connect(string url)
        {
            if(!this.IsInit)
            {
                return new KeyValuePair<bool, string>(false, "RTMP 未创建!");
            }
            try
            {
                var ret = EasyRTMP_Connect(this.RtmpHandle, url);
                if(ret)
                {
                    return new KeyValuePair<bool, string>(true, "RTMP 创建连接成功!");
                }
                else
                {
                    return new KeyValuePair<bool, string>(false, string.Format("RTMP 创建连接失败，错误代码：{0}", ret));
                }
            }
            catch (System.Exception e)
            {
                return new KeyValuePair<bool, string>(false, e.Message);
            }
        }

        /// <summary>
        /// 获取缓冲区大小
        /// </summary>
        /// <param name="usedSize"> 已使用缓冲区大小 </param>
        /// <param name="totalSize"> 总共缓冲区大小 </param>
        /// <returns></returns>
        public KeyValuePair<bool, string> GetBufInfo(out int usedSize, out int totalSize)
        {
            if(!this.IsInit)
            {
                usedSize = 0;
                totalSize = 0;
                return new KeyValuePair<bool, string>(false, "RTMP 未创建!");
            }
            try
            {
                int ret = EasyRTMP_GetBufInfo(this.RtmpHandle, out usedSize, out totalSize);
                if(ret == 0)
                {
                    return new KeyValuePair<bool, string>(true, "");
                }
                else
                {
                    return new KeyValuePair<bool, string>(false, "");
                }
            }
            catch (System.Exception e)
            {
                usedSize = 0;
                totalSize = 0;
                return new KeyValuePair<bool, string>(false, e.Message);
            }
        }

        /// <summary>
        /// 停止RTMP推送，释放句柄
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<bool, string> Release()
        {
            if(!this.IsInit)
            {
                return new KeyValuePair<bool, string>(false, "RTMP 未创建!");
            }
            try
            {
                EasyRTMP_Release(this.RtmpHandle);

                this.RtmpHandle = IntPtr.Zero;

                return new KeyValuePair<bool, string>(true, "RTMP 释放成功!");
            }
            catch (System.Exception e)
            {
                return new KeyValuePair<bool, string>(false, e.Message);
            }
        }

        /// <summary>
        /// 推送H264或AAC流
        /// </summary>
        /// <param name="frame"> 帧信息 </param>
        /// <returns></returns>
        public KeyValuePair<bool, string> SendPacket(EASY_AV_Frame frame)
        {
            if(!this.IsInit)
            {
                return new KeyValuePair<bool, string>(false, "RTMP 未创建!");
            }
            try
            {
                int ret = EasyRTMP_SendPacket(this.RtmpHandle, frame);
                if(ret != 0)
                {
                    return new KeyValuePair<bool, string>(false, string.Format("RTMP 发送数据包失败,错误代码：{0}", ret));
                }
                else 
                {
                    return new KeyValuePair<bool, string>(true, "发送流媒体数据包成功!");
                }
            }
            catch (System.Exception e)
            {
                return new KeyValuePair<bool, string>(false, string.Format("RTMP 发送数据包失败,异常消息：{0}", e.Message));
            }
        }
    }
}