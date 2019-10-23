using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.Foundation;

namespace NeonTDS
{
    public class BloomRendering
    {
        private CanvasRenderTarget bloomRenderTarget;
        private readonly LinearTransferEffect extractBrightAreas;
        private readonly GaussianBlurEffect blurBrightAreas;
        private readonly LinearTransferEffect adjustBloomIntensity;
        private readonly BlendEffect result;
        private Size renderSize;
        private bool canvasIsLoaded = false;

        public bool SpinEnabled { get; set; }
        public bool BloomEnabled { get; set; }
        public float BloomIntensity { get; set; }
        public float BloomThreshold { get; set; }
        public float BloomBlur { get; set; }

        public BloomRendering()
        {
            extractBrightAreas = new LinearTransferEffect
            {
                ClampOutput = true,
            };
            blurBrightAreas = new GaussianBlurEffect
            {
                Source = extractBrightAreas,
            };
            adjustBloomIntensity = new LinearTransferEffect
            {
                Source = blurBrightAreas,
            };
            result = new BlendEffect
            {
                Foreground = adjustBloomIntensity,
                Mode = BlendEffectMode.Screen,
            };
        }

        public void CanvasLoaded(CanvasAnimatedControl canvas)
        {
            canvasIsLoaded = true;
            RecreateRenderTarget(canvas);
        }

        public void Resize(CanvasAnimatedControl canvas, Size size)
        {
            renderSize = size;
            if (canvasIsLoaded) RecreateRenderTarget(canvas);
        }

        private void RecreateRenderTarget(CanvasAnimatedControl canvas)
        {
            if (bloomRenderTarget == null || bloomRenderTarget.Size != renderSize)
            {
                bloomRenderTarget = new CanvasRenderTarget(canvas, renderSize);
                extractBrightAreas.Source = bloomRenderTarget;
                result.Background = bloomRenderTarget;
            }
        }

        public void SetupParameters()
        {
            extractBrightAreas.RedSlope = 1 / (1 - BloomThreshold / 100);
            extractBrightAreas.GreenSlope = 1 / (1 - BloomThreshold / 100);
            extractBrightAreas.BlueSlope = 1 / (1 - BloomThreshold / 100);

            extractBrightAreas.RedOffset = -BloomThreshold / 100 / (1 - BloomThreshold / 100);
            extractBrightAreas.GreenOffset = -BloomThreshold / 100 / (1 - BloomThreshold / 100);
            extractBrightAreas.BlueOffset = -BloomThreshold / 100 / (1 - BloomThreshold / 100);

            blurBrightAreas.BlurAmount = BloomBlur;

            adjustBloomIntensity.RedSlope = BloomIntensity / 100;
            adjustBloomIntensity.GreenSlope = BloomIntensity / 100;
            adjustBloomIntensity.BlueSlope = BloomIntensity / 100;
        }

        public CanvasDrawingSession CreateDrawingSession()
        {
            return bloomRenderTarget.CreateDrawingSession();
        }

        public void DrawResult(CanvasDrawingSession drawingSession)
        {
            drawingSession.DrawImage(result);
        }
    }
}
