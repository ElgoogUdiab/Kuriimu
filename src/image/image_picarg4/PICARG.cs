﻿using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Kontract.Image;
using Kontract.Image.Format;
using Kontract.Image.Swizzle;
using Kontract.IO;
using System.Text;

namespace image_picarg4
{
    public class PICARG
    {
        public List<Bitmap> bmps = new List<Bitmap>();
        public ImageSettings settings;

        string magic;
        Header header;

        public PICARG(Stream input)
        {
            using (var br = new BinaryReaderX(input))
            {
                //Header
                magic = br.ReadString(7);
                header = br.ReadStruct<Header>();

                //Image
                settings = new ImageSettings
                {
                    Width = header.width,
                    Height = header.height,
                    Format = new RGBA(5, 5, 5, 1),
                    Swizzle = new CTRSwizzle(header.width, header.height)
                };
                bmps.Add(Common.Load(br.ReadBytes((int)br.BaseStream.Length - 0x10), settings));
            }
        }

        public void Save(string filename)
        {
            using (var bw = new BinaryWriterX(File.Create(filename)))
            {
                //Header
                header.width = (short)bmps[0].Width;
                header.height = (short)bmps[0].Height;
                bw.Write(Encoding.ASCII.GetBytes(magic));
                bw.WriteStruct(header);

                //Image
                var settings = new ImageSettings
                {
                    Width = bmps[0].Width,
                    Height = bmps[0].Height,
                    Format = new RGBA(5, 5, 5, 1),
                    Swizzle = new CTRSwizzle(bmps[0].Width, bmps[0].Height)
                };
                bw.Write(Common.Save(bmps[0], settings));
            }
        }
    }
}
