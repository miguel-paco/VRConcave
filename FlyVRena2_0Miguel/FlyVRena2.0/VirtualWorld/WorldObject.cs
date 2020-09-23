using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using FlyVRena2._0.VirtualWorld.ServiceFactories;
using FlyVRena2._0.VirtualWorld.ServiceFactories.DrawFactories;
using FlyVRena2._0.VirtualWorld.ServiceFactories.UpdateFactories;
using FlyVRena2._0.VirtualWorld.Services;

namespace FlyVRena2._0.VirtualWorld
{
    /* Interface service container implements a service provider with the ability of manage the services */
    public interface IServiceContainer : IServiceProvider
    {
        void AddService(Type serviceType, object serviceInstance);
        void RemoveService(Type serviceType);
    }
    /* Basic object of this program, has the properties of a service container */
    [XmlRoot("WorldObject")]
    public class WorldObject : IServiceContainer
    {
        // List of World Objects
        [XmlArray]
        [XmlArrayItem("WorldObject")]
        public List<WorldObject> WObjects { get; set; }

        // List of services
        private Dictionary<Type, object> services = new Dictionary<Type, object>();

        // List of service factories
        [XmlArray]
        [XmlArrayItem(ElementName = "NameFactory", Type = typeof(NameFactory))]
        [XmlArrayItem(ElementName = "PositionFactory", Type = typeof(PositionFactory))]
        [XmlArrayItem(ElementName = "TexturedSquareFactory", Type = typeof(TexturedSquareFactory))]
        [XmlArrayItem(ElementName = "WhiteSquareFactory", Type = typeof(WhiteSquareFactory))]
        [XmlArrayItem(ElementName = "DarkSquareFactory", Type = typeof(DarkSquareFactory))]
        [XmlArrayItem(ElementName = "DarkCircleFactory", Type = typeof(DarkCircleFactory))]
        [XmlArrayItem(ElementName = "WhiteCircleFactory", Type = typeof(WhiteCircleFactory))]
        [XmlArrayItem(ElementName = "DarkEllipseFactory", Type = typeof(DarkEllipseFactory))]
        [XmlArrayItem(ElementName = "UpdateWithFlyConstantDriftFactory", Type = typeof(UpdateWithFlyConstantDriftFactory))]
        [XmlArrayItem(ElementName = "UpdateOscilatingDriftFactory", Type = typeof(UpdateOscilatingDriftFactory))]
        [XmlArrayItem(ElementName = "UpdateFlickerClockFactory", Type = typeof(UpdateFlickerClockFactory))]
        [XmlArrayItem(ElementName = "UpdateNormalReverseGainFactory", Type = typeof(UpdateNormalReverseGainFactory))]
        [XmlArrayItem(ElementName = "RFMappingRotDotStimFactory", Type = typeof(RFMappingRotDotStimFactory))]
        [XmlArrayItem(ElementName = "VRProtocolFactory", Type = typeof(VRProtocolFactory))]
        [XmlArrayItem(ElementName = "UpdateRotationReplayFactory", Type = typeof(UpdateRotationReplayFactory))]
        [XmlArrayItem(ElementName = "UpdateStimulusFactory", Type = typeof(UpdateStimulusFactory))]


        public List<ServiceFactory> objectBuilder { get; set; }

        // Add a child WorldObject to the WorldObject list
        public void AddWorldObject(WorldObject obj)
        {
            WObjects.Add(obj);
        }

        // Get a child WorldObject from the WorldObject list using its name
        public WorldObject GetWorldObject(string str)
        {
            foreach (WorldObject obj in WObjects)
            {
                NameService Name = (NameService)obj.GetService(typeof(NameService));
                if (str == Name.ObjectName())
                {
                    return obj;
                }
            }
            return null;
        }

        // Remove a child WorldObject from the WorldObject list using its name
        public void RemoveWorldObject(string str)
        {
            foreach (WorldObject obj in WObjects)
            {
                NameService Name = (NameService)obj.GetService(typeof(NameService));
                if (str == Name.ObjectName())
                {
                    WObjects.Remove(obj);
                }
            }
        }

        // Add a Service to the Service list
        public void AddService(Type Service, object provider)
        {
            // If we already have this type of service, throw an exception
            if (services.ContainsKey(Service))
                throw new Exception("The service container already has a " + "service provider of type " + Service.Name);

            this.services.Add(Service, provider);
        }

        // Get all Services in the Service list
        public object[] GetAllSevices()
        {
            object[] list = new object[services.Count];
            int i = 0;
            foreach (Type type in services.Keys)
            {
                list[i] = services[type];
                i++;
            }
            return list;
        }

        // Get a specific Service from the Service list
        public object GetService(Type Service)
        {

            object service;
            // If the required Service is that of a service container return this WorldObject itself
            if (Service == typeof(IServiceContainer))
            {
                return this;
            }
            // If we have this type of service, return it, otherwise return null
            services.TryGetValue(Service, out service);
            return service;
        }

        // Check if a type of Service already exists in the Service list
        public bool ContainService(Type Service)
        {
            return services.ContainsKey(Service);
        }

        // Remove a specific Service from the Service list
        public void RemoveService(Type Service)
        {
            if (services.ContainsKey(Service))
                services.Remove(Service);
        }

        // Remove all Services from the Service list
        public void RemoveAllServices()
        {
            services.Clear();
        }
    }
}
