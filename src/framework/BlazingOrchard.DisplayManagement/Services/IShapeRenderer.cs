using System.Threading;
using System.Threading.Tasks;
using BlazingOrchard.DisplayManagement.Shapes;
using Microsoft.AspNetCore.Components;

namespace BlazingOrchard.DisplayManagement.Services
{
    public interface IShapeRenderer
    {
        Task<RenderFragment> RenderShapeAsync(IShape shape, CancellationToken cancellationToken = default);
        Task<string> RenderShapeAsStringAsync(IShape shape, CancellationToken cancellationToken = default);
    }
}