using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;

namespace Lab
{
    public struct BoardPosition
    {
        public string character;
        public int number;

        public string GetFullPosition()
        {
            return character + number.ToString();
        }
    }

    internal class GridBoard
    {
        public List<List<Cell>> board { get; private set; } = new List<List<Cell>>();
        Dictionary<string, float> calculatedCells = new();

        HashSet<Cell> cellsInProgress = new();
        List<string> errors = new();

        public int column { get; private set; }
        public int row { get; private set; }

        public GridBoard(int column, int row)
        {
            this.column = column;
            this.row = row;

            CreateGrid();
        }

        private void CreateGrid()
        {
            for (int i=0; i<row; i++)
            {
                List<Cell> cellRow = new List<Cell>();
                for (int j = 0; j < column; j++) {
                    cellRow.Add(new Cell(i, j));
                }
                board.Add(cellRow);
            }
        }

        public void AddRow()
        {
            List<Cell> cellRow = new List<Cell>();
            for (int j = 0; j < column; j++)
            {
                cellRow.Add(new Cell(board.Count, j));
            }
            board.Add(cellRow);
            row++;
        }

        public void AddColumn() { 
            for (int i=0; i<row; i++)
            {
                board[i].Add(new Cell(i, column));
            }
            column++;
        }

        public void DeleteRow()
        {
            board.Remove(board[^1]);
            row--;
        }

        public void DeleteColumn()
        {
            for (int i = 0; i < row; i++)
            {
                board[i].Remove(board[i][^1]);
            }
            column--;
        }

        public static BoardPosition GetCellsBoardPosition(Cell cell)
        {
            BoardPosition position = new BoardPosition();
            position.number = cell.X + 1;

            int y = cell.Y;
            while (y >= 0)
            {
                char letter = (char)(y % 26 + 'A'); 
                position.character = letter + position.character;                 
                
                if (y < 26) break;
                y /= 26;
            }

            return position;
        }

        private Cell GetCellFromBoardPosition(string boardPosition)
        {
            boardPosition = boardPosition.Substring(1, boardPosition.Length-2);

            string character = "";
            int number = 0;

            int i;
            for (i = 0; i < boardPosition.Length; i++)
            {
                if (char.IsDigit(boardPosition[i]))
                    break;
                character += boardPosition[i];
            }

            string numberString = boardPosition.Substring(i);
            int.TryParse(numberString, out number);

            int column = 0;
            for (int j = 0; j < character.Length; j++)
            {
                column *= 26;
                column += (character[j] - 'A' + 1);
            }
            column -= 1;

            int row = number - 1;

            return board[row][column];  
        }

        public bool CheckCellSyntax(int row, int column)
        {
            Cell cell = board[row][column];
            string content = cell.Content;

            bool res = FormulaEvaluator.IsCalculable(content, ref errors);
            return res;
        }

        private bool UpdateDependents(Cell cell)
        {
            if (cellsInProgress.Contains(cell)) return false;
            cellsInProgress.Add(cell);

            foreach (var dependent in cell.GetDependents())
            {
                string boardPosition = "{" + dependent.Position.GetFullPosition() + "}";
                if (calculatedCells.ContainsKey(boardPosition))
                {
                    calculatedCells.Remove(boardPosition);
                }

                GetEvaluation(dependent.X, dependent.Y);
                UpdateDependents(dependent);
            }
            cellsInProgress.Remove(cell);
            return true;
        }

        public float GetEvaluation(int row, int column)
        {
            Cell cell = board[row][column];
            string content = cell.Content;
            string boardPosition = "{" + cell.Position.GetFullPosition() + "}";


            if (calculatedCells.ContainsKey(boardPosition))
                return calculatedCells[boardPosition];

            List<string> links = FormulaEvaluator.GetAllCellLinks(content);

            if (cellsInProgress.Contains(cell))
            {
                errors.Add("A cell cannot depend on itself.");
                return float.NaN;
            }

            cellsInProgress.Add(cell);

            foreach (string link in links)
            {
                Cell referencedCell = GetCellFromBoardPosition(link);

                if (!float.IsNaN(GetEvaluation(referencedCell.X, referencedCell.Y)))
                {
                    cell.AddReference(referencedCell); 
                }
            }

            float evaluation = FormulaEvaluator.Evaluate(content, calculatedCells, ref errors);
            calculatedCells[boardPosition] = evaluation;
            cell.CalculatedData = evaluation;

            cellsInProgress.Remove(cell);
            return evaluation;
        }

        public List<string> GetErrors()
        {
            List<string> errors = new List<string>(this.errors);
            this.errors = new();
            return errors;
        }

        public void ChangeContent(int row, int column, string content)
        {
            Cell cell = board[row][column];
            cell.Content = content;
            string position = "{" + cell.Position.GetFullPosition() + "}";

            if (calculatedCells.ContainsKey(position))
            {
                calculatedCells.Remove(position);
            }
        }

        public void ChangeContent(Cell cell, ref List<string> errors)
        {
            string position = "{" + cell.Position.GetFullPosition() + "}";
            if (calculatedCells.ContainsKey(position))
            {
                calculatedCells.Remove(position);
            }

            int row = cell.X;
            int column = cell.Y;

            cellsInProgress = new();

            cell.ResetReferences();
            GetEvaluation(row, column);
            if (UpdateDependents(cell) == false)
            {
                cell.Content = "#SELFREF";
                cell.CalculatedData = 0;
                errors.Add("A cell cannot depend on itself.");
            }

            errors.AddRange(GetErrors());
        }

        public void EvaluateBoard()
        {
            for (int i = 0; i < board.Count; i++) {
                for (int j = 0; j < board[i].Count; j++) { 
                    ChangeContent(board[i][j], ref errors);
                }
            }
        }
    }
}
