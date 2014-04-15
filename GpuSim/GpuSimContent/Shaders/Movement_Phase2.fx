#include "root.fx"

void turn_right(inout float direction)
{
	direction += TURN;
	if (direction > DOWN)
		direction = RIGHT;
}

PROCEDURE
    
	UNIT result = lookup(HERE);
	UNIT prior = lookup2(HERE);
	
	UNIT ahead = lookup(HERE, prior.direction);
	if (ahead HAS_MOVED && ahead.direction == prior.direction)
		result = NOTHING;

	// If unit hasn't moved, change direction
	if (result.a == prior.a && result.direction IS_VALID_DIRECTION)
		turn_right(result.direction);
	
	PUT_HERE result;

END
