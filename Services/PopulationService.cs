namespace DyslexicDecode.Services;

using System;
using System.Net.Http;
using System.Threading.Tasks;

public class PopulationResult
{
    public double NinePercent { get; set; }
    public double TwelvePercent { get; set; }
}

public class PopulationService
{
    private readonly HttpClient _httpClient;

    public PopulationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PopulationResult> GetPopulationPercentage(string country)
    {
        var formattedDate = DateTime.Today.ToString("yyyy-MM-dd");
        var apiUrl = $"https://d6wn6bmjj722w.population.io:443/1.0/population/{country}/{formattedDate}/";

        var response = await _httpClient.GetAsync(apiUrl);

        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();

            // Find the index of the key "population" in the string
            var populationIndex = jsonString.IndexOf("\"population\":");

            populationIndex += "\"population\":".Length;

            // Find the end of the population number
            var endOfNumber = jsonString.IndexOfAny(new[] { ',', '}' }, populationIndex);

            // Access the population value
            var populationString = jsonString.Substring(populationIndex, endOfNumber - populationIndex);

            if (int.TryParse(populationString, out int population))
            {
                // Calculate 9% and 12% of the population
                var ninePercent = population * 0.09;
                var twelvePercent = population * 0.12;

                // Return the result
                return new PopulationResult
                {
                    NinePercent = ninePercent,
                    TwelvePercent = twelvePercent
                };
            }
            else
            {
                return new PopulationResult(); // Return empty result if parsing fails
            }
        }
        else
        {
            throw new HttpRequestException($"Failed to retrieve data. Status code: {response.StatusCode}");
        }
    }
}
