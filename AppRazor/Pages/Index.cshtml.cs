using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using Models.DTO;

namespace AppRazor.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private IFriendsService _service;

    public List<csFriendsByCountry> friendsByCountry = new List<csFriendsByCountry>();


    public IndexModel(ILogger<IndexModel> logger, IFriendsService service)
    {
        _logger = logger;
        _service = service;
    }

    public async Task OnGet()
    {
        var info = await _service.InfoAsync;
        var friends = info.Friends;
        var pets = info.Pets;

        var friendsbycoutry = friends.GroupBy(f => f.Country);
        foreach (var item in friendsbycoutry)
        {
                var f = new csFriendsByCountry();
                f.Country = item.Key;
                f.Cities = item.Where(f => f.City != null).ToList();

                friendsByCountry.Add(f);
        }


        var list = friends.Join(pets, f => f.City, p => p.City,
            (f, p) => new csFriendPetByCity() { City = f.City, NrFriends = f.NrFriends, NrPets = p.NrPets });
    }


    public class csFriendsByCountry
    {
        public string Country { get; set; }
        public List<gstusrInfoFriendsDto> Cities { get; set; }
    }

    public class csFriendPetByCity
    {
        public string City { get; set; }
        public int NrPets { get; set; }
        public int NrFriends { get; set; }
    }
}

