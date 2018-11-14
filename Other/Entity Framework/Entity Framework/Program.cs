using System;
using System.Data.Entity;
using System.Linq;
using Entity_Framework.SQLite.EF;

namespace Entity_Framework
{
    class Program
    {
        public class Repository : IRepository
        {
            //[UsersTableInCode("user")]
            public class UsersTableInCode
            {
                //[Key]
                //[Column("UserID")]
                public string UserIDInCode { get; set; }
                //[Column("UserName")]
                public string UserNameInCode { get; set; }
            }
        }

        public class UsersTableClass : ExternallyMappedContext<Repository>
        {
            static string mapping =
                    $"[DbSet-to-DB-Mapping v1.0]{Environment.NewLine}" +
                    $"[TABLE]user{Environment.NewLine}" +
                    $"[KEY]UserIDInCode{Environment.NewLine}" +
                    $"[COLUMN]UserID[MAPS-TO]UserIDInCode{Environment.NewLine}" +
                    $"[COLUMN]UserName[MAPS-TO]UserNameInCode{Environment.NewLine}" +
                    ""
                ;
            static string DBPath = @"Resources\DB.sqlite";
            public UsersTableClass() : base(DBPath, false, (typeof(Repository.UsersTableInCode), mapping))
            {

            }

            public DbSet<Repository.UsersTableInCode> UsersTable { get; set; }
        }

        static void Main(string[] args)
        {
            using (var reposs = new UsersTableClass())
            {
                var row = reposs.UsersTable.First();
                var userId = row.UserIDInCode;
                var userName = row.UserNameInCode;
                Console.WriteLine($"{userId}:{userName}");
            }
            Console.ReadLine();
        }
    }
}
