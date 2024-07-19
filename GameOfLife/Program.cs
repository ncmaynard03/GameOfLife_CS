namespace GameOfLife
{
    internal class Program
    {

        
        static void Main(string[] args)
        {
            using (var game = new Window(800, 800, "Graph"))
            {
                game.Run();
            }
        }
    }
}