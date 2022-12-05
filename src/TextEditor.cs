
public class TextEditor
{
	public string Text;
	public int CursorPos;

	private int _targetCol;

	public TextEditor(string text)
	{
		Text = text;
		CursorPos = text.Length;
		_targetCol = CursorPos;
	}

	public int FindLineStart()
	{
		int pos = CursorPos;
		while (pos > 0) {
			if (Text[--pos] == '\n') {
				++pos;
				break;
			}
		}
		return pos;
	}

	public int FindLineEnd()
	{
		int pos = CursorPos;
		while (pos < Text.Length) {
			if (Text[pos] == '\n')
				break;
			++pos;
		}
		return pos;
	}

	public void GotoColumn(int col)
	{
		CursorPos = Math.Min(FindLineStart() + col, FindLineEnd());
	}

	public void CursorUp()
	{
		int lineStart = FindLineStart();
		if (lineStart <= 0)
			return;

		CursorPos = lineStart - 1;
		GotoColumn(_targetCol);
	}

	public void CursorDown()
	{
		int lineEnd = FindLineEnd();
		if (lineEnd >= Text.Length)
			return;

		CursorPos = lineEnd + 1;
		GotoColumn(_targetCol);
	}

	public int FindWordForward()
	{
		int pos = CursorPos;
		while (pos < Text.Length && Char.IsWhiteSpace(Text[pos]))
			++pos;
		while (pos < Text.Length && !Char.IsWhiteSpace(Text[pos]))
			++pos;
		return pos;
	}

	public int FindWordBackward()
	{
		int pos = CursorPos;
		while (pos > 0 && Char.IsWhiteSpace(Text[pos - 1]))
			--pos;
		while (pos > 0 && !Char.IsWhiteSpace(Text[pos - 1]))
			--pos;
		return pos;
	}

	public void StepForward()
	{
		int lineEnd = FindLineEnd();
		if (CursorPos == lineEnd && CursorPos < Text.Length) {
			CursorPos = lineEnd + 1;
			return;
		}
		CursorPos = FindWordForward();
	}

	public void StepBackward()
	{
		int lineStart = FindLineStart();
		if (CursorPos == lineStart && CursorPos > 0) {
			CursorPos = lineStart - 1;
			return;
		}
		CursorPos = FindWordBackward();
	}

	public void DeleteWordForward()
	{
		int lineEnd = FindLineEnd();
		if (CursorPos == lineEnd && CursorPos < Text.Length) {
			Text = Text.Remove(CursorPos, 1);
			return;
		}
		Text = Text.Remove(CursorPos, FindWordForward() - CursorPos);
	}

	public void DeleteWordBackward()
	{
		int lineStart = FindLineStart();
		if (CursorPos == lineStart && CursorPos > 0) {
			Text = Text.Remove(CursorPos - 1, 1);
			--CursorPos;
			return;
		}
		int wordStart = FindWordBackward();
		Text = Text.Remove(wordStart, CursorPos - wordStart);
		CursorPos = wordStart;
	}

	public void Input(ConsoleKeyInfo key)
	{
		bool syncTargetCol = true;

		switch (key.Key)
		{

		// Cursor movement
		case ConsoleKey.LeftArrow:
			if (CursorPos <= 0)
				break;

			if ((key.Modifiers & ConsoleModifiers.Control) == 0)
				--CursorPos;
			else
				StepBackward();
			break;

		case ConsoleKey.RightArrow:
			if (CursorPos >= Text.Length)
				break;

			if ((key.Modifiers & ConsoleModifiers.Control) == 0)
				++CursorPos;
			else
				StepForward();
			break;

		case ConsoleKey.UpArrow:
			syncTargetCol = false;
			CursorUp();
			break;

		case ConsoleKey.DownArrow:
			syncTargetCol = false;
			CursorDown();
			break;

		case ConsoleKey.Home:
			CursorPos = FindLineStart();
			break;

		case ConsoleKey.End:
			CursorPos = FindLineEnd();
			break;

		// Deletion
		case ConsoleKey.Delete:
			if (CursorPos >= Text.Length)
				break;

			if ((key.Modifiers & ConsoleModifiers.Control) == 0)
				Text = Text.Remove(CursorPos, 1);
			else
				DeleteWordForward();
			break;

		case ConsoleKey.Backspace:
			if (CursorPos <= 0 || Text.Length <= 0)
				break;

			if ((key.Modifiers & ConsoleModifiers.Control) == 0) {
				Text = Text.Remove(CursorPos - 1, 1);
				--CursorPos;
			}
			else {
				DeleteWordBackward();
			}
			break;

		// Line break
		case ConsoleKey.Enter:
			Text = Text.Insert(CursorPos, "\n");
			CursorPos++;
			break;


		default:
			// Break if the key is not printable or a modifier is pressed
			if (key.KeyChar == '\u0000')
				break;
			if ((key.Modifiers & ~ConsoleModifiers.Shift) != 0)
				break;

			Text = Text.Insert(CursorPos, "" + key.KeyChar);
			CursorPos++;
			break;
		}

		if (syncTargetCol)
			_targetCol = CursorPos - FindLineStart();
	}

}

