using Match3.Core;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Match3.Gameplay
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private int width = 8;
        [SerializeField] private int height = 8;
        [SerializeField] private float tileSize = 1f;
        [SerializeField] private CellView tilePrefab;

        private BoardModel _model;
        private CellView[,] _views;
        private (int x, int y)? _firstPicked;

        private void Awake()
        {
            _model = new BoardModel(width, height);
            _views = new CellView[width, height];
            BuildView();
        }

        private void BuildView()
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    var v = Instantiate(tilePrefab, transform);
                    v.transform.localPosition = new Vector3(x * tileSize, y * tileSize);
                    v.Init(_model[x, y].Gem);
                    v.Coords = (x, y);
                    v.OnClicked += HandleClick;
                    _views[x, y] = v;
                }
        }

        private void HandleClick((int x, int y) coords)
        {
            if (_firstPicked == null)
            {
                _firstPicked = coords; Highlight(coords, true);
            }
            else
            {
                var a = _firstPicked.Value;
                var b = coords;
                Highlight(a, false);
                _firstPicked = null;

                if (_model.TrySwap(a, b))
                    StartCoroutine(RefreshBoardView());
            }
        }

        private IEnumerator RefreshBoardView()
        {
            // Simple brute‑force refresh; for production, animate diffs only.
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    _views[x, y].SetGem(_model[x, y].Gem);
            yield return null;
        }

        private void Highlight((int x, int y) c, bool on)
        {
            _views[c.x, c.y].SetHighlight(on);
        }
    }
}