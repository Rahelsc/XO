using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XO
{
    class ScoreBoard
    {
        private int size;
        private int[] columnScore;
        private int[] rowScore;



        private int leftDiagonalCounter = 0;

        public int LeftDiagonalCounter
        {
            get { return leftDiagonalCounter; }
        }

        private int rightDiagonalCounter = 0;

        public int RightDiagonalCounter
        {
            get { return rightDiagonalCounter; }
        }

        public void setDiagonalCounters(int index)
        {
            // middle of board, two diagonals should be updated
            if ((size%2)!=0 && index == (size*size-1)/2)
            {
                leftDiagonalCounter++;
                rightDiagonalCounter++;
            }
            else
            {
                if (index % (size + 1) == 0)
                {
                    rightDiagonalCounter++;
                }
                else if (index % (size - 1) == 0)
                {
                    leftDiagonalCounter++;
                }
            }
        }


        public int getColumnScore(int index)
        {
            return columnScore[index];
        }

        public int[] getAllColumnScore()
        {
            return columnScore;
        }

        public void SetColumnScore(int index)
        {
            columnScore[index]++;
        }



        public int getRowScore(int index)
        {
            return rowScore[index];
        }

        public int[] getAllRowScore()
        {
            return rowScore;
        }

        public void SetRowScore(int index)
        {
            rowScore[index]++;
        }

        public ScoreBoard(int size)
        {
            this.size = size;
            columnScore = new int[size];
            rowScore = new int[size];

        }

        public override string ToString()
        {
            string column = "";
            foreach (int item in columnScore)
            {
                column += " " + item.ToString();
            }

            string row = "";
            foreach (int item in rowScore)
            {
                row += " " + item.ToString();

            }
            return $"column: {column} \nrow: {row} \nleft diagonal {leftDiagonalCounter} \nright diagonal {rightDiagonalCounter}";
        }
    }
}
