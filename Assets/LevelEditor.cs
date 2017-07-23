using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditor : HexGrid {

	public string[] celltypes = { "none", "grass", "mountain", "ice", "reverse", "fire"};
	public Color[] colors = { Color.white, Color.green, Color.gray, Color.cyan, Color.magenta, Color.red };

	public string levelName;

	public string dragTile="grass";

	public SaveFile loadFile;

	void Awake(){

		if( loadFile != null ){
			Debug.Log("yes");
			width = loadFile.width;
			height = loadFile.height;
		}
		canvas = GetComponentInChildren<Canvas>();
		hexMesh = GetComponentInChildren<HexMesh>();

        cells = new HexCell[ width * height ];
		labels = new Text[ width * height ];

        for( int z = 0, i = 0; z < height; z++ ) {
            for( int x = 0; x < width; x++ ){
                CreateCell( x, z, i++ );
            }
        }

		

		if( loadFile != null ){
			for( int i = 0; i < width * height; i++ ) {
				Debug.Log("go");
				labels[i].text = loadFile.cells[i];
				cells[i].color = colors[ System.Array.IndexOf(celltypes, labels[i].text)];
				hexMesh.Triangulate(cells);
        	}
		}
	}


	void Update(){
		if (Input.GetMouseButtonDown(0)) {
			HandleInput();
		}
		if (Input.GetMouseButton(1)) {
			HandleInputHold();
		}
	}

	public void Save(){

		string[] texts = new string[ cells.Length ];

		int i = 0;
		foreach( Text label in labels ){
			bool found = false;
			foreach( string s in celltypes ){
				if( label.text.Equals( s ) || label.text.Contains( "blockade" ) ){
					found = true;
				} 
			}
			if( found == false ){
				label.text = "none";
			}
			texts[i] = label.text;
			i++;
		}
		
		SaveFile save = ScriptableObject.CreateInstance("SaveFile") as SaveFile;
		save.cells = texts;
		save.width = width;
		save.height = height;
		//AssetDatabase.CreateAsset( save , "Assets/Levels/" + levelName + ".asset" );

		
	}


	public void HandleInputHold(){
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {
			HexCoordinates coords = TouchCell(hit.point);
			int zShift = Mathf.FloorToInt( coords.Z/2 );
            int index = coords.X + zShift + coords.Z * width;

			HexCell cell = cells[ index ];
			Text label = labels[ index ];


			label.text = dragTile;

			if( label.text.Contains("blockade") ){
				cell.color = Color.yellow;
			} else {
				cell.color = colors[ System.Array.IndexOf(celltypes, label.text)];
			}

			hexMesh.Triangulate(cells);
				
		}
	}

	public override void HandleInput () {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {
			HexCoordinates coords = TouchCell(hit.point);
			int zShift = Mathf.FloorToInt( coords.Z/2 );
            int index = coords.X + zShift + coords.Z * width;

			UpdateCell( index );
            
		}
	}

	void UpdateCell( int index ){


        HexCell cell = cells[ index ];
		Text label = labels[ index ];


		bool next = false;
		bool found = false;
		foreach( string s in celltypes ){
			if( label.text.Equals( s ) ){
				next = true;
				found = true;
			} else if( next ){
				label.text = s;
				next = false;
			}
		}
		if( found == false || next == true ){
			label.text = "grass";
		}

		cell.color = colors[ System.Array.IndexOf(celltypes, label.text)];
		hexMesh.Triangulate(cells);
		
	}
}
