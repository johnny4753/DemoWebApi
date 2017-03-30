using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using DemoWebApi.Models.Domain;
using LinqKit;

namespace DemoWebApi.Models
{
    /// <summary>
    /// 訂單搜尋過濾條件
    /// </summary>
    public class OrderSearchFilter
    {
        /// <summary>
        /// 最小運費，意即搜尋運費 >= 此值的訂單
        /// </summary>
        [Column(TypeName = "money")]
        public decimal? SearchMinFreight { get; set; }

        /// <summary>
        /// 船名
        /// </summary>
        [StringLength(40)]
        public string SearchShipName { get; set; }

        /// <summary>
        /// 船城市
        /// </summary>
        [StringLength(15)]
        public string SearchShipCity { get; set; }

        public ExpressionStarter<Order> GetPredicate()
        {
            var predicate = PredicateBuilder.New<Order>(true);
            if (SearchMinFreight.HasValue)
            {
                predicate = predicate.And(x => x.Freight >= SearchMinFreight);
            }
            if (!string.IsNullOrEmpty(SearchShipName))
            {
                predicate = predicate.And(x => x.ShipName.Contains(SearchShipName));
            }
            if (!string.IsNullOrEmpty(SearchShipCity))
            {
                predicate = predicate.And(x => x.ShipCity.Contains(SearchShipCity));
            }

            return predicate;
        }
    }
}