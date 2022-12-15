namespace TestProject1.Models;
/*
    This is a very highly simplified user model. 
    Only the fields that are necessary for authorization 
    are present here, in the scope of the task at hand. 
    If necessary, this class can be extended.
 */
public class UserModel
{
    public UserModel(string? username, int id, string? password, string? token)
    {
        Username = username;
        Id = id;
        Password = password;
        Token = token;
    }

    public int Id { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Token { get; set; }
}