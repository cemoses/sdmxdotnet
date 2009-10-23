using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Common;
using System.Xml;

namespace OXM
{
    internal class ElementContainerMap<T> : ElementMapBase<T>, IElementMapContainer<T>
    {
        private MapList<T> _elementMaps = new MapList<T>();

        public ElementContainerMap(XName name, bool required)
            : base(name, required, false)
        {
        }

        public override void ReadXml(XmlReader reader)
        {
            _occurances++;

            reader.ReadStartElement();

            while (reader.NodeType == XmlNodeType.Element)
            {
                XNamespace ns = reader.NamespaceURI;
                XName name = ns + reader.Name;
                var elementMap = _elementMaps.Get(name);
                elementMap.ReadXml(reader);
                reader.ReadStartElement();
            }
        }

        bool isWritten = false;
        private void ElementWriting(XmlWriter writer)
        {
            if (!isWritten)
            {
                writer.WriteStartElement(Name.LocalName, Name.NamespaceName);
                isWritten = true;
            }
        }

        public override void WriteXml(XmlWriter writer, T obj)
        {
            foreach (var map in _elementMaps)
            {
                // before a child element start writing write the start element of the container
                // this is necessary to avoid writing and empty container element we we only 
                // write the container element if a child element starts writing
                ((IElementMap<T>)map).Writing = () => ElementWriting(writer);
                
                map.WriteXml(writer, obj);
            }
            
            // don't add empty container elements
            if (isWritten)
            {
                writer.WriteEndElement();
            }
        }

        public override void AssertValid()
        {
            base.AssertValid();

            // if the container element exists then check its children
            if (_occurances > 0)
            {
                foreach (var elementMap in _elementMaps)
                {
                    ((IElementMap<T>)elementMap).AssertValid();
                }
            }
        }

        #region IElementMapContainer<T> Members

        void IElementMapContainer<T>.AddElementMap(XName name, IMemberMap<T> map)
        {
            _elementMaps.Add(name, map);
        }

        #endregion

        #region IMapContainer<T> Members

        XNamespace IMapContainer<T>.Namespace
        {
            get { return Name.Namespace; }
        }

        #endregion
    }
}