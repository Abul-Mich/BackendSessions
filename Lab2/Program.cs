using DbApi.CodeFirst;
using DbApi.Mappings;
using DbApi.Models;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;

var builder = WebApplication.CreateBuilder(args);

// OData model - using DB First models to recreate Step 1 endpoints
var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<Student>("Students");
modelBuilder.EntitySet<Course>("Courses");
modelBuilder.EntitySet<Teacher>("Teachers");
modelBuilder.EntitySet<Enrollment>("Enrollments");

builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System
            .Text
            .Json
            .Serialization
            .ReferenceHandler
            .IgnoreCycles;
    })
    .AddOData(options =>
        options
            .Select()
            .Filter()
            .OrderBy()
            .Expand()
            .Count()
            .SetMaxTop(null)
            .AddRouteComponents("odata", modelBuilder.GetEdmModel())
    );

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<UniDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddDbContext<UniversityContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CodeFirstConnection"))
);

builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();
