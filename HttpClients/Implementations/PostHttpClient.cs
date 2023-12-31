using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Domain.DTOs;
using Domain.Models;
using HttpClients.ClientInterfaces;

namespace HttpClients.Implementations;

public class PostHttpClient : IPostService
{
    private readonly HttpClient client;

    public PostHttpClient(HttpClient client)
    {
        this.client = client;
    }
    public async Task<ICollection<Post>> GetAsync(string? userName, string? titleContains, string? bodyContains)
    {
        HttpResponseMessage message = await client.GetAsync("/posts");
        string content = await message.Content.ReadAsStringAsync();
        if (!message.IsSuccessStatusCode)
        {
            throw new Exception(content);
        }
        ICollection<Post> posts = JsonSerializer.Deserialize<ICollection<Post>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
        return posts;
    }

    public async Task<ICollection<Comment>> GetCommentsForPost(int postId)
    {
        HttpResponseMessage message = await client.GetAsync($"/comments?postId={postId}");
        string content = await message.Content.ReadAsStringAsync();
        if (!message.IsSuccessStatusCode)
        {
            throw new Exception(content);
        }

        ICollection<Comment> comments = JsonSerializer.Deserialize<ICollection<Comment>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
        return comments;
    }

    public async Task CreateAsync(PostCreationDto postCreationDto)
    {
        HttpResponseMessage message = await client.PostAsJsonAsync("/posts", postCreationDto);
        if (!message.IsSuccessStatusCode)
        {
            string content = await message.Content.ReadAsStringAsync();
            throw new Exception(content);
        }
    }

    public async Task<Comment> AddCommentAsync(CommentCreationDto commentCreationDto)
    {
        HttpResponseMessage message = await client.PatchAsJsonAsync("/posts/comment", commentCreationDto);
        if (!message.IsSuccessStatusCode)
        {
            string content = await message.Content.ReadAsStringAsync();
            throw new Exception(content);
        }
        
        var responseContent = await message.Content.ReadAsStringAsync();

        Comment newlyCreatedComment = JsonSerializer.Deserialize<Comment>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;

        return newlyCreatedComment;
    }

    public async Task<int> AddUpvoteAsync(UpvoteCreationDto upvoteCreationDto)
    {
        HttpResponseMessage message = await client.PatchAsJsonAsync("/posts/up", upvoteCreationDto);
        if (!message.IsSuccessStatusCode)
        {
            string content = await message.Content.ReadAsStringAsync();
            throw new Exception(content);
        }

        var responseContent = await message.Content.ReadAsStreamAsync();
        
        int vote = JsonSerializer.Deserialize<int>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
        
        Console.WriteLine(upvoteCreationDto.Vote);
        return vote;
    }

    public async Task DeletePost(int postID)
    {
        HttpResponseMessage message = await client.DeleteAsync($"Posts/{postID}");
        if (!message.IsSuccessStatusCode)
        {
            string content = await message.Content.ReadAsStringAsync();
            throw new Exception(content);
        }
    }
}