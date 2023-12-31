﻿namespace BomberMatch
{
	public static class Maps
	{
		// 0 - пустота
		// 1 - поле без респауна
		// 2 - поле с респауном

		public static readonly int[,] Classic5x5B2 = new int[5, 5]
		{
			{ 2, 1, 1, 1, 1 },
			{ 1, 0, 1, 0, 1 },
			{ 1, 1, 1, 1, 1 },
			{ 1, 0, 1, 0, 1 },
			{ 1, 1, 1, 1, 2 }
		};

		public static readonly int[,] Classic7x7B2 = new int[7, 7]
		{
			{ 2, 1, 1, 1, 1, 1, 1 },
			{ 1, 0, 1, 0, 1, 0, 1 },
			{ 1, 1, 1, 1, 1, 1, 1 },
			{ 1, 0, 1, 0, 1, 0, 1 },
			{ 1, 1, 1, 1, 1, 1, 1 },
			{ 1, 0, 1, 0, 1, 0, 1 },
			{ 1, 1, 1, 1, 1, 1, 2 }
		};

		public static readonly int[,] Classic7x7B4 = new int[7, 7]
		{
			{ 2, 1, 1, 1, 1, 1, 2 },
			{ 1, 0, 1, 0, 1, 0, 1 },
			{ 1, 1, 1, 1, 1, 1, 1 },
			{ 1, 0, 1, 0, 1, 0, 1 },
			{ 1, 1, 1, 1, 1, 1, 1 },
			{ 1, 0, 1, 0, 1, 0, 1 },
			{ 2, 1, 1, 1, 1, 1, 2 }
		};

		public static readonly int[,] Classic9x9B2 = new int[9, 9]
		{
			{ 2, 1, 1, 1, 1, 1, 1, 1, 1 },
			{ 1, 0, 1, 0, 1, 0, 1, 0, 1 },
			{ 1, 1, 1, 1, 1, 1, 1, 1, 1 },
			{ 1, 0, 1, 0, 1, 0, 1, 0, 1 },
			{ 1, 1, 1, 1, 1, 1, 1, 1, 1 },
			{ 1, 0, 1, 0, 1, 0, 1, 0, 1 },
			{ 1, 1, 1, 1, 1, 1, 1, 1, 1 },
			{ 1, 0, 1, 0, 1, 0, 1, 0, 1 },
			{ 1, 1, 1, 1, 1, 1, 1, 1, 2 }
		};

		public static readonly int[,] Classic9x9B4 = new int[9, 9]
		{
			{ 2, 1, 1, 1, 1, 1, 1, 1, 2 },
			{ 1, 0, 1, 0, 1, 0, 1, 0, 1 },
			{ 1, 1, 1, 1, 1, 1, 1, 1, 1 },
			{ 1, 0, 1, 0, 1, 0, 1, 0, 1 },
			{ 1, 1, 1, 1, 1, 1, 1, 1, 1 },
			{ 1, 0, 1, 0, 1, 0, 1, 0, 1 },
			{ 1, 1, 1, 1, 1, 1, 1, 1, 1 },
			{ 1, 0, 1, 0, 1, 0, 1, 0, 1 },
			{ 2, 1, 1, 1, 1, 1, 1, 1, 2 }
		};

        public static readonly int[,] Classic13x13B4 = new int[13, 13]
        {
            { 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2 },
            { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
            { 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2 }
        };

        public static readonly int[,] Empty13x13B4 = new int[13, 13]
        {
            { 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2 }
        };

        public static readonly int[,] Snake15x15B8 = new int[15, 15]
        {
            { 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 1, 2, 1, 0, 1, 2, 1, 0, 1, 2, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 2, 1, 0, 1, 2, 1, 0, 1, 2, 1, 0, 1, 2, 1 }
        };

        public static readonly int[,] Andromeda17x17B4 = new int[17, 17]
        {
            { 2, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 2 },
            { 1, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1 },
            { 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1 },
            { 1, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1 },
            { 0, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0 },
            { 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1 },
            { 1, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 1, 1 },
            { 1, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1 },
            { 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1 },
            { 1, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1 },
            { 1, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 1, 1 },
            { 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1 },
            { 0, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0 },
            { 1, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1 },
            { 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1 },
            { 1, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1 },
            { 2, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 2 }
        };
    }
}
