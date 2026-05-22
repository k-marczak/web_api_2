namespace web_api_2.Models;

public class PCComponent
{
    public int PCId { get; set; }
    public PC PC { get; set; } = null!;

    public string ComponentCode { get; set; } = null!;
    public Component Component { get; set; } = null!;

    public int Amount { get; set; }
}