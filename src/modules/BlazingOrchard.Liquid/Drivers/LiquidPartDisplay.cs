using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BlazingOrchard.Contents.Display.Services;
using BlazingOrchard.DisplayManagement.Services;
using BlazingOrchard.DisplayManagement.Shapes;
using BlazingOrchard.Liquid.Models;

namespace BlazingOrchard.Liquid.Drivers
{
    public class LiquidPartDisplay : ContentPartDisplayDriver<LiquidPart>
    {
        private readonly ILiquidTemplateManager _liquidTemplateManager;
        private readonly HtmlEncoder _htmlEncoder;

        public LiquidPartDisplay(ILiquidTemplateManager liquidTemplateManager, HtmlEncoder htmlEncoder)
        {
            _liquidTemplateManager = liquidTemplateManager;
            _htmlEncoder = htmlEncoder;
        }

        protected override IDisplayResult? BuildDisplay(LiquidPart contentPart)
        {
            return Shape(
                    nameof(LiquidPart),
                    async context =>
                    {
                        var shape = await context.New.LiquidPart();
                        shape.ContentItem = contentPart.ContentItem;
                        shape.Liquid = contentPart.Liquid!;
                        shape.LiquidBodyPart = contentPart;
                        shape.Html = (await ToHtmlAsync(contentPart, shape)) ?? "";
                        return shape;
                    })
                .DefaultLocation("5");
        }

        private async Task<string?> ToHtmlAsync(LiquidPart liquidPart, IShape shape) =>
            await _liquidTemplateManager.RenderAsync(
                liquidPart.Liquid,
                _htmlEncoder,
                shape,
                scope => scope.SetValue("ContentItem", liquidPart.ContentItem));
    }
}