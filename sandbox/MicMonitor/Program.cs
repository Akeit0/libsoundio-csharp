using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LibSoundIo.Interop;
using static LibSoundIo.Interop.SoundIo;

var app = new MicMonitorApp();
app.Run();

public unsafe class MicMonitorApp
{
    SoundIo* soundio = null;
    SoundIoInStream* instream = null;
    SoundIoOutStream* outstream = null;
    SoundIoDevice* inputDevice = null;
    SoundIoDevice* outputDevice = null;

    GCHandle gcHandle;

    SoundIoRingBuffer* ring_buffer;

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static void OnReadCallback(SoundIoInStream* instream, int frame_count_min, int frame_count_max)
    {
        var rc = (MicMonitorApp)GCHandle.FromIntPtr(instream->userdata).Target!;
        var write_ptr = soundio_ring_buffer_write_ptr(rc.ring_buffer);
        int free_bytes = soundio_ring_buffer_free_count(rc.ring_buffer);
        int free_count = free_bytes / instream->bytes_per_frame;

        if (frame_count_min > free_count)
        {
            Console.WriteLine("ring buffer overflow");
            //panic("ring buffer overflow");
        }

        int write_frames = Math.Min(free_count, frame_count_max);
        int frames_left = write_frames;

        for (;;)
        {
            int frame_count = frames_left;

            SoundIoError err;
            if ((err = soundio_instream_begin_read(instream, out var areas, ref frame_count)) != 0)
            {
                panic("begin read error: %s", soundio_strerror(err));
            }

            if (frame_count == 0)
                break;

            if (areas == null)
            {
                // Due to an overflow there is a hole. Fill the ring buffer with
                // silence for the size of the hole.
                new Span<byte>(write_ptr, frame_count * instream->bytes_per_frame).Clear();

                Console.Error.WriteLine($"Dropped {frame_count} frames due to internal overflow");
            }
            else
            {
                for (int frame = 0; frame < frame_count; frame += 1)
                {
                    for (int ch = 0; ch < instream->layout.channel_count; ch += 1)
                    {
                        var writeValue = *((float*)areas[ch].ptr);
                        *((float*)write_ptr) = writeValue;
                        areas[ch].ptr += areas[ch].step;
                        write_ptr += instream->bytes_per_sample;
                    }
                }
            }

            if ((err = soundio_instream_end_read(instream)) != 0)
                Console.Error.WriteLine($"end read error: {err}");

            frames_left -= frame_count;
            if (frames_left <= 0)
                break;
        }

        int advance_bytes = write_frames * instream->bytes_per_frame;
        soundio_ring_buffer_advance_write_ptr(rc.ring_buffer, advance_bytes);
    }

    static float seconds_offset;

    static void panic(string msg, IntPtr m)
    {
        Console.Error.WriteLine(msg);
    }

    static void memset(void* ptr, int size, int bytes)
    {
        new Span<bool>(ptr, bytes).Clear();
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static void underflow_callback(SoundIoOutStream* stream)
    {
        Console.Error.WriteLine("underflow");
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static void OnWriteCallback(SoundIoOutStream* outstream, int frame_count_min, int frame_count_max)
    {
        var rc = (MicMonitorApp)GCHandle.FromIntPtr(outstream->userdata).Target!;
        var ring_buffer = rc.ring_buffer;
        SoundIoChannelArea* areas;
        int frames_left;
        int frame_count;
        SoundIoError err;

        var read_ptr = soundio_ring_buffer_read_ptr(ring_buffer);
        int fill_bytes = soundio_ring_buffer_fill_count(ring_buffer);
        int fill_count = fill_bytes / outstream->bytes_per_frame;

        if (frame_count_min > fill_count)
        {
            // Ring buffer does not have enough data, fill with zeroes.
            frames_left = frame_count_min;
            for (;;)
            {
                frame_count = frames_left;
                if (frame_count <= 0)
                    return;
                if ((err = soundio_outstream_begin_write(outstream, out areas, ref frame_count)) != 0)
                    panic("begin write error: %s", soundio_strerror(err));
                if (frame_count <= 0)
                    return;
                for (int frame = 0; frame < frame_count; frame += 1)
                {
                    for (int ch = 0; ch < outstream->layout.channel_count; ch += 1)
                    {
                        memset(areas[ch].ptr, 0, outstream->bytes_per_sample);
                        areas[ch].ptr += areas[ch].step;
                    }
                }

                if ((err = soundio_outstream_end_write(outstream)) != 0)
                    panic("end write error: %s", soundio_strerror(err));
                frames_left -= frame_count;
            }
        }

        int read_count = Math.Min(frame_count_max, fill_count);
        frames_left = read_count;

        while (frames_left > 0)
        {
            frame_count = frames_left;

            if ((err = soundio_outstream_begin_write(outstream, out areas, ref frame_count)) != 0)
                panic("begin write error: %s", soundio_strerror(err));

            if (frame_count <= 0)
                break;

            for (int frame = 0; frame < frame_count; frame += 1)
            {
                for (int ch = 0; ch < outstream->layout.channel_count; ch += 1)
                {
                    *((float*)(areas[ch].ptr)) = *(float*)read_ptr;
                    areas[ch].ptr += areas[ch].step;
                    read_ptr += outstream->bytes_per_sample;
                }
            }

            if ((err = soundio_outstream_end_write(outstream)) != 0)
                panic("end write error: %s", soundio_strerror(err));

            frames_left -= frame_count;
        }

        soundio_ring_buffer_advance_read_ptr(ring_buffer, read_count * outstream->bytes_per_frame);
    }

    void ListDevices()
    {
        Console.WriteLine("\n=== Input Devices ===");
        int inputCount = soundio_input_device_count(soundio);
        int defaultInputIndex = soundio_default_input_device_index(soundio);

        for (int i = 0; i < inputCount; i++)
        {
            SoundIoDevice* devicePtr = soundio_get_input_device(soundio, i);
            SoundIoDevice device = *devicePtr;
            string name = Marshal.PtrToStringUTF8(device.name)!;
            string marker = (i == defaultInputIndex) ? " (default)" : "";
            var rawMarker = device.is_raw ? "(raw)" : "";
            Console.WriteLine($"{i}: {name}{marker}{rawMarker} {device.current_format} {device.sample_rate_current}");
            soundio_device_unref(devicePtr);
        }

        Console.WriteLine("\n=== Output Devices ===");
        int outputCount = soundio_output_device_count(soundio);
        int defaultOutputIndex = soundio_default_output_device_index(soundio);

        for (int i = 0; i < outputCount; i++)
        {
            SoundIoDevice* devicePtr = soundio_get_output_device(soundio, i);
            SoundIoDevice device = *devicePtr;
            string name = Marshal.PtrToStringUTF8(device.name)!;
            string marker = (i == defaultOutputIndex) ? " (default)" : "";
            var rawMarker = device.is_raw ? "(raw)" : "";
            Console.WriteLine($"{i}: {name}{marker}{rawMarker} {device.current_format} {device.sample_rate_current}");
            soundio_device_unref(devicePtr);
        }
    }

    void StopStreams()
    {
        if (outstream != null)
        {
            soundio_outstream_destroy(outstream);
            outstream = null;
        }

        if (instream != null)
        {
            soundio_instream_destroy(instream);
            instream = null;
        }

        if (outputDevice != null)
        {
            soundio_device_unref(outputDevice);
            outputDevice = null;
        }

        if (inputDevice != null)
        {
            soundio_device_unref(inputDevice);
            inputDevice = null;
        }
    }

    int[] prioritized_sample_rates =
    {
        48000,
        44100,
        96000,
        24000,
        0,
    };

    SoundIoFormat[] prioritized_formats =
    {
        SoundIoFormat.SoundIoFormatFloat32LE,
        SoundIoFormat.SoundIoFormatFloat32BE,
        SoundIoFormat.SoundIoFormatS32LE,
        SoundIoFormat.SoundIoFormatS32BE,
        0,
    };

    bool StartStreams(int selectedInputIndex, int selectedOutputIndex)
    {
        double microphone_latency = 0.1; // seconds
        // 入力デバイス設定
        inputDevice = soundio_get_input_device(soundio, selectedInputIndex);
        if (inputDevice == null)
        {
            Console.Error.WriteLine("Failed to get input device");
            return false;
        }


        // 出力デバイス設定
        outputDevice = soundio_get_output_device(soundio, selectedOutputIndex);
        if (outputDevice == null)
        {
            Console.Error.WriteLine("Failed to get output device");
            soundio_device_unref(inputDevice);
            inputDevice = null;
            return false;
        }

        soundio_device_sort_channel_layouts(outputDevice);


        SoundIoChannelLayout* layout = soundio_best_matching_channel_layout(
            outputDevice->layouts, outputDevice->layout_count,
            inputDevice->layouts, inputDevice->layout_count);


        int sample_rate = 0;
        foreach (var testSampleRate in prioritized_sample_rates)
        {
            if (soundio_device_supports_sample_rate(inputDevice, testSampleRate) &&
                soundio_device_supports_sample_rate(outputDevice, testSampleRate))
            {
                sample_rate = testSampleRate;
                break;
            }
        }

        Console.WriteLine(sample_rate);

        SoundIoFormat fmt = 0;
        foreach (var textFmt in prioritized_formats)
        {
            if (soundio_device_supports_format(inputDevice, textFmt) &&
                soundio_device_supports_format(outputDevice, textFmt))
            {
                fmt = textFmt;
                break;
            }
        }

        Console.WriteLine(fmt);
        // 入力ストリーム作成
        instream = soundio_instream_create(inputDevice);
        if (instream == null)
        {
            Console.Error.WriteLine("Failed to create input stream");
            StopStreams();
            return false;
        }

        ref SoundIoInStream instrStreamStruct = ref instream[0];
        instrStreamStruct.format = fmt;
        instrStreamStruct.sample_rate = sample_rate;
        instrStreamStruct.software_latency = microphone_latency;
        instrStreamStruct.layout = *layout;
        instrStreamStruct.read_callback = &OnReadCallback;
        instrStreamStruct.userdata = GCHandle.ToIntPtr(gcHandle);

        SoundIoError err = soundio_instream_open(instream);
        if (err != 0)
        {
            Console.Error.WriteLine($"Failed to open input stream: {err}");
            StopStreams();
            return false;
        }

        outstream = soundio_outstream_create(outputDevice);
        if (outstream == null)
        {
            Console.Error.WriteLine("Failed to create output stream");
            StopStreams();
            return false;
        }

        ref SoundIoOutStream outStreamStruct = ref outstream[0];
        outStreamStruct.format = fmt;
        outStreamStruct.sample_rate = sample_rate;
        outStreamStruct.software_latency = microphone_latency;
        outStreamStruct.write_callback = &OnWriteCallback;
        outStreamStruct.userdata = GCHandle.ToIntPtr(gcHandle);
        outStreamStruct.underflow_callback = &underflow_callback;

        err = soundio_outstream_open(outstream);
        if (err != 0)
        {
            Console.Error.WriteLine($"Failed to open output stream: {err}");
            StopStreams();
            return false;
        }

        int capacity = (int)(microphone_latency * 2 * instream->sample_rate * instream->bytes_per_frame);
        ring_buffer = soundio_ring_buffer_create(soundio, capacity);

        var buf = soundio_ring_buffer_write_ptr(ring_buffer);
        int fill_count = (int)(microphone_latency * outstream->sample_rate * outstream->bytes_per_frame);
        memset(buf, 0, fill_count);
        soundio_ring_buffer_advance_write_ptr(ring_buffer, fill_count);

        err = soundio_instream_start(instream);
        if (err != 0)
        {
            Console.Error.WriteLine($"Failed to start input stream: {err}");
            StopStreams();
            return false;
        }

        err = soundio_outstream_start(outstream);
        if (err != 0)
        {
            Console.Error.WriteLine($"Failed to start output stream: {err}");
            StopStreams();
            return false;
        }

        return true;
    }

    public void Run()
    {
        gcHandle = GCHandle.Alloc(this, GCHandleType.Normal);

        try
        {
            soundio = soundio_create();
            if (soundio == null)
            {
                Console.Error.WriteLine("soundio_create failed");
                return;
            }

            SoundIoError err = soundio_connect(soundio);
            if (err != 0)
            {
                Console.Error.WriteLine($"soundio_connect failed: {err}");
                soundio_destroy(soundio);
                return;
            }

            soundio_flush_events(soundio);

            ListDevices();

            Console.Write("\nSelect input device index (or press Enter for default): ");
            string inputIndexStr = Console.ReadLine();
            int selectedInputIndex = int.TryParse(inputIndexStr, out int parsedInput)
                ? parsedInput
                : soundio_default_input_device_index(soundio);

            Console.Write("Select output device index (or press Enter for default): ");
            string outputIndexStr = Console.ReadLine();
            int selectedOutputIndex = int.TryParse(outputIndexStr, out int parsedOutput)
                ? parsedOutput
                : soundio_default_output_device_index(soundio);

            if (!StartStreams(selectedInputIndex, selectedOutputIndex))
            {
                Console.Error.WriteLine("Failed to start streams");
                soundio_destroy(soundio);
                return;
            }

            while (true)
            {
                soundio_wait_events(soundio);
            }
        }
        finally
        {
            // クリーンアップ
            StopStreams();
            if (soundio != null)
            {
                soundio_destroy(soundio);
            }

            if (gcHandle.IsAllocated)
            {
                gcHandle.Free();
            }

            if (ring_buffer != null)
                soundio_ring_buffer_destroy(ring_buffer);
        }
    }
}