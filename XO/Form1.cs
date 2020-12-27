using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XO
{
    public partial class Form1 : Form
    {
        Label gameInstructions;
        int RUBRIC_SIZE = 80;
        int FORM_MARGIN = 30;
        int choice = 3; // default value
        Button[] grid;
        bool isFirstPlayerMove = true;
        ScoreBoard XmyScores;
        ScoreBoard OmyScores;
        static int xWins;
        static int oWins;
        int movesCounter = 0;
        bool isComputerPlaying = false;
        Label resultsLabel;


        public Form1()
        {
            InitializeComponent();
            this.BackColor = Color.FromArgb(234, 26, 192);
            this.MinimumSize = new Size(500, 400);
            LoadGameInstructions(this, new EventArgs());
            grideSet(this, new EventArgs());
            setNewScoreBoardLabel();
        }

        private void setNewScoreBoardLabel()
        {
            resultsLabel = new Label();
            resultsLabel.Size = new Size(this.Width + FORM_MARGIN, 25);
            resultsLabel.BackColor = Color.Azure;
            resultsLabel.Location = new Point(20, this.Height - RUBRIC_SIZE);
            resultsLabel.Font = new Font("Arial", 16);
            resultsLabel.TextAlign = ContentAlignment.BottomCenter;
            resultsLabel.Text = $"X Player Points: {xWins} O Player Points: {oWins}";
            this.Controls.Add(resultsLabel);
        }

        public void LoadGameInstructions(object sender, EventArgs e)
        {
            MenuStrip gameMenu = this.menuStrip1;
            this.Controls.Clear();
            this.Controls.Add(gameMenu);
            gameInstructions = new Label();
            gameInstructions.Text = "Welcome to XO Game" +
                "\nPress Start Game in Menu" +
                "\nDefault Settings Are:" +
                "\nPlayer Vs. Computer" +
                "\nand" +
                "\nA Three By Three Grid" +
                "\nIf You Wish To Change Setting Go To Menu";
            gameInstructions.Size = new Size(this.Width-50, this.Height-50);
            gameInstructions.Location = new Point(20, 20);
            gameInstructions.Font = new Font("Arial", 16);
            gameInstructions.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(gameInstructions);
        }


        private void grideSet(object sender, EventArgs e)
        {
            isFirstPlayerMove = true;
            movesCounter = 0;
            if (grid != null)
            {
                for (int i = 0; i < grid.Length; i++)
                {
                    this.Controls.Remove(grid[i]);
                    grid[i].Dispose();
                }
            }
            ToolStripMenuItem toolStripClick;
            for (int i = 1; i < this.Controls.Count; i++)
            {
                this.Controls.RemoveAt(i);
            }
            if (sender is ToolStripMenuItem)
            {
                toolStripClick = sender as ToolStripMenuItem;
                if (toolStripClick.Text.Contains("X"))
                {
                    choice = Convert.ToInt32(toolStripClick.Text[0] + ""); // get's the first char of choice turns to int
                }
                else
                    choice = 3;
            }

            XmyScores = new ScoreBoard(choice);
            OmyScores = new ScoreBoard(choice);


            this.Width = RUBRIC_SIZE * choice + FORM_MARGIN;
            this.Height = RUBRIC_SIZE * choice + FORM_MARGIN;

            grid = new Button[choice * choice];
            for (int i = 0; i < grid.Length; i++)
            {
                grid[i] = new Button();
                grid[i].BackColor = Color.Gray;
                grid[i].Font = new Font("Arial", 24);
                grid[i].FlatStyle = FlatStyle.Flat;
                grid[i].FlatAppearance.BorderColor = Color.White;
                grid[i].FlatAppearance.BorderSize = 1;
                grid[i].Size = new Size(RUBRIC_SIZE, RUBRIC_SIZE);
                grid[i].Location = new Point(RUBRIC_SIZE * (i % choice) + FORM_MARGIN * 2, RUBRIC_SIZE * (i/choice)+ FORM_MARGIN* 2);
                grid[i].Click += playerMove;
                grid[i].Tag = i;
                grid[i].MouseHover += changeCursor;
                grid[i].MouseLeave += deleteCursor;
                this.Controls.Add(grid[i]);                
            }

            setNewScoreBoardLabel();
        }

        private void changeCursor(object sender, EventArgs e)
        {
            Button b = sender as Button;
            if (b.Text == "")
            {
                b.ForeColor = Color.White;
                if (isFirstPlayerMove)
                    b.Text = "X";
                else
                    b.Text = "O";
            }

        }

        public void deleteCursor(object sender, EventArgs e)
        {
            Button b = sender as Button;
            if (b.Text != "" && b.ForeColor == Color.White)
            {
                b.Text = "";
            }
        }

        private void playerMove(object sender, EventArgs e)
        {
            Button playedMove = sender as Button;
            if (isFirstPlayerMove)
            {
                if (playedMove.Text == "" || playedMove.ForeColor == Color.White)
                {
                    movesCounter++;
                    playedMove.ForeColor = Color.Black;
                    playedMove.Text = "X";
                    isFirstPlayerMove = false;
                    calculateScore("X", playedMove);
                }
            }
            else
            {
                if (playedMove.Text == "" || playedMove.ForeColor == Color.White)
                {
                    movesCounter++;
                    playedMove.ForeColor = Color.Black;
                    playedMove.Text = "O";
                    isFirstPlayerMove = true;
                    calculateScore("O", playedMove);
                }
            }

            if (movesCounter == grid.Length)
            {
                gameAnnoucments("");
            }

            if (isComputerPlaying && !isFirstPlayerMove)
            {
                movesCounter++;
                Button computerMove = computerLogic();
                isFirstPlayerMove = true;
                calculateScore("O", computerMove);
            }
        }

        private void calculateScore(string whoPlayed, Button whatMove)
        {
            int rowIndexToUpdate = (Convert.ToInt32(whatMove.Tag) * choice) / grid.Length;
            int columnIndexToUpdate = ((Convert.ToInt32(whatMove.Tag) * choice) % grid.Length)/choice;
            int diagonalIndexToUpdate = Convert.ToInt32(whatMove.Tag);
            switch (whoPlayed)
            {
                case "X":
                    XmyScores.SetRowScore(rowIndexToUpdate);
                    XmyScores.SetColumnScore(columnIndexToUpdate);
                    XmyScores.setDiagonalCounters(diagonalIndexToUpdate);
                    break;
                case "O":
                    OmyScores.SetRowScore(rowIndexToUpdate);
                    OmyScores.SetColumnScore(columnIndexToUpdate);
                    OmyScores.setDiagonalCounters(diagonalIndexToUpdate);
                    break;
            }
            isThereAwinner(whoPlayed, rowIndexToUpdate, columnIndexToUpdate, diagonalIndexToUpdate);
        }

        private void isThereAwinner(string player, int lastUpdatedRowScore, int lastUpdatedColScore, int lastUpdateDiagonalScore)
        {
            if (
                XmyScores.getColumnScore(lastUpdatedColScore) == choice 
                || XmyScores.getRowScore(lastUpdatedRowScore) == choice
                || XmyScores.LeftDiagonalCounter == choice
                || XmyScores.RightDiagonalCounter == choice
                || OmyScores.getColumnScore(lastUpdatedColScore) == choice 
                || OmyScores.getRowScore(lastUpdatedRowScore) == choice
                || OmyScores.LeftDiagonalCounter == choice
                || OmyScores.RightDiagonalCounter == choice
                )
            {
                Console.WriteLine($"x: {XmyScores} O: {OmyScores}");
                switch (player)
                {
                    case "X":
                        xWins++;
                        break;
                    case "O":
                        oWins++;
                        break;
                }
                resultsLabel.Text = $"X Player Points: {xWins} O Player Points: {oWins}";
                gameAnnoucments(player);
            }
        }

        private void gameAnnoucments(string whatToannounce)
        {
            DialogResult dialogResult;
            if (whatToannounce != "")
            {
                dialogResult = MessageBox.Show($"{whatToannounce} is The WINNER! \ncongratulations\nwould you like another game?", "Winner Announcement", MessageBoxButtons.YesNo);
            }
            else
            {
                dialogResult = MessageBox.Show($"there is no winner here\nwould like to play again?", "Game Announcement", MessageBoxButtons.YesNo);
            }

            if (dialogResult == DialogResult.Yes)
            {
                movesCounter = 0;
                isFirstPlayerMove = true;
                choice = 3;
                grideSet(this, new EventArgs());
            }
            else if (dialogResult == DialogResult.No)
            {
                Application.Exit();
            }
        }

        private void GameAgainstComputer(object sender, EventArgs e)
        {
            isComputerPlaying = true;
            movesCounter = 0;
            grideSet(sender, e);
        }

        private void playerVSplayer(object sender, EventArgs e)
        {
            isComputerPlaying = false;
            movesCounter = 0;
            grideSet(sender, e);
        }

        private Button computerLogic()
        {
            // to decide if a block to the player is required
            if (XmyScores.LeftDiagonalCounter == choice-1 && OmyScores.LeftDiagonalCounter == 0)
            {
                Console.WriteLine("first condition");
                for (int i = choice-1; i < grid.Length; i+=choice-1)
                {
                    if (grid[i].Text == "")
                    {
                        grid[i].ForeColor = Color.Black;
                        grid[i].Text = "O";
                        return grid[i];
                    }
                }
            }
            if (XmyScores.RightDiagonalCounter == choice - 1 && OmyScores.RightDiagonalCounter == 0)
            {
                Console.WriteLine("second condition");
                for (int i = 0; i < grid.Length; i += choice + 1)
                {
                    if (grid[i].Text == "")
                    {
                        grid[i].ForeColor = Color.Black;
                        grid[i].Text = "O";
                        return grid[i];
                    }
                }
            }
            for (int i = 0; i < XmyScores.getAllColumnScore().Length; i++)
            {
                Console.WriteLine("third condition" + XmyScores.getAllColumnScore().Length);

                if (XmyScores.getAllColumnScore()[i] == choice - 1 && OmyScores.getAllColumnScore()[i] == 0)
                {
                    Console.WriteLine("I was here");
                    for (int colIndex = i; colIndex < grid.Length; colIndex+=choice)
                    {
                        if (grid[colIndex].Text == "")
                        {
                            grid[colIndex].ForeColor = Color.Black;
                            grid[colIndex].Text = "O";
                            return grid[colIndex];
                        }
                    }
                }
                if (XmyScores.getAllRowScore()[i] == choice - 1 && OmyScores.getAllRowScore()[i]==0)
                {
                    Console.WriteLine("I was here");

                    for (int rowIndex = i*choice; rowIndex < grid.Length; rowIndex++)
                    {
                        if (grid[rowIndex].Text == "")
                        {
                            grid[rowIndex].ForeColor = Color.Black;
                            grid[rowIndex].Text = "O";
                            return grid[rowIndex];
                        }
                    }
                }
            }

            // to decide what is the best move to make if a block is not required
            int max = -1;
            int maxIndex = 0;
            string whatGridPartToMoveOn = "";
            if (OmyScores.LeftDiagonalCounter > max && XmyScores.LeftDiagonalCounter == 0)
            {
                max = OmyScores.LeftDiagonalCounter;
                maxIndex = grid.Length;
                whatGridPartToMoveOn = "LeftDiagonalCounter";
            }

            if (OmyScores.RightDiagonalCounter > max && XmyScores.RightDiagonalCounter == 0)
            {
                max = OmyScores.RightDiagonalCounter;
                maxIndex = grid.Length + 1;
                whatGridPartToMoveOn = "RightDiagonalCounter";

            }
            for (int i = 0; i < OmyScores.getAllColumnScore().Length; i++)
            {
                if (OmyScores.getAllColumnScore()[i]>max && XmyScores.getAllColumnScore()[i] == 0)
                {
                    max = OmyScores.getAllColumnScore()[i];
                    maxIndex = i;
                    whatGridPartToMoveOn = "column";

                }
                if (OmyScores.getAllRowScore()[i] > max && XmyScores.getAllRowScore()[i] == 0)
                {
                    max = OmyScores.getAllRowScore()[i];
                    maxIndex = i;
                    whatGridPartToMoveOn = "row";
                }
            }

            if (max == -1)
            {
                for (int i = 0; i < grid.Length; i++)
                {
                    if (grid[i].Text == "")
                    {
                        grid[i].ForeColor = Color.Black;
                        grid[i].Text = "O";
                        return grid[i];
                    }
                }
            }
            else
            {
                switch (whatGridPartToMoveOn)
                {
                    case "LeftDiagonalCounter":
                        Console.WriteLine("s1");
                        for (int i = choice - 1; i < grid.Length; i += (choice - 1))
                        {
                            Console.WriteLine("1st loop");

                            if (grid[i].Text == "")
                            {
                                Console.WriteLine("inner condition");
                                grid[i].ForeColor = Color.Black;
                                grid[i].Text = "O";
                                return grid[i];
                            }
                        }
                        break;
                    case "RightDiagonalCounter":
                        Console.WriteLine("s2");

                        for (int i = 0; i < grid.Length; i += (choice + 1))
                        {
                            Console.WriteLine($"grid text: {grid[i].Text} index: {i}");
                            if (grid[i].Text == "")
                            {
                                Console.WriteLine("s2 inner condition");
                                grid[i].ForeColor = Color.Black;
                                grid[i].Text = "O";
                                return grid[i];
                            }
                        }
                        Console.WriteLine("I'm out");
                        break;
                    case "column":
                        Console.WriteLine("s3");

                        for (int colIndex = maxIndex; colIndex < grid.Length; colIndex += choice)
                        {
                            Console.WriteLine();
                            if (grid[colIndex].Text == "")
                            {
                                grid[colIndex].ForeColor = Color.Black;
                                grid[colIndex].Text = "O";
                                return grid[colIndex];
                            }
                        }
                        break;
                    case "row":
                        Console.WriteLine("s4");

                        for (int rowIndex = maxIndex * choice; rowIndex < grid.Length; rowIndex++)
                        {
                            if (grid[rowIndex].Text == "")
                            {
                                grid[rowIndex].ForeColor = Color.Black;
                                grid[rowIndex].Text = "O";
                                return grid[rowIndex];
                            }
                        }
                        break;
                }
            }

            return new Button();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }


}
