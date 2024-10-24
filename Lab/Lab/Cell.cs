﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab
{
    internal class Cell
    {
        int x, y;
        string content;
        BoardPosition position;

        public Cell(int x,int y, string content)
        {
            this.x = x;
            this.y = y;
            this.content = content;

            this.position = GridBoard.GetCellsBoardPosition(this);
        }

        public int X { get { return x; } }
        public int Y { get { return y; } }
        public string Content { get { return content; } }
        public BoardPosition Position { get { return position; } }
    }
}
