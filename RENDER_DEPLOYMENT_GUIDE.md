# 🚀 Deploy ASP.NET Core EShop to Render

## Prerequisites
- ✅ GitHub repository (your code is already pushed!)
- ✅ Render account (free tier available)
- ✅ Stripe account with API keys
- ✅ Your application builds successfully

## 📋 Step-by-Step Deployment

### 1. Prepare Your Application

First, let's ensure your `Program.cs` is configured for production:

```csharp
// Add this to handle port binding for Render
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://0.0.0.0:{port}");
```

### 2. Create Render Service

1. **Go to Render Dashboard**: https://dashboard.render.com/
2. **Click "New +"** → **"Web Service"**
3. **Connect GitHub**: Authorize Render to access your repositories
4. **Select Repository**: Choose `karadakoskid/IS-PROEKT`

### 3. Configure Web Service Settings

**Basic Settings:**
- **Name**: `eshop-bookstore` (or your preferred name)
- **Region**: Choose closest to your users
- **Branch**: `master`
- **Root Directory**: Leave empty
- **Runtime**: `Docker` (we'll use your Dockerfile)

**Build & Deploy:**
- **Build Command**: `docker build -t eshop .`
- **Start Command**: `docker run -p $PORT:5000 eshop`

### 4. Set Environment Variables

In Render dashboard, add these environment variables:

```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:$PORT

# Database (if using external DB)
ConnectionStrings__DefaultConnection=YOUR_DATABASE_CONNECTION_STRING

# Stripe Configuration
Stripe__PublishableKey=pk_test_YOUR_PUBLISHABLE_KEY
Stripe__SecretKey=sk_test_YOUR_SECRET_KEY
Stripe__WebhookSecret=whsec_YOUR_WEBHOOK_SECRET

# Email Settings (if using)
MailSettings__SmtpServer=YOUR_SMTP_SERVER
MailSettings__SmtpServerPort=587
MailSettings__SendersName=EShop Application
MailSettings__SendersEmail=YOUR_EMAIL
MailSettings__Password=YOUR_EMAIL_PASSWORD
```

### 5. Update Your Dockerfile (if needed)

Ensure your Dockerfile is optimized for Render:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["EShop.Web/EShop.Web.csproj", "EShop.Web/"]
COPY ["EShop.Service/EShop.Service.csproj", "EShop.Service/"]
COPY ["EShop.Repository/EShop.Repository.csproj", "EShop.Repository/"]
COPY ["EShop.Domain/EShop.Domain.csproj", "EShop.Domain/"]
RUN dotnet restore "EShop.Web/EShop.Web.csproj"
COPY . .
WORKDIR "/src/EShop.Web"
RUN dotnet build "EShop.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EShop.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EShop.Web.dll"]
```

### 6. Database Options

**Option A: SQLite (Simple)**
- Your current setup with SQLite will work
- Database file will be recreated on each deploy

**Option B: PostgreSQL (Recommended for Production)**
- Add PostgreSQL service in Render (free tier available)
- Update connection string in environment variables

### 7. Deploy & Configure Stripe Webhooks

After deployment:

1. **Get your Render URL**: `https://your-app-name.onrender.com`

2. **Configure Stripe Webhooks**:
   - Go to Stripe Dashboard → Webhooks
   - Add endpoint: `https://your-app-name.onrender.com/webhook/stripe`
   - Select events: `checkout.session.completed`, `payment_intent.succeeded`
   - Copy webhook secret to environment variables

### 8. Test Your Deployment

1. **Visit your app**: `https://your-app-name.onrender.com`
2. **Test registration/login**
3. **Test adding books to cart**
4. **Test Stripe checkout** with test card: `4242 4242 4242 4242`

## 🔧 Troubleshooting

### Common Issues:

1. **Port Binding Error**
   ```csharp
   // Add to Program.cs
   var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
   app.Urls.Add($"http://0.0.0.0:{port}");
   ```

2. **Database Migration**
   ```csharp
   // Add to Program.cs (before app.Run())
   using (var scope = app.Services.CreateScope())
   {
       var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
       context.Database.EnsureCreated();
   }
   ```

3. **HTTPS Redirect Issues**
   ```csharp
   // Only use HTTPS redirect in production with proper certificates
   if (app.Environment.IsProduction())
   {
       app.UseHttpsRedirection();
   }
   ```

## 📊 Monitoring

- **Render Logs**: Check deployment logs in Render dashboard
- **Stripe Dashboard**: Monitor test payments
- **Application Insights**: Consider adding for production monitoring

## 💰 Costs

- **Render Free Tier**: 750 hours/month (enough for testing)
- **Render Paid**: $7/month for always-on service
- **PostgreSQL**: Free tier available on Render

Your ASP.NET Core EShop with Stripe integration is ready for deployment! 🎉