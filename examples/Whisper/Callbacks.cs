using System;


namespace Whisper
{

    internal delegate void whisper_new_segment_callback(whisper_context ctx, int n_new, object user_data);

    internal delegate bool whisper_encoder_begin_callback(whisper_context ctx, object user_data);

}