using Sandbox;

namespace Amper.Source1;

partial class GameRules
{
	// This is being called by Sandbox's native "kill" command.
	public override void DoPlayerSuicide( Client cl )
	{
		var player = cl.Pawn as Source1Player;
		if ( player == null ) return;

		player.CommitSuicide( explode: false );
	}

	[ConCmd.Server( "noclip", Help = "Spontaneous combustion!" )]
	public static void Command_Noclip()
	{
		var client = ConsoleSystem.Caller;
		if ( client == null )
			return;

		var player = client.Pawn as Source1Player;
		if ( player == null ) 
			return;

		// If player is not in noclip, enable it.
		if ( player.MoveType != NativeMoveType.NoClip )
		{
			player.SetParent( null );
			player.MoveType = NativeMoveType.NoClip;
			Log.Info( $"noclip ON for {client.Name}" );
			return;
		}

		player.MoveType = NativeMoveType.Walk;
		Log.Info( $"noclip OFF for {client.Name}" );
	}

	[ConCmd.Server( "explode", Help = "Spontaneous combustion!" )]
	public static void Command_Explode()
	{
		var client = ConsoleSystem.Caller;
		if ( client == null )
			return;

		var player = client.Pawn as Source1Player;
		if ( player == null )
			return;

		player.CommitSuicide( explode: true );
	}

	[ConCmd.Admin( "god" )]
	public static void Command_God()
	{
		var client = ConsoleSystem.Caller;
		if ( client == null )
			return;

		var player = client.Pawn as Source1Player;
		if ( player == null )
			return;

		if ( player.IsInGodMode )
		{
			player.Tags.Remove( PlayerTags.GodMode );
			Log.Info( $"Disabled god for {client.Name}" );
		}
		else
		{
			player.Tags.Add( PlayerTags.GodMode );
			Log.Info( $"Enabled god for {client.Name}" );
		}
	}

	[ConCmd.Admin( "respawn" )]
	public static void Command_Respawn()
	{
		var client = ConsoleSystem.Caller;
		if ( client == null )
			return;

		var player = client.Pawn as Source1Player;
		if ( player == null )
			return;

		player.Respawn();
	}

	[ConVar.Server] public static float sv_damageforce_scale { get; set; } = 1;

#if false
	[ConCmd.Server( "sv_dumpteams" ), ConCmd.Client( "cl_dumpteams" )]
	public static void Command_DumpTeams()
	{
		foreach ( var team in TeamManager.Teams.Values )
		{
			Log.Info( $"Team: {team.Name}" );
			Log.Info( $"- Title: {team.Title}" );
			Log.Info( $"- Color: {team.Color}" );
			Log.Info( $"- Is Playable: {team.IsPlayable}" );
			Log.Info( $"- Is Joinable: {team.IsJoinable}" );
		}
	}
#endif
}
