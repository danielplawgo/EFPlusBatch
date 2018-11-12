using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace EFPlusBatch
{
    class Program
    {
        static void Main(string[] args)
        {
            HibernatingRhinos.Profiler.Appender.EntityFramework.EntityFrameworkProfiler.Initialize();

            DeleteWithEFPlus();
        }

        private static void SoftDelete()
        {
            using (var db = new DataContext())
            {
                var category = db.Categories.FirstOrDefault(c => c.IsActive);

                if (category == null)
                {
                    return;
                }

                category.IsActive = false;

                foreach (var product in category.Products)
                {
                    product.IsActive = false;
                }

                db.SaveChanges();
            }
        }

        private static void SoftDeleteWithEFPlus()
        {
            using (var db = new DataContext())
            {
                var category = db.Categories.FirstOrDefault(c => c.IsActive);

                if (category == null)
                {
                    return;
                }

                category.IsActive = false;

                db.Products.Where(p => p.CategoryId == category.Id)
                    .Update(p => new Product() { IsActive = false });

                db.SaveChanges();
            }
        }

        private static void SoftDeleteWithEFPlusWithTransaction()
        {
            using (var db = new DataContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    var category = db.Categories.FirstOrDefault(c => c.IsActive);

                    if (category == null)
                    {
                        return;
                    }

                    category.IsActive = false;

                    db.Products.Where(p => p.CategoryId == category.Id)
                        .Update(p => new Product() { IsActive = false });

                    db.SaveChanges();

                    transaction.Commit();
                }
            }
        }

        private static void DeleteWithEFPlus()
        {
            using (var db = new DataContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    var category = db.Categories.FirstOrDefault(c => c.IsActive);

                    if (category == null)
                    {
                        return;
                    }

                    category.IsActive = false;

                    db.Products.Where(p => p.CategoryId == category.Id)
                        .Delete();

                    db.SaveChanges();

                    transaction.Commit();
                }
            }
        }
    }
}
