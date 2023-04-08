using System;
using System.Collections.Generic;
using System.Text;
using RobotService.Models.Contracts;
using RobotService.Utilities.Messages;

namespace RobotService.Models
{
    public abstract class Robot : IRobot
    {
        private string model;
        private int batteryCapacity;
        private readonly List<int> interfaceStandards;

        public string Model
        {
            get
            {
                return this.model;
            }
            private set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentException(String.Format(ExceptionMessages.ModelNullOrWhitespace));
                }
                this.model = value;
            }
        }

        public int BatteryCapacity
        {
            get
            {
                return this.batteryCapacity;
            }
            private set
            {
                if (value < 0)
                {
                    throw new ArgumentException(String.Format(ExceptionMessages.BatteryCapacityBelowZero));
                }
                this.batteryCapacity = value;
            }
        }

        public int BatteryLevel { get; private set; }
        public int ConvertionCapacityIndex { get; private set; }
        public IReadOnlyCollection<int> InterfaceStandards => this.interfaceStandards.AsReadOnly();
        public void Eating(int minutes)
        {
            int value = this.ConvertionCapacityIndex * minutes;
            if (this.BatteryCapacity == this.BatteryLevel)
            {
                return;
            }
            this.BatteryLevel += value;
        }

        public void InstallSupplement(ISupplement supplement)
        {
            this.BatteryCapacity -= supplement.BatteryUsage;
            this.BatteryLevel -= supplement.BatteryUsage;
            this.interfaceStandards.Add(supplement.InterfaceStandard);
        }

        public bool ExecuteService(int consumedEnergy)
        {
            if (this.BatteryLevel >= consumedEnergy)
            {
                this.BatteryLevel -= consumedEnergy;
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{this.GetType().Name} {this.Model}:");
            sb.AppendLine($"--Maximum battery capacity: {this.BatteryCapacity}");
            sb.AppendLine($"--Current battery level: {this.BatteryLevel}");
            string result = this.InterfaceStandards.Count == 0 ? "none" : string.Join(" ", InterfaceStandards);
            sb.AppendLine($"--Supplements installed: {result}");
            return sb.ToString().TrimEnd();
        }
        protected Robot(string model, int batteryCapacity, int conversionCapacityIndex)
        {
            this.Model = model;
            this.BatteryCapacity = batteryCapacity;
            this.BatteryLevel = batteryCapacity;
            this.ConvertionCapacityIndex = conversionCapacityIndex;
            this.interfaceStandards = new List<int>();
        }
    }
}
