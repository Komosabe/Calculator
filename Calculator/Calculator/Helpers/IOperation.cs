namespace Calculator.Helpers;

public interface IOperation
{
    decimal Operation(decimal a, decimal b);
}

public class Sum : IOperation
{
    public decimal Operation(decimal a, decimal b) => a + b;
}

public class Subtract : IOperation
{
    public decimal Operation(decimal a, decimal b) => a - b;
}

public class Multiply : IOperation
{
    public decimal Operation(decimal a, decimal b) => a * b;
}

public class Divide : IOperation
{
    public decimal Operation(decimal a, decimal b) => a / b;
}