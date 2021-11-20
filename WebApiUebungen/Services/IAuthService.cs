using System.Threading;
using System.Threading.Tasks;
using WebApiUebungen.Entities;
using WebApiUebungen.Models;

namespace WebApiUebungen.Services
{
    public interface IAuthService
    {
        Task<int> SignUp(Credentials credentials, CancellationToken cancellationToken);

        Task<string> SignIn(Credentials credentials, CancellationToken cancellationToken);
    }
}