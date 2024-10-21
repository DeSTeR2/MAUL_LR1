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

    }
}
