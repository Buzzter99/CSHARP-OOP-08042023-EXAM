using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RobotFactory.Tests
{
    [TestFixture]
    public class Tests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void RobotConstructorShouldWork()
        {
            Robot robot = new Robot("robot", 50, 60);
            Assert.AreEqual("robot", robot.Model);
            Assert.AreEqual(50, robot.Price);
            Assert.AreEqual(60, robot.InterfaceStandard);
            Assert.IsNotNull(robot.Supplements);
            Assert.AreEqual(0, robot.Supplements.Count);
            string toStringOnRobot = "Robot model: robot IS: 60, Price: 50.00";
            string actual = robot.ToString();
            Assert.AreEqual(toStringOnRobot, actual);
        }
        [Test]
        public void SupplementConstructorShouldWork()
        {
            Supplement supplement = new Supplement("nameS", 20);
            Assert.AreEqual("nameS", supplement.Name);
            Assert.AreEqual(20, supplement.InterfaceStandard);
            string expected = "Supplement: nameS IS: 20";
            string actual = supplement.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FactoryConstructorShouldWork()
        {
            Factory factory = new Factory("FactoryName", 50);
            Assert.AreEqual("FactoryName", factory.Name);
            Assert.AreEqual(50, factory.Capacity);
            Assert.IsNotNull(factory.Robots);
            Assert.IsNotNull(factory.Supplements);
            Assert.AreEqual(0, factory.Robots.Count);
            Assert.AreEqual(0, factory.Supplements.Count);
        }

        [Test]
        public void ProduceRobotShouldWork()
        {
            Factory factory = new Factory("FactoryName", 50);
            string actual = factory.ProduceRobot("robot", 50, 60);
            string expected = "Produced --> Robot model: robot IS: 60, Price: 50.00";
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(1, factory.Robots.Count);
        }

        [Test]
        public void ProduceShouldReturnSecondOutput()
        {
            Factory factory = new Factory("FactoryName", 1);
            factory.ProduceRobot("robot", 50, 60);
            string result = factory.ProduceRobot("robot", 50, 60);
            string expected = "The factory is unable to produce more robots for this production day!";
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ProduceWithMultipleOutputs()
        {
            Factory factory = new Factory("FactoryName", 10);
            factory.ProduceRobot("robot", 50, 60);
            factory.ProduceRobot("robot", 50, 60);
            factory.ProduceRobot("robot", 50, 60);
            factory.ProduceRobot("robot", 50, 60);
            factory.ProduceRobot("robot", 50, 60);
            Assert.AreEqual(5,factory.Robots.Count);
        }

        [Test]
        public void SupplementAddShoudlWork()
        {
            Factory factory = new Factory("FactoryName", 1);
            Supplement supplement = new Supplement("Supplement", 50);
            string actual = factory.ProduceSupplement(supplement.Name, supplement.InterfaceStandard);
            string expected = "Supplement: Supplement IS: 50";
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(1,factory.Supplements.Count);
        }
        [Test]
        public void UpgrageShoudlBeFalse()
        {
            Factory factory = new Factory("FactoryName", 1);
            Supplement supplement = new Supplement("Supplement", 30);
            factory.ProduceSupplement(supplement.Name, supplement.InterfaceStandard);
            Robot robot = new Robot("model", 20, 30);
            bool shouldBeTrue = factory.UpgradeRobot(robot, supplement);
            Assert.IsTrue(shouldBeTrue);
            Assert.AreEqual(1, robot.Supplements.Count);

        }

        [Test]
        public void ShouldReturnFalse()
        {
            Factory factory = new Factory("FactoryName", 1);
            Supplement supplement = new Supplement("Supplement", 30);
            factory.ProduceSupplement(supplement.Name, supplement.InterfaceStandard);
            Robot robot = new Robot("model", 20, 30);
            factory.UpgradeRobot(robot, supplement);
            bool shouldBeFalse = factory.UpgradeRobot(robot, supplement);
            Assert.IsFalse(shouldBeFalse);
        }

        [Test]
        public void ShouldBeFalse()
        {
            Factory factory = new Factory("FactoryName", 1);
            Supplement supplement = new Supplement("Supplement", 30);
            factory.ProduceSupplement(supplement.Name, supplement.InterfaceStandard);
            Robot robot = new Robot("model", 20, 30);
            robot.Supplements.Add(supplement);
            bool shoudBeFalse = factory.UpgradeRobot(robot, supplement);
            Assert.IsFalse(shoudBeFalse);
        }

        [Test]
        public void ShouldReturnFalseWithDifferentInterfaces()
        {
            Factory factory = new Factory("FactoryName", 1);
            Supplement supplement = new Supplement("Supplement", 30);
            factory.ProduceSupplement(supplement.Name, supplement.InterfaceStandard);
            Robot robot = new Robot("model", 20, 40);
            Assert.IsFalse(factory.UpgradeRobot(robot, supplement));
        }

        [Test]
        public void SellRobotShouldWork()
        {
            Factory factory = new Factory("FactoryName", 5);
            factory.ProduceRobot("robot1", 10, 20);
            factory.ProduceRobot("robot2", 20, 30);
            factory.ProduceRobot("robot3", 30, 40);
            Robot robot = new Robot("name", 40, 50);
            factory.Robots.Add(robot);
            Robot actual = factory.SellRobot(50);
            Assert.AreEqual(robot, actual);
            Robot secondRobot = factory.SellRobot(20);
        }

        [Test]
        public void SellShouldWorkAgain()
        {
            Factory factory = new Factory("FactoryName", 5);
            factory.ProduceRobot("robot1", 10, 20);
            factory.ProduceRobot("robot2", 20, 30);
            factory.ProduceRobot("robot3", 30, 40);
            Robot robotActual = factory.SellRobot(30);
            Assert.AreEqual("robot3", robotActual.Model);
            Assert.AreEqual(30, robotActual.Price);
            Assert.AreEqual(40, robotActual.InterfaceStandard);
        }

        [Test]
        public void SellWorking()
        {
            Factory factory = new Factory("FactoryName", 5);
            factory.ProduceRobot("robot1", 10, 20);
            factory.ProduceRobot("robot2", 20, 30);
            factory.ProduceRobot("robot3", 30, 40);
            Robot robotActual = factory.SellRobot(20);
            Assert.AreEqual("robot2", robotActual.Model);
            Assert.AreEqual(20, robotActual.Price);
            Assert.AreEqual(30, robotActual.InterfaceStandard);
        }
        [Test]
        public void SellShouldReturnNull()
        {
            Factory factory = new Factory("FactoryName", 5);
            factory.ProduceRobot("robot1", 10, 20);
            factory.ProduceRobot("robot2", 20, 30);
            factory.ProduceRobot("robot3", 30, 40);
            Robot robot = factory.SellRobot(9);
            Assert.IsNull(robot);
        }
    }
}