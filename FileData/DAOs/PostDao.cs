using Application.DaoInterfaces;
using Domain.Models;
using FileData;

namespace DefaultNamespace.DAOs;

public class PostDao : IPostDao
{
    private readonly FileContext fileContext;

    public PostDao(FileContext fileContext)
    {
        this.fileContext = fileContext;
    }

    public Task<Post> CreateAsync(Post post)
    {
        int id = 1;
        if (fileContext.Posts.Any())
        {
            id = fileContext.Posts.Max(t => t.Id);
            id++;
        }
        post.Id = id;
        fileContext.Posts.Add(post);
        fileContext.SaveChanges();
        return Task.FromResult(post);
    }

    public async Task<IEnumerable<Post>> GetAsync()
    {
        IEnumerable<Post> posts = fileContext.Posts.AsEnumerable();
        return posts;
    }

    public Task<Comment> AddCommentAsync(Comment comment, Post post)
    {
        post.Comments.Add(comment);
        Post existing = fileContext.Posts.FirstOrDefault(p => post.Id == p.Id)!;
        fileContext.Posts.Remove(existing);
        fileContext.Posts.Add(post);
        fileContext.SaveChanges();
        return Task.FromResult(comment);
    }
}