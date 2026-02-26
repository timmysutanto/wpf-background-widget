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
        "club", "spade", "heart", "diamond"
    };
    private static readonly List<string> CardNumbers = new List<string>
    {
        "2", "3", "4", "5", "6",  "7", "8", "9", "10", "J", "Q", "K", "A"
    };

    private static List<string> CardsOnTable = new List<string>
    { };
    private Canvas? _playerCanvas3;
    private int playerCardsCount = 0;
    private int dealerCardsCount = 0;

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
    
    private void Init_Dealer_And_Player_Card(object sender, RoutedEventArgs e)
    {
        
        string cardSuitNumber;
        
        // First Dealer Card
        cardSuitNumber = GetRandomCardNumberSuit();
        if (cardSuitNumber  == "")
        {
            return;
        }
        DealerCanvas1 = LoadImage(DealerCanvas1, "pack://application:,,,/Resources/Cards/" + cardSuitNumber + ".png");
        
        // Second Dealer Card
        cardSuitNumber = GetRandomCardNumberSuit();
        if (cardSuitNumber  == "")
        {
            return;
        }
        DealerCanvas2 = LoadImage(DealerCanvas2,"pack://application:,,,/Resources/Cards/" + cardSuitNumber + ".png");
        
        // First Player Card
        cardSuitNumber = GetRandomCardNumberSuit();
        if (cardSuitNumber  == "")
        {
            return;
        }
        PlayerCanvas1 = LoadImage(PlayerCanvas1, "pack://application:,,,/Resources/Cards/" + cardSuitNumber + ".png");
        
        // Second Player Card
        cardSuitNumber = GetRandomCardNumberSuit();
        if (cardSuitNumber  == "")
        {
            return;
        }
        PlayerCanvas2 = LoadImage(PlayerCanvas2,"pack://application:,,,/Resources/Cards/" + cardSuitNumber + ".png");

        playerCardsCount += 2;
        dealerCardsCount += 2;
    }

    private void Hit_Player_Card(object sender, RoutedEventArgs e)
    {
        // if (_playerCanvas3 != null)
        // {
        //     return;
        // }

        string cardSuitNumber = GetRandomCardNumberSuit();
        playerCardsCount += 1;
        if (cardSuitNumber == "")
        {
            return;
        }
        
        FormLayoutGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        _playerCanvas3 = CreateCardCanvas("pack://application:,,,/Resources/Cards/" + cardSuitNumber + ".png");
        Grid.SetRow(_playerCanvas3, 4);
        Grid.SetColumn(_playerCanvas3, playerCardsCount - 1);
        FormLayoutGrid.Children.Add(_playerCanvas3);
        
        Grid.SetColumnSpan(ButtonStackPanel, playerCardsCount);
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left) DragMove();
    }

    // private void Reset_Click(object sender, RoutedEventArgs e) => CounterDisplay.Text = (_count = 0).ToString();
    private void Exit_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

    private Canvas LoadImage(Canvas canvas, string imagePath)
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

        Canvas newCanvas = new Canvas
        {
            Width = canvas.Width,
            Height = canvas.Height,
            Background = canvas.Background,
            Margin = canvas.Margin,
            HorizontalAlignment = canvas.HorizontalAlignment,
            VerticalAlignment = canvas.VerticalAlignment
        };
        newCanvas.Children.Add(img);

        Grid.SetRow(newCanvas, Grid.GetRow(canvas));
        Grid.SetColumn(newCanvas, Grid.GetColumn(canvas));
        Grid.SetRowSpan(newCanvas, Grid.GetRowSpan(canvas));
        Grid.SetColumnSpan(newCanvas, Grid.GetColumnSpan(canvas));

        if (canvas.Parent is Panel parent)
        {
            int index = parent.Children.IndexOf(canvas);
            parent.Children.Remove(canvas);
            parent.Children.Insert(index, newCanvas);
        }

        return newCanvas;
    }

    private Canvas CreateCardCanvas(string imagePath)
    {
        BitmapImage image = new BitmapImage();
        image.BeginInit();
        image.UriSource = new Uri(imagePath);
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.EndInit();

        Image img = new Image
        {
            Source = image,
            Width = 150,
            Height = 210,
            Stretch = System.Windows.Media.Stretch.Uniform
        };

        Canvas canvas = new Canvas
        {
            Width = 150,
            Height = 210,
            Background = System.Windows.Media.Brushes.White,
            Margin = new Thickness(10)
        };
        canvas.Children.Add(img);

        return canvas;
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
