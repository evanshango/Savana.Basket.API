using Microsoft.AspNetCore.Mvc;
using Savana.Basket.API.Dtos;
using Savana.Basket.API.Interfaces;
using Treasures.Common.Helpers;

namespace Savana.Basket.API.Controllers;

[ApiController, Route("basket"), Produces("application/json"), Tags("Basket")]
public class BasketController : ControllerBase {
    private readonly IBasketService _basketService;

    public BasketController(IBasketService basketService) => _basketService = basketService;

    [HttpGet]
    public async Task<ActionResult<BasketDto>> GetBasket() {
        var basket = await RetrieveBasket(GetBuyerId());
        return basket != null ? Ok(basket) : NotFound(new ApiException(404, "Basket not found"));
    }

    [HttpPost("")]
    public async Task<ActionResult<BasketDto>> AddBasket([FromBody] BasketDto basketDto) {
        var basket = await RetrieveBasket(GetBuyerId()) ?? PrepareBasket(basketDto);
        var update = await _basketService.AddBasket(basket);
        return update != null ? Ok(update) : BadRequest(new ApiException(400, "Unable to create basket"));
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteBasket(int productId, int quantity) {
        var basket = await RetrieveBasket(GetBuyerId());
        if (basket == null) return NotFound(new ApiException(404, "Basket not found"));
        
        var res = await _basketService.DeleteBasket(basket, productId, quantity);
        return res ? NoContent() : BadRequest(new ApiException(400, "Unable to clear basket"));
    }

    private async Task<BasketDto?> RetrieveBasket(string buyerId) {
        if (!string.IsNullOrEmpty(buyerId))
            return await _basketService.GetBasket(buyerId);
        Response.Cookies.Delete("buyerId");
        return null;
    }

    private string GetBuyerId() => (User.Identity?.Name ?? Request.Cookies["buyerId"])!;

    private BasketDto PrepareBasket(BasketDto basketDto) {
        var buyerId = User.Identity?.Name;
        if (string.IsNullOrEmpty(buyerId)) {
            buyerId = Guid.NewGuid().ToString();
            var cookieOptions = new CookieOptions { IsEssential = true, Expires = DateTime.Now.AddDays(30) };
            Response.Cookies.Append("buyerId", buyerId, cookieOptions);
        }

        basketDto.BuyerId = buyerId;
        return basketDto;
    }
}