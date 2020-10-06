using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MathParser
{
    class NonExistentFunction : Exception
    {
        public NonExistentFunction()
        {

        }
        public NonExistentFunction(string message)
            : base(message)
        {

        }
    }
    class Program
    {
        public static readonly char[] openBrackets = new char[] { '(', '[', '{', '<' };
        public static readonly char[] closeBrackets = new char[] { ')', ']', '}', '>' };
        public static readonly char[][] lvls = new char[][]
        {
            new char[] { '+', '-' },
            new char[] { '*', '/' },
            new char[] { '^' }
        };

        public static Tuple<string, int> fkey(string text)
        {
            text = text.Trim();
            if (text.Length <= 1) { return new Tuple<string, int>("", 0); }

            int? openIndex = null;
            int? closeIndex = null;

            for (int i = 0; i < text.Length; i++)
            {
                if (openBrackets.Contains(text[i]) & openIndex == null)
                {
                    openIndex = i;
                }
                if (closeBrackets.Contains(text[i]) & closeIndex == null)
                {
                    closeIndex = i;
                }
            }
            string name;
            string args;
            name = openIndex != null ? text[0..((int)openIndex)] : text;
            args = openIndex != null & closeIndex != null ? text[((int)openIndex + 1)..((int)closeIndex)] : "";
            int argcount = args.Split(',').Length;

            return new Tuple<string, int>(name, argcount);
        }

        public static string[] SmartSplit(string text, char[] deliminators)
        {
            return SmartSplit(text, deliminators, openBrackets, closeBrackets);
        }
        public static string[] SmartSplit(string text, char deliminator, char levelin, char levelout)
        {
            return SmartSplit(text, new char[] { deliminator }, new char[] { levelin }, new char[] { levelout });
        }
        public static string[] SmartSplit(string text, char[] deliminators, char[] levelins, char[] levelouts)
        {
            List<string> arrayBuilder = new List<string>();
            StringBuilder cache = new StringBuilder();
            int level = 0;
            void BreakCache()
            {
                if (cache.Length != 0) { arrayBuilder.Add(cache.ToString()); }
                
                cache.Clear();
            }
            void Append(char character) { cache.Append(character); }

            foreach (char character in text)
            {
                bool dontAppend = false;
                if (levelins.Contains(character)) { level++; }
                if(level == 0 & deliminators.Contains(character))
                {
                    BreakCache();
                    dontAppend = true;
                }
                if (levelouts.Contains(character)) { level--; }

                if (!dontAppend) { Append(character); }
            }
            BreakCache();
            return arrayBuilder.ToArray();
        }

        public static bool IsWrapped(string text)
        {
            return IsWrapped(text, new char[] { '(' }, new char[] { ')' });
        }
        public static bool IsWrapped(string text, char[] openBrackets, char[] closeBrackets)
        {

            if (text.Length < 2) { return false; }

            if (!(openBrackets.Contains(text[0]) & closeBrackets.Contains(text[^1]))) { return false; }

            //(* = not 0): depth pattern for a wrapped string "1*..0"
            {
                int depth = 0;

                for (int i = 0; i < text.Length; i++)
                {
                    if (openBrackets.Contains(text[i])) { depth++; }
                    if (closeBrackets.Contains(text[i])) { depth--; }

                    if (i == 0)
                    {
                        //^first character
                        if (depth != 1) { return false; }
                    }
                    else if (i == text.Length - 1)
                    {
                        //^last character
                        if (depth != 0) { return false; }
                    }
                    else
                    {
                        if (depth == 0) { return false; }
                        //^not first or last, middle character
                    }
                }
                //^ past test
                return true;
            }
        }

        ////public static bool IsWrapped(string text)
        ////{
        ////    ////check0
        ////    //if (text.Length < 2) { return false; }
        ////    //// What makes a string wrapped?
        ////    ////  If the text within is wrapped in parenthasis...
        ////    ////  Here is an exaple: "(hello)".
        ////    ////  *note*: The string is not wrapped if it starts or ends with white-space, i.e. " (hello) ". Trim your text before sending to this method.
        ////    //bool check1 = false;
        ////    //bool check2 = false;
        ////    //bool check3 = false;

        ////    //#region check1
        ////    //// "(*****...     first character is open bracket
        ////    //check1 = openBrackets.Contains(text[0]);
        ////    //#endregion

        ////    //#region check2
        ////    //// ...*****)"     last characer is close bracket
        ////    //check2 = closeBrackets.Contains(text[^1]);
        ////    //#endregion

        ////    //#region check3
        ////    //// 0(1(2)1(2(3)2(3)2)1)0:yes   0(1)0(1)0:no  Level does not reach zero before last character
        ////    //{
        ////    //    bool testPassed = true;
        ////    //    int level = 0;
        ////    //    char character;
        ////    //    for(int i = 0; i > text.Length; i++)
        ////    //    {
        ////    //        character = text[i];
        ////    //        if (openBrackets.Contains(character)) { level++; }
        ////    //        if (closeBrackets.Contains(character)) { level--; }

        ////    //        if (i != text.Length - 1)
        ////    //        {
        ////    //            // The loop is not at the last character of the string
        ////    //            if (level == 0)
        ////    //            {
        ////    //                // If the string is wrapped, then it shouldn't reach level 0 until the last character.
        ////    //                testPassed = false;
        ////    //                break;
        ////    //            }
        ////    //        }
        ////    //    }
        ////    //    // TestPassed will still be true if the nested if statement in the loop never trips.
        ////    //    check3 = testPassed;
        ////    }
        //    #endregion

        //    // If the string passes all 3 checks, then it is wrapped.
        //    return check1 & check2 & check3;
        //}
        public static bool IsFunction(string text)
        {
            //check0
            if (text.Length < 3) { return false; }
            // Is it a function?
            bool check1;
            bool check2;
            bool check3;
            check1 = closeBrackets.Contains(text[^1]);
            check2 = !openBrackets.Contains(text[0]);
            #region check3
            check3 = false;
            foreach (char character in text)
            {
                if (openBrackets.Contains(character))
                {
                    check3 = true;
                    break;
                }
            }
            #endregion
            return check1 & check2 & check3;
        }
        public static IDefinable ParseDefinable(string text, Dictionary<string, Variable> variableIndex, Dictionary<Tuple<string, int>, IFunctional> functionIndex)
        {
            return ParseDefinable(text, variableIndex, functionIndex, new Dictionary<string, Variable>());
        }
        public static IDefinable ParseDefinable(string text, Dictionary<string, Variable> variableIndex, Dictionary<Tuple<string, int>, IFunctional> functionIndex, Dictionary<string, Variable> localVariableIndex)
        {
            // Take care of white-space.
            text = text.Trim();
            // Take care of wrapping parenthesis, i.e. "(1+2)" -> "1+2".
            while (IsWrapped(text))
            {
                text = text[1..^1];
            }

            #region stuff to keep track of
            bool isExpression = false;
            int expressionLevel = 0;
            string[] expressionParts = null;
            bool[] negatives = null;
            bool[] underOnes = null;

            bool isFunction = false;
            string functionName = null;
            IDefinable[] argumentLinks = null;
            #endregion
            #region is it an Expression, Function, or neither.
            for (int i = 0; i < lvls.Length; i++)
            {
                isExpression = SmartSplit(text, lvls[i]).Length > 1;
                if (isExpression)
                {
                    expressionLevel = i;

                    // I'm gonna make my own smart split. With black jack and hookers (and two arrays that store whether each element is negative or under one)!
                    {
                        List<bool> negativesBuilder = new List<bool>();
                        List<bool> underOnesBuilder = new List<bool>();
                        List<string> arrayBuilder = new List<string>();
                        bool nextIsNeg = false;
                        bool nextIsUnderOne = false;
                        StringBuilder cache = new StringBuilder();
                        int level = 0;
                        void BreakCache()
                        {
                            if (cache.Length != 0)
                            {
                                arrayBuilder.Add(cache.ToString());
                                negativesBuilder.Add(nextIsNeg);
                                underOnesBuilder.Add(nextIsUnderOne);
                            }
                            cache.Clear();
                        }
                        void Append(char character) { cache.Append(character); }

                        foreach (char character in text)
                        {
                            bool dontAppend = false;
                            if (openBrackets.Contains(character)) { level++; }

                            if (level == 0 & lvls[i].Contains(character))
                            {
                                BreakCache();
                                nextIsNeg = character == lvls[0][1];
                                nextIsUnderOne = character == lvls[1][1];
                                dontAppend = true;
                            }

                            if (closeBrackets.Contains(character)) { level--; }

                            if (!dontAppend) { Append(character); }
                        }
                        BreakCache();
                        expressionParts = arrayBuilder.ToArray();
                        negatives = negativesBuilder.ToArray();
                        underOnes = underOnesBuilder.ToArray();
                    }
                    break;
                }
            }

            if (!isExpression)
            {
                isFunction = IsFunction(text);
            }
            #endregion

            // Now lets get down to creating these IDefinable things.
            if (isExpression)
            {
                Expression newExpression = new Expression() { Level = expressionLevel };

                {
                    int i = 0;
                    foreach(string part in expressionParts)
                    {
                        newExpression.Elements.Add(new Element(ParseDefinable(part, variableIndex, functionIndex, localVariableIndex))
                        {
                            Negative = negatives[i],
                            UnderOne = underOnes[i]
                        });
                        i++;
                    }
                }
                return newExpression;
            }
            else if (isFunction)
            {
                int openIndex = 0;
                {
                    for(int i = 0; i < text.Length; i++)
                    {
                        if (openBrackets.Contains(text[i]))
                        {
                            openIndex = i;
                            break;
                        }
                    }
                }
                argumentLinks = (text[(openIndex + 1)..^1].Split(',', StringSplitOptions.RemoveEmptyEntries)).Select(at => ParseDefinable(at, variableIndex, functionIndex, localVariableIndex)).ToArray();
                functionName = text[0..openIndex];

                IFunctional existingFunction = null;
                if (!functionIndex.TryGetValue(new Tuple<string, int>(functionName, argumentLinks.Length), out existingFunction))
                {
                    throw new NonExistentFunction($"{functionName} with {argumentLinks.Length}");
                }

                FunctionContainer newFunctionContainer = new FunctionContainer();

                newFunctionContainer.Item = existingFunction;
                newFunctionContainer.PopulateArguments(argumentLinks);

                return newFunctionContainer;
                
            }
            else
            {
                // It's iether a constant or a variable.
                double constantValue;
                Variable existingVariable;
                if (text.Length == 0)
                {
                    return new Constant(null);
                }
                else if (double.TryParse(text, out constantValue))
                {
                    return new Constant(constantValue);
                }
                else if (localVariableIndex.TryGetValue(text, out existingVariable))
                {
                    return existingVariable;
                }
                else if (variableIndex.TryGetValue(text, out existingVariable))
                {
                    return existingVariable;
                }
                else
                {
                    Variable newVariable = new Variable(text);
                    variableIndex.Add(newVariable.Name, newVariable);
                    return newVariable;
                }
            }
        }
        static void Main(string[] args)
        {
            string input;
            IDefinable newDefinable;
            Dictionary<string, Variable> varIndex = new Dictionary<string, Variable>();
            Dictionary<Tuple<string, int>, IFunctional> funcIndex = new Dictionary<Tuple<string, int>, IFunctional>();
            varIndex.Add("the meaning of life", new Variable("the meaning of life") { Value = 42 });
            

            #region built in functions
            {
                List<IFunctional> funcs = new List<IFunctional>();
                funcs.Add(new Sin());
                funcs.Add(new Cos());
                funcs.Add(new Tan());
                funcs.Add(new Arcsin());
                funcs.Add(new Arccos());
                funcs.Add(new Arctan());
                funcs.Add(new Rad());
                funcs.Add(new Deg());
                funcs.Add(new Abs());
                funcs.Add(new Sign());
                funcs.Add(new Floor());
                funcs.Add(new Ceiling());
                funcs.Add(new Round());
                funcs.Add(new E());
                funcs.Add(new Pi());

                foreach (IFunctional func in funcs)
                {
                    funcIndex.Add(
                        new Tuple<string, int>(func.Name, func.ArgumentNames.Length),
                        func);
                }
            }
            #endregion

            while (true)
            {
                input = Console.ReadLine();
                if (input.Contains(":="))
                {
                    string[] input_split = input.Split(":=");
                    input_split[0] = input_split[0].Trim();
                    input_split[1] = input_split[1].Trim();
                    if (IsFunction(input_split[0].Trim()))
                    {

                    }
                    else
                    {
                        Variable existingVar;
                        if (varIndex.TryGetValue(input_split[0].Trim(), out existingVar))
                        {
                            existingVar.Link = ParseDefinable(input_split[1], varIndex, funcIndex);
                        }
                        else
                        {
                            existingVar = new Variable(input_split[0].Trim());
                            existingVar.Link = ParseDefinable(input_split[1], varIndex, funcIndex);
                            varIndex.Add(existingVar.Name, existingVar);
                        }
                    }
                }
                else if (input.Contains("="))
                {
                    string[] input_split = input.Split("=");
                    input_split[0] = input_split[0].Trim();
                    input_split[1] = input_split[1].Trim();
                    if (IsFunction(input_split[0].Trim()))
                    {
                        int openIndex = 0;
                        for (int i = 0; i < input_split[0].Length; i++)
                        {
                            if (openBrackets.Contains(input_split[0][i])) { openIndex = i; break; }
                        }

                        string name = input_split[0][0..openIndex];
                        string[] argNames = input_split[0][(openIndex + 1)..^1].Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
                        Dictionary<string, Variable> arguments = new Dictionary<string, Variable>();
                        foreach(string argName in argNames)
                        {
                            arguments.Add(argName, new Variable(argName));
                        }
                        IDefinable behavior = ParseDefinable(input_split[1], varIndex, funcIndex, arguments);

                        funcIndex.Remove(new Tuple<string, int>(name, argNames.Length));
                        funcIndex.Add(new Tuple<string, int>(name, argNames.Length), new Function(arguments, behavior));
                    }
                    else
                    {
                        Variable existingVar;
                        if (varIndex.TryGetValue(input_split[0].Trim(), out existingVar))
                        {
                            existingVar.Value = ParseDefinable(input_split[1], varIndex, funcIndex).Value;
                        }
                        else
                        {
                            existingVar = new Variable(input_split[0].Trim());
                            existingVar.Value = ParseDefinable(input_split[1], varIndex, funcIndex).Value;
                            varIndex.Add(existingVar.Name, existingVar);
                        }
                    }
                }
                else if (input.Length > 1 ? input[0] == '$' : false)
                {
                    string input_sub = input[1..].Trim();
                    Variable existingVar;
                    IFunctional existingFunc;
                    string definition = "";
                    bool varOfuncNfound = false;
                    if (varIndex.TryGetValue(input_sub, out existingVar))
                    {
                        definition = $"{existingVar.Definition}";
                    }
                    else if(funcIndex.TryGetValue(fkey(input_sub), out existingFunc))
                    {
                        definition = $"{existingFunc.ToString()} = {existingFunc.Definition}";
                    }
                    else
                    {
                        varOfuncNfound = true;
                    }
                    if (varOfuncNfound)
                    {
                        Console.WriteLine("    Variable or Function not found");
                    }
                    else
                    {
                        Console.WriteLine("    " + definition);
                    }
                }
                {
                    try
                    {
                        newDefinable = ParseDefinable(input, varIndex, funcIndex);
                        Console.WriteLine(newDefinable.Value);
                    }
                    catch(NonExistentFunction e)
                    {
                        Console.WriteLine($"!!Non existant function: {e.Message} arguments");
                    }
                }
            }
        }
    }
}