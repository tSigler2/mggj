using System;
using System;
using Godot;
using Godot;

public partial class BossTwoBulletThree : BaseEntity
{
	[Export]
	public int Damage;

	[Export]
	public string SpritePath = "./assets/art/projectile/PhoebeBullet.png";

	public Sprite2D sprite;
	public Vector2 Velocity;
	public Vector2 Target;
	public Player p;

	//public float Speed = 150f;
	public double bulletSpeed;

	private CollisionShape2D Collision;

	public int ShiftDirection = 1;
	public float ShiftStrength = 200f;

	public override void _Ready()
	{
		base._Ready();

		sprite = new Sprite2D();
		sprite.Texture = (Texture2D)ResourceLoader.Load(SpritePath);
		AddChild(sprite);

		Collision = new CollisionShape2D();
		Collision.Shape = new RectangleShape2D { Size = sprite.Texture.GetSize() };
		Collision.Disabled = false;

		SetCollisionLayerValue(1, false);
		SetCollisionLayerValue(2, true);
		SetCollisionMaskValue(1, true);
		SetCollisionMaskValue(2, false);

		AddChild(Collision);

		Monitoring = true;
		Monitorable = true;

		BodyEntered += OnBodyEntered;

		ProcessMode = Node.ProcessModeEnum.Always;
		Collision.ProcessMode = Node.ProcessModeEnum.Always;
		Show();
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 toCenter = (Target - Position).Normalized();

		Vector2 tangent = toCenter.Rotated(ShiftDirection * Mathf.Pi / 2);

		Vector2 finalVel = toCenter * (float)bulletSpeed + tangent * ShiftStrength;

		Position += finalVel * (float)delta;

		Rotation = finalVel.Angle() + Mathf.Pi;

		if (this.p.cooldown == 0)
			CheckPlayerCollision();

		if ((Target - Position).Length() < 5f)
			QueueFree();
	}

	private void OnBodyEntered(Node body)
	{
		if (body is Player player)
		{
			player.Health -= Damage;
			QueueFree();
			GD.Print("Collision Detected with Player");
		}
	}

	private void CheckPlayerCollision()
	{
		var bx = Position.X;
		var by = Position.Y;
		var bex = Collision.Shape.GetRect().Size.X;
		var bey = Collision.Shape.GetRect().Size.Y;

		var px = this.p.Position.X;
		var py = this.p.Position.Y;
		var pex = this.p.Collision.Shape.GetRect().Size.X;
		var pey = this.p.Collision.Shape.GetRect().Size.Y;

		bool overlapX = bx <= px + pex && bx + bex >= px;
		bool overlapY = by <= py + pey && by + bey >= py;

		if (overlapX && overlapY)
		{
			this.p.Health -= 1;
			this.p.cooldown = 30;
			GD.Print("Collision Detected");
		}
	}
}
