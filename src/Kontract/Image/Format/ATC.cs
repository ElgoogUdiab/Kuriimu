﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Kontract.Interface;
using Kontract.IO;

namespace Kontract.Image.Format
{
    public class ATC : IImageFormat
    {
        public int BitDepth { get; set; }
        public int BlockBitDepth { get; set; }
        public string FormatName { get; set; }

        bool alpha;
        ByteOrder byteOrder;

        public ATC(bool alpha = false, ByteOrder byteOrder = ByteOrder.LittleEndian)
        {
            BitDepth = (alpha) ? 8 : 4;
            BlockBitDepth = (alpha) ? 128 : 64;

            this.alpha = alpha;
            this.byteOrder = byteOrder;

            FormatName = "ATC_RGB" + ((alpha) ? "A" : "");
        }

        public IEnumerable<Color> Load(byte[] tex)
        {
            using (var br = new BinaryReaderX(new MemoryStream(tex), byteOrder))
            {
                var atcdecoder = new Support.ATC.Decoder(alpha);

                while (true)
                {
                    yield return atcdecoder.Get(() =>
                    {
                        var Alpha = alpha ? br.ReadUInt64() : 0;
                        return (Alpha, br.ReadUInt64());
                    });
                }
            }
        }

        public byte[] Save(IEnumerable<Color> colors)
        {
            var atcencoder = new Support.ATC.Encoder(alpha);

            var ms = new MemoryStream();
            using (var bw = new BinaryWriterX(ms))
                foreach (var color in colors)
                    atcencoder.Set(color, data =>
                    {
                        if (alpha) bw.Write(data.alpha);
                        bw.Write(data.block);
                    });

            return ms.ToArray();
        }
    }
}
