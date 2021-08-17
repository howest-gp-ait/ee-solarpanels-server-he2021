using System;
using System.Collections.Generic;
using System.Text;

namespace Ait.SolarPanels.Core.Entities
{
    public enum Suncondition { Zwaarbewolkt, Lichtbewolkt, OverwegendZon, VolleZon}
    public class Panel
    {
        public int ID { get; private set; }
        public int Surface { get; private set; }  // opp in m²
        public int MaxPower { get; private set; }  // maximaal vermogen per paneel


        public Panel(int id)
        {
            ID = id;
            Random rnd = new Random();
            Surface = rnd.Next(2, 5); // een paneel is 2, 3 of 4 m² groot
            int maxPowerPerSquareMeter = rnd.Next(150, 301); // max vermogen per m² ligt tussen 150 en 300
            MaxPower = Surface * maxPowerPerSquareMeter;
        }
        public Panel(int id, int surface, int maxPowerPerSquareMeter)
        {
            ID = id;
            Surface = surface;
            MaxPower = Surface * maxPowerPerSquareMeter;
        }
        public float GetCurrentPower(Suncondition sunCondition)
        {
            float sunfactor = 0.2F;
            if (sunCondition == Suncondition.Lichtbewolkt) sunfactor = 0.5F;
            else if (sunCondition == Suncondition.OverwegendZon) sunfactor = 0.8F;
            else if (sunCondition == Suncondition.VolleZon) sunfactor = 1F;

            int hour = DateTime.Now.Hour;
            if (hour < 6)
                return 0F;
            else if (hour < 8)
                return 0.4F * MaxPower * sunfactor;
            else if (hour < 10)
                return 0.6F * MaxPower * sunfactor;
            else if (hour < 12)
                return 0.8F * MaxPower * sunfactor;
            else if (hour < 16)
                return sunfactor * MaxPower;
            else if (hour < 20)
                return 0.7F * MaxPower * sunfactor;
            else if (hour < 22)
                return 0.3F * MaxPower * sunfactor;
            else
                return 0F;
        }
        public override string ToString()
        {
            return $"ID {ID} - Surface {Surface}m² - MaxPow {MaxPower}W";
        }
    }
}
