using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TurnManager))]
public class LevelGrid : HexGrid {

    public static List<string> completedLevels;

    public SaveFile level;
    public string[] tiles;

    public GameObject[] grassPrefabs;
    public GameObject[] mountainPrefabs;
    public GameObject icePrefab;
    public GameObject blockadePrefab;

    public string levelName;

    private HexCoordinates playerStart;

    void Awake() {
        if( levelName == "overworld" ){
            playerStart = level.playerStart;
        } 

        width = level.width;
        height = level.height;
        tiles = new string[ level.cells.Length ];
        System.Array.Copy( level.cells, tiles, level.cells.Length );

		canvas = GetComponentInChildren<Canvas>();
		hexMesh = GetComponentInChildren<HexMesh>();

        cells = new HexCell[ width * height ];
		labels = new Text[ width * height ];

        for( int z = 0, i = 0; z < height; z++ ) {
            for( int x = 0; x < width; x++ ){
                CreateCell( x, z, i++ );
            }
        }

        SpawnLevel();
    }

    void SpawnLevel(){

        for( int i = 0; i < cells.Length; i++ ){
            GameObject tile = null;
            tiles[i] = level.cells[i];

            if( level.cells[i].Contains("blockade") ){
                int blockIndex = int.Parse( level.cells[i].Substring(9) );
                if( LevelGrid.completedLevels != null && LevelGrid.completedLevels.Contains( "level-" + blockIndex ) ){
                    tile = Instantiate( grassPrefabs[ Mathf.FloorToInt( Random.Range( 0, grassPrefabs.Length-1) ) ], cells[i].transform.position, Quaternion.identity ) as GameObject;
                    tile.transform.Rotate( 0, 30, 0 );
                    tiles[i] = "grass";
                } else {
                    tile = Instantiate( blockadePrefab, cells[i].transform.position, Quaternion.identity ) as GameObject;
                    tile.transform.Rotate(0,180,0);
                }
            }

            if( level.cells[i].Contains("level")){
                int levelIndex = int.Parse( level.cells[i].Substring(6) );
                tile = Instantiate( grassPrefabs[ Mathf.FloorToInt( Random.Range( 0, grassPrefabs.Length-1) ) ], cells[i].transform.position, Quaternion.identity ) as GameObject;
                tile.transform.Rotate( 0, 30, 0 );
                if( LevelGrid.completedLevels != null && LevelGrid.completedLevels.Contains("level-" + levelIndex ) ){
                }
            }

            if( level.cells[i].Equals( "ice" ) ) {
                tile = Instantiate( icePrefab, cells[i].transform.position, Quaternion.identity ) as GameObject;
                tile.transform.Rotate( 0, 30, 0 );
            }

            if( level.cells[i].Equals( "reverse" ) ) {
                tile = Instantiate( icePrefab, cells[i].transform.position, Quaternion.identity ) as GameObject;
                tile.transform.Rotate( 0, 30, 0 );
            }

            if( level.cells[i].Equals( "fire" ) ){
                tile = Instantiate( icePrefab, cells[i].transform.position, Quaternion.identity ) as GameObject;
                tile.transform.Rotate( 0, 30, 0 );
            }

            if( level.cells[i].Equals( "grass" ) ) {
                tile = Instantiate( grassPrefabs[ Mathf.FloorToInt( Random.Range( 0, grassPrefabs.Length-1) ) ], cells[i].transform.position, Quaternion.identity ) as GameObject;
                tile.transform.Rotate( 0, 30, 0 );
            }

            if( level.cells[i].Equals( "mountain" ) ) {
                tile = Instantiate( mountainPrefabs[ Mathf.FloorToInt( Random.Range( 0, mountainPrefabs.Length-1) ) ], cells[i].transform.position, Quaternion.identity ) as GameObject;
            }
            
            if( tile != null ){
                tile.transform.Rotate( 0, 30, 0 );
                tile.transform.position = new Vector3( tile.transform.position.x, tile.transform.position.y + Random.Range( -0.2f, 0.2f ) + 3f, tile.transform.position.z );
                tile.transform.SetParent( transform );
            }
            
        }
        
        GetComponentInChildren<Renderer>().enabled = false;
        GetComponentInChildren<Canvas>().enabled =false;
    }

    public void CheckLevelLoad( HexCell cell ){
        if( level.cells[ IndexOfCell( cell ) ].Contains( "level" ) ) {
			UnityEngine.SceneManagement.SceneManager.LoadScene( level.cells[ IndexOfCell( cell ) ] );
		}
    }

    public bool IsPositionAvailable( Vector3 position ){
        
        HexCoordinates coords = HexCoordinates.FromPosition( position );

        int zShift = Mathf.FloorToInt( coords.Z/2 );

        int index = coords.X + zShift + coords.Z * width;

        if( index < 0 || index >= level.cells.Length ) {
            return false;
        }

        if(tiles[ index ].Equals("mountain") || tiles[index].Equals("none") || tiles[index].Contains("blockade")){
            return false;
        }

        return true;
        
    }

    public bool IsCellAvailable( HexCell cell ){
        HexCoordinates coords = cell.coordinates;

        int zShift = Mathf.FloorToInt( coords.Z/2 );

        int index = coords.X + zShift + coords.Z * width;

        if( index < 0 || index >= level.cells.Length ) {
            return false;
        }

        if(tiles[ index ].Equals("mountain") || tiles[index].Equals("none")|| tiles[index].Contains("blockade") ){
            return false;
        }
        return true;

    }

    public static void CompleteLevel( string levelName ){
        
        if( completedLevels == null ){
            completedLevels = new List<string>();
        }
        LevelGrid.completedLevels.Add( levelName );
    }

    public string GetCellType( HexCell cell ){
        return level.cells[  IndexOfCell( cell ) ];
    }
	
}
