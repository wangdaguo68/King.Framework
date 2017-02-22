namespace King.Framework.Linq.Data.Mapping
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Xml.Linq;

    public class XmlMapping : AttributeMapping
    {
        private Dictionary<string, XElement> entities;
        private static readonly XName Entity = XName.Get("Entity");
        private static readonly XName Id = XName.Get("Id");

        public XmlMapping(XElement root) : base(null)
        {
            this.entities = (from e in root.Elements()
                where e.Name == Entity
                select e).ToDictionary<XElement, string>(e => (string) e.Attribute(Id));
        }

        private Type FindType(string name)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(name);
                if (type != null)
                {
                    return type;
                }
            }
            return null;
        }

        public static XmlMapping FromXml(string xml)
        {
            return new XmlMapping(XElement.Parse(xml));
        }

        private MappingAttribute GetMappingAttribute(XElement element)
        {
            switch (element.Name.LocalName)
            {
                case "Table":
                    return this.GetMappingAttribute(typeof(TableAttribute), element);

                case "ExtensionTable":
                    return this.GetMappingAttribute(typeof(ExtensionTableAttribute), element);

                case "Column":
                    return this.GetMappingAttribute(typeof(ColumnAttribute), element);

                case "Association":
                    return this.GetMappingAttribute(typeof(AssociationAttribute), element);
            }
            return null;
        }

        private MappingAttribute GetMappingAttribute(Type attrType, XElement element)
        {
            MappingAttribute attribute = (MappingAttribute) Activator.CreateInstance(attrType);
            foreach (PropertyInfo info in attrType.GetProperties())
            {
                XAttribute attribute2 = element.Attribute(info.Name);
                if (attribute2 != null)
                {
                    if (info.PropertyType == typeof(Type))
                    {
                        info.SetValue(attribute, this.FindType(attribute2.Value), null);
                    }
                    else
                    {
                        info.SetValue(attribute, Convert.ChangeType(attribute2.Value, info.PropertyType), null);
                    }
                }
            }
            return attribute;
        }

        protected override IEnumerable<MappingAttribute> GetMappingAttributes(string rootEntityId)
        {
            XElement iteratorVariable0;
            if (this.entities.TryGetValue(rootEntityId, out iteratorVariable0))
            {
                foreach (XElement iteratorVariable1 in iteratorVariable0.Elements())
                {
                    if (iteratorVariable1 == null)
                    {
                        continue;
                    }
                    yield return this.GetMappingAttribute(iteratorVariable1);
                }
            }
        }

       
    }
}
