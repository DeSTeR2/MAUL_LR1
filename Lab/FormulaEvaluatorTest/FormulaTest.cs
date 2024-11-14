using Lab;

namespace FormulaEvaluatorTest
{
    public class FormulaTest
    {
        [Fact]
        public void IsCalculable_ValidFormula_ReturnsTrue()
        {
            List<string> errors = new List<string>();
            bool result = FormulaEvaluator.IsCalculable("5 + 3", ref errors);
            Assert.True(result);
            Assert.Empty(errors);
        }

        [Fact]
        public void IsCalculable_InvalidFormulaCharacters_ReturnsFalse()
        {
            List<string> errors = new List<string>();
            bool result = FormulaEvaluator.IsCalculable("5 + 3 &", ref errors);
            Assert.False(result);
            Assert.NotEmpty(errors);
        }

        [Fact]
        public void IsCalculable_EmptyFormula_ReturnsFalse()
        {
            List<string> errors = new List<string>();
            bool result = FormulaEvaluator.IsCalculable("", ref errors);
            Assert.False(result);
            Assert.Empty(errors);
        }

        [Fact]
        public void IsCalculable_NullFormula_ReturnsFalse()
        {
            List<string> errors = new List<string>();
            bool result = FormulaEvaluator.IsCalculable(null, ref errors);
            Assert.False(result);
            Assert.Empty(errors);
        }

        [Fact]
        public void IsCalculable_UnmatchedParenthesis_ReturnsFalse()
        {
            List<string> errors = new List<string>();
            bool result = FormulaEvaluator.IsCalculable("(5 + 3", ref errors);
            Assert.False(result);
            Assert.NotEmpty(errors);
        }

        [Fact]
        public void Evaluate_SimpleAddition_ReturnsCorrectResult()
        {
            List<string> errors = new List<string>();
            float result = FormulaEvaluator.Evaluate("5 + 3", new Dictionary<string, float>(), ref errors);
            Assert.Equal(8, result);
            Assert.Empty(errors);
        }

        [Fact]
        public void Evaluate_DivisionByZero_ReturnsNaN()
        {
            List<string> errors = new List<string>();
            float result = FormulaEvaluator.Evaluate("5 / 0", new Dictionary<string, float>(), ref errors);
            Assert.True(float.IsNaN(result));
            Assert.NotEmpty(errors);
        }

        [Fact]
        public void Evaluate_CellReference_ReturnsCorrectValue()
        {
            Dictionary<string, float> cellValues = new Dictionary<string, float> { { "{A1}", 10 } };
            List<string> errors = new List<string>();
            float result = FormulaEvaluator.Evaluate("{A1} + 5", cellValues, ref errors);
            Assert.Equal(15, result);
            Assert.Empty(errors);
        }

        [Fact]
        public void Evaluate_UndefinedCellReference_ReturnsNaN()
        {
            Dictionary<string, float> cellValues = new Dictionary<string, float>();
            List<string> errors = new List<string>();
            float result = FormulaEvaluator.Evaluate("{A1} + 5", cellValues, ref errors);
            Assert.True(float.IsNaN(result));
            Assert.NotEmpty(errors);
        }

        [Fact]
        public void Evaluate_IncFunction_ReturnsIncrementedValue()
        {
            List<string> errors = new List<string>();
            float result = FormulaEvaluator.Evaluate("inc 5", new Dictionary<string, float>(), ref errors);
            Assert.Equal(6, result);
            Assert.Empty(errors);
        }

        [Fact]
        public void Evaluate_NotFunction_ReturnsOneForZero()
        {
            List<string> errors = new List<string>();
            float result = FormulaEvaluator.Evaluate("not 0", new Dictionary<string, float>(), ref errors);
            Assert.Equal(1, result);
            Assert.Empty(errors);
        }

        [Fact]
        public void Evaluate_NotFunction_ReturnsZeroForNonZero()
        {
            List<string> errors = new List<string>();
            float result = FormulaEvaluator.Evaluate("not 5", new Dictionary<string, float>(), ref errors);
            Assert.Equal(0, result);
            Assert.Empty(errors);
        }

        [Fact]
        public void Evaluate_MultipleOperators_ReturnsCorrectResult()
        {
            List<string> errors = new List<string>();
            float result = FormulaEvaluator.Evaluate("5 + 3 * 2", new Dictionary<string, float>(), ref errors);
            Assert.Equal(11, result);
            Assert.Empty(errors);
        }

        [Fact]
        public void Evaluate_UnmatchedParenthesis_ReturnsNaN()
        {
            List<string> errors = new List<string>();
            float result = FormulaEvaluator.Evaluate("(5 + 3", new Dictionary<string, float>(), ref errors);
            Assert.True(float.IsNaN(result));
            Assert.NotEmpty(errors);
        }

        [Fact]
        public void GetAllCellLinks_ValidFormula_ReturnsCellLinks()
        {
            List<string> links = FormulaEvaluator.GetAllCellLinks("{A1} + {B2} * 5");
            Assert.Equal(2, links.Count);
            Assert.Contains("{A1}", links);
            Assert.Contains("{B2}", links);
        }

        [Fact]
        public void Evaluate_Subtraction_ReturnsCorrectResult()
        {
            List<string> errors = new List<string>();
            float result = FormulaEvaluator.Evaluate("10 - 3", new Dictionary<string, float>(), ref errors);
            Assert.Equal(7, result);
            Assert.Empty(errors);
        }

        [Fact]
        public void Evaluate_Multiplication_ReturnsCorrectResult()
        {
            List<string> errors = new List<string>();
            float result = FormulaEvaluator.Evaluate("4 * 3", new Dictionary<string, float>(), ref errors);
            Assert.Equal(12, result);
            Assert.Empty(errors);
        }

        [Fact]
        public void Evaluate_Exponentiation_ReturnsCorrectResult()
        {
            List<string> errors = new List<string>();
            float result = FormulaEvaluator.Evaluate("2 ^ 3", new Dictionary<string, float>(), ref errors);
            Assert.Equal(8, result);
            Assert.Empty(errors);
        }

        [Fact]
        public void Evaluate_ComplexExpressionWithParentheses_ReturnsCorrectResult()
        {
            List<string> errors = new List<string>();
            float result = FormulaEvaluator.Evaluate("(2 + 3) * 4", new Dictionary<string, float>(), ref errors);
            Assert.Equal(20, result);
            Assert.Empty(errors);
        }

        [Fact]
        public void Evaluate_IncFunctionAfterAddition_ReturnsCorrectResult()
        {
            List<string> errors = new List<string>();
            float result = FormulaEvaluator.Evaluate("inc (5 + 2)", new Dictionary<string, float>(), ref errors);
            Assert.Equal(8, result);
            Assert.Empty(errors);
        }

        [Fact]
        public void Evaluate_ComparisonGreaterThan_ReturnsCorrectResult()
        {
            List<string> errors = new List<string>();
            float result = FormulaEvaluator.Evaluate("5 > 3", new Dictionary<string, float>(), ref errors);
            Assert.Equal(1, result);
            Assert.Empty(errors);
        }

        [Fact]
        public void Evaluate_ComparisonLessThan_ReturnsCorrectResult()
        {
            List<string> errors = new List<string>();
            float result = FormulaEvaluator.Evaluate("3 < 5", new Dictionary<string, float>(), ref errors);
            Assert.Equal(1, result);
            Assert.Empty(errors);
        }

        [Fact]
        public void Evaluate_ComparisonEqual_ReturnsCorrectResult()
        {
            List<string> errors = new List<string>();
            float result = FormulaEvaluator.Evaluate("5 = 5", new Dictionary<string, float>(), ref errors);
            Assert.Equal(1, result);
            Assert.Empty(errors);
        }

        [Fact]
        public void Evaluate_ComplexNestedParentheses_ReturnsCorrectResult()
        {
            List<string> errors = new List<string>();
            float result = FormulaEvaluator.Evaluate("((2 + 3) * 2) - 4", new Dictionary<string, float>(), ref errors);
            Assert.Equal(6, result);
            Assert.Empty(errors);
        }
    }
}