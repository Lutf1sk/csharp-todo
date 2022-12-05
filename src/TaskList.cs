
public class TaskList
{
	public string Title { get; set; }
	public List<Task> Tasks { get; set; }

	public TaskList()
	{
		Title = "";
		Tasks = new List<Task>();
	}

	public TaskList(string title)
	{
		Title = title;
		Tasks = new List<Task>();
	}

	public TaskList(string title, List<Task> tasks)
	{
		Title = title;
		Tasks = tasks;
	}
}

