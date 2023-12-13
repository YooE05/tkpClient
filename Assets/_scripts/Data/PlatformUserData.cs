using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class PlatformUserData
{
    public string id;
    public string name;
    public string surname;
    public string userClass;
    public Dictionary<string, bool> games;

    public PlatformUserData(string id, string name, string surname, string userClass, Dictionary<string, bool> games)
    {
        this.id = id;
        this.name = name;
        this.surname = surname;
        this.userClass = userClass;
        this.games = games;
    }
}

