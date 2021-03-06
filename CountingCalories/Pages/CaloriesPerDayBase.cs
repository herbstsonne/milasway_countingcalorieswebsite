﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using CountingCalories.Models;
using CountingCalories.Services;


namespace CountingCalories.Pages
{
    public class CaloriesPerDayBase : ComponentBase
    {
        public List<Food> AllFoodItems { get; set; }
        public string Name { get; set; }
        public FoodEntry FoodEntry { get; set; }
        public string CurrentDate { get; set; }
        public FoodInDay FoodToday { get; set; }

        [Inject]
        public CountCalorieService _CalorieService { get; set; }

        [Inject]
        public FoodService _FoodService { get; set; }

        protected override void OnInitialized()
        {
            AllFoodItems = _FoodService.GetAllFood();
            if(AllFoodItems.Any())
                Name = AllFoodItems.ElementAt(0)?.Name;
            FoodToday = _CalorieService.GetFoodOfDay(DateTime.Now) ??
                        new FoodInDay()
                        {
                            Day = DateTime.Now,
                            TotalCalories = new List<FoodEntry>()
                        };
            FoodEntry = new FoodEntry() { Amount = 0, Food = new Food() };
            CurrentDate = DateTime.Now.ToShortDateString();

            base.OnInitialized();
        }

        public void AddFoodEntry()
        {
            FoodEntry.Food = AllFoodItems.FirstOrDefault(f => f.Name.Equals(Name));
            FoodEntry.Calories = CalculateCalories(FoodEntry);
            FoodToday.TotalCalories.Add(FoodEntry);
            FoodEntry = new FoodEntry() { Amount = 0, Food = new Food() };

            StateHasChanged();
        }

        public int SumUpCaloriesOfToday()
        {
            var sum = 0;
            foreach (var e in FoodToday.TotalCalories)
            {
                sum += e.Calories;
            }
            return sum;
        }

        private int CalculateCalories(FoodEntry foodEntry)
        {
            var relative = foodEntry.Amount / 100.0f;
            return (int)(relative * (foodEntry.Food?.CaloriesPer100g ?? 0.0f));
        }
    }
}
