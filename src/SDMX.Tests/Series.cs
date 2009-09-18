using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDMX.Tests
{
    public class Series
    {
        //public SeriesKey Key { get; private set; }
        private Dictionary<Dimension, object> key = new Dictionary<Dimension,object>();
        private Dictionary<TimePeriod, Observation> observations = new Dictionary<TimePeriod, Observation>();
        private DataSet dataSet;

        public IList<Attribute> Attributes { get; internal set; }
        public IList<Annotation> Annotations { get; internal set; }

        internal Series()
        {
            Attributes = new List<Attribute>();
            Annotations = new List<Annotation>();
        }

        internal Series(DataSet dataSet) : this()
        {
            this.dataSet = dataSet;
        }

        public void AddKeyValue(string concept, string value)
        {
            Dimension dimension = dataSet.KeyFamily.GetDimension(concept);
            if (dimension == null)
            { 
                throw new SDMXException("Cannot find demension '{0}'".F(concept));
            }

            object dimValue = dimension.GetValue(value);

            key[dimension] = value;
        }

        public object GetKeyValue(string concept)
        {
            var dimension = dataSet.KeyFamily.GetDimension(concept);
            if (dimension == null)
            {
                throw new SDMXException("dimension doesn't exist in key family");
            }

            object value = key.GetValueOrDefault(dimension, null);

            if (value == null)
            {
                throw new SDMXException("value not found for dimension '{0}'".F(concept));
            }

            return value;
        }

        internal void AddObservation(Observation observation)
        {
            if (observations.Keys.Contains(observation.TimePeriod))
            {
                throw new SDMXException("observation with time period already exsists '{0}'".F(observation.TimePeriod));
            }

            observations.Add(observation.TimePeriod, observation);

        }
    }

    public class DimensionValue
    {
        public Dimension Dimension { get; internal set; }
        public object Value { get; internal set; }

    }
}