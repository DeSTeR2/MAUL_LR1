using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab
{
    class BoardSerializer
    {
        string startHash = "MySuperSerializeSystem";

        public BoardSerializer() { }

        public string Serialize(GridBoard board)
        {
            string text = startHash + "\n";
            int column = board.column;
            int row = board.row;

            text += $"{column} {row}\n";

            for (int i=0; i < row; i++)
            {
                for (int j=0; j < column; j++)
                {
                    string content = board.board[i][j].Content;
                    content = content.Replace(" ", "");
                    text += content + " ";
                }
                text += "\n";
            }
            return text;
        }

        public GridBoard DeSerialize(string text)
        {
            int column = 0;
            int row = 0;

            string[] textRows = text.Split('\n');
            if (textRows[0] != startHash) return null;

            string[] sizes = textRows[1].Split(" ");
            int.TryParse(sizes[0], out column);
            int.TryParse(sizes[1], out row);

            GridBoard board = new GridBoard(column, row);

            for (int i=2; i < textRows.Length - 1; i++)
            {
                string[] line = textRows[i].Split(" ");
                for (int j = 0; j < line.Length - 1; j++)
                {
                    try
                    {
                        float x = float.Parse(line[j]);
                        board.board[i - 2][j].Content = x.ToString();
                    }
                    catch
                    {
                        board.board[i - 2][j].Content = line[j];
                    }
                }
            }

            return board;
        }
    }
}
