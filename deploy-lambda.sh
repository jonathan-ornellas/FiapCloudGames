#!/bin/bash

set -e

echo "=== FIAP Cloud Games - Lambda Deploy Script ==="

REGION="us-east-1"
ROLE_ARN="arn:aws:iam::048249931892:role/FiapCloudGamesLambdaRole"
RUNTIME="dotnet8"

echo "Building Lambda functions..."
dotnet build FiapCloudGames.Lambda/FiapCloudGames.Lambda.csproj -c Release

echo "Publishing Lambda functions..."
dotnet publish FiapCloudGames.Lambda/FiapCloudGames.Lambda.csproj -c Release -o ./lambda-publish

echo "Creating deployment package..."
cd lambda-publish
zip -r ../lambda-deployment.zip .
cd ..

echo "Deploying NotificationFunction..."
aws lambda update-function-code \
  --function-name FiapCloudGames-NotificationFunction \
  --zip-file fileb://lambda-deployment.zip \
  --region $REGION 2>/dev/null || \
aws lambda create-function \
  --function-name FiapCloudGames-NotificationFunction \
  --runtime $RUNTIME \
  --role $ROLE_ARN \
  --handler FiapCloudGames.Lambda::FiapCloudGames.Lambda.Functions.NotificationFunction::HandlePaymentNotificationAsync \
  --zip-file fileb://lambda-deployment.zip \
  --timeout 60 \
  --memory-size 512 \
  --environment "Variables={ELASTICSEARCH_URL=http://localhost:9200}" \
  --region $REGION

echo "Deploying RecommendationFunction..."
aws lambda update-function-code \
  --function-name FiapCloudGames-RecommendationFunction \
  --zip-file fileb://lambda-deployment.zip \
  --region $REGION 2>/dev/null || \
aws lambda create-function \
  --function-name FiapCloudGames-RecommendationFunction \
  --runtime $RUNTIME \
  --role $ROLE_ARN \
  --handler FiapCloudGames.Lambda::FiapCloudGames.Lambda.Functions.RecommendationFunction::GenerateRecommendationsAsync \
  --zip-file fileb://lambda-deployment.zip \
  --timeout 60 \
  --memory-size 512 \
  --environment "Variables={ELASTICSEARCH_URL=http://localhost:9200}" \
  --region $REGION

echo "Cleaning up..."
rm -rf lambda-publish lambda-deployment.zip

echo "=== Deploy completed successfully! ==="
