using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace BlazingOrchard.DisplayManagement.Blazor
{
    public class TestHost
    {
        private readonly Lazy<TestRenderer> _renderer;
        
        public TestHost(IServiceProvider serviceProvider)
        {
            Services = serviceProvider;

            _renderer = new Lazy<TestRenderer>(
                () =>
                {
                    var loggerFactory = Services.GetService<ILoggerFactory>() ?? new NullLoggerFactory();
                    return new TestRenderer(Services, loggerFactory);
                });
        }

        public IServiceProvider Services { get; }
        private TestRenderer Renderer => _renderer.Value;

        public RenderedComponent<TComponent> AddComponent<TComponent>() where TComponent : IComponent =>
            AddComponent<TComponent>(ParameterView.Empty);

        public RenderedComponent AddComponent(Type componentType) => AddComponent(componentType, ParameterView.Empty);

        public RenderedComponent<TComponent> AddComponent<TComponent>(IDictionary<string, object> parameters)
            where TComponent : IComponent =>
            AddComponent<TComponent>(ParameterView.FromDictionary(parameters));
        
        public RenderedComponent AddComponent(Type componentType, IDictionary<string, object> parameters) =>
            AddComponent(componentType, ParameterView.FromDictionary(parameters));
        
        public RenderedComponent<TComponent> AddComponent<TComponent>(ParameterView parameters)
            where TComponent : IComponent
        {
            var result = new RenderedComponent<TComponent>(Renderer);
            result.SetParametersAndRender(parameters);
            return result;
        }

        public RenderedComponent AddComponent(Type componentType, ParameterView parameters)
        {
            var result = new RenderedComponent(componentType, Renderer);
            result.SetParametersAndRender(parameters);
            return result;
        }
    }
}