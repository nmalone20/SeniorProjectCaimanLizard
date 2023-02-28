using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WatchParty.DAL.Abstract;
using WatchParty.Models;

namespace WatchParty.Controllers;

[Authorize]
public class PostController : Controller
{
    private readonly IRepository<Post> _postRepository;

    public PostController(IRepository<Post> postRepository)
    {
        _postRepository = postRepository;
    }

    // GET: Post
    public IActionResult Index()
    {
        var allPosts = _postRepository.GetAll()
            .OrderByDescending(p => p.DatePosted)
            .ToList();

        return View(allPosts);
    }
    /*
    // GET: Post/Create
    public IActionResult Create()
    {
        ViewData["UserId"] = new SelectList(_postRepository.Watchers, "Id", "Id");
        return View();
    }

    // POST: Post/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,PostTitle,PostDescription,DatePosted,UserId")] Post post)
    {
        if (ModelState.IsValid)
        {
            _postRepository.Add(post);
            await _postRepository.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["UserId"] = new SelectList(_postRepository.Watchers, "Id", "Id", post.UserId);
        return View(post);
    }

    // GET: Post/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _postRepository.Posts == null)
        {
            return NotFound();
        }

        var post = await _postRepository.Posts.FindAsync(id);
        if (post == null)
        {
            return NotFound();
        }
        ViewData["UserId"] = new SelectList(_postRepository.Watchers, "Id", "Id", post.UserId);
        return View(post);
    }

    // POST: Post/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,PostTitle,PostDescription,DatePosted,UserId")] Post post)
    {
        if (id != post.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _postRepository.Update(post);
                await _postRepository.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(post.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        ViewData["UserId"] = new SelectList(_postRepository.Watchers, "Id", "Id", post.UserId);
        return View(post);
    }

    // GET: Post/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _postRepository.Posts == null)
        {
            return NotFound();
        }

        var post = await _postRepository.Posts
            .Include(p => p.User)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (post == null)
        {
            return NotFound();
        }

        return View(post);
    }

    // POST: Post/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_postRepository.Posts == null)
        {
            return Problem("Entity set 'WatchPartyDbContext.Posts'  is null.");
        }
        var post = await _postRepository.Posts.FindAsync(id);
        if (post != null)
        {
            _postRepository.Posts.Remove(post);
        }
            
        await _postRepository.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool PostExists(int id)
    {
        return (_postRepository.Posts?.Any(e => e.Id == id)).GetValueOrDefault();
    }*/
}
