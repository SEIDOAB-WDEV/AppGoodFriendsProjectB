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
            //in project case via Input Model
            IPet pet = new csPet().Seed(_seeder);
            pet.Friend = Friend;


            //Create the Pet in the database
            //in project case via New button submit
            var dtoPet = new csPetCUdto(pet);
            dtoPet.PetId = null;
            pet = await _service.CreatePetAsync(null, dtoPet);

            //Update the Friend by dto
            //in project case via New button submit
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

        public async Task<IActionResult> OnPostEditFriend()
        {
            //Reload Friend from the database
            Friend = await _service.ReadFriendAsync(null, FriendId, false);

            //Edit Friend
            //in project case via Edit button submit and find Friend by Id
            Friend.FirstName = "Happy";
            Friend.LastName = "Joy";

            //Update the Friend by dto
            var dtoFriend = new csFriendCUdto(Friend);
            Friend = await _service.UpdateFriendAsync(null, dtoFriend);

            return Page();
        }

        public async Task<IActionResult> OnPostEditPet()
        {
            //Reload Friend from the database
            Friend = await _service.ReadFriendAsync(null, FriendId, false);

            //Find first Pet
            //in project case via Edit button submit and find Pet by Id
            var pet = Friend.Pets.First();

            //Change name
            pet.Name = "Wanda";
            pet.Kind = enAnimalKind.Fish;
            pet.Mood = enAnimalMood.Happy;

            //Update the Pet in the database
            //in project case via Edit button submit and find Pet by Id
            var dtoPet = new csPetCUdto(pet);
            pet = await _service.UpdatePetAsync(null, dtoPet);

            //Reload Friend from the database
            Friend = await _service.ReadFriendAsync(null, FriendId, false);

            return Page();
        }

        public async Task<IActionResult> OnPostEditQuote()
        {
            //Reload Friend from the database
            Friend = await _service.ReadFriendAsync(null, FriendId, false);

            //Find first Quote
            //in project case via Edit button submit and find quote by Id
            var quoute = Friend.Quotes.First();

            //Change name
            quoute.Quote = "Wanda Wanda";
            quoute.Author = "Simba";

            //Update the Quote in the database
            //in project case via Edit button submit and find Quote by Id
            var dtoQuote = new csQuoteCUdto(quoute);
            quoute = await _service.UpdateQuoteAsync(null, dtoQuote);

            //Reload Friend from the database
            Friend = await _service.ReadFriendAsync(null, FriendId, false);

            return Page();
        }

        public async Task<IActionResult> OnPostDeletePet()
        {
            //Reload Friend from the database
            Friend = await _service.ReadFriendAsync(null, FriendId, false);

            //Find last Pet
            //in project case via delete button submit and find Pet by Id
            var pet = Friend.Pets.Last();

            //Delete the Pet in the database
            //in project case via Delete button submit and find Pet by Id
            pet = await _service.DeletePetAsync(null, pet.PetId);

            //Reload Friend from the database
            Friend = await _service.ReadFriendAsync(null, FriendId, false);

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteQuote()
        {
            //Reload Friend from the database
            Friend = await _service.ReadFriendAsync(null, FriendId, false);

            //Find last Quote
            //in project case via delete button submit and find Quote by Id
            var quote = Friend.Quotes.Last();

            //Update the Friend by dto
            var dtoFriend = new csFriendCUdto(Friend);
            dtoFriend.QuotesId.Remove(quote.QuoteId);

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
