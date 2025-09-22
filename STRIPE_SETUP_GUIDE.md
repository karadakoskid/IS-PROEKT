# Stripe Payment Integration - Setup and Testing Guide

## Overview
Your EBook store now has Stripe payment integration for secure online payments. This guide will help you set up and test the payment functionality.

## 🚀 How to Activate Stripe

### Step 1: Create a Stripe Account
1. Go to [https://stripe.com](https://stripe.com)
2. Click "Start now" and create a free account
3. Complete the account verification process

### Step 2: Get Your API Keys
1. Log into your Stripe Dashboard
2. Navigate to **Developers > API Keys**
3. You'll see two types of keys:
   - **Publishable Key** (starts with `pk_test_`)
   - **Secret Key** (starts with `sk_test_`)

### Step 3: Configure Your Application
1. Open `appsettings.json` in your EShop.Web project
2. Replace the placeholder values with your actual Stripe keys:

```json
{
  "Stripe": {
    "PublishableKey": "pk_test_YOUR_ACTUAL_PUBLISHABLE_KEY_HERE",
    "SecretKey": "sk_test_YOUR_ACTUAL_SECRET_KEY_HERE",
    "WebhookSecret": "whsec_YOUR_WEBHOOK_SECRET_HERE"
  }
}
```

### Step 4: Set Up Webhooks (Optional for basic testing)
1. In Stripe Dashboard, go to **Developers > Webhooks**
2. Click **Add endpoint**
3. Use your application URL + `/Payment/Webhook` (e.g., `https://yourdomain.com/Payment/Webhook`)
4. Select events: `checkout.session.completed` and `payment_intent.succeeded`
5. Copy the **Webhook Secret** and add it to your `appsettings.json`

## 🧪 How to Test Stripe Integration

### Test Credit Card Numbers
Stripe provides test card numbers that simulate different scenarios:

#### ✅ Successful Payments:
- **Visa**: `4242424242424242`
- **Visa (debit)**: `4000056655665556`
- **Mastercard**: `5555555555554444`
- **American Express**: `378282246310005`

#### ❌ Failed Payments:
- **Declined**: `4000000000000002`
- **Insufficient funds**: `4000000000009995`
- **Expired card**: `4000000000000069`

#### Test Details for All Cards:
- **Expiry Date**: Any future date (e.g., `12/34`)
- **CVC**: Any 3-digit number (e.g., `123`)
- **ZIP Code**: Any valid ZIP (e.g., `12345`)

### Testing Steps:

#### 1. **Test Without Authentication**
```bash
# Start your application
cd /Users/damjankaradakoski/IS-PROEKT/EShop.Web
dotnet run --urls "http://localhost:5000"
```

- Visit `http://localhost:5000`
- Try to add books to cart → Should see "Login to Add to Cart"
- Shopping cart button should be hidden from navbar

#### 2. **Test With Authentication**
- Register a new account or login
- Add books to your cart
- Navigate to shopping cart
- Click "Proceed to Checkout"
- You should see the checkout page with order summary

#### 3. **Test Payment Flow**
- On checkout page, click "Proceed to Payment"
- You'll be redirected to Stripe Checkout
- Use test card number: `4242424242424242`
- Complete the payment form
- You should be redirected back to success page

#### 4. **Test Failed Payments**
- Repeat the process with failed card: `4000000000000002`
- Should see appropriate error messages

## 🛠️ Current Features

### ✅ Implemented:
- **Secure Checkout**: Stripe Checkout integration
- **Authentication Required**: Only logged-in users can purchase
- **Order Processing**: Cart items moved to orders after payment
- **Success/Failure Handling**: Appropriate user feedback
- **Responsive Design**: Mobile-friendly checkout experience
- **Tax Calculation**: 10% tax applied to orders
- **Free Shipping**: No shipping costs

### 🎯 Payment Flow:
1. User adds books to cart (authentication required)
2. User navigates to checkout page
3. User reviews order and clicks "Proceed to Payment"
4. User is redirected to Stripe Checkout
5. User enters payment details
6. Upon success, user is redirected back to success page
7. Order is created and cart is cleared

## 💰 Pricing and Currency
- All prices are displayed in USD ($)
- Test mode allows unlimited transactions
- Live mode requires Stripe fees (2.9% + 30¢ per transaction)

## 🔧 Troubleshooting

### Common Issues:

#### 1. **Build Errors**
```bash
# If you see Stripe-related build errors, restore packages:
dotnet restore
dotnet build
```

#### 2. **Invalid API Keys**
- Ensure your keys are correctly copied from Stripe Dashboard
- Test keys start with `pk_test_` and `sk_test_`
- Don't mix test and live keys

#### 3. **Webhook Issues**
- Webhooks are optional for basic testing
- Only needed for production deployments
- Ensure webhook URL is publicly accessible

#### 4. **Payment Not Processing**
- Check browser console for JavaScript errors
- Verify internet connection
- Ensure using valid test card numbers

## 🚀 Going Live

### Before Production:
1. **Get Live API Keys**: Replace test keys with live keys from Stripe
2. **Set Up Live Webhooks**: Configure webhooks for production URL
3. **SSL Certificate**: Ensure your domain has valid SSL
4. **PCI Compliance**: Stripe handles this, but review their guidelines

### Security Notes:
- Never expose secret keys in client-side code
- All payments are processed securely through Stripe
- No card details are stored on your servers
- PCI compliance is handled by Stripe

## 📞 Support
- **Stripe Documentation**: [https://stripe.com/docs](https://stripe.com/docs)
- **Stripe Test Cards**: [https://stripe.com/docs/testing](https://stripe.com/docs/testing)
- **Stripe Dashboard**: [https://dashboard.stripe.com](https://dashboard.stripe.com)

---

**🎉 Your EBook store is now ready for secure online payments!**

Start testing with the provided test card numbers and when ready, simply replace with live API keys for production use.
