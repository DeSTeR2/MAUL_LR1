﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public static class FormulaEvaluator
{
    private static readonly string Pattern = @"^(\s*({[A-Za-z]+\d+}|\d+|inc|dec|not|\^|\+|\-|\*|\/|=|>|<|\(|\)|\s)+\s*)$";
    private static readonly Regex TokenizerRegex = new Regex(@"({[A-Za-z]+\d+}|\d+|inc|dec|not|\^|\+|\-|\*|\/|=|>|<|\(|\))", RegexOptions.Compiled);

    public static bool IsCalculable(string formula, out List<string> errors)
    {
        errors = new List<string>();

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

    public static double Evaluate(string formula, Dictionary<string, double> cellValues, out List<string> errors)
    {
        errors = new List<string>();
        if (!IsCalculable(formula, out errors))
        {
            errors.Add("Formula is not calculable.");
            return double.NaN;
        }

        try
        {
            Stack<double> values = new Stack<double>();
            Stack<string> operators = new Stack<string>();
            string[] tokens = Tokenize(formula);

            foreach (string token in tokens)
            {
                if (IsNumber(token))
                {
                    values.Push(double.Parse(token));
                }
                else if (IsCellReference(token))
                {
                    if (cellValues.TryGetValue(token, out double cellValue))
                        values.Push(cellValue);
                    else
                        throw new Exception($"Undefined cell reference {token}");
                }
                else if (token == "inc")
                {
                    if (values.TryPop(out double val))
                        values.Push(val + 1);
                }
                else if (token == "dec")
                {
                    if (values.TryPop(out double val))
                        values.Push(val - 1);
                }
                else if (token == "not")
                {
                    if (values.TryPop(out double val))
                        values.Push(val == 0 ? 1 : 0);
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
            return double.NaN;
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
        return TokenizerRegex.Matches(formula)
                             .Cast<Match>()
                             .Select(match => match.Value)
                             .ToArray();
    }

    private static bool IsOperator(string token) => "+-*/=><^".Contains(token);

    private static bool IsNumber(string token) => double.TryParse(token, out _);

    private static bool IsCellReference(string token) => Regex.IsMatch(token, @"^{[A-Za-z]+\d+}$");

    private static int Precedence(string op)
    {
        return op switch
        {
            "+" or "-" => 1,
            "*" or "/" => 2,
            "^" => 3,
            "=" or ">" or "<" => 0,
            _ => -1
        };
    }

    private static void ApplyOperator(string op, Stack<double> values)
    {
        double right = values.Pop();
        double left = values.Pop();
        double result = op switch
        {
            "+" => left + right,
            "-" => left - right,
            "*" => left * right,
            "/" => left / right,
            "=" => left == right ? 1 : 0,
            ">" => left > right ? 1 : 0,
            "<" => left < right ? 1 : 0,
            "^" => Math.Pow(left, right),
            _ => throw new Exception($"Unsupported operator: {op}")
        };
        values.Push(result);
    }
}