public static class EnemySpawnOrder
{
    private static long nextSpawnOrder = 0;

    public static long GetNext()
    {
        return nextSpawnOrder++;
    }
}