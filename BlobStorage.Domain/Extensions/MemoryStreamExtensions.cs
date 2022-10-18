using BlobStorage.Domain.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
//using SixLabors.ImageSharp;
//using SixLabors.ImageSharp.Formats.Jpeg;
//using SixLabors.ImageSharp.Formats.Png;
//using SixLabors.ImageSharp.Formats.Gif;
//using SixLabors.ImageSharp.Processing;
using System.Drawing;

namespace BlobStorage.Domain.Extensions
{
	//public static class MemoryStreamExtensions
	//{
	//	public static MemoryStream Resize(Stream input, ContentType contype)
	//	{
	//		MemoryStream outputFINAL = new MemoryStream();
	//		Stream output = null;

	//		using (Image image = Image.Load(input))
	//		{
	//			image.Mutate(x => x.Resize(100, 100));
	//			switch (contype)
	//			{
	//				case ContentType.image_jpeg:
	//					image.Save(output, new JpegEncoder());
	//					break;
	//				case ContentType.image_png:
	//					image.Save(output, new PngEncoder());
	//					break;
	//				case ContentType.image_gif:
	//					image.Save(output, new GifEncoder());
	//					break;
	//			}

	//		}
	//		output.Position = 0;
	//		output.CopyTo(outputFINAL);
	//		return outputFINAL;

	//	}
	//}




	public static class MemoryStreamExtensions
	{
		/// <summary>Resizes an image loaded in memory stream.
		/// </summary>
		/// <param name="imageBytes">Bytes of the image.</param>
		/// <param name="newWidth">The new width of the image. The height of the image is calculated proportionally.</param>
		public static MemoryStream ResizeImage(this MemoryStream ms, int newSizeWH, ContentType contype)
		{

			MemoryStream output = new MemoryStream();

			try
			{
				Image img = Image.FromStream(ms);

				int h = 0;
				int w = 0;

				int oldw = img.Width;
				int oldh = img.Height;
				
				
				if (oldw >= oldh)
				{
					w = newSizeWH;
					h = oldh * w / oldw;
				}
				else
				{
					h = newSizeWH;
					w = oldw * h / oldh;
				}

				Bitmap b = new Bitmap(img, new Size(w, h));

				MemoryStream ms2 = new MemoryStream();


				switch (contype)
				{
					case ContentType.image_jpeg:
						b.Save(ms2, System.Drawing.Imaging.ImageFormat.Jpeg);
						output = ms2;
						break;
					case ContentType.image_png:
						b.Save(ms2, System.Drawing.Imaging.ImageFormat.Png);
						output = ms2;
						break;
					case ContentType.image_gif:
						b.Save(ms2, System.Drawing.Imaging.ImageFormat.Gif);
						output = ms2;
						break;
					case ContentType.image_bmp:
						b.Save(ms2, System.Drawing.Imaging.ImageFormat.Bmp);
						output = ms2;
						break;
					case ContentType.image_tiff:
						b.Save(ms2, System.Drawing.Imaging.ImageFormat.Tiff);
						output = ms2;
						break;
				}

			}


			catch (Exception ex)
			{
				Console.Write(ex.Message);
				return null;
			}

			output.Position = 0;
			return output;
		}
	}
}
