using Microsoft.Maui.Controls.Compatibility;
using System;
using System.Collections.Generic;
using Grid = Microsoft.Maui.Controls.Grid;
namespace Lab
{
    public partial class MainPage : ContentPage
    {
        GridBoard gridBoard;


        public MainPage()
        {
            InitializeComponent();
            gridBoard = new GridBoard(10, 5);
            ShowBoard();
        }

        private void ShowBoard()
        {
            int childNumber = 0;
            for (int i = 0; i <= gridBoard.board.Count; i++)
            {
                if (i < gridBoard.board.Count)
                {
                    for (int j = 0; j < gridBoard.board[i].Count; j++)
                    {
                        if (j + 1 >= grid.RowDefinitions.Count)
                        {
                            grid.AddRowDefinition(new RowDefinition());
                            var label = new Label()
                            {
                                Text = (j + 1).ToString(),
                                VerticalOptions = LayoutOptions.Center,
                                HorizontalOptions = LayoutOptions.Center
                            };
                            Grid.SetRow(label, j + 1);
                            Grid.SetColumn(label, 0);
                            grid.Children.Add(label);
                        }

                        if (childNumber >= grid.Children.Count - gridBoard.board.Count - gridBoard.board[0].Count)
                        {
                            Entry cell = new Entry
                            {
                                Text = gridBoard.board[i][j].Content,
                                VerticalOptions = LayoutOptions.Center,
                                HorizontalOptions = LayoutOptions.Center
                            };
                            Grid.SetRow(cell, j + 1);
                            Grid.SetColumn(cell, i + 1);
                            grid.Children.Add(cell);
                        }
                        childNumber++;
                    }
                }

                if (i > 0)
                {
                    if (gridBoard.board.Count >= grid.ColumnDefinitions.Count)
                    {
                        grid.AddColumnDefinition(new ColumnDefinition());
                        BoardPosition pos = gridBoard.board[i - 1][0].Position;
                        var label = new Label()
                        {
                            Text = pos.character,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Center
                        };
                        Grid.SetColumn(label, i);
                        Grid.SetRow(label, 0);
                        grid.Children.Add(label);
                    }
                } else
                {
                    grid.AddColumnDefinition(new ColumnDefinition());
                    var label = new Label()
                    {
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center
                    };
                    Grid.SetColumn(label, i);
                    Grid.SetRow(label, 0);
                    grid.Children.Add(label);
                }
                
            }
        }

        private void Entry_Focuded(object sender, FocusEventArgs e)
        {
            var entry = (Entry)sender;
            var row = Grid.GetRow(entry) - 1;
            var col = Grid.GetColumn(entry) - 1;
            var content = entry.Text;

            textInput.Text = content;
        }

        private void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            var entry = (Entry)sender;
            var row = Grid.GetRow(entry) - 1;
            var col = Grid.GetColumn(entry) - 1;
            var content = entry.Text;
            // Додайте додаткову логіку, яка виконується при виході зі зміненої клітинки
        }
        private void CalculateButton_Clicked(object sender, EventArgs e)
        {
            // Обробка кнопки "Порахувати"
        }
        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            // Обробка кнопки "Зберегти"
        }
        private void ReadButton_Clicked(object sender, EventArgs e)
        {
            // Обробка кнопки "Прочитати"
        }
        private async void ExitButton_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Підтвердження", "Ви дійсно хочете вийти?",
            "Так", "Ні");
            if (answer)
            {
                Environment.Exit(0);
            }
        }
        private async void HelpButton_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Довідка", "Лабораторна робота 1. Студента Власенка Захара К-25",
            "OK");
        }
        private void DeleteRowButton_Clicked(object sender, EventArgs e)
        {

        }
        private void DeleteColumnButton_Clicked(object sender, EventArgs e)
        {

        }

        private void AddRowButton_Clicked(object sender, EventArgs e)
        {
            gridBoard.AddRow();
            ShowBoard();
        }
        private void AddColumnButton_Clicked(object sender, EventArgs e)
        {

        }
    }
}