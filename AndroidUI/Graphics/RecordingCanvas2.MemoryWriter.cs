using AndroidUI.Extensions;
using SkiaSharp;
using System.Text;

namespace AndroidUI.Graphics
{
    public partial class RecordingCanvas2
    {
        public class MemoryReader : BinaryReader
        {
            public long Position { get => BaseStream.Position; set => BaseStream.Position = value; }
            public long Length => BaseStream.Length;

            public MemoryReader(Stream input) : base(input)
            {
            }

            public MemoryReader(Stream input, bool leaveOpen) : base(input, Encoding.UTF8, leaveOpen)
            {
            }

            public ReadOnlySpan<byte> ReadByteSpan()
            {
                int length = ReadInt32();
                Span<byte> span = new byte[length];
                Read(span);
                return span;
            }

            public SKData ReadSKData()
            {
                return SKData.CreateCopy(ReadByteSpan());
            }

            /// <returns>true if the object is null</returns>
            public bool ReadNullable()
            {
                return !ReadBoolean();
            }

            public SKColorSpace ReadSKColorSpace()
            {
                if (ReadNullable()) return null;
                using SKData data = ReadSKData();
                return SKColorSpace.Deserialize(data);
            }

            public SKImageInfo ReadSKImageInfo()
            {
                int w = ReadInt32();
                int h = ReadInt32();
                SKColorType colorType = (SKColorType)ReadByte();
                SKAlphaType alphaType = (SKAlphaType)ReadByte();
                SKColorSpace colorspace = ReadSKColorSpace();
                return new SKImageInfo(w, h, colorType, alphaType, colorspace);
            }

            public SKImage ReadSKImage()
            {
                // if image or pixels are null, return null
                if (ReadNullable() || ReadNullable()) return null;

                bool IsTexureBacked = ReadBoolean();
                SKImageInfo imageInfo = ReadSKImageInfo();
                using SKData pixelData = ReadSKData();

                SKImage raster = SKImage.FromPixelCopy(imageInfo, pixelData.Data);
                // TODO: somehow obtain a GRContext to convert raster back to texture
                // TODO: a GR context might not always be available when reading data
                // TODO: the GR context may differ than what was serialized
                // TODO: we may serialize from Vulkan and read on GL and vice versa
                return raster;
            }

            public SKTypeface ReadSKTypeface()
            {
                if (ReadNullable()) return null;
                using var data = ReadSKData();
                return SKTypeface.Deserialize(data);
            }

            public SKRect ReadSKRect()
            {
                return new(
                    ReadSingle(),
                    ReadSingle(),
                    ReadSingle(),
                    ReadSingle()
                );
            }

            public SKRectI ReadSKRectI()
            {
                return new(
                    ReadInt32(),
                    ReadInt32(),
                    ReadInt32(),
                    ReadInt32()
                );
            }

            public SKPoint ReadSKPoint()
            {
                return new(ReadSingle(), ReadSingle());
            }

            public SKPointI ReadSKPointI()
            {
                return new(ReadInt32(), ReadInt32());
            }

            public SKPoint3 ReadSKPoint3()
            {
                return new(ReadSingle(), ReadSingle(), ReadSingle());
            }

            public SKRoundRect ReadSKRoundRect()
            {
                if (ReadNullable()) return null;
                SKRoundRect roundRect = new();

                roundRect.SetRectRadii(ReadSKRect(), ReadSKPointArray());

                return roundRect;
            }

            public SKColor ReadSKColor()
            {
                return new SKColor(ReadUInt32());
            }

            public SKColorF ReadSKColorF()
            {
                return ReadUInt32().ToSKColorF();
            }

            public SKColorFilter ReadSKColorFilter()
            {
                if (ReadNullable()) return null;
                using var data = ReadSKData();
                return SKColorFilter.Deserialize(data);
            }

            public SKDrawable ReadSKDrawable()
            {
                if (ReadNullable()) return null;
                using var data = ReadSKData();
                return SKDrawable.Deserialize(data);
            }

            public SKImageFilter ReadSKImageFilter()
            {
                if (ReadNullable()) return null;
                using var data = ReadSKData();
                return SKImageFilter.Deserialize(data);
            }

            public SKMaskFilter ReadSKMaskFilter()
            {
                if (ReadNullable()) return null;
                using var data = ReadSKData();
                return SKMaskFilter.Deserialize(data);
            }

            public SKPath ReadSKPath()
            {
                if (ReadNullable()) return null;
                using var data = ReadSKData();
                return SKPath.Deserialize(data);
            }

            public SKPathEffect ReadSKPathEffect()
            {
                if (ReadNullable()) return null;
                using var data = ReadSKData();
                return SKPathEffect.Deserialize(data);
            }

            public SKPicture ReadSKPicture()
            {
                if (ReadNullable()) return null;
                using var data = ReadSKData();
                return SKPicture.Deserialize(data);
            }

            public SKRegion ReadSKRegion()
            {
                if (ReadNullable()) return null;
                using var data = ReadSKData();
                return SKRegion.Deserialize(data);
            }

            public SKShader ReadSKShader()
            {
                if (ReadNullable()) return null;
                using var data = ReadSKData();
                return SKShader.Deserialize(data);
            }

            public SKTextBlob ReadSKTextBlob()
            {
                if (ReadNullable()) return null;
                using var data = ReadSKData();
                return SKTextBlob.Deserialize(data);
            }

            public SKFont ReadSKFont()
            {
                if (ReadNullable()) return null;
                SKFont font = new(ReadSKTypeface(), ReadSingle(), ReadSingle(), ReadSingle());
                font.BaselineSnap = ReadBoolean();
                font.Edging = (SKFontEdging)ReadByte();
                font.EmbeddedBitmaps = ReadBoolean();
                font.Embolden = ReadBoolean();
                font.ForceAutoHinting = ReadBoolean();
                font.Hinting = (SKFontHinting)ReadByte();
                font.LinearMetrics = ReadBoolean();
                font.Subpixel = ReadBoolean();
                return font;
            }

            public SKPaint ReadSKPaint()
            {
                if (ReadNullable()) return null;
                var font = ReadSKFont();
                SKPaint paint = font == null ? new() : new(font);
                paint.BlendMode = (SKBlendMode)ReadByte();
                paint.ColorF = ReadSKColorF();
                paint.ColorFilter = ReadSKColorFilter();
                paint.FakeBoldText = ReadBoolean();
                paint.FilterQuality = (SKFilterQuality)ReadByte();
                paint.HintingLevel = (SKPaintHinting)ReadByte();
                paint.ImageFilter = ReadSKImageFilter();
                paint.IsAntialias = ReadBoolean();
                paint.IsAutohinted = ReadBoolean();
                paint.IsDither = ReadBoolean();
                paint.IsEmbeddedBitmapText = ReadBoolean();
                paint.IsLinearText = ReadBoolean();
                paint.IsStroke = ReadBoolean();
                paint.LcdRenderText = ReadBoolean();
                paint.MaskFilter = ReadSKMaskFilter();
                paint.PathEffect = ReadSKPathEffect();
                paint.Shader = ReadSKShader();
                paint.StrokeCap = (SKStrokeCap)ReadByte();
                paint.StrokeJoin = (SKStrokeJoin)ReadByte();
                paint.StrokeMiter = ReadSingle();
                paint.StrokeWidth = ReadSingle();
                paint.Style = (SKPaintStyle)ReadByte();
                paint.SubpixelText = ReadBoolean();
                paint.TextAlign = (SKTextAlign)ReadByte();
                paint.TextEncoding = (SKTextEncoding)ReadByte();
                paint.TextScaleX = ReadSingle();
                paint.TextSize = ReadSingle();
                paint.TextSkewX = ReadSingle();
                return paint;
            }

            public SKMatrix ReadSKMatrix()
            {
                return new SKMatrix(
                    ReadSingle(), ReadSingle(), ReadSingle(),
                    ReadSingle(), ReadSingle(), ReadSingle(),
                    ReadSingle(), ReadSingle(), ReadSingle()
                );
            }

            public SKRotationScaleMatrix ReadSKRotationScaleMatrix()
            {
                return new(
                    ReadSingle(),
                    ReadSingle(),
                    ReadSingle(),
                    ReadSingle()
                );
            }

            public SKLattice ReadSKLattice()
            {
                SKLattice lattice = new();

                if (ReadNullable())
                {
                    lattice.Bounds = null;
                }
                else
                {
                    lattice.Bounds = ReadSKRectI();
                }

                lattice.Colors = ReadSKColorArray();
                lattice.XDivs = ReadIntArray();
                lattice.YDivs = ReadIntArray();
                lattice.RectTypes = ReadEnumArray<SKLatticeRectType>();
                return lattice;
            }

            public E[] ReadEnumArray<E>() where E : struct, Enum
            {
                if (ReadNullable()) return null;
                E[] array = new E[ReadInt32()];
                for (int i = 0; i < array.Length; i++)
                {
                    byte b = ReadByte();
                    array[i] = System.Runtime.CompilerServices.Unsafe.As<byte, E>(ref b);
                }
                return array;
            }

            public ushort[] ReadUShortArray()
            {
                if (ReadNullable()) return null;
                ushort[] shorts = new ushort[ReadInt32()];
                for (int i = 0; i < shorts.Length; i++)
                {
                    shorts[i] = ReadUInt16();
                }
                return shorts;
            }

            public short[] ReadShortArray()
            {
                if (ReadNullable()) return null;
                short[] shorts = new short[ReadInt32()];
                for (int i = 0; i < shorts.Length; i++)
                {
                    shorts[i] = ReadInt16();
                }
                return shorts;
            }

            public uint[] ReadUIntArray()
            {
                if (ReadNullable()) return null;
                uint[] ints = new uint[ReadInt32()];
                for (int i = 0; i < ints.Length; i++)
                {
                    ints[i] = ReadUInt32();
                }
                return ints;
            }

            public int[] ReadIntArray()
            {
                if (ReadNullable()) return null;
                int[] ints = new int[ReadInt32()];
                for (int i = 0; i < ints.Length; i++)
                {
                    ints[i] = ReadInt32();
                }
                return ints;
            }

            public SKColor[] ReadSKColorArray()
            {
                if (ReadNullable()) return null;
                SKColor[] colors = new SKColor[ReadInt32()];
                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i] = ReadSKColor();
                }
                return colors;
            }

            public SKPoint[] ReadSKPointArray()
            {
                if (ReadNullable()) return null;
                SKPoint[] points = new SKPoint[ReadInt32()];
                for (int i = 0; i < points.Length; i++)
                {
                    points[i] = ReadSKPoint();
                }
                return points;
            }

            public SKRect[] ReadSKRectArray()
            {
                if (ReadNullable()) return null;
                SKRect[] rects = new SKRect[ReadInt32()];
                for (int i = 0; i < rects.Length; i++)
                {
                    rects[i] = ReadSKRect();
                }
                return rects;
            }

            public SKRotationScaleMatrix[] ReadSKRotationScaleMatrixArray()
            {
                if (ReadNullable()) return null;
                SKRotationScaleMatrix[] rotationScaleMatrices = new SKRotationScaleMatrix[ReadInt32()];
                for (int i = 0; i < rotationScaleMatrices.Length; i++)
                {
                    rotationScaleMatrices[i] = ReadSKRotationScaleMatrix();
                }
                return rotationScaleMatrices;
            }

            public SKVertices ReadSKVertices()
            {
                if (ReadNullable()) return null;
                return SKVertices.CreateCopy(
                    (SKVertexMode)ReadByte(),
                    ReadSKPointArray(),
                    ReadSKPointArray(),
                    ReadSKColorArray(),
                    ReadUShortArray()
                );
            }
        }

        public class MemoryWriter : BinaryWriter
        {
            public long Position { get => BaseStream.Position; set => BaseStream.Position = value; }
            public long Length => BaseStream.Length;

            public MemoryWriter(Stream output) : base(output)
            {
            }

            public MemoryWriter(Stream output, bool leaveOpen) : base(output, Encoding.UTF8, leaveOpen)
            {
            }

            public MemoryWriter() : this(new MemoryStream(), false)
            {
            }

            /// <returns>true if the object is not null</returns>
            public bool WriteNullable<T>(T obj) where T : class
            {
                if (obj == null)
                {
                    Write(false);
                    return false;
                }
                Write(true);
                return true;
            }

            /// <returns>true if the object is not null</returns>
            public bool WriteNullable<T>(T? obj) where T : struct
            {
                if (obj == null)
                {
                    Write(false);
                    return false;
                }
                Write(true);
                return true;
            }

            /// <returns>true if the object is not null</returns>
            public unsafe bool WriteNullable(void* obj)
            {
                if (obj == IntPtr.Zero.ToPointer())
                {
                    Write(false);
                    return false;
                }
                Write(true);
                return true;
            }

            public void WriteByteSpan(ReadOnlySpan<byte> span)
            {
                Write(span.Length);
                Write(span);
            }

            public void WriteSKData(SKData data)
            {
                WriteByteSpan(data.Span);
            }

            public void WriteSKData(SKData data, bool dispose)
            {
                WriteSKData(data);
                if (dispose) data.Dispose();
            }

            public void WriteSKColorSpace(SKColorSpace colorSpace)
            {
                if (WriteNullable(colorSpace))
                {
                    WriteSKData(colorSpace.Serialize(), true);
                }
            }

            public void WriteSKImageInfo(SKImageInfo imageInfo)
            {
                Write(imageInfo.Width);
                Write(imageInfo.Height);
                Write((byte)imageInfo.ColorType);
                Write((byte)imageInfo.AlphaType);
                WriteSKColorSpace(imageInfo.ColorSpace);
            }

            public void WriteSKImage(SKImage image)
            {
                if (WriteNullable(image))
                {
                    SKImage raster = image;
                    if (image.IsTextureBacked)
                    {
                        raster = image.ToRasterImage();
                    }
                    var pixmap = new SKPixmap();
                    if (!raster.PeekPixels(pixmap))
                    {
                        pixmap.Dispose();
                        pixmap = null;
                    }
                    if (WriteNullable(pixmap))
                    {
                        Write(image.IsTextureBacked);
                        WriteSKImageInfo(image.Info);
                        IntPtr pixels = pixmap.GetPixels();
                        WriteSKData(SKData.Create(pixels, pixmap.BytesSize), true);
                        pixmap.Dispose();
                    }
                    if (image.IsTextureBacked)
                    {
                        raster.Dispose();
                    }
                }
            }

            public void WriteSKTypeface(SKTypeface typeface)
            {
                if (WriteNullable(typeface))
                {
                    WriteSKData(typeface.Serialize(), true);
                }
            }

            public void WriteSKRect(SKRect rect)
            {
                Write(rect.Left);
                Write(rect.Top);
                Write(rect.Right);
                Write(rect.Bottom);
            }

            public void WriteSKRectI(SKRectI rect)
            {
                Write(rect.Left);
                Write(rect.Top);
                Write(rect.Right);
                Write(rect.Bottom);
            }

            public void WriteSKPoint(SKPoint point)
            {
                Write(point.X);
                Write(point.Y);
            }

            public void WriteSKPointI(SKPointI point)
            {
                Write(point.X);
                Write(point.Y);
            }

            public void WriteSKPoint3(SKPoint3 point)
            {
                Write(point.X);
                Write(point.Y);
                Write(point.Z);
            }

            public void WriteSKRoundRect(SKRoundRect rect)
            {
                if (WriteNullable(rect))
                {
                    WriteSKRect(rect.Rect);
                    WriteSKPointArray(rect.Radii);
                }
            }

            public void WriteSKColor(SKColor color)
            {
                Write((uint)color);
            }

            public void WriteSKColorF(SKColorF color)
            {
                WriteSKColor(color.ToSKColor());
            }

            public void WriteSKColorFilter(SKColorFilter colorFilter)
            {
                if (WriteNullable(colorFilter))
                {
                    WriteSKData(colorFilter.Serialize(), true);
                }
            }

            public void WriteSKDrawable(SKDrawable drawable)
            {
                if (WriteNullable(drawable))
                {
                    WriteSKData(drawable.Serialize(), true);
                }
            }

            public void WriteSKImageFilter(SKImageFilter imageFilter)
            {
                if (WriteNullable(imageFilter))
                {
                    WriteSKData(imageFilter.Serialize(), true);
                }
            }

            public void WriteSKMaskFilter(SKMaskFilter maskFilter)
            {
                if (WriteNullable(maskFilter))
                {
                    WriteSKData(maskFilter.Serialize(), true);
                }
            }

            public void WriteSKPath(SKPath path)
            {
                if (WriteNullable(path))
                {
                    WriteSKData(path.Serialize(), true);
                }
            }

            public void WriteSKPathEffect(SKPathEffect pathEffect)
            {
                if (WriteNullable(pathEffect))
                {
                    WriteSKData(pathEffect.Serialize(), true);
                }
            }

            public void WriteSKPicture(SKPicture picture)
            {
                if (WriteNullable(picture))
                {
                    WriteSKData(picture.Serialize(), true);
                }
            }

            public void WriteSKRegion(SKRegion region)
            {
                if (WriteNullable(region))
                {
                    WriteSKData(region.Serialize(), true);
                }
            }

            public void WriteSKShader(SKShader shader)
            {
                if (WriteNullable(shader))
                {
                    WriteSKData(shader.Serialize(), true);
                }
            }

            public void WriteSKTextBlob(SKTextBlob blob)
            {
                if (WriteNullable(blob))
                {
                    WriteSKData(blob.Serialize(), true);
                }
            }

            public void WriteSKFont(SKFont font)
            {
                if (WriteNullable(font))
                {
                    WriteSKTypeface(font.Typeface);
                    Write(font.Size);
                    Write(font.ScaleX);
                    Write(font.SkewX);
                    Write(font.BaselineSnap);
                    Write((byte)font.Edging);
                    Write(font.EmbeddedBitmaps);
                    Write(font.Embolden);
                    Write(font.ForceAutoHinting);
                    Write((byte)font.Hinting);
                    Write(font.LinearMetrics);
                    Write(font.Subpixel);
                }
            }

            public void WriteSKPaint(SKPaint paint)
            {
                if (WriteNullable(paint))
                {
                    WriteSKFont(paint.ToFont());
                    Write((byte)paint.BlendMode);
                    WriteSKColorF(paint.ColorF);
                    WriteSKColorFilter(paint.ColorFilter);
                    Write(paint.FakeBoldText);
                    Write((byte)paint.FilterQuality);
                    Write((byte)paint.HintingLevel);
                    WriteSKImageFilter(paint.ImageFilter);
                    Write(paint.IsAntialias);
                    Write(paint.IsAutohinted);
                    Write(paint.IsDither);
                    Write(paint.IsEmbeddedBitmapText);
                    Write(paint.IsLinearText);
                    Write(paint.IsStroke);
                    Write(paint.LcdRenderText);
                    WriteSKMaskFilter(paint.MaskFilter);
                    WriteSKPathEffect(paint.PathEffect);
                    WriteSKShader(paint.Shader);
                    Write((byte)paint.StrokeCap);
                    Write((byte)paint.StrokeJoin);
                    Write(paint.StrokeMiter);
                    Write(paint.StrokeWidth);
                    Write((byte)paint.Style);
                    Write(paint.SubpixelText);
                    Write((byte)paint.TextAlign);
                    Write((byte)paint.TextEncoding);
                    Write(paint.TextScaleX);
                    Write(paint.TextSize);
                    Write(paint.TextSkewX);
                }
            }

            public void WriteSKMatrix(ref SKMatrix m)
            {
                Write(m.ScaleX);
                Write(m.SkewX);
                Write(m.TransX);
                Write(m.SkewY);
                Write(m.ScaleY);
                Write(m.TransY);
                Write(m.Persp0);
                Write(m.Persp1);
                Write(m.Persp2);
            }

            public void WriteSKRotationScaleMatrix(SKRotationScaleMatrix rotationScaleMatrix)
            {
                Write(rotationScaleMatrix.SCos);
                Write(rotationScaleMatrix.SSin);
                Write(rotationScaleMatrix.TX);
                Write(rotationScaleMatrix.TY);
            }

            public void WriteSKLattice(SKLattice lattice)
            {
                if (WriteNullable(lattice.Bounds))
                {
                    WriteSKRectI(lattice.Bounds.Value);
                }

                WriteSKColorArray(lattice.Colors);
                WriteIntArray(lattice.XDivs);
                WriteIntArray(lattice.YDivs);
                WriteEnumArray(lattice.RectTypes);
            }

            public void WriteEnumArray<E>(E[] array) where E : struct, Enum
            {
                if (WriteNullable(array))
                {
                    Write(array.Length);
                    for (int i = 0; i < array.Length; i++)
                    {
                        E e = array[i];
                        byte b = System.Runtime.CompilerServices.Unsafe.As<E, byte>(ref e);
                        Write(b);
                    }
                }
            }

            public void WriteUShortArray(ushort[] shorts)
            {
                if (WriteNullable(shorts))
                {
                    Write(shorts.Length);
                    for (int i = 0; i < shorts.Length; i++)
                    {
                        Write(shorts[i]);
                    }
                }
            }

            public void WriteShortArray(short[] shorts)
            {
                if (WriteNullable(shorts))
                {
                    Write(shorts.Length);
                    for (int i = 0; i < shorts.Length; i++)
                    {
                        Write(shorts[i]);
                    }
                }
            }

            public void WriteUIntArray(uint[] ints)
            {
                if (WriteNullable(ints))
                {
                    Write(ints.Length);
                    for (int i = 0; i < ints.Length; i++)
                    {
                        Write(ints[i]);
                    }
                }
            }

            public void WriteIntArray(int[] ints)
            {
                if (WriteNullable(ints))
                {
                    Write(ints.Length);
                    for (int i = 0; i < ints.Length; i++)
                    {
                        Write(ints[i]);
                    }
                }
            }

            public void WriteSKColorArray(SKColor[] colors)
            {
                if (WriteNullable(colors))
                {
                    Write(colors.Length);
                    for (int i = 0; i < colors.Length; i++)
                    {
                        WriteSKColor(colors[i]);
                    }
                }
            }
            public void WriteSKPointArray(SKPoint[] points)
            {
                if (WriteNullable(points))
                {
                    Write(points.Length);
                    for (int i = 0; i < points.Length; i++)
                    {
                        WriteSKPoint(points[i]);
                    }
                }
            }

            public void WriteSKRectArray(SKRect[] rects)
            {
                if (WriteNullable(rects))
                {
                    Write(rects.Length);
                    for (int i = 0; i < rects.Length; i++)
                    {
                        WriteSKRect(rects[i]);
                    }
                }
            }

            public void WriteSKSKRotationScaleMatrixArray(SKRotationScaleMatrix[] rotationScaleMatrices)
            {
                if (WriteNullable(rotationScaleMatrices))
                {
                    Write(rotationScaleMatrices.Length);
                    for (int i = 0; i < rotationScaleMatrices.Length; i++)
                    {
                        WriteSKRotationScaleMatrix(rotationScaleMatrices[i]);
                    }
                }
            }

            public void WriteSKVertices(SKVertices vertices)
            {
                if (WriteNullable(vertices))
                {
                    Write((byte)vertices.VMode_FOR_SERIALIZATION);
                    WriteSKPointArray(vertices.Positions_FOR_SERIALIZATION);
                    WriteSKPointArray(vertices.TexCoords_FOR_SERIALIZATION);
                    WriteSKColorArray(vertices.Colors_FOR_SERIALIZATION);
                    WriteUShortArray(vertices.Indices_FOR_SERIALIZATION);
                }
            }
        }
    }
}