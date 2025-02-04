﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
namespace hungaryTDv1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Rectangle background;
        public Label lblMouseTest;
        public Button tempTwrBtn;
        public Rectangle tempRect;
        public bool mouseTest;
        public Button[] towerIcons = new Button[4];
        Button btnStart = new Button();
        public ImageBrush[] towerFill = new ImageBrush[4];
        System.Windows.Threading.DispatcherTimer gameTimer = new System.Windows.Threading.DispatcherTimer();
        public GameState gameState;
        public enum GameState {play, store, test};
        public TowerType towerType;
        public enum TowerType {normal, police, family, tank}
        public EnemyType enemyType;
        public enum EnemyType {apple, pizza, donut, hamburger, fries}
        public List<Enemy> enemies = new List<Enemy>();
        public Polygon trackHit = new Polygon();
        public Point[] track = new Point[1450];
        public int[] positions = new int[1450];
        StreamWriter sw;
        StreamReader sr;
        public MainWindow()
        {
            InitializeComponent();
            btnStart.Height = 20;
            btnStart.Width = 70;
            btnStart.Content = "start";
            btnStart.Click += BtnStart_Click;
            cBackground.Children.Add(btnStart);
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = -1;
            }
            sr = new StreamReader("trackLine.txt");
            int counter = 0;
            while (!sr.EndOfStream)
            {
                string currentLine = sr.ReadLine();
                double xPosition, yPosition;
                double.TryParse(currentLine.Split(',')[0], out xPosition);
                double.TryParse(currentLine.Split(',')[1], out yPosition);
                Point point = new Point(xPosition, yPosition);
                track[counter] = point;
                counter++;
            }
            sr.Close();
            gameState = GameState.play;
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (gameState == GameState.store)
            {
                Canvas.SetTop(tempRect, Mouse.GetPosition(cBackground).Y - tempRect.Height / 2);
                Canvas.SetLeft(tempRect, Mouse.GetPosition(cBackground).X - tempRect.Width / 2);
                bool valid = true;
                double x = Mouse.GetPosition(cBackground).X;
                double y = Mouse.GetPosition(cBackground).Y;
                bool check1 = cObstacles.InputHitTest(new Point(x + tempRect.Width / 2, y + tempRect.Height / 2)) == null;
                bool check2 = cObstacles.InputHitTest(new Point(x - tempRect.Width / 2, y + tempRect.Height / 2)) == null;
                bool check3 = cObstacles.InputHitTest(new Point(x + tempRect.Width / 2, y - tempRect.Height / 2)) == null;
                bool check4 = cObstacles.InputHitTest(new Point(x - tempRect.Width / 2, y - tempRect.Height / 2)) == null;
                if (check1 && check2 && check3 && check4)
                {
                    valid = true;
                    tempRect.Stroke = Brushes.Transparent;
                }
                else
                {
                    valid = false;
                    tempRect.Stroke = Brushes.Red;
                    tempRect.StrokeThickness = 5;
                }
                MouseButtonState pmbs = MouseButtonState.Released;
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    if (valid)
                    {
                        Point temp = Mouse.GetPosition(cBackground);
                        Rectangle tempRect2 = new Rectangle();
                        tempRect2.Fill = tempRect.Fill;
                        tempRect2.Width = tempRect.ActualWidth;
                        tempRect2.Height = tempRect.ActualHeight;
                        Canvas.SetTop(tempRect2, Canvas.GetTop(tempRect));
                        Canvas.SetLeft(tempRect2, Canvas.GetLeft(tempRect));
                        cBackground.Children.Remove(tempRect);
                        cBackground.Children.Add(tempRect2);
                        cObstacles.Children.Add(tempRect);
                    }
                    else
                    {
                        cBackground.Children.Remove(tempRect);
                    }
                    cObstacles.Children.Remove(trackHit);
                    gameState = GameState.play;
                }
                else
                {
                    pmbs = Mouse.LeftButton;
                }
            }
            else if (gameState == GameState.test)
            {
                /*
                 * MouseButtonState pmbs = MouseButtonState.Released;
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    Point temp = Mouse.GetPosition(cBackground);
                    try
                    {
                        sw.WriteLine(temp.ToString());
                    }
                    catch (FileNotFoundException) //exception for if the file does not exist
                    {
                        sw.WriteLine(temp.ToString());
                    }
                }
                else
                {
                    pmbs = Mouse.LeftButton;
                }
                */
            }
            else if (gameState == GameState.play)
            {
                for (int i = enemies.Count - 1; i > -1; i--)
                {
                    enemies[i].update(i);
                }
            }
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000/60);
            gameTimer.Start();

            sr = new StreamReader("trackBox.txt");
            PointCollection myPointCollection = new PointCollection();
            while (!sr.EndOfStream)
            {
                string currentLine = sr.ReadLine();
                double xPosition, yPosition;
                double.TryParse(currentLine.Split(',')[0], out xPosition);
                double.TryParse(currentLine.Split(',')[1], out yPosition);
                Point point = new Point(xPosition, yPosition);
                myPointCollection.Add(point);
            }
            sr.Close();
            trackHit.Points = myPointCollection;
            trackHit.Fill = Brushes.Transparent;

            cBackground.Children.Remove(btnStart);
            background = new Rectangle();
            background.Height = 650;
            background.Width = 1125;
            BitmapImage bi = new BitmapImage(new Uri("track.png", UriKind.Relative));
            ImageBrush img = new ImageBrush(bi);
            background.Fill = img;
            cBackground.Children.Add(background);

            tempTwrBtn = new Button();
            tempTwrBtn.Height = 20;
            tempTwrBtn.Width = 40;
            tempTwrBtn.Content = "test";
            tempTwrBtn.Click += TempTwrBtn_Click;
            Canvas.SetTop(tempTwrBtn, 17);
            Canvas.SetLeft(tempTwrBtn, 857);
            cBackground.Children.Add(tempTwrBtn);

            bi = new BitmapImage(new Uri("normal.png", UriKind.Relative));
            towerFill[0] = new ImageBrush(bi);
            bi = new BitmapImage(new Uri("police.png", UriKind.Relative));
            towerFill[1] = new ImageBrush(bi);
            bi = new BitmapImage(new Uri("family.png", UriKind.Relative));
            towerFill[2] = new ImageBrush(bi);
            bi = new BitmapImage(new Uri("tank.png", UriKind.Relative));
            towerFill[3] = new ImageBrush(bi);
            for (int i = 0; i < towerIcons.Length; i++)
            {
                towerIcons[i] = new Button();
                towerIcons[i].Background = towerFill[i];
                towerIcons[i].Height = 80;
                towerIcons[i].Width = 80;
                towerIcons[i].Click += iconsClick;
                towerIcons[i].BorderBrush = Brushes.Transparent;
                Canvas.SetTop(towerIcons[i], i * 150 + 60);
                Canvas.SetLeft(towerIcons[i], 910);
                cBackground.Children.Add(towerIcons[i]);
            }
            gameState = GameState.play;
            enemies.Add(new Enemy((int)EnemyType.hamburger, cEnemies, cBackground, track, positions));
            enemies.Add(new Enemy((int)EnemyType.apple, cEnemies, cBackground, track, positions));
            enemies.Add(new Enemy((int)EnemyType.pizza, cEnemies, cBackground, track, positions));
            enemies.Add(new Enemy((int)EnemyType.pizza, cEnemies, cBackground, track, positions));
            enemies.Add(new Enemy((int)EnemyType.pizza, cEnemies, cBackground, track, positions));
        }

        private void TempTwrBtn_Click(object sender, RoutedEventArgs e)
        {
                gameState = GameState.test;
        }
        private void iconsClick(object sender, RoutedEventArgs e)
        {
            //sw.Close();
            gameState = GameState.store;
            cObstacles.Children.Add(trackHit);
            Button button = sender as Button;
            int towerType = -1;
            for (int i = 0; i < towerIcons.Length; i++)
            {
                if (towerIcons[i] == button)
                {
                    towerType = i;
                }
            }
            tempRect = new Rectangle();
            tempRect.Fill = towerFill[towerType];
            if (towerType < 2)
            {
                tempRect.Height = 35;
                tempRect.Width = 35;
            }
            else if (towerType == 2)
            {
                tempRect.Height = 45;
                tempRect.Width = 70;
            }
            else
            {
                tempRect.Height = 70;
                tempRect.Width = 70;
            }
            cBackground.Children.Add(tempRect);
        }
    }
}