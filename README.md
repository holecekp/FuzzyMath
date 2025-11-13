# Fuzzy numbers library for .NET
An open source library for basic operations with fuzzy numbers. It supports piece-wise linear fuzzy numbers of arbitrary high degree. This makes it computationally efficient. Also, piece-wise linear fuzzy numbers can be used to approximate more complex types of fuzzy numbers.

# Usage
## Create fuzzy number

Using  `FuzzyNumber` class constructor, you can create various common types of fuzzy numbers. A triangular fuzzy number can be created by passing three values to the constructor:
```csharp
var triangular = new FuzzyNumber(1, 2, 3);
```

A trapezoidal fuzzy number is created by passing four values:
```csharp
var trapeziodal = new FuzzyNumber(1, 2, 3, 4);
```

Providing just two values creates a fuzzy representation of a closed interval, and similarly, passing just a single numeric value
creates a fuzzy representation of a crisp number:
```csharp
var interval = new FuzzyNumber(5, 6);
var crisp = new FuzzyNumber(7);
```

Another way is to provide the list of α-cuts in the order from support to the kernel:
```csharp
var fuzzyNumber = new FuzzyNumber([
    new Interval(0, 7),
    new Interval(1, 6),
    new Interval(3, 4)
]);
```
To define a valid fuzzy number, the list of α-cuts must have at least 2 items and any α-cut must be a sub-interval of the previous one.

## Basic methods

### Get an α-cut
An α-cut of a fuzzy number can be calculated by `GetAlphaCut` method for any value of α in [0, 1]. For example:
```csharp
FuzzyNumber fuzzyNumber = new FuzzyNumber(1, 2, 3);
Interval alphaCut = fuzzyNumber.GetAlphaCut(0.75);
```
*Mathematical note:* For convenience, GetAlphaCut(0) returns the support of the fuzzy number instead of a real 0-cut,
which would be (-∞, ∞), if we would stick strictly to the mathematical definition. This makes the practical usage
much easier.

### Get membership degree
The membership degree of an element is calculated by `GetMembership`. For example:
```csharp
FuzzyNumber fuzzyNumber = new FuzzyNumber(1, 2, 3);
double alpha = fuzzyNumber.GetMembershi(1.5);
```
# Fuzzy numbers library for .NET
An open source library for basic operations with fuzzy numbers. It supports piece-wise linear fuzzy numbers of arbitrary high degree. This makes it computationally efficient. Also, piece-wise linear fuzzy numbers can be used to approximate more complex types of fuzzy numbers.

# Usage
## Create fuzzy number

Using  `FuzzyNumber` class constructor, you can create various common types of fuzzy numbers. A triangular fuzzy number can be created by passing three values to the constructor:
```csharp
var triangular = new FuzzyNumber(1, 2, 3);
```

A trapezoidal fuzzy number is created by passing four values:
```csharp
var trapeziodal = new FuzzyNumber(1, 2, 3, 4);
```

Providing just two values creates a fuzzy representation of a closed interval, and similarly, passing just a single numeric value
creates a fuzzy representation of a crisp number:
```csharp
var interval = new FuzzyNumber(5, 6);
var crisp = new FuzzyNumber(7);
```

Another way is to provide the list of α-cuts in the order from support to the kernel:
```csharp
var fuzzyNumber = new FuzzyNumber([
    new Interval(0, 7),
    new Interval(1, 6),
    new Interval(3, 4)
]);
```
To define a valid fuzzy number, the list of α-cuts must have at least 2 items and any α-cut must be a sub-interval of the previous one.

## Basic methods

### Get an α-cut
An α-cut of a fuzzy number can be calculated by `GetAlphaCut` method for any value of α in [0, 1]. For example:
```csharp
FuzzyNumber fuzzyNumber = new FuzzyNumber(1, 2, 3);
Interval alphaCut = fuzzyNumber.GetAlphaCut(0.75);
```
*Mathematical note:* For convenience, GetAlphaCut(0) returns the support of the fuzzy number instead of a real 0-cut,
which would be (-∞, ∞), if we would stick strictly to the mathematical definition. This makes the practical usage
much easier.

### Get membership degree
The membership degree of an element is calculated by `GetMembership`. For example:
```csharp
FuzzyNumber fuzzyNumber = new FuzzyNumber(1, 2, 3);
double alpha = fuzzyNumber.GetMembership(1.5);
```
