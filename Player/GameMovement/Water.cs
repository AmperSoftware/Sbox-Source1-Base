﻿using Sandbox;

namespace Amper.Source1;

public enum WaterLevelType
{
	NotInWater,
	Feet,
	Waist,
	Eyes
}

partial class GameMovement
{
	WaterLevelType m_nOldWaterLevel { get; set; }

	protected void CheckWaterJump()
	{
		// Determine movement angles
		Move.ViewAngles.AngleVectors( out var forward, out _, out _ );

		// Already water jumping.
		if ( Player.m_flWaterJumpTime != 0 ) 
			return;

		// Don't hop out if we just jumped in
		if ( Move.Velocity[2] < -180 ) 
			return; // only hop out if we are moving up

		// See if we are backing up
		var flatvelocity = Move.Velocity;
		flatvelocity[2] = 0;

		// Must be moving
		var curspeed = flatvelocity.Length;
		flatvelocity = flatvelocity.Normal;

		// see if near an edge
		var flatforward = forward;
		flatforward[2] = 0;
		flatforward = flatforward.Normal;

		// Are we backing into water from steps or something?  If so, don't pop forward
		if ( curspeed != 0 && Vector3.Dot( flatvelocity, flatforward ) < 0 )
			return;

		var vecStart = Move.Position + (GetPlayerMins() + GetPlayerMaxs()) * .5f;
		var vecEnd = vecStart + flatforward * 24;

		var tr = TraceBBox( vecStart, vecEnd );
		if ( tr.Fraction == 1 )
			return;

		vecStart.z = Move.Position.z + GetPlayerViewOffset().z + WATERJUMP_HEIGHT;
		vecEnd = vecStart + flatforward * 24;
		Player.m_vecWaterJumpVel = tr.Normal * -50;

		tr = TraceBBox( vecStart, vecEnd );
		if ( tr.Fraction < 1 )
			return;

		// Now trace down to see if we would actually land on a standable surface.
		vecStart = vecEnd;
		vecEnd.z -= 1024;

		tr = TraceBBox( vecStart, vecEnd );
		if ( tr.Fraction < 1 && tr.Normal.z >= 0.7f )
		{
			Move.Velocity.z = 256;
			Player.AddFlags( PlayerFlags.FL_WATERJUMP );
			Player.m_flWaterJumpTime = 2000;
		}
	}

	public bool InWater()
	{
		return Player.WaterLevelType > WaterLevelType.Feet;
	}

	public virtual bool CheckWater()
	{
		var vPlayerExtents = GetPlayerExtents();
		var vPlayerView = GetPlayerViewOffset();

		// Assume that we are not in water at all.
		Player.WaterLevelType = WaterLevelType.NotInWater;

		var fraction = Player.WaterLevel;
		var playerHeight = vPlayerExtents.z;
		var viewHeight = vPlayerView.z;

		var viewFraction = viewHeight / playerHeight;
		if ( fraction > viewFraction )
		{
			Player.WaterLevelType = WaterLevelType.Eyes;
		}
		else if ( fraction >= 0.5f )
		{
			Player.WaterLevelType = WaterLevelType.Waist;
		}
		else if ( fraction > 0 )
		{
			Player.WaterLevelType = WaterLevelType.Feet;
		}

		if ( m_nOldWaterLevel == WaterLevelType.NotInWater && Player.WaterLevelType != WaterLevelType.NotInWater )
		{
			Player.m_flWaterEntryTime = Time.Now;
		}

		return Player.WaterLevelType > WaterLevelType.Feet;
	}

}
