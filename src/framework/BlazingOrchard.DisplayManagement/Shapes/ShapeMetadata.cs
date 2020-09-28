using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BlazingOrchard.DisplayManagement.Models;

namespace BlazingOrchard.DisplayManagement.Shapes
{
    public class ShapeMetadata
    {
        public string Type { get; set; } = default!;
        public string? DisplayType { get; set; }
        public string? Position { get; set; }
        public string? Name { get; set; }
        public string? Differentiator { get; set; }
        public string? PlacementSource { get; set; }
        public AlternatesCollection Alternates { get; set; } = new AlternatesCollection();
        public AlternatesCollection Wrappers { get; set; } = new AlternatesCollection();

        /// <summary>
        /// Event used for a specific shape instance.
        /// </summary>
        [JsonIgnore]
        public IEnumerable<Action<ShapeDisplayContext>> Displaying { get; private set; } =
            Enumerable.Empty<Action<ShapeDisplayContext>>();

        /// <summary>
        /// Event used for a specific shape instance.
        /// </summary>
        [JsonIgnore]
        public IEnumerable<Func<IShape, Task>> ProcessingAsync { get; private set; } =
            Enumerable.Empty<Func<IShape, Task>>();

        /// <summary>
        /// Event used for a specific shape instance.
        /// </summary>
        [JsonIgnore]
        public IEnumerable<Action<ShapeDisplayContext>> Displayed { get; private set; } =
            Enumerable.Empty<Action<ShapeDisplayContext>>();

        [JsonIgnore] public IList<string> BindingSources { get; set; } = new List<string>();

        public void OnDisplaying(Action<ShapeDisplayContext> context) =>
            Displaying = Displaying.Concat(new[] { context });

        public void OnProcessing(Func<IShape, Task> context) =>
            ProcessingAsync = ProcessingAsync.Concat(new[] { context });

        public void OnDisplayed(Action<ShapeDisplayContext> context) => Displayed = Displayed.Concat(new[] { context });
    }
}