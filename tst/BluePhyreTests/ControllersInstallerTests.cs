using System;
using System.Linq;
using System.Web.Mvc;
using BluePhyre.Controllers;
using BluePhyre.Installers;
using Castle.Core;
using Castle.Core.Internal;
using Castle.MicroKernel;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BluePhyreTests
{

    [TestClass]
    public class ControllersInstallerTests
    {

        private readonly IWindsorContainer _containerWithControllers;

        public ControllersInstallerTests()
        {
            _containerWithControllers = new WindsorContainer().Install(new ControllersInstaller());
        }

        public TestContext TestContext { get; set; }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void All_controllers_implement_IController()
        {
            var allHandlers = GetAllHandlers(_containerWithControllers);
            var controllerHandlers = GetHandlersFor(typeof(IController), _containerWithControllers);

            Assert.AreNotEqual(0, allHandlers.Count());
            Assert.AreEqual(allHandlers.Count(), controllerHandlers.Count());
        }

        [TestMethod]
        public void All_controllers_are_registered()
        {
            // Is<TType> is an helper, extension method from Windsor
            // which behaves like 'is' keyword in C# but at a Type, not instance level
            var allControllers = GetPublicClassesFromApplicationAssembly(c => c.Is<IController>());
            var registeredControllers = GetImplementationTypesFor(typeof(IController), _containerWithControllers);
            Assert.AreEqual(allControllers.Count(), registeredControllers.Count());
        }

        [TestMethod]
        public void All_and_only_controllers_have_Controllers_suffix()
        {
            var allControllers = GetPublicClassesFromApplicationAssembly(c => c.Name.EndsWith("Controller"));
            var registeredControllers = GetImplementationTypesFor(typeof(IController), _containerWithControllers);
            Assert.AreEqual(allControllers.Count(), registeredControllers.Count());
        }

        [TestMethod]
        public void All_and_only_controllers_live_in_Controllers_namespace()
        {
            var allControllers = GetPublicClassesFromApplicationAssembly(c => c.Namespace.Contains("Controllers"));
            var registeredControllers = GetImplementationTypesFor(typeof(IController), _containerWithControllers);
            Assert.AreEqual(allControllers.Count(), registeredControllers.Count());
        }

        [TestMethod]
        public void All_controllers_are_transient()
        {
            var nonTransientControllers = GetHandlersFor(typeof(IController), _containerWithControllers)
                .Where(controller => controller.ComponentModel.LifestyleType != LifestyleType.Transient)
                .ToArray();

            Assert.AreEqual(0, nonTransientControllers.Count());
        }

        //[TestMethod]
        //public void All_controllers_expose_themselves_as_service()
        //{
        //    var controllersWithWrongName = GetHandlersFor(typeof(IController), _containerWithControllers)
        //        .Where(controller => controller.Service != controller.ComponentModel.Implementation)
        //        .ToArray();

        //    Assert.AreEqual(0,controllersWithWrongName.Count());
        //}

        #region "Helper methods"

        private IHandler[] GetAllHandlers(IWindsorContainer container)
        {
            return GetHandlersFor(typeof(object), container);
        }

        private IHandler[] GetHandlersFor(Type type, IWindsorContainer container)
        {
            return container.Kernel.GetAssignableHandlers(type);
        }

        private Type[] GetImplementationTypesFor(Type type, IWindsorContainer container)
        {
            return GetHandlersFor(type, container)
            .Select(h => h.ComponentModel.Implementation)
            .OrderBy(t => t.Name)
            .ToArray();
        }

        private Type[] GetPublicClassesFromApplicationAssembly(Predicate<Type> where)
        {
            return typeof(HomeController).Assembly.GetExportedTypes()
            .Where(t => t.IsClass)
            .Where(t => t.IsAbstract == false)
            .Where(where.Invoke)
            .OrderBy(t => t.Name)
            .ToArray();
        }

        #endregion

    }
}
