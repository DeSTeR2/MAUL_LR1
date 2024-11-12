using Grid = Microsoft.Maui.Controls.Grid;
using Microsoft.Maui.Controls;
using static System.Net.Mime.MediaTypeNames;
using CommunityToolkit.Maui.Storage;
namespace Lab
{
    public partial class MainPage : ContentPage
    {
        GridBoard gridBoard;
        Entry focusedCell;
        Cell focusedBoardCell;
        FileSystem fileSystem;

        public MainPage(IFileSaver fileSaver)
        {
            InitializeComponent();
            gridBoard = new GridBoard(11, 10);
            ShowBoard();

            fileSystem = new FileSystem(fileSaver);
        }

        private void ShowBoard()
        {
            int rowCount = gridBoard.board.Count;
            int columnCount = gridBoard.board[0].Count;

            grid.Children.Clear();
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();

            grid.AddRowDefinition(new RowDefinition { Height = GridLength.Auto });
            grid.AddColumnDefinition(new ColumnDefinition { Width = GridLength.Auto });

            for (int j = 0; j < columnCount; j++)
            {
                var columnHeader = new Label
                {
                    Text = GetExcelColumnName(j + 1),  
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    FontAttributes = FontAttributes.Bold
                };
                Grid.SetRow(columnHeader, 0);
                Grid.SetColumn(columnHeader, j + 1);
                grid.Children.Add(columnHeader);

                grid.AddColumnDefinition(new ColumnDefinition { Width = GridLength.Star });
            }

            for (int i = 0; i < rowCount; i++)
            {
                var rowHeader = new Label
                {
                    Text = (i + 1).ToString(),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    FontAttributes = FontAttributes.Bold
                };
                Grid.SetRow(rowHeader, i + 1);
                Grid.SetColumn(rowHeader, 0);
                grid.Children.Add(rowHeader);

                grid.AddRowDefinition(new RowDefinition { Height = GridLength.Auto });
            }

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    Entry cell = new Entry
                    {
                        Text = gridBoard.board[i][j].Content,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center
                    };

                    cell.Focused += Entry_Focused;
                    cell.Unfocused += Entry_Unfocused;

                    Grid.SetRow(cell, i + 1);
                    Grid.SetColumn(cell, j + 1);
                    grid.Children.Add(cell);
                }
            }
        }

        private string GetExcelColumnName(int index)
        {
            string columnName = "";
            while (index > 0)
            {
                index--;  
                columnName = (char)('A' + (index % 26)) + columnName;
                index /= 26;
            }
            return columnName;
        }



        private void Entry_Focused(object sender, FocusEventArgs e)
        {
            var entry = (Entry)sender;
            var row = Grid.GetRow(entry) - 1;
            var col = Grid.GetColumn(entry) - 1;
            var content = entry.Text;

            focusedCell = entry;
            focusedBoardCell = gridBoard.board[row][col];
            textInput.Text = content;

            entry.Text = focusedBoardCell.Content;

            gridBoard.ChangeContent(row, col, content);
        }

        private void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            var entry = (Entry)sender;
            var row = Grid.GetRow(entry) - 1;
            var col = Grid.GetColumn(entry) - 1;
            var content = entry.Text;

            if (focusedBoardCell.Content != null || focusedBoardCell.Content != "")
            {
                focusedCell.Text = focusedBoardCell.CalculatedData.ToString();
            }

            focusedBoardCell.Content = content;
            Evaluate();
        }

        private void PlaceHolderUnFocus(object sender, FocusEventArgs e)
        {
            var placeHolder = (Entry)sender;
            string text = placeHolder.Text;

            focusedCell.Text = text;
            focusedBoardCell.Content = text;
            focusedCell.TextColor = Colors.Aqua;
        }

        private async void CalculateButton_Clicked(object sender, EventArgs e)
        {
            Evaluate();
        }
        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            fileSystem.Save(gridBoard);
        }
        private async void ReadButton_Clicked(object sender, EventArgs e)
        {
            GridBoard board = await fileSystem.LoadAsync();
            if (board == null)
            {
                await DisplayAlert("Помилка зчитування", "Ви обрали не валідний файл!", "OK");
            }

            gridBoard = board;
            ShowBoard();
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
            await DisplayAlert("Довідка", "Лабораторна робота 1. Студента Власенка Захара К-25. Варіант 59: реалізувати операції +, - (унарні операції);піднесення у степінь; іnc, dec; =, <, >; not;",
            "OK");
        }
        
        private void DeleteRowButton_Clicked(object sender, EventArgs e)
        {
            gridBoard.DeleteRow();
            ShowBoard();
        }
        private void DeleteColumnButton_Clicked(object sender, EventArgs e)
        {
            gridBoard.DeleteColumn();
            ShowBoard();
        }

        private void AddRowButton_Clicked(object sender, EventArgs e)
        {
            gridBoard.AddRow();
            ShowBoard();
        }
        private void AddColumnButton_Clicked(object sender, EventArgs e)
        {
            gridBoard.AddColumn();
            ShowBoard();
        }

        private void ApplyData(object sender, EventArgs e)
        {
            var placeHolder = (Entry)sender;
            string text = placeHolder.Text;

            focusedCell.Text = text;
            focusedBoardCell.Content = text;

            focusedCell.TextColor = Colors.Aqua;
        }

        private async void Evaluate()
        {
            List<string> errors = new();
            float evaluation = gridBoard.GetEvaluation(focusedBoardCell.X, focusedBoardCell.Y);
            errors = gridBoard.GetErrors();

            if (errors.Count == 0)
            {
                focusedCell.Text = evaluation.ToString();
            }
            else
            {
                string error = "";
                for (int i = 0; i < errors.Count; i++)
                {
                    error += errors[i] + "\n";
                }

                await DisplayAlert("Error!", error, "OK");
            }
        }
    }
}