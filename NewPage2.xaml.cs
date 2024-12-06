using Microsoft.Maui.Layouts;
using System;
using System.Diagnostics;

namespace Uygulamam
{
    public partial class NewPage2 : ContentPage
    {
        double _xOffset, _yOffset;
        double _startX, _startY;
        private Random _random = new Random();
        private const int CreateInterval = 200, MovingTime = 5000,callingtime=3000; // Milisaniye cinsinden oluþturma aralýðý
        private double _currentX, _currentY;
        private double MyShipX= 0.5, MyShipY= 0.5;
        private string MyShipPath= "spaceship.png", BulletPath= "bullet.png";
        private Border MyShipBorder,EnemyShipBorder;
        private string EnemyShip = "enemyship.png";
        private List<Border> enemyShipBorders = new List<Border>();

        public NewPage2()
        {
            InitializeComponent();
            MyShip(MyShipX, MyShipY);
            fired();
            calltheenemies();
        }
        private void GenerateAndPlaceEnemyShip()
        {
            // Create a random number generator
            Random random = new Random();

            // Generate a random X position between 0 and 1
            double randomX = random.NextDouble();

            // Y position at the top of the AbsoluteLayout
            double yPosition = 0;

            // Call EnemyShip1 with the random positions
            EnemyShip1(randomX, yPosition);
        }

        private async void fired()
        {
            while (true)
            {
                CreateNewBullet(_currentX + 0.53, _currentY + 0.5);
                CreateNewBullet(_currentX + 0.47, _currentY + 0.5);
                await Task.Delay(CreateInterval); // Yeni Image oluþturma aralýðý
                                                  // Example of an async task (e.g., logging)
            }
        }
        private async void calltheenemies()
        {
            while (true)
            {
                GenerateAndPlaceEnemyShip();
                await Task.Delay(callingtime); // Yeni Image oluþturma aralýðý
                                                  // Example of an async task (e.g., logging)
            }
        }

        private async void CreateNewBullet(double XPosition, double YPosition)
        {

            // Create a new Bullet Image
            var newImage = new Image
            {
                Source = $"{BulletPath}",
                Aspect = Aspect.AspectFit,
                Rotation = -90,
                Opacity = 1,
            };

            AbsoluteLayout.SetLayoutBounds(newImage, new Microsoft.Maui.Graphics.Rect(XPosition, YPosition, 27, -1));
            AbsoluteLayout.SetLayoutFlags(newImage, AbsoluteLayoutFlags.PositionProportional);

            // Check for NullReferenceException
            if (absoluteLayout == null)
            {
                throw new InvalidOperationException("absoluteLayout is not initialized.");
            }

            absoluteLayout.Children.Add(newImage);
            MovingBullet(newImage);
            AnimateImage(newImage);
        }
        private async void MovingBullet(Image image)
        {
            double y = AbsoluteLayout.GetLayoutBounds(image).Y;
            DateTime startTime = DateTime.Now;
            while ((DateTime.Now - startTime).TotalMilliseconds < MovingTime)
            {
                await Task.Delay(100);
                y -= 0.02;

                // Log the current y position
                //Console.WriteLine($"Current y position: {y}");

                // Update the position
                AbsoluteLayout.SetLayoutBounds(image, new Rect(AbsoluteLayout.GetLayoutBounds(image).X, y, AbsoluteLayout.GetLayoutBounds(image).Width, AbsoluteLayout.GetLayoutBounds(image).Height));
                KonumTakipi(image);
            }
        }
        private async void AnimateImage(Image image)
        {
            // Make the image visible
            image.Opacity = 1;

            // Wait for the fade-out animation to complete
            await image.FadeTo(0, MovingTime);

            // Remove the image from the layout after the animation is complete
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (absoluteLayout != null)
                {
                    absoluteLayout.Children.Remove(image);
                }
            });
        }

        private async void KonumTakipi(Image image)
        {
            //Debug.WriteLine($"{image.X}{image.Y}");
            foreach (var enemyShipBorder in enemyShipBorders.ToList())  // ToList() used to avoid modification issues during iteration
            {
                if (IsBulletInsideEnemyBorder(image))
                {
                    if (absoluteLayout != null)
                    {
                        absoluteLayout.Children.Remove(enemyShipBorder);
                        enemyShipBorders.Remove(enemyShipBorder);  // Remove from the list
                    }
                }
            }
        }
        private async void EnemyShip1(double XPosition, double YPosition)
        {
            // Create the MyShip Image with Gesture Recognizers
            var enemyship = new Image
            {
                Source = $"{EnemyShip}",
                Aspect = Aspect.AspectFit,
                Rotation=180,
            };

            // Add gesture recognizers
            var tapGestureRecognizer = new TapGestureRecognizer { NumberOfTapsRequired = 2 }; tapGestureRecognizer.Tapped += OnTapped;

            var panGestureRecognizer = new PanGestureRecognizer(); panGestureRecognizer.PanUpdated += OnPanUpdated;
            enemyship.GestureRecognizers.Add(tapGestureRecognizer); enemyship.GestureRecognizers.Add(panGestureRecognizer);
            // Create the Border
            EnemyShipBorder = new Border
            {
                Stroke = Colors.Green,
                StrokeThickness = 2,
                Content = enemyship,
            };

            AbsoluteLayout.SetLayoutBounds(EnemyShipBorder, new Rect(XPosition, YPosition, 100, AbsoluteLayout.AutoSize));
            AbsoluteLayout.SetLayoutFlags(EnemyShipBorder, AbsoluteLayoutFlags.PositionProportional);

            // Add Border to the Layout (assuming you have an AbsoluteLayout named 'absoluteLayout')
            absoluteLayout.Children.Add(EnemyShipBorder);
            enemyShipBorders.Add(EnemyShipBorder);

        }
        private async void MyShip(double x, double y)
        {
            // Create the MyShip Image with Gesture Recognizers
            var myShipImage = new Image
            {
                Source = $"{MyShipPath}",
                Aspect = Aspect.AspectFit,
            };

            // Add gesture recognizers
            var tapGestureRecognizer = new TapGestureRecognizer { NumberOfTapsRequired = 2 }; tapGestureRecognizer.Tapped += OnTapped;

            var panGestureRecognizer = new PanGestureRecognizer(); panGestureRecognizer.PanUpdated += OnPanUpdated;
            myShipImage.GestureRecognizers.Add(tapGestureRecognizer); myShipImage.GestureRecognizers.Add(panGestureRecognizer);
            // Create the Border
            MyShipBorder = new Border
            {
                Stroke = Colors.Green,
                StrokeThickness = 2,
                Content = myShipImage
            };

            AbsoluteLayout.SetLayoutBounds(MyShipBorder, new Rect(x, y, 100, AbsoluteLayout.AutoSize));
            AbsoluteLayout.SetLayoutFlags(MyShipBorder, AbsoluteLayoutFlags.PositionProportional);

            // Add Border to the Layout (assuming you have an AbsoluteLayout named 'absoluteLayout')
            absoluteLayout.Children.Add(MyShipBorder);
        }
        /*
        private async void AnimateImage(Image image)
        {
            // Make the image visible
            image.Opacity = 1;

            // Tasks to wait for animations to complete
            var translateTask = image.TranslateTo(0, -300, MovingTime);
            var fadeTask = image.FadeTo(0, MovingTime);

            // Timer task
            var timerTask = Task.Run(async () =>
            {
                while (!translateTask.IsCompleted)
                {
                    // Get the current position of the bullet
                    var bulletCurrentPosition = AbsoluteLayout.GetLayoutBounds(image);

                    if (IsBulletInsideEnemyBorder(bulletCurrentPosition))
                    {
                        // Bullet hit the enemy ship, perform necessary actions
                        Debug.WriteLine("Bullet hit the enemy ship!");

                        // Update UI on the main thread
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            if (absoluteLayout != null && EnemyShipBorder != null)
                            {
                                absoluteLayout.Children.Remove(EnemyShipBorder);
                            }
                        });
                        break;
                    }

                    // Wait for 100 milliseconds
                    await Task.Delay(100);
                }
            });

            // Wait for all animations to complete
            await Task.WhenAll(translateTask, fadeTask, timerTask);

            // Update UI on the main thread
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (absoluteLayout != null && absoluteLayout.Children.Contains(image))
                {
                    absoluteLayout.Children.Remove(image);
                }
            });
        }
        */
        private bool IsBulletInsideEnemyBorder(Image image)
        {
            if (EnemyShipBorder == null) return false;

            // Get the position and size of the EnemyShipBorder
            var enemyBounds = AbsoluteLayout.GetLayoutBounds(EnemyShipBorder);
            var enemyRect = new Rect(
                enemyBounds.X * absoluteLayout.Width - (EnemyShipBorder.Width / 2),
                enemyBounds.Y * absoluteLayout.Height - (EnemyShipBorder.Height / 2),
                EnemyShipBorder.Width,
                EnemyShipBorder.Height
            );
            /*
            // Create a rectangle for the current position of the bullet
            var bulletBounds = AbsoluteLayout.GetLayoutBounds(bulletImage);
            var bulletRect = new Rect(
                bulletBounds.X * absoluteLayout.Width - (bulletImage.Width / 2),
                bulletBounds.Y * absoluteLayout.Height - (bulletImage.Height / 2),
                bulletImage.Width,
                bulletImage.Height
            );
            */
            // Check if the bullet's bounding box is entirely within the enemy's bounding box
            bool isWithinXBounds = image.X >= enemyRect.Left && image.X <= enemyRect.Right;
            bool isWithinYBounds =image.Y >= enemyRect.Top && image.Y <= enemyRect.Bottom;

            // Additional debugging information
            //Debug.WriteLine($"isWithinXBounds: {isWithinXBounds}");
            //Debug.WriteLine($"isWithinYBounds: {isWithinYBounds}");

            return isWithinXBounds && isWithinYBounds;
        }



        /*
        private bool IsBulletInsideEnemyBorder(Rect bulletBounds, Image bullet)
        {
            if (EnemyShipBorder == null) return false;

            // Get the position and size of the EnemyShipBorder
            var enemyBounds = AbsoluteLayout.GetLayoutBounds(EnemyShipBorder);
            var enemyRect = new Rect(
                enemyBounds.X * absoluteLayout.Width - (EnemyShipBorder.Width / 2),
                enemyBounds.Y * absoluteLayout.Height - (EnemyShipBorder.Height / 2),
                EnemyShipBorder.Width,
                EnemyShipBorder.Height
            );

            // Update the bullet rectangle to current position
            var bulletRect = new Rect(
                bulletBounds.X * absoluteLayout.Width - (bullet.Width / 2),
                bulletBounds.Y * absoluteLayout.Height - (bullet.Height / 2),
                bullet.Width,
                bullet.Height
            );

            // Check if the bullet's bounding box is entirely within the enemy's bounding box
            bool isWithinXBounds = bulletRect.Left >= enemyRect.Left && bulletRect.Right <= enemyRect.Right;
            bool isWithinYBounds = bulletRect.Top >= enemyRect.Top && bulletRect.Bottom <= enemyRect.Bottom;

            // Additional debugging information
            Debug.WriteLine($"EnemyShipBorder Rect: {enemyRect}");
            Debug.WriteLine($"Bullet Rect: {bulletRect}");
            Debug.WriteLine($"isWithinXBounds: {isWithinXBounds}");
            Debug.WriteLine($"isWithinYBounds: {isWithinYBounds}");

            return isWithinXBounds && isWithinYBounds;
        }
        

        */



        // Alt satýrdaki kodlar dokunarak bizim gemimizi hareket ettirmeye yarar
        private async void OnTapped(object sender, EventArgs e)
        {
            if (sender is Image image && image.Parent is Border border)
            {
                double newX = _startX;
                double newY = _startY;

                // Yeni konumu sýnýrlarla kontrol ederek uygula
                double maxX = Width - border.Width;
                double maxY = Height - border.Height;

                newX = Math.Clamp(newX, -maxX, maxX);
                newY = Math.Clamp(newY, -maxY, maxY);

                border.TranslationX = newX;
                border.TranslationY = newY;

                // Asenkron bir iþleme örnek ekle (örneðin, bir log kaydý)
                await Task.Run(() => Console.WriteLine($"Tap moved to X: {newX}, Y: {newY}"));
            }
        }

        private async void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (sender is Image image && image.Parent is Border border)
            {
                switch (e.StatusType)
                {

                    case GestureStatus.Started:
                        // Calculate the point where the user touched
                        _xOffset = e.TotalX;
                        _yOffset = e.TotalY;
                        _startX = border.TranslationX;
                        _startY = border.TranslationY;
                        break;

                    case GestureStatus.Running:
                        // Calculate the new position
                        double newX = _startX + e.TotalX - _xOffset;
                        double newY = _startY + e.TotalY - _yOffset;

                        // Apply new position with boundary checks
                        double maxX = Width - border.Width;
                        double maxY = Height - border.Height;

                        newX = Math.Clamp(newX, -maxX, maxX);
                        newY = Math.Clamp(newY, -maxY, maxY);

                        border.TranslationX = newX;
                        border.TranslationY = newY;
                        double layoutWidth = absoluteLayout.Width; 
                        double layoutHeight= absoluteLayout.Height;
                        // Normalize edilmiþ deðerler .
                        double normalizedX = newX / layoutWidth; 
                        double normalizedY = newY / layoutHeight;
                        _currentX=normalizedX;
                        _currentY=normalizedY;
                        //Debug.WriteLine($"MyShip position: X = {newX}, Y = {newY}, Width = {border.Width}, Height = {border.Height}");
                        //Debug.WriteLine($"Running: newX={newX}, newY={newY}");
                        await Task.Run(() => Console.WriteLine($"Pan running at X: {newX}, Y: {newY}"));
                        break;

                    case GestureStatus.Completed:
                        // Actions to perform when the pan gesture is completed
                        await Task.Run(() => Console.WriteLine("Pan completed"));
                        break;
                }
            }
        }

    }
}
