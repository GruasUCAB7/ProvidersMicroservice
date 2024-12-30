using ProvidersMS.src.Drivers.Application.Repositories;

namespace ProvidersMS.src.Drivers.Application.Commands.DisconnectDriver
{
    public class DisconnectDriverCommandHandler(IDriverRepository driverRepository)
    {
        private readonly IDriverRepository _driverRepository = driverRepository;

        public async Task<bool> Execute()
        {
            await _driverRepository.ValidateUpdateTimeDriver();
            return true;
        }
    }
}
