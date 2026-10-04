using MyApp.Api;
using HotChocolate.Types;

public class CategoryObjectType : ObjectType<Category>
{
    protected override void Configure(IObjectTypeDescriptor<Category> descriptor)
    {
        descriptor.Field(c => c.Id).Type<NonNullType<IdType>>();
        descriptor.Field(c => c.Name).Type<NonNullType<StringType>>();
        // add other fields as needed
    }
}