using Spectre.Console;

namespace QuizAppSolver;

public class UserInputBuilder {
    private readonly List<(string Prompt, string Default, string Postfix, bool UseSuffix, Action<string> Setter, Func<bool>? Condition)> _inputs = new();
    private readonly List<(string Prompt, int Count, string Default, string Postfix, bool UseSuffix, Action<List<double>> Setter, Func<bool>? Condition)> _listInputs = new();

    public UserInputBuilder AddTextInput(string prompt, Action<string> setter, string defaultValue = "", string postfix = "", bool useSuffix = true, Func<bool>? condition = null) {
        _inputs.Add((prompt, defaultValue, postfix, useSuffix, setter, condition));
        return this;
    }

    public UserInputBuilder AddNumericInput(string prompt, Action<double> setter, string defaultValue = "", string postfix = "", bool useSuffix = true, Func<bool>? condition = null) {
        _inputs.Add((prompt, defaultValue, postfix, useSuffix, val => setter(UnitConverter.ConvertUnits(val)), condition));
        return this;
    }
    
    public UserInputBuilder AddResistorInput(string prompt, Action<double> setter, string defaultValue = "1k", string postfix = "", bool useSuffix = true, Func<bool>? condition = null) {
        _inputs.Add((FormatPrompt(prompt, "resistor value", useSuffix), defaultValue, postfix, useSuffix, val => setter(UnitConverter.ConvertUnits(val)), condition));
        return this;
    }
    
    public UserInputBuilder AddVoltageInput(string prompt, Action<double> setter, string defaultValue = "12", string postfix = "", bool useSuffix = true, Func<bool>? condition = null) {
        _inputs.Add((FormatPrompt(prompt, "voltage", useSuffix), defaultValue, postfix, useSuffix, val => setter(UnitConverter.ConvertUnits(val)), condition));
        return this;
    }
    
    public UserInputBuilder AddCurrentInput(string prompt, Action<double> setter, string defaultValue = "2n", string postfix = "", bool useSuffix = true, Func<bool>? condition = null) {
        _inputs.Add((FormatPrompt(prompt, "current", useSuffix), defaultValue, postfix, useSuffix, val => setter(UnitConverter.ConvertUnits(val)), condition));
        return this;
    }
    
    public UserInputBuilder AddMultipleInputs(string prompt, int count, Action<List<double>> setter, string defaultValue = "", string postfix = "", bool useSuffix = true, Func<bool>? condition = null) {
        _listInputs.Add((prompt, count, defaultValue, postfix, useSuffix, setter, condition));
        return this;
    }
    
    public UserInputBuilder AddMultipleResistorInput(string prompt, int count, Action<List<double>> setter, string defaultValue = "1k", string postfix = "", bool useSuffix = true, Func<bool>? condition = null) {
        _listInputs.Add((FormatPrompt(prompt, "resistor value", useSuffix), count, defaultValue, postfix, useSuffix, setter, condition));
        return this;
    }
    
    public UserInputBuilder AddMultipleVoltageInput(string prompt, int count, Action<List<double>> setter, string defaultValue = "12", string postfix = "", bool useSuffix = true, Func<bool>? condition = null) {
        _listInputs.Add((FormatPrompt(prompt, "voltage", useSuffix), count, defaultValue, postfix, useSuffix, setter, condition));
        return this;
    }

    public UserInputBuilder AddSelection<T>(string prompt, Action<T> setter, Dictionary<T, string> options, Func<bool>? condition = null) where T : notnull {
        if (condition != null && !condition()) return this;
        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<T>()
                .Title($"[green]{prompt}[/]")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                .AddChoices(options.Keys)
                .UseConverter(opt => options[opt])
        );
        setter(selection);
        return this;
    }

    public UserInputBuilder AddSelection<T>(string prompt, Action<T> setter, T[] options, Func<bool>? condition = null) where T : notnull {
        if (condition != null && !condition()) return this;
        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<T>()
                .Title($"[green]{prompt}[/]")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                .AddChoices(options)
        );
        setter(selection);
        return this;
    }

    public void Build() {
        foreach (var (prompt, defaultValue, postfix, useSuffix, setter, condition) in _inputs) {
            if (condition != null && !condition()) continue;
            var input = AnsiConsole.Prompt(
                new TextPrompt<string>($"Enter the [green]{prompt}[/] {postfix}")
                    .PromptStyle("grey")
                    .DefaultValue(defaultValue)
                    .DefaultValueStyle("grey")
            );
            setter(input);
        }

        foreach (var (prompt, count, defaultValue, postfix, useSuffix, setter, condition) in _listInputs) {
            if (condition != null && !condition()) continue;
            List<double> inputs = [];
            for (int i = 0; i < count; i++) {
                string value = AnsiConsole.Prompt(
                    new TextPrompt<string>($"Enter the [green]{prompt} {i + 1}[/] {postfix}")
                        .PromptStyle("grey")
                        .DefaultValue(defaultValue)
                        .DefaultValueStyle("grey")
                );
                inputs.Add(UnitConverter.ConvertUnits(value));
            }
            setter(inputs);
        }
    }

    private static string FormatPrompt(string basePrompt, string suffix, bool useSuffix) {
        return useSuffix ? $"{basePrompt} {suffix}" : basePrompt;
    }
}
