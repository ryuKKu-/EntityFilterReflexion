using System;
using System.Linq;
using EntityFilter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityFilterTest
{
    [TestClass]
    public class UnitTest
    {
        private FakeRepository _repository => new FakeRepository();

        [TestMethod]
        public void FilterOnCustomerId()
        {
            var expected = _repository.Orders.AsQueryable().Where(x => x.CustomerId == 3).OrderBy(x => x.Price).ToList();

            var viewModel = new OrderFilterExampleModel
            {
                CustomerId = 3,
                OrderBy = "Price",
                Way = "asc"
            };

            var actual =
                _repository.Orders.AsQueryable()
                    .FilterFromModel(viewModel)
                    .OrderByExpressionBuilder(viewModel.OrderBy, viewModel.Way).ToList();


            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
