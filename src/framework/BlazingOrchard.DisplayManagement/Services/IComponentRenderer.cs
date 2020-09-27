using System;
using Microsoft.AspNetCore.Components;

namespace BlazingOrchard.DisplayManagement.Services
{
    public interface IComponentRenderer
    {
        MarkupString RenderComponent(Type componentType);
    }
}