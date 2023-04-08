using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RobotService.Core.Contracts;
using RobotService.Models;
using RobotService.Models.Contracts;
using RobotService.Repositories;
using RobotService.Utilities.Messages;

namespace RobotService.Core
{
    public class Controller : IController
    {
        private SupplementRepository supplements;
        private RobotRepository robots;
        public Controller()
        {
            this.supplements = new SupplementRepository();
            this.robots = new RobotRepository();
        }
        public string CreateRobot(string model, string typeName)
        {
            IRobot robot = null;
            if (typeName != nameof(DomesticAssistant) && typeName != nameof(IndustrialAssistant))
            {
                return String.Format(OutputMessages.RobotCannotBeCreated, typeName);
            } else if (typeName == nameof(DomesticAssistant))
            {
                robot = new DomesticAssistant(model);
            } else if (typeName == nameof(IndustrialAssistant))
            {
                robot = new IndustrialAssistant(model);
            }
            this.robots.AddNew(robot);
            return String.Format(OutputMessages.RobotCreatedSuccessfully, typeName, model);
        }

        public string CreateSupplement(string typeName)
        {
            if (typeName != nameof(SpecializedArm) && typeName != nameof(LaserRadar))
            {
                return String.Format(OutputMessages.SupplementCannotBeCreated, typeName);
            }
            ISupplement supplement = null;
            if (typeName == nameof(SpecializedArm))
            {
                supplement = new SpecializedArm();
            } else if (typeName == nameof(LaserRadar))
            {
                supplement = new LaserRadar();
            }
            this.supplements.AddNew(supplement);
            return String.Format(OutputMessages.SupplementCreatedSuccessfully, typeName);
        }

        public string UpgradeRobot(string model, string supplementTypeName)
        {
            ISupplement supplement = this.supplements.Models().FirstOrDefault(x => x.GetType().Name == supplementTypeName);
            IRobot robot = this.robots.Models().Where(x => x.Model == model).FirstOrDefault(x => !x.InterfaceStandards.Contains(supplement.InterfaceStandard));
            if (robot == null)
            {
                return String.Format(OutputMessages.AllModelsUpgraded, model);
            }
            robot.InstallSupplement(supplement);
            this.supplements.RemoveByName(supplementTypeName);
            return String.Format(OutputMessages.UpgradeSuccessful, model, supplementTypeName);
        }

        public string RobotRecovery(string model, int minutes)
        {
            List<IRobot> robots = this.robots.Models().Where(x => x.Model == model && x.BatteryLevel <= x.BatteryCapacity / 2).ToList();
            foreach (var test in robots)
            {
                test.Eating(minutes);
            }
            return String.Format(OutputMessages.RobotsFed, robots.Count);
        }

        public string PerformService(string serviceName, int intefaceStandard, int totalPowerNeeded)
        {
            List<IRobot> robots = this.robots.Models().Where(x => x.InterfaceStandards.Contains(intefaceStandard)).OrderByDescending(x => x.BatteryLevel).ToList();
            int counter = 0;
            if (robots.Count == 0)
            {
                return String.Format(OutputMessages.UnableToPerform, intefaceStandard);
            }
            int sum = robots.Sum(x => x.BatteryLevel);
            if (sum < totalPowerNeeded)
            {
                return String.Format(OutputMessages.MorePowerNeeded,serviceName,totalPowerNeeded - sum);
            } else if (sum >= totalPowerNeeded)
            {
                while (totalPowerNeeded > 0)
                {
                    var element = robots.FirstOrDefault();
                    if (element.BatteryLevel >= totalPowerNeeded)
                    {
                        element.ExecuteService(totalPowerNeeded);
                        counter++;
                        totalPowerNeeded = 0;
                    } else if (element.BatteryLevel < totalPowerNeeded)
                    {
                        totalPowerNeeded -= element.BatteryLevel;
                        element.ExecuteService(element.BatteryLevel);
                        robots.Remove(element);
                        counter++;
                    }
                }
            }
            return String.Format(OutputMessages.PerformedSuccessfully,serviceName,counter);

        }

        public string Report()
        {
            var collection = this.robots.Models().OrderByDescending(x => x.BatteryLevel).ThenBy(x => x.BatteryCapacity)
                .ToList();
            StringBuilder sb = new StringBuilder();
            foreach (var item in collection)
            {
                sb.AppendLine(item.ToString());
            }
            return sb.ToString().TrimEnd();
        }
    }
}
