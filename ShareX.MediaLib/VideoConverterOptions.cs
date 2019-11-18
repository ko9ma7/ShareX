﻿#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2019 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using ShareX.HelpersLib;
using System.IO;
using System.Text;

namespace ShareX.MediaLib
{
    public class VideoConverterOptions
    {
        public string InputFilePath { get; set; }
        public string OutputFolderPath { get; set; }
        public string OutputFileName { get; set; }

        public string OutputFilePath
        {
            get
            {
                string path = Path.Combine(OutputFolderPath, OutputFileName);
                string extension = GetFileExtension();
                return Path.ChangeExtension(path, extension);
            }
        }

        public ConverterVideoCodecs VideoCodec { get; set; } = ConverterVideoCodecs.x264;
        public int VideoQuality { get; set; } = 23;

        public string GetFFmpegArgs()
        {
            StringBuilder args = new StringBuilder();

            // Input file path
            args.Append($"-i \"{InputFilePath}\" ");

            // Video codec
            switch (VideoCodec)
            {
                case ConverterVideoCodecs.x264: // https://trac.ffmpeg.org/wiki/Encode/H.264
                    args.Append($"-c:v libx264 -preset medium -crf {VideoQuality.Clamp(0, 51)} ");
                    args.Append("-pix_fmt yuv420p -movflags +faststart "); // For browser support
                    break;
                case ConverterVideoCodecs.x265: // https://trac.ffmpeg.org/wiki/Encode/H.265
                    args.Append($"-c:v libx265 -preset medium -crf {VideoQuality.Clamp(0, 51)} ");
                    break;
                case ConverterVideoCodecs.vp8: // https://trac.ffmpeg.org/wiki/Encode/VP8
                    args.Append($"-c:v libvpx -crf {VideoQuality.Clamp(0, 63)} -b:v 0 ");
                    break;
                case ConverterVideoCodecs.vp9: // https://trac.ffmpeg.org/wiki/Encode/VP9
                    args.Append($"-c:v libvpx-vp9 -crf {VideoQuality.Clamp(0, 63)} -b:v 0 ");
                    break;
                case ConverterVideoCodecs.xvid: // https://trac.ffmpeg.org/wiki/Encode/MPEG-4
                    args.Append($"-c:v libxvid -q:v {VideoQuality.Clamp(1, 31)} ");
                    break;
                case ConverterVideoCodecs.gif: // https://ffmpeg.org/ffmpeg-filters.html#palettegen-1
                    args.Append("-lavfi \"palettegen=stats_mode=full[palette],[0:v][palette]paletteuse=dither=sierra2_4a\" ");
                    break;
                case ConverterVideoCodecs.webp: // https://www.ffmpeg.org/ffmpeg-codecs.html#libwebp
                    args.Append("-c:v libwebp -lossless 0 -preset default -loop 0 ");
                    break;
                case ConverterVideoCodecs.apng:
                    args.Append("-f apng -plays 0 ");
                    break;
            }

            // Audio codec
            switch (VideoCodec)
            {
                case ConverterVideoCodecs.x264: // https://trac.ffmpeg.org/wiki/Encode/AAC
                case ConverterVideoCodecs.x265:
                    args.Append("-c:a aac -b:a 128k ");
                    break;
                case ConverterVideoCodecs.vp8: // https://trac.ffmpeg.org/wiki/TheoraVorbisEncodingGuide
                case ConverterVideoCodecs.vp9:
                    args.Append("-c:a libvorbis -q:a 3 ");
                    break;
                case ConverterVideoCodecs.xvid: // https://trac.ffmpeg.org/wiki/Encode/MP3
                    args.Append("-c:a libmp3lame -q:a 4 ");
                    break;
            }

            // Output file path
            args.Append($"-y \"{OutputFilePath}\"");

            return args.ToString();
        }

        public string GetFileExtension()
        {
            switch (VideoCodec)
            {
                default:
                case ConverterVideoCodecs.x264:
                case ConverterVideoCodecs.x265:
                    return "mp4";
                case ConverterVideoCodecs.vp8:
                case ConverterVideoCodecs.vp9:
                    return "webm";
                case ConverterVideoCodecs.xvid:
                    return "avi";
                case ConverterVideoCodecs.gif:
                    return "gif";
                case ConverterVideoCodecs.webp:
                    return "webp";
                case ConverterVideoCodecs.apng:
                    return "apng";
            }
        }
    }
}