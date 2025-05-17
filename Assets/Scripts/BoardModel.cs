using System;
using System.Collections.Generic;
using System.Linq;

namespace Match3.Core
{
    public sealed class BoardModel
    {
        public readonly int Width;
        public readonly int Height;

        private readonly Cell[,] _cells;
        private readonly Random _rng;

        public BoardModel(int width, int height, Random rng = null)
        {
            Width = width;
            Height = height;
            _cells = new Cell[width, height];
            _rng = rng ?? new Random();

            FillRandom();
            ResolveInitialMatches();
        }

        public Cell this[int x, int y]
        {
            get => _cells[x, y];
            private set => _cells[x, y] = value;
        }

        // Try to swap; returns true if swap produced at least one match.
        public bool TrySwap((int x, int y) a, (int x, int y) b)
        {
            if (!AreAdjacent(a, b)) return false;
            SwapInternally(a, b);

            var matches = FindAllMatches();
            if (matches.Count == 0)
            {
                // Undo swap
                SwapInternally(a, b);
                return false;
            }

            Resolve(matches);
            return true;
        }

        // ───── private helpers ─────
        private void FillRandom()
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    _cells[x, y] = new Cell(RandomGem());
        }

        private GemType RandomGem() => (GemType)_rng.Next(1, 6); // 1..5 inclusive

        private static bool AreAdjacent((int x, int y) a, (int x, int y) b) =>
            Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) == 1;

        private void SwapInternally((int x, int y) a, (int x, int y) b)
        {
            var tmp = _cells[a.x, a.y];
            _cells[a.x, a.y] = _cells[b.x, b.y];
            _cells[b.x, b.y] = tmp;
        }

        private HashSet<(int x, int y)> FindAllMatches()
        {
            var matches = new HashSet<(int, int)>();

            // Horizontal
            for (int y = 0; y < Height; y++)
            {
                int run = 1;
                for (int x = 1; x < Width; x++)
                {
                    if (SameGem(x, y, x - 1, y)) run++;
                    else { FlushRun(); run = 1; }
                    void FlushRun()
                    {
                        if (run >= 3)
                            for (int k = 0; k < run; k++) matches.Add((x - 1 - k, y));
                    }
                }
                if (run >= 3) for (int k = 0; k < run; k++) matches.Add((Width - 1 - k, y));
            }

            // Vertical
            for (int x = 0; x < Width; x++)
            {
                int run = 1;
                for (int y = 1; y < Height; y++)
                {
                    if (SameGem(x, y, x, y - 1)) run++;
                    else { FlushRun(); run = 1; }
                    void FlushRun()
                    {
                        if (run >= 3)
                            for (int k = 0; k < run; k++) matches.Add((x, y - 1 - k));
                    }
                }
                if (run >= 3) for (int k = 0; k < run; k++) matches.Add((x, Height - 1 - k));
            }

            return matches;
        }

        private bool SameGem(int x1, int y1, int x2, int y2) =>
            !_cells[x1, y1].IsEmpty && _cells[x1, y1].Gem == _cells[x2, y2].Gem;

        private void Resolve(HashSet<(int x, int y)> matches)
        {
            Clear(matches);
            CollapseColumns();
            Refill();

            var newMatches = FindAllMatches();
            if (newMatches.Count > 0) Resolve(newMatches);
        }

        private void Clear(HashSet<(int, int)> coords)
        {
            foreach (var (x, y) in coords) _cells[x, y] = new Cell(GemType.None);
        }

        private void CollapseColumns()
        {
            for (int x = 0; x < Width; x++)
            {
                int writeY = 0;
                for (int y = 0; y < Height; y++)
                {
                    if (!_cells[x, y].IsEmpty)
                    {
                        if (writeY != y) _cells[x, writeY] = _cells[x, y];
                        writeY++;
                    }
                }
                for (int y = writeY; y < Height; y++) _cells[x, y] = new Cell(GemType.None);
            }
        }

        private void Refill()
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    if (_cells[x, y].IsEmpty) _cells[x, y] = new Cell(RandomGem());
        }

        private void ResolveInitialMatches()
        {
            var matches = FindAllMatches();
            if (matches.Count > 0) Resolve(matches);
        }
    }
}