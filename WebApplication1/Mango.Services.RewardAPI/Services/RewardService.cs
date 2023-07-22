﻿using Mango.Services.RewardAPI.Data;
using Mango.Services.RewardAPI.Message;
using Mango.Services.RewardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.RewardAPI.Services
{
    public class RewardService : IRewardService
    {
        private DbContextOptions<AppDbContext> _dpOptions;
        private readonly AppDbContext _db;

        public RewardService(DbContextOptions<AppDbContext> dbOptions)
        {
            _dpOptions = dbOptions;
        }

        public async Task UpdateRewards(RewardsMessage rewardsMessage)
        {
            try
            {
                Rewards rewards = new()
                {
                    OrderId = rewardsMessage.OrderId,
                    RewardsActivity = rewardsMessage.RewardsActivity,
                    UserId = rewardsMessage.UserId,
                    RewardsDate = DateTime.Now,
                };

                await using var _db = new AppDbContext(_dpOptions);
                await _db.Rewards.AddAsync(rewards);
                await _db.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                
            }
        }
    }
}
