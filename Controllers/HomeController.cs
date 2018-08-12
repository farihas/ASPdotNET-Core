using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DojodachiApp.Models;

namespace DojodachiApp.Controllers
{
    public enum GameState{
        Playing = 0,
        Win,
        Lose
    }
    
    public class HomeController : Controller
    {
        private static Random _rand = new Random();

        public Dachi SessionDachi
        {
            get
            {
                // Lookup dachi instance in the session
                Dachi dachi = HttpContext.Session.GetObjectFromJson<Dachi>("dachi");

                // Check if instance exists or needs to be created
                if (dachi == null)
                {
                    // Create and store new instance in session and return new instance to caller.
                    dachi = new Dachi();
                    HttpContext.Session.SetObjectAsJson("dachi", dachi);
                }
                return dachi;
            }
            set
            {
                HttpContext.Session.SetObjectAsJson("dachi", value);
            }
        }

        public IActionResult Index()
        {
            Dachi dachi = this.SessionDachi;
            GameState state = GetGameStatus();

            if (state == GameState.Lose)
            {
                ViewBag.Message = ":( This is so sad. Your Dojodachi is no more. You Lose!";
            }

            else if (state == GameState.Win)
            {
                ViewBag.Message = "Congratulations! You WON!";
            } 

            ViewBag.State = state;

            return View(dachi);
        }

        [HttpGet("feed")]
        public IActionResult Feed()
        {
            // Every time you feed your dojodachi there's a 25% chance that it won't like it. 
            // Meals will still decrease, but happiness won't change.            
            
            Dachi dachi = this.SessionDachi;     // Get Dachi instance from session
            dachi.Fullness += _rand.Next(5, 11); // Gains random fullness between 5 and 10 

            int luck = _rand.Next(0, 4);
            if (luck > 0)
            {
                if(dachi.Meals > 0)
                {
                    dachi.Meals--;            // Feeding your Dojodachi costs 1 meal
                    TempData["Message"] = "You fed your Dojodachi. Your Dojodachi is fuller!";
                }
                else if(dachi.Meals <= 0)     // you cannot feed your Dojodachi if you do not have meals
                {
                    TempData["Message"] = "!! You cannot feed your Dojodachi! There are no more meals!!";
                }                
            }
            this.SessionDachi = dachi;    // Store modified instance back in session            
            
            return RedirectToAction("Index");
        }

        [HttpGet("play")]
        public IActionResult Play()
        {
            // Every time you play with your dojodachi there's a 25% chance that it won't like it. 
            // Energy will still decrease, but happiness won't change.

            Dachi dachi = this.SessionDachi;      // Get Dachi instance from session
            dachi.Energy -= 5;   // Costs 5 energy 
            
            int luck = _rand.Next(0, 4); 
            if (luck > 0)       // there's a 25% chance that it won't like it and happiness won't change
            {
                dachi.Happiness += _rand.Next(5, 11); // Gains random happiness between 5 and 10
            }
            this.SessionDachi = dachi;            // Store modified instance back in session
            TempData["Message"] = "You played with your Dojodachi. Your Dojodachi is happier!";
            
            return RedirectToAction("Index");
        }

        [HttpGet("work")]
        public IActionResult Work()
        {
            Dachi dachi = this.SessionDachi;   // Get Dachi instance from session 
            dachi.Energy -= 5;                 // Working costs 5 energy
            dachi.Meals += _rand.Next(1, 4);   // Earns between 1 and 3 meals
            this.SessionDachi = dachi;         // Store modified instance back in session
            TempData["Message"] = "Your Dojodachi worked hard and needs some rest!";
            
            return RedirectToAction("Index");
        }

        [HttpGet("sleep")]
        public IActionResult Sleep()
        {
            GameState state = GetGameStatus();
            Dachi dachi = this.SessionDachi; // Get Dachi instance from session
            dachi.Energy += 15;              // sleeping earns 15 energy 
            dachi.Fullness -= 5;             // decreases fullness by 5
            dachi.Happiness -= 5;            // decreases happiness by 5
            this.SessionDachi = dachi;       // Store modified instance back in session
            TempData["Message"] = "Your Dojodachi caught some zzs and is well rested!";                
            
            return RedirectToAction("Index");
        }

        public GameState GetGameStatus()
        {
            Dachi dachi = this.SessionDachi;

            // If fullness or happiness ever drop to 0, you lose, 
            // and a restart button should be displayed.
            if (dachi.Fullness <= 0 || dachi.Happiness <= 0)
            {
                return GameState.Lose;                                
            }

            // If energy, fullness, and happiness are all raised to over 100, you win! 
            // a restart button should be displayed.
            else if (dachi.Energy > 100 && dachi.Fullness > 100 && dachi.Happiness > 100)
            {
                return GameState.Win;
            }

            return GameState.Playing;
        }

        [HttpGet("new")]
        public IActionResult NewGame()
        {
            Console.WriteLine(TempData["Variable"]);
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}


