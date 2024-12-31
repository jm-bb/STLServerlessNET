namespace STLServerlessNET.Models;

public class CountryConst
{
    private int _id;
    private string _abbreviation;
    private string _name;

    public CountryConst() { }

    public int ID
    {
        get { return _id; }
        set { _id = value; }
    }

    public string Abberviation
    {
        get { return _abbreviation; }
        set { _abbreviation = value; }
    }

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }
}

public class FbiState
{
    private string _ticket;
    private List<StateConst> _states;
    private string _response;

    public string Ticket
    {
        get { return _ticket; }
        set { _ticket = value; }
    }

    public List<StateConst> States
    {
        get { return _states; }
        set { _states = value; }
    }

    public string Response
    {
        get { return _response; }
        set { _response = value; }
    }
}

public class FbiCountry
{
    private string _ticket;
    private List<CountryConst> _countries;
    private string _response;

    public string Ticket
    {
        get { return _ticket; }
        set { _ticket = value; }
    }

    public List<CountryConst> Countries
    {
        get { return _countries; }
        set { _countries = value; }
    }

    public string Response
    {
        get { return _response; }
        set { _response = value; }
    }
}

public class StateConst
{
    private int _id;
    private string _code;
    private int _countryConstID;
    private string _name;

    public StateConst() { }

    public int ID
    {
        get { return _id; }
        set { _id = value; }
    }

    public string Code
    {
        get { return _code; }
        set { _code = value; }
    }

    public int CountryConstID
    {
        get { return _countryConstID; }
        set { _countryConstID = value; }
    }

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }
}

public class StatePackage
{
    private StateConst _state;
    private string _response;

    public StatePackage() { }

    public StateConst State
    {
        get { return _state; }
        set { _state = value; }
    }

    public string Response
    {
        get { return _response; }
        set { _response = value; }
    }
}