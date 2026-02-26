using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace wpf_background_widget;

public partial class MainWindow : Window
{
    // Win32 constants for desktop pinning
    private static readonly IntPtr HWND_BOTTOM = new(9999);
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOACTIVATE = 0x0010;
    private static readonly List<string> CardSuits = new List<string>
    {
        "club", "spade", "heart", "heart"
    };
    private static readonly List<string> CardNumbers = new List<string>
    {
        "2", "3", "4", "5", "6",  "7", "8", "9", "10", "J", "Q", "K", "A"
    };

    private static List<string> CardsOnTable = new List<string>
    { };

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
    
    private void Init_Dealer_Card(object sender, RoutedEventArgs e)
    {
        string cardSuitNumber;
        
        // First Card
        cardSuitNumber = GetRandomCardNumberSuit();
        if (cardSuitNumber  == "")
        {
            return;
        }
        LoadImage(DealerCanvas1, "pack://application:,,,/Resources/Cards/" + GetRandomCardNumberSuit() + ".png");
        
        // Second Card
        cardSuitNumber = GetRandomCardNumberSuit();
        if (cardSuitNumber  == "")
        {
            return;
        }
        LoadImage(DealerCanvas2,"pack://application:,,,/Resources/Cards/" + GetRandomCardNumberSuit() + ".png");
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left) DragMove();
    }

    // private void Reset_Click(object sender, RoutedEventArgs e) => CounterDisplay.Text = (_count = 0).ToString();
    private void Exit_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

    private void LoadImage(Canvas canvas, string imagePath)
    {
        BitmapImage  image = new BitmapImage();
        image.BeginInit();
        image.UriSource = new Uri(imagePath);
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.EndInit();

        Image img = new Image();
        img.Source = image;
        img.Width = 150;
        img.Height = 210;
        img.Stretch = System.Windows.Media.Stretch.Uniform;
        
        canvas.Children.Clear();
        canvas.Children.Add(img);
    }

    private string GetRandomCardNumberSuit()
    {
        var result="";

        if (CardsOnTable.Count() >= 52)
        {
            return result;
        }
        
        string chosenCardSuit;
        string chosenCardNumber;
        while (true)
        {
            chosenCardSuit = Random.Shared.GetItems(CardSuits.ToArray(), 1)[0];
            chosenCardNumber = Random.Shared.GetItems(CardNumbers.ToArray(), 1)[0];
            result = chosenCardSuit + "_" + chosenCardNumber;
            if (CardsOnTable.Contains(result))
            {
                continue;
            }

            CardsOnTable.Add(result);
            return result;
        }
    }
}
