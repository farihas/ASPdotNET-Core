using System.ComponentModel.DataAnnotations;

namespace DojodachiApp
{
    public class Dachi
    {
        // Your Dojodachi should start with 20 happiness, 20 fullness, 50 energy, and 3 meals.
        public const int DefaultFullness = 20;
        public const int DefaultHappiness = 20;
        public const int DefaultEnergy = 50;
        public const int DefaultMeals = 3;

        public int? Fullness { get; set; }
        public int? Happiness { get; set; }
        public int? Meals { get; set; }
        public int? Energy { get; set; }    

        // Constructor for Dojodachi
        public Dachi()
        {
            this.Fullness = DefaultFullness;
            this.Happiness = DefaultHappiness;
            this.Energy = DefaultEnergy;
            this.Meals = DefaultMeals;
        }      
    }
}