using System;
using Engine.Resources;

namespace Engine.Utilities
{
    public class GreetingBuilder
    {
        public static string GetCurrentGreetingFor(string name)
        {
            DateTime now = DateTime.Now;

            if(now.Hour >= 4 && now.Hour <= 11)
            {
                return string.Format(Literals.Greeting_Morning, name);
            }

            if(now.Hour >= 12 && now.Hour < 18)
            {
                return string.Format(Literals.Greeting_Afternoon, name);
            }

            if((now.Hour >= 18 && now.Hour <= 24) ||
               (now.Hour >= 0 && now.Hour < 4))
            {
                return string.Format(Literals.Greeting_Evening, name);
            }

            return string.Format(Literals.Greeting_General, name);
        }
    }
}