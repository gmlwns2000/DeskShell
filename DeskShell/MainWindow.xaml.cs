using DeskShell.Natives;
using Gma.System.MouseKeyHook;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace DeskShell
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private ShellWindow shell;
        private IKeyboardMouseEvents globalHook;
        
        public MainWindow()
        {
            InitializeComponent();

            globalHook = Hook.GlobalEvents();
            globalHook.MouseDragStartedExt += GlobalHook_MouseDragStartedExt;
            globalHook.MouseDragFinishedExt += GlobalHook_MouseDragFinishedExt;

            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            ShellUtility.ShowFolder();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            (PresentationSource.FromVisual(this) as HwndSource).AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WinAPI.WM_MOUSEACTIVATE)
            {
                handled = true;
                return new IntPtr(WinAPI.MA_NOACTIVATE);
            }
            else
            {
                return IntPtr.Zero;
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ShellUtility.SetDesktop(this);

            shell = new ShellWindow
            {
                Owner = this
            };

            shell.Show();
        }

        private void GlobalHook_MouseDragStartedExt(object sender, MouseEventExtArgs e)
        {
            ShellUtility.SetTranspernt(this, true);
        }

        private void GlobalHook_MouseDragFinishedExt(object sender, MouseEventExtArgs e)
        {
            var delayTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(10)
            };

            delayTimer.Tick += (tS, tE) =>
            {
                ShellUtility.SetTranspernt(this, false);
                delayTimer.Stop();
            };

            delayTimer.Start();
        }

        private void Menu_OpneMedia_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Media Files|*.mp4;*.mp3;*.mpeg;*.mp2;*.wmv;*.wma|All Files|*.*";
            if((bool)ofd.ShowDialog())
            {
                mediaElement.Source = new Uri(ofd.FileName);
                mediaElement.Play();
            }
        }

        private void Menu_OpenImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.tiff;*.gif;*.bmp|All Files|*.*";
            if ((bool)ofd.ShowDialog())
            {
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.UriSource = new Uri(ofd.FileName);
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.EndInit();

                image.Source = img;

                img.Freeze();
            }
        }

        private void Menu_CloseMedia_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Stop();
            mediaElement.Close();
            mediaElement.Source = null;
        }

        private void Menu_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Sld_Blur_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            blurEffect.Radius = e.NewValue;
            Grid_Background.Margin = new Thickness(-e.NewValue);
        }
    }
}
