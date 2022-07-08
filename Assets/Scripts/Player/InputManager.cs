using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	// AXIS
	public float horizontal = 0.0f;
	public float vertical = 0.0f;
	public Vector3 inputMoveVector;
	public float inputMoveMagnitude = 0f;

	// BUTTONS
	public enum buttonState
	{
		NULL,
		PRESS,
		HOLD,
		RELEASE
	}

	public buttonState P = buttonState.NULL;
	public buttonState SPACE = buttonState.NULL;

    private void Update()
    {
		// AXIS
		horizontal = Input.GetAxis("Horizontal");
		vertical = Input.GetAxis("Vertical");
		inputMoveVector = new Vector3(horizontal, 0f, vertical);
		inputMoveMagnitude = inputMoveVector.magnitude;

		// BUTTONS
		if (Input.GetKeyDown(KeyCode.P))
			P = buttonState.PRESS;
		else if (Input.GetKeyDown(KeyCode.P))
			P = buttonState.RELEASE;
		else if (Input.GetKeyDown(KeyCode.P))
			P = buttonState.HOLD;
		else
			P = buttonState.NULL;

		if (Input.GetKeyDown(KeyCode.Space))
			SPACE = buttonState.PRESS;
		else if (Input.GetKeyDown(KeyCode.Space))
			SPACE = buttonState.RELEASE;
		else if (Input.GetKeyDown(KeyCode.Space))
			SPACE = buttonState.HOLD;
		else
			SPACE = buttonState.NULL;
	}
}
