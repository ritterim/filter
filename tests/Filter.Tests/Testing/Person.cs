using System;

namespace Filter.Tests.Testing
{
    public class Person
    {
        public int Id { get; set; }
        public DateTime FavoriteDate { get; set; }
        public DateTimeOffset FavoriteDateTimeOffset { get; set; }
        public char FavoriteLetter { get; set; }
        public int FavoriteNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal? Rating { get; set; }
    }
}