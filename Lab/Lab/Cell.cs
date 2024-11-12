using System.ComponentModel.Design;

namespace Lab
{
    internal class Cell
    {
        int x, y;
        string content = null;
        float calculatedData = 0;
        BoardPosition position;

        List<Cell> references = new();

        public Cell(int x,int y, string content)
        {
            this.x = x;
            this.y = y;
            this.content = content;

            this.position = GridBoard.GetCellsBoardPosition(this);
        }

        public Cell(int x,int y)
        {
            this.x = x;
            this.y = y;

            this.position = GridBoard.GetCellsBoardPosition(this);
        }

        public void AddReference(Cell cell)
        {
            references.Add(this);
        }

        public void DeleteReference(Cell cell)
        {
            if (references.Contains(cell)) {
                references.Remove(cell);
            }
        }

        public int X { get { return x; } }
        public int Y { get { return y; } }

        public int X_boardPos { get { return y; } }
        public int Y_boardPos { get { return x; } }

        public string Content { 
            get { return content; } 
            set {
                if (value != "Nan")
                {
                    content = value;
                }
            } 
        }

        public float CalculatedData { 
            get { return calculatedData; } 
            set {
                if (value != float.NaN) {
                    calculatedData = value;
                }
            } 
        }

        public BoardPosition Position { get { return position; } }
    }
}
