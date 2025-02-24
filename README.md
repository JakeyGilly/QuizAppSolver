# QuizApp Solver

## Overview
This **QuizApp Solver** is designed to assist with solving quizzes available on [QuizApp](https://elecquiz.york.ac.uk/), a platform used for the **analogue electronics** module at the University of York. The solver aids the solving of the questions.

## Installation
1. Clone this repository:
   ```sh
   git clone https://github.com/JakeyGilly/QuizAppSolver.git
   cd quizappsolver
   ```
2. Build the C# project:
   ```sh
   dotnet build
   ```

## Usage
1. Run the solver:
   ```sh
   dotnet run
   ```
2. Input the required values from the quiz.
3. The solver will calculate and display the correct answer.

## Example Calculation
For a **forward biased diode circuit** with a 10V supply, 2kΩ resistor and a diode with a 2nA saturation current and an ideality factor of 2:

```sh
> Enter the saturation current (2n): 2n
> Enter the ideality factor (2): 2
> Enter the supply voltage (12): 10
> Enter the resistance of the resistor (1k): 2k
```

The solver computes:
```
The current is 4.63mA.
The voltage across the resistor is 9.27V.
The voltage across the diode is 732.78mV.
```
