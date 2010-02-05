using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace SDMX
{
    public abstract class Component : AnnotableArtefact
    {
        public Concept Concept { get; set; }
        public CodeList CodeList { get; set; }
        public ITextFormat TextFormat { get; set; }
        public CrossSectionalAttachmentLevel CrossSectionalAttachmentLevel { get; set; }

        public int Order { get; set; }

        public Component(Concept concept)
        {
            this.Concept = concept;
        }

        public Component(Concept concept, CodeList codeList)
        {
            this.Concept = concept;
            this.CodeList = codeList;
        }

        public bool IsCoded
        {
            get
            {
                return CodeList != null;
            }
        }

        //public virtual object Parse(string value)
        //{
        //    return Parse(value, null);    
        //}

        //public virtual object Parse(string value, string startTime)
        //{
        //    if (IsCoded)
        //    {
        //        return CodeList.Get((ID)value);
        //    }
        //    else
        //    {
        //        throw new Exception("non coded components are not supported yet.");
        //    }
        //}       

        public virtual bool TryParse(string s, string startTime, out IValue value, out string reason)
        {
            value = null;
            reason = null;
            if (IsCoded)
            {
                var code = CodeList.Get((ID)s);
                if (code == null)
                {
                    reason = string.Format("Code not found for value '{0}'.", s);
                    return false;
                }
                value = code;
                return true;
            }
            else
            {
                return TextFormat.TryParse(s, startTime, out value, out reason);
            }
        }

        public virtual void Serialize(IValue value, out string stringValue, out string startTime)
        {
            if (IsCoded)
            {
                stringValue = null; //((ID)value).ToString();
                startTime = null;
            }
            else
            {   
               TextFormat.Serialize(value, out stringValue, out startTime);               
            }
        }

        public virtual bool IsValid(IValue value)
        {
            if (IsCoded)
            {
                return false;
                // return value.Is<Code>() && CodeList.Contains((ID)value);
            }
            else
            {
                return TextFormat.IsValid(value);
            }
        }
    }
}
