using System;
using System.Collections.Generic;
using System.Text;
using Ait.SolarPanels.Core.Entities;

namespace Ait.SolarPanels.Core.Services
{
    public class PanelService
    {
        public List<Panel> Panels { get; private set; }
        public PanelService()
        {
            Panels = new List<Panel>();
        }
        public void AddPanel(int id)
        {
            Panels.Add(new Panel(id));
        }
        public void AddPanel(int id, int surface, int maxPowerPerSquareMeter)
        {
            Panels.Add(new Panel(id, surface, maxPowerPerSquareMeter));
        }
    }
}
