using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoad : MonoBehaviour {

	public SaveFile level;

	public void LoadScene(){
		level.playerStart = new HexCoordinates( 0, 0 );
		UnityEngine.SceneManagement.SceneManager.LoadScene( "Overworld" );
	}
}
