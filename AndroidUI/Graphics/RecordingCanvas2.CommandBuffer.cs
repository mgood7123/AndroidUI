using AndroidUI.Utils;
using SkiaSharp;

namespace AndroidUI.Graphics
{
    public partial class RecordingCanvas2
    {
        public class CommandBuffer : Disposable
        {
            internal MemoryReader mem;

            public CommandBuffer(MemoryReader reader)
            {
                this.mem = reader;
            }

            protected override void OnDispose()
            {
                mem.Dispose();
                base.OnDispose();
            }

            internal void PrepareMem()
            {
                mem.Position = 0;
            }

            public void Playback(SKCanvas canvas)
            {
                if (canvas == null)
                {
                    throw new ArgumentNullException(nameof(canvas));
                }

                mem.Position = 0;
                while (mem.Position != mem.Length)
                {
                    COMMANDS command = (COMMANDS)mem.ReadByte();
                    switch (command)
                    {
                        case COMMANDS.CLEAR:
                            canvas.Clear(mem.ReadSKColor());
                            break;
                        case COMMANDS.CLEARF:
                            canvas.Clear(mem.ReadSKColorF());
                            break;
                        case COMMANDS.CLIP_PATH:
                            {
                                SKPath path = mem.ReadSKPath();
                                canvas.ClipPath(
                                    path,
                                    (SKClipOperation)mem.ReadByte(),
                                    mem.ReadBoolean()
                                );
                                path?.Dispose();
                                break;
                            }
                        case COMMANDS.CLIP_RECT:
                            canvas.ClipRect(
                                mem.ReadSKRect(),
                                (SKClipOperation)mem.ReadByte(),
                                mem.ReadBoolean()
                            );
                            break;
                        case COMMANDS.CLIP_ROUND_RECT:
                            {
                                SKRoundRect rect = mem.ReadSKRoundRect();
                                canvas.ClipRoundRect(
                                    rect,
                                    (SKClipOperation)mem.ReadByte(),
                                    mem.ReadBoolean()
                                );
                                rect?.Dispose();
                                break;
                            }
                        case COMMANDS.CLIP_REGION:
                            {
                                SKRegion region = mem.ReadSKRegion();
                                canvas.ClipRegion(
                                    region,
                                    (SKClipOperation)mem.ReadByte()
                                );
                                region?.Dispose();
                                break;
                            }
                        case COMMANDS.CONCAT:
                            {
                                SKMatrix m = mem.ReadSKMatrix();
                                canvas.Concat(ref m);
                                break;
                            }
                        case COMMANDS.DRAW_ANNOTATION:
                            {
                                var rect = mem.ReadSKRect();
                                var key = mem.ReadString();
                                var value = mem.ReadSKData();
                                canvas.DrawAnnotation(rect, key, value);
                                value?.Dispose();
                                break;
                            }
                        case COMMANDS.DRAW_ARC:
                            {
                                var oval = mem.ReadSKRect();
                                var startAngle = mem.ReadSingle();
                                var sweepAngle = mem.ReadSingle();
                                var useCenter = mem.ReadBoolean();
                                var paint = mem.ReadSKPaint();
                                canvas.DrawArc(oval, startAngle, sweepAngle, useCenter, paint);
                                paint?.Dispose();
                                break;
                            }

                        case COMMANDS.DRAW_ATLAS:
                            {
                                var atlas = mem.ReadSKImage();
                                SKRect[] sprites = mem.ReadSKRectArray();
                                SKRotationScaleMatrix[] transforms = mem.ReadSKRotationScaleMatrixArray();
                                SKColor[] colors = mem.ReadSKColorArray();
                                var mode = (SKBlendMode)mem.ReadByte();

                                SKRect? cullRect;
                                if (mem.ReadNullable())
                                {
                                    cullRect = null;
                                }
                                else
                                {
                                    cullRect = mem.ReadSKRect();
                                }

                                var paint = mem.ReadSKPaint();

                                if (cullRect == null)
                                {
                                    canvas.DrawAtlas(atlas, sprites, transforms, colors, mode, paint);
                                }
                                else
                                {
                                    canvas.DrawAtlas(atlas, sprites, transforms, colors, mode, cullRect.Value, paint);
                                }

                                atlas?.Dispose();
                                paint?.Dispose();

                                break;
                            }
                        case COMMANDS.DRAW_CIRCLE:
                            {
                                var cx = mem.ReadSingle();
                                var cy = mem.ReadSingle();
                                var radius = mem.ReadSingle();
                                var paint = mem.ReadSKPaint();
                                canvas.DrawCircle(cx, cy, radius, paint);
                                paint?.Dispose();
                                break;
                            }
                        case COMMANDS.DRAW_COLOR:
                            canvas.DrawColor(
                                mem.ReadSKColor(),
                                (SKBlendMode)mem.ReadByte()
                            );
                            break;
                        case COMMANDS.DRAW_COLORF:
                            canvas.DrawColor(
                                mem.ReadSKColorF(),
                                (SKBlendMode)mem.ReadByte()
                            );
                            break;
                        case COMMANDS.DRAW_DRAWABLE:
                            {
                                var drawable = mem.ReadSKDrawable();
                                var matrix = mem.ReadSKMatrix();
                                canvas.DrawDrawable(drawable, ref matrix);
                                drawable?.Dispose();
                                break;
                            }
                        case COMMANDS.DRAW_IMAGE_SKRECT_SKPAINT:
                            {
                                var image = mem.ReadSKImage();
                                var dest = mem.ReadSKRect();
                                var paint = mem.ReadSKPaint();
                                canvas.DrawImage(image, dest, paint);
                                image?.Dispose();
                                paint?.Dispose();
                                break;
                            }
                        case COMMANDS.DRAW_IMAGE_SKRECT_SKRECT_SKPAINT:
                            {
                                var image = mem.ReadSKImage();
                                var source = mem.ReadSKRect();
                                var dest = mem.ReadSKRect();
                                var paint = mem.ReadSKPaint();
                                canvas.DrawImage(image, source, dest, paint);
                                image?.Dispose();
                                paint?.Dispose();
                                break;
                            }
                        case COMMANDS.DRAW_IMAGE_FLOAT_FLOAT_SKPAINT:
                            {
                                var image = mem.ReadSKImage();
                                float x = mem.ReadSingle();
                                float y = mem.ReadSingle();
                                var paint = mem.ReadSKPaint();
                                canvas.DrawImage(image, x, y, paint);
                                image?.Dispose();
                                paint?.Dispose();
                                break;
                            }
                        case COMMANDS.DRAW_IMAGE_LATTICE:
                            {
                                var image = mem.ReadSKImage();
                                var lattice = mem.ReadSKLattice();
                                var dst = mem.ReadSKRect();
                                var paint = mem.ReadSKPaint();
                                canvas.DrawImageLattice(image, lattice, dst, paint);
                                image?.Dispose();
                                paint?.Dispose();
                                break;
                            }
                        case COMMANDS.DRAW_IMAGE_NINEPATCH:
                            {
                                var image = mem.ReadSKImage();
                                var center = mem.ReadSKRectI();
                                var dst = mem.ReadSKRect();
                                var paint = mem.ReadSKPaint();
                                canvas.DrawImageNinePatch(image, center, dst, paint);
                                image?.Dispose();
                                paint?.Dispose();
                                break;
                            }
                        case COMMANDS.DRAW_LINE:
                            {
                                var x0 = mem.ReadSingle();
                                var y0 = mem.ReadSingle();
                                var x1 = mem.ReadSingle();
                                var y1 = mem.ReadSingle();
                                var paint = mem.ReadSKPaint();
                                canvas.DrawLine(x0, y0, x1, y1, paint);
                                paint?.Dispose();
                                break;
                            }
                        case COMMANDS.DRAW_LINK_DESTINATION_ANNOTATION:
                            {
                                var rect = mem.ReadSKRect();
                                var data = mem.ReadSKData();
                                canvas.DrawLinkDestinationAnnotation(rect, data);
                                data?.Dispose();
                                break;
                            }
                        case COMMANDS.DRAW_NAMED_DESTINATION_ANNOTATION:
                            {
                                var point = mem.ReadSKPoint();
                                var data = mem.ReadSKData();
                                canvas.DrawNamedDestinationAnnotation(point, data);
                                data?.Dispose();
                                break;
                            }
                        case COMMANDS.DRAW_OVAL:
                            {
                                var rect = mem.ReadSKRect();
                                var paint = mem.ReadSKPaint();
                                canvas.DrawOval(rect, paint);
                                paint?.Dispose();
                                break;
                            }
                        case COMMANDS.DRAW_PAINT:
                            {
                                var paint = mem.ReadSKPaint();
                                canvas.DrawPaint(paint);
                                paint?.Dispose();
                                break;
                            }
                        case COMMANDS.DRAW_PATCH:
                            {
                                var cubics = mem.ReadSKPointArray();
                                var colors = mem.ReadSKColorArray();
                                var texCoords = mem.ReadSKPointArray();
                                var mode = (SKBlendMode)mem.ReadByte();
                                var paint = mem.ReadSKPaint();
                                canvas.DrawPatch(cubics, colors, texCoords, mode, paint);
                                paint?.Dispose();
                                break;
                            }
                        case COMMANDS.DRAW_PATH:
                            {
                                var path = mem.ReadSKPath();
                                var paint = mem.ReadSKPaint();
                                canvas.DrawPath(path, paint);
                                path?.Dispose();
                                paint?.Dispose();
                                break;
                            }
                        case COMMANDS.DRAW_PICTURE_WITH_MATRIX:
                            {
                                var picture = mem.ReadSKPicture();
                                var matrix = mem.ReadSKMatrix();
                                var paint = mem.ReadSKPaint();
                                canvas.DrawPicture(picture, ref matrix, paint);
                                picture?.Dispose();
                                paint?.Dispose();
                                break;
                            }
                        case COMMANDS.DRAW_PICTURE:
                            {
                                var picture = mem.ReadSKPicture();
                                var paint = mem.ReadSKPaint();
                                canvas.DrawPicture(picture, paint);
                                picture?.Dispose();
                                paint?.Dispose();
                                break;
                            }
                        case COMMANDS.DRAW_POINT:
                            {
                                var x = mem.ReadSingle();
                                var y = mem.ReadSingle();
                                var paint = mem.ReadSKPaint();
                                canvas.DrawPoint(x, y, paint);
                                paint?.Dispose();
                                break;
                            }
                        case COMMANDS.DRAW_POINTS:
                            {
                                var mode = (SKPointMode)mem.ReadByte();
                                var points = mem.ReadSKPointArray();
                                var paint = mem.ReadSKPaint();
                                canvas.DrawPoints(mode, points, paint);
                                paint?.Dispose();
                                break;
                            }
                        case COMMANDS.DRAW_RECT__XYWH:
                            {
                                var x = mem.ReadSingle();
                                var y = mem.ReadSingle();
                                var w = mem.ReadSingle();
                                var h = mem.ReadSingle();
                                var paint = mem.ReadSKPaint();
                                canvas.DrawRect(x, y, w, h, paint);
                                paint?.Dispose();
                                break;
                            }
                        case COMMANDS.DRAW_RECT__RECT:
                            {
                                var rect = mem.ReadSKRect();
                                var paint = mem.ReadSKPaint();
                                canvas.DrawRect(rect, paint);
                                paint?.Dispose();
                                break;
                            }
                        case COMMANDS.DRAW_REGION:
                            {
                                var region = mem.ReadSKRegion();
                                var paint = mem.ReadSKPaint();
                                canvas.DrawRegion(region, paint);
                                region?.Dispose();
                                paint?.Dispose();
                                break;
                            }

                        case COMMANDS.DRAW_ROUNDED_RECT__RECT_XY:
                            {
                                var rect = mem.ReadSKRect();
                                var rx = mem.ReadSingle();
                                var ry = mem.ReadSingle();
                                var paint = mem.ReadSKPaint();
                                canvas.DrawRoundRect(rect, rx, ry, paint);
                                paint?.Dispose();
                                break;
                            }
                        case COMMANDS.DRAW_ROUNDED_RECT:
                            {
                                var rect = mem.ReadSKRoundRect();
                                var paint = mem.ReadSKPaint();
                                canvas.DrawRoundRect(rect, paint);
                                rect?.Dispose();
                                paint?.Dispose();
                                break;
                            }
                        case COMMANDS.DRAW_ROUNDED_RECT_DIFFERENCE:
                            {
                                var outer = mem.ReadSKRoundRect();
                                var inner = mem.ReadSKRoundRect();
                                var paint = mem.ReadSKPaint();
                                canvas.DrawRoundRectDifference(outer, inner, paint);
                                outer?.Dispose();
                                inner?.Dispose();
                                paint?.Dispose();
                                break;
                            }
                        case COMMANDS.DRAW_TEXTBLOB:
                            {
                                var blob = mem.ReadSKTextBlob();
                                var x = mem.ReadSingle();
                                var y = mem.ReadSingle();
                                var paint = mem.ReadSKPaint();
                                canvas.DrawText(blob, x, y, paint);
                                blob?.Dispose();
                                paint?.Dispose();
                                break;
                            }
                        case COMMANDS.DRAW_URL_ANNOTATION:
                            {
                                var rect = mem.ReadSKRect();
                                var data = mem.ReadSKData();
                                canvas.DrawUrlAnnotation(rect, data);
                                data?.Dispose();
                                break;
                            }
                        case COMMANDS.DRAW_VERTICES:
                            {
                                var vertices = mem.ReadSKVertices();
                                var mode = (SKBlendMode)mem.ReadByte();
                                var paint = mem.ReadSKPaint();
                                canvas.DrawVertices(vertices, mode, paint);
                                vertices?.Dispose();
                                paint?.Dispose();
                                break;
                            }
                        case COMMANDS.DISCARD:
                            canvas.Discard();
                            break;
                        case COMMANDS.FLUSH:
                            canvas.Flush();
                            break;
                        case COMMANDS.RESET_MATRIX:
                            canvas.ResetMatrix();
                            break;
                        case COMMANDS.RESTORE:
                            canvas.Restore();
                            break;
                        case COMMANDS.RESTORE_TO_COUNT:
                            canvas.RestoreToCount(mem.ReadInt32());
                            break;
                        case COMMANDS.ROTATE_DEGREES:
                            canvas.RotateDegrees(mem.ReadSingle());
                            break;
                        case COMMANDS.ROTATE_RADIANS:
                            canvas.RotateRadians(mem.ReadSingle());
                            break;
                        case COMMANDS.SAVE:
                            canvas.Save();
                            break;
                        case COMMANDS.SAVE_LAYER:
                            {
                                var paint = mem.ReadSKPaint();
                                canvas.SaveLayer(paint);
                                paint?.Dispose();
                                break;
                            }
                        case COMMANDS.SAVE_LAYER_RECT:
                            {
                                var limit = mem.ReadSKRect();
                                var paint = mem.ReadSKPaint();
                                canvas.SaveLayer(limit, paint);
                                paint?.Dispose();
                                break;
                            }
                        case COMMANDS.SCALE:
                            canvas.Scale(mem.ReadSingle());
                            break;
                        case COMMANDS.SCALE_XY:
                            canvas.Scale(mem.ReadSingle(), mem.ReadSingle());
                            break;
                        case COMMANDS.SCALE_POINT:
                            canvas.Scale(mem.ReadSKPoint());
                            break;
                        case COMMANDS.SET_MATRIX:
                            canvas.SetMatrix(mem.ReadSKMatrix());
                            break;
                        case COMMANDS.SKEW_XY:
                            canvas.Skew(mem.ReadSingle(), mem.ReadSingle());
                            break;
                        case COMMANDS.SKEW_POINT:
                            canvas.Skew(mem.ReadSKPoint());
                            break;
                        case COMMANDS.TRANSLATE_XY:
                            canvas.Translate(mem.ReadSingle(), mem.ReadSingle());
                            break;
                        case COMMANDS.TRANSLATE_POINT:
                            canvas.Translate(mem.ReadSKPoint());
                            break;
                    }
                }
            }
        }
    }
}