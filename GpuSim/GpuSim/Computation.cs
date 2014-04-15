using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using System.Diagnostics;

using FragSharpFramework;

namespace GpuSim
{
    public partial class BaseShader : Shader
    {
        protected const float w = 1024.0f, h = 1024.0f;
        protected const float dx = 1.0f / 1024.0f, dy = 1.0f / 1024.0f;

        [VertexShader]
        VertexOut SimpleVertexShader(Vertex data, vec4 cameraPos, float cameraAspect)
        {
            VertexOut Output = VertexOut.Zero;

            Output.Position.w = 1;

            Output.Position.x = (data.Position.x - cameraPos.x) / cameraAspect * cameraPos.z;
            Output.Position.y = (data.Position.y - cameraPos.y) * cameraPos.w;

            Output.TexCoords = data.TextureCoordinate;
            Output.Color = data.Color;

            return Output;
        }
    }

    public partial class DrawGrass : BaseShader
    {
        [FragmentShader]
        color FragmentShader(VertexOut vertex, Sampler Texture)
        {
            color output;

            color lookup1 = Texture[vertex.TexCoords];
            color lookup2 = Texture[vertex.TexCoords / 50];
            color lookup3 = Texture[vertex.TexCoords / 150];

            output = (lookup1 + rgba(1,1,1,1)) * (lookup2 + rgba(1, 1, 1, 1)) / 8 + lookup3 / 4;
            output *= vertex.Color;
            return output;
        }
    }

    public partial class DrawUnit : BaseShader
    {
        readonly vec2 SpriteSize = vec(1.0f / 5.0f, 1.0f / 4.0f);

        color Circle(vec2 pos)
        {
            float r = length(pos - vec(.5f, .5f));
            if (r < .3f)
                return rgba(1, 1, 1, 1);
            else
                return rgba(0, 0, 0, 0);
        }

        color Sprite(unit data, vec2 pos, float cycle_offset, Sampler Texture, float PercentSimStepComplete)
        {
            if (pos.x > 1 || pos.y > 1 || pos.x < 0 || pos.y < 0)
                return color.TransparentBlack;

            pos *= SpriteSize;
            pos.x += SpriteSize.x * (((int)(PercentSimStepComplete / SpriteSize.x) + (int)(cycle_offset * 255)) % 5) * data.b;
            pos.y += (data.direction * 255 - 1) * SpriteSize.y;

            return Texture[pos];
            //return rgba(1,1,1,1);
            //return Circle(pos);
            //return tex2D(TextureSampler, pos);
        }

        vec2 get_subcell_pos(VertexOut vertex)
        {
            vec2 coords = vertex.TexCoords * vec(w, h);
            float i = floor(coords.x);
            float j = floor(coords.y);

            return coords - vec(i, j);
        }

        vec2 direction_to_vec(float direction)
        {
	        float angle = (direction * 255 - 1) * (3.1415926f / 2.0f);
	        return IsValid(direction) ? vec(cos(angle), sin(angle)) : vec2.Zero;
        }


        [FragmentShader]
        color FragmentShader(VertexOut vertex, UnitField Current, UnitField Previous, Sampler Texture, float PercentSimStepComplete)
        {
            color output = color.TransparentBlack;

            unit cur = Current[Here];
	        unit pre = Previous[Here];
            
            //pre = cur;
            //output = rgba(cur.direction * 50, 0, 0, 1);
            //return output;

            vec2 subcell_pos = get_subcell_pos(vertex);


            //output = rgba(subcell_pos.x, subcell_pos.y, cur.direction * 50, 1);


            if (cur.a == pre.a && cur.a != 0)
	        {
		        if (PercentSimStepComplete > .5) pre = cur;

                pre.b = 0;
                output += Sprite(pre, subcell_pos, cur.a, Texture, PercentSimStepComplete);
	        }
            else
            {
                if (IsValid(cur.direction))
                {
                    vec2 vel = direction_to_vec(cur.direction);

                    cur.b = 1;
                    output += Sprite(cur, subcell_pos + (1 - PercentSimStepComplete) * vel, cur.a, Texture, PercentSimStepComplete);
                }

                if (IsValid(pre.direction))
                {
                    vec2 vel = direction_to_vec(pre.direction);

                    pre.b = 1;
                    output += Sprite(pre, subcell_pos - PercentSimStepComplete * vel, pre.a, Texture, PercentSimStepComplete);
                }
            }

            return output;
        }
    }

    public partial class GridComputation : BaseShader
    {
        [VertexShader]
        VertexOut GridVertexShader(Vertex data, vec4 cameraPos, float cameraAspect)
        {
            VertexOut Output = VertexOut.Zero;

            Output.Position.w = 1;

            Output.Position.x = (data.Position.x - cameraPos.x) / cameraAspect * cameraPos.z;
            Output.Position.y = (data.Position.y - cameraPos.y) * cameraPos.w;

            Output.TexCoords = data.TextureCoordinate;
            Output.Color = data.Color;

            return Output;
        }
    }

    public partial class Movement_Phase1 : BaseShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, UnitField Current)
        {
            unit output = unit.Nothing;

            //unit check = Current[UpOne];
            //if (abs(check.direction - Dir.Up) < .01)
            //{
            //    return check;
            //}
            //else
            //{
            //    unit _here = Current[Here];
            //    return _here;
            //}

	        // Check four directions to see if something is incoming
	        unit right = Current[RightOne];
	        if (right.direction == Dir.Left) output = right;

	        unit up = Current[UpOne];
	        if (up.direction == Dir.Down) output = up;

	        unit left = Current[LeftOne];
	        if (left.direction == Dir.Right) output = left;

	        unit down = Current[DownOne];
	        if (down.direction == Dir.Up) output = down;

	        output.change = Change.Moved;

	        // If something is here already, they have the right to stay here
	        unit here = Current[Here];
	        if (IsValid(here.direction))
	        {
		        output = here;
                output.change = Change.Stayed;
	        }

            return output;
/*
            unit here = Current[Here], output = unit.Nothing;

            // Check if something is here already
            if (Something(here))
            {
                // If so, they have the right to stay so keep them here
                output = here;
                output.change = Change.Stayed;
                return output;
            }
            else
            {
                // Otherwise, check each direction to see if something is incoming
                unit
                    right = Current[RightOne],
                    up = Current[UpOne],
                    left = Current[LeftOne],
                    down = Current[DownOne];

                if (right.direction == Dir.Left)  output = right;
                if (up.direction    == Dir.Down)  output = up;
                if (left.direction  == Dir.Right) output = left;
                if (down.direction  == Dir.Up)    output = down;

                output.change = Change.Moved;

                return output;
            }
 * */
        }
    }

    public partial class Movement_Phase2 : BaseShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, UnitField Current, UnitField Previous)
        {
            unit result = Current[Here];
            unit prior = Previous[Here];

            unit ahead = Current[dir_to_vec(prior.direction)];
            if (ahead.change == Change.Moved && ahead.direction == prior.direction)
                result = unit.Nothing;

            // If unit hasn't moved, change direction
            if (result.a == prior.a && Something(result))
                TurnLeft(ref result);

            return result;
        }
    }
}
