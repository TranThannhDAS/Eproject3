using backend.Entity;
using backend.Exceptions;
using webapi.Dao.UnitofWork;

namespace backend.BussinessLogic
{
    public class UserBussinessLogic
    {
        private IUnitofWork unitofWork;
        public UserBussinessLogic(IUnitofWork _unitofWork)
        {
            unitofWork = _unitofWork;
        }
        public async Task<User> GetUserByCondition(int id)
        {
            var check = await unitofWork.Repository<User>().GetByIdAsync(id);
            if(check == null)
            {
                return null;
            }
            return check;
        }
    }
}
