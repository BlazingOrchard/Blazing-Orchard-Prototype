using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BlazingOrchard.DisplayManagement.Shapes;
using BlazingOrchard.Liquid.Ast;
using Fluid;
using Fluid.Ast;

namespace BlazingOrchard.Liquid.Tags
{
    public class ShapeRemoveItemTag : ExpressionArgumentsTag
    {
        public override async ValueTask<Completion> WriteToAsync(TextWriter writer, TextEncoder encoder, TemplateContext context, Expression expression, FilterArgument[] args)
        {
            var objectValue = (await expression.EvaluateAsync(context)).ToObjectValue();

            if (objectValue is Shape shape && shape.Items != null)
            {
                var arguments = (FilterArguments)(await new ArgumentsExpression(args).EvaluateAsync(context)).ToObjectValue();
                shape.Remove(arguments["item"].Or(arguments.At(0)).ToStringValue());
            }

            return Completion.Normal;
        }
    }
}
