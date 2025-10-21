#!/bin/bash
# API Test Script for Wallpaper Website
# This script demonstrates the main API functionality

BASE_URL="https://localhost:7001"  # Update with your actual URL
API_URL="${BASE_URL}/api"

echo "=========================================="
echo "Wallpaper Website API Test Suite"
echo "=========================================="
echo ""

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Test 1: Register a new user
echo -e "${BLUE}Test 1: Register a new user${NC}"
REGISTER_RESPONSE=$(curl -s -X POST "${API_URL}/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "email": "test@example.com",
    "password": "Test123!",
    "displayName": "Test User"
  }')

echo "$REGISTER_RESPONSE" | jq .
TOKEN=$(echo "$REGISTER_RESPONSE" | jq -r '.token')
USER_ID=$(echo "$REGISTER_RESPONSE" | jq -r '.user.id')

if [ "$TOKEN" != "null" ]; then
    echo -e "${GREEN}✓ User registered successfully${NC}"
else
    echo -e "${RED}✗ User registration failed${NC}"
fi
echo ""

# Test 2: Login
echo -e "${BLUE}Test 2: Login with credentials${NC}"
LOGIN_RESPONSE=$(curl -s -X POST "${API_URL}/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "usernameOrEmail": "testuser",
    "password": "Test123!"
  }')

echo "$LOGIN_RESPONSE" | jq .
TOKEN=$(echo "$LOGIN_RESPONSE" | jq -r '.token')

if [ "$TOKEN" != "null" ]; then
    echo -e "${GREEN}✓ Login successful${NC}"
else
    echo -e "${RED}✗ Login failed${NC}"
fi
echo ""

# Test 3: Get current user
echo -e "${BLUE}Test 3: Get current user info${NC}"
curl -s -X GET "${API_URL}/auth/me" \
  -H "Authorization: Bearer ${TOKEN}" | jq .
echo ""

# Test 4: Update profile
echo -e "${BLUE}Test 4: Update user profile${NC}"
curl -s -X PUT "${API_URL}/user/profile" \
  -H "Authorization: Bearer ${TOKEN}" \
  -H "Content-Type: application/json" \
  -d '{
    "displayName": "Updated Test User",
    "bio": "This is my bio"
  }' | jq .
echo ""

# Test 5: Get wallpapers (public endpoint)
echo -e "${BLUE}Test 5: Get all wallpapers${NC}"
curl -s -X GET "${API_URL}/wallpaper?page=1&pageSize=10" | jq .
echo ""

# Test 6: Upload wallpaper (would need an actual image file)
echo -e "${BLUE}Test 6: Upload wallpaper${NC}"
echo "Note: This requires a real image file. Example:"
echo "curl -X POST \"${API_URL}/wallpaper\" \\"
echo "  -H \"Authorization: Bearer \${TOKEN}\" \\"
echo "  -F \"title=Beautiful Sunset\" \\"
echo "  -F \"description=A stunning sunset view\" \\"
echo "  -F \"category=Nature\" \\"
echo "  -F \"tags=sunset,nature,beautiful\" \\"
echo "  -F \"image=@/path/to/image.jpg\""
echo ""

# Test 7: Password reset request
echo -e "${BLUE}Test 7: Request password reset${NC}"
curl -s -X POST "${API_URL}/auth/forgot-password" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com"
  }' | jq .
echo ""

echo "=========================================="
echo "Test Suite Completed"
echo "=========================================="
echo ""
echo "Note: Some tests may fail if the database is not set up or if"
echo "there are existing users with the same credentials."
echo ""
echo "To set up the database, run:"
echo "  cd WallpaperApi"
echo "  dotnet ef database update"
