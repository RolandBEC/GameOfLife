namespace WPFClient1.Core
{
    public class EvolutionEngine : IEvolutionEngine
    {
        private Generation CurrentGeneration { get; set; }
        public int CurrentGenerationNumber { get; private set; }

        /// <summary>
        /// Initialises a new instance of the LifeEngine with a specified initial generation.
        /// </summary>
        /// <param name="initialGeneration">The initial generation to start from.</param>
        public EvolutionEngine(Generation initialGeneration)
        {
            CurrentGeneration = initialGeneration;
            CurrentGenerationNumber = 1;
        }

        public Generation GetCurrentGeneration()
        {
            return CurrentGeneration;
        }

        /// <summary>
        /// Update current generation
        /// </summary>
        public EvolutionEngineActionResult UpdateGeneration(bool[,] newUniverse)
        {
            bool hadModification = false;

            for (int i = 0; i < CurrentGeneration.ColumnNumber; i++)
            {
                for (int j = 0; j < CurrentGeneration.RowNumber; j++)
                {
                    Cell cell = CurrentGeneration.GetCell(j, i);
                    if ((cell.Alive == false) && (newUniverse[i, j] == true))
                    {
                        hadModification = true;
                        cell.Alive = true;
                    }
                    else if ((cell.Alive == true) && (newUniverse[i, j] == false))
                    {
                        hadModification = true;
                        cell.Alive = false;
                    }
                }
            }

            return new EvolutionEngineActionResult(
                evolutionEnded: !hadModification,
                generationNumber: CurrentGenerationNumber
            );
        }

        public EvolutionEngineActionResult ResetGeneration()
        {
            CurrentGeneration.Reset();

            CurrentGenerationNumber = 1;

            return new EvolutionEngineActionResult(
                evolutionEnded: false,
                generationNumber: CurrentGenerationNumber
            );
        }

        public Cell GetCell(int row, int column)
        {
            return CurrentGeneration.GetCell(row, column);
        }

        public void SetCell(int row, int column, bool alive)
        {
            CurrentGeneration.SetCell(row, column, alive);
        }

        public void ToggleCellLife(int row, int column)
        {
            CurrentGeneration.ToggleCellLife(row, column);
        }
    }
}
