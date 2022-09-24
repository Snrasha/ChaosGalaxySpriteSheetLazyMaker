using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;

namespace ChaosGalaxySpriteSheetLazyMaker
{
    internal class SpriteSheetCG
    {
        private static void ReplaceColor(Graphics graphics, Bitmap sprite, Bitmap colorPalette, int from, int to,int x,int y)
        {


            // Set the image attribute's color mappings
            ColorMap[] colorMap = new ColorMap[colorPalette.Width];
            for (int j = 0; j < colorPalette.Width; j++)
            {
                colorMap[j] = new ColorMap();
                colorMap[j].OldColor = colorPalette.GetPixel(j, from);
                colorMap[j].NewColor = colorPalette.GetPixel(j, to);
            }


            ImageAttributes attr = new ImageAttributes();
            attr.SetRemapTable(colorMap);
            // Draw using the color map
            Rectangle rect = new Rectangle(x* sprite.Width, y * sprite.Height, sprite.Width, sprite.Height);
            graphics.DrawImage(sprite, rect, 0, 0, rect.Width, rect.Height, GraphicsUnit.Pixel, attr);

        }
        private static void FillingWhite(Graphics graphics, Bitmap sprite, Bitmap colorPalette, int from, int to, int x, int y)
        {
            Color color = colorPalette.GetPixel(0, to);
            Bitmap bitmap = (Bitmap)sprite.Clone();

            for (int n = 0; n < bitmap.Width; n++)
            {
                for (int m = 0; m < bitmap.Height; m++)
                {
                    Color pixelColor = bitmap.GetPixel(n, m);

                    if (pixelColor.A > 125)
                    {
                        bitmap.SetPixel(n, m, color);
                    }
                }
            }

            // Draw using the color map
            Rectangle rect = new Rectangle(x * sprite.Width, y * sprite.Height, sprite.Width, sprite.Height);
            graphics.DrawImage(bitmap, rect, 0, 0, rect.Width, rect.Height, GraphicsUnit.Pixel);
        }

        private static Bitmap[] Get4Sprites(Bitmap sprite)
        {
            bool sprites_4frame = sprite.Width == 64 * 4;
            bool sprites_2frame = sprite.Width == 64 * 2;
            if (sprites_2frame || sprites_4frame)
            {
                Bitmap[] bitmaps = new Bitmap[4];
                for (int i = 0; i < 4; i++)
                {
                    bitmaps[i] = new Bitmap(64, 64);
                }
                Rectangle srcrect = new Rectangle(0, 0, 64, 64);
                Rectangle dstrect = new Rectangle(0, 0, 64, 64);


                if (sprites_2frame)
                {
                    using (Graphics graphics = Graphics.FromImage(bitmaps[0]))
                    {
                        graphics.DrawImage(sprite, dstrect, srcrect, GraphicsUnit.Pixel);
                    }
                    using (Graphics graphics = Graphics.FromImage(bitmaps[1]))
                    {
                        graphics.TranslateTransform(bitmaps[1].Width, 0);
                        graphics.ScaleTransform(-1, 1);
                        graphics.DrawImage(sprite, dstrect, srcrect, GraphicsUnit.Pixel);
                    }
                    srcrect = new Rectangle(64, 0, 64, 64);
                    using (Graphics graphics = Graphics.FromImage(bitmaps[2]))
                    {
                        graphics.DrawImage(sprite, dstrect, srcrect, GraphicsUnit.Pixel);
                    }
                    using (Graphics graphics = Graphics.FromImage(bitmaps[3]))
                    {
                        graphics.TranslateTransform(bitmaps[3].Width, 0);
                        graphics.ScaleTransform(-1, 1);
                        graphics.DrawImage(sprite, dstrect, srcrect, GraphicsUnit.Pixel);
                    }
                }
                else if (sprites_4frame)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        srcrect = new Rectangle(i * 64, 0, 64, 64);
                        using (Graphics graphics = Graphics.FromImage(bitmaps[i]))
                        {
                            graphics.DrawImage(sprite, dstrect, srcrect, GraphicsUnit.Pixel);
                        }
                    }
                }

                return bitmaps;
            }
            else
            {
                return null;
            }
        }
        private static Bitmap GetVanillaSprite(Image sprite, Bitmap colorPalette)
        {
            Bitmap bitmap = new Bitmap((Image)sprite.Clone());
            int boolcount;
            for (int i = 0; i < colorPalette.Height - 1; i++)
            {
                boolcount = 0;
                bool[] bools = new bool[colorPalette.Width];
                for (int j = 0; j < colorPalette.Width; j++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        if (bools[j])
                        {
                            break;
                        }
                        for (int y = 0; y < bitmap.Height; y++)
                        {
                            if (bools[j])
                            {
                                break;
                            }
                            Color pixelColor = bitmap.GetPixel(x, y);
                            bools[j] = pixelColor.ToArgb() == colorPalette.GetPixel(j, i).ToArgb();

                        }
                    }
                    if (bools[j])
                    {
                        boolcount += 1;
                    }
                }

                if (boolcount > 3)
                {
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        ReplaceColor(graphics, bitmap, colorPalette, i, 0, 0, 0);
                    }
                    
                    //for (int j = 0; j < colorPalette.Width; j++)
                    //{
                    //    for (int x = 0; x < bitmap.Width; x++)
                    //    {
                    //        for (int y = 0; y < bitmap.Height; y++)
                    //        {
                    //            Color pixelColor = bitmap.GetPixel(x, y);

                    //            if (pixelColor.ToArgb() == colorPalette.GetPixel(j, i).ToArgb())
                    //            {
                    //                bitmap.SetPixel(x, y, colorPalette.GetPixel(0, j));
                    //            }
                    //        }
                    //    }
                    //}
                    return bitmap;
                }
            }
            return null;
        }


        public static void CreateSpriteSheet(string filepath, string out_filepath)
        {
            if (filepath == null)
            {
                return;
            }
            Image sprites = Image.FromFile(filepath);
            if (sprites.Height != 64)
            {
                return;
            }
            bool sprites_4frame = sprites.Width == 64 * 4;
            bool sprites_2frame = sprites.Width == 64 * 2;
            if (!sprites_2frame && !sprites_4frame)
            {
                return;
            }


            Stream imgStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ChaosGalaxySpriteSheetLazyMaker.Resources.ColorPalette.png");
            Image colorPalette = Image.FromStream(imgStream);
            Bitmap colorPaletteBitmap = new Bitmap(colorPalette);
            imgStream.Close();

            int width = 64 * 4 * 3;
            int height = (int)(64 * Math.Ceiling(colorPalette.Height / 3.0));
            Bitmap output_bitmap = new Bitmap(width, height);

            Bitmap spritesBitmap = GetVanillaSprite(sprites, colorPaletteBitmap);
            if (spritesBitmap == null)
            {
                return;
            }

            Bitmap[] bitmapsSprites=Get4Sprites(spritesBitmap);

            using (Graphics graphics = Graphics.FromImage(output_bitmap))
            {
                graphics.Clear(Color.Transparent);
                graphics.CompositingMode = CompositingMode.SourceOver;

                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                graphics.SmoothingMode = SmoothingMode.None;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                int j = 0;
                for (int i = 0; i < colorPalette.Height - 1; i++)
                {
                    for (int n = 0; n < bitmapsSprites.Length; n++)
                    {
                        ReplaceColor(graphics, bitmapsSprites[n], colorPaletteBitmap, 0, i, bitmapsSprites.Length*j + n, i/3);

                    }
                    j = (j + 1) % 3;

                }
                for (int n = 0; n < bitmapsSprites.Length; n++)
                {
                    FillingWhite(graphics, bitmapsSprites[n], colorPaletteBitmap, 0, colorPalette.Height - 1, bitmapsSprites.Length * j + n, (colorPalette.Height - 1) / 3);
 
                }
                j = (j + 1) % 3;
                
            }

            output_bitmap.Save(out_filepath);//save the image file
        }


    }
}
