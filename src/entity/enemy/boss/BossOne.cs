using Godot;

public partial class BossOne : Enemy
{
    public override void _Ready()
    {
        BulletGenerator = (
            double Radius,
            double GenerateSpaceInterval,
            double x,
            double y,
            double BulletSpeed
        ) =>
        {
            double spaceAccumulate = 0.0;
            while (spaceAccumulate < 360.0)
            {
                var bullet = new Bullet();
                AddChild(bullet);
                spaceAccumulate += GenerateSpaceInterval;
            }
        };
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }
}
