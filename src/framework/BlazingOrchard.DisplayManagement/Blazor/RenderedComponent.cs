using System;
using System.Collections.Generic;
using System.Linq;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Components;

namespace BlazingOrchard.DisplayManagement.Blazor
{
    public class RenderedComponent
    {
        private readonly Type _componentType;
        private readonly TestRenderer _renderer;
        private readonly ContainerComponent _containerTestRootComponent;
        private int _testComponentId;
        private IComponent? _testComponentInstance;

        internal RenderedComponent(Type componentType, TestRenderer renderer)
        {
            _componentType = componentType;
            _renderer = renderer;
            _containerTestRootComponent = new ContainerComponent(_renderer);
        }

        public IComponent? Instance => _testComponentInstance;

        public string GetMarkup()
        {
            return Htmlizer.GetHtml(_renderer, _testComponentId);
        }

        internal void SetParametersAndRender(ParameterView parameters)
        {
            _containerTestRootComponent.RenderComponent(
                _componentType, parameters);
            var foundTestComponent = _containerTestRootComponent.FindComponentUnderTest();
            _testComponentId = foundTestComponent.Item1;
            _testComponentInstance = (IComponent)foundTestComponent.Item2;
        }

        public HtmlNode? Find(string selector) => FindAll(selector).FirstOrDefault();

        public ICollection<HtmlNode> FindAll(string selector)
        {
            // Rather than using HTML strings, it would be faster and more powerful
            // to implement Fizzler's APIs for walking directly over the rendered
            // frames, since Fizzler's core isn't tied to HTML (or HtmlAgilityPack).
            // The most awkward part of this will be handling Markup frames, since
            // they are HTML strings so would need to be parsed, or perhaps you can
            // pass through those calls into Fizzler.Systems.HtmlAgilityPack.

            var markup = GetMarkup();
            var html = new TestHtmlDocument(_renderer);

            html.LoadHtml(markup);
            return html.DocumentNode.QuerySelectorAll(selector).ToList();
        }
    }
    
    public class RenderedComponent<TComponent> : RenderedComponent where TComponent : IComponent
    {
        internal RenderedComponent(TestRenderer renderer) : base(typeof(TComponent), renderer)
        {
        }

        public TComponent TypedInstance => (TComponent)Instance;
    }
}
