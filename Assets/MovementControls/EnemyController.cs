using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MovementController
{
	public HexCoordinates startingCoordinates;

	private HexCell targetCell;
	private HexCell currentCell;
	private bool isMoving = false;
	private float startMoveTime = 0f;
	private Vector3 raiseVector = Vector3.up * 6f;

	public int strength = 0;

	private bool reverseTraversal = false;

	private MoveDirection lastMove;

	private Animator anim;

	private void Start()
	{
		anim = GetComponent<Animator>();
		currentCell = grid.GetCellFromCoordinates(startingCoordinates);
		transform.position = currentCell.transform.position + raiseVector;
	}

	private void Update()
	{
		HandleMovement();
	}

	public override void HandleMovement()
	{
		if (targetCell == null || targetCell.Equals(currentCell)) return;

		float speed = 50f;
        float distCovered = (Time.time - startMoveTime) * speed;
        float fracJourney = distCovered / ( HexMetrics.innerRadius*2 );


		Vector3 start = transform.position;
		Vector3 end = targetCell.transform.position + raiseVector;

		if( PlayerController.GetCurrentCell().Equals( targetCell ) ){
			end = new Vector3( end.x + HexMetrics.innerRadius/2, end.y, end.z );
			anim.SetTrigger( "die");
		}
        transform.position = Vector3.Lerp( start, end, fracJourney);


		if( Vector3.Distance( transform.position, end ) <  Vector3.Distance( start, end )*0.05 ){
			currentCell = targetCell;

			if( (grid as LevelGrid ).GetCellType( currentCell ) == "fire" ){
				targetCell = grid.GetAdjacentCell( currentCell, ReverseDirection( lastMove ) );
				startMoveTime = Time.time;
			}
			transform.position = end;
			isMoving = false;
		}

		
		Quaternion newRot = Quaternion.LookRotation( end - transform.position );
		transform.rotation = Quaternion.Lerp(transform.rotation, newRot, 5f * Time.deltaTime);
	}

	private bool slipped = false;
	public void TargetHexCell()
	{

		MoveDirection playerMove = PlayerController.GetMoveDirection();
		MoveDirection nextMove;

		LevelGrid levelGrid = grid as LevelGrid;

		if( reverseTraversal ) {
			nextMove = playerMove;
		} else {
			nextMove = ReverseDirection( playerMove );
		}
		
		if( isMoving ){
			currentCell = targetCell;
		}
		targetCell = grid.GetAdjacentCell( currentCell, nextMove );
		
		if( !( grid as LevelGrid ).IsCellAvailable( targetCell ) ){
			targetCell = currentCell;
		} else {
			isMoving = true;
			startMoveTime = Time.time;
			
		}
		
		lastMove = nextMove;
		if( levelGrid.GetCellType( targetCell ) == "ice" ){
			HexCell next =  grid.GetAdjacentCell( targetCell, nextMove );
			if( ( grid as LevelGrid ).IsCellAvailable( next ) ){
				TargetHexCell();
			}
		}

		if( levelGrid.GetCellType( targetCell ) == "reverse"  ){
			reverseTraversal = !reverseTraversal;
		}
	}

	public HexCell GetCurrentCell(){
		return currentCell;
	}

	public HexCell GetTargetCell(){
		return targetCell;
	}

}
