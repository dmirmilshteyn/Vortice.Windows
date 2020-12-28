﻿// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using SharpGen.Runtime;
using SharpGen.Runtime.Win32;
using Vortice.DXGI;

namespace Vortice.Direct3D11
{
    public partial class ID3D11VideoDevice
    {
        public Guid CheckCryptoKeyExchange(Guid cryptoType, Guid? decoderProfile, int index)
        {
            CheckCryptoKeyExchange(cryptoType, decoderProfile, index, out Guid keyExchangeType).CheckError();
            return keyExchangeType;
        }

        public bool CheckVideoDecoderFormat(Guid decoderProfile, Format format)
        {
            CheckVideoDecoderFormat(decoderProfile, format, out RawBool supported).CheckError();
            return supported;
        }

        public ID3D11AuthenticatedChannel CreateAuthenticatedChannel(AuthenticatedChannelType channelType)
        {
            CreateAuthenticatedChannel(channelType, out ID3D11AuthenticatedChannel authenticatedChannel).CheckError();
            return authenticatedChannel;
        }

        public ID3D11CryptoSession CreateCryptoSession(Guid cryptoType, Guid? decoderProfile, Guid keyExchangeType)
        {
            CreateCryptoSession(cryptoType, decoderProfile, keyExchangeType, out ID3D11CryptoSession cryptoSession).CheckError();
            return cryptoSession;
        }

        public ID3D11VideoDecoder CreateVideoDecoder(VideoDecoderDescription description, VideoDecoderConfig config)
        {
            CreateVideoDecoder(ref description, ref config, out ID3D11VideoDecoder decoder).CheckError();
            return decoder;
        }

        public Result CreateVideoDecoder(VideoDecoderDescription description, VideoDecoderConfig config, out ID3D11VideoDecoder decoder)
        {
            return CreateVideoDecoder(ref description, ref config, out decoder);
        }

        public ID3D11VideoDecoderOutputView CreateVideoDecoderOutputView(ID3D11Resource resource, VideoDecoderOutputViewDescription description)
        {
            CreateVideoDecoderOutputView(resource, ref description, out ID3D11VideoDecoderOutputView view).CheckError();
            return view;
        }

        public Result CreateVideoDecoderOutputView(ID3D11Resource resource, VideoDecoderOutputViewDescription description, out ID3D11VideoDecoderOutputView view)
        {
            return CreateVideoDecoderOutputView(resource, ref description, out view);
        }

        public ID3D11VideoProcessor CreateVideoProcessor(ID3D11VideoProcessorEnumerator enumerator, int rateConversionIndex)
        {
            CreateVideoProcessor(enumerator, rateConversionIndex, out ID3D11VideoProcessor videoProcessor).CheckError();
            return videoProcessor;
        }

        public ID3D11VideoProcessorEnumerator CreateVideoProcessorEnumerator(VideoProcessorContentDescription description)
        {
            CreateVideoProcessorEnumerator(ref description, out ID3D11VideoProcessorEnumerator enumerator).CheckError();
            return enumerator;
        }

        public Result CreateVideoProcessorEnumerator(VideoProcessorContentDescription description, out ID3D11VideoProcessorEnumerator enumerator)
        {
            return CreateVideoProcessorEnumerator(ref description, out enumerator);
        }

        public ID3D11VideoProcessorInputView CreateVideoProcessorInputView(ID3D11Resource resource, ID3D11VideoProcessorEnumerator enumerator, VideoProcessorInputViewDescription description)
        {
            CreateVideoProcessorInputView(resource, enumerator, description, out ID3D11VideoProcessorInputView view).CheckError();
            return view;
        }

        public ID3D11VideoProcessorOutputView CreateVideoProcessorOutputView(ID3D11Resource resource, ID3D11VideoProcessorEnumerator enumerator, VideoProcessorOutputViewDescription description)
        {
            CreateVideoProcessorOutputView(resource, enumerator, description, out ID3D11VideoProcessorOutputView view).CheckError();
            return view;
        }

        public VideoContentProtectionCaps GetContentProtectionCaps(Guid? cryptoType, Guid? decoderProfile)
        {
            GetContentProtectionCaps(cryptoType, decoderProfile, out VideoContentProtectionCaps caps).CheckError();
            return caps;
        }

        public VideoDecoderConfig GetVideoDecoderConfig(VideoDecoderDescription description, int index)
        {
            GetVideoDecoderConfig(ref description, index, out VideoDecoderConfig config).CheckError();
            return config;
        }

        public Result GetVideoDecoderConfig(VideoDecoderDescription description, int index, out VideoDecoderConfig config)
        {
            return GetVideoDecoderConfig(ref description, index, out config);
        }

        public int GetVideoDecoderConfigCount(VideoDecoderDescription description)
        {
            GetVideoDecoderConfigCount(ref description, out int count).CheckError();
            return count;
        }

        public Result GetVideoDecoderConfigCount(VideoDecoderDescription description, out int count)
        {
            return GetVideoDecoderConfigCount(ref description, out count);
        }

        public Guid GetVideoDecoderProfile(int index)
        {
            GetVideoDecoderProfile(index, out Guid decoderProfile).CheckError();
            return decoderProfile;
        }
    }
}
