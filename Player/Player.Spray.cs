using Sandbox;

namespace Amper.Source1;

partial class Source1Player
{
	public TimeSince TimeSinceSprayed { get; set; }

	[ConCmd.Server( "spray" )]
	public static void Command_Spray()
	{
		if ( ConsoleSystem.Caller.Pawn is Source1Player player ) 
		{
			if ( player.TimeSinceSprayed < sv_spray_cooldown ) 
				return;

			var tr = Trace.Ray( player.EyePosition, player.EyePosition + player.EyeRotation.Forward * sv_spray_max_distance )
				.WorldOnly()
				.Run();

			if ( tr.Hit )
			{
				/*
				Sound.FromWorld( "player.sprayer", tr.EndPosition );
				var decal = GameResource.D["data/decal/spray.default.decal"];
				decal.PlaceUsingTrace( tr );
				*/
				player.TimeSinceSprayed = 0;
			}
		}
	}

	[ConVar.Replicated] public static float sv_spray_max_distance { get; set; } = 100;
	[ConVar.Replicated] public static float sv_spray_cooldown { get; set; } = 60;

}
