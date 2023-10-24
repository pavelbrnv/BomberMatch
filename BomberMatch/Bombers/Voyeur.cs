namespace BomberMatch.Bombers
{
    public sealed class Voyeur : IBomber
    {
        #region Legend

        // Arena
        // -1 - wall (can't move)
        // 0  - empty field	(can move)
        // 1  - bomb will explode in 1 step (can't move)
        // 2  - bomb will explode in 2 steps (can't move)
        // ...
        // N  - bomb will explode in N steps (can't move)

        // Bombers
        // first row [i,j] - your place
        // second row [i,j] - enemy 1 place
        // third row [i,j] - enemy 2 place
        // ... row [i,j] - enemy N place

        // Available moves
        // 1 - can move up
        // 2 - can move down
        // 3 - can move left
        // 4 - can move right

        // Return
        // 0 - no action
        // 1 - move up
        // 2 - move down
        // 3 - move left
        // 4 - move right
        // +10 - plant bomb

        #endregion

        public string Name => "Voyeur";

        private int round = 0;

        private string names = "abcdefghijklmnopqrstuvwxyz";

        private readonly IBomber bomber;

        public Voyeur(IBomber bomber)
        {
            this.bomber = bomber;
        }

        public void SetRules(int matchActionsNumber, int detonationRadius, int timeToDetonate)
        {
            //I take no rules! 

            bomber.SetRules(matchActionsNumber, detonationRadius, timeToDetonate);
        }

        public int Go(int[,] arena, int[,] bombers, int[] availableMoves)
        {
            Console.Clear();
            Console.WriteLine($"Round: {++round}");
            Console.WriteLine();

            var rows = arena.GetUpperBound(0) + 1;
            var columns = arena.Length / rows;

            //Arena

            for (var x = 0; x < rows; x++)
            {
                for (var y = 0; y < columns; y++)
                {
                    char glyph = '?';

                    var isBomber = false;
                    var count = bombers.GetUpperBound(0) + 1;
                    for (var index = 0; index < count; index++)
                    {
                        var bx = bombers[index, 0];
                        var by = bombers[index, 1];

                        if (x == bx && y == by)
                        {
                            if (index == 0)
                            {
                                glyph = '@';
                            }
                            else
                            {
                                glyph = names[index]; // I know some day I shall fail
                            }

                            isBomber = true;
                            break;
                        }
                    }

                    if (!isBomber)
                    {
                        switch (arena[x, y])
                        {
                            case -1:
                                glyph = '#'; //wall
                                break;
                            case 0:
                                glyph = '.'; //empty
                                break;
                            default:
                                glyph = arena[x, y] > 9 ? '+' : arena[x, y].ToString()[0]; //bomb
                                break;

                        }
                    }

                    Console.Write($" {glyph} ");
                }

                Console.WriteLine();
            }

            Console.WriteLine();
            Console.ReadKey();

            return bomber.Go(arena, bombers, availableMoves);
        }
    }
}
