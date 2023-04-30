﻿using System.Diagnostics;
using Azure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WatchParty.Areas.Identity.Data;
using WatchParty.DAL.Abstract;
using WatchParty.Models;

namespace WatchParty.Controllers;

public class UserController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IWatcherRepository _watcherRepository;
    private readonly IFollowingListRepository _followingListRepository;
    private readonly WatchPartyDbContext _context;

    public UserController(ILogger<HomeController> logger, IWatcherRepository watcherRepsoitory, UserManager<IdentityUser> userManager, IFollowingListRepository followingListRepository, WatchPartyDbContext context)
    {
        _logger = logger;
        _userManager = userManager;
        _watcherRepository = watcherRepsoitory;
        _followingListRepository = followingListRepository;
        _context = context;
    }

    // GET: user/ {username}
    [Authorize]
    public async Task<ActionResult<Watcher>> Profile(string username)
    {
        if (_watcherRepository == null)
        {
            return View("Notfound");
        }
        ProfileVM vm = new ProfileVM();

        Watcher? loggedInUser = _watcherRepository.FindByUsername(User.Identity.Name);

        Watcher? watcher = _watcherRepository.FindByUsername(username);
        List<FollowingList> followingList = _followingListRepository.GetFollowingList(watcher.Id);
        List<FollowingList> followerList = _followingListRepository.GetFollowerList(watcher.Id);

        vm.Watcher = watcher;
        vm.Following = followingList;
        vm.Followers = followerList;

        vm.isFollowing = watcher.Id != loggedInUser?.Id ? _followingListRepository.IsFollowing(loggedInUser.Id, watcher.Id) : null;

        var currentUser = await _userManager.GetUserAsync(User);
        vm.isCurrentUser = _watcherRepository.IsCurrentUser(username, currentUser);


        if (watcher == null)
        {
            return View("Notfound");
        }

        return View(vm);
    }

    public IActionResult Notfound()
    {
        return View();
    }

    public async Task<IActionResult> Edit(string? username)
    {
        if (username == null || _context.Watchers == null)
        {
            return NotFound();
        }

        ProfileVM vm = new ProfileVM();

        Watcher watcher = _watcherRepository.FindByUsername(username);
        vm.Watcher = watcher;

        var currentUser = await _userManager.GetUserAsync(User);
        vm.isCurrentUser = _watcherRepository.IsCurrentUser(username, currentUser);


        if (watcher == null)
        {
            return NotFound();
        }

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProfileAsync([Bind("Id,AspNetIdentityId,Username,FirstName,LastName,Email,FollowingCount,FollowerCount,Bio,WatchListPrivacy")] Watcher watcher)
    {
        ModelState.ClearValidationState("watcher.AspNetIdentityId");
        ModelState.ClearValidationState("watcher.Id");
        //if (ModelState.IsValid)
        //{
        //}
        _watcherRepository.AddOrUpdate(watcher);

        Watcher? loggedInUser = _watcherRepository.FindByUsername(User.Identity.Name);

        ProfileVM vm = new ProfileVM();
        vm.Watcher = watcher;

        var currentUser = await _userManager.GetUserAsync(User);
        vm.isCurrentUser = _watcherRepository.IsCurrentUser(watcher.Username, currentUser);
        vm.Followers = _followingListRepository.GetFollowerList(watcher.Id);
        vm.Following = _followingListRepository.GetFollowingList(watcher.Id);
        vm.isFollowing = watcher.Id != loggedInUser?.Id ? _followingListRepository.IsFollowing(loggedInUser.Id, watcher.Id) : null;

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> SwitchTheme(string theme)
    {
        try
        {
            await Task.Delay(1000); // simulate changing theme preference in db
            Console.WriteLine("Theme Switched" + theme);
            return Ok(new { theme });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error switching theme: " + ex.Message);
            return StatusCode(500, "An error occurred while switching theme.");
        }
    }

    private bool WatcherExists(int id)
    {
        return (_context.Watchers?.Any(e => e.Id == id)).GetValueOrDefault();
    }

}
