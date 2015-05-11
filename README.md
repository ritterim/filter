# Filter [![RimDev.Filter NuGet badge](https://img.shields.io/nuget/v/RimDev.Filter.svg)](http://www.nuget.org/packages/RimDev.Filter/) [![RimDev.Filter NuGet downloads](https://img.shields.io/nuget/dt/RimDev.Filter.svg)](http://www.nuget.org/packages/RimDev.Filter/)

> "the classy way to filter collections"

This library allows an IEnumerable to be filtered using either a typed or anonymous class.

## Installation

Install the [RimDev.Filter](https://www.nuget.org/packages/RimDev.Filter/) NuGet package.

```shell
Install-Package RimDev.Filter
```

## Usage

```csharp
using RimDev.Filter.Generic;
using RimDev.Filter.Range;

public class Person
{
    public Person(string firstName, int favoriteNumber)
    {
        FavoriteNumber = favoriteNumber;
        FirstName = firstName;
    }

    public int FavoriteNumber { get; set; }
    public string FirstName { get; set; }
}

var people = new List<Person>()
{
    new Person("John", 1),
    new Person("Tom", 2),
    new Person("Jane", 3),
    new Person("Sally", 4)
};

IEnumerable<Person> filteredPeople = null;

// returns just the first-record.
filteredPeople = people.Filter(new
{
    FirstName = "John"
});

// returns the first and second-record.
filteredPeople = people.Filter(new
{
    FirstName = new[] { "John", "Tom" }
});

// returns the second-record.
filteredPeople = people.Filter(new
{
    FavoriteNumber = Range.FromString<int>("(1,3)")
});

// returns the first three-records.
filteredPeople = people.Filter(new
{
    FavoriteNumber = Range.FromString<int>("[1,4)")
});
```

## Range-type

This is a new type which allows an easy way to represent a range of values when filtering. An instance can either be created manually or via a helper which takes a string formatted using standard [interval notation](http://en.wikipedia.org/wiki/Interval_%28mathematics%29).

While any type is allowed as a range, only certain types will work when filtering:

  - byte
  - char
  - DateTime
  - decimal
  - double
  - float
  - int
  - long
  - sbyte
  - short
  - uint
  - ulong
  - ushort

Any other types will just silently get passed-over.

### Usage

```csharp
using RimDev.Filter.Range;
using RimDev.Filter.Range.Generic;

var range = new Range<int>()
{
    MinValue = 0,
    MaxValue = 10,
    IsMinInclusive = false,
    IsMaxInclusive = true
};

// or with helper.
var range = Range.FromString<int>("(0,10]");
```

### Implicit casting

Instead of having to call the static-helper `Range.FromString<T>`, you can also leverage implicit casting support:

```csharp
Range<int> range = "[1,5]";

// or...

// Type is Range<int>.
var range2 = (Range<int>)"[1,5]";
```

### Open-ended support

Instead of having to specify both lower and upper-bound, you can truncate one of the two:

```csharp

// x <= 5
var range = (Range<int>)"(,5]";

// range.MinValue.HasValue = false;

// or use Unicode

var range2 = (Range<int>)"(-∞,5]";
var range3 = (Range<int>)"[1,+∞)";
```

## Current limitations

  - The library currently only works with class-type collections.
  - Constant filter-value (i.e. non-enumerable or range) exceptions are trapped within the `Filter` call.

## License

[MIT](https://github.com/ritterim/filter/blob/master/LICENSE)

## Contributing

Issue documentation and pull-requests are welcomed. This project follows the [fork & pull](https://help.github.com/articles/using-pull-requests/#fork--pull) model.
