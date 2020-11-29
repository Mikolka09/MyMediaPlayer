using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Win32;

namespace MyMediaPlayer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timerVideoTime;
        private MediaPlayer player = new MediaPlayer();
        Binding binding1 = new Binding();
        Binding binding2 = new Binding();
        public MainWindow()
        {
            InitializeComponent();
            mediaElement.Volume = 1;
            player.Volume = 1;
            image.Opacity = 1;


        }

        private void EnableButtons(bool is_playing)
        {
            if (is_playing)
            {
                butPlay.IsEnabled = false;
                butPause.IsEnabled = true;
                butPlay.Opacity = 0.5;
                butPause.Opacity = 1.0;
            }
            else
            {
                butPlay.IsEnabled = true;
                butPause.IsEnabled = false;
                butPlay.Opacity = 1.0;
                butPause.Opacity = 0.5;
            }
            timerVideoTime.IsEnabled = is_playing;
        }

        private void Button_Play(object sender, RoutedEventArgs e)
        {
            if (player.HasVideo)
            {
                image.Opacity = 0;
                mediaElement.Play();
            }
            else
            {
                image.Opacity = 1;
                player.Play();
            }
            EnableButtons(true);
        }

        private void Button_Pause(object sender, RoutedEventArgs e)
        {
            if (player.HasVideo)
                mediaElement.Pause();
            else
                player.Pause();
            EnableButtons(false);
        }

        private void Button_Stop(object sender, RoutedEventArgs e)
        {
            if (player.HasVideo)
                mediaElement.Stop();
            else
                player.Stop();
            EnableButtons(false);
            ShowPosition();
        }

        private void Button_Open(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Все форматы (*.*)|*.*|Видео MPEG-4 (*.mp4)|*.mp4|Видео AVI (*.avi)|*.avi|" +
                "Видео WMV (*.wmv)|*.wmv|Видео Matroska (*.mkv)|*.mkv|Аудио MP3 (*.mp3)|*.mp3|Аудио WAV (*.wav)|*.wav";
            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                Uri source = new Uri(openFileDialog.FileName);
                player.Open(source);
                if (player.HasAudio)
                {
                    player.MediaOpened += new EventHandler(player_MediaOpened);
                    binding1.Source = player;
                    binding1.Path = new PropertyPath("Volume");
                    binding1.Mode = BindingMode.TwoWay;
                    sliderVolume1.SetBinding(Slider.ValueProperty, binding1);
                    binding2.Source = player;
                    binding2.Path = new PropertyPath("Balance");
                    binding2.Mode = BindingMode.TwoWay;
                    sliderBalance1.SetBinding(Slider.ValueProperty, binding2);
                }
                else
                {
                    mediaElement.Source = source;
                    binding1.Source = mediaElement;
                    binding1.Path = new PropertyPath("Volume");
                    binding1.Mode = BindingMode.TwoWay;
                    sliderVolume1.SetBinding(Slider.ValueProperty, binding1);
                    binding2.Source = mediaElement;
                    binding2.Path = new PropertyPath("Balance");
                    binding2.Mode = BindingMode.TwoWay;
                    sliderBalance1.SetBinding(Slider.ValueProperty, binding2);
                }

            }
        }

        private void Button_Mute(object sender, RoutedEventArgs e)
        {
            if (player.HasVideo)
                if (mediaElement.Volume == 1)
                {
                    mediaElement.Volume = 0;
                    butMute.Content = "Listen";
                }
                else
                {
                    mediaElement.Volume = 1;
                    butMute.Content = "Mute";
                }
            else
            {
                if (player.Volume == 1)
                {
                    player.Volume = 0;
                    butMute.Content = "Listen";
                }
                else
                {
                    player.Volume = 1;
                    butMute.Content = "Mute";
                }
            }
        }

        private void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            sliderVideo.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
        }

        private void player_MediaOpened(object sender, EventArgs e)
        {
            sliderVideo.Maximum = player.NaturalDuration.TimeSpan.TotalSeconds;
        }

        private void ShowPosition()
        {
            if (player.HasVideo)
                sliderVideo.Value = mediaElement.Position.TotalSeconds;
            else
                sliderVideo.Value = player.Position.TotalSeconds;
            int minuts = Convert.ToInt32(sliderVideo.Value) / 60;
            int seconds = Convert.ToInt32(sliderVideo.Value) % 60;
            txtVideo.Text = (minuts < 10 ? "0" + minuts.ToString() : minuts.ToString())
                + ":" + (seconds < 10 ? "0" + seconds.ToString() : seconds.ToString());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timerVideoTime = new DispatcherTimer();
            timerVideoTime.Interval = TimeSpan.FromSeconds(1);
            timerVideoTime.Tick += new EventHandler(timer_Tick);
            timerVideoTime.Start();
        }

        private void timer_Tick(object sender, EventArgs e) => ShowPosition();

        private void sliderVideo_LostMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
        {
            TimeSpan time = new TimeSpan(0, 0, Convert.ToInt32(Math.Round(sliderVideo.Value)));
            if (player.HasVideo)
                mediaElement.Position = time;
            else
                player.Position = time;
        }


    }
}
