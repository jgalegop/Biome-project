using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlexibleGridLayout : LayoutGroup
{
    public enum FitType { Uniform, Width, Height, FixedRows, FixedColumns}

    [SerializeField]
    private FitType _fitType = FitType.Uniform;

    public int Rows;
    public int Columns;
    public Vector2 CellSize = Vector2.one;
    public Vector2 Spacing;

    [SerializeField]
    private bool _fitX = false;
    [SerializeField]
    private bool _fitY = false;


    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();


        if (_fitType == FitType.Height || _fitType == FitType.Width || _fitType == FitType.Uniform)
        {
            _fitX = true;
            _fitY = true;

            float sqrt = Mathf.Sqrt(transform.childCount);
            Rows = Mathf.CeilToInt(sqrt);
            Columns = Mathf.CeilToInt(sqrt);
        }

        if (_fitType == FitType.Width || _fitType == FitType.FixedColumns)
        {
            _fitX = true;
            Rows = Mathf.CeilToInt(transform.childCount / (float)Columns);
        }
        else if (_fitType == FitType.Height || _fitType == FitType.FixedRows)
        {
            _fitY = true;
            Columns = Mathf.CeilToInt(transform.childCount / (float)Rows);
        }

        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        float cellWidth = parentWidth / (float)Columns - Spacing.x * (float)(Columns - 1) / (float)Columns - padding.left / (float)Columns - padding.right / (float)Columns;
        float cellHeight = parentHeight / (float)Rows - Spacing.y * (float)(Rows - 1) / (float)Rows - padding.top / (float)Rows - padding.bottom / (float)Rows;

        CellSize.x = (_fitX && cellWidth > 0) ? cellWidth : CellSize.x;
        CellSize.y = (_fitY && cellHeight > 0) ? cellHeight : CellSize.y;

        int columnCount;
        int rowCount;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            rowCount = i / Columns;
            columnCount = i % Columns;

            var item = rectChildren[i];

            float xPos = (CellSize.x + Spacing.x) * columnCount + padding.left;
            float yPos = (CellSize.y + Spacing.y) * rowCount + padding.top;

            SetChildAlongAxis(item, 0, xPos, CellSize.x);
            SetChildAlongAxis(item, 1, yPos, CellSize.y);
        }
    }

    public override void CalculateLayoutInputVertical()
    {
        //throw new System.NotImplementedException();
    }

    public override void SetLayoutHorizontal()
    {
        //throw new System.NotImplementedException();
    }

    public override void SetLayoutVertical()
    {
        //throw new System.NotImplementedException();
    }
}
