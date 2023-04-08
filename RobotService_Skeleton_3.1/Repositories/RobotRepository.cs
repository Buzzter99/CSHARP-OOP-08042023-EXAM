using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RobotService.Models.Contracts;
using RobotService.Repositories.Contracts;

namespace RobotService.Repositories
{
    public class RobotRepository : IRepository<IRobot>
    {
        private readonly List<IRobot> models;
        public IReadOnlyCollection<IRobot> Models() => models.AsReadOnly();

        public void AddNew(IRobot model)
        {
            this.models.Add(model);
        }
        public bool RemoveByName(string typeName)
        {
            return this.models.Remove(this.models.FirstOrDefault(x => x.Model == typeName));
        }
        public IRobot FindByStandard(int interfaceStandard)
        {
            return this.models.FirstOrDefault(x => x.InterfaceStandards.Any(x => x == interfaceStandard));
        }
        public RobotRepository()
        {
            this.models = new List<IRobot>();
        }
    }
}
