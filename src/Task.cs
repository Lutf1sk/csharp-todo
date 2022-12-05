
public class Task
{
	public string Heading = "";

	public string Content
	{
		get { return _content; }
		set {
			_content = value;
			using var reader = new StringReader(_content);
			Heading = reader.ReadLine() ?? "";
		}
	}

	public bool Done { get; set; }

	public string _content = "";

	public Task()
	{
		Content = "";
		Done = false;
	}

	public Task(string content)
	{
		Content = content;
		Done = false;
	}
}

