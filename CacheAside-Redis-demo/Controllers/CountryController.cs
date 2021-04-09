using CacheAside_Redis_demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CacheAside_Redis_demo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CountryController : ControllerBase
    {
        private readonly IDistributedCache cache;
        private readonly CountryContext countryContext;

        public CountryController(IDistributedCache cache, CountryContext countryContext)
        {
            this.cache = cache;
            this.countryContext = countryContext;
        }

        [HttpGet]
        public async Task<IEnumerable<Country>> GetCountries()
        {
            var countriesCache = await cache.GetAsync("countries");
            var value = countriesCache != null ? JsonConvert.DeserializeObject<IEnumerable<Country>>(Encoding.Default.GetString(countriesCache)) : default;
            if(value == null)
            {
                var countries = countryContext.Countries.ToList();
                if (countries != null && countries.Any())
                {
                    await cache.SetStringAsync("countries", JsonConvert.SerializeObject(countries), new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });
                    return countries;
                }
            }
            return value;
        }

        [HttpPost]
        public async Task<ActionResult<string>> AddCountries([FromBody] Country country, CancellationToken cancellationToken)
        {
            if (country == null)
                return BadRequest("country is null");

            await countryContext.AddAsync(country);
            await countryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            await cache.RemoveAsync("countries", cancellationToken).ConfigureAwait(false);
            return Ok("OK!");
        }
    }
}
