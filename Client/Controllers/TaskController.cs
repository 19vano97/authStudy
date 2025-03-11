using System;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Client.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers;

public class TaskController : Controller
{
    public async Task<IActionResult> IndexAsync()
    {
        using (HttpClient client = new HttpClient())
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var response = await client.GetAsync("https://localhost:7099/api/TaskItem/all");

            if (!response.IsSuccessStatusCode)
                return View("Error", $"API request failed: {response.StatusCode}");

            var responseContent = await response.Content.ReadAsStringAsync();
            var tasks = JsonSerializer.Deserialize<List<TaskItem>>(responseContent, new JsonSerializerOptions {IncludeFields = true, PropertyNameCaseInsensitive = true});

            return View(tasks);
        }
    }
}
