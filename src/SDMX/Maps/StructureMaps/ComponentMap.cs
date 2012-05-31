using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Common;
using OXM;

namespace SDMX.Parsers
{
    internal abstract class ComponentMap<T> : AnnotableArtefactMap<T>
            where T : Component
    {
        protected abstract T Create(Concept conecpt);

        T _component;

        public ComponentMap(StructureMessage message)
        {
            Map(o => ConceptRef.Create(o.Concept)).ToAttributeGroup("conceptRef")
                .Set(v => _component = Create(GetConcept(message, v)))
                .GroupTypeMap(new ConceptRefMap());

            Map(o => TempCodelistRef.Create(o.CodeList)).ToAttributeGroup("codelist")
                .Set(v => _component.CodeList = GetCodeList(message, v))
                .GroupTypeMap(new TempCodelistRefMap());           

            Map(o => GetTextFormat(o)).ToElement("TextFormat", false)
                .Set(v => SetTextFormat(v))
                .ClassMap(() => new TextFormatMap());
        }

        TextFormat GetTextFormat(Component compoenent)
        {
            if (compoenent.TextFormat.Equals(compoenent.DefaultTextFormat))
                return null;
            else
                return compoenent.TextFormat;
        }

        void SetTextFormat(TextFormat value)
        {
            if (typeof(T) == typeof(TimeDimension) && !(value is TimePeriodTextFormatBase))
            {
                throw new SDMXException("The text format for TimeDimension must be of time specific type (ObservationalTimePeriod, DateTime, Date, etc) but was found to be of type '{0}'.", value.GetType());
            }

            if (value != null)
            {
                _component.TextFormat = value;
            }
        }

        CodeList GetCodeList(StructureMessage message, TempCodelistRef v)
        {
            var codelist = message.FindCodeList(v.Id, v.AgencyId, v.Version);
            
            if (codelist == null)
                throw new SDMXException("Codelist not found: id='{0}',agencyId='{1}',version='{2}'. Codelists thar are referenced by a key family must exist in the same file of the key family.",
                    v.Id, v.AgencyId, v.Version);

            return codelist;
        }

        Concept GetConcept(StructureMessage message, ConceptRef v)
        {
            var concept = message.GetConcept(v.SchemeRef.Id, v.SchemeRef.AgencyId, v.SchemeRef.Version, v.Id, v.AgencyId, v.Version);

            if (concept == null)
                throw new SDMXException("Concept not found: conceptSchemeId='{0}',conceptSchemeAgencyId='{1}',conceptSchemeVersion='{2}, conceptId='{3}',concpetAgencyId='{4}',conceptVersion='{5}'. Concepts thar are referenced by a key family must exist in the same file of the key family.",
                    v.SchemeRef.Id, v.SchemeRef.AgencyId, v.SchemeRef.Version, v.Id, v.AgencyId, v.Version);

            return concept;
        }
    }
}