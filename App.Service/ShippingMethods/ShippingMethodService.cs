using System.Collections.Generic;
using App.Core.Utils;
using App.Domain.Shippings;
using App.Infra.Data.Common;
using App.Infra.Data.Repository.ShippingMethods;
using App.Infra.Data.UOW.Interfaces;

namespace App.Service.ShippingMethodes
{
    public class ShippingMethodService : BaseService<ShippingMethod>, IShippingMethodService
	{
		private readonly IShippingMethodRepository _shippingMethodRepository;

	    public ShippingMethodService(IUnitOfWork unitOfWork, IShippingMethodRepository shippingMethodRepository) : base(unitOfWork, shippingMethodRepository)
		{
		    _shippingMethodRepository = shippingMethodRepository;
		}

		public ShippingMethod GetById(int id)
		{
			return _shippingMethodRepository.GetById(id);
		}

		public IEnumerable<ShippingMethod> PagedList(SortingPagingBuilder sortbuBuilder, Paging page)
		{
			return _shippingMethodRepository.PagedSearchList(sortbuBuilder, page);
		}
	}
}