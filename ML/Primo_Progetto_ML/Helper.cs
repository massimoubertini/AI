﻿using System;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace MNIST_Demo
{
    public class Helper
    {
        private VideoFrame cropped_vf = null;
        /* la funzionalità per convertire il numero, scritto all’interno dell’InkCanvas,
         * ad un oggetto VideoFrame da impostare poi come input nella variabile
         * Input3 è stata scritta all'interno di una classe chiamata Helper */

        public async Task<VideoFrame> GetHandWrittenImage(Grid grid)
        {
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap();
            await renderBitmap.RenderAsync(grid);
            var buffer = await renderBitmap.GetPixelsAsync();
            var softwareBitmap = SoftwareBitmap.CreateCopyFromBuffer(buffer, BitmapPixelFormat.Bgra8, renderBitmap.PixelWidth, renderBitmap.PixelHeight, BitmapAlphaMode.Ignore);
            buffer = null;
            renderBitmap = null;
            VideoFrame vf = VideoFrame.CreateWithSoftwareBitmap(softwareBitmap);
            await CropAndDisplayInputImageAsync(vf);
            return cropped_vf;
        }

        private async Task CropAndDisplayInputImageAsync(VideoFrame inputVideoFrame)
        {
            bool useDX = inputVideoFrame.SoftwareBitmap == null;
            BitmapBounds cropBounds = new BitmapBounds();
            uint h = 28;
            uint w = 28;
            var frameHeight = useDX ? inputVideoFrame.Direct3DSurface.Description.Height : inputVideoFrame.SoftwareBitmap.PixelHeight;
            var frameWidth = useDX ? inputVideoFrame.Direct3DSurface.Description.Width : inputVideoFrame.SoftwareBitmap.PixelWidth;
            var requiredAR = ((float)28 / 28);
            w = Math.Min((uint)(requiredAR * frameHeight), (uint)frameWidth);
            h = Math.Min((uint)(frameWidth / requiredAR), (uint)frameHeight);
            cropBounds.X = (uint)((frameWidth - w) / 2);
            cropBounds.Y = 0;
            cropBounds.Width = w;
            cropBounds.Height = h;
            cropped_vf = new VideoFrame(BitmapPixelFormat.Bgra8, 28, 28, BitmapAlphaMode.Ignore);
            await inputVideoFrame.CopyToAsync(cropped_vf, cropBounds, null);
        }
    }
}