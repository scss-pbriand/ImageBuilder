# ImageBuilder

ImageBuilder is a personal Blazor Server application targeting .NET 9 for managing image collections.

## Features

- Local registration and external login with Google or Microsoft accounts.
- Admin account seeded at startup for user and content management.
- Users require admin approval before accessing the site.
- Create collections containing categories with probability weights and uploaded images.

## Getting Started

1. Configure authentication in `appsettings.json` with your provider credentials.
2. Ensure the .NET 9 SDK is installed.
3. Run `dotnet run --project src/ImageBuilder.Server` to start the application.

## License

This project uses only free components and libraries and is intended for personal, non-commercial use.
