# Fuzzy Numbers Library for .NET
An open-source library for performing basic operations with fuzzy numbers. It supports piecewise linear fuzzy numbers of arbitrarily high degree, which makes it computationally efficient. Piecewise linear fuzzy numbers can also be used to approximate more complex types of fuzzy numbers.

The library is written in C# and it targets .NET Standard 2.0 so that it could be used in the modern .NET versions as well
as in older .NET Framework applications.

The library will be available as a Nuget (Holecek.FuzzyMath). It's currently in a pre-release version.

# Usage
## Create a fuzzy number

The classes related to fuzzy numbers are located in the `Holecek.FuzzyMath.FuzzyNumbers` namespace.
Using the `FuzzyNumber` class constructor, you can create various common types of fuzzy numbers. A **triangular fuzzy number** can be created by passing three values to the constructor:

```csharp
var triangular = new FuzzyNumber(1, 2, 3);
```

A **trapezoidal fuzzy number** is created by passing four values:
```csharp
var trapezoidal = new FuzzyNumber(1, 2, 3, 4);
```

Providing just two values creates a fuzzy representation of a closed **interval**, and passing a single numeric value
creates a fuzzy representation of a **crisp number**:
```csharp
var interval = new FuzzyNumber(5, 6);
var crisp = new FuzzyNumber(7);
```

Another way to create a fuzzy number is to provide a **list of α-cuts** in order from the support to the kernel:
```csharp
var fuzzyNumber = new FuzzyNumber(
[
    new Interval(0, 7),
    new Interval(1, 6),
    new Interval(3, 4)
]);
```
To define a valid fuzzy number, the list of α-cuts must contain at least two items, and each α-cut must be a subinterval of the previous one.

## Basic methods

### Get an α-cut
An α-cut of a fuzzy number can be obtained using the `GetAlphaCut` method for any α in the range [0, 1]. For example:
```csharp
var fuzzyNumber = new FuzzyNumber(1, 2, 3);
Interval alphaCut = fuzzyNumber.GetAlphaCut(0.75);
```
**Mathematical note:**
For convenience, `GetAlphaCut(0)` returns the *support* of the fuzzy number instead of the actual 0-cut,
which would be (-∞, ∞) if we followed the strict mathematical definition.
This makes the method much more practical to use.

### Get membership degree
The membership degree of an element is calculated by `GetMembership`. For example:
```csharp
var fuzzyNumber = new FuzzyNumber(1, 2, 3);
double alpha = fuzzyNumber.GetMembership(1.5);
```

### Changing the number of α-cuts
The `WithAlphaCutsCount` method creates a copy of the fuzzy number with the specified number of α-cuts.
This is especially useful when performing operations with multiple fuzzy numbers that require all of them to have the same number of α-cuts.
In the following example, a triangular fuzzy number is recreated with 60 α-cuts:
```csharp
var fuzzyNumber = new FuzzyNumber(1, 2, 3).WithAlphaCutsCount(60);
```

### Equality of fuzzy numbers
The method `IsEqualTo` checks if one fuzzy number is equal to another. The comparison is made according to the definition: two fuzzy numbers
are considered equal if their respective α-cuts are equal. The compared fuzzy numbers do not need to have the same number of α-cuts.

The method takes two arguments: another fuzzy number for comparison and a tolerance value for the comparison.

```csharp
const double Tolerance = 0.001;
var first = new FuzzyNumber(1, 2, 3);
var sameWithMoreAlphaCuts = new FuzzyNumber(1, 2, 3).WithAlphaCutsCount(4);

bool areEqual = first.IsEqualTo(sameWithMoreAlphaCuts, Tolerance); // true
```

## Arithmetic operations with fuzzy numbers

The basic arithmetic operators `+`, `-`, `*`, and `/` are overloaded. Performing arithmetic operations with
fuzzy numbers is therefore very simple.

```csharp
var a = new FuzzyNumber(1, 2, 3);
var b = new FuzzyNumber(3, 4, 5);
var c = new FuzzyNumber(6, 7, 8, 9);
FuzzyNumber result = 0.3  * a + 0.6 * b + 0.1 * c;
```

It can be seen from the  example above that also a combination of `FuzzyNumber` and double values is supported for the arithmetic operations.

This is a very simple and convenient way but it has a limitation that the fuzzy numbers must have the same way of α-cuts. Otherwise an exception is thrown.

If you need more control or perform the calculation for fuzzy numbers with different number of α-cuts, you can use
the static methods in the `FuzzyNumberArithmetic` class. The `Add`, `Subtract`, `Multiply`, and `Divide` have
an overload with the number of α-cuts for the result as an additional argument. When this argument is specified,
the input fuzzy numbers can have a different number of α-cuts and the conversion is performed automatically.

```csharp
var a = new FuzzyNumber(1, 2, 3).WithAlphaCutsCount(15);
var b = new FuzzyNumber(3, 4, 5).WithAlphaCutsCount(40);;
FuzzyNumber sum = FuzzyNumberArithmetic.Add(a, b, alphaCutsCount: 30);
```

The `FuzzyNumberArithmetic` contains also additional methods:
* `Negation` - for a fuzzy number A, the value -A is returned.
* `Reciprocal` - for a fuzzy number A, the value 1/A is returned.


## Advanced operations with α-cuts

### Creating a fuzzy number using an α-cuts function
Instead of directly providing a list of α-cuts to the constructor, you can create a fuzzy number from a function that defines its α-cuts.
This is done using the static method `FuzzyNumber.FromAlphaCutFunction`.
It expects a function that takes the α (from 0 to 1) and returns the corresponding α-cut interval.

In the following example, a fuzzy number is created with 60 α-cuts defined as [2 + 3α, 10 - 2α], for any α in [0, 1]:

```csharp
const int AlphaCutCount = 60;
FuzzyNumber fuzzyNumber = FuzzyNumber.FromAlphaCutFunction(
    alpha => new Interval(2 + 3 * alpha, 10 - 2 * alpha),
    AlphaCutCount);
```

An important difference from providing a list of α-cuts directly to the constructor is that the constructor requires the α-cuts to be valid.
Any α-cut must be a subinterval of all α-cuts with a lower value of α to form a valid fuzzy number. If this condition is not met, the constructor throws an exception.

The `FuzzyNumber.FromAlphaCutFunction` method, however, behaves differently. If the function returns α-cuts that do not satisfy the condition, they are automatically
adjusted to ensure validity. This is especially important when creating fuzzy numbers as a result of mathematical operations, as rounding errors in the `double` values
used for the interval boundaries could otherwise lead to invalid α-cuts.

For example, due to rounding errors, the resulting α-cuts could be {[2, 3], [1.99999, 3], [2, 3.00001]}.
The constructor would throw an exception, whereas the `FromAlphaCutFunction` method would automatically adjust these α-cuts to {[2, 3], [2, 3], [2, 3]} to ensure validity.

### Custom operations with fuzzy numbers

Unary and binary operations with fuzzy numbers can be performed using the `FuzzyNumber.FromFuzzyNumberOperation` static method. In the case of a **unary operation**,
the method takes the input fuzzy number and a function that transforms its α-cuts. By default, the result will have the same number of α-cuts as the input, but a different
number of α-cuts can be optionally provided as another argument.

For example, the negation of a fuzzy number can be created as follows:

```csharp
var inputFuzzyNumber  = new FuzzyNumber(1, 2, 3);

FuzzyNumber result = FuzzyNumber.FromFuzzyNumberOperation(
    inputFuzzyNumber,
    alphaCut => -1 * alphaCut);
```

A **binary operation** with two fuzzy numbers can be done in a similar way. The following arguments are provided to the method: two input fuzzy numbers, a function that
takes the α-cuts from the first and the second input fuzzy numbers and returns the corresponding α-cut for the result.

The following example shows the multiplication of two fuzzy numbers:

```csharp
var fuzzyNumberA  = new FuzzyNumber(1, 2, 3);
var fuzzyNumberB  = new FuzzyNumber(4, 5, 6);

FuzzyNumber result = FuzzyNumber.FromFuzzyNumberOperation(
    fuzzyNumberA,
    fuzzyNumberB,
    (alphaCutA, alphaCutB) => alphaCutA * alphaCutB;
```

The input fuzzy numbers must have the same number of α-cut, because otherwhise it isn't possible to determine the number of α-cuts for the result. An exception is thrown in that case.
However, the method has another overload, that takes the number of α-cuts for the resulting fuzzy number as an additional argument. This overload can operate also on input fuzzy numbers with different numbers of α-cuts.

## Intervals
Intervals related clases are in the `Holecek.FuzzyMath.Intervals` namespace. The `Interval` class represents a closed interval and it's
used in this library primarily to represent an α-cut of a fuzzy number.

The `Min` and `Max` properties represent the lower and the upper bound of the interval. Arithmetic operators `+`, `-`, `*`, and `/` are overloaded so they can be used for the interval arithmetic easily in a similar way described for the fuzzy numbers before.