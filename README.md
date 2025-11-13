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
