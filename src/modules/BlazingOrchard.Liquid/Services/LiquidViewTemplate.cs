using BlazingOrchard.DisplayManagement.Shapes;
using BlazingOrchard.DisplayManagement.Zones;
using BlazingOrchard.Liquid.Accessors;
using BlazingOrchard.Liquid.Filters;
using BlazingOrchard.Liquid.Tags;
using Fluid;
using Fluid.Values;

namespace BlazingOrchard.Liquid.Services
{
    public class LiquidViewTemplate : BaseFluidTemplate<LiquidViewTemplate>
    {
        static LiquidViewTemplate()
        {
            FluidValue.SetTypeMapping<Shape>(o => new ObjectValue(o));
            FluidValue.SetTypeMapping<ZoneHolding>(o => new ObjectValue(o));

            TemplateContext.GlobalMemberAccessStrategy.Register<Shape>("*", new ShapeAccessor());
            TemplateContext.GlobalMemberAccessStrategy.Register<ZoneHolding>("*", new ShapeAccessor());

            Factory.RegisterTag<ClearAlternatesTag>("shape_clear_alternates");
            Factory.RegisterTag<AddAlternatesTag>("shape_add_alternates");
            Factory.RegisterTag<ClearWrappers>("shape_clear_wrappers");
            Factory.RegisterTag<AddWrappersTag>("shape_add_wrappers");
            Factory.RegisterTag<ShapeTypeTag>("shape_type");
            Factory.RegisterTag<ShapeDisplayTypeTag>("shape_display_type");
            Factory.RegisterTag<ShapePositionTag>("shape_position");
            Factory.RegisterTag<ShapeRemoveItemTag>("shape_remove_item");
            Factory.RegisterTag<ShapeAddPropertyTag>("shape_add_properties");
            Factory.RegisterTag<ShapeRemovePropertyTag>("shape_remove_property");
            Factory.RegisterTag<ShapePagerTag>("shape_pager");
            // Factory.RegisterTag<NamedHelperTag>("shape");
            // Factory.RegisterTag<NamedHelperTag>("contentitem");

            //Factory.RegisterBlock<HelperBlock>("block");
            // Factory.RegisterBlock<NamedHelperBlock>("a");
            // Factory.RegisterBlock<NamedHelperBlock>("form");

            // NamedHelperTag.RegisterDefaultArgument("shape", "type");
            // NamedHelperBlock.RegisterDefaultArgument("zone", "name");

            TemplateContext.GlobalFilters.WithLiquidViewFilters();
        }
    }
}
