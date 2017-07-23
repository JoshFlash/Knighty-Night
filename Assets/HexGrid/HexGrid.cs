using UnityEngine;
using UnityEngine.UI;
public class HexGrid : MonoBehaviour {

	public int width = 6;
	public int height = 6;

	public HexCell cellPrefab;
    public Text cellLabelPrefab;
    public Canvas canvas;

    public HexMesh hexMesh;

    public HexCell[] cells;
	public Text[] labels;

    public Color defaultColor = Color.white;
	public Color touchedColor = Color.magenta;

    void Awake() {
		canvas = GetComponentInChildren<Canvas>();
		hexMesh = GetComponentInChildren<HexMesh>();

        cells = new HexCell[ width * height ];
		labels = new Text[ width * height ];

        for( int z = 0, i = 0; z < height; z++ ) {
            for( int x = 0; x < width; x++ ){
                CreateCell( x, z, i++ );
            }
        }
    }

    void Start () {
		hexMesh.Triangulate(cells);
	}

    public void CreateCell (int x, int z, int i) {
		Vector3 position;
		position.x = x * 10f;
		position.y = 0f;
		position.z = z * 10f;

        position.x = x * (HexMetrics.innerRadius * 2f);
		position.z = z * (HexMetrics.outerRadius * 1.5f);
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);


		HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
		cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;
		cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);

        
		Text label = Instantiate<Text>(cellLabelPrefab);
		label.rectTransform.SetParent(canvas.transform, false);
		label.rectTransform.anchoredPosition =
			new Vector2(position.x, position.z);
		label.text = cell.coordinates.ToStringOnSeparateLines();
		labels[ i ] = label;
		cell.color = defaultColor;
	}

    void Update () {
		if (Input.GetMouseButtonDown(0)) {
			HandleInput();
		}
		
	}

	public virtual void HandleInput () {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {
			HexCoordinates coords = TouchCell(hit.point);
            int index = coords.X + coords.Z * width;

            HexCell cell = cells[index];
		    cell.color = touchedColor;
		    hexMesh.Triangulate(cells);
            
		}
	}
	
	public HexCoordinates TouchCell (Vector3 position) {
		position = transform.InverseTransformPoint(position);
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        return coordinates;
	}

    public HexCell GetCellFromCoordinates( int x, int z ){
        int index = x + z * width;
        int zShift = Mathf.FloorToInt( z/2 );
		index += zShift;
        HexCell cell = cells[index];
        return cell;
    }

    public HexCell GetCellFromCoordinates( HexCoordinates coords ){
        int index = coords.X + coords.Z * width;
        int zShift = Mathf.FloorToInt( coords.Z/2 );
		index += zShift;
        HexCell cell = cells[index];
        return cell;
    }

	public int IndexOfCell( HexCell cell ){
		int i = 0;
		foreach( HexCell c in cells ){
			if( c.Equals( cell ) ) {
				return i;
			}
			i++;
		}
		return -1;
	}

	public HexCell GetAdjacentCell( HexCell origin, MovementController.MoveDirection direction){

		int x = origin.coordinates.X;
		int z = origin.coordinates.Z;

		if( direction == MovementController.MoveDirection.W ){
			x--;
		} else if( direction == MovementController.MoveDirection.E ) {
			x++;
		} else if( direction == MovementController.MoveDirection.NE ){
			z++;
		} else if( direction == MovementController.MoveDirection.SE ){
			z--;
			x++;
		} else if( direction == MovementController.MoveDirection.SW ){
			z--;
		} else if( direction == MovementController.MoveDirection.NW ){
			z++;
			x--;
		}

		return GetCellFromCoordinates( x, z );
	}

}