namespace GameOfLife.Core
{
    public class EvolutionAlgorithm
    {
        /// <summary>
        /// horizontal size of grid to calculate next generation for
        /// </summary>
        private int WidthX;
        /// <summary>
        /// vertical size of grid to calculate next generation for
        /// </summary>
        private int WidthY;

        private const int healthCondition1 = 2; // two adjacent dots required to survive
        private const int healthCondition2 = 3; // three adjacent dots required to grow

        public EvolutionAlgorithm()
        {

        }

        private int ToInt(bool value)
        {
            return value ? 1 : 0;
        }

        /// <summary>
        /// calculate next generation of game of life cells based on existing board of cells
        /// </summary>
        /// <param name="board">existing board to base calculations on</param>
        /// <returns>bool[,] array with next generation board of cells</returns>
        public bool[,] calculateNextBoard(bool[,] board)
        {
            WidthX = board.GetLength(0);
            WidthY = board.GetLength(1);

            bool[,] Tempboard = new bool[WidthX, WidthY];

            for (int i = 0; i < WidthX; i++)
            {
                for (int j = 0; j < WidthY; j++)
                {
                    int willLive = 0;

                    //upper row
                    if (j > 0)
                    {
                        if (i > 0) //left of
                            willLive += ToInt(board[i - 1, j - 1]);
                        willLive += ToInt(board[i, j - 1]);
                        if (i < WidthX - 1)
                            willLive += ToInt(board[i + 1, j - 1]);
                    }

                    //same row
                    if (i > 0)
                        willLive += ToInt(board[i - 1, j]); //left of
                    if (i < WidthX - 1)
                        willLive += ToInt(board[i + 1, j]);

                    //lower row
                    if (j < WidthY - 1)
                    {
                        if (i > 0) //left of
                            willLive += ToInt(board[i - 1, j + 1]);
                        willLive += ToInt(board[i, j + 1]);
                        if (i < WidthX - 1)
                            willLive += ToInt(board[i + 1, j + 1]);
                    }


                    if (board[i, j] == true)
                    {
                        if (willLive >= healthCondition1 && willLive <= healthCondition2)
                            Tempboard[i, j] = true;
                        else
                            Tempboard[i, j] = false;
                    }
                    else // if original cell was empty
                    {
                        if (willLive == healthCondition2)
                            Tempboard[i, j] = true;
                        else
                            Tempboard[i, j] = false;
                    }
                }
            }
            return Tempboard;
        }
    }
}