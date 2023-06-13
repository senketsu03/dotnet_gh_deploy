﻿using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Deploy.Core;
using SystemTextJsonPatch;

#pragma warning disable CA1303

namespace Deploy.Client;

public static class Program
{
    public const string IPv6ServerAddress = "https://[::1]:5230";

    public const string IPv4ServerAddress = "https://localhost:5230";

    static async Task Main()
    {
        var serverAddress = IPv4ServerAddress;
        var userId = 1;

        using var httpClient = new HttpClient
        {
            DefaultRequestVersion = HttpVersion.Version30,
            DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact
        };

        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

        #region GET

        Console.WriteLine("GET");

        var uri = new Uri($"{serverAddress}/{userId}");

        await GetAsync(httpClient, uri).ConfigureAwait(false);

        #endregion

        #region HEAD

        Console.WriteLine("HEAD");

        await HeadAsync(httpClient, uri).ConfigureAwait(false);

        #endregion

        #region POST

        Console.WriteLine("POST");

        uri = new Uri($"{serverAddress}/create");

        var postId = await PostAsync(httpClient, uri).ConfigureAwait(false);

        #endregion

        #region PATCH

        Console.WriteLine("PATCH");

        uri = new Uri($"{serverAddress}/patch/{postId}");

        await PatchAsync(httpClient, uri).ConfigureAwait(false);

        #endregion
    }

    public static async Task GetAsync(HttpClient httpClient, Uri uri)
    {
        if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));

        using var response = await httpClient.GetAsync(uri).ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            Console.WriteLine(responseText);
        }
        else
        {
            var user = await response.Content.ReadFromJsonAsync<User>().ConfigureAwait(false);

            WriteUserInfo(user!);
        }
    }

    public static async Task HeadAsync(HttpClient httpClient, Uri uri)
    {
        if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));

        using var request = new HttpRequestMessage(HttpMethod.Head, uri);

        using var response = await httpClient.SendAsync(request).ConfigureAwait(false);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Status code: {response.StatusCode}");
            Console.WriteLine($"Content type: {response.Content.Headers.ContentType}");
            Console.WriteLine($"Content length: {response.Content.Headers.ContentLength}");
            Console.WriteLine($"Date: {response.Headers.Date}");
            Console.WriteLine($"Server: {response.Headers.Server}");
        }
        else
        {
            Console.WriteLine($"Error: {response.ReasonPhrase}");
        }
    }

    public static async Task<int> PostAsync(HttpClient httpClient, Uri uri)
    {
        if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));

        var user = new User { Name = "Vladimir", Age = 99 };

        using var response = await httpClient.PostAsJsonAsync(uri, user).ConfigureAwait(false);

        var vladimirWithId = await response.Content.ReadFromJsonAsync<User>().ConfigureAwait(false);

        WriteUserInfo(vladimirWithId!);

        return vladimirWithId!.Id;
    }

    public static async Task PatchAsync(HttpClient httpClient, Uri uri)
    {
        if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));

        var patch = new JsonPatchDocument<User>();
        patch.Replace((v) => v.Name, "Vovik");
        patch.Replace((v) => v.Age, 36);

        var patchJson = JsonSerializer.Serialize(patch, options: new());
        using var content = new StringContent(patchJson, Encoding.UTF8, "application/json-patch+json");

        using var response = await httpClient.PatchAsync(uri, content).ConfigureAwait(false);

        var patchedUser = await response.Content.ReadFromJsonAsync<User>().ConfigureAwait(false);

        WriteUserInfo(patchedUser!);
    }

    private static void WriteUserInfo(User user) =>
        Console.WriteLine($"Id: {user.Id} Name: {user.Name} Age: {user.Age}");
}

#pragma warning restore CA1303
