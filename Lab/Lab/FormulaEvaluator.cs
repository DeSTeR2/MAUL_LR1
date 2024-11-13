using System.Text.RegularExpressions;

namespace Lab
{
    public static class FormulaEvaluator
    {
        private static readonly string Pattern = @"^(\s*({[A-Za-z]+\d+}|\d+|inc|dec|not|\^|\+|\-|\*|\/|=|>|<|\(|\)|\s)+\s*)$";
        private static readonly Regex TokenizerRegex = new Regex(@"({[A-Za-z]+\d+}|\d+|inc|dec|not|\^|\+|\-|\*|\/|=|>|<|\(|\))", RegexOptions.Compiled);

        public static bool IsCalculable(string formula, ref List<string> errors)
        {
            if (formula == null || formula == "")
            {
                return false;
            }

            if (!Regex.IsMatch(formula, Pattern))
            {
                errors.Add("Formula contains invalid characters or syntax.");
                return false;
            }

            try
            {
                Stack<string> stack = new Stack<string>();
                string[] tokens = Tokenize(formula);
                bool expectOperand = true;

                foreach (string token in tokens)
                {
                    if (IsOperator(token))
                    {
                        if (expectOperand)
                        {
                            errors.Add($"Unexpected operator '{token}' at position {formula.IndexOf(token)}.");
                            return false;
                        }
                        expectOperand = true;
                        stack.Push(token);
                    }
                    else if (token == "(")
                    {
                        stack.Push(token);
                        expectOperand = true;
                    }
                    else if (token == ")")
                    {
                        while (stack.Count > 0 && stack.Peek() != "(")
                        {
                            stack.Pop();
                        }
                        if (stack.Count == 0 || stack.Peek() != "(")
                        {
                            errors.Add("Unmatched closing parenthesis.");
                            return false;
                        }
                        stack.Pop();
                        expectOperand = false;
                    }
                    else if (IsNumber(token) || IsCellReference(token))
                    {
                        if (!expectOperand)
                        {
                            errors.Add($"Unexpected operand '{token}' at position {formula.IndexOf(token)}.");
                            return false;
                        }
                        expectOperand = false;
                    }
                    else if (token == "inc" || token == "dec" || token == "not")
                    {
                        if (!expectOperand)
                        {
                            errors.Add($"Unexpected function '{token}' at position {formula.IndexOf(token)}.");
                            return false;
                        }
                        stack.Push(token);
                        expectOperand = true;
                    }
                    else
                    {
                        errors.Add($"Invalid token '{token}' in formula.");
                        return false;
                    }
                }

                if (stack.Contains("("))
                {
                    errors.Add("Unmatched opening parenthesis.");
                    return false;
                }

                return !expectOperand;
            }
            catch (Exception ex)
            {
                errors.Add($"Error parsing formula: {ex.Message}");
                return false;
            }
        }

        public static float Evaluate(string formula, Dictionary<string, float> cellValues, ref List<string> errors)
        {
            errors = new List<string>();

            if (!IsCalculable(formula, ref errors))
            {
                return float.NaN;
            }

            try
            {
                Stack<float> values = new Stack<float>();
                Stack<string> operators = new Stack<string>();
                string[] tokens = Tokenize(formula);

                foreach (string token in tokens)
                {
                    if (IsNumber(token))
                    {
                        values.Push(float.Parse(token));
                    }
                    else if (IsCellReference(token))
                    {
                        if (cellValues.TryGetValue(token, out float cellValue))
                            values.Push(cellValue);
                        else
                            throw new Exception($"Undefined cell reference {token}");
                    }
                    else if (token == "inc" || token == "dec" || token == "not")
                    {
                        operators.Push(token);
                    }
                    else if (IsOperator(token))
                    {
                        while (operators.Count > 0 && Precedence(operators.Peek()) >= Precedence(token))
                            ApplyOperator(operators.Pop(), values);
                        operators.Push(token);
                    }
                    else if (token == "(")
                    {
                        operators.Push(token);
                    }
                    else if (token == ")")
                    {
                        while (operators.Peek() != "(")
                            ApplyOperator(operators.Pop(), values);
                        operators.Pop();
                    }
                }

                while (operators.Count > 0)
                    ApplyOperator(operators.Pop(), values);

                return values.Pop();
            }
            catch (Exception ex)
            {
                errors.Add($"Evaluation error: {ex.Message}");
                return float.NaN;
            }
        }

        public static List<string> GetAllCellLinks(string formula)
        {
            string[] tokens = Tokenize(formula);
            List<string> links = new List<string>();
            foreach (string token in tokens)
            {
                if (IsCellReference(token))
                {
                    links.Add(token);
                }
            }

            return links;
        }

        private static string[] Tokenize(string formula)
        {
            try
            {
                string[] res = TokenizerRegex.Matches(formula)
                                     .Cast<Match>()
                                     .Select(match => match.Value)
                                     .ToArray();
                return res;
            }
            catch
            {
                return [""];
            }
        }

        private static bool IsOperator(string token) => "+-*/=><^".Contains(token);

        private static bool IsNumber(string token) => float.TryParse(token, out _);

        private static bool IsCellReference(string token) => Regex.IsMatch(token, @"^{[A-Za-z]+\d+}$");

        private static int Precedence(string op)
        {
            return op switch
            {
                "+" or "-" => 1,
                "*" or "/" => 2,
                "^" => 3,
                "=" or ">" or "<" => 0,
                "inc" or "dec" or "not" => 4,  // Give high precedence for unary operators
                _ => -1
            };
        }

        private static void ApplyOperator(string op, Stack<float> values)
        {
            if (op == "inc" || op == "dec" || op == "not")
            {
                float val = values.Pop();
                float result = op switch
                {
                    "inc" => val + 1,
                    "dec" => val - 1,
                    "not" => val == 0 ? 1 : 0,
                    _ => throw new Exception($"Unsupported operator: {op}")
                };
                values.Push(result);
            }
            else
            {
                float right = values.Pop();
                float left = values.Pop();
                float result = op switch
                {
                    "+" => left + right,
                    "-" => left - right,
                    "*" => left * right,
                    "/" => left / right,
                    "=" => left == right ? 1 : 0,
                    ">" => left > right ? 1 : 0,
                    "<" => left < right ? 1 : 0,
                    "^" => (float)Math.Pow(left, right),
                    _ => throw new Exception($"Unsupported operator: {op}")
                };
                values.Push(result);
            }
        }
    }
}