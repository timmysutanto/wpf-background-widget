using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace CounterWidget;

public partial class MainWindow : Window
{
    private int _count = 0;

    // Win32 constants for desktop pinning
    private static readonly IntPtr HWND_BOTTOM = new(9999);
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOACTIVATE = 0x0010;

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    public MainWindow()
    {
        InitializeComponent();
        this.Top = SystemParameters.WorkArea.Height - this.Height;
        this.Left = SystemParameters.WorkArea.Width - this.Width;
        this.Loaded += (s, e) => PinToDesktop();
    }

    private void PinToDesktop()
    {
        IntPtr hWnd = new WindowInteropHelper(this).Handle;
        SetWindowPos(hWnd, HWND_BOTTOM, Convert.ToInt32(this.Left), Convert.ToInt32(this.Top), Convert.ToInt32(this.Left), Convert.ToInt32(this.Top), SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
    }

    // private void Increment_Click(object sender, RoutedEventArgs e)
    // {
    //     _count++;
    //     CounterDisplay.Text = _count.ToString();
    // }
    //
    // private void Decrement_Click(object sender, RoutedEventArgs e)
    // {
    //     _count--;
    //     CounterDisplay.Text = _count.ToString();
    // }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left) DragMove();
    }

    // private void Reset_Click(object sender, RoutedEventArgs e) => CounterDisplay.Text = (_count = 0).ToString();
    private void Exit_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
}