using IdentityModel.Client;
using Movies.Client.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Movies.Client.ApiServices
{
    public class MovieApiService : IMovieApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public MovieApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ??
                throw new ArgumentNullException(nameof(httpClientFactory));
        }
        public Task<Movie> CreateMovie(Movie movie)
        {
            throw new NotImplementedException();
        }

        public Task DeleteMovie(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Movie> GetMovie(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Movie>> GetMovies()
        {
            //WAY 1
            var httpClient = _httpClientFactory.CreateClient("MovieAPIClient");

            var request = new HttpRequestMessage(HttpMethod.Get, "/api/movies/");

            var response = await httpClient.SendAsync(request,
                HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var movieList = JsonConvert.DeserializeObject<List<Movie>>(content);
            return movieList;

            ////WAY 2

            ////1 - Get token from Identity Server, of course we should provide the IS configuration like address, client id and client secret.
            ////2 - Send request to the protected api
            ////3 - Deserialized the object to MoviesList

            ////1 - Get token from Identity Server, of course we should provide the IS configuration like address, client id and client secret.

            ////1. Retrieve our api credentials. This must be registered on Identity Server.
            //var apiClientCredentials = new ClientCredentialsTokenRequest
            //{
            //    Address = "https://localhost:5005/connect/token",

            //    ClientId = "movieClient",
            //    ClientSecret = "secret",

            //    //This is the scope of our Protected API requires
            //    Scope = "movieAPI"
            //};

            ////Creates a new HTTP Client to talk to our Identity Server (localhost:5005)
            //var client = new HttpClient();

            ////Just check if we can reach the Discovery Document.Not 100% needed but...
            //var discovery = await client.GetDiscoveryDocumentAsync("https://localhost:5005");

            //if (discovery.IsError)
            //{
            //    return null; //throw 500 error.
            //}

            ////2. Authenticates and get an access token from Identity Server.
            //var tokenResponse = await client.RequestClientCredentialsTokenAsync(apiClientCredentials);
            //if (tokenResponse.IsError)
            //{
            //    return null;
            //}

            ////2 - Send request to the protected api

            ////Another HTTPClient for talking now with our Protected API
            //var apiClient = new HttpClient();

            ////3. Set the access token in the request Authoriation: Bearer <token>
            //apiClient.SetBearerToken(tokenResponse.AccessToken);

            ////4. Send a request to our protected API
            //var response = await apiClient.GetAsync("https://localhost:5001/api/movies");
            //response.EnsureSuccessStatusCode();

            //var content = await response.Content.ReadAsStringAsync();

            ////Deserialize object to movie list
            //List<Movie> movieList = JsonConvert.DeserializeObject<List<Movie>>(content);
            //return movieList;            
        }

        public Task<Movie> UpdateMovie(Movie movie)
        {
            throw new NotImplementedException();
        }
    }
}
