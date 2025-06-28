using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace weatherApp;
public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
        DataContext = this;
        Loaded += StartAsync;
        Loaded += UpdateTimer;
    }


    void StartAsync(object sender, RoutedEventArgs e) {
        OnLoadAsync();
    }

    private void UpdateTimer(object sender, RoutedEventArgs e) {
        var dispatcherTimer = new DispatcherTimer();
        dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
        dispatcherTimer.Interval = TimeSpan.FromSeconds(300);
        dispatcherTimer.Start();
    }

    private void dispatcherTimer_Tick(object? sender, EventArgs e) {
        OnLoadAsync();
    }

    private void Clear() {
        header.Children.Clear();
        timestampPannel.Children.Clear();
        dayStempPannel.Children.Clear();
    }

    private async void OnLoadAsync() {
        WeatherParser wp = new();
        var data = await wp.ParseAsync();

        Clear();

        //Header
        int frameIndex = data.weatherHeader.iconID;
        double frameSize = 48;
        HeaderUC headerUc = new();
        headerUc.headerIcon.RenderTransform = new TranslateTransform(-frameIndex * frameSize, 0);
        headerUc.headerText.Text = data.weatherHeader.descr;
        headerUc.headerTemp.Content = data.weatherHeader.currentTemp;
        header.Children.Add(headerUc);

        // Timestaps
        bool bgSwitched = false;
        foreach (var item in data.timeStamps) {
            TimestampUC stamp = new();
            stamp.Time.Content = item.time;
            stamp.Temperature.Content = item.temperature;

            frameIndex = item.iconID;
            stamp.timestampIco.RenderTransform = new TranslateTransform(-frameIndex * frameSize, 0);

            if (bgSwitched) {
                stamp.bgSwitch.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#ECE2DD");
                bgSwitched = false;
            } else {
                stamp.bgSwitch.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#CAC4CE");
                bgSwitched = true;
            }

            timestampPannel.Children.Add(stamp);
        }

        //DayStamps
        bgSwitched = false;
        foreach (var item in data.dayStamps) {
            DaystampUC stamp = new();
            stamp.weekDay.Content = item.weekDay;
            stamp.date.Content = item.date;
            stamp.dayTemp.Content = item.dayTemp;
            stamp.nightTemp.Content = item.nightTemp;

            frameIndex = item.iconID;
            stamp.timestampIco.RenderTransform = new TranslateTransform(-frameIndex * frameSize, 0);

            if (bgSwitched) {
                stamp.bgSwitch.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#ECE2DD");
                bgSwitched = false;
            } else {
                stamp.bgSwitch.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#CAC4CE");
                bgSwitched = true;
            }
            dayStempPannel.Children.Add(stamp);
        }
    }

    private void CloseBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
        Environment.Exit(0);
        Close();
    }

    private void MoveBar_MouseDown(object sender, MouseButtonEventArgs e) {
        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }
}
