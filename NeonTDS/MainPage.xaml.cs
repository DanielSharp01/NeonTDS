using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Win2DEngine
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Game game;
        public MainPage()
        {
            InitializeComponent();
            game = new Game();
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown += KeyDown_UIThread;
            Window.Current.CoreWindow.KeyUp += KeyUp_UIThread;
            Window.Current.CoreWindow.PointerPressed += Pointer_UIThread;
            Window.Current.CoreWindow.PointerMoved += Pointer_UIThread;
            Window.Current.CoreWindow.PointerReleased += Pointer_UIThread;
            game.Load();
        }

        private void PageUnloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown -= KeyDown_UIThread;
            Window.Current.CoreWindow.KeyUp -= KeyUp_UIThread;
            Window.Current.CoreWindow.PointerPressed -= Pointer_UIThread;
            Window.Current.CoreWindow.PointerMoved -= Pointer_UIThread;
            Window.Current.CoreWindow.PointerReleased -= Pointer_UIThread;
        }

        private void KeyDown_UIThread(CoreWindow sender, KeyEventArgs args)
        {
            args.Handled = true;
            var virtualKey = args.VirtualKey;
            var action = canvasControl.RunOnGameLoopThreadAsync(() => game.InputManager.KeyDown_GameLoopThread(virtualKey));
        }

        private void KeyUp_UIThread(CoreWindow sender, KeyEventArgs args)
        {
            args.Handled = true;
            var virtualKey = args.VirtualKey;
            var action = canvasControl.RunOnGameLoopThreadAsync(() => game.InputManager.KeyUp_GameLoopThread(virtualKey));
        }

        private void Pointer_UIThread(CoreWindow sender, PointerEventArgs args)
        {
            args.Handled = true;
            var mouseStates = new bool[5];
            mouseStates[0] = args.CurrentPoint.Properties.IsLeftButtonPressed;
            mouseStates[1] = args.CurrentPoint.Properties.IsRightButtonPressed;
            mouseStates[2] = args.CurrentPoint.Properties.IsMiddleButtonPressed;
            mouseStates[3] = args.CurrentPoint.Properties.IsXButton1Pressed;
            mouseStates[4] = args.CurrentPoint.Properties.IsXButton2Pressed;
            var position = args.CurrentPoint.Position.ToVector2();
            var action = canvasControl.RunOnGameLoopThreadAsync(() => game.InputManager.PointerEvent_GameLoopThread(mouseStates, position));
        }

        private void canvasControl_CreateResources(CanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            game.CreateResources(sender);
        }

        private void canvasControl_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            game.Update(args.Timing);
        }

        private void canvasControl_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            game.Draw(args.DrawingSession, args.Timing);
        }

        private void canvasControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            game.SizeChanged(sender as CanvasAnimatedControl, e.NewSize, e.PreviousSize);
        }
    }
}
