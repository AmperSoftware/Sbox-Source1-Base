﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace Amper.Source1;

partial class GameMovement
{
	public const float CHECK_STUCK_INTERVAL = 1;
	public int CHECK_STUCK_TICK_INTERVAL => (int)(CHECK_STUCK_TICK_INTERVAL / Global.TickInterval);
	public const float CHECKSTUCK_MINTIME = 0.05f;

	public bool CanStuck()
	{
		return Player.MoveType != MoveType.MOVETYPE_NOCLIP &&
			Player.MoveType != MoveType.None &&
			Player.MoveType != MoveType.MOVETYPE_ISOMETRIC &&
			Player.MoveType != MoveType.MOVETYPE_OBSERVER;
	}

	protected void ResetStuckOffsets()
	{
		Player.m_StuckLast = 0;
	}

	protected bool CheckStuck()
	{
		CreateStuckTable();

		var traceresult = TraceBBox( Move.Position, Move.Position );
		var hitent = traceresult.Entity;
		if ( !hitent.IsValid() ) 
		{
			ResetStuckOffsets();
			return false;
		}

		var vBase = Move.Position;

		Vector3 offset, test;

		// Only an issue on the client.
		var idx = Host.IsServer ? 0 : 1;

		var fTime = Time.Now;

		// Too soon?
		if ( Player.m_StuckCheckTime[idx] >= fTime - CHECKSTUCK_MINTIME )
			return true;

		Player.m_StuckCheckTime[idx] = fTime;

		GetRandomStuckOffsets( out offset );
		test = vBase + offset;

		traceresult = TraceBBox( test, test );
		if ( !traceresult.Entity.IsValid() ) 
		{
			ResetStuckOffsets();
			Move.Position = test;
			return false;
		}

		return true;
	}

	Vector3[] rgv3tStuckTable { get; set; } = new Vector3[54];
	static bool stuckFirstTime { get; set; } = true;

	int GetRandomStuckOffsets( out Vector3 offset )
	{
		// Last time we did a full
		int idx;
		idx = Player.m_StuckLast++;

		offset = rgv3tStuckTable[idx % 54];
		return (idx % 54);
	}

	void CreateStuckTable()
	{
		float x, y, z;
		int idx;
		int i;
		var zi = new float[3];

		if ( !stuckFirstTime )
			return;

		stuckFirstTime = false;
		// Log.Info( "Created Stuck Table" );

		idx = 0;
		// Little Moves.
		x = y = 0;
		// Z moves
		for ( z = -0.125f; z <= 0.125; z += 0.125f )
		{
			rgv3tStuckTable[idx][0] = x;
			rgv3tStuckTable[idx][1] = y;
			rgv3tStuckTable[idx][2] = z;
			idx++;
		}
		x = z = 0;
		// Y moves
		for ( y = -0.125f; y <= 0.125; y += 0.125f )
		{
			rgv3tStuckTable[idx][0] = x;
			rgv3tStuckTable[idx][1] = y;
			rgv3tStuckTable[idx][2] = z;
			idx++;
		}
		y = z = 0;
		// X moves
		for ( x = -0.125f; x <= 0.125; x += 0.125f )
		{
			rgv3tStuckTable[idx][0] = x;
			rgv3tStuckTable[idx][1] = y;
			rgv3tStuckTable[idx][2] = z;
			idx++;
		}

		// Remaining multi axis nudges.
		for ( x = -0.125f; x <= 0.125; x += 0.250f )
		{
			for ( y = -0.125f; y <= 0.125; y += 0.250f )
			{
				for ( z = -0.125f; z <= 0.125; z += 0.250f )
				{
					rgv3tStuckTable[idx][0] = x;
					rgv3tStuckTable[idx][1] = y;
					rgv3tStuckTable[idx][2] = z;
					idx++;
				}
			}
		}

		// Big Moves.
		x = y = 0;
		zi[0] = 0.0f;
		zi[1] = 1.0f;
		zi[2] = 6.0f;

		for ( i = 0; i < 3; i++ )
		{
			// Z moves
			z = zi[i];
			rgv3tStuckTable[idx][0] = x;
			rgv3tStuckTable[idx][1] = y;
			rgv3tStuckTable[idx][2] = z;
			idx++;
		}

		x = z = 0;

		// Y moves
		for ( y = -2.0f; y <= 2.0f; y += 2 )
		{
			rgv3tStuckTable[idx][0] = x;
			rgv3tStuckTable[idx][1] = y;
			rgv3tStuckTable[idx][2] = z;
			idx++;
		}
		y = z = 0;
		// X moves
		for ( x = -2.0f; x <= 2.0f; x += 2.0f )
		{
			rgv3tStuckTable[idx][0] = x;
			rgv3tStuckTable[idx][1] = y;
			rgv3tStuckTable[idx][2] = z;
			idx++;
		}

		// Remaining multi axis nudges.
		for ( i = 0; i < 3; i++ )
		{
			z = zi[i];

			for ( x = -2.0f; x <= 2.0f; x += 2.0f )
			{
				for ( y = -2.0f; y <= 2.0f; y += 2 )
				{
					rgv3tStuckTable[idx][0] = x;
					rgv3tStuckTable[idx][1] = y;
					rgv3tStuckTable[idx][2] = z;
					idx++;
				}
			}
		}
	}
}
