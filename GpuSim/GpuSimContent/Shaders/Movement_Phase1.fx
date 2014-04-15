#include "root.fx"

bool IsStationary(UNIT unit)
{
/* modulo bit lookup */
	int _b = ((int)(unit.a * 255) - (int)(unit.a * 128) * 2) / 1;
	bool _bit = _b;

	int _b2 = ((int)(unit.b * 255) - (int)(unit.a * 128) * 2) / 1;
	bool _bit2 = _b2;

	int _b3 = ((int)(unit.g * 255) - (int)(unit.a * 128) * 2) / 1;
	bool _bit3 = _b3;

	return _bit && _bit2 && _bit3;


/* texture info lookup */
	UNIT extra_data = tex2D(TextureSampler, float2(unit.g, unit.a));
	bool r1 = extra_data.a > .5;
	bool r2 = extra_data.g > .5;
	bool r3 = extra_data.b > .5;
	return r1 && r2 && r3;


/* simple calc
	return unit.a > .5 && unit.b > .5 && unit.g > .5;
*/

/* arithmetic bit lookup
	int b = (int)(unit.a * 255);
	bool bit = (b - (b / 2) * 2) / 1;

	int b2 = (int)(unit.b * 255);
	bool bit2 = (b2 - (b2 / 4) * 4) / 2;

	int b3 = (int)(unit.g * 255);
	bool bit3 = (b3 - (b3 / 8) * 8) / 4;

	return bit && bit2 && bit3;
*/
}

PROCEDURE

	// Check four directions to see if something is incoming
	UNIT right = lookup(RIGHT_ONE);
	if (right.direction == LEFT) PUT_HERE right;

	UNIT up = lookup(UP_ONE);
	if (up.direction == DOWN) PUT_HERE up;

	UNIT left = lookup(LEFT_ONE);
	if (left.direction == RIGHT) PUT_HERE left;

	UNIT down = lookup(DOWN_ONE);
	if (down.direction == UP) PUT_HERE down;

	OUTPUT.change = MOVED;

	// If something is here already, they have the right to stay here
	UNIT here = lookup(HERE);
	if (here.direction IS_VALID_DIRECTION)
	{
		PUT_HERE here;
		OUTPUT.change = STAYED;
	}

END
