using AutoMapper;
using backend.Dao.Specification;
using backend.Dao.Specification.Order1;
using backend.Dao.Specification.OrderDetail1;
using backend.Dao.Specification.OrderDetailSpec;
using backend.Dtos.OrderDetaiDtos;
using backend.Dtos.OrderDetailDtos;
using backend.Entity;
using backend.Exceptions;
using backend.Helper;
using webapi.Dao.UnitofWork;

namespace backend.BussinessLogic
{
    public class OrderDetailBusinessLogic
    {
        public IUnitofWork unitofWork;
        public IMapper mapper;

        public OrderDetailBusinessLogic(IUnitofWork _unitofWork, IMapper mapper)
        {
            unitofWork = _unitofWork;
            this.mapper = mapper;
        }

        //list orderDetail
        public async Task<IReadOnlyList<OrderDetail>> SelectAllOrderDetail()
        {
            var data = await unitofWork.Repository<OrderDetail>().GetAllAsync();
            return data;
        }
        public async Task<IEnumerable<OrderDetail>> SelectAllOrderDetail2(int id)
        {
            var spec = new OrderDetailSpec(id);
            var data = await unitofWork.Repository<OrderDetail>().GetAllWithAsync(spec);
            return data;
        }

        //create orderDetail
        public async Task Create(OrderDetail orderDetail)
        {
            if (orderDetail is null)
            {
                throw new NotFoundExceptions("Cattegory not found");
            }
            await unitofWork.Repository<OrderDetail>().AddAsync(orderDetail);
            var check = await unitofWork.Complete();
            if (check < 1)
            {
                throw new BadRequestExceptions("chua dc thuc thi");
            }
        }

        //update orderDetail
        public async Task Update(OrderDetail orderDetail, int id)
        {
            if (orderDetail is null)
            {
                throw new NotFoundExceptions("not found");
            }

            var existingOrderDetail = await unitofWork.Repository<OrderDetail>().GetByIdAsync(id);

            if (existingOrderDetail is null)
            {
                throw new NotFoundExceptions("not found");
            }
            existingOrderDetail.UpdateDate = orderDetail.UpdateDate;
            existingOrderDetail.CreateDate = orderDetail.CreateDate;
            existingOrderDetail.UpdateBy = orderDetail.UpdateBy;
            existingOrderDetail.CreateBy = orderDetail.CreateBy;
            existingOrderDetail.IsActive = orderDetail.IsActive;            
            existingOrderDetail.OrderID = orderDetail.OrderID;
            existingOrderDetail.Quantity = orderDetail.Quantity;
            existingOrderDetail.Price = orderDetail.Price;
            existingOrderDetail.Rating = orderDetail.Rating;          
            existingOrderDetail.UserID = orderDetail.UserID;
            existingOrderDetail.Payment_ID = orderDetail.Payment_ID;
            existingOrderDetail.Type_Payment = orderDetail.Type_Payment;
            existingOrderDetail.Tour_Detail_ID = orderDetail.Tour_Detail_ID;

            await unitofWork.Repository<OrderDetail>().Update(existingOrderDetail);
            var check = await unitofWork.Complete();
            if (check < 1)
            {
                throw new BadRequestExceptions("chua dc thuc thi");
            }
        }

        //delete orderDetail
        public async Task Delete(int id)
        {

            var existingOrderDetail = await unitofWork.Repository<OrderDetail>().GetByIdAsync(id);
            if (existingOrderDetail == null)
            {
                throw new NotFoundExceptions("not found");
            }
            await unitofWork.Repository<OrderDetail>().Delete(existingOrderDetail);
            var check = await unitofWork.Complete();
            if (check < 1)
            {
                throw new BadRequestExceptions("chua dc thuc thi");
            }
        }
        public async Task<OrderDetail> GetOrderDetailById(int Id)
        {
            var result = await unitofWork.Repository<OrderDetail>().GetByIdAsync(Id);
            if(result == null)
            {
                return null;
            }
            return result;
        }
        public async Task<Pagination<OrderDetailDto>> SelectAllOrderDetailPagination(SpecParams specParams)
        {

            var spec = new SearchOrderDetailSpec(specParams);
            var orderDetail = await unitofWork.Repository<OrderDetail>().GetAllWithAsync(spec);

            var data = mapper.Map<IReadOnlyList<OrderDetail>, IReadOnlyList<OrderDetailDto>>(orderDetail);
            var orderDetailPage = data.Skip((specParams.PageIndex - 1) * specParams.PageSize).Take(specParams.PageSize).ToList();

            var countSpec = new SearchOrderDetailSpec(specParams);
            var count = await unitofWork.Repository<OrderDetail>().GetCountWithSpecAsync(countSpec);

            var totalPageIndex = count % specParams.PageSize == 0 ? count / specParams.PageSize : (count / specParams.PageSize) + 1;

            var pagination = new Pagination<OrderDetailDto>(specParams.PageIndex, specParams.PageSize, orderDetailPage, count, totalPageIndex);

            return pagination;
        }
    }
}
