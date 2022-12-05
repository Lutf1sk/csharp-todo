
public static class ANSI
{
	public const string CSI = "\x1B[";

	// Text styles
	public const string FG_BLK = CSI+"30m";
	public const string FG_RED = CSI+"31m";
	public const string FG_GRN = CSI+"32m";
	public const string FG_YLW = CSI+"33m";
	public const string FG_BLU = CSI+"34m";
	public const string FG_MGN = CSI+"35m";
	public const string FG_CYN = CSI+"36m";
	public const string FG_WHT = CSI+"37m";
	public const string FG_BBLK = CSI+"90m";
	public const string FG_BRED = CSI+"91m";
	public const string FG_BGRN = CSI+"92m";
	public const string FG_BYLW = CSI+"93m";
	public const string FG_BBLU = CSI+"94m";
	public const string FG_BMGN = CSI+"95m";
	public const string FG_BCYN = CSI+"96m";
	public const string FG_BWHT = CSI+"97m";

	public const string BG_BLK = CSI+"40m";
	public const string BG_RED = CSI+"41m";
	public const string BG_GRN = CSI+"42m";
	public const string BG_YLW = CSI+"43m";
	public const string BG_BLU = CSI+"44m";
	public const string BG_MGN = CSI+"45m";
	public const string BG_CYN = CSI+"46m";
	public const string BG_WHT = CSI+"47m";
	public const string BG_BBLK = CSI+"100m";
	public const string BG_BRED = CSI+"101m";
	public const string BG_BGRN = CSI+"102m";
	public const string BG_BYLW = CSI+"103m";
	public const string BG_BBLU = CSI+"104m";
	public const string BG_BMGN = CSI+"105m";
	public const string BG_BCYN = CSI+"106m";
	public const string BG_BWHT = CSI+"107m";

	public const string BOLD = CSI+"1m";
	public const string FAINT = CSI+"2m";
	public const string ITALIC = CSI+"3m";
	public const string UNDERLINE = CSI+"4m";
	public const string NORMAL = CSI+"22m";

	public const string RESET = CSI+"0m";

	// Clear screen
	public const string CLS_TO_END = CSI+"0J";
	public const string CLS_TO_BEG = CSI+"1J";
	public const string CLS = CSI+"2J"; // Some implementations will also reset the cursor position, some don't
	public const string CLS_DELETE_SCROLLBACK = CSI+"3J";

	// Clear line
	public const string CLL_TO_END = CSI+"0K";
	public const string CLL_TO_BEG = CSI+"1K";
	public const string CLL = CSI+"2K";

	// Cursor movement
	public const string CSAVE = CSI+"s";
	public const string CRESTORE = CSI+"u";
	// #define CSET(y, x)	CSI #y ";" #x "H"
	// #define CUP(y)		CSI #y "A"
	// #define CDOWN(y)		CSI #y "B"
	// #define CRIGHT(x)	CSI #x "C"
	// #define CLEFT(x)		CSI #x "D"
	// #define CNEXTL(y)	CSI #y "E"
	// #define CPREVL(y)	CSI #y "F"

	// Extensions
	public const string ALTBUF_ENABLE = CSI+"?1049h";
	public const string ALTBUF_DISABLE = CSI+"?1049l";

	public static class Terminal {
		private static string _buffer = "";

		public static void EnableAltBuf()
		{
			_buffer += ALTBUF_ENABLE;
		}

		public static void DisableAltBuf()
		{
			_buffer += ALTBUF_DISABLE;
		}

		public static void Write(string str)
		{
			_buffer += str;
		}

		public static void Clear()
		{
			_buffer += CLS;
			SetCursor(0, 0);
		}

		public static void SetCursor(int x, int y)
		{
			_buffer += CSI+(x + 1)+";"+(y + 1)+"H";
		}

		public static void SaveCursor()
		{
			_buffer += CSAVE;
		}

		public static void RestoreCursor()
		{
			_buffer += CRESTORE;
		}

		public static void Flush()
		{
			System.Console.Write(_buffer);
			_buffer = "";
		}
	}
}

