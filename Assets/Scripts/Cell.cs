namespace Match3.Core
{
    public struct Cell
    {
        public GemType Gem { get; }
        public bool IsObstacle { get; }

        public Cell(GemType gem, bool isObstacle = false)
        {
            Gem = gem;
            IsObstacle = isObstacle;
        }

        public bool IsEmpty => Gem == GemType.None;
    }
}