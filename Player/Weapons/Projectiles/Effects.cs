using Sandbox;

namespace Amper.Source1;

public partial class Projectile
{
	public virtual string ExplosionParticleName => "";
	public virtual string ExplosionSoundEffect => "";


	/// <summary>
	/// Display clientside particle effects on detonation.
	/// </summary>
	[ClientRpc]
	public virtual void DoExplosionEffect( Vector3 position, Vector3 normal )
	{
		Host.AssertClient();

		var boom = Particles.Create( ExplosionParticleName, position );
		boom.SetForward( 0, normal );
		Sound.FromWorld( ExplosionSoundEffect, Position );
	}

	[ClientRpc]
	public void DoScorchTrace( Vector3 position, Vector3 normal )
	{
		var tr = Trace.Ray( position + normal * 10, position - normal * 10 )
			.Ignore( this )
			.WorldOnly()
			.Run();

		if ( tr.Hit )
		{
			DecalSystem.PlaceOnWorld(
				Material.Load( "materials/decals/scorch.vmat" ),
				tr.EndPosition,
				Rotation.LookAt( tr.Normal ),
				new Vector3( 128, 128, 3 )
			);
		}
	}

	public virtual string TrailAttachment => "trail";
	public virtual string TrailParticleName => "";
	public Particles Trail { get; set; }

	[ClientRpc]
	public virtual void CreateTrails()
	{
		DeleteTrails( true );

		if ( !string.IsNullOrEmpty( TrailParticleName ) )
			Trail = Particles.Create( TrailParticleName, this, TrailAttachment );
	}

	[ClientRpc]
	public virtual void DeleteTrails( bool immediate = false )
	{
		Trail?.Destroy( true );
	}
}
