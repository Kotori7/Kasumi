using System.Linq;
using Kasumi.Entities;

namespace Kasumi.Economy
{
    public class Happiness
    {
        public static int GetHappiness(ulong id)
        {
            using(var db = new HappinessContext())
            {
                return db.Users.Single(b => b.Id == id.ToString()).Happiness;
            }
        }
        public static HappinessLevel GetHappinessLevel(ulong id)
        {
            using (var db = new HappinessContext())
            {
                return db.Users.Single(b => b.Id == id.ToString()).Level;
            }
        }
        public static void IncrementHappiness(ulong id, int amount)
        {
            using (var db = new HappinessContext())
            {
                db.Users.Single(b => b.Id == id.ToString()).Happiness += amount;
                db.SaveChanges();
            }
        }
        public static void DecrementHappiness(ulong id, int amount)
        {
            using (var db = new HappinessContext())
            {
                db.Users.Single(b => b.Id == id.ToString()).Happiness -= amount;
                db.SaveChanges();
            }
        }
    }
}
