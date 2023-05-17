﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WatchParty.DAL.Abstract;
using WatchParty.Models;
using WatchParty.ViewModels;

namespace WatchParty.Controllers;

[Authorize]
public class WatchPartyGroupController : Controller
{
    private readonly IWatchPartyGroupRepository _groupRepository;
    private readonly IWatchPartyGroupAssignmentRepository _assignmentRepository;
    private readonly IWatcherRepository _watcherRepository;

    public WatchPartyGroupController(IWatchPartyGroupRepository groupRepository, IWatchPartyGroupAssignmentRepository assignmentRepository, IWatcherRepository watcherRepository)
    {
        _groupRepository = groupRepository;
        _assignmentRepository = assignmentRepository;
        _watcherRepository = watcherRepository;
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create([Bind("GroupTitle, GroupDescription, StartDate")] WatchPartyGroup newGroup)
    {
        if (newGroup.StartDate.Equals(new DateTime(1, 1, 1, 0, 0, 0)))
            newGroup.StartDate = new DateTime(DateTime.Now.AddDays(7).Year, DateTime.Now.AddDays(7).Month, DateTime.Now.AddDays(7).Day, 20, 0, 0);

        ModelState.Clear();

        Watcher? watcher = _watcherRepository.FindByUsername(User?.Identity?.Name);

        if (watcher == null)
            throw new ArgumentException(nameof(watcher));

        newGroup.HostId = watcher.Id;

        TryValidateModel(newGroup);

        if (ModelState.IsValid)
        {
            _groupRepository.CreateWatchPartyGroup(newGroup);

            int currentGroupIndex = _groupRepository.GetAll().Count();
            WatchPartyGroup? group = _groupRepository.GetById(currentGroupIndex);

            if (group == null)
                throw new NullReferenceException(nameof(group));

            _assignmentRepository.AddToGroup(new WatchPartyGroupAssignment{ Group = group, GroupId = group.Id, Watcher = group.Host, WatcherId = group.HostId });
            return RedirectToAction("Profile", "User", new { username = User.Identity.Name });
        }

        return View();
    }

    [HttpGet]
    public IActionResult Details(int groupId)
    {
        WatchPartyGroup group = _groupRepository.FindById(groupId);
        List<Watcher>? watchers = _watcherRepository.FindAllWatchers();
        bool userInGroup = _assignmentRepository.UserInGroup(groupId, User.Identity.Name);
        bool hasOccurred = group.StartDate <= DateTime.Now;

        PartyGroupVM vm = new()
        {
            Group = group,
            Watchers = watchers,
            UserInGroup = userInGroup,
            HasOccurred = hasOccurred
        };

        return View(vm);
    }

    [HttpGet]
    public IActionResult Edit(int groupId)
    {
        WatchPartyGroup? group = _groupRepository.GetById(groupId);
        ViewBag.IsVisible = group?.Host.Username == User?.Identity?.Name;
        return View(group);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit([Bind("Id, GroupTitle, GroupDescription, StartDate, TelePartyUrl, HostId")] WatchPartyGroup updatedGroup)
    {
        ViewBag.IsVisible = true;

        ModelState.Clear();
        TryValidateModel(updatedGroup);

        if (!ModelState.IsValid) return View(updatedGroup);

        _groupRepository.UpdateGroup(updatedGroup);
        return RedirectToAction("Details", new { groupId = updatedGroup.Id });
    }
}

