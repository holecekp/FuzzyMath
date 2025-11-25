# Fuzzy Numbers Library for .NET
An open-source library for performing basic operations with fuzzy numbers. It supports piecewise linear fuzzy numbers of arbitrarily high degree, which makes it computationally efficient. Piecewise linear fuzzy numbers can also be used to approximate more complex types of fuzzy numbers.

# Usage
## Create a fuzzy number

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
FuzzyNumber fuzzyNumber = new FuzzyNumber(1, 2, 3);
Interval alphaCut = fuzzyNumber.GetAlphaCut(0.75);
```
**Mathematical note:**
For convenience, `GetAlphaCut(0)` returns the *support* of the fuzzy number instead of the actual 0-cut,
which would be (-∞, ∞) if we followed the strict mathematical definition.
This makes the method much more practical to use.

### Get membership degree
The membership degree of an element is calculated by `GetMembership`. For example:
```csharp
FuzzyNumber fuzzyNumber = new FuzzyNumber(1, 2, 3);
double alpha = fuzzyNumber.GetMembership(1.5);
```

## Advanced operations with α-cuts

### Changing the number of α-cuts
The `WithAlphaCutsCount` method creates a copy of the fuzzy number with the specified number of α-cuts.
This is especially useful when performing operations with multiple fuzzy numbers that require all of them to have the same number of α-cuts.
In the following example, a triangular fuzzy number is recreated with 60 α-cuts:
```csharp
FuzzyNumber fuzzyNumber = new FuzzyNumber(1, 2, 3).WithAlphaCutsCount(60);
```

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