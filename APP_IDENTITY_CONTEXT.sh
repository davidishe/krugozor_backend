# dotnet ef migrations script -c AppDbContext  --project  /Users/akobiyada/Documents/SpecialProjects/30_PROPERTY_CAT/krugozor_backend/Krugozor.Infrastructure.Database/Krugozor.Infrastructure.Database.csproj

cd Krugozor.WebAPI
dotnet ef database drop --force --context IdentityContext
rm -rf /Users/akobiyada/Documents/SpecialProjects/30_PROPERTY_CAT/krugozor_backend/Krugozor.Identity.Database/Migrations
dotnet ef migrations add mew_migration_identity_5 -c IdentityContext --project /Users/akobiyada/Documents/SpecialProjects/30_PROPERTY_CAT/krugozor_backend/Krugozor.Identity.Database/Krugozor.Identity.Database.csproj
dotnet ef database update -c IdentityContext --project /Users/akobiyada/Documents/SpecialProjects/30_PROPERTY_CAT/krugozor_backend/Krugozor.Identity.Database/Krugozor.Identity.Database.csproj
dotnet ef migrations script -c IdentityContext --project /Users/akobiyada/Documents/SpecialProjects/30_PROPERTY_CAT/krugozor_backend/Krugozor.Identity.Database/Krugozor.Identity.Database.csproj

rm -rf /Users/akobiyada/Documents/SpecialProjects/30_PROPERTY_CAT/krugozor_backend/Krugozor.Identity.Database/Migrations