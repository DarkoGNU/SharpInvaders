namespace SharpInvaders
{
    // Store some settings
    public static class Settings
    {
        public static int Width { get; } = 800; // Screen width
        public static int Height { get; } = 600; // screen height
        public static float EnemyScale { get; } = 0.7f; // enemy scale
        public static float PlayerScale { get; } = 1f; // player scale
        public static int PlayerMargin { get; } = 20; // player's margin from edges of screen
        public static float BulletSpeed { get; } = 800f; // speed of player's bullets
        public static float EnemyBulletSpeed { get; } = 300f; // speed of enemies' bullets
        public static double EnemyMoveInterval { get; } = 2d; // how often enemies move at the beginning
        public static float PlayerSpeed { get; } = 200f; // player's speed
        public static double PlayerShootInterval = 0.5d; // how often player shoots
        public static double EnemyShootChance = 0.0002d; // how probable is it for the enemy to shoot on each update (initially)
    }
}
