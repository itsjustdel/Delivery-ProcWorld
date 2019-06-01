using UnityEngine;
using UnityEngine.EventSystems;

using System.Collections.Generic;
using System.Linq;

//using HexGrid;

public class HexGridRectangle : MonoBehaviour, IPointerClickHandler
{
    public float hexSize = 1;
    public bool pointyTop = true;
    public int width = 10;
    public int height = 10;
    public Vector2 origin;

    public ShapeGenerators.Axis axisX = ShapeGenerators.Axis.q;
    public ShapeGenerators.Axis axisY = ShapeGenerators.Axis.r;

    public OffsetCoord.Which coordOffset = OffsetCoord.Which.Odd;

    public int tilesX = 1;
    public int tilesY = 1;

    private Layout layout;
    private Grid grid;

    // Use this for initialization
    void Start()
    {
        Generate();
	//	gameObject.GetComponent<AutoWeld>().enabled = true;
    }

    void Generate()
    {
        layout = new Layout(pointyTop ? Orientation.pointy : Orientation.flat, hexSize, origin);
		grid = new Grid(layout, tilesX, tilesY);

        var tileCount = tilesX * tilesY;
        var map = ShapeGenerators.Rectangle(axisX, axisY, height, width).ToDictionary((h) => h, (h) => Random.Range(0, tileCount));

        var m = grid.MakeMesh(map);

        GetComponent<MeshFilter>().mesh = m;
        GetComponent<MeshCollider>().sharedMesh = m;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var p = eventData.pointerCurrentRaycast.worldPosition;

        var h = layout.WorldToRoundHex(p);
        Debug.Log(h);
        Debug.Log(new OffsetCoord(h, pointyTop ? OffsetCoord.Type.Pointy : OffsetCoord.Type.Flat, coordOffset));
    }
}
