#pragma warning disable CS8500
#pragma warning disable CS8981
using System;
using System.Runtime.InteropServices;


namespace LibSoundIo.Interop
{
    public unsafe partial struct SoundIo
    {
        const string __DllName = "libsoundio";

        /// <summary>
        ///  See also ::soundio_version_major, ::soundio_version_minor, ::soundio_version_patch
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_version_string", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr soundio_version_string();

        /// <summary>
        ///  See also ::soundio_version_string, ::soundio_version_minor, ::soundio_version_patch
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_version_major", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int soundio_version_major();

        /// <summary>
        ///  See also ::soundio_version_major, ::soundio_version_string, ::soundio_version_patch
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_version_minor", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int soundio_version_minor();

        /// <summary>
        ///  See also ::soundio_version_major, ::soundio_version_minor, ::soundio_version_string
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_version_patch", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int soundio_version_patch();

        /// <summary>
        ///  Create a SoundIo context. You may create multiple instances of this to
        ///  connect to multiple backends. Sets all fields to defaults.
        ///  Returns `NULL` if and only if memory could not be allocated.
        ///  See also ::soundio_destroy
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_create", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern SoundIo* soundio_create();

        [DllImport(__DllName, EntryPoint = "soundio_destroy", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void soundio_destroy(SoundIo* soundio);

        /// <summary>
        ///  Tries ::soundio_connect_backend on all available backends in order.
        ///  Possible errors:
        ///  * #SoundIoErrorInvalid - already connected
        ///  * #SoundIoErrorNoMem
        ///  * #SoundIoErrorSystemResources
        ///  * #SoundIoErrorNoSuchClient - when JACK returns `JackNoSuchClient`
        ///  See also ::soundio_disconnect
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_connect", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern SoundIoError soundio_connect(SoundIo* soundio);

        /// <summary>
        ///  Instead of calling ::soundio_connect you may call this function to try a
        ///  specific backend.
        ///  Possible errors:
        ///  * #SoundIoErrorInvalid - already connected or invalid backend parameter
        ///  * #SoundIoErrorNoMem
        ///  * #SoundIoErrorBackendUnavailable - backend was not compiled in
        ///  * #SoundIoErrorSystemResources
        ///  * #SoundIoErrorNoSuchClient - when JACK returns `JackNoSuchClient`
        ///  * #SoundIoErrorInitAudioBackend - requested `backend` is not active
        ///  * #SoundIoErrorBackendDisconnected - backend disconnected while connecting
        ///  See also ::soundio_disconnect
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_connect_backend", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern SoundIoError soundio_connect_backend(SoundIo* soundio, SoundIoBackend backend);

        [DllImport(__DllName, EntryPoint = "soundio_disconnect", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void soundio_disconnect(SoundIo* soundio);

        /// <summary>
        ///  Get a string representation of a #SoundIoError
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_strerror", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr soundio_strerror(SoundIoError error);

        /// <summary>
        ///  Get a string representation of a #SoundIoBackend
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_backend_name", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr soundio_backend_name(SoundIoBackend backend);

        /// <summary>
        ///  Returns the number of available backends.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_backend_count", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int soundio_backend_count(SoundIo* soundio);

        /// <summary>
        ///  get the available backend at the specified index
        ///  (0 &lt;= index &lt; ::soundio_backend_count)
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_get_backend", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern SoundIoBackend soundio_get_backend(SoundIo* soundio, int index);

        /// <summary>
        ///  Returns whether libsoundio was compiled with backend.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_have_backend", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern byte soundio_have_backend(SoundIoBackend backend);

        /// <summary>
        ///  Atomically update information for all connected devices. Note that calling
        ///  this function merely flips a pointer; the actual work of collecting device
        ///  information is done elsewhere. It is performant to call this function many
        ///  times per second.
        /// 
        ///  When you call this, the following callbacks might be called:
        ///  * SoundIo::on_devices_change
        ///  * SoundIo::on_backend_disconnect
        ///  This is the only time those callbacks can be called.
        /// 
        ///  This must be called from the same thread as the thread in which you call
        ///  these functions:
        ///  * ::soundio_input_device_count
        ///  * ::soundio_output_device_count
        ///  * ::soundio_get_input_device
        ///  * ::soundio_get_output_device
        ///  * ::soundio_default_input_device_index
        ///  * ::soundio_default_output_device_index
        /// 
        ///  Note that if you do not care about learning about updated devices, you
        ///  might call this function only once ever and never call
        ///  ::soundio_wait_events.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_flush_events", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void soundio_flush_events(SoundIo* soundio);

        /// <summary>
        ///  This function calls ::soundio_flush_events then blocks until another event
        ///  is ready or you call ::soundio_wakeup. Be ready for spurious wakeups.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_wait_events", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void soundio_wait_events(SoundIo* soundio);

        /// <summary>
        ///  Makes ::soundio_wait_events stop blocking.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_wakeup", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void soundio_wakeup(SoundIo* soundio);

        /// <summary>
        ///  If necessary you can manually trigger a device rescan. Normally you will
        ///  not ever have to call this function, as libsoundio listens to system events
        ///  for device changes and responds to them by rescanning devices and preparing
        ///  the new device information for you to be atomically replaced when you call
        ///  ::soundio_flush_events. However you might run into cases where you want to
        ///  force trigger a device rescan, for example if an ALSA device has a
        ///  SoundIoDevice::probe_error.
        /// 
        ///  After you call this you still have to use ::soundio_flush_events or
        ///  ::soundio_wait_events and then wait for the
        ///  SoundIo::on_devices_change callback.
        /// 
        ///  This can be called from any thread context except for
        ///  SoundIoOutStream::write_callback and SoundIoInStream::read_callback
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_force_device_scan", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void soundio_force_device_scan(SoundIo* soundio);

        /// <summary>
        ///  Returns whether the channel count field and each channel id matches in
        ///  the supplied channel layouts.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_channel_layout_equal", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern byte soundio_channel_layout_equal(SoundIoChannelLayout* a, SoundIoChannelLayout* b);

        [DllImport(__DllName, EntryPoint = "soundio_get_channel_name", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr soundio_get_channel_name(SoundIoChannelId id);

        /// <summary>
        ///  Given UTF-8 encoded text which is the name of a channel such as
        ///  "Front Left", "FL", or "front-left", return the corresponding
        ///  SoundIoChannelId. Returns SoundIoChannelIdInvalid for no match.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_parse_channel_id", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern SoundIoChannelId soundio_parse_channel_id(IntPtr str_, int str_len);

        /// <summary>
        ///  Returns the number of builtin channel layouts.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_channel_layout_builtin_count", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int soundio_channel_layout_builtin_count();

        /// <summary>
        ///  Returns a builtin channel layout. 0 &lt;= `index` &lt; ::soundio_channel_layout_builtin_count
        /// 
        ///  Although `index` is of type `int`, it should be a valid
        ///  #SoundIoChannelLayoutId enum value.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_channel_layout_get_builtin", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern SoundIoChannelLayout* soundio_channel_layout_get_builtin(int index);

        /// <summary>
        ///  Get the default builtin channel layout for the given number of channels.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_channel_layout_get_default", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern SoundIoChannelLayout* soundio_channel_layout_get_default(int channel_count);

        /// <summary>
        ///  Return the index of `channel` in `layout`, or `-1` if not found.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_channel_layout_find_channel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int soundio_channel_layout_find_channel(SoundIoChannelLayout* layout, SoundIoChannelId channel);

        /// <summary>
        ///  Populates the name field of layout if it matches a builtin one.
        ///  returns whether it found a match
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_channel_layout_detect_builtin", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern byte soundio_channel_layout_detect_builtin(SoundIoChannelLayout* layout);

        /// <summary>
        ///  Iterates over preferred_layouts. Returns the first channel layout in
        ///  preferred_layouts which matches one of the channel layouts in
        ///  available_layouts. Returns NULL if none matches.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_best_matching_channel_layout", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern SoundIoChannelLayout* soundio_best_matching_channel_layout(SoundIoChannelLayout* preferred_layouts, int preferred_layout_count, SoundIoChannelLayout* available_layouts, int available_layout_count);

        /// <summary>
        ///  Sorts by channel count, descending.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_sort_channel_layouts", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void soundio_sort_channel_layouts(SoundIoChannelLayout* layouts, int layout_count);

        /// <summary>
        ///  Returns -1 on invalid format.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_get_bytes_per_sample", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int soundio_get_bytes_per_sample(SoundIoFormat format);

        /// <summary>
        ///  Returns string representation of `format`.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_format_string", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr soundio_format_string(SoundIoFormat format);

        /// <summary>
        ///  Get the number of input devices.
        ///  Returns -1 if you never called ::soundio_flush_events.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_input_device_count", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int soundio_input_device_count(SoundIo* soundio);

        /// <summary>
        ///  Get the number of output devices.
        ///  Returns -1 if you never called ::soundio_flush_events.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_output_device_count", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int soundio_output_device_count(SoundIo* soundio);

        /// <summary>
        ///  Always returns a device. Call ::soundio_device_unref when done.
        ///  `index` must be 0 &lt;= index &lt; ::soundio_input_device_count
        ///  Returns NULL if you never called ::soundio_flush_events or if you provide
        ///  invalid parameter values.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_get_input_device", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern SoundIoDevice* soundio_get_input_device(SoundIo* soundio, int index);

        /// <summary>
        ///  Always returns a device. Call ::soundio_device_unref when done.
        ///  `index` must be 0 &lt;= index &lt; ::soundio_output_device_count
        ///  Returns NULL if you never called ::soundio_flush_events or if you provide
        ///  invalid parameter values.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_get_output_device", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern SoundIoDevice* soundio_get_output_device(SoundIo* soundio, int index);

        /// <summary>
        ///  returns the index of the default input device
        ///  returns -1 if there are no devices or if you never called
        ///  ::soundio_flush_events.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_default_input_device_index", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int soundio_default_input_device_index(SoundIo* soundio);

        /// <summary>
        ///  returns the index of the default output device
        ///  returns -1 if there are no devices or if you never called
        ///  ::soundio_flush_events.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_default_output_device_index", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int soundio_default_output_device_index(SoundIo* soundio);

        /// <summary>
        ///  Add 1 to the reference count of `device`.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_device_ref", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void soundio_device_ref(SoundIoDevice* device);

        /// <summary>
        ///  Remove 1 to the reference count of `device`. Clean up if it was the last
        ///  reference.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_device_unref", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void soundio_device_unref(SoundIoDevice* device);

        /// <summary>
        ///  Return `true` if and only if the devices have the same SoundIoDevice::id,
        ///  SoundIoDevice::is_raw, and SoundIoDevice::aim are the same.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_device_equal", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern byte soundio_device_equal(SoundIoDevice* a, SoundIoDevice* b);

        /// <summary>
        ///  Sorts channel layouts by channel count, descending.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_device_sort_channel_layouts", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void soundio_device_sort_channel_layouts(SoundIoDevice* device);

        /// <summary>
        ///  Convenience function. Returns whether `format` is included in the device's
        ///  supported formats.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_device_supports_format", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern byte soundio_device_supports_format(SoundIoDevice* device, SoundIoFormat format);

        /// <summary>
        ///  Convenience function. Returns whether `layout` is included in the device's
        ///  supported channel layouts.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_device_supports_layout", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern byte soundio_device_supports_layout(SoundIoDevice* device, SoundIoChannelLayout* layout);

        /// <summary>
        ///  Convenience function. Returns whether `sample_rate` is included in the
        ///  device's supported sample rates.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_device_supports_sample_rate", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern byte soundio_device_supports_sample_rate(SoundIoDevice* device, int sample_rate);

        /// <summary>
        ///  Convenience function. Returns the available sample rate nearest to
        ///  `sample_rate`, rounding up.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_device_nearest_sample_rate", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int soundio_device_nearest_sample_rate(SoundIoDevice* device, int sample_rate);

        /// <summary>
        ///  Allocates memory and sets defaults. Next you should fill out the struct fields
        ///  and then call ::soundio_outstream_open. Sets all fields to defaults.
        ///  Returns `NULL` if and only if memory could not be allocated.
        ///  See also ::soundio_outstream_destroy
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_outstream_create", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern SoundIoOutStream* soundio_outstream_create(SoundIoDevice* device);

        /// <summary>
        ///  You may not call this function from the SoundIoOutStream::write_callback thread context.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_outstream_destroy", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void soundio_outstream_destroy(SoundIoOutStream* outstream);

        /// <summary>
        ///  After you call this function, SoundIoOutStream::software_latency is set to
        ///  the correct value.
        /// 
        ///  The next thing to do is call ::soundio_outstream_start.
        ///  If this function returns an error, the outstream is in an invalid state and
        ///  you must call ::soundio_outstream_destroy on it.
        /// 
        ///  Possible errors:
        ///  * #SoundIoErrorInvalid
        ///    * SoundIoDevice::aim is not #SoundIoDeviceAimOutput
        ///    * SoundIoOutStream::format is not valid
        ///    * SoundIoOutStream::channel_count is greater than #SOUNDIO_MAX_CHANNELS
        ///  * #SoundIoErrorNoMem
        ///  * #SoundIoErrorOpeningDevice
        ///  * #SoundIoErrorBackendDisconnected
        ///  * #SoundIoErrorSystemResources
        ///  * #SoundIoErrorNoSuchClient - when JACK returns `JackNoSuchClient`
        ///  * #SoundIoErrorIncompatibleBackend - SoundIoOutStream::channel_count is
        ///    greater than the number of channels the backend can handle.
        ///  * #SoundIoErrorIncompatibleDevice - stream parameters requested are not
        ///    compatible with the chosen device.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_outstream_open", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern SoundIoError soundio_outstream_open(SoundIoOutStream* outstream);

        /// <summary>
        ///  After you call this function, SoundIoOutStream::write_callback will be called.
        /// 
        ///  This function might directly call SoundIoOutStream::write_callback.
        /// 
        ///  Possible errors:
        ///  * #SoundIoErrorStreaming
        ///  * #SoundIoErrorNoMem
        ///  * #SoundIoErrorSystemResources
        ///  * #SoundIoErrorBackendDisconnected
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_outstream_start", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern SoundIoError soundio_outstream_start(SoundIoOutStream* outstream);

        /// <summary>
        ///  Call this function when you are ready to begin writing to the device buffer.
        ///   * `outstream` - (in) The output stream you want to write to.
        ///   * `areas` - (out) The memory addresses you can write data to, one per
        ///     channel. It is OK to modify the pointers if that helps you iterate.
        ///   * `frame_count` - (in/out) Provide the number of frames you want to write.
        ///     Returned will be the number of frames you can actually write, which is
        ///     also the number of frames that will be written when you call
        ///     ::soundio_outstream_end_write. The value returned will always be less
        ///     than or equal to the value provided.
        ///  It is your responsibility to call this function exactly as many times as
        ///  necessary to meet the `frame_count_min` and `frame_count_max` criteria from
        ///  SoundIoOutStream::write_callback.
        ///  You must call this function only from the SoundIoOutStream::write_callback thread context.
        ///  After calling this function, write data to `areas` and then call
        ///  ::soundio_outstream_end_write.
        ///  If this function returns an error, do not call ::soundio_outstream_end_write.
        /// 
        ///  Possible errors:
        ///  * #SoundIoErrorInvalid
        ///    * `*frame_count` &lt;= 0
        ///    * `*frame_count` &lt; `frame_count_min` or `*frame_count` &gt; `frame_count_max`
        ///    * function called too many times without respecting `frame_count_max`
        ///  * #SoundIoErrorStreaming
        ///  * #SoundIoErrorUnderflow - an underflow caused this call to fail. You might
        ///    also get a SoundIoOutStream::underflow_callback, and you might not get
        ///    this error code when an underflow occurs. Unlike #SoundIoErrorStreaming,
        ///    the outstream is still in a valid state and streaming can continue.
        ///  * #SoundIoErrorIncompatibleDevice - in rare cases it might just now
        ///    be discovered that the device uses non-byte-aligned access, in which
        ///    case this error code is returned.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_outstream_begin_write", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern SoundIoError soundio_outstream_begin_write(SoundIoOutStream* outstream, out SoundIoChannelArea* areas, ref int frame_count);

        /// <summary>
        ///  Commits the write that you began with ::soundio_outstream_begin_write.
        ///  You must call this function only from the SoundIoOutStream::write_callback thread context.
        /// 
        ///  Possible errors:
        ///  * #SoundIoErrorStreaming
        ///  * #SoundIoErrorUnderflow - an underflow caused this call to fail. You might
        ///    also get a SoundIoOutStream::underflow_callback, and you might not get
        ///    this error code when an underflow occurs. Unlike #SoundIoErrorStreaming,
        ///    the outstream is still in a valid state and streaming can continue.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_outstream_end_write", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern SoundIoError soundio_outstream_end_write(SoundIoOutStream* outstream);

        /// <summary>
        ///  Clears the output stream buffer.
        ///  This function can be called from any thread.
        ///  This function can be called regardless of whether the outstream is paused
        ///  or not.
        ///  Some backends do not support clearing the buffer. On these backends this
        ///  function will return SoundIoErrorIncompatibleBackend.
        ///  Some devices do not support clearing the buffer. On these devices this
        ///  function might return SoundIoErrorIncompatibleDevice.
        ///  Possible errors:
        /// 
        ///  * #SoundIoErrorStreaming
        ///  * #SoundIoErrorIncompatibleBackend
        ///  * #SoundIoErrorIncompatibleDevice
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_outstream_clear_buffer", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int soundio_outstream_clear_buffer(SoundIoOutStream* outstream);

        /// <summary>
        ///  If the underlying backend and device support pausing, this pauses the
        ///  stream. SoundIoOutStream::write_callback may be called a few more times if
        ///  the buffer is not full.
        ///  Pausing might put the hardware into a low power state which is ideal if your
        ///  software is silent for some time.
        ///  This function may be called from any thread context, including
        ///  SoundIoOutStream::write_callback.
        ///  Pausing when already paused or unpausing when already unpaused has no
        ///  effect and returns #SoundIoErrorNone.
        /// 
        ///  Possible errors:
        ///  * #SoundIoErrorBackendDisconnected
        ///  * #SoundIoErrorStreaming
        ///  * #SoundIoErrorIncompatibleDevice - device does not support
        ///    pausing/unpausing. This error code might not be returned even if the
        ///    device does not support pausing/unpausing.
        ///  * #SoundIoErrorIncompatibleBackend - backend does not support
        ///    pausing/unpausing.
        ///  * #SoundIoErrorInvalid - outstream not opened and started
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_outstream_pause", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern SoundIoError soundio_outstream_pause(SoundIoOutStream* outstream, byte pause);

        /// <summary>
        ///  Obtain the total number of seconds that the next frame written after the
        ///  last frame written with ::soundio_outstream_end_write will take to become
        ///  audible. This includes both software and hardware latency. In other words,
        ///  if you call this function directly after calling ::soundio_outstream_end_write,
        ///  this gives you the number of seconds that the next frame written will take
        ///  to become audible.
        /// 
        ///  This function must be called only from within SoundIoOutStream::write_callback.
        /// 
        ///  Possible errors:
        ///  * #SoundIoErrorStreaming
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_outstream_get_latency", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int soundio_outstream_get_latency(SoundIoOutStream* outstream, double* out_latency);

        [DllImport(__DllName, EntryPoint = "soundio_outstream_set_volume", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int soundio_outstream_set_volume(SoundIoOutStream* outstream, double volume);

        /// <summary>
        ///  Allocates memory and sets defaults. Next you should fill out the struct fields
        ///  and then call ::soundio_instream_open. Sets all fields to defaults.
        ///  Returns `NULL` if and only if memory could not be allocated.
        ///  See also ::soundio_instream_destroy
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_instream_create", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern SoundIoInStream* soundio_instream_create(SoundIoDevice* device);

        /// <summary>
        ///  You may not call this function from SoundIoInStream::read_callback.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_instream_destroy", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void soundio_instream_destroy(SoundIoInStream* instream);

        /// <summary>
        ///  After you call this function, SoundIoInStream::software_latency is set to the correct
        ///  value.
        ///  The next thing to do is call ::soundio_instream_start.
        ///  If this function returns an error, the instream is in an invalid state and
        ///  you must call ::soundio_instream_destroy on it.
        /// 
        ///  Possible errors:
        ///  * #SoundIoErrorInvalid
        ///    * device aim is not #SoundIoDeviceAimInput
        ///    * format is not valid
        ///    * requested layout channel count &gt; #SOUNDIO_MAX_CHANNELS
        ///  * #SoundIoErrorOpeningDevice
        ///  * #SoundIoErrorNoMem
        ///  * #SoundIoErrorBackendDisconnected
        ///  * #SoundIoErrorSystemResources
        ///  * #SoundIoErrorNoSuchClient
        ///  * #SoundIoErrorIncompatibleBackend
        ///  * #SoundIoErrorIncompatibleDevice
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_instream_open", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern SoundIoError soundio_instream_open(SoundIoInStream* instream);

        /// <summary>
        ///  After you call this function, SoundIoInStream::read_callback will be called.
        /// 
        ///  Possible errors:
        ///  * #SoundIoErrorBackendDisconnected
        ///  * #SoundIoErrorStreaming
        ///  * #SoundIoErrorOpeningDevice
        ///  * #SoundIoErrorSystemResources
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_instream_start", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern SoundIoError soundio_instream_start(SoundIoInStream* instream);

        /// <summary>
        ///  Call this function when you are ready to begin reading from the device
        ///  buffer.
        ///  * `instream` - (in) The input stream you want to read from.
        ///  * `areas` - (out) The memory addresses you can read data from. It is OK
        ///    to modify the pointers if that helps you iterate. There might be a "hole"
        ///    in the buffer. To indicate this, `areas` will be `NULL` and `frame_count`
        ///    tells how big the hole is in frames.
        ///  * `frame_count` - (in/out) - Provide the number of frames you want to read;
        ///    returns the number of frames you can actually read. The returned value
        ///    will always be less than or equal to the provided value. If the provided
        ///    value is less than `frame_count_min` from SoundIoInStream::read_callback this function
        ///    returns with #SoundIoErrorInvalid.
        ///  It is your responsibility to call this function no more and no fewer than the
        ///  correct number of times according to the `frame_count_min` and
        ///  `frame_count_max` criteria from SoundIoInStream::read_callback.
        ///  You must call this function only from the SoundIoInStream::read_callback thread context.
        ///  After calling this function, read data from `areas` and then use
        ///  ::soundio_instream_end_read` to actually remove the data from the buffer
        ///  and move the read index forward. ::soundio_instream_end_read should not be
        ///  called if the buffer is empty (`frame_count` == 0), but it should be called
        ///  if there is a hole.
        /// 
        ///  Possible errors:
        ///  * #SoundIoErrorInvalid
        ///    * `*frame_count` &lt; `frame_count_min` or `*frame_count` &gt; `frame_count_max`
        ///  * #SoundIoErrorStreaming
        ///  * #SoundIoErrorIncompatibleDevice - in rare cases it might just now
        ///    be discovered that the device uses non-byte-aligned access, in which
        ///    case this error code is returned.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_instream_begin_read", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern SoundIoError soundio_instream_begin_read(SoundIoInStream* instream, out SoundIoChannelArea* areas, ref int frame_count);

        /// <summary>
        ///  This will drop all of the frames from when you called
        ///  ::soundio_instream_begin_read.
        ///  You must call this function only from the SoundIoInStream::read_callback thread context.
        ///  You must call this function only after a successful call to
        ///  ::soundio_instream_begin_read.
        /// 
        ///  Possible errors:
        ///  * #SoundIoErrorStreaming
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_instream_end_read", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern SoundIoError soundio_instream_end_read(SoundIoInStream* instream);

        /// <summary>
        ///  If the underyling device supports pausing, this pauses the stream and
        ///  prevents SoundIoInStream::read_callback from being called. Otherwise this returns
        ///  #SoundIoErrorIncompatibleDevice.
        ///  This function may be called from any thread.
        ///  Pausing when already paused or unpausing when already unpaused has no
        ///  effect and always returns #SoundIoErrorNone.
        /// 
        ///  Possible errors:
        ///  * #SoundIoErrorBackendDisconnected
        ///  * #SoundIoErrorStreaming
        ///  * #SoundIoErrorIncompatibleDevice - device does not support pausing/unpausing
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_instream_pause", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int soundio_instream_pause(SoundIoInStream* instream, byte pause);

        /// <summary>
        ///  Obtain the number of seconds that the next frame of sound being
        ///  captured will take to arrive in the buffer, plus the amount of time that is
        ///  represented in the buffer. This includes both software and hardware latency.
        /// 
        ///  This function must be called only from within SoundIoInStream::read_callback.
        /// 
        ///  Possible errors:
        ///  * #SoundIoErrorStreaming
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_instream_get_latency", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int soundio_instream_get_latency(SoundIoInStream* instream, double* out_latency);

        /// <summary>
        ///  A ring buffer is a single-reader single-writer lock-free fixed-size queue.
        ///  libsoundio ring buffers use memory mapping techniques to enable a
        ///  contiguous buffer when reading or writing across the boundary of the ring
        ///  buffer's capacity.
        ///  `requested_capacity` in bytes.
        ///  Returns `NULL` if and only if memory could not be allocated.
        ///  Use ::soundio_ring_buffer_capacity to get the actual capacity, which might
        ///  be greater for alignment purposes.
        ///  See also ::soundio_ring_buffer_destroy
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_ring_buffer_create", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern SoundIoRingBuffer* soundio_ring_buffer_create(SoundIo* soundio, int requested_capacity);

        [DllImport(__DllName, EntryPoint = "soundio_ring_buffer_destroy", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void soundio_ring_buffer_destroy(SoundIoRingBuffer* ring_buffer);

        /// <summary>
        ///  When you create a ring buffer, capacity might be more than the requested
        ///  capacity for alignment purposes. This function returns the actual capacity.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_ring_buffer_capacity", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int soundio_ring_buffer_capacity(SoundIoRingBuffer* ring_buffer);

        /// <summary>
        ///  Do not write more than capacity.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_ring_buffer_write_ptr", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern byte* soundio_ring_buffer_write_ptr(SoundIoRingBuffer* ring_buffer);

        /// <summary>
        ///  `count` in bytes.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_ring_buffer_advance_write_ptr", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void soundio_ring_buffer_advance_write_ptr(SoundIoRingBuffer* ring_buffer, int count);

        /// <summary>
        ///  Do not read more than capacity.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_ring_buffer_read_ptr", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern byte* soundio_ring_buffer_read_ptr(SoundIoRingBuffer* ring_buffer);

        /// <summary>
        ///  `count` in bytes.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_ring_buffer_advance_read_ptr", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void soundio_ring_buffer_advance_read_ptr(SoundIoRingBuffer* ring_buffer, int count);

        /// <summary>
        ///  Returns how many bytes of the buffer is used, ready for reading.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_ring_buffer_fill_count", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int soundio_ring_buffer_fill_count(SoundIoRingBuffer* ring_buffer);

        /// <summary>
        ///  Returns how many bytes of the buffer is free, ready for writing.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_ring_buffer_free_count", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int soundio_ring_buffer_free_count(SoundIoRingBuffer* ring_buffer);

        /// <summary>
        ///  Must be called by the writer.
        /// </summary>
        [DllImport(__DllName, EntryPoint = "soundio_ring_buffer_clear", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void soundio_ring_buffer_clear(SoundIoRingBuffer* ring_buffer);


    }

    /// <summary>
    ///  The size of this struct is OK to use.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct SoundIoChannelLayout
    {
        public IntPtr name;
        public int channel_count;
        public fixed int/* SoundIoChannelId */ channels[24];
    }

    /// <summary>
    ///  The size of this struct is OK to use.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct SoundIoSampleRateRange
    {
        public int min;
        public int max;
    }

    /// <summary>
    ///  The size of this struct is OK to use.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct SoundIoChannelArea
    {
        /// <summary>
        ///  Base address of buffer.
        /// </summary>
        public byte* ptr;
        /// <summary>
        ///  How many bytes it takes to get from the beginning of one sample to
        ///  the beginning of the next sample.
        /// </summary>
        public int step;
    }

    /// <summary>
    ///  The size of this struct is not part of the API or ABI.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct SoundIo
    {
        /// <summary>
        ///  Optional. Put whatever you want here. Defaults to NULL.
        /// </summary>
        public IntPtr userdata;
        /// <summary>
        ///  Optional callback. Called when the list of devices change. Only called
        ///  during a call to ::soundio_flush_events or ::soundio_wait_events.
        /// </summary>
        public delegate* unmanaged[Cdecl]<SoundIo*, void> on_devices_change;
        /// <summary>
        ///  Optional callback. Called when the backend disconnects. For example,
        ///  when the JACK server shuts down. When this happens, listing devices
        ///  and opening streams will always fail with
        ///  SoundIoErrorBackendDisconnected. This callback is only called during a
        ///  call to ::soundio_flush_events or ::soundio_wait_events.
        ///  If you do not supply a callback, the default will crash your program
        ///  with an error message. This callback is also called when the thread
        ///  that retrieves device information runs into an unrecoverable condition
        ///  such as running out of memory.
        /// 
        ///  Possible errors:
        ///  * #SoundIoErrorBackendDisconnected
        ///  * #SoundIoErrorNoMem
        ///  * #SoundIoErrorSystemResources
        ///  * #SoundIoErrorOpeningDevice - unexpected problem accessing device
        ///    information
        /// </summary>
        public delegate* unmanaged[Cdecl]<SoundIo*, int, void> on_backend_disconnect;
        /// <summary>
        ///  Optional callback. Called from an unknown thread that you should not use
        ///  to call any soundio functions. You may use this to signal a condition
        ///  variable to wake up. Called when ::soundio_wait_events would be woken up.
        /// </summary>
        public delegate* unmanaged[Cdecl]<SoundIo*, void> on_events_signal;
        /// <summary>
        ///  Read-only. After calling ::soundio_connect or ::soundio_connect_backend,
        ///  this field tells which backend is currently connected.
        /// </summary>
        public SoundIoBackend current_backend;
        /// <summary>
        ///  Optional: Application name.
        ///  PulseAudio uses this for "application name".
        ///  JACK uses this for `client_name`.
        ///  Must not contain a colon (":").
        /// </summary>
        public IntPtr app_name;
        /// <summary>
        ///  Optional: Real time priority warning.
        ///  This callback is fired when making thread real-time priority failed. By
        ///  default, it will print to stderr only the first time it is called
        ///  a message instructing the user how to configure their system to allow
        ///  real-time priority threads. This must be set to a function not NULL.
        ///  To silence the warning, assign this to a function that does nothing.
        /// </summary>
        public delegate* unmanaged[Cdecl]<void> emit_rtprio_warning;
        /// <summary>
        ///  Optional: JACK info callback.
        ///  By default, libsoundio sets this to an empty function in order to
        ///  silence stdio messages from JACK. You may override the behavior by
        ///  setting this to `NULL` or providing your own function. This is
        ///  registered with JACK regardless of whether ::soundio_connect_backend
        ///  succeeds.
        /// </summary>
        public delegate* unmanaged[Cdecl]<IntPtr, void> jack_info_callback;
        /// <summary>
        ///  Optional: JACK error callback.
        ///  See SoundIo::jack_info_callback
        /// </summary>
        public delegate* unmanaged[Cdecl]<IntPtr, void> jack_error_callback;
    }

    /// <summary>
    ///  The size of this struct is not part of the API or ABI.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct SoundIoDevice
    {
        /// <summary>
        ///  Read-only. Set automatically.
        /// </summary>
        public SoundIo* soundio;
        /// <summary>
        ///  A string of bytes that uniquely identifies this device.
        ///  If the same physical device supports both input and output, that makes
        ///  one SoundIoDevice for the input and one SoundIoDevice for the output.
        ///  In this case, the id of each SoundIoDevice will be the same, and
        ///  SoundIoDevice::aim will be different. Additionally, if the device
        ///  supports raw mode, there may be up to four devices with the same id:
        ///  one for each value of SoundIoDevice::is_raw and one for each value of
        ///  SoundIoDevice::aim.
        /// </summary>
        public IntPtr id;
        /// <summary>
        ///  User-friendly UTF-8 encoded text to describe the device.
        /// </summary>
        public IntPtr name;
        /// <summary>
        ///  Tells whether this device is an input device or an output device.
        /// </summary>
        public SoundIoDeviceAim aim;
        /// <summary>
        ///  Channel layouts are handled similarly to SoundIoDevice::formats.
        ///  If this information is missing due to a SoundIoDevice::probe_error,
        ///  layouts will be NULL. It's OK to modify this data, for example calling
        ///  ::soundio_sort_channel_layouts on it.
        ///  Devices are guaranteed to have at least 1 channel layout.
        /// </summary>
        public SoundIoChannelLayout* layouts;
        public int layout_count;
        /// <summary>
        ///  See SoundIoDevice::current_format
        /// </summary>
        public SoundIoChannelLayout current_layout;
        /// <summary>
        ///  List of formats this device supports. See also
        ///  SoundIoDevice::current_format.
        /// </summary>
        public SoundIoFormat* formats;
        /// <summary>
        ///  How many formats are available in SoundIoDevice::formats.
        /// </summary>
        public int format_count;
        /// <summary>
        ///  A device is either a raw device or it is a virtual device that is
        ///  provided by a software mixing service such as dmix or PulseAudio (see
        ///  SoundIoDevice::is_raw). If it is a raw device,
        ///  current_format is meaningless;
        ///  the device has no current format until you open it. On the other hand,
        ///  if it is a virtual device, current_format describes the
        ///  destination sample format that your audio will be converted to. Or,
        ///  if you're the lucky first application to open the device, you might
        ///  cause the current_format to change to your format.
        ///  Generally, you want to ignore current_format and use
        ///  whatever format is most convenient
        ///  for you which is supported by the device, because when you are the only
        ///  application left, the mixer might decide to switch
        ///  current_format to yours. You can learn the supported formats via
        ///  formats and SoundIoDevice::format_count. If this information is missing
        ///  due to a probe error, formats will be `NULL`. If current_format is
        ///  unavailable, it will be set to #SoundIoFormatInvalid.
        ///  Devices are guaranteed to have at least 1 format available.
        /// </summary>
        public SoundIoFormat current_format;
        /// <summary>
        ///  Sample rate is the number of frames per second.
        ///  Sample rate is handled very similar to SoundIoDevice::formats.
        ///  If sample rate information is missing due to a probe error, the field
        ///  will be set to NULL.
        ///  Devices which have SoundIoDevice::probe_error set to #SoundIoErrorNone are
        ///  guaranteed to have at least 1 sample rate available.
        /// </summary>
        public SoundIoSampleRateRange* sample_rates;
        /// <summary>
        ///  How many sample rate ranges are available in
        ///  SoundIoDevice::sample_rates. 0 if sample rate information is missing
        ///  due to a probe error.
        /// </summary>
        public int sample_rate_count;
        /// <summary>
        ///  See SoundIoDevice::current_format
        ///  0 if sample rate information is missing due to a probe error.
        /// </summary>
        public int sample_rate_current;
        /// <summary>
        ///  Software latency minimum in seconds. If this value is unknown or
        ///  irrelevant, it is set to 0.0.
        ///  For PulseAudio and WASAPI this value is unknown until you open a
        ///  stream.
        /// </summary>
        public double software_latency_min;
        /// <summary>
        ///  Software latency maximum in seconds. If this value is unknown or
        ///  irrelevant, it is set to 0.0.
        ///  For PulseAudio and WASAPI this value is unknown until you open a
        ///  stream.
        /// </summary>
        public double software_latency_max;
        /// <summary>
        ///  Software latency in seconds. If this value is unknown or
        ///  irrelevant, it is set to 0.0.
        ///  For PulseAudio and WASAPI this value is unknown until you open a
        ///  stream.
        ///  See SoundIoDevice::current_format
        /// </summary>
        public double software_latency_current;
        /// <summary>
        ///  Raw means that you are directly opening the hardware device and not
        ///  going through a proxy such as dmix, PulseAudio, or JACK. When you open a
        ///  raw device, other applications on the computer are not able to
        ///  simultaneously access the device. Raw devices do not perform automatic
        ///  resampling and thus tend to have fewer formats available.
        /// </summary>
        public byte is_raw;
        /// <summary>
        ///  Devices are reference counted. See ::soundio_device_ref and
        ///  ::soundio_device_unref.
        /// </summary>
        public int ref_count;
        /// <summary>
        ///  This is set to a SoundIoError representing the result of the device
        ///  probe. Ideally this will be SoundIoErrorNone in which case all the
        ///  fields of the device will be populated. If there is an error code here
        ///  then information about formats, sample rates, and channel layouts might
        ///  be missing.
        /// 
        ///  Possible errors:
        ///  * #SoundIoErrorOpeningDevice
        ///  * #SoundIoErrorNoMem
        /// </summary>
        public int probe_error;
    }

    /// <summary>
    ///  The size of this struct is not part of the API or ABI.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct SoundIoOutStream
    {
        /// <summary>
        ///  Populated automatically when you call ::soundio_outstream_create.
        /// </summary>
        public SoundIoDevice* device;
        /// <summary>
        ///  Defaults to #SoundIoFormatFloat32NE, followed by the first one
        ///  supported.
        /// </summary>
        public SoundIoFormat format;
        /// <summary>
        ///  Sample rate is the number of frames per second.
        ///  Defaults to 48000 (and then clamped into range).
        /// </summary>
        public int sample_rate;
        /// <summary>
        ///  Defaults to Stereo, if available, followed by the first layout
        ///  supported.
        /// </summary>
        public SoundIoChannelLayout layout;
        /// <summary>
        ///  Ignoring hardware latency, this is the number of seconds it takes for
        ///  the last sample in a full buffer to be played.
        ///  After you call ::soundio_outstream_open, this value is replaced with the
        ///  actual software latency, as near to this value as possible.
        ///  On systems that support clearing the buffer, this defaults to a large
        ///  latency, potentially upwards of 2 seconds, with the understanding that
        ///  you will call ::soundio_outstream_clear_buffer when you want to reduce
        ///  the latency to 0. On systems that do not support clearing the buffer,
        ///  this defaults to a reasonable lower latency value.
        /// 
        ///  On backends with high latencies (such as 2 seconds), `frame_count_min`
        ///  will be 0, meaning you don't have to fill the entire buffer. In this
        ///  case, the large buffer is there if you want it; you only have to fill
        ///  as much as you want. On backends like JACK, `frame_count_min` will be
        ///  equal to `frame_count_max` and if you don't fill that many frames, you
        ///  will get glitches.
        /// 
        ///  If the device has unknown software latency min and max values, you may
        ///  still set this, but you might not get the value you requested.
        ///  For PulseAudio, if you set this value to non-default, it sets
        ///  `PA_STREAM_ADJUST_LATENCY` and is the value used for `maxlength` and
        ///  `tlength`.
        /// 
        ///  For JACK, this value is always equal to
        ///  SoundIoDevice::software_latency_current of the device.
        /// </summary>
        public double software_latency;
        /// <summary>
        ///  Core Audio and WASAPI only: current output Audio Unit volume. Float, 0.0-1.0.
        /// </summary>
        public float volume;
        /// <summary>
        ///  Defaults to NULL. Put whatever you want here.
        /// </summary>
        public IntPtr userdata;
        /// <summary>
        ///  In this callback, you call ::soundio_outstream_begin_write and
        ///  ::soundio_outstream_end_write as many times as necessary to write
        ///  at minimum `frame_count_min` frames and at maximum `frame_count_max`
        ///  frames. `frame_count_max` will always be greater than 0. Note that you
        ///  should write as many frames as you can; `frame_count_min` might be 0 and
        ///  you can still get a buffer underflow if you always write
        ///  `frame_count_min` frames.
        /// 
        ///  For Dummy, ALSA, and PulseAudio, `frame_count_min` will be 0. For JACK
        ///  and CoreAudio `frame_count_min` will be equal to `frame_count_max`.
        /// 
        ///  The code in the supplied function must be suitable for real-time
        ///  execution. That means that it cannot call functions that might block
        ///  for a long time. This includes all I/O functions (disk, TTY, network),
        ///  malloc, free, printf, pthread_mutex_lock, sleep, wait, poll, select,
        ///  pthread_join, pthread_cond_wait, etc.
        /// </summary>
        public delegate* unmanaged[Cdecl]<SoundIoOutStream*, int, int, void> write_callback;
        /// <summary>
        ///  This optional callback happens when the sound device runs out of
        ///  buffered audio data to play. After this occurs, the outstream waits
        ///  until the buffer is full to resume playback.
        ///  This is called from the SoundIoOutStream::write_callback thread context.
        /// </summary>
        public delegate* unmanaged[Cdecl]<SoundIoOutStream*, void> underflow_callback;
        /// <summary>
        ///  Optional callback. `err` is always SoundIoErrorStreaming.
        ///  SoundIoErrorStreaming is an unrecoverable error. The stream is in an
        ///  invalid state and must be destroyed.
        ///  If you do not supply error_callback, the default callback will print
        ///  a message to stderr and then call `abort`.
        ///  This is called from the SoundIoOutStream::write_callback thread context.
        /// </summary>
        public delegate* unmanaged[Cdecl]<SoundIoOutStream*, int, void> error_callback;
        /// <summary>
        ///  Optional: Name of the stream. Defaults to "SoundIoOutStream"
        ///  PulseAudio uses this for the stream name.
        ///  JACK uses this for the client name of the client that connects when you
        ///  open the stream.
        ///  WASAPI uses this for the session display name.
        ///  Must not contain a colon (":").
        /// </summary>
        public IntPtr name;
        /// <summary>
        ///  Optional: Hint that this output stream is nonterminal. This is used by
        ///  JACK and it means that the output stream data originates from an input
        ///  stream. Defaults to `false`.
        /// </summary>
        public byte non_terminal_hint;
        /// <summary>
        ///  computed automatically when you call ::soundio_outstream_open
        /// </summary>
        public int bytes_per_frame;
        /// <summary>
        ///  computed automatically when you call ::soundio_outstream_open
        /// </summary>
        public int bytes_per_sample;
        /// <summary>
        ///  If setting the channel layout fails for some reason, this field is set
        ///  to an error code. Possible error codes are:
        ///  * #SoundIoErrorIncompatibleDevice
        /// </summary>
        public int layout_error;
    }

    /// <summary>
    ///  The size of this struct is not part of the API or ABI.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct SoundIoInStream
    {
        /// <summary>
        ///  Populated automatically when you call ::soundio_outstream_create.
        /// </summary>
        public SoundIoDevice* device;
        /// <summary>
        ///  Defaults to #SoundIoFormatFloat32NE, followed by the first one
        ///  supported.
        /// </summary>
        public SoundIoFormat format;
        /// <summary>
        ///  Sample rate is the number of frames per second.
        ///  Defaults to max(sample_rate_min, min(sample_rate_max, 48000))
        /// </summary>
        public int sample_rate;
        /// <summary>
        ///  Defaults to Stereo, if available, followed by the first layout
        ///  supported.
        /// </summary>
        public SoundIoChannelLayout layout;
        /// <summary>
        ///  Ignoring hardware latency, this is the number of seconds it takes for a
        ///  captured sample to become available for reading.
        ///  After you call ::soundio_instream_open, this value is replaced with the
        ///  actual software latency, as near to this value as possible.
        ///  A higher value means less CPU usage. Defaults to a large value,
        ///  potentially upwards of 2 seconds.
        ///  If the device has unknown software latency min and max values, you may
        ///  still set this, but you might not get the value you requested.
        ///  For PulseAudio, if you set this value to non-default, it sets
        ///  `PA_STREAM_ADJUST_LATENCY` and is the value used for `fragsize`.
        ///  For JACK, this value is always equal to
        ///  SoundIoDevice::software_latency_current
        /// </summary>
        public double software_latency;
        /// <summary>
        ///  Defaults to NULL. Put whatever you want here.
        /// </summary>
        public IntPtr userdata;
        /// <summary>
        ///  In this function call ::soundio_instream_begin_read and
        ///  ::soundio_instream_end_read as many times as necessary to read at
        ///  minimum `frame_count_min` frames and at maximum `frame_count_max`
        ///  frames. If you return from read_callback without having read
        ///  `frame_count_min`, the frames will be dropped. `frame_count_max` is how
        ///  many frames are available to read.
        /// 
        ///  The code in the supplied function must be suitable for real-time
        ///  execution. That means that it cannot call functions that might block
        ///  for a long time. This includes all I/O functions (disk, TTY, network),
        ///  malloc, free, printf, pthread_mutex_lock, sleep, wait, poll, select,
        ///  pthread_join, pthread_cond_wait, etc.
        /// </summary>
        public delegate* unmanaged[Cdecl]<SoundIoInStream*, int, int, void> read_callback;
        /// <summary>
        ///  This optional callback happens when the sound device buffer is full,
        ///  yet there is more captured audio to put in it.
        ///  This is never fired for PulseAudio.
        ///  This is called from the SoundIoInStream::read_callback thread context.
        /// </summary>
        public delegate* unmanaged[Cdecl]<SoundIoInStream*, void> overflow_callback;
        /// <summary>
        ///  Optional callback. `err` is always SoundIoErrorStreaming.
        ///  SoundIoErrorStreaming is an unrecoverable error. The stream is in an
        ///  invalid state and must be destroyed.
        ///  If you do not supply `error_callback`, the default callback will print
        ///  a message to stderr and then abort().
        ///  This is called from the SoundIoInStream::read_callback thread context.
        /// </summary>
        public delegate* unmanaged[Cdecl]<SoundIoInStream*, int, void> error_callback;
        /// <summary>
        ///  Optional: Name of the stream. Defaults to "SoundIoInStream";
        ///  PulseAudio uses this for the stream name.
        ///  JACK uses this for the client name of the client that connects when you
        ///  open the stream.
        ///  WASAPI uses this for the session display name.
        ///  Must not contain a colon (":").
        /// </summary>
        public IntPtr name;
        /// <summary>
        ///  Optional: Hint that this input stream is nonterminal. This is used by
        ///  JACK and it means that the data received by the stream will be
        ///  passed on or made available to another stream. Defaults to `false`.
        /// </summary>
        public byte non_terminal_hint;
        /// <summary>
        ///  computed automatically when you call ::soundio_instream_open
        /// </summary>
        public int bytes_per_frame;
        /// <summary>
        ///  computed automatically when you call ::soundio_instream_open
        /// </summary>
        public int bytes_per_sample;
        /// <summary>
        ///  If setting the channel layout fails for some reason, this field is set
        ///  to an error code. Possible error codes are: #SoundIoErrorIncompatibleDevice
        /// </summary>
        public int layout_error;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct SoundIoRingBuffer
    {
        public fixed byte _unused[1];
    }


    /// <summary>
    ///  Specifies where a channel is physically located.
    /// </summary>
    public enum SoundIoChannelId : int
    {
        SoundIoChannelIdInvalid = 0,
        /// <summary>
        /// &lt; First of the more commonly supported ids.
        /// </summary>
        SoundIoChannelIdFrontLeft = 1,
        SoundIoChannelIdFrontRight = 2,
        SoundIoChannelIdFrontCenter = 3,
        SoundIoChannelIdLfe = 4,
        SoundIoChannelIdBackLeft = 5,
        SoundIoChannelIdBackRight = 6,
        SoundIoChannelIdFrontLeftCenter = 7,
        SoundIoChannelIdFrontRightCenter = 8,
        SoundIoChannelIdBackCenter = 9,
        SoundIoChannelIdSideLeft = 10,
        SoundIoChannelIdSideRight = 11,
        SoundIoChannelIdTopCenter = 12,
        SoundIoChannelIdTopFrontLeft = 13,
        SoundIoChannelIdTopFrontCenter = 14,
        SoundIoChannelIdTopFrontRight = 15,
        SoundIoChannelIdTopBackLeft = 16,
        SoundIoChannelIdTopBackCenter = 17,
        /// <summary>
        /// &lt; Last of the more commonly supported ids.
        /// </summary>
        SoundIoChannelIdTopBackRight = 18,
        /// <summary>
        /// &lt; First of the less commonly supported ids.
        /// </summary>
        SoundIoChannelIdBackLeftCenter = 19,
        SoundIoChannelIdBackRightCenter = 20,
        SoundIoChannelIdFrontLeftWide = 21,
        SoundIoChannelIdFrontRightWide = 22,
        SoundIoChannelIdFrontLeftHigh = 23,
        SoundIoChannelIdFrontCenterHigh = 24,
        SoundIoChannelIdFrontRightHigh = 25,
        SoundIoChannelIdTopFrontLeftCenter = 26,
        SoundIoChannelIdTopFrontRightCenter = 27,
        SoundIoChannelIdTopSideLeft = 28,
        SoundIoChannelIdTopSideRight = 29,
        SoundIoChannelIdLeftLfe = 30,
        SoundIoChannelIdRightLfe = 31,
        SoundIoChannelIdLfe2 = 32,
        SoundIoChannelIdBottomCenter = 33,
        SoundIoChannelIdBottomLeftCenter = 34,
        SoundIoChannelIdBottomRightCenter = 35,
        /// <summary>
        ///  Mid/side recording
        /// </summary>
        SoundIoChannelIdMsMid = 36,
        /// <summary>
        ///  Mid/side recording
        /// </summary>
        SoundIoChannelIdMsSide = 37,
        /// <summary>
        ///  first order ambisonic channels
        /// </summary>
        SoundIoChannelIdAmbisonicW = 38,
        /// <summary>
        ///  first order ambisonic channels
        /// </summary>
        SoundIoChannelIdAmbisonicX = 39,
        /// <summary>
        ///  first order ambisonic channels
        /// </summary>
        SoundIoChannelIdAmbisonicY = 40,
        /// <summary>
        ///  first order ambisonic channels
        /// </summary>
        SoundIoChannelIdAmbisonicZ = 41,
        /// <summary>
        ///  X-Y Recording
        /// </summary>
        SoundIoChannelIdXyX = 42,
        /// <summary>
        ///  X-Y Recording
        /// </summary>
        SoundIoChannelIdXyY = 43,
        /// <summary>
        /// &lt; First of the "other" channel ids
        /// </summary>
        SoundIoChannelIdHeadphonesLeft = 44,
        SoundIoChannelIdHeadphonesRight = 45,
        SoundIoChannelIdClickTrack = 46,
        SoundIoChannelIdForeignLanguage = 47,
        SoundIoChannelIdHearingImpaired = 48,
        SoundIoChannelIdNarration = 49,
        SoundIoChannelIdHaptic = 50,
        /// <summary>
        /// &lt; Last of the "other" channel ids
        /// </summary>
        SoundIoChannelIdDialogCentricMix = 51,
        SoundIoChannelIdAux = 52,
        SoundIoChannelIdAux0 = 53,
        SoundIoChannelIdAux1 = 54,
        SoundIoChannelIdAux2 = 55,
        SoundIoChannelIdAux3 = 56,
        SoundIoChannelIdAux4 = 57,
        SoundIoChannelIdAux5 = 58,
        SoundIoChannelIdAux6 = 59,
        SoundIoChannelIdAux7 = 60,
        SoundIoChannelIdAux8 = 61,
        SoundIoChannelIdAux9 = 62,
        SoundIoChannelIdAux10 = 63,
        SoundIoChannelIdAux11 = 64,
        SoundIoChannelIdAux12 = 65,
        SoundIoChannelIdAux13 = 66,
        SoundIoChannelIdAux14 = 67,
        SoundIoChannelIdAux15 = 68,
    }

    public enum SoundIoBackend : int
    {
        SoundIoBackendNone = 0,
        SoundIoBackendJack = 1,
        SoundIoBackendPulseAudio = 2,
        SoundIoBackendAlsa = 3,
        SoundIoBackendCoreAudio = 4,
        SoundIoBackendWasapi = 5,
        SoundIoBackendDummy = 6,
    }

    public enum SoundIoDeviceAim : int
    {
        /// <summary>
        /// &lt; capture / recording
        /// </summary>
        SoundIoDeviceAimInput = 0,
        /// <summary>
        /// &lt; playback
        /// </summary>
        SoundIoDeviceAimOutput = 1,
    }

    /// <summary>
    ///  For your convenience, Native Endian and Foreign Endian constants are defined
    ///  which point to the respective SoundIoFormat values.
    /// </summary>
    public enum SoundIoFormat : int
    {
        SoundIoFormatInvalid = 0,
        /// <summary>
        /// &lt; Signed 8 bit
        /// </summary>
        SoundIoFormatS8 = 1,
        /// <summary>
        /// &lt; Unsigned 8 bit
        /// </summary>
        SoundIoFormatU8 = 2,
        /// <summary>
        /// &lt; Signed 16 bit Little Endian
        /// </summary>
        SoundIoFormatS16LE = 3,
        /// <summary>
        /// &lt; Signed 16 bit Big Endian
        /// </summary>
        SoundIoFormatS16BE = 4,
        /// <summary>
        /// &lt; Unsigned 16 bit Little Endian
        /// </summary>
        SoundIoFormatU16LE = 5,
        /// <summary>
        /// &lt; Unsigned 16 bit Big Endian
        /// </summary>
        SoundIoFormatU16BE = 6,
        /// <summary>
        /// &lt; Signed 24 bit Little Endian using low three bytes in 32-bit word
        /// </summary>
        SoundIoFormatS24LE = 7,
        /// <summary>
        /// &lt; Signed 24 bit Big Endian using low three bytes in 32-bit word
        /// </summary>
        SoundIoFormatS24BE = 8,
        /// <summary>
        /// &lt; Unsigned 24 bit Little Endian using low three bytes in 32-bit word
        /// </summary>
        SoundIoFormatU24LE = 9,
        /// <summary>
        /// &lt; Unsigned 24 bit Big Endian using low three bytes in 32-bit word
        /// </summary>
        SoundIoFormatU24BE = 10,
        /// <summary>
        /// &lt; Signed 32 bit Little Endian
        /// </summary>
        SoundIoFormatS32LE = 11,
        /// <summary>
        /// &lt; Signed 32 bit Big Endian
        /// </summary>
        SoundIoFormatS32BE = 12,
        /// <summary>
        /// &lt; Unsigned 32 bit Little Endian
        /// </summary>
        SoundIoFormatU32LE = 13,
        /// <summary>
        /// &lt; Unsigned 32 bit Big Endian
        /// </summary>
        SoundIoFormatU32BE = 14,
        /// <summary>
        /// &lt; Float 32 bit Little Endian, Range -1.0 to 1.0
        /// </summary>
        SoundIoFormatFloat32LE = 15,
        /// <summary>
        /// &lt; Float 32 bit Big Endian, Range -1.0 to 1.0
        /// </summary>
        SoundIoFormatFloat32BE = 16,
        /// <summary>
        /// &lt; Float 64 bit Little Endian, Range -1.0 to 1.0
        /// </summary>
        SoundIoFormatFloat64LE = 17,
        /// <summary>
        /// &lt; Float 64 bit Big Endian, Range -1.0 to 1.0
        /// </summary>
        SoundIoFormatFloat64BE = 18,
    }
    
    public enum SoundIoError {
        SoundIoErrorNone,
        /// <summary>
        /// Out of memory.
        /// </summary>
        SoundIoErrorNoMem,
        /// <summary>
        /// The backend does not appear to be active or running.
        /// </summary>
        SoundIoErrorInitAudioBackend,
        /// <summary>
        /// A system resource other than memory was not available.
        /// </summary>
        SoundIoErrorSystemResources,
        /// <summary>
        /// Attempted to open a device and failed.
        /// </summary>
        SoundIoErrorOpeningDevice,
        SoundIoErrorNoSuchDevice,
        /// <summary>
        /// The programmer did not comply with the API.
        /// </summary>
        SoundIoErrorInvalid,
       /// <summary>
       /// libsoundio was compiled without support for that backend.
       /// </summary>
        SoundIoErrorBackendUnavailable,
        /// <summary>
        /// An open stream had an error that can only be recovered from by
        /// destroying the stream and creating it again.
        /// </summary>
        SoundIoErrorStreaming,
        /// <summary>
        /// Attempted to use a device with parameters it cannot support.
        /// </summary>
        SoundIoErrorIncompatibleDevice,
        /// <summary>
        /// When JACK returns `JackNoSuchClient`
        /// </summary>
        SoundIoErrorNoSuchClient,
        /// <summary>
        /// Attempted to use parameters that the backend cannot support.
        /// </summary>
        SoundIoErrorIncompatibleBackend,
        /// <summary>
        /// Backend server shutdown or became inactive.
        /// </summary>
        SoundIoErrorBackendDisconnected,
        SoundIoErrorInterrupted,
        /// <summary>
        /// Buffer underrun occurred.
        /// </summary>
        SoundIoErrorUnderflow,
        /// <summary>
        /// Unable to convert to or from UTF-8 to the native string format.
        /// </summary>
        SoundIoErrorEncodingString,
    }
}
