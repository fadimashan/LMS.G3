using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Bogus;
using LMS.API.Models.Entities;

namespace LMS.API.Data
{
    public class SeedData
    {
        private static Faker _faker;
        private const int _maxAuthors = 16;
        private const int _maxPublications = 32;

        public static async Task InitAsync(IServiceProvider services)
        {
            using (var db = services.GetRequiredService<ApiDbContext>())
            {
                _faker = new Faker();

                // Fill the database only if Authors and Literature tables are empty 
                if (db.Authors.Any() && db.Publications.Any())
                {
                    return;
                }
                
                Random rnd = new Random();
                var maxLevels = Enum.GetValues(typeof(DifficultyLevel)).Length;

                // Create Subjects and PublicationTypes
                var allTypes = CreatePublicationTypes();
                await db.PublicationTypes.AddRangeAsync(allTypes);
                var maxTypeIndex = allTypes.Length;
                var allSubjects = CreateSubjects();
                await db.Subjects.AddRangeAsync(allSubjects);
                var maxSubjectIndex = allSubjects.Length;

                // Create authors
                var authors = new List<Author>();
                for (var i = 0; i < _maxAuthors; i++)
                {
                    var author = new Author
                    {
                        FirstName = _faker.Name.FirstName(),
                        LastName = _faker.Name.LastName(),
                        DateOfBirth = _faker.Date.Past(50, DateTime.Parse("1999-12-31")).Date,
                        Publications = new List<Publication>()
                    };
                    authors.Add(author);
                }

                // Create literature
                var publications = new List<Publication>();
                for (var i = 0; i < _maxPublications; i++)
                {
                    // Retrieve random LiteratureType and random Subject
                    var type = allTypes[rnd.Next(maxTypeIndex)];
                    var subject = allSubjects[rnd.Next(maxSubjectIndex)];
                    
                    var publication = new Publication
                    {
                        Title = _faker.Commerce.ProductName(),
                        Description = _faker.Commerce.ProductDescription(),
                        PublicationDate = _faker.Date.Past(10, DateTime.Parse("2021-03-01")).Date,
                        // Random difficulty level
                        Level = (DifficultyLevel)rnd.Next(maxLevels),
                        // Add random LiteratureType & corresponding TypeId
                        Type = type,
                        TypeId = type.Id,
                        // Add random Subject & corresponding SubjectId
                        Subject = subject,
                        SubjectId = subject.Id,
                        Authors = new List<Author>()
                    };
                    // type.Publications.Add(publication);
                    // subject.Publications.Add(publication);
                    publications.Add(publication);
                }
                
                // Associate Authors with Publications
                // 1. Go over all the authors and to each add a random publication
                foreach (var author in authors)
                {
                    var publication = publications[rnd.Next(_maxPublications)];
                    author.Publications.Add(publication);
                }
                // 2. Go over all the publications and to each add some author
                foreach (var publication in publications)
                {
                    var author = authors[rnd.Next(_maxAuthors)];
                    publication.Authors.Add(author);
                }
                
                await db.Authors.AddRangeAsync(authors);
                await db.Publications.AddRangeAsync(publications);

                AddSpecial2(db, allTypes, allSubjects);

                await db.SaveChangesAsync();
            }
        }

        private static PublicationType[] CreatePublicationTypes()
        {
            return new[]
            {
                new PublicationType
                {
                    Name = "Book",
                    Publications = new List<Publication>()
                },
                new PublicationType
                {
                    Name = "Blogg",
                    Publications = new List<Publication>()
                },
                new PublicationType
                {
                    Name = "Article",
                    Publications = new List<Publication>()
                }
            };
        }

        private static Subject[] CreateSubjects()
        {
            return new[]
            {
                new Subject
                {
                    Name = "Java",
                    Publications = new List<Publication>()
                }, 
                new Subject
                {
                    Name = "JavaScript",
                    Publications = new List<Publication>()
                },
                new Subject
                {
                    Name = "C# Foundations",
                    Publications = new List<Publication>()
                },
                new Subject
                {
                    Name = "ASP.NET MVC",
                    Publications = new List<Publication>()
                },
                new Subject
                {
                    Name = "Databases",
                    Publications = new List<Publication>()
                },
                new Subject
                {
                    Name = "Security",
                    Publications = new List<Publication>()
                },
                new Subject
                {
                    Name = "Python",
                    Publications = new List<Publication>()
                },
                new Subject
                {
                    Name = "FrontEnd",
                    Publications = new List<Publication>()
                },
                new Subject
                {
                    Name = "RESTful API",
                    Publications = new List<Publication>()
                },
                new Subject
                {
                    Name = "Other",
                    Publications = new List<Publication>()
                }
            };
        }

        private static void AddSpecial2(ApiDbContext db, PublicationType[] types, Subject[] subjects)
        {
            var author1 = new Author
            {
                FirstName = "Zaphod",
                LastName = "Beeblebrox",
                DateOfBirth = DateTime.Parse("1501-04-14"),
                Publications = new List<Publication>()
            };
            var author2 = new Author
            {
                FirstName = "Ford",
                LastName = "Prefect",
                DateOfBirth = DateTime.Parse("1501-06-04"),
                Publications = new List<Publication>()
            };
            var book1 = new Publication
            {
                Title = "How to get elected the President of the Galaxy",
                Description = "A fascinating autobiography of the most (in)famous president of the Galaxy",
                PublicationDate = DateTime.Parse("1600-06-18"),
                Level = DifficultyLevel.Beginner,
                Type = types[0],
                TypeId = types[0].Id,
                Subject = subjects.Last(),
                SubjectId = subjects.Last().Id,
                Authors = new List<Author>()
            };
            var book2 = new Publication
            {
                Title = "An interstellar tourist's guide to Earth: Hitchhiker's companion",
                Description = "Best bars, best clubs, best ways to blend in with the locals -- an indispensable help to any serious interstellar hitchhiker!",
                PublicationDate = DateTime.Parse("1560-01-18"),
                Level = DifficultyLevel.Beginner,
                Type = types[0],
                TypeId = types[0].Id,
                Subject = subjects.Last(),
                SubjectId = subjects.Last().Id,
                Authors = new List<Author>()
            };
            var book3 = new Publication
            {
                Title = "Best restaurants at the end of the Universe",
                Description = "Account of a first-hand experience at the Milliways",
                PublicationDate = DateTime.Parse("1990-01-18"),
                Level = DifficultyLevel.Beginner,
                Type = types[0],
                TypeId = types[0].Id,
                Subject = subjects.Last(),
                SubjectId = subjects.Last().Id,
                Authors = new List<Author>()
            };
            author1.Publications.Add(book1);
            book1.Authors.Add(author1);
            author2.Publications.Add(book2);
            book2.Authors.Add(author2);
            author2.Publications.Add(book3);
            book3.Authors.Add(author2);

            db.Authors.AddRange(author1, author2);
            db.Publications.AddRange(book1, book2, book3);
        }

        /*
        private static void AddSpecial(ApiDbContext db)
        {
            db.Authors.AddRange(
                    new Author
                    {
                        FirstName = "Zaphod",
                        LastName = "Beeblebrox",
                        DateOfBirth = DateTime.Parse("1501-04-14"),
                        Publications = new List<Publication>()
                        {
                            new Publication
                            {
                                Title = "How to get elected the President of the Galaxy",
                                Description = "A fascinating autobiography of the most (in)famous president of the Galaxy",
                                PublicationDate = DateTime.Parse("1600-06-18"),
                                Level = DifficultyLevel.Beginner
                            }
                        }
                    }, 
                    new Author()
                    {
                        FirstName = "Ford",
                        LastName = "Prefect",
                        DateOfBirth = DateTime.Parse("1501-06-04"),
                        Publications = new List<Publication>()
                        {
                            new Publication
                            {
                                Title = "An interstellar tourist's guide to Earth: Hitchhiker's companion",
                                Description = "Best bars, best clubs, best ways to blend in with the locals -- an indispensable help to any serious interstellar hitchhiker!",
                                PublicationDate = DateTime.Parse("1560-01-18"),
                                Level = DifficultyLevel.Intermediate
                            },
                            new Publication
                            {
                                Title = "Best restaurants at the end of the Universe",
                                Description = "Account of a first-hand experience at the Milliways",
                                PublicationDate = DateTime.Parse("1560-01-18"),
                                Level = DifficultyLevel.Intermediate
                            }
                        }
                    });
        }*/
    }
}