namespace WPFClient1.Core
{
    public interface IEvolutionEngine
    {
        int CurrentGenerationNumber { get; }

        EvolutionEngineActionResult UpdateGeneration(bool[,] newUniverse);

        EvolutionEngineActionResult ResetGeneration();

        Generation GetCurrentGeneration();

        Cell GetCell(int row, int column);

        void SetCell(int row, int column, bool alive);

        void ToggleCellLife(int row, int column);
    }
}
