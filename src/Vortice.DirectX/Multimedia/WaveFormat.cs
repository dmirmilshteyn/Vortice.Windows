// Copyright � Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Vortice.Multimedia;

/// <summary>
/// Represents a Wave file format
/// </summary>
public partial class WaveFormat
{
    /// <summary>
    /// The format type
    /// </summary>
    protected WaveFormatEncoding waveFormatTag;

    /// <summary>
    /// The number of channels
    /// </summary>
    protected short channels;
    /// <summary>sample rate</summary>
    protected int sampleRate;
    /// <summary>for buffer estimation</summary>
    protected int averageBytesPerSecond;
    /// <summary>block size of data</summary>
    protected short blockAlign;
    /// <summary>number of bits per sample of mono data</summary>
    protected short bitsPerSample;
    /// <summary>number of following bytes</summary>
    protected short extraSize;

    /// <summary>
    /// Returns the encoding type used
    /// </summary>
    public WaveFormatEncoding Encoding
    {
        get
        {
            return waveFormatTag;
        }
    }

    /// <summary>
    /// Returns the number of channels (1=mono,2=stereo etc)
    /// </summary>
    public int Channels
    {
        get
        {
            return channels;
        }
    }

    /// <summary>
    /// Returns the sample rate (samples per second)
    /// </summary>
    public int SampleRate
    {
        get
        {
            return sampleRate;
        }
    }

    /// <summary>
    /// Returns the average number of bytes used per second
    /// </summary>
    public int AverageBytesPerSecond
    {
        get
        {
            return averageBytesPerSecond;
        }
    }

    /// <summary>
    /// Returns the block alignment
    /// </summary>
    public int BlockAlign
    {
        get
        {
            return blockAlign;
        }
    }

    /// <summary>
    /// Returns the number of bits per sample (usually 16 or 32, sometimes 24 or 8)
    /// Can be 0 for some codecs
    /// </summary>
    public int BitsPerSample
    {
        get
        {
            return bitsPerSample;
        }
    }

    /// <summary>
    /// Returns the number of extra bytes used by this waveformat. Often 0,
    /// except for compressed formats which store extra data after the WAVEFORMATEX header
    /// </summary>
    public int ExtraSize
    {
        get
        {
            return extraSize;
        }
    }

    /// <summary>
    /// Creates a new PCM 44.1Khz stereo 16 bit format
    /// </summary>
    public WaveFormat() : this(44100, 16, 2)
    {

    }


    /// <summary>
    /// Creates a new 16 bit wave format with the specified sample
    /// rate and channel count
    /// </summary>
    /// <param name="sampleRate">Sample Rate</param>
    /// <param name="channels">Number of channels</param>
    public WaveFormat(int sampleRate, int channels)
        : this(sampleRate, 16, channels)
    {
    }

    /// <summary>
    /// Gets the size of a wave buffer equivalent to the latency in milliseconds.
    /// </summary>
    /// <param name="milliseconds">The milliseconds.</param>
    /// <returns></returns>
    public int ConvertLatencyToByteSize(int milliseconds)
    {
        int bytes = (int)((AverageBytesPerSecond / 1000.0) * milliseconds);
        if ((bytes % BlockAlign) != 0)
        {
            // Return the upper BlockAligned
            bytes = bytes + BlockAlign - (bytes % BlockAlign);
        }
        return bytes;
    }

    /// <summary>
    /// Creates a WaveFormat with custom members
    /// </summary>
    /// <param name="tag">The encoding</param>
    /// <param name="sampleRate">Sample Rate</param>
    /// <param name="channels">Number of channels</param>
    /// <param name="averageBytesPerSecond">Average Bytes Per Second</param>
    /// <param name="blockAlign">Block Align</param>
    /// <param name="bitsPerSample">Bits Per Sample</param>
    /// <returns></returns>
    public static WaveFormat CreateCustomFormat(
        WaveFormatEncoding tag,
        int sampleRate,
        int channels,
        int averageBytesPerSecond,
        int blockAlign,
        int bitsPerSample)
    {
        var waveFormat = new WaveFormat
        {
            waveFormatTag = tag,
            channels = (short)channels,
            sampleRate = sampleRate,
            averageBytesPerSecond = averageBytesPerSecond,
            blockAlign = (short)blockAlign,
            bitsPerSample = (short)bitsPerSample,
            extraSize = 0
        };
        return waveFormat;
    }

    /// <summary>
    /// Creates an A-law wave format
    /// </summary>
    /// <param name="sampleRate">Sample Rate</param>
    /// <param name="channels">Number of Channels</param>
    /// <returns>Wave Format</returns>
    public static WaveFormat CreateALawFormat(int sampleRate, int channels)
    {
        return CreateCustomFormat(WaveFormatEncoding.ALaw, sampleRate, channels, sampleRate * channels, channels, 8);
    }

    /// <summary>
    /// Creates a Mu-law wave format
    /// </summary>
    /// <param name="sampleRate">Sample Rate</param>
    /// <param name="channels">Number of Channels</param>
    /// <returns>Wave Format</returns>
    public static WaveFormat CreateMuLawFormat(int sampleRate, int channels)
    {
        return CreateCustomFormat(WaveFormatEncoding.MuLaw, sampleRate, channels, sampleRate * channels, channels, 8);
    }

    /// <summary>
    /// Creates a new PCM format with the specified sample rate, bit depth and channels
    /// </summary>
    public WaveFormat(int rate, int bits, int channels)
    {
        if (channels < 1)
        {
            throw new ArgumentOutOfRangeException("channels", "Channels must be 1 or greater");
        }

        // minimum 16 bytes, sometimes 18 for PCM
        waveFormatTag = bits < 32 ? WaveFormatEncoding.Pcm : WaveFormatEncoding.IeeeFloat;
        this.channels = (short)channels;
        sampleRate = rate;
        bitsPerSample = (short)bits;
        extraSize = 0;

        blockAlign = (short)(channels * (bits / 8));
        averageBytesPerSecond = sampleRate * blockAlign;
    }

    /// <summary>
    /// Creates a new 32 bit IEEE floating point wave format
    /// </summary>
    /// <param name="sampleRate">sample rate</param>
    /// <param name="channels">number of channels</param>
    public static WaveFormat CreateIeeeFloatWaveFormat(int sampleRate, int channels)
    {
        WaveFormat format = new()
        {
            waveFormatTag = WaveFormatEncoding.IeeeFloat,
            channels = (short)channels,
            bitsPerSample = 32,
            sampleRate = sampleRate,
            blockAlign = (short)(4 * channels),
            averageBytesPerSecond = sampleRate * channels * 4,
            extraSize = 0,
        };

        return format;
    }

    /// <summary>
    /// Helper function to retrieve a WaveFormat structure from a pointer
    /// </summary>
    /// <param name="rawData">Buffer to the WaveFormat rawdata</param>
    /// <returns>WaveFormat structure</returns>
    public unsafe static WaveFormat MarshalFrom(byte[] rawData)
    {
        fixed (void* pRawData = rawData)
        {
            return MarshalFrom(pRawData);
        }
    }

    /// <summary>
    /// Helper function to retrieve a WaveFormat structure from a pointer
    /// </summary>
    /// <param name="pointer">Pointer to the WaveFormat rawdata</param>
    /// <returns>WaveFormat structure</returns>
    public unsafe static WaveFormat MarshalFrom(IntPtr pointer) => MarshalFrom(pointer.ToPointer());

    /// <summary>
    /// Helper function to retrieve a WaveFormat structure from a pointer
    /// </summary>
    /// <param name="pointer">Pointer to the WaveFormat rawdata</param>
    /// <returns>WaveFormat structure</returns>
    public unsafe static WaveFormat MarshalFrom(void* pointer)
    {
        if (pointer == null)
        {
            throw new InvalidOperationException("Pointer cannot be null");
        }

        var pcmWaveFormat = *(__PcmNative*)pointer;
        var encoding = pcmWaveFormat.waveFormatTag;

        // Load simple PcmWaveFormat if channels <= 2 and encoding is Pcm, IeeFloat, Wmaudio2, Wmaudio3
        // See http://msdn.microsoft.com/en-us/library/microsoft.directx_sdk.xaudio2.waveformatex%28v=vs.85%29.aspx
        if (pcmWaveFormat.channels <= 2 &&
            (encoding == WaveFormatEncoding.Pcm || encoding == WaveFormatEncoding.IeeeFloat || encoding == WaveFormatEncoding.WindowsMediaAudio || encoding == WaveFormatEncoding.WindowsMediaAudioProfessional))
        {
            var waveFormat = new WaveFormat();
            waveFormat.__MarshalFrom(ref pcmWaveFormat);
            return waveFormat;
        }

        if (encoding == WaveFormatEncoding.Extensible)
        {
            var waveFormat = new WaveFormatExtensible();
            waveFormat.__MarshalFrom(ref *(WaveFormatExtensible.__Native*)pointer);
            return waveFormat;
        }

        if (encoding == WaveFormatEncoding.Adpcm)
        {
            var waveFormat = new WaveFormatAdpcm();
            waveFormat.__MarshalFrom(ref *(WaveFormatAdpcm.__Native*)pointer);
            return waveFormat;
        }

        throw new InvalidOperationException(string.Format("Unsupported WaveFormat [{0}]", encoding));
    }

    protected unsafe virtual IntPtr MarshalToPtr()
    {
        IntPtr result = Marshal.AllocHGlobal(Unsafe.SizeOf<__Native>());
        __MarshalTo(ref *(__Native*)result);
        return result;
    }

    /// <summary>
    /// Helper function to marshal WaveFormat to an IntPtr
    /// </summary>
    /// <param name="format">WaveFormat</param>
    /// <returns>IntPtr to WaveFormat structure (needs to be freed by callee)</returns>
    public static IntPtr MarshalToPtr(WaveFormat? format)
    {
        if (format == null)
            return IntPtr.Zero;
        return format.MarshalToPtr();
    }

    /// <summary>
    /// Reads a new WaveFormat object from a stream
    /// </summary>
    /// <param name="reader">A binary reader that wraps the stream</param>
    public WaveFormat(BinaryReader reader)
    {
        int formatChunkLength = reader.ReadInt32();
        ReadWaveFormat(reader, formatChunkLength);
    }

    private void ReadWaveFormat(BinaryReader br, int formatChunkLength)
    {
        if (formatChunkLength < 16)
            throw new InvalidDataException("Invalid WaveFormat Structure");
        waveFormatTag = (WaveFormatEncoding)br.ReadUInt16();
        channels = br.ReadInt16();
        sampleRate = br.ReadInt32();
        averageBytesPerSecond = br.ReadInt32();
        blockAlign = br.ReadInt16();
        bitsPerSample = br.ReadInt16();
        if (formatChunkLength > 16)
        {
            extraSize = br.ReadInt16();
            if (extraSize != formatChunkLength - 18)
            {
                Debug.WriteLine("Format chunk mismatch");
                extraSize = (short)(formatChunkLength - 18);
            }
        }
    }


    /// <summary>
    /// Reports this WaveFormat as a string
    /// </summary>
    /// <returns>String describing the wave format</returns>
    public override string ToString()
    {
        switch (waveFormatTag)
        {
            case WaveFormatEncoding.Pcm:
            case WaveFormatEncoding.Extensible:
                // extensible just has some extra bits after the PCM header
                return string.Format(
                    CultureInfo.InvariantCulture,
                    "{0} bit PCM: {1}kHz {2} channels",
                    bitsPerSample,
                    sampleRate / 1000,
                    channels);

            default:
                return waveFormatTag.ToString();
        }
    }

    /// <summary>
    /// Compares with another WaveFormat object
    /// </summary>
    /// <param name="obj">Object to compare to</param>
    /// <returns>True if the objects are the same</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not WaveFormat)
            return false;

        WaveFormat other = (WaveFormat)obj;
        return waveFormatTag == other.waveFormatTag &&
            channels == other.channels &&
            sampleRate == other.sampleRate &&
            averageBytesPerSecond == other.averageBytesPerSecond &&
            blockAlign == other.blockAlign &&
            bitsPerSample == other.bitsPerSample;
    }

    /// <summary>
    /// Provides a hash code for this WaveFormat
    /// </summary>
    /// <returns>A hash code</returns>
    public override int GetHashCode()
    {
        return (int)waveFormatTag ^
            (int)channels ^
            sampleRate ^
            averageBytesPerSecond ^
            (int)blockAlign ^
            (int)bitsPerSample;
    }

    #region Marshal 
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal struct __Native
    {
        public __PcmNative pcmWaveFormat;
        /// <summary>number of following bytes</summary>
        public short extraSize;
        // Method to free native struct
        internal unsafe void __MarshalFree()
        {
        }
    }

    internal unsafe void __MarshalFree(ref __Native @ref)
    {
        @ref.__MarshalFree();
    }
    internal unsafe void __MarshalFrom(ref __Native @ref)
    {
        waveFormatTag = @ref.pcmWaveFormat.waveFormatTag;
        channels = @ref.pcmWaveFormat.channels;
        sampleRate = @ref.pcmWaveFormat.sampleRate;
        averageBytesPerSecond = @ref.pcmWaveFormat.averageBytesPerSecond;
        blockAlign = @ref.pcmWaveFormat.blockAlign;
        bitsPerSample = @ref.pcmWaveFormat.bitsPerSample;
        extraSize = @ref.extraSize;
    }
    internal unsafe void __MarshalTo(ref __Native @ref)
    {
        @ref.pcmWaveFormat.waveFormatTag = waveFormatTag;
        @ref.pcmWaveFormat.channels = channels;
        @ref.pcmWaveFormat.sampleRate = sampleRate;
        @ref.pcmWaveFormat.averageBytesPerSecond = averageBytesPerSecond;
        @ref.pcmWaveFormat.blockAlign = blockAlign;
        @ref.pcmWaveFormat.bitsPerSample = bitsPerSample;
        @ref.extraSize = extraSize;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal struct __PcmNative
    {
        /// <summary>format type</summary>
        public WaveFormatEncoding waveFormatTag;
        /// <summary>number of channels</summary>
        public short channels;
        /// <summary>sample rate</summary>
        public int sampleRate;
        /// <summary>for buffer estimation</summary>
        public int averageBytesPerSecond;
        /// <summary>block size of data</summary>
        public short blockAlign;
        /// <summary>number of bits per sample of mono data</summary>
        public short bitsPerSample;

        // Method to free native struct
        internal unsafe void __MarshalFree()
        {
        }
    }
    internal unsafe void __MarshalFree(ref __PcmNative @ref)
    {
        @ref.__MarshalFree();
    }

    internal unsafe void __MarshalFrom(ref __PcmNative @ref)
    {
        waveFormatTag = @ref.waveFormatTag;
        channels = @ref.channels;
        sampleRate = @ref.sampleRate;
        averageBytesPerSecond = @ref.averageBytesPerSecond;
        blockAlign = @ref.blockAlign;
        bitsPerSample = @ref.bitsPerSample;
        extraSize = 0;
    }
    internal unsafe void __MarshalTo(ref __PcmNative @ref)
    {
        @ref.waveFormatTag = waveFormatTag;
        @ref.channels = channels;
        @ref.sampleRate = sampleRate;
        @ref.averageBytesPerSecond = averageBytesPerSecond;
        @ref.blockAlign = blockAlign;
        @ref.bitsPerSample = bitsPerSample;
    }
    #endregion
}
