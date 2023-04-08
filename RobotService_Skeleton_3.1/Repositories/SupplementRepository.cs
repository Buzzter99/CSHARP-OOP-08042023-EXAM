using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RobotService.Models.Contracts;
using RobotService.Repositories.Contracts;

namespace RobotService.Repositories
{
    public class SupplementRepository : IRepository<ISupplement>
    {
        private readonly List<ISupplement> models;
        public IReadOnlyCollection<ISupplement> Models() => models.AsReadOnly();

        public void AddNew(ISupplement model)
        {
            this.models.Add(model);
        }
        public bool RemoveByName(string typeName)
        {
            return this.models.Remove(this.models.FirstOrDefault(x => x.GetType().Name == typeName));
        }
        public ISupplement FindByStandard(int interfaceStandard)
        {
            return this.models.FirstOrDefault(x => x.InterfaceStandard == interfaceStandard);
        }
        public SupplementRepository()
        {
            this.models = new List<ISupplement>();
        }
    }
}
