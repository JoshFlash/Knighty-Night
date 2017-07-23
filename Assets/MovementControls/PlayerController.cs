using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MovementController
{

	public MoveDirection lastMove;
	public HexCoordinates startingCoordinates;

	public LevelGrid level;

	private static HexCell currentCell;
	private static HexCell lastCell;

	public int strength = 1;


	private float startSlip = 0f;
	private float startBurn = 0f;

	
	private Vector3 moveVector = Vector3.zero;
	private Vector3 oldVector;
	private float lastPressed;
	private HexCell targetCell;

	private bool won = false;
	private float winTime = 0f;

	private Animator anim;

	private void Awake()
	{
		anim = GetComponent<Animator>();
		grid = FindObjectOfType<HexGrid>();

		if( Camera.main.transform.parent == null ){
			GameObject parent = new GameObject();
			parent.transform.position = Camera.main.transform.position;
			parent.transform.rotation = new Quaternion( 0f, Camera.main.transform.rotation.y, 0f, 0f );
			Camera.main.transform.SetParent( parent.transform );	
		}
	}

	private void Start()
	{
		if( level.levelName == "overworld"){
			currentCell = grid.GetCellFromCoordinates(level.level.playerStart);
		} else {
			currentCell = grid.GetCellFromCoordinates(startingCoordinates);
		}
		transform.position = currentCell.transform.position;
		Vector3 pos = transform.position;
		transform.position = new Vector3( pos.x, pos.y + 4.6f, pos.z );
	}

	private void Update()
	{
		if( won && Time.time - winTime > 1.5f){
			UnityEngine.SceneManagement.SceneManager.LoadScene( "Overworld" );
		}
		HandleCellChange();
		HandleMovement();
	}

	public override void HandleMovement()
	{
		
		Vector3 currentCellPos = new Vector3( currentCell.transform.position.x, transform.position.y, currentCell.transform.position.z);
		if( IsSlipping() || IsBurning() ){
			moveVector = new Vector3( targetCell.transform.position.x, transform.position.y, targetCell.transform.position.z ) - transform.position;
		} else {
			oldVector = moveVector;
			moveVector = Vector3.zero;

			
			if (Input.GetKey(KeyCode.W))
			{
				lastPressed = Time.time;
				moveVector += Camera.main.transform.parent.forward;
			}
			if (Input.GetKey(KeyCode.S))
			{
				lastPressed = Time.time;
				moveVector -= Camera.main.transform.parent.forward;
			}
			if (Input.GetKey(KeyCode.A))
			{
				lastPressed = Time.time;
				moveVector -= Camera.main.transform.parent.right;
			}
			if (Input.GetKey(KeyCode.D))
			{
				lastPressed = Time.time;
				moveVector += Camera.main.transform.parent.right;
			}

			// This is a really hacky way to remove the players ability to move after they have won the game
			// It is done this way so that the player still centres on the winning tile, but doesn't let them move off it.
			// Im sorry
			if( won ){
				moveVector = Vector3.zero;
			}


			if( moveVector.Equals( Vector3.zero ) ){
				if( Time.time - lastPressed < 0.25f ){
					moveVector = oldVector;
				} else if( won ){
					moveVector =  new Vector3( currentCellPos.x - HexMetrics.innerRadius/2, currentCellPos.y, currentCellPos.z) - transform.position;
				} else  {
					moveVector =  currentCellPos - transform.position;
				}
				
			}
		
		}
		
		moveVector.Normalize();
		
		Vector3 oldPosition = transform.position;
		Vector3 newPosition =  transform.position + moveVector;
		
		Quaternion newRot = Quaternion.LookRotation(moveVector);
		transform.rotation = Quaternion.Lerp(transform.rotation, newRot, 5f * Time.deltaTime);
		
		transform.position = Vector3.Lerp( transform.position, newPosition, speed*Time.deltaTime ) ;
		
		if( !level.IsPositionAvailable( transform.position ) ){
			transform.position = oldPosition;
		}


		if( won &&  Vector3.Distance( transform.position, new Vector3( currentCellPos.x - HexMetrics.innerRadius/2, currentCellPos.y, currentCellPos.z))< 0.2f ) {
			transform.position = new Vector3( currentCellPos.x - HexMetrics.innerRadius/2, currentCellPos.y, currentCellPos.z);
			transform.rotation = Quaternion.LookRotation( Camera.main.transform.parent.right );
			anim.SetTrigger( "attac" );
			anim.SetBool( "moving", false);
		} else if( Vector3.Distance( transform.position, currentCellPos)< 0.2f ){
			transform.position = currentCellPos;
			anim.SetBool( "moving", false);
		} else {
			anim.SetBool( "moving", true);
		}

		
		
		
	}

	void HandleCellChange()
	{
		HexCoordinates playerHexCoords = HexCoordinates.FromPosition(transform.position);
		HexCell cell = grid.GetCellFromCoordinates(playerHexCoords);
		if (currentCell != cell)
		{
			OnCellEnter(cell);
		}
	}

	

	void OnCellEnter(HexCell cell)
	{
		lastCell = currentCell;
		currentCell = cell;
		lastMove = GetMoveDirection();


		if( !IsSlipping() && !IsBurning() ){
			TurnManager.TriggerNextTurn();
		}


		if( level.level.cells[ level.IndexOfCell( cell ) ] == "ice" ){
			startSlip = Time.time;
			targetCell = level.GetAdjacentCell( currentCell, lastMove );
		}

		if( level.level.cells[ level.IndexOfCell( cell ) ] == "fire" || level.level.cells[ level.IndexOfCell( cell ) ] == "blockade"){
			startBurn = Time.time + 0.2f;
			targetCell = lastCell;
		}

		
		level.CheckLevelLoad( cell );
		EvaluateCombat( cell );
	}

	private void EvaluateCombat( HexCell cell ){
		if( level.GetCellType( cell ) == "fire" ){
			return;
		}
		foreach (EnemyController ec in TurnManager.enemyControllers)
		{
			if( ec.GetTargetCell().Equals( cell ) ){
				if( ec.strength > strength ){
					Debug.Log( "Lose, back to overworld" );

				}else {
					won = true;
					winTime = Time.time;
					LevelGrid.CompleteLevel( level.levelName );
					SoundManager.soundManager.PlaySolvedSound();
				}
			}
		}
	}

	public static MoveDirection GetMoveDirection()
	{
		{
			if (currentCell.coordinates.Z == lastCell.coordinates.Z) // if we stayed on the same row
			{
				int xMove = lastCell.coordinates.X - currentCell.coordinates.X;
				switch (xMove)
				{
					case -1:
						return MoveDirection.E;
					case 1:
						return MoveDirection.W;
					default:
						return MoveDirection.NULL_MOVE;
				}
			}
			else if (currentCell.coordinates.X == lastCell.coordinates.X) //if we moved along line y = x
			{
				int zMove = lastCell.coordinates.Z - currentCell.coordinates.Z;
				switch (zMove)
				{
					case -1:
						return MoveDirection.NE;
					case 1:
						return MoveDirection.SW;
					default:
						return MoveDirection.NULL_MOVE;
				}
			}
			else // if we moved along y = -x
			{
				int zMove = lastCell.coordinates.Z - currentCell.coordinates.Z;
				switch (zMove)
				{
					case -1:
						return MoveDirection.NW;
					case 1:
						return MoveDirection.SE;
					default:
						return MoveDirection.NULL_MOVE;
				}
			}
		}
	}

	private bool IsSlipping(){
		return Time.time - startSlip < 1f;
	}

	private bool IsBurning(){
		return  ( Time.time - startBurn < 0.45f && Time.time - startBurn > 0f );
	}

	public static HexCell GetCurrentCell(){
		return currentCell;
	}

}
