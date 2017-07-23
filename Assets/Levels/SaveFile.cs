using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SaveFile : ScriptableObject {

	public string[] cells;

	public int width;
	public int height;

	public HexCoordinates playerStart;
}
