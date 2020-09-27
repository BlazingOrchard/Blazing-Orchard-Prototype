using HtmlAgilityPack;

namespace BlazingOrchard.DisplayManagement.Blazor
{
    internal class TestHtmlDocument : HtmlDocument
    {
        public TestHtmlDocument(TestRenderer renderer)
        {
            Renderer = renderer;
        }

        public TestRenderer Renderer { get; }
    }
}
