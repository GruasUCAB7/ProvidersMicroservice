using ProvidersMS.Core.Application.IdGenerator;

namespace ProvidersMS.Core.Infrastructure.UUID
{
    public class GuidGenerator : IdGenerator<string>
    {
        public string Generate()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
