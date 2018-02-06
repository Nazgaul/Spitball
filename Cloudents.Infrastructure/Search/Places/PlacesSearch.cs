﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;

namespace Cloudents.Infrastructure.Search.Places
{
    public class PlacesSearch : IPlacesSearch
    {
        private readonly IGooglePlacesSearch _placesSearch;
        private readonly IReadRepositoryAsync<IEnumerable<HookedDto>, IEnumerable<string>> _hookedRepository;
        private readonly IUrlRedirectBuilder<PlaceDto> _urlRedirectBuilder;

        public PlacesSearch(IGooglePlacesSearch placesSearch, IReadRepositoryAsync<IEnumerable<HookedDto>, IEnumerable<string>> hookedRepository, IUrlRedirectBuilder<PlaceDto> urlRedirectBuilder)
        {
            _placesSearch = placesSearch;
            _hookedRepository = hookedRepository;
            _urlRedirectBuilder = urlRedirectBuilder;
        }

        public async Task<(string token, IEnumerable<PlaceDto> data)> SearchAsync(IEnumerable<string> term, PlacesRequestFilter filter, GeoPoint location, string nextPageToken,
            CancellationToken token)
        {
            var result = await _placesSearch.SearchNearbyAsync(term, filter, location, nextPageToken, token).ConfigureAwait(false);
            var data = result.data.ToList();
            var resultHooked = await _hookedRepository.GetAsync(data.Select(s => s.PlaceId), token).ConfigureAwait(false);
            var hash = new HashSet<string>(resultHooked.Select(s => s.Id));
            foreach (var place in data)
            {
                place.Hooked = hash.Contains(place.PlaceId);
            }

            var resultBuilder = _urlRedirectBuilder.BuildUrl(data);

            return (result.token, resultBuilder);
        }
    }
}
