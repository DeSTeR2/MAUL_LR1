using System.Collections.Generic;

namespace Lab
{
    internal class Cell
    {
        int x, y;
        string content = null;
        float calculatedData = float.NaN;
        BoardPosition position;

        HashSet<Cell> references = new(); 
        HashSet<Cell> dependents = new();

        public static Action<Cell> OnDataChanges;

        public Cell(int x, int y, string content)
        {
            this.x = x;
            this.y = y;
            this.content = content;
            this.position = GridBoard.GetCellsBoardPosition(this);
        }

        public Cell(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.position = GridBoard.GetCellsBoardPosition(this);
        }

        public List<Cell> GetReferences() => references.ToList();
        public void AddReference(Cell cell)
        {
            references.Add(cell);
            cell.dependents.Add(this); 
        }

        public void ResetReferences()
        {
            foreach (var reference in references)
            {
                reference.dependents.Remove(this); 
            }
            references.Clear();
        }

        public List<Cell> GetDependents() => dependents.ToList(); 

        public int X => x;
        public int Y => y;

        public string Content
        {
            get => content;
            set
            {
                if (!string.IsNullOrEmpty(value) && value != "NaN")
                {
                    content = value;
                }
            }
        }

        public float CalculatedData
        {
            get => calculatedData;
            set
            {
                if (!float.IsNaN(value))
                {
                    calculatedData = value;
                    OnDataChanges?.Invoke(this);
                }
            }
        }

        public BoardPosition Position => position;
    }
}
