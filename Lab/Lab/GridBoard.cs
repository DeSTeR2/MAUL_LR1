using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

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

        int column, row;
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
                    cellRow.Add(new Cell(i, j, "Test contrent"));
                }
                board.Add(cellRow);
            }
        }

        public void AddRow()
        {
            List<Cell> cellRow = new List<Cell>();
            for (int j = 0; j < column; j++)
            {
                cellRow.Add(new Cell(board.Count, j, "Test contrent"));
            }
            board.Add(cellRow);
            row++;
        }

        public static BoardPosition GetCellsBoardPosition(Cell cell)
        {
            BoardPosition position = new BoardPosition();
            position.number = cell.Y;

            int y = cell.X;
            while (y >= 0)
            {
                char letter = (char)(y % 26 + 'A'); 
                position.character = letter + position.character;                 
                
                if (y < 26) break;
                y /= 26;
            }

            return position;
        }

        public bool CheckCellSyntax(int row, int column)
        {
            Cell cell = board[row][column];
            string content = cell.Content;
            List<string> errors = new();

            bool res = FormulaEvaluator.IsCalculable(content, out errors);
            return res;
        }

        public void ChangeContent(int row, int column, string content)
        {
            Cell cell = board[row][column];
            cell.Content = content;
        }
    }
}
