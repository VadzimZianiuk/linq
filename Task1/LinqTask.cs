using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Task1.DoNotChange;

namespace Task1
{
    public static class LinqTask
    {
        public static IEnumerable<Customer> Linq1(IEnumerable<Customer> customers, decimal limit)
        {
            return customers?.Where(c => (c?.Orders?.Sum(o => o?.Total) ?? 0) > limit);
        }

        public static IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> Linq2(
            IEnumerable<Customer> customers,
            IEnumerable<Supplier> suppliers
        )
        {
            return customers?.Select(c => (c, suppliers?.Where(s => s?.Country == c?.Country && s?.City == c?.City)));
        }

        public static IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> Linq2UsingGroup(
            IEnumerable<Customer> customers,
            IEnumerable<Supplier> suppliers
        )
        {
            return customers?.GroupJoin(suppliers, c => (c?.Country, c?.City), s => (s?.Country, s?.City), (c, s) => (c, s));
        }

        public static IEnumerable<Customer> Linq3(IEnumerable<Customer> customers, decimal limit)
        {
            return customers?.Where(x => x?.Orders?.Any(o => o?.Total > limit) ?? false);
        }

        public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq4(
            IEnumerable<Customer> customers
        )
        {
            return customers?.Where(c => c?.Orders?.Length > 0)
                .Select(c => (c, c.Orders.Where(o => o != null).Min(o => o.OrderDate)));
        }

        public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq5(
            IEnumerable<Customer> customers
        )
        {
            return Linq4(customers)?.OrderBy(x => x.dateOfEntry.Year)
                .ThenBy(x => x.dateOfEntry.Month)
                .ThenByDescending(x => x.customer.Orders
                    .Where(o=>o != null).Sum(o => o.Total))
                .ThenBy(x => x.customer.CompanyName);
        }

        public static IEnumerable<Customer> Linq6(IEnumerable<Customer> customers)
        {
            return customers?.Where(c => string.IsNullOrWhiteSpace(c?.Region)
                                         || !(c.PostalCode?.All(char.IsDigit) ?? false)
                                         || !Regex.IsMatch(c.Phone, @"^\({1}\d+\){1}"));
        }

        public static IEnumerable<Linq7CategoryGroup> Linq7(IEnumerable<Product> products)
        {
            return products?.Where(x => x?.Category != null)
                .GroupBy(x => x.Category)
                .Select(categoryGr =>
                    new Linq7CategoryGroup
                    {
                        Category = categoryGr.Key,
                        UnitsInStockGroup = categoryGr.GroupBy(x => x.UnitsInStock)
                            .Select(unitsInStockGr =>
                                new Linq7UnitsInStockGroup
                                {
                                    UnitsInStock = unitsInStockGr.Key,
                                    Prices = unitsInStockGr.Select(x => x.UnitPrice).OrderBy(x => x)
                                })
                    });
        }

        public static IEnumerable<(decimal category, IEnumerable<Product> products)> Linq8(
            IEnumerable<Product> products,
            decimal cheap,
            decimal middle,
            decimal expensive
        )
        {
            return products?.GroupBy(x => x switch
            {
                _ when x.UnitPrice <= cheap => cheap,
                _ when x.UnitPrice <= middle => middle,
                _ => expensive
            }).Select(x => (x.Key, x.AsEnumerable()));
        }

        public static IEnumerable<(string city, int averageIncome, int averageIntensity)> Linq9(
            IEnumerable<Customer> customers
        )
        {
            return customers?.Where(x => x?.City != null)
                .GroupBy(x => x.City)
                .Select(x =>
                (
                    x.Key,
                    (int)Math.Round(x.Average(c => c.Orders?.Where(o => o != null).Sum(o => o.Total) ?? 0)),
                    (int)Math.Round(x.Average(c => c.Orders?.Length ?? 0)))
                );
        }

        public static string Linq10(IEnumerable<Supplier> suppliers)
        {
            var data = suppliers?.Where(x => x?.Country != null)
                .Select(x => x.Country)
                .Distinct()
                .OrderBy(x => x.Length)
                .ThenBy(x => x);

            return data is null ? null : string.Concat(data);
        }
    }
}