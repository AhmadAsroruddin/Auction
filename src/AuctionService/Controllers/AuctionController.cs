﻿using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionControllers : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly AuctionDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public AuctionControllers(AuctionDbContext context, IMapper mapper, IPublishEndpoint publishEndpoint){
        _context = context;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>>GetAllAuctions(string date){
        var query = _context.Auctions.OrderBy(x => x.Item.Make).AsQueryable();

        if(!string.IsNullOrEmpty(date))
        {
            query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) >0);
        }

        return await query.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id){
        var auction = await _context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);
        
        if(auction == null) return NotFound();

        return _mapper.Map<AuctionDto>(auction);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto createAuctionDto)
    {
        var auction = _mapper.Map<Auction>(createAuctionDto);
        auction.Seller = User.Identity.Name;

        _context.Auctions.Add(auction);

        var newAuction = _mapper.Map<AuctionDto>(auction);

        //PUBLISH TO SERVICE BUS
        await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));

        var result = await _context.SaveChangesAsync() > 0;
        if (!result) return BadRequest("Cannot create Auction");


        return CreatedAtAction(nameof(GetAuctionById), new { auction.Id }, newAuction);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto){
        var auction = await _context.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);

        if(auction == null) return NotFound("Auction Not Found");

        //TODO : CHECK SELLER == CURRENT USER
        if(auction.Seller != User.Identity.Name) return Forbid ();
         
        auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
        auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;
       
        await _publishEndpoint.Publish(_mapper.Map<AuctionUpdated>(auction));

        var result = await _context.SaveChangesAsync() > 0;

        if(result) return Ok("Data updated successfully");

        return BadRequest("Data updated unsuccessfully");
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id){
        var auction = await _context.Auctions.FindAsync(id);

        if(auction == null) return NotFound();

        //TODO : check if user == seller
        if(auction.Seller != User.Identity.Name) return Forbid();
         
        _context.Auctions.Remove(auction);

        await _publishEndpoint.Publish<AuctionDeleted>(new {Id = auction.Id.ToString()});

        var result = await _context.SaveChangesAsync() > 0;

        if(!result) return BadRequest("Couldn't delete Auction");

        return Ok("Auction Deleted");
    }

}
