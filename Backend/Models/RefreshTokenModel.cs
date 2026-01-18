public class RefreshTokenModel
{
    public int Id { get; set; }
    public int User { get; set; }
    public string Token { get; set; }
    public DateTime Expire { get; set; }
    public bool Valid { get; set; }
}
