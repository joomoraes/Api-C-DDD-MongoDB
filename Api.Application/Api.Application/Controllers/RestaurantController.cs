﻿
namespace Api.Application.Controllers
{
    using Api.Application.Controllers.Inputs;
    using Api.Application.Controllers.Outputs;
    using Api.Application.Data.Repositories;
    using Api.Application.Domain.Entities;
    using Api.Application.Domain.Enums;
    using Api.Application.Domain.ValueObjects;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;
    using System.Threading.Tasks;

    [ApiController]
    public class RestaurantController : Controller
    {

        private readonly RestaurantRepository _restaurantRepository;

        public RestaurantController(RestaurantRepository restaurantRepository)
        {
            _restaurantRepository = restaurantRepository;
        }

        [HttpPost("restaurant")]
        public IActionResult IncludeRestaurant([FromBody] IncluseRestaurant incluseRestaurant)
        {
            var kitchen = EKitchenHelper.ParseInt(incluseRestaurant.Kitchen);

            var restaurant = new Restaurant(
                incluseRestaurant.Name,
                kitchen);

            var address = new Address(
                incluseRestaurant.PublicReplace,
                incluseRestaurant.Number,
                incluseRestaurant.City,
                incluseRestaurant.State,
                incluseRestaurant.ZipCode);

            restaurant.AtributeAddress(address);

            if(!restaurant._Validate())
            {
                return BadRequest(
                    new
                    {
                        errors = restaurant.ValidationResult.Errors.Select(_ => _.ErrorMessage)
                    });
            }

            _restaurantRepository.Insert(restaurant);

            return Ok(
                    new
                    {
                        data = "Successful! Insert new restaurant"
                    }
                );
        }

        [HttpGet("restaurant/all")]
        public async Task<ActionResult> GetAllRestaurants()
        {
            var restaurants = await _restaurantRepository.GetAll();

            var list = restaurants.Select(_ => new RestaurantList
            {
                Id = _.Id,
                Name = _.Name,
                Kitchen = (int)_.Kitchen,
                City = _.Address.City
            });

            return Ok(
                new
                {
                    data = list
                });
        }
    }
}