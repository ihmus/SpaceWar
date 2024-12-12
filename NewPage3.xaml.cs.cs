using Microsoft.Maui.Layouts;
using System;
using System.Diagnostics;

namespace Uygulamam
{
    public partial class NewPage3 : ContentPage
    {
        double _xOffset, _yOffset;
        double _startX, _startY;
        private Random _random = new Random();
        private const int CreateInterval = 200, MovingTime = 1000,callingtime=3000; // Milisaniye cinsinden oluşturma aralığı
        private double _currentX, _currentY;
        private double MyShipX= 0.5, MyShipY= 0.5;
        private string MyShipPath= "spaceship.png", BulletPath= "bullet.png";
        private Border MyShipBorder,EnemyShipBorder;
        private string EnemyShip = "enemyship.png";
        private List<Border> enemyShipBorders = new List<Border>();
        private int score=0;
        private double speed = Math.Sqrt(0.0001);

        public NewPage3()
        {
            InitializeComponent();
            MyShip(MyShipX, MyShipY);
            fired();
            enemyfired();
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
                double x = MyShipBorder.X / absoluteLayout.Width;
                double y = MyShipBorder.Y / absoluteLayout.Height;
                CreateNewBullet(x +0.01, _currentY,true);
                CreateNewBullet(x +0.08, _currentY , true);
                //Debug.WriteLine($"{_currentX},{_currentX},{x},{y}");
                await Task.Delay(CreateInterval); // Yeni Image oluşturma aralığı
                                                  // Example of an async task (e.g., logging)
            }
        }
        private async void enemyfired()
        {
            double layoutWidth = absoluteLayout.Width; double layoutHeight = absoluteLayout.Height;
            if (!enemyShipBorders.Any()) // Liste boşsa {
                Console.WriteLine("Enemy ship borders list is empty.");// Döngüyü sonlandır
            while (true)
            {
                double i= 0.7;
                foreach (var enemyShipBorder in enemyShipBorders.ToList())  // ToList() kullanarak güvenli iterasyon sağlanır
                {
                    double Y = AbsoluteLayout.GetLayoutBounds(enemyShipBorder).Y;
                    double X = AbsoluteLayout.GetLayoutBounds(enemyShipBorder).X;
                    CreateNewBullet(X+0.04 , Y+0.08 , false);
                    Debug.WriteLine($"{Y}{X}oluşturuldu");
                    i += 0.01;
                }
                await Task.Delay(CreateInterval); // Yeni Image oluşturma aralığı
                                                  // Example of an async task (e.g., logging)
            }
        }
        private async void calltheenemies()
        {
            while (true)
            {
                GenerateAndPlaceEnemyShip();
                GenerateAndPlaceEnemyShip();
                GenerateAndPlaceEnemyShip();
                GenerateAndPlaceEnemyShip();
                await Task.Delay(callingtime); // Yeni Image oluşturma aralığı
                                               // Example of an async task (e.g., logging)

            }
        }

        private async void CreateNewBullet(double XPosition, double YPosition,bool ismybullet)
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
            try
            {
                absoluteLayout.Children.Add(newImage);
                MovingBullet(newImage, ismybullet);
                AnimateImage(newImage);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
        private async void MovingBullet(Image image,bool ismybullet)
        {
            double y = AbsoluteLayout.GetLayoutBounds(image).Y;
            DateTime startTime = DateTime.Now;
            while ((DateTime.Now - startTime).TotalMilliseconds < MovingTime)
            {
                await Task.Delay(35);//mermi hızını etkiler ters orantılı
                if (ismybullet) y -= 0.021;
                else y += 0.021;
                // Log the current y position
                //Console.WriteLine($"Current y position: {y}");

                // Update the position
                AbsoluteLayout.SetLayoutBounds(image, new Rect(AbsoluteLayout.GetLayoutBounds(image).X, y, AbsoluteLayout.GetLayoutBounds(image).Width, AbsoluteLayout.GetLayoutBounds(image).Height));
                if (ismybullet) KonumTakipi(image, true);
                else KonumTakipi(image, false);
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

        private async void KonumTakipi(Image image,bool ismybullet)
        {
            //Debug.WriteLine($"{image.X}{image.Y}");
            if (ismybullet)
            {
                foreach (var enemyShipBorder in enemyShipBorders.ToList())  // ToList() kullanarak güvenli iterasyon sağlanır
                {
                    if (IsBulletInsideEnemyBorder(image, enemyShipBorder))
                    {
                        if (absoluteLayout != null)
                        {
                            absoluteLayout.Children.Remove(enemyShipBorder);  // Orijinal listeden çıkar
                            enemyShipBorders.Remove(enemyShipBorder);  // Orijinal listeden çıkar
                        }
                    }
                }
            }
            else { 
                if(IsBulletInsideEnemyBorder(image, MyShipBorder))
                {
                    absoluteLayout.Children.Remove(MyShipBorder);  // Orijinal listeden çıkar
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
            MovingEnemies(EnemyShipBorder);

        }
        private async void  MovingEnemies(Border EnemyShipBorder)
        {
            double y = AbsoluteLayout.GetLayoutBounds(EnemyShipBorder).Y;
            double x = AbsoluteLayout.GetLayoutBounds(EnemyShipBorder).X;
            double myshipx = _currentX ;
            double myshipy = _currentY ;

            while (true)
            {
                y = AbsoluteLayout.GetLayoutBounds(EnemyShipBorder).Y;
                x = AbsoluteLayout.GetLayoutBounds(EnemyShipBorder).X;
                myshipx = _currentX ;
                myshipy = _currentY ;
                await Task.Delay(16);
                y -= speed * (double)((y - myshipy) / Math.Sqrt((Math.Pow((y - myshipy), 2) + Math.Pow((x - myshipx), 2))));
                x -=speed * (double)((x - myshipx) / Math.Sqrt((Math.Pow((y - myshipy), 2) + Math.Pow((x - myshipx), 2))));

                // Yeni pozisyonu güncelle
                //AbsoluteLayout.SetLayoutBounds(EnemyShipBorder, new Rect(AbsoluteLayout.GetLayoutBounds(EnemyShipBorder).X, y, AbsoluteLayout.GetLayoutBounds(EnemyShipBorder).Width, AbsoluteLayout.GetLayoutBounds(EnemyShipBorder).Height));

                AbsoluteLayout.SetLayoutBounds(EnemyShipBorder, new Rect(x, y, AbsoluteLayout.GetLayoutBounds(EnemyShipBorder).Width, AbsoluteLayout.GetLayoutBounds(EnemyShipBorder).Height));

                // Sınır kontrolü yap
                var enemyBounds = AbsoluteLayout.GetLayoutBounds(EnemyShipBorder);
                if (y >= 1 || y < 0)
                {
                    // Y dışına çıkma durumu
                    //Debug.WriteLine($"{y}{x}{myshipy}{myshipx}");
                    if (absoluteLayout != null)
                    {
                        absoluteLayout.Children.Remove(EnemyShipBorder);  // Orijinal listeden çıkar
                        enemyShipBorders.Remove(EnemyShipBorder);  // Orijinal listeden çıkar
                    }
                    break;
                }
            }

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
       
        private bool IsBulletInsideEnemyBorder(Image image,Border enemshipborder)
        {
            if (enemshipborder == null) return false;

            // Get the position and size of the EnemyShipBorder
            var enemyBounds = AbsoluteLayout.GetLayoutBounds(enemshipborder);
            var enemyRect = new Rect(
                enemyBounds.X * absoluteLayout.Width - (enemshipborder.Width / 2),
                enemyBounds.Y * absoluteLayout.Height - (enemshipborder.Height / 2),
                enemshipborder.Width,
                enemshipborder.Height
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





        // Alt satırdaki kodlar dokunarak bizim gemimizi hareket ettirmeye yarar
        private async void OnTapped(object sender, EventArgs e)
        {
            if (sender is Image image && image.Parent is Border border)
            {
                double newX = _startX;
                double newY = _startY;

                // Yeni konumu sınırlarla kontrol ederek uygula
                double maxX = Width - border.Width;
                double maxY = Height - border.Height;

                newX = Math.Clamp(newX, -maxX, maxX);
                newY = Math.Clamp(newY, -maxY, maxY);

                border.TranslationX = newX;
                border.TranslationY = newY;

                // Asenkron bir işleme örnek ekle (örneğin, bir log kaydı)
                await Task.Run(() => Console.WriteLine($"Tap moved to X: {newX}, Y: {newY}"));
            }
        }

        private async void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (sender is Image image && image.Parent is Border border)
            {
                double screenWidth = absoluteLayout.Width;
                double screenHeight = absoluteLayout.Height;

                switch (e.StatusType)
                {
                    case GestureStatus.Started:
                        // Kullanıcının dokunduğu noktayı hesapla
                        _xOffset = (e.TotalX - (screenWidth / 2)) / screenWidth;
                        _yOffset = (e.TotalY - (screenHeight / 2)) / screenHeight;
                        _startX = border.TranslationX;
                        _startY = border.TranslationY;
                        break;

                    case GestureStatus.Running:
                        // Normalize edilmiş pan hareketi değerleri
                        double normalizedTotalX = e.TotalX / screenWidth;
                        double normalizedTotalY = e.TotalY / screenHeight;

                        // Yeni konumu hesapla
                        double newX = _startX + (normalizedTotalX - _xOffset) * screenWidth;
                        double newY = _startY + (normalizedTotalY - _yOffset) * screenHeight;

                        // Sınır kontrolü ile yeni konumu uygula
                        double maxX = Width - border.Width;
                        double maxY = Height - border.Height;

                        newX = Math.Clamp(newX, -maxX, maxX);
                        newY = Math.Clamp(newY, -maxY, maxY);
                        /*
                        border.TranslationX = newX;
                        border.TranslationY = newY;
                        */
                        double layoutWidth = absoluteLayout.Width;
                        double layoutHeight = absoluteLayout.Height;

                        // Normalize edilmiş değerler
                        double normalizedX = (newX / layoutWidth) ;
                        double normalizedY = (newY / layoutHeight) ;

                        _currentX = normalizedX;
                        _currentY = normalizedY;
                        AbsoluteLayout.SetLayoutBounds(MyShipBorder, new Rect(_currentX, _currentY, AbsoluteLayout.GetLayoutBounds(MyShipBorder).Width, AbsoluteLayout.GetLayoutBounds(MyShipBorder).Height));
                        // Pan hareketi sırasında console'a yazdır
                        await Task.Run(() => Console.WriteLine($"Pan running at X: {newX}, Y: {newY}"));
                        break;

                    case GestureStatus.Completed:
                        // Pan hareketi tamamlandığında yapılacak işlemler
                        await Task.Run(() => Console.WriteLine("Pan completed"));
                        break;
                }
            }
        }


    }
}
