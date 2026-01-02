using System;
using UnityEngine;

public class UnitSelectionManagerUI : MonoBehaviour {
    [SerializeField] private RectTransform selectionAreaRectTransform;

    private void Start() {
        UnitSelectionManager.Instance.OnSelectionAreaStart += UnitSelectionManager_OnSelectionAreaStart;
        UnitSelectionManager.Instance.OnSelectionAreaEnd += UnitSelectionManager_OnSelectionAreaEnd;
        selectionAreaRectTransform.gameObject.SetActive(false);
    }
    private void Update() {
        if (selectionAreaRectTransform.gameObject.activeSelf) {
            UpdateVisual();
        }
    }
    private void UnitSelectionManager_OnSelectionAreaStart(object sender, EventArgs e) {
        selectionAreaRectTransform.gameObject.SetActive(true);
        UpdateVisual();
    }
    private void UnitSelectionManager_OnSelectionAreaEnd(object sender, EventArgs e) {
        selectionAreaRectTransform.gameObject.SetActive(false);
    }

    private void UpdateVisual() {
        Rect selectionRect = UnitSelectionManager.Instance.GetSelectionAreaRect();
        selectionAreaRectTransform.anchoredPosition = selectionRect.position;
        selectionAreaRectTransform.sizeDelta = selectionRect.size;
    }
}
