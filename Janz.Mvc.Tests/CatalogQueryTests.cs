using Janz.Mvc.Helpers;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Janz.Mvc.Tests
{
    public class CatalogQueryTests
    {
        private static HttpRequest Request(string query)
        {
            var context = new DefaultHttpContext();
            context.Request.QueryString = new QueryString(query);
            return context.Request;
        }

        [Theory]
        [InlineData("Coffee with low acidity", "coffee-with-low-acidity")]
        [InlineData("  Whole bean 1 kg ", "whole-bean-1-kg")]
        [InlineData("For milk / drinks!", "for-milk-drinks")]
        public void Slug_LowercasesAndJoinsWordsWithDashes(string name, string expected)
        {
            Assert.Equal(expected, CatalogQuery.Slug(name));
        }

        [Fact]
        public void Has_IgnoresCase()
        {
            Assert.True(CatalogQuery.Has(Request("?tag=Milk-Drinks"), "tag", "milk-drinks"));
            Assert.False(CatalogQuery.Has(Request("?tag=portafilter"), "tag", "milk-drinks"));
        }

        [Fact]
        public void ApiUrl_ForwardsFiltersAndAddsTheCategory()
        {
            var url = CatalogQuery.ApiUrl(Request("?tag=milk-drinks&feature=limited&category=ignored"), "coffee");

            Assert.StartsWith("api/products?", url);
            Assert.Contains("tag=milk-drinks", url);
            Assert.Contains("feature=limited", url);
            Assert.Contains("category=coffee", url);
            Assert.DoesNotContain("ignored", url);
        }

        [Fact]
        public void ApiUrl_HasNoQueryString_WhenThereIsNothingToSend()
        {
            Assert.Equal("api/products", CatalogQuery.ApiUrl(Request(string.Empty), null));
        }

        [Fact]
        public void ApiUrl_EscapesValues()
        {
            var url = CatalogQuery.ApiUrl(Request("?q=a%26b"), null);

            Assert.Contains("q=a%26b", url);
        }

        [Fact]
        public void ActiveCount_DoesNotCountTabSortOrSearch()
        {
            var count = CatalogQuery.ActiveCount(Request("?tab=new&sort=price-asc&q=natty&tag=milk-drinks&feature=limited"));

            Assert.Equal(2, count);
        }
    }
}
