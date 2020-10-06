using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MathParser
{
    public interface IFunctional : IDefined
    {
        public string Name { get; }
        public string[] ArgumentNames { get; }
        public IDefinable Behavior { get; }
        public double? Calculate(double?[] argumentValues);
        public double? Calculate(Dictionary<string, double?> argumentValues);
    }
    class Function : IFunctional
    {
        public IDefinable Definition
        {
            get { return Behavior; }
        }
        public string Name { get; }
        public string[] ArgumentNames { get { return Arguments.Keys.ToArray(); } }
        public Dictionary<string, Variable> Arguments { get; }
        public IDefinable Behavior { get; }
        public double? Calculate(double?[] argumentValues)
        {
            if (argumentValues.Length != ArgumentNames.Length) { return null; }
            Dictionary<string, double?> argdict = new Dictionary<string, double?>();
            int i = 0;
            foreach(double? val in argumentValues)
            {
                argdict.Add(ArgumentNames[i], val);
                i++;
            }
            return Calculate(argdict);
        }
        public double? Calculate(Dictionary<string, double?> argumentValues)
        {
            List<Variable> args = new List<Variable>();
            List<double?> vals = new List<double?>();
            foreach (KeyValuePair<string, double?> avp in argumentValues)
            {
                Variable arg;
                if (Arguments.TryGetValue(avp.Key, out arg))
                {
                    args.Add(arg);
                    vals.Add(avp.Value);
                }
                else
                {
                    //abort
                    return null;
                }
            }
            int i = 0;
            foreach (Variable arg in args)
            {
                arg.Value = vals[i];
                i++;
            }
            return Behavior.Value;
        }
        public Function(Dictionary<string, Variable> arguments, IDefinable behavior)
        {
            Arguments = arguments;
            Behavior = behavior;
        }
        public Function(Variable[] arguments, IDefinable behavior)
            : this(arguments.ToDictionary(a => a.Name), behavior)
        { }
        public override string ToString()
        {
            return Name;
        }
    }
    #region built in functions
    abstract class BuiltInFunction : IFunctional
    {
        public IDefinable Definition
        {
            get { return Behavior; }
        }
        public abstract string Name { get; }
        public abstract string[] ArgumentNames { get; }
        public abstract IDefinable Behavior { get; }
        public double? Calculate(double?[] argumentValues)
        {
            if (argumentValues.Length != ArgumentNames.Length) { return null; }
            Dictionary<string, double?> argdic = new Dictionary<string, double?>();
            int i = 0;
            foreach(double? value in argumentValues)
            {
                argdic.Add(ArgumentNames[i], value);
                i++;
            }
            return Calculate(argdic);
        }
        public abstract double? Calculate(Dictionary<string, double?> argumentValues);
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Name);
            sb.Append('(');

            {
                bool first = true;
                foreach(string argname in ArgumentNames)
                {
                    if (!first) { sb.Append(", "); }
                    sb.Append(argname);

                    first = false;
                }
            }
            sb.Append(')');
            return sb.ToString();
        }
    }
    class Sin : BuiltInFunction
    {
        public override string Name { get { return "sin"; } }
        public override string[] ArgumentNames { get { return new string[] { "x" }; } }
        public override IDefinable Behavior
        {
            get
            {
                FunctionContainer nfc = new FunctionContainer();
                nfc.Item = new Sin();
                return nfc;
            }
        }
        public override double? Calculate(Dictionary<string, double?> argumentValues)
        {
            double? argValue = null;
            if (argumentValues.Count == 1 & argumentValues.TryGetValue(ArgumentNames[0], out argValue))
            {
                return argValue == null ? null : (double?)Math.Sin((double)argValue);
            }
            return null;
        }
    }
    class Cos : BuiltInFunction
    {
        public override string Name { get { return "cos"; } }
        public override string[] ArgumentNames { get { return new string[] { "x" }; } }
        public override IDefinable Behavior
        {
            get
            {
                FunctionContainer nfc = new FunctionContainer();
                nfc.Item = new Cos();
                return nfc;
            }
        }
        public override double? Calculate(Dictionary<string, double?> argumentValues)
        {
            double? argValue = null;
            if (argumentValues.Count == 1 & argumentValues.TryGetValue(ArgumentNames[0], out argValue))
            {
                return argValue == null ? null : (double?)Math.Cos((double)argValue);
            }
            return null;
        }
    }
    class Tan : BuiltInFunction
    {
        public override string Name { get { return "tan"; } }
        public override string[] ArgumentNames { get { return new string[] { "x" }; } }
        public override IDefinable Behavior
        {
            get
            {
                FunctionContainer nfc = new FunctionContainer();
                nfc.Item = new Tan();
                return nfc;
            }
        }
        public override double? Calculate(Dictionary<string, double?> argumentValues)
        {
            double? argValue = null;
            if (argumentValues.Count == 1 & argumentValues.TryGetValue(ArgumentNames[0], out argValue))
            {
                return argValue == null ? null : (double?)Math.Tan((double)argValue);
            }
            return null;
        }
    }
    class Arcsin : BuiltInFunction
    {
        public override string Name { get { return "asin"; } }
        public override string[] ArgumentNames { get { return new string[] { "x" }; } }
        public override IDefinable Behavior
        {
            get
            {
                FunctionContainer nfc = new FunctionContainer();
                nfc.Item = new Arcsin();
                return nfc;
            }
        }
        public override double? Calculate(Dictionary<string, double?> argumentValues)
        {
            double? argValue = null;
            if (argumentValues.Count == 1 & argumentValues.TryGetValue(ArgumentNames[0], out argValue))
            {
                return argValue == null ? null : (double?)Math.Asin((double)argValue);
            }
            return null;
        }
    }
    class Arccos : BuiltInFunction
    {
        public override string Name { get { return "acos"; } }
        public override string[] ArgumentNames { get { return new string[] { "x" }; } }
        public override IDefinable Behavior
        {
            get
            {
                FunctionContainer nfc = new FunctionContainer();
                nfc.Item = new Arccos();
                return nfc;
            }
        }
        public override double? Calculate(Dictionary<string, double?> argumentValues)
        {
            double? argValue = null;
            if (argumentValues.Count == 1 & argumentValues.TryGetValue(ArgumentNames[0], out argValue))
            {
                return argValue == null ? null : (double?)Math.Acos((double)argValue);
            }
            return null;
        }
    }
    class Arctan : BuiltInFunction
    {
        public override string Name { get { return "atan"; } }
        public override string[] ArgumentNames { get { return new string[] { "x" }; } }
        public override IDefinable Behavior
        {
            get
            {
                FunctionContainer nfc = new FunctionContainer();
                nfc.Item = new Arctan();
                return nfc;
            }
        }
        public override double? Calculate(Dictionary<string, double?> argumentValues)
        {
            double? argValue = null;
            if (argumentValues.Count == 1 & argumentValues.TryGetValue(ArgumentNames[0], out argValue))
            {
                return argValue == null ? null : (double?)Math.Atan((double)argValue);
            }
            return null;
        }
    }
    class Rad : BuiltInFunction
    {
        public override string Name { get { return "rad"; } }
        public override string[] ArgumentNames { get { return new string[] { "x" }; } }
        public override IDefinable Behavior
        {
            get
            {
                FunctionContainer nfc = new FunctionContainer();
                nfc.Item = new Rad();
                return nfc;
            }
        }
        public override double? Calculate(Dictionary<string, double?> argumentValues)
        {
            double? argValue = null;
            if (argumentValues.Count == 1 & argumentValues.TryGetValue(ArgumentNames[0], out argValue))
            {
                return argValue * (Math.PI / 180.0);
            }
            return null;
        }
    }
    class Deg : BuiltInFunction
    {
        public override string Name { get { return "deg"; } }
        public override string[] ArgumentNames { get { return new string[] { "x" }; } }
        public override IDefinable Behavior
        {
            get
            {
                FunctionContainer nfc = new FunctionContainer();
                nfc.Item = new Deg();
                return nfc;
            }
        }
        public override double? Calculate(Dictionary<string, double?> argumentValues)
        {
            double? argValue = null;
            if (argumentValues.Count == 1 & argumentValues.TryGetValue(ArgumentNames[0], out argValue))
            {
                return argValue * (180.0 / Math.PI);
            }
            return null;
        }
    }
    class Abs : BuiltInFunction
    {
        public override string Name { get { return "abs"; } }
        public override string[] ArgumentNames { get { return new string[] { "x" }; } }
        public override IDefinable Behavior
        {
            get
            {
                FunctionContainer nfc = new FunctionContainer();
                nfc.Item = new Abs();
                return nfc;
            }
        }
        public override double? Calculate(Dictionary<string, double?> argumentValues)
        {
            double? argValue = null;
            if (argumentValues.Count == 1 & argumentValues.TryGetValue(ArgumentNames[0], out argValue))
            {
                return argValue == null ? null : (double?)Math.Abs((double)argValue);
            }
            return null;
        }
    }
    class Sign : BuiltInFunction
    {
        public override string Name { get { return "sign"; } }
        public override string[] ArgumentNames { get { return new string[] { "x" }; } }
        public override IDefinable Behavior
        {
            get
            {
                FunctionContainer nfc = new FunctionContainer();
                nfc.Item = new Sign();
                return nfc;
            }
        }
        public override double? Calculate(Dictionary<string, double?> argumentValues)
        {
            double? argValue = null;
            if (argumentValues.Count == 1 & argumentValues.TryGetValue(ArgumentNames[0], out argValue))
            {
                return argValue == null ? null : (double?)Math.Sign((double)argValue);
            }
            return null;
        }
    }
    class Floor : BuiltInFunction
    {
        public override string Name { get { return "floor"; } }
        public override string[] ArgumentNames { get { return new string[] { "x" }; } }
        public override IDefinable Behavior
        {
            get
            {
                FunctionContainer nfc = new FunctionContainer();
                nfc.Item = new Floor();
                return nfc;
            }
        }
        public override double? Calculate(Dictionary<string, double?> argumentValues)
        {
            double? argValue = null;
            if (argumentValues.Count == 1 & argumentValues.TryGetValue(ArgumentNames[0], out argValue))
            {
                return argValue == null ? null : (double?)Math.Floor((double)argValue);
            }
            return null;
        }
    }
    class Ceiling : BuiltInFunction
    {
        public override string Name { get { return "ceiling"; } }
        public override string[] ArgumentNames { get { return new string[] { "x" }; } }
        public override IDefinable Behavior
        {
            get
            {
                FunctionContainer nfc = new FunctionContainer();
                nfc.Item = new Ceiling();
                return nfc;
            }
        }
        public override double? Calculate(Dictionary<string, double?> argumentValues)
        {
            double? argValue = null;
            if (argumentValues.Count == 1 & argumentValues.TryGetValue(ArgumentNames[0], out argValue))
            {
                return argValue == null ? null : (double?)Math.Ceiling((double)argValue);
            }
            return null;
        }
    }
    class Round : BuiltInFunction
    {
        public override string Name { get { return "round"; } }
        public override string[] ArgumentNames { get { return new string[] { "x" }; } }
        public override IDefinable Behavior
        {
            get
            {
                FunctionContainer nfc = new FunctionContainer();
                nfc.Item = new Round();
                return nfc;
            }
        }
        public override double? Calculate(Dictionary<string, double?> argumentValues)
        {
            double? argValue = null;
            if (argumentValues.Count == 1 & argumentValues.TryGetValue(ArgumentNames[0], out argValue))
            {
                return argValue == null ? null : (double?)Math.Round((double)argValue);
            }
            return null;
        }
    }
    class E : BuiltInFunction
    {
        public override string Name { get { return "e"; } }
        public override string[] ArgumentNames { get { return new string[0]; } }
        public override IDefinable Behavior
        {
            get
            {
                FunctionContainer nfc = new FunctionContainer();
                nfc.Item = new E();
                return nfc;
            }
        }
        public override double? Calculate(Dictionary<string, double?> argumentValues)
        {
            if (argumentValues.Count == 0) { return Math.E; }
            else { return null; }
        }
    }
    class Pi : BuiltInFunction
    {
        public override string Name { get { return "pi"; } }
        public override string[] ArgumentNames { get { return new string[0]; } }
        public override IDefinable Behavior
        {
            get
            {
                FunctionContainer nfc = new FunctionContainer();
                nfc.Item = new Pi();
                return nfc;
            }
        }
        public override double? Calculate(Dictionary<string, double?> argumentValues)
        {
            if (argumentValues.Count == 0) { return Math.PI; }
            else { return null; }
        }
    }
    #endregion
}