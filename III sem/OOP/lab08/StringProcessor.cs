using System;

public class StringProcessor
{
    private readonly Func<string, string>[] processingSteps;

    public StringProcessor()
    {
        processingSteps = new Func<string, string>[]
        {
            str => str.Trim(),                           
            str => str.Replace(",", ""),               
            str => str.Replace("!", ""),                
            str => str.ToUpper(),                    
            str => string.Join(" ", str.Split(" ", StringSplitOptions.RemoveEmptyEntries))
        };
    }

    public string Process(string input)
    {
        foreach (var step in processingSteps)
        {
            input = step(input);
        }
        return input;
    }
}
