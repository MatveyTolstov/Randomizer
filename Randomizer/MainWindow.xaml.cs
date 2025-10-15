using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.IO;
using System.Windows.Media;

namespace Randomizer
{
    public partial class MainWindow : Window
    {
        private bool isRandomizing = false;
        private Random random = new Random();
        private DispatcherTimer animationTimer;
        private double rotationAngle = 90;
        private bool isWin;
        private MediaPlayer mediaPlayer;

        public MainWindow()
        {
            InitializeComponent();

            ArrowTransform.Angle = 90;

            animationTimer = new DispatcherTimer();
            animationTimer.Interval = TimeSpan.FromMilliseconds(16);
            animationTimer.Tick += AnimationTimer_Tick;

            mediaPlayer = new MediaPlayer();
            mediaPlayer.MediaEnded += (s, e) =>
            {
                if (isRandomizing)
                {
                    mediaPlayer.Position = TimeSpan.Zero;
                    mediaPlayer.Play();
                }
            };
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            rotationAngle += 10;
            if (rotationAngle >= 360) rotationAngle = 0;

            ArrowTransform.Angle = rotationAngle;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (isRandomizing) return;

            isRandomizing = true;
            StartButton.IsEnabled = false;

            try
            {
                PlayRotationMusic();

                rotationAngle = ArrowTransform.Angle;
                animationTimer.Start();

                int randomTime = random.Next(2000, 5000);
                await Task.Delay(randomTime);

                animationTimer.Stop();

                bool result = random.Next(2) == 0;

                isWin = result;

                await RotateToFinalPosition(result ? 180 : 0);

                StopMusic();
                PlayResultMusic();
            }
            finally
            {
                isRandomizing = false;
                StartButton.IsEnabled = true;
            }
        }

        private void PlayRotationMusic()
        {
            try
            {
                string musicPath = "..\\..\\bam_bam.mp3";

                if (File.Exists(musicPath))
                {
                    mediaPlayer.Open(new Uri(musicPath, UriKind.RelativeOrAbsolute));
                    mediaPlayer.Play();
                }
                else
                {
                    MessageBox.Show($"Файл музыки вращения не найден: {musicPath}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка воспроизведения музыки вращения: {ex.Message}");
            }
        }

        private void PlayResultMusic()
        {
            try
            {
                string musicPath;

                if (!isWin)
                {
                    musicPath = "..\\..\\Sounds\\win.mp3";

                }
                else
                {
                    musicPath = "..\\..\\Sounds\\lose.mp3";
                }

                if (File.Exists(musicPath))
                {
                    mediaPlayer.Open(new Uri(musicPath, UriKind.RelativeOrAbsolute));
                    mediaPlayer.Play();
                }
                else
                {
                    MessageBox.Show($"Файл музыки результата не найден: {musicPath}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка воспроизведения музыки результата: {ex.Message}");
            }
        }

        private async Task RotateToFinalPosition(double targetAngle)
        {
            double startAngle = ArrowTransform.Angle;
            double duration = 500;
            int steps = 25;
            double stepTime = duration / steps;
            double angleStep = (targetAngle - startAngle) / steps;

            for (int i = 0; i <= steps; i++)
            {
                double currentAngle = startAngle + (angleStep * i);
                ArrowTransform.Angle = currentAngle;
                await Task.Delay((int)stepTime);
            }

            ArrowTransform.Angle = targetAngle;
        }

        private void StopMusic()
        {
            mediaPlayer.Stop();
        }

        protected override void OnClosed(EventArgs e)
        {
            mediaPlayer.Close();
            base.OnClosed(e);
        }
    }
}