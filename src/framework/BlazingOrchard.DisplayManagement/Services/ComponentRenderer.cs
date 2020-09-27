using System;
using BlazingOrchard.DisplayManagement.Blazor;
using Microsoft.AspNetCore.Components;

namespace BlazingOrchard.DisplayManagement.Services
{
    public class ComponentRenderer : IComponentRenderer
    {
        private readonly IServiceProvider _serviceProvider;

        public ComponentRenderer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public MarkupString RenderComponent(Type componentType)
        {
            var host = new TestHost(_serviceProvider);
            var renderedComponent = host.AddComponent(componentType);
            return (MarkupString)renderedComponent.GetMarkup();
        }
    }
}