using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BlazingOrchard.DisplayManagement.Shapes;
using BlazingOrchard.Liquid.Ast;
using Fluid;
using Fluid.Ast;
using Humanizer;

namespace BlazingOrchard.Liquid.Tags
{
    public class ShapeRemovePropertyTag : ExpressionArgumentsTag
    {
        public override async ValueTask<Completion> WriteToAsync(TextWriter writer, TextEncoder encoder, TemplateContext context, Expression expression, FilterArgument[] args)
        {
            var objectValue = (await expression.EvaluateAsync(context)).ToObjectValue();

            if (objectValue is IShape shape)
            {
                var arguments = (FilterArguments)(await new ArgumentsExpression(args).EvaluateAsync(context)).ToObjectValue();
                var propName = arguments["property"].Or(arguments.At(0)).ToStringValue();
                if (!string.IsNullOrEmpty(propName))
                {
                    shape.Properties.Remove(propName.Pascalize().Underscore());
                }
            }

            return Completion.Normal;
        }
    }
}
