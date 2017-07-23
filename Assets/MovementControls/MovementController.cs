using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementController : MonoBehaviour
{
	public static HexGrid grid;
	public enum MoveDirection { NULL_MOVE, NW, NE, E, SE, SW, W }
	public float speed = 10f;

	public abstract void HandleMovement();

	public virtual MoveDirection ReverseDirection( MoveDirection dir ){
		MoveDirection nextMove;
		switch (dir)
			{
				case MoveDirection.NULL_MOVE:
					nextMove = MoveDirection.NULL_MOVE;
					break;
				case MoveDirection.NW:
					nextMove = MoveDirection.SE;
					break;
				case MoveDirection.NE:
					nextMove = MoveDirection.SW;
					break;
				case MoveDirection.E:
					nextMove = MoveDirection.W;
					break;
				case MoveDirection.SE:
					nextMove = MoveDirection.NW;
					break;
				case MoveDirection.SW:
					nextMove = MoveDirection.NE;
					break;
				case MoveDirection.W:
					nextMove = MoveDirection.E;
					break;
				default:
					nextMove = MoveDirection.NULL_MOVE;
					break;
			}
		return nextMove;
	}

}
