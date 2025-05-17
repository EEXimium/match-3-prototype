using UnityEngine;
using UnityEngine.UI;
using System;
using Match3.Core;
using UnityEngine.EventSystems;

namespace Match3.Gameplay
{
    [RequireComponent(typeof(Image))]
    public class CellView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Sprite red, green, blue, yellow, purple, empty;
        [SerializeField] private Image highlightRing;

        private Image _img;
        public (int x, int y) Coords { get; set; }
        public event Action<(int x, int y)> OnClicked;

        private void Awake() { _img = GetComponent<Image>(); }

        public void Init(GemType type) => SetGem(type);

        public void SetGem(GemType type)
        {
            _img.sprite = type switch
            {
                GemType.Red => red,
                GemType.Green => green,
                GemType.Blue => blue,
                GemType.Yellow => yellow,
                GemType.Purple => purple,
                _ => empty
            };
        }

        public void SetHighlight(bool on) => highlightRing.enabled = on;

        public void OnPointerClick(PointerEventData eventData) => OnClicked?.Invoke(Coords);
    }
}