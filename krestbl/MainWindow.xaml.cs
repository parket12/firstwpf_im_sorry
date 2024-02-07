using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Linq;
using System;
using System.Globalization;
using System.Windows.Data;

namespace krestbl
{

    public partial class MainWindow : Window
    {
        public GameBoard MyGameBoard = new GameBoard();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = MyGameBoard;
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MyGameBoard.HasWon)
                return;

            var clickedButton = sender as Button;

            if (MyGameBoard.currentPlayer == GameBoard.CurrentPlayer.X)
            {
                clickedButton.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#811717"));
            }
            else if (MyGameBoard.currentPlayer == GameBoard.CurrentPlayer.O)
            {
                clickedButton.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#126712"));
            }
            clickedButton.Background = Brushes.WhiteSmoke;
            clickedButton.Content = MyGameBoard.currentPlayer;
            clickedButton.IsHitTestVisible = false;

            MyGameBoard.UpdateBoard(clickedButton.Name);

            if (!MyGameBoard.HasWon)
            {
                MakeRobotMove();
            }
        }

        private void MakeRobotMove()
        {

            var availableButtons = MyGrid.Children.OfType<Button>()
                .Where(button => button.Content == null)
                .ToList();

            if (availableButtons.Count > 0)
            {

                var random = new Random();
                var randomButton = availableButtons[random.Next(availableButtons.Count)];

                randomButton.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#126712"));
                randomButton.Background = Brushes.WhiteSmoke;
                randomButton.Content = MyGameBoard.currentPlayer;
                randomButton.IsHitTestVisible = false;

                MyGameBoard.UpdateBoard(randomButton.Name);
            }


            var allButtons = MyGrid.Children.OfType<Button>().ToList();

            if (availableButtons.Count == 0 && !MyGameBoard.HasWon)
            {
                MyGameBoard.HasWon = true;
                messageLabel.Content = "Ничья!";
                messageLabel.Visibility = Visibility.Visible;
            }


            if (MyGameBoard.HasWon)
            {
                messageLabel.Content = $"Игрок {MyGameBoard.currentPlayer} победил!";
                messageLabel.Visibility = Visibility.Visible;
            }
        }


        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            foreach (var child in MyGrid.Children.OfType<Button>())
            {
                child.Content = null;
                child.IsHitTestVisible = true;
                child.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDDDDDD"));
            }

            MyGameBoard = new GameBoard();
            this.DataContext = MyGameBoard;


            if (MyGameBoard.HasWon)
            {
                messageLabel.Content = $"Игрок {MyGameBoard.currentPlayer} победил!";
                messageLabel.Visibility = Visibility.Visible;
            }

            if (MyGameBoard.HasWon)
            {
                messageLabel.Content = "Ничья!";
                messageLabel.Visibility = Visibility.Visible;
            }
        }

        public class GameBoard : INotifyPropertyChanged
        {
            public enum CurrentPlayer
            {
                X = 1,
                O
            }

            private int turn = 1;
            public CurrentPlayer currentPlayer = CurrentPlayer.X;
            private bool hasWon = false;
            public bool HasWon
            {
                get { return hasWon; }
                set { hasWon = value; NotifyPropertyChanged("HasWon"); }
            }

            private Dictionary<string, int> board = new Dictionary<string, int>()
    {
        {"TopXLeft",0 },
        {"TopXMiddle",0 },
        {"TopXRight",0 },
        {"CenterXLeft",0 },
        {"CenterXMiddle",0 },
        {"CenterXRight",0 },
        {"BottomXLeft",0 },
        {"BottomXMiddle",0 },
        {"BottomXRight",0 }
    };

            public void NotifyPropertyChanged(string info)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
            }

            public event PropertyChangedEventHandler PropertyChanged;

            private bool CheckIfWon(string buttonName)
            {
                if (WonInRow(buttonName))
                {
                    return true;
                }
                else if (WonInColumn(buttonName))
                {
                    return true;
                }
                else if (WonInDiagonal(buttonName))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            private bool WonInRow(string name)
            {
                string row = name.Substring(0, name.IndexOf('X') - 1);

                foreach (var element in board)
                {
                    string keyName = element.Key;
                    string rowOfElement = keyName.Substring(0, keyName.IndexOf('X') - 1);

                    if (rowOfElement == row)
                    {
                        if (element.Value != (int)currentPlayer)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            private bool WonInColumn(string name)
            {
                string column = name.Substring(name.IndexOf('X') + 1);

                foreach (var element in board)
                {
                    string keyName = element.Key;
                    string columnOfElement = keyName.Substring(keyName.IndexOf('X') + 1);

                    if (columnOfElement == column)
                    {
                        if (element.Value != (int)currentPlayer)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            private bool WonInDiagonal(string name)
            {
                if (name == "TopXLeft" || name == "CenterXMiddle" || name == "BottomXRight")
                {
                    if (board["CenterXMiddle"] == (int)currentPlayer && board["BottomXRight"] == (int)currentPlayer && board["TopXLeft"] == (int)currentPlayer)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                if (name == "TopXRight" || name == "CenterXMiddle" || name == "BottomXLeft")
                {
                    if (board["CenterXMiddle"] == (int)currentPlayer && board["BottomXLeft"] == (int)currentPlayer && board["TopXRight"] == (int)currentPlayer)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            private void UpdateDictionary(string buttonName)
            {
                board[buttonName] = (int)currentPlayer;
            }

            public void UpdateBoard(string buttonName)
            {
                UpdateDictionary(buttonName);

                if (turn >= 5)
                {
                    if (CheckIfWon(buttonName))
                    {
                        HasWon = true;
                    }
                }

                turn++;

                if (currentPlayer == CurrentPlayer.X)
                {
                    currentPlayer = CurrentPlayer.O;
                }
                else if (currentPlayer == CurrentPlayer.O)
                {
                    currentPlayer = CurrentPlayer.X;
                }

                NotifyPropertyChanged("HasWon"); 
            }
        }
    }
}