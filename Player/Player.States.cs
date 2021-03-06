using Sandbox;
using System;

namespace Amper.Source1;

partial class Source1Player
{

	public bool InWater => WaterLevelType >= WaterLevelType.Feet;
	public bool IsGrounded => GroundEntity.IsValid();
	public bool IsInAir => !IsGrounded;
	public bool IsUnderwater => WaterLevelType >= WaterLevelType.Eyes;
	public bool IsAlive => LifeState == LifeState.Alive;
	public bool IsDead => !IsAlive;

	public bool IsInGodMode => Tags.Has( PlayerTags.GodMode );
	public bool IsInBuddha => Tags.Has( PlayerTags.Buddha );
}
