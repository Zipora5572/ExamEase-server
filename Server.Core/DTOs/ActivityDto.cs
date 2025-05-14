public class UserActivityPoint
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
}

public class PopularPage
{
    public string Path { get; set; } = "";
    public int Views { get; set; }
}

public class HourlyActivity
{
    public int Hour { get; set; }
    public int Count { get; set; }
}
