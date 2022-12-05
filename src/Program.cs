using static ANSI;
using System.Text.Json;

public class Program
{
	public static int ClampIndex(int index, int count)
	{
		if (index >= count)
			index = count - 1;
		if (index < 0)
			index = 0;
		return index;
	}

	public static void DrawControlHints(string[] controlHints)
	{
		foreach (string hint in controlHints)
			Terminal.Write(BG_WHT+FG_BLK + hint + RESET + " ");
	}

	public static bool Confirm(string prompt)
	{
		Terminal.Write(BG_RED+FG_BLK + prompt + " (Y/n)" + RESET);
		Terminal.Flush();
		ConsoleKeyInfo key = Console.ReadKey();
		return key.Key == ConsoleKey.Y;
	}

	public static string? EditText(string line, string prompt, bool singleLineMode = false)
	{
		TextEditor editor = new TextEditor(line);

 		while (true) {
 			Terminal.Clear();

			// Draw header
	 		DrawControlHints(new string[] {
				" CTRL+D/E Cancel ",
				" CTRL+S Save & Exit ",
			});
			Terminal.Write(FG_BGRN + "\n// TODO: " + prompt + "\n" + RESET);

			// Draw text content
			Terminal.Write(editor.Text.Substring(0, editor.CursorPos));
			Terminal.SaveCursor();
			Terminal.Write(editor.Text.Substring(editor.CursorPos));
			Terminal.RestoreCursor();

			Terminal.Flush();

			// Keyboard input
			ConsoleKeyInfo key = Console.ReadKey();
			bool ctrlPressed = (key.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control;

	 		if ((key.Key == ConsoleKey.E || key.Key == ConsoleKey.D) && ctrlPressed)
				return null;
			if (key.Key == ConsoleKey.S && ctrlPressed)
				return editor.Text;

			if (singleLineMode && key.Key == ConsoleKey.Enter)
				continue;
			editor.Input(key);
 		}
	}

	public static void EditTaskList(List<Task> tasks)
	{
		int taskIdx = 0;

		while (true) {
			Terminal.Clear();

			// Draw header
			DrawControlHints(new string[] {
				" CTRL+D Back ",
				" CTRL+W Delete ",
				" CTRL+E Edit ",
				" CTRL+N New ",
				" Space Mark/Unmark "
			});
			Terminal.Write(FG_BGRN + "\n// TODO: View tasks\n" + RESET);

			// Draw body
			// TODO: This does not scroll if there are more tasks than available lines on the screen
			for (int i = 0; i < tasks.Count; ++i) {
				if (tasks[i].Done)
					Terminal.Write("[" + FG_CYN + "X" + RESET + "]");
				else
					Terminal.Write("[ ]");

				if (i == taskIdx)
					Terminal.Write(BG_WHT+FG_BLK + " " + tasks[i].Heading + " \n" + RESET);
				else
					Terminal.Write(" " + tasks[i].Heading + " \n");
			}

			Terminal.Flush();

			// Keyboard input
			ConsoleKeyInfo key = Console.ReadKey();
			bool ctrlPressed = (key.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control;

			if ((key.Key == ConsoleKey.D && ctrlPressed) || key.Key == ConsoleKey.Escape)
				return;
			if (key.Key == ConsoleKey.N && ctrlPressed) {
				string? newContent = EditText("", "Edit task description");
				if (newContent != null)
					tasks.Add(new Task(newContent));
			}

			if (tasks.Count > 0) {
				if (key.Key == ConsoleKey.UpArrow)
					taskIdx--;
				if (key.Key == ConsoleKey.DownArrow)
					taskIdx++;

				if ((key.Key == ConsoleKey.E && ctrlPressed) || key.Key == ConsoleKey.Enter) {
					string? newContent = EditText(tasks[taskIdx].Content, "Edit task description");
					if (newContent != null)
						tasks[taskIdx] = new Task(newContent);
				}
				if (key.Key == ConsoleKey.W && ctrlPressed)
					if (Confirm("Delete task?"))
						tasks.RemoveAt(taskIdx);
				if (key.Key == ConsoleKey.Spacebar)
					tasks[taskIdx].Done = !tasks[taskIdx].Done;
			}

			taskIdx = ClampIndex(taskIdx, tasks.Count);
		}
	}

	public static void Main(string[] args)
	{
		const string path = "./todo.json";

		List<TaskList> lists;
		try {
			lists = JsonSerializer.Deserialize<List<TaskList>>(File.ReadAllText(path)) ?? new List<TaskList>();
		}
		catch (Exception) {
			Console.WriteLine("Failed to load '" + path + "', creating new list");
			lists = new List<TaskList>();
		}

		int listIdx = 0;

		Terminal.EnableAltBuf();

		while (true) {
			Terminal.Clear();

			// Draw header
			DrawControlHints(new string[] {
				" CTRL+D Quit ",
				" CTRL+W Delete ",
				" CTRL+E Rename ",
				" CTRL+N New ",
			});

			Terminal.Write(FG_BGRN + "\n// TODO: View task lists\n" + RESET);

			// Draw body
			// TODO: This does not scroll if there are more lists than available lines on the screen
			for (int i = 0; i < lists.Count; ++i) {
				if (i == listIdx)
					Terminal.Write(" - " + BG_WHT+FG_BLK + " " + lists[i].Title + " \n" + RESET);
				else
					Terminal.Write(" -  " + lists[i].Title + " \n");
			}

			Terminal.Flush();

			// Keyboard input
			ConsoleKeyInfo key = Console.ReadKey();
			bool ctrlPressed = (key.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control;

			if ((key.Key == ConsoleKey.D && ctrlPressed) || key.Key == ConsoleKey.Escape) {
				try {
					var options = new JsonSerializerOptions { WriteIndented = true };
					File.WriteAllText(path, JsonSerializer.Serialize(lists, options));
				}
				catch (Exception) {
					if (!Confirm("Failed to save '" + path + "', do you still want to quit?"))
						continue;
				}

				Terminal.DisableAltBuf();
				Terminal.Flush();
				return;
			}
			if (key.Key == ConsoleKey.N && ctrlPressed)
				lists.Add(new TaskList("New list"));

			if (lists.Count > 0) {
				if (key.Key == ConsoleKey.UpArrow)
					listIdx--;
				if (key.Key == ConsoleKey.DownArrow)
					listIdx++;

				if (key.Key == ConsoleKey.Enter)
					EditTaskList(lists[listIdx].Tasks);
				if (key.Key == ConsoleKey.W && ctrlPressed)
					if (Confirm("Delete list?"))
						lists.RemoveAt(listIdx);
				if (key.Key == ConsoleKey.E && ctrlPressed) {
					string? newName = EditText(lists[listIdx].Title, "Rename task list", true);
					if (newName != null)
						lists[listIdx].Title = newName;
				}
			}

			listIdx = ClampIndex(listIdx, lists.Count);
		}
	}
}

