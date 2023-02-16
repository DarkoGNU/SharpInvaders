namespace SharpInvaders
{
    public static class Settings
    {
        public static int Width { get; } = 800;
        public static int Height { get; } = 600;
        public static float EnemyScale { get; } = 0.7f;
        public static float PlayerScale { get; } = 1f;
        public static int PlayerMargin { get; } = 20;
        public static float BulletSpeed { get; } = 800f;
        public static double EnemyMoveInterval { get; } = 2d;
        public static float PlayerSpeed { get; } = 200f;
        public static double PlayerShootInterval = 0.01d;//0.5d;
        public static double EnemyShootChance = 0.05d;
    }
}
