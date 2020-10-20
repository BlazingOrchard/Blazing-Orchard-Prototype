using System.Threading;
using System.Threading.Tasks;
using BlazingOrchard.DisplayManagement.Shapes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BlazingOrchard.DisplayManagement.Services
{
    public interface IShapeRenderer
    {
        Task<RenderFragment> RenderShapeAsync(IShape shape, CancellationToken cancellationToken = default);

        Task<int> RenderShapeAsync(IShape shape,
            int sequence,
            RenderTreeBuilder renderTreeBuilder,
            CancellationToken cancellationToken = default);
    }
}