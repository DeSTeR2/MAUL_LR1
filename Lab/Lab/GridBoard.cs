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
            List<string> errors = new();

            bool res = FormulaEvaluator.IsCalculable(content, out errors);
            return res;
        }

        public float GetEvaluation(int row, int column)
        {
            Cell cell = board[row][column];
            string content = cell.Content;
            string boardPosition = cell.Position.GetFullPosition();
            boardPosition = "{" + boardPosition + "}";

            if (calculatedCells.ContainsKey(boardPosition)) {
                return calculatedCells[boardPosition];
            }

            List<string> links = FormulaEvaluator.GetAllCellLinks(content);
            List<Cell> requareToCalculate = new();

            foreach (string link in links)
            {
                if (calculatedCells.ContainsKey(link) == false)
                {
                    Cell cellToCalc = GetCellFromBoardPosition(link);
                    requareToCalculate.Add(cellToCalc);

                    GetEvaluation(cellToCalc.X, cellToCalc.Y);
                }
            }

            if (errors.Count == 0)
            {
                List<string> linkCalcErrors = new();
                float evaluation = FormulaEvaluator.Evaluate(content, calculatedCells, out linkCalcErrors);

                if (linkCalcErrors.Count == 0 && evaluation != float.NaN)
                {
                    calculatedCells.Add(boardPosition, evaluation);
                    cell.CalculatedData = evaluation;
                    return evaluation;
                }
                errors.AddRange(linkCalcErrors);
            }
            return float.NaN; 
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

            string position = cell.Position.GetFullPosition();

            if (calculatedCells.ContainsKey(position))
            {
                calculatedCells.Remove(position);
            }
        }
    }
}
