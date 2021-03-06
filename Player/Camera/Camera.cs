using Sandbox;
using System;

namespace Amper.Source1;

partial class Source1Camera : CameraMode
{
	Vector3 LastPosition { get; set; }

	bool LerpEnabled { get; set; }

	public override void Build( ref CameraSetup camSetup )
	{
		var player = Source1Player.LocalPlayer;
		if ( player == null )
			return;

		DefaultFieldOfView = camSetup.FieldOfView;

		base.Build( ref camSetup );

		camSetup.ViewModel.FieldOfView = ViewModelFieldOfView;
	}

	public override void Update()
	{
		var player = Source1Player.LocalPlayer;
		if ( player == null ) 
			return;

		//
		// Reset default values
		//

		Viewer = player;
		Position = player.EyePosition;
		Rotation = player.EyeRotation;
		ZNear = 1;
		FieldOfView = 0;
		LerpEnabled = true;
		ViewModelFieldOfView = cl_viewmodel_fov;

		CalculateView( player );

		LastPosition = Position;
		LastFieldOfView = FieldOfView;
	}

	public virtual void CalculateView( Source1Player player )
	{
		if ( player.IsObserver )
		{
			CalculateObserverView( player );
		}
		else
		{
			CalculatePlayerView( player );
		}

		CalculateFieldOfView( player );
	}

	public virtual void CalculatePlayerView( Source1Player player )
	{
		var punch = player.ViewPunchAngle;
		Rotation *= Rotation.From( punch.x, punch.y, punch.z );

		if( cl_thirdperson )
		{
			Viewer = null;

			LerpEnabled = false;
			Position -= Rotation.Forward * cl_thirdperson_distance;
		}
	}

	[ConVar.Client] public static bool cl_thirdperson { get; set; }
	[ConVar.Client] public static float cl_thirdperson_distance { get; set; } = 120;

	public virtual void CalculateObserverView( Source1Player player )
	{
		switch( player.ObserverMode )
		{
			case ObserverMode.Roaming:
				CalculateRoamingCamView( player );
				break;

			case ObserverMode.InEye:
				CalculateInEyeCamView( player );
				break;

			case ObserverMode.Chase:
				CalculateChaseCamView( player );
				break;

			case ObserverMode.Deathcam:
				CalculateDeathCamView( player );
				break;
		}
	}
}
