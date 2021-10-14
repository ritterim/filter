using System;
using NPoco;

namespace Filter.NPoco.Tests.Testing
{
    [TableName("Person")]
    [PrimaryKey(nameof(Id))]
    public class Person
    {
        public int Id { get; set; }

        /// <summary>
        /// Used for any tests of nullable-values.
        /// </summary>
        public string DONOTUSE { get; }

        public DateTime FavoriteDate { get; set; }
        public DateTimeOffset FavoriteDateTimeOffset { get; set; }
        public char FavoriteLetter { get; set; }
        public int FavoriteNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal? Rating { get; set; }
    }
}
