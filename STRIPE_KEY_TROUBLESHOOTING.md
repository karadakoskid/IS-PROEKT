# Stripe API Key Troubleshooting

## Current Issue
Error: `Invalid API Key provided: sk_test_****************HERE`

## Possible Causes:
1. **Truncated Key**: The secret key might be cut off
2. **Copy/Paste Error**: Hidden characters or incomplete copy
3. **Configuration Issue**: Key not being read properly

## Steps to Fix:

### 1. Verify Your Secret Key
- Go to: https://dashboard.stripe.com/test/apikeys
- Copy the **complete** secret key (starts with `sk_test_`)
- Should be approximately 100+ characters long

### 2. Replace the Key Carefully
Your Stripe test secret key should:
- Start with: `sk_test_`
- Be approximately 100+ characters long
- Contain only alphanumeric characters
- Come from your Stripe Dashboard → API Keys section

### 3. Common Key Issues:
- ❌ Key ends with "HERE" (from template)
- ❌ Key is too short (less than 95 characters)
- ❌ Extra spaces or line breaks
- ❌ Using live key instead of test key

### 4. Test After Update:
1. Save the file
2. Restart the application
3. Try checkout again

## Quick Validation:
Your secret key should:
- ✅ Start with: `sk_test_`
- ✅ Be ~100 characters long
- ✅ End with random letters/numbers (not "HERE")
- ✅ Match your publishable key account ID
