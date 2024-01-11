using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.DTO;
using Services;

namespace AppRazor.Pages
{
    public class ServiceAccessModel : PageModel
    {
        private readonly ILogger<ServiceAccessModel> _logger;
        private IFriendsService _service;
        private csSeedGenerator _seeder = new csSeedGenerator();


        public IFriend Friend { get; set; } = null;

        [BindProperty]
        public Guid FriendId { get; set; }

        public async Task<IActionResult> OnGet()
        {
            await _service.RemoveSeedAsync(null, true);
            await _service.RemoveSeedAsync(null, false);

            return Page();
        }
        public async Task<IActionResult> OnPostNewFriend()
        {
            Friend = new csFriend().Seed(_seeder);

            var dtoFriend = new csFriendCUdto(Friend);
            dtoFriend.FriendId = null;
            var FriendFromDb = await _service.CreateFriendAsync(null, dtoFriend);
            FriendId = FriendFromDb.FriendId;

            return Page();
        }

        public async Task<IActionResult> OnPostAddPet()
        {
            //Reload Friend from the database
            Friend = await _service.ReadFriendAsync(null, FriendId, false);

            //Create a new Pet and set an owner
            IPet pet = new csPet().Seed(_seeder);
            pet.Friend = Friend;

            //Create the Pet in the database
            var dtoPet = new csPetCUdto(pet);
            dtoPet.PetId = null;
            pet = await _service.CreatePetAsync(null, dtoPet);

            //Update the Friend by dto
            var dtoFriend = new csFriendCUdto(Friend);
            dtoFriend.PetsId.Add(pet.PetId);
            Friend = await _service.UpdateFriendAsync(null, dtoFriend);

            return Page();
        }

        public async Task<IActionResult> OnPostAddQuote()
        {
            //Reload Friend from Database
            Friend = await _service.ReadFriendAsync(null, FriendId, false);

            //Create a new Quote
            IQuote quote = new csQuote() { Quote = $"{_seeder.LatinSentence}", Author = $"{_seeder.PetName}", Seeded = true};

            //Create the quote in the database
            var dtoQuote = new csQuoteCUdto(quote);
            dtoQuote.QuoteId = null;
            quote = await _service.CreateQuoteAsync(null, dtoQuote);

            //Update the Friend by dto
            var dtoFriend = new csFriendCUdto(Friend);
            dtoFriend.QuotesId.Add(quote.QuoteId);
            Friend = await _service.UpdateFriendAsync(null, dtoFriend);

            return Page();
        }

        public ServiceAccessModel(ILogger<ServiceAccessModel> logger, IFriendsService service)
        {
            _logger = logger;
            _service = service;
        }
    }
}
