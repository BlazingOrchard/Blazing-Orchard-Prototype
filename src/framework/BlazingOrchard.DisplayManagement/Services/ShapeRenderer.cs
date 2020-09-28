using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlazingOrchard.DisplayManagement.Blazor;
using BlazingOrchard.DisplayManagement.Models;
using BlazingOrchard.DisplayManagement.Shapes;
using BlazingOrchard.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Logging;

namespace BlazingOrchard.DisplayManagement.Services
{
    public class ShapeRenderer : IShapeRenderer
    {
        private readonly IShapeTableManager _shapeTableManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        public ShapeRenderer(IShapeTableManager shapeTableManager,
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory,
            ILogger<ShapeRenderer> logger)
        {
            _shapeTableManager = shapeTableManager;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task<RenderFragment> RenderShapeAsync(IShape shape, CancellationToken cancellationToken)
        {
            var componentType = await GetComponentTypeAsync(shape);
            return RenderComponent(componentType, shape);
        }
        
        public async Task<string> RenderShapeAsStringAsync(IShape shape, CancellationToken cancellationToken)
        {
            var host = new TestHost(_serviceProvider);
            var attributes = new Dictionary<string, object>{ ["Model"] = shape };
            var componentType = await GetComponentTypeAsync(shape);
            var renderedComponent = host.AddComponent(componentType, attributes);
            return renderedComponent.GetMarkup();
        }

        private async Task<Type> GetComponentTypeAsync(IShape shape)
        {
            var shapeTable = _shapeTableManager.GetShapeTable();
            var shapeMetadata = shape.Metadata;

            var displayContext = new ShapeDisplayContext
            {
                Shape = shape,
                ServiceProvider = _serviceProvider
            };

            if (shapeTable.Descriptors.TryGetValue(shapeMetadata.Type, out var shapeDescriptor))
            {
                await shapeDescriptor.DisplayingAsync.InvokeAsync(
                    (action, displayContext) => action(displayContext),
                    displayContext,
                    _logger);
            }

            // Invoking ShapeMetadata displaying events.
            shapeMetadata.Displaying.Invoke(action => action(displayContext), _logger);

            var shapeTypes = shapeMetadata.Alternates.Reverse().Concat(new[] { shape.Metadata.Type }).ToList();

            var query =
                from shapeType in shapeTypes
                from b in shapeTable.ShapeBindings
                where b.ShapeType == shapeType
                select b;

            var binding = query.FirstOrDefault();

            if (binding == null)
                throw new Exception($"No component binding found for shape {shape.Metadata.Type}.");

            if (shapeDescriptor != null)
            {
                await shapeDescriptor.DisplayedAsync.InvokeAsync(
                    (action, displayContext) => action(displayContext),
                    displayContext,
                    _logger);
            }

            // Invoking ShapeMetadata displayed events.
            shapeMetadata.Displayed.Invoke((action, displayContext) => action(displayContext), displayContext, _logger);

            return binding.ComponentType;
        }

        private RenderFragment RenderComponent(Type componentType, IShape shape) => builder =>
        {
            builder.OpenComponent(0, componentType);
            builder.AddAttribute(1, "Model", shape);
            builder.CloseComponent();
        };
    }
}