using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathParser
{
    public interface IDefined
    {
        public IDefinable Definition { get; }
    }
    public interface IDefinable : IDefined
    {
        public double? Value { get; }
    }
    class Constant : IDefinable
    {
        public IDefinable Definition
        {
            get { return this; }
        }
        public double? Value { get; set; }
        public Constant(double? value) { Value = value; }
        public override string ToString()
        {
            return Value.ToString();
        }
    }
    class Variable : IDefinable
    {
        public IDefinable Definition
        {
            get { return Link; }
        }
        public double? Value
        {
            get
            {
                if (Link == null) { return null; }

                return Link.Value;
            }
            set
            {
                Link = new Constant(value);
            }
        }
        public string Name { get; }
        public IDefinable Link { get; set; }

        public Variable(string name) { Name = name; }
        public override string ToString() { return Name; }
    }
    class Element : IDefinable
    {
        public IDefinable Definition
        {
            get { return this; }
        }
        public double? Value
        {
            get
            {
                if (Item == null) { return null; }
                double? returnValue = Item.Value;

                if (returnValue == null) { return null; }

                if (negative) { returnValue = -returnValue; }
                if (underOne) { returnValue = 1 / returnValue; }

                return returnValue;
            }
        }
        public IDefinable Item { get; set; }

        public Element(IDefinable item) { Item = item; }

        #region modifiers: negative and underOne and others
        private bool negative;
        private bool underOne;
        public bool Negative { get { return negative; } set { negative = value; } }
        public bool UnderOne { get { return underOne; } set { underOne = value; } }
        #endregion

        #region tostring
        public string ItemToString() { return ItemToString(null); }
        public string ItemToString(int? parentLevel)
        {
            if (Item == null) { return "null"; }

            //if wrap is true the result is wrapped in parenthesis before returning, ex: (-9):negative constant; (4+7):Elements
            bool wrap = false;

            //wrap if ElementList (plus level based condition, this condition is not checked if parentLevel is null)
            if (Item.GetType() == typeof(Expression))
            {
                if (parentLevel != null)
                {
                    if (parentLevel >= ((Expression)Item).Level) { wrap = true; }
                }
                else { wrap = true; }
            }
            //num *10^exp 52 1 
            //wrap if negative constant
            if (Item.GetType() == typeof(Constant) && Item.Value != null) { if (Item.Value < 0) { wrap = true; } }

            if (wrap) { return '(' + Item.ToString() + ')'; }

            return Item.ToString();
        }


        public override string ToString()
        {
            return this.ToString(null);
        }
        public string ToString(int? parentLevel)
        {
            StringBuilder returnString = new StringBuilder();
            int? passinLevel = parentLevel;

            if (Negative || UnderOne) { returnString.Append('('); }
            if (Negative) { returnString.Append('-'); passinLevel = null; }
            if (UnderOne) { returnString.Append("1/"); if (!Negative) { passinLevel = 1; } }

            returnString.Append(ItemToString(passinLevel));

            if (Negative || UnderOne) { returnString.Append(')'); }

            return returnString.ToString();
        }
        #endregion
    }
    class Expression : IDefinable
    {
        #region properties
        public IDefinable Definition
        {
            get { return this; }
        }
        public double? Value { get { return this.Calculate(); } }
        public List<Element> Elements { get; }
        public int Level
        {
            get
            {
                return level;
            }
            set
            {
                if (value <= maxLevel & value >= 0) { level = value; }
            }
        }
        #endregion

        #region variables
        public static readonly int maxLevel = 2;
        private int level = 0;
        #endregion

        #region constructors
        public Expression()
        {
            Elements = new List<Element>();
        }
        #endregion

        public double? Calculate()
        {
            if (Elements.Count == 0) { return null; }
            else if (Elements.Count == 1) { return Elements[0].Value; }

            double? returnValue;
            double? itemValue;
            switch (Level)
            {
                default:
                    //null
                    return null;

                case 0:
                    //Addition
                    returnValue = 0.0;
                    foreach (Element item in Elements)
                    {
                        itemValue = item.Value;
                        if (itemValue == null) { return null; }
                        returnValue += itemValue;
                    }
                    return returnValue;

                case 1:
                    //Multiplication
                    returnValue = 1.0;
                    foreach (Element item in Elements)
                    {
                        itemValue = item.Value;
                        if (itemValue == null) { return null; }
                        returnValue *= itemValue;
                    }
                    return returnValue;

                case 2:
                    //Exponent
                    returnValue = Elements[^1].Value;
                    if (returnValue == null) { return null; }

                    for (int index = Elements.Count - 2; index >= 0; index--)
                    {
                        itemValue = Elements[index].Value;
                        if (itemValue == null) { return null; }

                        returnValue = Math.Pow((double)itemValue, (double)returnValue);
                    }
                    return returnValue;
            }
        }

        public override string ToString()
        {
            int elementCount = Elements.Count;
            if (elementCount == 0)
            {
                return "null";
            }
            else if (elementCount == 1)
            {
                return Elements[0].ToString();
            }

            StringBuilder returnString = new StringBuilder();
            bool first;
            switch (Level)
            {
                default:
                    //null
                    first = true;
                    foreach (Element item in Elements)
                    {
                        if (!first) { returnString.Append(", "); }

                        returnString.Append(item.ToString(null));

                        first = false;//No code beyond this point
                    }

                    return returnString.ToString();

                case 0:
                    //Addition
                    first = true;
                    foreach (Element item in Elements)
                    {
                        int? passinLevel = Level;
                        if (first)
                        {
                            returnString.Append(item.ToString(passinLevel));
                        }
                        else
                        {
                            if (item.Negative) { returnString.Append(" - "); }
                            else { returnString.Append(" + "); }

                            if (item.UnderOne) { returnString.Append("(1/"); passinLevel = 1; }
                            returnString.Append(item.ItemToString(passinLevel));
                            if (item.UnderOne) { returnString.Append(')'); }
                        }

                        first = false;//No code beyond this point
                    }
                    return returnString.ToString();

                case 1:
                    //Miltiplication
                    first = true;
                    foreach (Element item in Elements)
                    {
                        int? passinLevel = Level;
                        if (first)
                        {
                            returnString.Append(item.ToString(passinLevel));
                        }
                        else
                        {
                            if (item.UnderOne) { returnString.Append("/"); }
                            else { returnString.Append("*"); }

                            if (item.Negative) { returnString.Append("(-"); passinLevel = null; }
                            returnString.Append(item.ItemToString(passinLevel));
                            if (item.Negative) { returnString.Append(')'); }
                        }
                        first = false;//No code beyond this point
                    }
                    return returnString.ToString();

                case 2:
                    //Exponent
                    first = true;
                    foreach (Element item in Elements)
                    {
                        if (!first) { returnString.Append('^'); }
                        returnString.Append(item.ToString(Level));
                        first = false;//No code beyond this point
                    }
                    return returnString.ToString();
            //switch
            }
        //method
        }
    //class
    }
    class FunctionContainer : IDefinable
    {
        #region properties
        public IDefinable Definition
        {
            get { return this; }
        }
        private IFunctional item;
        public IFunctional Item
        {
            get { return item; }
            set
            {
                ArgumentLinks.Clear();
                if (value != null)
                {
                    foreach (string name in value.ArgumentNames)
                    {
                        ArgumentLinks.Add(name, null);
                    }
                }
                item = value;
            }
        }
        public Dictionary<string, IDefinable> ArgumentLinks { get; private set; }
        public double? Value
        { 
            get { return Item?.Calculate(ArgumentLinks.Select(alp => alp.Value?.Value).ToArray()); }
        }
        #endregion
        #region constructors
        public FunctionContainer()
        {
            ArgumentLinks = new Dictionary<string, IDefinable>();
        }
        public FunctionContainer(IFunctional item)
            : this()
        {
            Item = item;
        }
        #endregion
        public bool PopulateArguments(IDefinable[] argumentLinks)
        {
            if (argumentLinks.Length != ArgumentLinks.Count) { return false; }
            Dictionary<string, IDefinable> newLinks = new Dictionary<string, IDefinable>();
            int i = 0;
            foreach(KeyValuePair<string, IDefinable> alp in ArgumentLinks)
            {
                newLinks.Add(alp.Key, argumentLinks[i]);
                i++;
            }
            ArgumentLinks = newLinks;
            return true;
        }
        public override string ToString()
        {
            if (Item == null) { return "null()"; }
            StringBuilder sb = new StringBuilder(Item.Name);
            sb.Append('(');

            {
                bool first = true;
                foreach(KeyValuePair<string, IDefinable> arg in ArgumentLinks)
                {
                    if (!first) { sb.Append(", "); }
                    sb.Append(arg.Value.ToString());
                    first = false;
                }
            }
            sb.Append(')');
            return sb.ToString();
        }
    }
}